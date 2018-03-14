using System;
using System.Collections.Generic;
using System.Linq;
using GRaff;
using static GRaff.GMath;

namespace Sandbox
{
    static class Program
    {
        static double sidelength = 300;
        static double kappa = 0.197;
        static double meanBreakpoints = 35;
        static List<Point[]> visibleBreakpoints = new List<Point[]>();
        static Point origin = new Point(640, 700);

        static void Main(string[] args)
        {
            Giraffe.Run(1280, 800, gameStart);
        }

        static void gameStart()
        {
            Instance.Create(new Background { Color = Colors.LightGray });
            GlobalEvent.ExitOnEscape = true;

            for (var i = 0; i < 1000; i++)
                ArtruMennessier(); 
    
            GlobalEvent.KeyPressed += key => ArtruMennessier();

            GlobalEvent.DrawForeground += delegate ()
            {
                var polygon = new Polygon(new[] {
                    origin,
                    origin + new Vector(sidelength / Sqrt(2), -sidelength / Sqrt(2)),
                    origin + new Vector(0, -sidelength * Sqrt(2)),
                    origin + new Vector(-sidelength / Sqrt(2), -sidelength / Sqrt(2))
                });
                Draw.FillPolygon(polygon, Colors.ForestGreen);
                Draw.Polygon(polygon, Colors.Black);

                foreach (var set in visibleBreakpoints)
                {
                    for (var i = 0; i < set.Length; i++)
                    {
                        var color = Colors.Blue;
                        for (var j = 0; j < i; j++)
                        {
                            if (Abs(set[j].Y - set[i].Y) > Abs(set[j].X - set[i].X))
                            {
                                color = Colors.Red.Transparent(0.2);
                                break;
                            }
                        }
                        Draw.FillCircle(set[i], 2, color);
                    }
                }
            };
        }

        static Random rnd = new Random();
        static double rand()
        {
            var x = rnd.NextDouble();
            if (x > 0)
                return x;
            else
                return rand();
        }

        static void ArtruMennessier()
        {
            var nBreaks = Poisson(meanBreakpoints);
            var breakpoints = new Point[nBreaks];
            for (var i = 0; i < nBreaks; i++)
            {
                var l = rand() * sidelength;
                var r = rand() * sidelength;
                breakpoints[i] = new Point((l + r) / Math.Sqrt(2), (r - l) / Math.Sqrt(2));
            }

            var newSet = breakpoints.Select(p => origin + new Vector(p.Y, -p.X))
                                    .OrderByDescending(p => p.Y)
                                    .ToArray();
            visibleBreakpoints.Add(newSet);
        }

        static int Poisson(double mean)
        {
            var threshold = rnd.NextDouble();
            var sum = 0.0;
            var term = Math.Exp(-mean);
            for (int n = 0; ; )
            {
                sum += term;
                if (sum + term > threshold)
                    return n;
                n++;
                term *= mean / n;
            }
        }
        
    }
}
