using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using System.Windows.Navigation;
using Pitch;
using Microsoft.Xna.Framework;
using WP_Pitchdetection.ViewModels;
using System.Windows.Controls.Primitives;

namespace WP_Pitchdetection
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.LayoutRoot.DataContext = App.ViewModel;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            App.ViewModel.stopDetecting();
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            App.ViewModel.startDetecting();
        }

        private void toggleAlgorithm_Click(object sender, RoutedEventArgs e)
        {
           ToggleButton _clicked = sender as ToggleButton;
           _clicked.IsChecked = true;

           if(_clicked.Name == "toggleAutoCorrelation")
           {
               toggleYin.IsChecked = false;
               if (App.ViewModel.Algorithm != DetectionAlgorithm.AUTOCORRELATION)
               {
                   App.ViewModel.Algorithm = DetectionAlgorithm.AUTOCORRELATION;
                   App.ViewModel.restartDetection();
               }
                    
           }
           else if (_clicked.Name == "toggleYin")
           {
               toggleAutoCorrelation.IsChecked = false;
               if (App.ViewModel.Algorithm != DetectionAlgorithm.YIN)
               {
                   App.ViewModel.Algorithm = DetectionAlgorithm.YIN;
                   App.ViewModel.restartDetection();
               }
           }

        }

    }
}