using Grasshopper.Kernel;
using NAudio.Dsp;
using NAudio.Wave;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sound_Visualization
{
    public class AudioAnalyzer : GH_Component
    {
        private WasapiLoopbackCapture capture;
        private float peakVolume = 0f;
        private float peakFrequency = 0f;
        private float rmsVolume = 0f;
        private float lowEnergy = 0f;
        private float midEnergy = 0f;
        private float highEnergy = 0f;
        private float spectralCentroid = 0f;          // 新增：频率质心
        private List<float> spectrum = new List<float>();
        private readonly object lockObject = new object();
        private bool isCapturing = false;
        private bool newDataAvailable = false;

        public AudioAnalyzer()
          : base("AudioAnalyzer",
                "Audio",
                "Analyze System Audio",
                "SoundVisualization",
                "Audio")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Start", "S", "True 开始捕获，False 停止捕获", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Peak Volume", "V", "当前峰值音量 (0-1)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Peak Frequency", "F", "当前峰值频率 (Hz)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Spectrum", "Spec", "频谱数据 (频率幅度列表)", GH_ParamAccess.list);
            pManager.AddNumberParameter("RMS Volume", "RMS", "实时RMS音量", GH_ParamAccess.item);
            pManager.AddNumberParameter("Low Freq Energy", "Low", "低频能量 (20-250Hz)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mid Freq Energy", "Mid", "中频能量 (250Hz-4kHz)", GH_ParamAccess.item);
            pManager.AddNumberParameter("High Freq Energy", "High", "高频能量 (4kHz-20kHz)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Spectral Centroid", "Centroid", "频率质心 (Hz)", GH_ParamAccess.item); // 新增
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool start = false;
            if (!DA.GetData(0, ref start)) return;

            if (start && !isCapturing)
                StartCapture();
            else if (!start && isCapturing)
                StopCapture();

            lock (lockObject)
            {
                DA.SetData(0, peakVolume);
                DA.SetData(1, peakFrequency);
                DA.SetDataList(2, spectrum);
                DA.SetData(3, rmsVolume);
                DA.SetData(4, lowEnergy);
                DA.SetData(5, midEnergy);
                DA.SetData(6, highEnergy);
                DA.SetData(7, spectralCentroid);      // 新增
            }
        }

        private void StartCapture()
        {
            try
            {
                capture = new WasapiLoopbackCapture();
                capture.DataAvailable += OnDataAvailable;
                capture.RecordingStopped += OnRecordingStopped;
                capture.StartRecording();
                isCapturing = true;
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "音频捕获已启动");
            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"启动捕获失败: {ex.Message}");
            }
        }

        private void StopCapture()
        {
            if (capture != null && isCapturing)
            {
                capture.StopRecording();
                isCapturing = false;
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "音频捕获已停止");
            }
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            int sampleCount = e.BytesRecorded / 4;
            if (sampleCount == 0) return;

            float[] allSamples = new float[sampleCount];
            Buffer.BlockCopy(e.Buffer, 0, allSamples, 0, e.BytesRecorded);

            int channels = capture.WaveFormat.Channels;
            int samplesPerChannel = sampleCount / channels;

            float[] mono = new float[samplesPerChannel];
            for (int i = 0; i < samplesPerChannel; i++)
            {
                float sum = 0f;
                for (int ch = 0; ch < channels; ch++)
                    sum += allSamples[ch + i * channels];
                mono[i] = sum / channels;
            }

            // 时域：峰值和RMS
            float currentPeak = 0f;
            float sumSquares = 0f;
            foreach (float s in mono)
            {
                float absVal = Math.Abs(s);
                if (absVal > currentPeak) currentPeak = absVal;
                sumSquares += s * s;
            }
            float currentRMS = (float)Math.Sqrt(sumSquares / mono.Length);

            int fftSize = 1024;
            if (mono.Length >= fftSize)
            {
                float[] fftData = new float[fftSize];
                Array.Copy(mono, mono.Length - fftSize, fftData, 0, fftSize);

                // 移除直流
                float mean = fftData.Average();
                for (int i = 0; i < fftData.Length; i++)
                    fftData[i] -= mean;

                Complex[] complexData = new Complex[fftSize];
                for (int i = 0; i < fftSize; i++)
                {
                    complexData[i].X = fftData[i] * (float)FastFourierTransform.HammingWindow(i, fftSize);
                    complexData[i].Y = 0;
                }

                FastFourierTransform.FFT(true, (int)Math.Log(fftSize, 2), complexData);

                float[] magnitude = new float[fftSize / 2];
                float maxMag = 0f;
                int peakIdx = 0;
                float sampleRate = capture.WaveFormat.SampleRate;

                // 频段累加及质心计算
                float lowSum = 0f, midSum = 0f, highSum = 0f;
                float lowFreqMin = 20f, lowFreqMax = 250f;
                float midFreqMin = 250f, midFreqMax = 4000f;
                float highFreqMin = 4000f, highFreqMax = 20000f;

                // 用于质心：加权和与总幅度（跳过直流）
                float weightedSum = 0f;
                float totalMag = 0f;

                for (int i = 0; i < fftSize / 2; i++)
                {
                    float real = complexData[i].X;
                    float imag = complexData[i].Y;
                    float mag = (float)Math.Sqrt(real * real + imag * imag);
                    magnitude[i] = mag;

                    if (i > 0 && mag > maxMag)
                    {
                        maxMag = mag;
                        peakIdx = i;
                    }

                    float freq = i * sampleRate / fftSize;

                    // 频段能量
                    if (freq >= lowFreqMin && freq <= lowFreqMax)
                        lowSum += mag;
                    else if (freq > midFreqMin && freq <= midFreqMax)
                        midSum += mag;
                    else if (freq > highFreqMin && freq <= highFreqMax)
                        highSum += mag;

                    // 质心累加（跳过直流 i=0）
                    if (i > 0)
                    {
                        weightedSum += freq * mag;
                        totalMag += mag;
                    }
                }

                // 计算质心
                float centroid = (totalMag > 0f) ? (weightedSum / totalMag) : 0f;

                // 处理峰值索引直流问题
                if (peakIdx == 0 && magnitude.Length > 1)
                {
                    float secondMax = 0f;
                    int secondIdx = 1;
                    for (int i = 1; i < magnitude.Length; i++)
                    {
                        if (magnitude[i] > secondMax)
                        {
                            secondMax = magnitude[i];
                            secondIdx = i;
                        }
                    }
                    peakIdx = secondIdx;
                }

                float peakFreq = (float)peakIdx * sampleRate / fftSize;

                lock (lockObject)
                {
                    peakVolume = currentPeak;
                    peakFrequency = peakFreq;
                    spectrum = magnitude.ToList();
                    rmsVolume = currentRMS;
                    lowEnergy = lowSum;
                    midEnergy = midSum;
                    highEnergy = highSum;
                    spectralCentroid = centroid;   // 新增
                }
            }
            else
            {
                // 数据不足时只更新时域值，质心设0
                lock (lockObject)
                {
                    peakVolume = currentPeak;
                    rmsVolume = currentRMS;
                    // 其他值保持上次或设为0，这里可选择性清零
                    spectralCentroid = 0f;
                }
            }
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            capture?.Dispose();
            capture = null;
            isCapturing = false;
            if (e.Exception != null)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, $"停止异常: {e.Exception.Message}");
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            StopCapture();
            base.RemovedFromDocument(document);
        }

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("56C635E9-7C37-4807-92B7-C674FED9C6C3");
    }
}