using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rhino.Geometry;

namespace Audio_Visualization
{
    public abstract class VisualStyle
    {
        public string GraphicStyleId { get; set; }

        public Point3d Geometry { get; set; }

        public double MusicInput { get; set; }
    }
}
