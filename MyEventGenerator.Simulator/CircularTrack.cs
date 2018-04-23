using System;
using GRaff;

namespace MyEventGenerator.Simulator
{
    public class CircularTrack : GameElement, ITrack
    {
        //public CircularTrack(Point center, double radius, double angularVelocity, double creationTime, double annihilationTime, Angle phaseOrigin, Color color)
        //{
        //    Center = center;
        //    Radius = radius;
        //    AngularVelocity = angularVelocity;
        //    CreationTime = creationTime;
        //    AnnihilationTime = annihilationTime;
        //    PhaseOrigin = phaseOrigin;
        //    Color = color;
        //}

        public CircularTrack(Point origin, Vector velocity, double curvature, double creationTime, double annihilationTime, Color color)
        {
            Contract.Requires<ArgumentOutOfRangeException>(curvature != 0);
            Radius = 1 / GMath.Abs(curvature);
            if (curvature < 0)
                Center = origin + new Vector(Radius, velocity.Direction - Angle.Deg(90));
            else
                Center = origin + new Vector(Radius, velocity.Direction + Angle.Deg(90));
            AngularVelocity = GMath.Sign(curvature) * velocity.Magnitude / Radius;
            PhaseOrigin = GMath.Atan2(velocity - Angle.Deg(90 * GMath.Sign(curvature)));
            CreationTime = creationTime;
            AnnihilationTime = annihilationTime;
            Color = color;
        }

        public Point Center { get; }

        public double Radius { get; }

        public double AngularVelocity { get; }

        public double CreationTime { get; }

        public double AnnihilationTime { get; }

        public Point CreationLocation => Center + new Vector(Radius, PhaseOrigin);

        public Point AnnihilationLocation => Center + new Vector(Radius, PhaseOrigin + Angle.Rad(AngularVelocity * Lifetime));

        public Angle PhaseOrigin { get; }

		public double Lifetime => AnnihilationTime - CreationTime;

        public double TimeOffset => Global.Time - CreationTime;

        public double TrackLength { get; } = 100;

        public Color Color { get; set; } = Colors.White;

		public override void OnDraw()
		{
            var tailTimeOffset = TimeOffset - TrackLength;
            var headTimeOffset = TimeOffset;

            if (headTimeOffset < 0 || tailTimeOffset > Lifetime)
                return;

            var backAngle = PhaseOrigin + Angle.Rad(AngularVelocity * GMath.Max(tailTimeOffset, 0));
            var dAngle = Angle.Rad(AngularVelocity * (GMath.Min(headTimeOffset, Lifetime) - GMath.Max(tailTimeOffset, 0)) / 20.0);
            var backTransparency = GMath.Median(0, -tailTimeOffset / TrackLength, 1.0);
            var dTransparency = (GMath.Median(0, -(headTimeOffset - Lifetime - TrackLength) / TrackLength, 1.0) - backTransparency) / 20.0;

            for (var i = 0; i < 20; i++)
            {
                Draw.Line(Center + new Vector(Radius, backAngle + i * dAngle), 
                          Center + new Vector(Radius, backAngle + (i + 1) * dAngle), 
                          Color.Transparent(backTransparency + i * dTransparency),
                          Color.Transparent(backTransparency + (i + 1) * dTransparency));
            }
		}
	}
}
