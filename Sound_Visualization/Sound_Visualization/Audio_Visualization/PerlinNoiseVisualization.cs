using Rhino.Geometry;
using System;
using System.Collections.Generic;


namespace Sound_Visualization
{
    public class PerlinNoiseVisualization : GeometryVisualization
    {
        // 用NAduio解析输入音频的Frequency
        // 用音频Frequency来控制PerlinNoise的频率wq
        // Define points and boxes to store the geometry
        List<Point3d> pts = new List<Point3d>();
        List<Box> boxes = new List<Box>();
        double remapVol = 0;
        public PerlinNoiseVisualization(
            Surface srf,
            int intensity,
            int volume,
            int columns,
            int rows) : base(srf, intensity, volume, columns, rows)
        {
            // Define parameters for setting up the grid and the blocks
            // rows
            // columns

            // xRange = srf.edgelength
            // yRange = srf.edgelength

            // zHeight
            // xSize = xLength / columns * scale
            // ySize = yLength / rows * scale

            double noiseFrequency = intensity;

            // Get surface edge length
            // Calculate the length and width of the blocks
            double scale = 0.8;

            // Set up the grid
            Interval u = srf.Domain(0);
            Interval v = srf.Domain(1);

            Curve bottom = srf.IsoCurve(0, v.T0);
            Curve left = srf.IsoCurve(1, u.T0);

            double length = bottom.GetLength();
            double width = left.GetLength();

            double blockLength = length / columns * scale;
            double blockWidth = width / rows * scale;


            // Set the height of block using Perlin noise
            double blockHeight = 100;

            for (int i = 0; i < rows; i++)
            {
                double uu = u.ParameterAt((double)i / (rows - 1));

                for (int j = 0; j < columns; j++)
                {
                    double vv = v.ParameterAt((double)j / (columns - 1));

                    Point3d pt = srf.PointAt(uu, vv);

                    pts.Add(pt);

                    Plane frame;
                    srf.FrameAt(uu, vv, out frame);

                    double noise = PerlinNoise.Noise(pt.X * noiseFrequency, pt.Y * noiseFrequency);
                    double height = noise * blockHeight;

                    Interval bHeight = new Interval(0, height);
                    Interval bLength = new Interval(-blockLength * 0.5, blockLength * 0.5);
                    Interval bWidth = new Interval(-blockWidth * 0.5, blockWidth * 0.5);

                    Box box = new Box(frame, bLength, bWidth, bHeight);
                    boxes.Add(box);
                }
            }
        }
        public List<Box> GetBoxes()
        {
            return this.boxes;
        }
        public double GetVol()
        {
            return this.remapVol;
        }
    }
}
