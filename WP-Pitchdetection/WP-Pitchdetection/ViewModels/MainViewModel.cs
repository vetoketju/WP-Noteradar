using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Pitch;
using System.Windows.Threading;
using Microsoft.Xna.Framework;

namespace WP_Pitchdetection.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        DispatcherTimer timer = new DispatcherTimer();
        MicRecorder micRecorder;
        public DetectionAlgorithm Algorithm = DetectionAlgorithm.AUTOCORRELATION;

        public void startDetecting()
        {
            //start the timer
            timer.Start();
            FrameworkDispatcher.Update();
            // Start micrecorder
            micRecorder.Start(Algorithm);
        }

        public void stopDetecting()
        {
            micRecorder.Stop();
            timer.Stop();
        }

        public void restartDetection()
        {
            micRecorder.Stop();
            micRecorder.Start(Algorithm);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            FrameworkDispatcher.Update();
            Pitch = micRecorder.Pitch;
            Note = micRecorder.Note;
            // Binding does not work with slider, fix:
            //PitchSlider.Value = micRecorder.Pitch;
        }


        public MainViewModel() {
            micRecorder = new MicRecorder();
            timer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Tick += timer_Tick;
            pitch = 0.0f;
            note = "";
        }

        private double pitch;
        public double Pitch
        {
            get { return pitch; }
            set { if (value != pitch) { pitch = value; NotifyPropertyChanged("Pitch"); } }
        }


        private string note;
        public string Note
        {
            get { return note; }
            set { if (value != note) { note = value; NotifyPropertyChanged("Note"); } }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
