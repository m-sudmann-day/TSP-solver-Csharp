/*
 * Class: Program
 * Matthew Sudmann-Day
 * Barcelona GSE Data Science
 *
 * The calling application, just a console application that uses TspSolver.
 * 
 */
 
using System;

namespace SMO_PS2
{
    class Program
    {
        static void Main(string[] args)
        {
            //TspSolver solver = new TspSolver(@"../../../fifteen.txt", 1000, "fifteen");
            //TspSolver solver = new TspSolver(@"../../../fifteen.txt", 5, "fifteen");
            //TspSolver solver = new TspSolver(@"../../../wi29.tsp.txt", 5, "wsahara");
            //TspSolver solver = new TspSolver(@"../../../qa194.tsp.txt", 2, "qatar");
            TspSolver solver = new TspSolver(@"../../../uy734.tsp.txt", 4, "uruguay");
            //TspSolver solver = new TspSolver(@"../../../nu3496.tsp.txt", 3, "oman");
            //TspSolver solver = new TspSolver(@"../../../ja9847.tsp.txt", 3, "japan");

            var solution = solver.Solve();

            TspSolver.WriteSolutionToFile();

            Console.WriteLine("BEST PATH");
            Console.WriteLine(TspSolver.MinPathText);
            Console.ReadLine();
        }
    }
}