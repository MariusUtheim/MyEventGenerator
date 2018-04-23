using System;
using GRaff;

namespace MyEventGenerator.Simulator
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Giraffe.Run(1000, 1000, gameStart);
        }

        static void gameStart()
        {
            GlobalEvent.ExitOnEscape = true;
            Instance.Create(new Background { Color = Colors.Black });

            Instance.Create(new StraightTrack(new Point(0, -400), new Vector(1, 2), 0, 400) { Color = Colors.Red });
            Instance.Create(new StraightTrack(new Point(-400, 0), new Vector(2, 1), 0, 400) { Color = Colors.Teal });

            Instance.Create(new StraightTrack(new Point(400, 400), new Vector(2, 2), 400, 450) { Color = Colors.Yellow });

            Instance.Create(new CircularTrack(new Point(500, 500), new Vector(2, 2), -0.003, 450, 1000) { Color = Colors.Teal });
            Instance.Create(new CircularTrack(new Point(500, 500), new Vector(2, 2), 0.003, 450, 1000) { Color = Colors.Red });


            //Instance.Create(new CircularTrack(new Point(500, 500), new Vector(0, -1), 0.01, 100, 1000));


            GlobalEvent.BeginStep += () =>
            {
                Global.Time++;
                //Global.Time = Mouse.X;
                Window.Title = Global.Time.ToString();
            };
            GlobalEvent.KeyPressed += key =>
            {
                if (key == Key.Space)
                    Global.Time = 0;
            };
        }
    }
}
