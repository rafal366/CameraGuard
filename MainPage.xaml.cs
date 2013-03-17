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
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;
using CameraProject;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CameraGuard
{
    
    public partial class MainPage : PhoneApplicationPage
    {
        private bool start = false;
       int[] buffer = new int[524288];
       byte[] bufferr = new byte[524288];
       byte[] bufferLast = new byte[524288];
        private PhotoCamera camera;
        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timer1 = new DispatcherTimer();
        private WriteableBitmap zapisany;
        private int t = 128;
        private int lastCount = 0;
        int count = 0;
        // Constructor
        public MainPage()
        {
            timer.Interval = new TimeSpan(0,0,0,0, 1);
            timer.Tick += TimerOnTick;

            timer1.Interval = new TimeSpan(0, 0, 0, 1);
            timer1.Tick += TimerOnTick1;
           timer1.Start();
            InitializeComponent();
         //   kopia.Source = new BitmapImage(new Uri("example.bmp", UriKind.Relative));
        }

        private void TimerOnTick1(object sender, EventArgs e)
        {
            if (start)
            {
                timer1.Stop();
              //  timer.Start();

            }
            

        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            lastCount = count;
            count = 0;
            int[] pixelData = new int[(int)(camera.PreviewResolution.Width * camera.PreviewResolution.Height)];  //640x480

            camera.GetPreviewBufferArgb32(pixelData);

            WriteableBitmap freezeWriteableBitmap = new WriteableBitmap((int)camera.PreviewResolution.Width, (int)camera.PreviewResolution.Height);

            pixelData.CopyTo(freezeWriteableBitmap.Pixels, 0);

            freezeWriteableBitmap.Invalidate();
         //   timer.Stop();
            int[] buffer1 = buffer;

            try
            {
                camera.GetPreviewBufferArgb32(buffer);
               // camera.GetPreviewBufferY(bufferr);
            }
            catch (Exception e)
            {
                
                
            }
            //int t = 128;
           freezeWriteableBitmap = freezeWriteableBitmap.Resize(t, t,
                                                                 WriteableBitmapExtensions.Interpolation.Bilinear);
/*
            for (int i = 0; i < bufferr.Length; i++)
            {
                if (bufferLast[i] != bufferr[i])
                {
                    count++;
                }
            }*/
/*
          for (int i = 0; i < t; i++)
           {
               for (int j = 0; j < t; j++)
               {
                   var a = freezeWriteableBitmap.GetPixel(i, j);
                  byte d = (byte)((a.R + a.G + a.B)/3);
                   
                    freezeWriteableBitmap.SetPixel(i,j, d,d,d);
               }
           }*/
            int srednia = 0;
            for (int i = 0; i < t; i++)
            {
                for (int j = 0; j <t; j++)
                {
                    var d = freezeWriteableBitmap.GetPixel(i, j);

                    srednia += (d.R + d.G + d.B)/ 3;
                }
            }
            srednia = srednia/(128*128);
            for (int i = 0; i < t; i++)
            {
                for (int j = 0; j <t; j++)
                {
                    var a = freezeWriteableBitmap.GetPixel(i, j);
                    if ((a.R + a.G + a.B)/3 <srednia)
                    {
                        freezeWriteableBitmap.SetPixel(i,j,0,0,0);
                    }
                    else
                    {
                        freezeWriteableBitmap.SetPixel(i,j,255,255,255);
                    }
                }
            }









   /*          int ApetureMin = -(10 / 2);
       int ApetureMax = (2 / 2);
            for (int x = 0; x < t; ++x)
            {
                for (int y = 0; y < t; ++y)
                {
                    int RValue = 0;
                    int GValue = 0;
                    int BValue = 0;
                    for (int x2 = ApetureMin; x2 < ApetureMax; ++x2)
                    {
                        int TempX = x + x2;
                        if (TempX >= 0 && TempX < t)
                        {
                            for (int y2 = ApetureMin; y2 < ApetureMax; ++y2)
                            {
                                int TempY = y + y2;
                                if (TempY >= 0 && TempY < t)
                                {
                                    Color TempColor = freezeWriteableBitmap.GetPixel(TempX, TempY);
                                    if (TempColor.R > RValue)
                                        RValue = TempColor.R;
                                    if (TempColor.G > GValue)
                                        GValue = TempColor.G;
                                    if (TempColor.B > BValue)
                                        BValue = TempColor.B;
                                }
                            }
                        }
                    }
                    //Color TempPixel = Color.FromArgb(0, (byte) RValue, (byte) GValue, (byte) BValue);
                    freezeWriteableBitmap.SetPixel(x, y, (byte)RValue, (byte)GValue, (byte)BValue);
                }
            }*/











            kopia.Source = freezeWriteableBitmap;
            
            for (int i = 0; i < t; i++)
            {
                for (int j = 0; j < t; j++)
                {
                    
                    if (zapisany.GetPixel(i, j) != freezeWriteableBitmap.GetPixel(i, j))
                    {
                        count++;

                    }
                }
            }
            if ((count - lastCount) > 100)
            {
                startButton.Content = "Ruch";
            }
            else
            {
                startButton.Content = "Brak ruchu";
            }
            
            if (count < 200)
            {
                setFrame();
            }

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

            if (start)
            {
                timer.Start();
            }
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
                    start = true;
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

            //    NavigationService.Navigate(new Uri("/result.xaml", UriKind.Relative));

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


        internal int ColorToGray(int color)
        {
            int gray = 0;

            int a = color >> 24;
            int r = (color & 0x00ff0000) >> 16;
            int g = (color & 0x0000ff00) >> 8;
            int b = (color & 0x000000ff);

            if ((r == g) && (g == b))
            {
                gray = color;
            }
            else
            {
                int i = (7 * r + 38 * g + 19 * b + 32) >> 6;

                gray = ((a & 0xFF) << 24) | ((i & 0xFF) << 16) | ((i & 0xFF) << 8) | (i & 0xFF);
            }
            return gray;
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {

            setFrame();
            timer.Start();
        }

        void setFrame()
        {
            camera.GetPreviewBufferYCbCr(bufferLast);
            int[] pixelData = new int[(int)(camera.PreviewResolution.Width * camera.PreviewResolution.Height)];  //640x480

            camera.GetPreviewBufferArgb32(pixelData);
            zapisany = new WriteableBitmap((int)camera.PreviewResolution.Width, (int)camera.PreviewResolution.Height);

            pixelData.CopyTo(zapisany.Pixels, 0);

            zapisany.Invalidate();

            zapisany = zapisany.Resize(t, t,
                                                                 WriteableBitmapExtensions.Interpolation.Bilinear);

/*            int ApetureMin = -(2 / 2);
            int ApetureMax = (2 / 2);
            for (int x = 0; x < t; ++x)
            {
                for (int y = 0; y < t; ++y)
                {
                    int RValue = 0;
                    int GValue = 0;
                    int BValue = 0;
                    for (int x2 = ApetureMin; x2 < ApetureMax; ++x2)
                    {
                        int TempX = x + x2;
                        if (TempX >= 0 && TempX < t)
                        {
                            for (int y2 = ApetureMin; y2 < ApetureMax; ++y2)
                            {
                                int TempY = y + y2;
                                if (TempY >= 0 && TempY < t)
                                {
                                    Color TempColor = zapisany.GetPixel(TempX, TempY);
                                    if (TempColor.R > RValue)
                                        RValue = TempColor.R;
                                    if (TempColor.G > GValue)
                                        GValue = TempColor.G;
                                    if (TempColor.B > BValue)
                                        BValue = TempColor.B;
                                }
                            }
                        }
                    }
                    //Color TempPixel = Color.FromArgb(0, (byte) RValue, (byte) GValue, (byte) BValue);
                    zapisany.SetPixel(x, y, (byte)RValue, (byte)GValue, (byte)BValue);
                }
            }*/

            /*        for (int i = 0; i < 128; i++)
                    {
                        for (int j = 0; j < 128; j++)
                        {
                            var a = zapisany.GetPixel(i, j);
                            byte d = (byte)((a.R + a.G + a.B) / 3);

                           zapisany.SetPixel(i, j, d, d, d);
                        }
                    }*/
/*            for (int i = 0; i < t; i++)
            {
                for (int j = 0; j < t; j++)
                {
                    var a = zapisany.GetPixel(i, j);
                    byte d = (byte)((a.R + a.G + a.B) / 3);

                    zapisany.SetPixel(i, j, d, d, d);
                }
            }*/

            for (int i = 0; i < t; i++)
            {
                for (int j = 0; j < t; j++)
                {
                    var a = zapisany.GetPixel(i, j);
                    if ((a.R + a.G + a.B) / 3 < 128)
                    {
                        zapisany.SetPixel(i, j, 0, 0, 0);
                    }
                    else
                    {
                        zapisany.SetPixel(i, j, 255, 255, 255);
                    }
                }
            }
        }
    }
}