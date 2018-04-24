using System;
using System.Collections.Generic;
using System.Linq;
using GRaff;

namespace MyEventGenerator.Simulator
{
    class MainClass
    {
        static SoundBuffer shockwave = SoundBuffer.Load("Assets/Shockwave.wav");
        static GRaff.Audio.SoundElement snd;

        public static void Main(string[] args)
        {
            Giraffe.Run(1000, 1000, gameStart);
        }

        static void gameStart()
        {
            GlobalEvent.ExitOnEscape = true;
            Instance.Create(new Background { Color = Colors.Black });

            simulateEvent();

            GlobalEvent.BeginStep += () =>
            {
                Global.Time++;
                //Global.Time = Mouse.X / 10;
                Window.Title = Global.Time.ToString();
            };
            GlobalEvent.KeyPressed += key =>
            {
                if (key == Key.Space)
                    Global.Time = 0;
                else if (key == Key.R)
                {
                    Console.Clear();
                    Instance<StraightTrack>.Do(t => t.Destroy());
                    Instance<CircularTrack>.Do(t => t.Destroy());
                    simulateEvent();
                }
                else if (key == Key.P)
                {
                    snd = shockwave.Play(false, 1, 1);
                }
            };
        }


        const double c = 3.0;
        const double Bfield = 0.000001;
        const double stepLength = 1;
        static void simulateEvent()
        {
            Global.Time = -GMath.Round(500 / c);
            var ev = HardProcess.produce(Particles.Up, 1700);
            ev = Decays.decay(1.0e-6, ev);
            ev = PartonShower.radiate(0.15, 0.01, 0.99, 1.0, ev);
            Console.WriteLine(ev.Entries.Count());//Event.Print(ev);

            var eventMap = new Dictionary<int, ITrack>();
            var initials = ev.Entries.Where(e => e.IsInitial).ToArray();

            Instance.Create(new QuadraticTrack(new Point(0, 500), new Vector(c, 0), -500 / c, 0, Colors.Teal));
            Instance.Create(new QuadraticTrack(new Point(1000, 500), new Vector(-c, 0), -500 / c, 0, Colors.Red));

            foreach (var entry in ev.Entries)
            {
                var v = new Vector(entry.Particle.Momentum.X, entry.Particle.Momentum.Y) * c / entry.Particle.Energy;
                var t = entry.Particle.Type;

                var color = entry.Particle.Type.Charge > 0 ? Colors.Red
                            : entry.Particle.Type.Charge < 0 ? Colors.Teal
                            : Colors.White.Transparent(0.4);
                if (entry.Mother == -1)
                {
                    if (t.Mass == 0 || t.Charge == 0)
                        eventMap[entry.Index] = Instance.Create(new StraightTrack(new Point(500, 500), v, 0, entry.IsFinal ? 10000 : stepLength, color));
                    else
                        eventMap[entry.Index] = Instance.Create(new CircularTrack(new Point(500, 500), v,
                                                                              Bfield * entry.Particle.Type.Charge / entry.Particle.Type.Mass,
                                                                              0, entry.IsFinal ? 10000 : stepLength, color));
                }
                else
                {
                    var mother = eventMap[entry.Mother];
                    if (t.Mass == 0 || t.Charge == 0)
                        eventMap[entry.Index] = Instance.Create(new StraightTrack(mother.AnnihilationLocation, v, mother.AnnihilationTime, mother.AnnihilationTime + (entry.IsFinal ? 10000 : stepLength), color));
                    else
                        eventMap[entry.Index] = Instance.Create(new CircularTrack(mother.AnnihilationLocation, v,
                                                                              entry.Particle.Type.Mass == 0 ? 0 : Bfield * entry.Particle.Type.Charge / entry.Particle.Type.Mass,
                                                                              mother.AnnihilationTime, mother.AnnihilationTime + (entry.IsFinal ? 10000 : stepLength), color));
                }
            }
        }

        static void demo()
        {

            Instance.Create(new StraightTrack(new Point(0, -400), new Vector(1, 2), 0, 400, Colors.Red));
            Instance.Create(new StraightTrack(new Point(-400, 0), new Vector(2, 1), 0, 400, Colors.Teal));

            Instance.Create(new StraightTrack(new Point(400, 400), new Vector(2, 2), 400, 450, Colors.Yellow));

            Instance.Create(new CircularTrack(new Point(500, 500), new Vector(2, 2), -0.003, 450, 1000, Colors.Teal));
            Instance.Create(new CircularTrack(new Point(500, 500), new Vector(2, 2), 0.003, 450, 1000, Colors.Red));

        }
    }
}
