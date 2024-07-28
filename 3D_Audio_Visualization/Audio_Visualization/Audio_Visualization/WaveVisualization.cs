using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio_Visualization
{
    public class WaveVisualization : GeometryVisualization
    {
        List<Point3d> pts = new List<Point3d>();
        List<Box> boxes = new List<Box>();
        double remapVol = 0;
        public WaveVisualization(Surface srf, int intensity, int volume) : base(srf, intensity, volume)
        {



            remapVol = (double)volume / 1000;

            Brep brepSrf = srf.ToBrep();

            var edges = brepSrf.Edges;

            List<Point3d> corners = new List<Point3d>();


            foreach (var edge in edges)
            {
                var edgeCurve = edge.EdgeCurve;
                if (edgeCurve.IsClosed)
                    continue;

                var start = edgeCurve.PointAtStart;
                var end = edgeCurve.PointAtEnd;

                if (!corners.Contains(start))
                    corners.Add(start);

                if (!corners.Contains(end))
                    corners.Add(end);
            }
            double edgelength1 = edges[0].GetLength();
            double edgelength2 = edges[1].GetLength();


            Point3d pt0 = corners[0];
            Point3d pt1 = corners[2];

            //Calculate Edge Points maybe in a seperat method

            int rows = 30;
            int columns = 30;//predefined for testing
            double bX = (edgelength1-1) / rows;
            double bY = (edgelength2-1) / columns;
            double bZ = edgelength2 / (rows + columns) * intensity;
            double factor = remapVol;
            double factor1 = 0;




            double xRange = pt1.X - pt0.X;
            double yRange = pt1.Y - pt0.Y;
            double dx = xRange / (columns - 1);
            double dy = yRange / (rows - 1);

            for (int j = 0; j < rows; j++)
            {
                double y = pt0.Y + dy * j;
                for (int i = 0; i < columns; i++)
                {
                    double x = pt0.X + dx * i;
                    pts.Add(new Point3d(x, y, 0.0));
                }
            }

            for (int k = 0; k < pts.Count; k++)
            {
                Plane p = new Plane(pts[k], Vector3d.XAxis, Vector3d.YAxis);
                Interval bdx = new Interval(-bX * 0.5, bX * 0.5);
                Interval bdy = new Interval(-bY * 0.5, bY * 0.5);

                double h1 = Math.Abs(Math.Sin(p.Origin.X * factor) + Math.Cos(p.Origin.Y * factor)) * bZ;
                double h2 = Math.Abs(Math.Sin(pts[k].X * factor1) + Math.Cos(pts[k].Y * factor1)) * bZ;

                Interval bdz = new Interval(0, h1);

                Box b = new Box(p, bdx, bdy, bdz);
                boxes.Add(b);
            }
        }

        public  List<Box> GetBoxes()
        {
            return this.boxes;
        }
        public double GetVol()
        {
            return this.remapVol;
        }

    }
    }

