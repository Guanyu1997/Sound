using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sound_Visualization
{
    public abstract class GeometryVisualization : Visualization
    {
        public Surface srf { get; set; }
        public int intensity { get; set; }
        public int volume { get; set; }
        public int columns { get; set; }
        public int rows { get; set; }

        // private Random r = new Random();
        public GeometryVisualization(Surface srf, int intensity, int volume, int columns, int rows) {}
    }
}
