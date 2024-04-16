using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System.ComponentModel;
using Microsoft.Toolkit.Uwp.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPTest
{
    
    public sealed partial class MainPage : Page
    {
        public bool recording = false;
        public bool paused = false;
        public bool setup = false;

        private double _volume;
        private bool mouseCaptured = false;

        public double Volume
        {
            get { return _volume; }
            set
            {
                _volume = value; 
            }
        }

        Storyboard animateSpinner = new Storyboard();

        public MainPage()
        {
            var size = new Windows.Foundation.Size(800, 330);
            ApplicationView.GetForCurrentView().SetPreferredMinSize(size);
            ApplicationView.PreferredLaunchViewSize = size;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            Window.Current.CoreWindow.SizeChanged += (s, e) =>
            {
                ApplicationView.GetForCurrentView().TryResizeView(size);

            };

            DataContext = this;
            this.InitializeComponent();
            Window.Current.SetTitleBar(null);

            
        }

        /*
        =========================================================================
        =========================== PLAY PAUSE BUTTON ===========================
        =========================================================================
        */

        private void PauseButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            if (recording)
            {
                PauseButton.Visibility = Visibility.Collapsed;
                PauseButton.IsTapEnabled = false;
                PlayButton.Visibility = Visibility.Visible;
                PlayButton.IsTapEnabled = true;
                animateSpinner.Pause();
                paused = true;
            }
        }

        private void PlayButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            if (recording)
            {
                PlayButton.Visibility = Visibility.Collapsed;
                PlayButton.IsTapEnabled = false;
                PauseButton.Visibility = Visibility.Visible;
                PauseButton.IsTapEnabled = true;
                animateSpinner.Resume();
                paused = true;
            }
        }

        /*
        =========================================================================
        ============================= RECORD BUTTON =============================
        =========================================================================
        */

        private void RecordButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            recording = !recording;

            if (recording)
            {
                if (setup)
                {
                    animateSpinner.Resume();
                }
                else
                {
                    Spinner.Margin = new Thickness(-255, -156, 0, 0);
                    Spinner.Height = 700;
                    Spinner.Width = 700;

                    DoubleAnimation spinnerAnimation = new DoubleAnimation();
                    
                    spinnerAnimation.From = SpinnerRotationVar.Rotation;
                    spinnerAnimation.To = SpinnerRotationVar.Rotation - 360;

                    spinnerAnimation.Duration = new Duration(TimeSpan.FromSeconds(2));
                    spinnerAnimation.RepeatBehavior = RepeatBehavior.Forever;
                    Storyboard.SetTarget(spinnerAnimation, Spinner);
                    Storyboard.SetTargetProperty(spinnerAnimation, "(Image.RenderTransform).(CompositeTransform.Rotation)");

                    animateSpinner.Children.Add(spinnerAnimation);
                    animateSpinner.Begin();

                    setup = true;
                }
            }
            else
            {
                animateSpinner.Pause();
            }

            Trace.WriteLine(recording);
        }

        /*
        =========================================================================
        ======================== VOLUME KNOB CONTROLLER =========================
        =========================================================================
        */

        private void KnobController_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            mouseCaptured = true;
            var y = e.GetCurrentPoint(KnobController).Position.Y;
            var ratio = y / KnobController.ActualHeight;
            Volume = ratio*KnobController.Maximum;
            Trace.WriteLine("Mouse Captured at " + y);
        }

        private void KnobController_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (mouseCaptured)
            {
                var y = e.GetCurrentPoint(KnobController).Position.Y;
                var ratio = y / KnobController.ActualHeight;
                Volume = ratio * KnobController.Maximum;
                Trace.WriteLine("Volume: " + Volume);
                
                var Knob = KnobController.FindDescendant(name: "Knob");
                RotateTransform rotation = new RotateTransform();
                rotation.Angle = -140 + (2.8 * (float)Volume);
                Knob.RenderTransform = rotation;

                TextBlock textBlock = (TextBlock)KnobController.FindDescendant(name: "VolumeText");
                textBlock.Text = ((int)Volume + 1).ToString();
            }
        }

        private void KnobController_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            mouseCaptured = false;
        }

        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
