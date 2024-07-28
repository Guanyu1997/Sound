using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using NAudio;
using NAudio.CoreAudioApi;
using Eto.Forms;
using System.Linq;
using MathNet.Numerics;
using NAudio.Wave;
using MathNet.Numerics.IntegralTransforms;

namespace Audio_Visualization
{
    public class AudioConverter : GH_Component
    {
        public WaveInEvent waveIn;
        public float[] audioBuffer;
        public int bufferSize = 1024;
        //public double frequenzy;
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
   /*        
            waveIn = new WaveInEvent();
            waveIn.BufferMilliseconds = 50; // Set buffer size
            waveIn.NumberOfBuffers = 1; // Set number of buffers
            waveIn.WaveFormat = new WaveFormat(44100, 16, 1); // 44.1 kHz, 16-bit, mono

            audioBuffer = new float[bufferSize];
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.StartRecording();
           */
        }
/*
        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            // Convert byte buffer to float samples
            int sampleCount = e.BytesRecorded / sizeof(short);
            short[] shortBuffer = new short[sampleCount];
            Buffer.BlockCopy(e.Buffer, 0, shortBuffer, 0, e.BytesRecorded);
            for (int i = 0; i < sampleCount; i++)
            {
                audioBuffer[i] = shortBuffer[i] / 32768f; // Convert to float
            }

            // Perform FFT
            var fft = new Complex32[bufferSize];
            for (int i = 0; i < bufferSize; i++)
            {
                fft[i] = new Complex32(audioBuffer[i], 0);
            }

            Fourier.Forward(fft, FourierOptions.Matlab);

            // Analyze frequency bands
            AnalyzeFrequencyBands(fft);//, ref frequenzy
        }

        private void AnalyzeFrequencyBands(Complex32[] fftResult)//, ref double frequenzy
        {
          

            for (int i = 0; i < fftResult.Length / 2; i++)
            {
                double frequenzy = (i * 44100.0) / bufferSize;

                Console.WriteLine(frequenzy.ToString());
                
            }

      
        }
       */ 

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