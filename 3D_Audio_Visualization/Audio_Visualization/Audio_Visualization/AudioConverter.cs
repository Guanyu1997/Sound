using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using NAudio;
using NAudio.CoreAudioApi;
using Eto.Forms;
using System.Linq;

namespace Audio_Visualization
{
    public class AudioConverter : GH_Component
    {   
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public AudioConverter()
          : base("Audio_Converter", "AudiConv",
            "Description",
            "AudioVisualization", "Audio")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Volume", "vol", "Volume of Audio", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
          //  var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
          //  string targetDeviceName = "Lautsprecher (Realtek(R) Audio)";

            // Find the device by its friendly name
            //MMDevice selectedDevice = devices.FirstOrDefault(d => d.FriendlyName.Contains(targetDeviceName));

            MMDevice selectedDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            int maxMasterValue = 0;

            if (selectedDevice != null)
            {
    
                maxMasterValue =  (int)(Math.Round(selectedDevice.AudioMeterInformation.MasterPeakValue * 100));

            }
            else
            {
                throw new Exception("Device not found.");
            }

            DA.SetData("Volume", maxMasterValue);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("75a64878-0e54-41f9-8a2e-2911a5dec3f8");
    }
}