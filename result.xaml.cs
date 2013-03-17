using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace CameraGuard
{
    public partial class result : PhoneApplicationPage
    {
        
        public result()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
/*
            WriteableBitmap bitmap = (WriteableBitmap)PhoneApplicationService.Current.State["obraz"];
          //  bitmap.
            bitmap.Invert();
       
            WriteableBitmap b1 = new WriteableBitmap(640, 480);
            byte[] b = bitmap.ToByteArray();
            b1.FromByteArray(b);
            if (bitmap != null)
            {
                resultImage.Source = bitmap;
            }
*/

            base.OnNavigatedTo(e);
            
        }

       
    }
}