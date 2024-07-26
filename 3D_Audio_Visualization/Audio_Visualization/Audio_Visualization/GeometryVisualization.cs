using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio_Visualization
{
    public abstract class GeometryVisualization : Visualization
    {
        public Surface srf { get; set; }
        public int intensity { get; set; }

        public int volume { get; set; }

       // private Random r = new Random();

       public GeometryVisualization(Surface srf, int intensity, int volume) { }


    }
}
