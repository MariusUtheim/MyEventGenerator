using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRaff;

namespace MyEventGenerator.Simulator
{
    public class FlashEffect : GameElement
    {
        public FlashEffect(double creationTime, double annihilationTime, Color color)
        {
            CreationTime = creationTime;
            AnnihilationTime = annihilationTime;
            Color = color;
            Depth = -1;
        }

        public double CreationTime { get; }
        public double AnnihilationTime { get; }
        public Color Color { get; }

        public override void OnDraw()
        {
            if (Global.Time < CreationTime || Global.Time > AnnihilationTime)
                return;

            var t = (Global.Time - CreationTime) / (AnnihilationTime - Global.Time);

            Draw.FillRectangle(Window.ClientRectangle, Color.Transparent(1 - t));
        }
    }
}
