using System;
using System.Collections.Generic;
using System.Linq;
using GRaff;
using GRaff.Synchronization;

namespace MyEventGenerator.Simulator
{
    class MainClass
    {
        static SoundBuffer shockwave;

        public static void Main(string[] args)
        {
            Giraffe.Run(1000, 1000, gameStart);
        }

        static void gameStart()
        {
            GRaff.Graphics.BlendMode.Current = GRaff.Graphics.BlendMode.Additive;
            shockwave = SoundBuffer.Load("Assets/Chargeup.wav");
            GlobalEvent.ExitOnEscape = true;
            Instance.Create(new Background { Color = Colors.Black });

            var snd = shockwave.Play(false);
            Async.Run(() => snd.Destroy());

            GlobalEvent.BeginStep += () =>
            {
                Global.Time++;
                //Global.Time = Mouse.X / 10;
                Window.Title = Global.Time.ToString();
            };
            GlobalEvent.KeyPressed += key =>
            {
                if (key == Key.R)
                {
                    Console.Clear();
                    Instance<StraightTrack>.Do(t => t.Destroy());
                    Instance<CircularTrack>.Do(t => t.Destroy());
                    simulateEvent();
                    shockwave.Play(false);
                }
                else if (key == Key.P)
                {
                    
                }
            };
        }


        const double c = 3.0;
        const double Bfield = 0.000001;
        const double stepLength = 1;
        static void simulateEvent()
        {
            Global.Time = GMath.Round(-60 * 2.2);
            var ev = HardProcess.produce(Particles.Tau, 300);
            ev = Decays.decay(1.0e-6, ev);
            ev = FinalStateRadiation.partonShower(0.15, 0.01, 0.99, 1.0, ev);
            Event.Print(ev);

            var eventMap = new Dictionary<int, ITrack>();
            var initials = ev.Entries.Where(e => e.IsInitial).ToArray();

            Instance.Create(new QuadraticTrack(new Point(-4000, 500), new Vector(4500 / -Global.Time, 0), Global.Time, 0, Colors.Teal));
            Instance.Create(new QuadraticTrack(new Point(5000, 500), new Vector(-4500 / -Global.Time, 0), Global.Time, 0, Colors.Red));
            Instance.Create(new FlashEffect(0, 15, Colors.White));

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
