using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;

namespace Audio_Visualization
{
    public class MainComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MainComponent()
          : base("MainComponent", "MP",
              "Handels all Computing",
              "AudioVisualization", "Visual")
        {
        }


        //public double Map(double s, double a1, double a2, double b1, double b2)
        //{
        //    return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
        //}
        public int Map(int x, int in_min, int out_min, int in_max, int out_max)
        {
            return (((x - in_min) * (out_max - out_min)) / (in_max - in_min)) + out_min;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface1", "srf", "Surface to Project to", GH_ParamAccess.item);

            pManager.AddIntegerParameter("GraphicStyle", "GS", "Select Graphical Appearance", GH_ParamAccess.item);

           //pManager.AddIntegerParameter("LightStyle", "LS", "Select Lighting Appearance", GH_ParamAccess.item);

            pManager.AddIntegerParameter("Intensity", "i", "Select wanted Intensity", GH_ParamAccess.item);

            //pManager.AddGenericParameter("Audio", "a", "Audio input from Audio Converter Component", GH_ParamAccess.item);

            pManager.AddIntegerParameter("Volume", "v", "Audio input from Audio Converter Component", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("VolumeOut", "vol", "Volume of Audio", GH_ParamAccess.item);
//this is for debugging
            pManager.AddGeometryParameter("GeometryVisuals", "GeoVisu", "List of Geometries showing", GH_ParamAccess.list);
            // pManager.AddGeometryParameter("Lightign", "light", "List of lighting data", GH_ParamAccess.list);

            pManager.AddColourParameter("c", "c", "c", GH_ParamAccess.list);

            pManager.AddIntegerParameter("Tester", "t", "Select Lighting Appearance", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Tester2", "t2", "Select Lighting Appearance", GH_ParamAccess.list);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Surface srf1 = null;
            DA.GetData("Surface1",ref srf1);

            int graphicStyle = 0;
            DA.GetData("GraphicStyle", ref graphicStyle);

            int intensity = 0;
            DA.GetData("Intensity", ref intensity);
            
            int volume = 0;
            DA.GetData("Volume", ref volume);

            List<Surface> surfaces = new List<Surface>();
            List<Point3d> points = new List<Point3d>();
            List<Box> boxes = new List<Box>();
            List<GH_Colour> colors = new List<GH_Colour>();


            double volAfter = 0;

            switch (graphicStyle)
            {
                case  1:

                    WaveVisualization geometries= new WaveVisualization(srf1,intensity,volume);
                  


                    boxes = geometries.GetBoxes();
                    volAfter = geometries.GetVol();


                    List<int> h = new List<int>();
                    foreach (Box b in boxes)
                    {
                        var a = Math.Round(b.Center.Z);
                        var a1 = Convert.ToInt32(a);
                        h.Add(a1);
                    }

                    int min = h.Min();
                    int max = h.Max();

                    List<int> h2 = new List<int>();
                    List<Color> colors2 = new List<Color>();
                    foreach (int h1 in h)
                    {
                        
                        var c = Map(h1, 0,1,70,255);
                        colors2.Add(Color.FromArgb(255, c, 100, 100));
                    }


                    Rhino.RhinoApp.WriteLine("min is " + min.ToString());
                    Rhino.RhinoApp.WriteLine("max is " + max.ToString());
                    //ColorByHeight rgb = new ColorByHeight(srf1, intensity, volume, boxes);
                    //colors = rgb.GetColors();





                    DA.SetData("VolumeOut", volAfter);
                    DA.SetDataList("GeometryVisuals", boxes);
                    DA.SetDataList("c", colors2);
                    DA.SetDataList("Tester", h);
                    DA.SetDataList("Tester2", h2);
                    break;

                default:
                    Console.WriteLine($"Measured value is not Vaild.");
                    break;
            }


      

            DA.SetData("GeometryVisuals", surfaces);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("0FC20972-B17B-4555-A9EE-75D2346CE742"); }
        }
    }
}