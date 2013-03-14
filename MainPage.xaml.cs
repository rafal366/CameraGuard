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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CameraGuard
{
    public partial class MainPage : PhoneApplicationPage
    {
        private PhotoCamera camera;
        DispatcherTimer timer = new DispatcherTimer();

        // Constructor
        public MainPage()
        {
            timer.Interval = new TimeSpan(0,0,0,5);
            timer.Tick += TimerOnTick;
            timer.Start();
            InitializeComponent();
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            byte[] buffer = new byte[524288];
            camera.GetPreviewBufferY(buffer);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            camera = new PhotoCamera();

            video.SetSource(camera);
            camera.Initialized += CameraOnInitialized;
            camera.AutoFocusCompleted += CameraOnAutoFocusCompleted;
            camera.CaptureImageAvailable += CameraOnCaptureImageAvailable;
            camera.CaptureCompleted += CameraOnCaptureCompleted;

            CameraButtons.ShutterKeyHalfPressed += CameraButtonsOnShutterKeyHalfPressed;
            CameraButtons.ShutterKeyPressed += CameraButtonsOnShutterKeyPressed;
            CameraButtons.ShutterKeyReleased += CameraButtonsOnShutterKeyReleased;


            base.OnNavigatedTo(e);
        }

        private void CameraOnInitialized(object sender, CameraOperationCompletedEventArgs cameraOperationCompletedEventArgs)
        {
            if (cameraOperationCompletedEventArgs.Succeeded)
            {
                try
                {
                    var res = from resolution in camera.AvailableResolutions
                              orderby resolution.Width
                              select resolution;

                    camera.Resolution = res.First();
                }
                catch (Exception)
                {


                }
            }
        }

        private void CameraOnAutoFocusCompleted(object sender, CameraOperationCompletedEventArgs cameraOperationCompletedEventArgs)
        {

        }

        private void CameraOnCaptureImageAvailable(object sender, ContentReadyEventArgs contentReadyEventArgs)
        {
            Dispatcher.BeginInvoke(delegate()
            {
                WriteableBitmap bitmap = new WriteableBitmap((int)camera.Resolution.Width, (int)camera.Resolution.Height);
                contentReadyEventArgs.ImageStream.Position = 0;
                bitmap.LoadJpeg(contentReadyEventArgs.ImageStream);
                PhoneApplicationService.Current.State["obraz"] = bitmap;

                NavigationService.Navigate(new Uri("/Result.xaml", UriKind.Relative));

            });


        }

        private void CameraOnCaptureCompleted(object sender, CameraOperationCompletedEventArgs cameraOperationCompletedEventArgs)
        {

        }

        private void CameraButtonsOnShutterKeyHalfPressed(object sender, EventArgs eventArgs)
        {
            camera.Focus();
        }

        private void CameraButtonsOnShutterKeyPressed(object sender, EventArgs eventArgs)
        {
            try
            {
                camera.CaptureImage();
            }
            catch (Exception)
            {

            }

        }

        private void CameraButtonsOnShutterKeyReleased(object sender, EventArgs eventArgs)
        {
            camera.CancelFocus();
        }
    }
}