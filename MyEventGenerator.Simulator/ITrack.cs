using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GRaff;

namespace MyEventGenerator.Simulator
{
    public interface ITrack
    {
        Point CreationLocation { get; }
        double CreationTime { get; }
        Point AnnihilationLocation { get; }
        double AnnihilationTime { get; }
    }
}
