using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Audio_Visualization
{
    public class WeaveBlock : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public WeaveBlock()
          : base("WeaveBlock", "Weave",
              "Generate weavy block on top of a plane",
              "AudioVisualization", "VisualStyle")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("pt0", "pt0", "Start point", GH_ParamAccess.item);
            pManager.AddPointParameter("pt1", "pt1", "End point", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Rows", "Rows", "Number of rows", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Columns", "Columns", "Number of columns", GH_ParamAccess.item);
            pManager.AddNumberParameter("bX", "bX", "Box dimension X", GH_ParamAccess.item);
            pManager.AddNumberParameter("bY", "bY", "Box dimension Y", GH_ParamAccess.item);
            pManager.AddNumberParameter("bZ", "bZ", "Box dimension Z", GH_ParamAccess.item);
            pManager.AddNumberParameter("Factor", "Factor", "Factor for height calculation", GH_ParamAccess.item);
            pManager.AddNumberParameter("Factor1", "Factor1", "Factor1 for height calculation", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "Pts", "Grid points", GH_ParamAccess.list);
            pManager.AddBoxParameter("Boxes", "Boxes", "Generated boxes", GH_ParamAccess.list);
        }
    }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
        Point3d pt0 = Point3d.Unset;
        Point3d pt1 = Point3d.Unset;
        int rows = 0;
        int columns = 0;
        double bX = 0.0;
        double bY = 0.0;
        double bZ = 0.0;
        double factor = 0.0;
        double factor1 = 0.0;

        if (!DA.GetData(0, ref pt0)) return;
        if (!DA.GetData(1, ref pt1)) return;
        if (!DA.GetData(2, ref rows)) return;
        if (!DA.GetData(3, ref columns)) return;
        if (!DA.GetData(4, ref bX)) return;
        if (!DA.GetData(5, ref bY)) return;
        if (!DA.GetData(6, ref bZ)) return;
        if (!DA.GetData(7, ref factor)) return;
        if (!DA.GetData(8, ref factor1)) return;

        List<Point3d> pts = new List<Point3d>();
        List<Box> boxes = new List<Box>();

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

            Interval bdz = new Interval(-h2, h1);

            Box b = new Box(p, bdx, bdy, bdz);
            boxes.Add(b);
        }

        DA.SetDataList(0, pts);
        DA.SetDataList(1, boxes);
    }

    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    protected override System.Drawing.Bitmap Icon => null;

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid => new Guid("75a64878-0e54-41f9-8a2e-2911a5dec3f8");
}