/*
 * Class: TspSolver
 * Matthew Sudmann-Day
 * Barcelona GSE Data Science
 *
 * Oversees the mechanical aspects of setting up the TSP problem, creating
 * the classes, starting them running, and wrapping up afterwards.
 *
 * This is admittedly a poor software design for purposes of a bried demonstration.
 * The static members of this class, and others in this solution, should not be
 * static and should have less visibility.  For performance purposes, this allows
 * us to put less on the stack and therefore have improved performance.  Obviously,
 * we can only have one instance of this class at a time.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace SMO_PS2
{
    internal class TspSolver
    {
        //All vertices, all edges, and the first vertex, in the current problem.
        public static List<Vertex> Vertices;
        public static List<Edge> Edges;
        public static Vertex FirstVertex;

        //The number of active edges in real time, and the unchanging number of active
        //edges required to have a complete path.
        public static int NumActiveEdges = 0; 
        public static int MaxActiveEdges;

        //The breadth limit is the width of our tree at any given node, unless no solution
        //is found, in which case greater width is allowed.
        public static int BreadthLimit;

        //The length of the best complete path found so far.
        public static float MinPath = float.MaxValue;

        //Text that describes the best complete path found so far, for purposes of writing to a CSV.
        public static StringBuilder MinPathText = null;

        //A time-tracking variable.
        public static DateTime StartTime = DateTime.MinValue;

        //A string used to help the caller track a particular execution.
        public static string Key;

        //Instantiates this class, creates all vertices and edges.
        public TspSolver(string dataFile, int breadthLimit, string key)
        {
            BreadthLimit = breadthLimit;
            Key = key;

            _createVertices(dataFile);
            _createEdges();
        }

        //Run the process.
        public string Solve()
        {
            //Store the start time for logging purposes.
            StartTime = DateTime.Now;

            //Initiate the process by telling the first vertex to begin processing.
            //At this point, we have a zero total path length.
            FirstVertex.Process(0);

            //Return the solution which contains a CSV-friendly list of edges.
            return MinPathText.ToString();
        }

        //Create all vertices by reading from a .tsp.txt file downloaded from the
        //'Solving TSP' web pages at http://www.math.uwaterloo.ca/tsp/world/countries.html.
        public static void _createVertices(string dataFile)
        {
            Vertices = new List<Vertex>();

            FileInfo fi = new FileInfo(dataFile);
            StreamReader sr = fi.OpenText();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.Length > 0 && line[0] >= '0' && line[0] <= '9')
                {
                    string[] parts = line.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 3)
                    {
                        int n = int.Parse(parts[0]);
                        float x = float.Parse(parts[1]);
                        float y = float.Parse(parts[2]);

                        Vertex v = new Vertex() { ID = n, X = x, Y = y };
                        Vertices.Add(v);
                    }
                }
            }

            FirstVertex = Vertices.First();
        }

        //Create all edges based on the vertices already created.
        public static void _createEdges()
        {
            //Initialize some static variables.
            Edges = new List<Edge>();
            NumActiveEdges = 0;
            MaxActiveEdges = Vertices.Count();

            //Loop through all PAIRS of vertices.
            for (int i = 0; i < Vertices.Count(); i++)
            {
                Vertex vi = Vertices[i];
                for (int j = 0; j < i; j++)
                {
                    Vertex vj = Vertices[j];

                    //Calculate the length.
                    float x = vi.X - vj.X;
                    float y = vi.Y - vj.Y;
                    float d = (float)Math.Sqrt(x * x + y * y);

                    //Create the edge and set the basic properties.
                    Edge edge = new Edge() { Vertex1 = vj, Vertex2 = vi, Length = d };
                    edge.ExportFriendlyPath = string.Format("{0},{1},{2},{3}", vi.X, vi.Y, vj.X, vj.Y);

                    //Add the new edge to both vertices and to the collection of all edges.
                    vi.TempEdges.Add(edge);
                    vj.TempEdges.Add(edge);
                    Edges.Add(edge);
                }
            }

            /*************************** OPTIMIZATION 2 ***************************
             * All edges are pre-sorted within their two vertices' collections in order of shortest to longest
             * length.  This enables the system to produce a short path quickly at the beginning of its
             * processing thereby allowing it to truncate more of the process.  Empirically, this was
             * confirmed by excluding this sort and also by reversing it.  Ascending order appeared to
             * be best.
             **********************************************************************/
            foreach (Vertex v in Vertices)
            {
                v.Edges = v.TempEdges.OrderBy(e => e.Length).ToArray();
                v.TempEdges = null;
            }
        }

        //Write the solution to a file.  The filename consists of useful fields for interpretation.
        //The content is the comma-delimited coordinates of each included edge.
        public static void WriteSolutionToFile()
        {
            TimeSpan timeSpan = DateTime.Now - StartTime;

            Console.WriteLine(MinPath);

            FileInfo fi = new FileInfo(string.Format("../../../Output/{0}_{1}_{2}_{3}.csv",
                                                  Key, BreadthLimit,
                                                  Math.Round(timeSpan.TotalSeconds, 2), MinPath));

            using (StreamWriter sw = fi.CreateText())
            {
                sw.WriteLine("X1,Y1,X2,Y2");
                sw.Write(MinPathText.ToString());
                sw.Close();
            }
        }
    }
}
