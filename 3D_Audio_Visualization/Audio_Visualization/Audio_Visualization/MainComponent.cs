using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Grasshopper.Kernel;
using Rhino.DocObjects;
using Rhino.Geometry;

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

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddSurfaceParameter("Surface1", "srf", "Surface to Project to", GH_ParamAccess.item);

            pManager.AddIntegerParameter("GraphicStyle", "GS", "Select Graphical Appearance", GH_ParamAccess.item);

           // pManager.AddIntegerParameter("LightStyle", "LS", "Select Lighting Appearance", GH_ParamAccess.item);

            pManager.AddIntegerParameter("Intensity", "i", "Select wanted Intensity", GH_ParamAccess.item);

            //pManager.AddGenericParameter("Audio", "a", "Audio input from Audio Converter Component", GH_ParamAccess.item);

            pManager.AddIntegerParameter("Volume", "v", "Audio input from Audio Converter Component", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("GeometryVisuals", "GeoVisu", "List of Geometries showing", GH_ParamAccess.list);
           // pManager.AddGeometryParameter("Lightign", "light", "List of lighting data", GH_ParamAccess.list);

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

            switch (graphicStyle)
            {
                case  1:

                    WaveVisualization geometries= new WaveVisualization();
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