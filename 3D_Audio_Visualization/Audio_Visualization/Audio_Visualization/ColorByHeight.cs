using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Grasshopper.Kernel.Types;

namespace Audio_Visualization
{
    public class ColorByHeight : GeometryVisualization
    {
        List<GH_Colour> colors = new List<GH_Colour>();
        //List<int> colorVal = new List<int>();
        public ColorByHeight(Surface srf, int intensity, int volume, List<Box> boxes) : base(srf, intensity, volume)
        {
            List<double> h = new List<double>();
            foreach (Box b in boxes)
            {
                h.Add(b.Center.Z);
            }
            double min = h.Min();
            double max = h.Max();
        
            //List<double> cValues = new List<double>();
            foreach (double d in h)
            {
                int d1 = this.Map(d, min, 0, max, 255);
                GH_Colour c = new GH_Colour(Color.FromArgb(255, d1, 150, 150));
           
                colors.Add(c);
            }
        }

        public int Map(double value, double from1, double to1, double from2, double to2)
        {
            int remaped = Convert.ToInt32((value - from1) / (to1 - from1) * (to2 - from2) + from2);
            return remaped;
        }

        public List<GH_Colour> GetColors()
        {
            return this.colors;
        }
    }
   
    }
