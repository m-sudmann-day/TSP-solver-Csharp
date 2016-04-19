/*
 * Class: Edge
 * Matthew Sudmann-Day
 * Barcelona GSE Data Science
 *
 * Represents the edge between two vertices, regardless of whether it is used in a solution.
 * Contains limited logic as the process is driven by the Vertex class.
 */

using System;

namespace SMO_PS2
{
    internal class Edge
    {
        //Instance variables that do not change after creation.
        public Vertex Vertex1;
        public Vertex Vertex2;
        public float Length;
        public string ExportFriendlyPath; //A string that can be written to CSV if this edge is part of a winning path.

        //Instance variables that change during processing.
        public bool IsIncluded; //Whether or not this edge is included in the current solution attempt.

        //Given one of the two vertices of this edge, return the other one.  Helps simplify the caller's code.
        public Vertex Other(Vertex v)
        {
            return (v == Vertex1) ? Vertex2 : Vertex1;
        }

        public override string ToString()
        {
            return string.Format("Edge V{0} V{1} {2} Incl.{3}", Vertex1.ID, Vertex2.ID, Length, IsIncluded);
        }
    }
}
