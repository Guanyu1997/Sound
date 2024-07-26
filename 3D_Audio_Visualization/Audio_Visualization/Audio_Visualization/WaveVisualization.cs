using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio_Visualization
{
    internal class WaveVisualization : GeometryVisualization
    {
        public WaveVisualization(Surface srf, int intensity, int volume) : base(srf, intensity, volume)
        {

        }
    }
}
