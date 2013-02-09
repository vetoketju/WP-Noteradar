using System;
using System.IO;
using System.ComponentModel;
using Microsoft.Xna.Framework.Audio;
using System.Windows.Media;
using Pitch;

namespace Pitch
{

    public enum DetectionAlgorithm
    {
        YIN,
        AUTOCORRELATION
    }

    public class MicRecorder
    {
        private Microphone _microphone;

        private byte[] _buffer;
        private int _bufferSize;
        private TimeSpan _duration;
        private Yin yin;
        private PitchTracker ptracker;
        private BackgroundWorker worker = new BackgroundWorker();
        

        public String Note = String.Empty;
        public float MidiNote = 0;
        public float Pitch = 0.0f;

        public MicRecorder()
        {
            worker.WorkerReportsProgress = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);

            _microphone = Microphone.Default;
            
            /*
             * The duration of the capture buffer must be between 100ms and 1000ms. Additionally, the capture buffer must be 10ms aligned (BufferDuration % 10 == 0).
             * Silverlight applications must ensure that FrameworkDispatcher.Update is called regularly in order for "fire and forget" sounds to work correctly.
             * BufferDuration throws an InvalidOperationException if FrameworkDispatcher.Update has not been called at least once before making this call.
             * For more information, see Enable XNA Framework Events in Windows Phone Applications.
             */
            _microphone.BufferDuration = TimeSpan.FromMilliseconds(100);
            _duration = _microphone.BufferDuration;
            _bufferSize = _microphone.GetSampleSizeInBytes(_microphone.BufferDuration);
            
            _microphone.BufferReady += new EventHandler<EventArgs>(MicrophoneBufferReady);
            
        }

        public void Start(DetectionAlgorithm algorithm)
        {
            if (algorithm == DetectionAlgorithm.AUTOCORRELATION)
            {
                ptracker = new PitchTracker();
                ptracker.PitchDetected += ptracker_PitchDetected;
                ptracker.SampleRate = _microphone.SampleRate;
            }
            else if (algorithm == DetectionAlgorithm.YIN)
            {
                yin = new Yin(_microphone.SampleRate, _bufferSize/4);
            }
            
            _microphone.Start();
        }

        public void Stop()
        {
            
            ptracker.PitchDetected-=ptracker_PitchDetected;
            yin = null;
            _microphone.Stop();
        }

        private void MicrophoneBufferReady(object sender, EventArgs e)
        {
            _buffer = new byte[_bufferSize];
            _microphone.GetData(_buffer);
            if (!worker.IsBusy) worker.RunWorkerAsync(_buffer);            
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            float[] floatBuffer = bytesToFloats((byte[])e.Argument);
            
            if(yin != null)
                setNumbers(yin.getPitch(floatBuffer).getPitch());
            else if(ptracker != null)
                ptracker.ProcessBuffer(floatBuffer);
        }

        void ptracker_PitchDetected(PitchTracker sender, PitchTracker.PitchRecord pitchRecord)
        {
            setNumbers(pitchRecord.Pitch);
        }

        private void setNumbers(float pitch)
        {
            if (pitch < 0) pitch = 0;
            // Set pitch
            Pitch = pitch;
            MidiNote = PitchDsp.PitchToMidiNote(pitch);
            Note = PitchDsp.GetNoteName((int)MidiNote, true, true);
        }


        private static float[] bytesToFloats(byte[] bytes)
        {
            float[] floats = new float[bytes.Length / 2];
            for (int i = 0; i < bytes.Length; i += 2)
            {
                floats[i / 2] = bytes[i] | (bytes[i + 1] << 8);
            }
            return floats;
        }

        // For FFT(not implemented yet)
        public static double[] floatsToDoubles(float[] input)
        {
            double[] output = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i];
            }
            return output;
        }
    }
}
