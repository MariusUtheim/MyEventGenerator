using System;
using GRaff;

namespace MyEventGenerator.Simulator
{
    public class QuadraticTrack : GameElement, ITrack
    {
        public QuadraticTrack(Point origin, Vector velocity, double creationTime, double annihilationTime, Color color)
        {
            CreationLocation = origin;
            Velocity = velocity;
            CreationTime = creationTime;
            AnnihilationTime = annihilationTime;
            Color = color;
        }

        public Point CreationLocation { get; }

        public Point AnnihilationLocation => CreationLocation + Lifetime * Velocity;

        public Vector Velocity { get; }

        public double CreationTime { get; }

        public double AnnihilationTime { get; }

        public Color Color { get; set; }

        public double Lifetime => AnnihilationTime - CreationTime;

        public double TimeOffset => Global.Time - CreationTime;

        public double TrackLength { get; } = 100;

        private double _mapTime(double offset)
        {
            var t = GMath.Median(0, offset / Lifetime, 1);
            
            return GMath.Pow(t, 1.5) * offset;
        }
        public override void OnDraw()
        {
            var tailTimeOffset = _mapTime( TimeOffset - TrackLength);
            var headTimeOffset = _mapTime(TimeOffset);

            if (headTimeOffset < 0 || tailTimeOffset > Lifetime)
                return;

            var back = CreationLocation + Velocity * GMath.Max(tailTimeOffset, 0);
            var front = CreationLocation + Velocity * GMath.Min(headTimeOffset, Lifetime);
            Draw.Line(back, front,
                      Color.Transparent(-tailTimeOffset / TrackLength),
                      Color.Transparent(-(headTimeOffset - Lifetime - TrackLength) / TrackLength)
                     );
        }
    }
}
