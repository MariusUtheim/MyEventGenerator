using System;
using GRaff;

namespace MyEventGenerator.Simulator
{
    public class StraightTrack : GameElement
    {
        public StraightTrack(Point origin, Vector velocity, double creationTime, double annihilationTime)
        {
            Origin = origin;
            Velocity = velocity;
            CreationTime = creationTime;
            AnnihilationTime = annihilationTime;
        }

        public Point Origin { get; }

        public Vector Velocity { get; }

        public double CreationTime { get; }

        public double AnnihilationTime { get; }

        public Color Color { get; set; }

        public double Lifetime => AnnihilationTime - CreationTime;

        public double TimeOffset => Global.Time - CreationTime;

        public double TrackLength { get; } = 100;

        public override void OnDraw()
        {
            var tailTimeOffset = TimeOffset - TrackLength;
            var headTimeOffset = TimeOffset;

            if (headTimeOffset < 0 || tailTimeOffset > Lifetime)
                return;
            
            var back = Origin + Velocity * GMath.Max(tailTimeOffset, 0);
            var front = Origin + Velocity * GMath.Min(headTimeOffset, Lifetime);
            Draw.Line(back, front, 
                      Color.Transparent(-tailTimeOffset / TrackLength), 
                      Color.Transparent(-(headTimeOffset - Lifetime - TrackLength) / TrackLength)
                     );
        }
	}
}
