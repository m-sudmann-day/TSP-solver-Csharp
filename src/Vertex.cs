/*
 * Class: Edge
 * Matthew Sudmann-Day
 * Barcelona GSE Data Science
 *
 * Represents the a vertex, or a city in the Traveling Salesman Problem.
 * Encapsulates most of the logic required to solve the problem.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMO_PS2
{
    internal class Vertex
    {
        //Instance variables that do not change after creation.
        public int ID;
        public float X;
        public float Y;

        //TempEdges is used while setting up the problem but is disposed of before processing begins.
        public List<Edge> TempEdges = new List<Edge>();

        //Instance variables that change during processing.
        public Edge[] Edges; //All edges that touch this vertex.  Connects it to all other vertices.
        public int IncludedEdges = 0; //The number of edges (0, 1, or 2) that are currently included in a potential solution.
        public bool IsVisited = false; //Whether or not this node has been visited (and is therefore included) in the current solution.

        //At the end of the processing, we just need to connect the first and last vertices.
        //This method is intended to be called on the first vertex only.  It will return that missing edge
        //which the caller will then add to the solution.
        public Edge GetMissingEdge()
        {
            foreach (Edge edge in Edges)
            {
                if (!edge.IsIncluded)
                {
                    if (edge.Other(this).IncludedEdges == 1)
                    {
                        return edge;
                    }
                }
            }

            return null;
        }

        //While processing, when a complete path is found that touches all cities once, this method is called.
        //If it's the best solution so far, we update our account of what the best path is.
        public void ProposeSolution(float totalLength)
        {
            //Get the missing edge to connect the first and last nodes.
            Edge finalEdge = TspSolver.FirstVertex.GetMissingEdge();
            if (finalEdge != null)
            {
                //Update the total path length accordingly.
                totalLength += finalEdge.Length;

                //If it's a better solution than our best so far...
                if (totalLength < TspSolver.MinPath)
                {
                    finalEdge.IsIncluded = true;
                    TspSolver.MinPath = totalLength;
                    TspSolver.MinPathText = new StringBuilder();

                    //Build a text representation of the path that is ready for export to CSV.
                    foreach (Edge e in TspSolver.Edges)
                    {
                        if (e.IsIncluded)
                        {
                            TspSolver.MinPathText.AppendLine(e.ExportFriendlyPath);
                        }
                    }

                    finalEdge.IsIncluded = false;

                    //Write to file.
                    TspSolver.WriteSolutionToFile();
                }
            }
        }

        //The Process function is the engine that drives this recursive algorithm.
        //The totalLength parameter is the length of the current path that is being built and is
        //hopefully added to on a given iteration.
        //The return value is true if a solution is found, false otherwise.
        public bool Process(float totalLength)
        {
            /*************************** OPTIMIZATION 1 ***************************
             * If our current path is already at least as long as the best solution
             * we have found so far, there is no point in continuing so we quit.
             **********************************************************************/
            if (totalLength >= TspSolver.MinPath)
            {
                return false;
            }

            //If we've built up enough edges to complete a solution, call ProposeSolution to
            //take it from here.
            if (TspSolver.NumActiveEdges == TspSolver.MaxActiveEdges - 1)
            {
                ProposeSolution(totalLength);

                return true;
            }

            /*******************************************************************************/
            /*************************** THE HEART OF IT - BEGIN ***************************/

            //Flag this vertex as visited so that other vertices further down the call stack know
            //not to "travel" to it.
            IsVisited = true;

            bool haveASolution = false;

            for (int edgeIndex = 0; edgeIndex < Edges.Count(); edgeIndex++)
            {
                /*************************** OPTIMIZATION 3 ***************************
                 * Noting that the edges collection that belongs to every vertex is sorted
                 * from shortest to longest, we choose to examine only the shortest few edges,
                 * as specified by the 'breadth limit'.  If no breadth limit is desired, the
                 * caller should set a limit higher than the total number of nodes.
                 * After exceeding this limit, we will take whatever solution we've got as
                 * the best we can do from this point.  However, if we do not have a solution,
                 * we will keep looping and looking at all the edges if we have to.
                 **********************************************************************/
                if (haveASolution && edgeIndex >= TspSolver.BreadthLimit)
                {
                    break;
                }

                //Get the current edge we're working with.
                Edge edge = Edges[edgeIndex];

                //If the edge has not yet been used...
                if (!edge.IsIncluded)
                {
                    //And if the "other" vertex on this edge has not been visited...
                    Vertex other = (this == edge.Vertex1) ? edge.Vertex2 : edge.Vertex1;
                    if (!other.IsVisited)
                    {
                        //Include the edge and update all our counters.
                        edge.IsIncluded = true;
                        edge.Vertex1.IncludedEdges++;
                        edge.Vertex2.IncludedEdges++;
                        TspSolver.NumActiveEdges++;

                        /******************************* RECURSE ********************************/
                        haveASolution = other.Process(totalLength + edge.Length) || haveASolution;
                        /************************************************************************/

                        //Now exclude the edge once again and put all of our counters back the way they were.
                        edge.IsIncluded = false;
                        edge.Vertex1.IncludedEdges--;
                        edge.Vertex2.IncludedEdges--;
                        TspSolver.NumActiveEdges--;
                    }
                }
            }

            //Flag this vertex as no longer visited.
            IsVisited = false;

            return haveASolution;
            /*************************** THE HEART OF IT - END ***************************/
            /*******************************************************************************/
        }

        public override string ToString()
        {
            return string.Format("Vertex {0} ({1},{2}) Included Edges: {3}", ID, X, Y, Edges.Count(e => e.IsIncluded));
        }
    }
}
