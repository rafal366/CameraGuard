using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CameraProject
{
   public class imageFilters
    {
        public enum ColorArea
        {
            Midtones,
            Shadows,
            Highlights
        }

        public static WriteableBitmap Colorize(WriteableBitmap b, int red, int green, int blue, ColorArea ca)
        {
            if (red < -255 || red > 255) throw new ArgumentException("Red must be between -255 and +255");
            if (green < -255 || green > 255) throw new ArgumentException("Green must be between -255 and +255");
            if (blue < -255 || blue > 255) throw new ArgumentException("Blue must be between -255 and +255");

            int stride = b.PixelWidth * 4;  //The pixelformat for brga32 is 32 bits - so that's 4 bytes per pixel
            int bytes = Math.Abs(stride) * b.PixelHeight;

            byte[] p = b.ToByteArray();

            int i = 0;

            int nOffset = stride - b.PixelWidth * 4;

            for (int y = 0; y < b.PixelHeight; ++y)
            {
                for (int x = 0; x < b.PixelWidth; ++x)
                {
                    int pdif = (p[i + 2] + p[i + 1] + p[i]) / 3;

                    int newred = p[i + 2];
                    int newgreen = p[i + 1];
                    int newblue = p[i];

                    switch (ca)
                    {
                        case ColorArea.Shadows:
                            float multi = (1 - newred / 255);
                            newred += (int)(red * multi);

                            multi = (1 - newgreen / 255);
                            newgreen += (int)(green * multi);

                            multi = (1 - newblue / 255);
                            newblue += (int)(blue * multi);
                            break;

                        case ColorArea.Highlights:
                            float hmulti = (newred / 255);
                            newred += (int)(red * hmulti);

                            hmulti = (newgreen / 255);
                            newgreen += (int)(green * hmulti);

                            hmulti = (newblue / 255);
                            newblue += (int)(blue * hmulti);
                            break;

                        case ColorArea.Midtones:
                            float mmulti = 0;

                            mmulti = (newred > 127) ? 127f / newred : newred / 127f;
                            newred += (int)(red * mmulti);

                            mmulti = (newgreen > 127) ? 127f / newgreen : newgreen / 127f;
                            newgreen += (int)(green * mmulti);

                            mmulti = (newblue > 127) ? 127f / newblue : newblue / 127f;
                            newblue += (int)(blue * mmulti);

                            break;
                    }

                    p[i + 2] = (byte)Math.Min(255, Math.Max(newred, 0));
                    p[i + 1] = (byte)Math.Min(255, Math.Max(newgreen, 0));
                    p[i + 0] = (byte)Math.Min(255, Math.Max(newblue, 0));

                    i += 4;
                }
                i += nOffset;
            }

            WriteableBitmap finalImg = new WriteableBitmap(b.PixelWidth, b.PixelHeight);
            return finalImg.FromByteArray(p);
        }

        public static WriteableBitmap SCurve(WriteableBitmap b)
        {
            int stride = b.PixelWidth * 4; //brga32 is 32            
            int bytes = Math.Abs(stride) * b.PixelHeight;

            byte[] p = b.ToByteArray();

            int i = 0;

            int nOffset = stride - b.PixelWidth * 4;

            Point[] points = GetCoordinates();

            for (int y = 0; y < b.PixelHeight; ++y)
            {
                for (int x = 0; x < b.PixelWidth; ++x)
                {
                    int hue = p[i];     //if pi = 255 then 255, otherwise -1   (so if pi = -1,  its ! 255,  NOT max)
                    int hue1 = p[i + 1];
                    int hue2 = p[i + 2];

                    foreach (var point in points)
                    {
                        if (hue != 255 && hue1 != 255 && hue2 != 255)
                        {
                            //white pixel - no processing necessary
                            break;
                        }
                        if (point.X >= p[i + 2] && hue2 != 255)    //translate this to a function for the curve
                            hue2 = (int)point.Y;

                        if (point.X >= p[i + 1] && hue1 != 255)
                            hue1 = (int)point.Y;

                        if (point.X >= p[i] && hue != 255)
                            hue = (int)point.Y;
                    }

                    p[i + 2] = (byte)Math.Min(255, Math.Max(hue2, 0));
                    p[i + 1] = (byte)Math.Min(255, Math.Max(hue1, 0));
                    p[i + 0] = (byte)Math.Min(255, Math.Max(hue, 0));

                    i += 4;
                }
                i += nOffset;
            }


            WriteableBitmap finalImg = new WriteableBitmap(b.PixelWidth, b.PixelHeight);
            return finalImg.FromByteArray(p);



            //return true;
        }
        private static Point[] GetCoordinates()
        {
            List<Point> points = new List<Point>();
            int height = 255;
            int width = 255;

            double y0 = height;
            double y1 = height;
            double y2 = height * 0.75d;
            double y3 = height * 0.5d;

            double x0 = 0;
            double x1 = width * 0.25d;
            double x2 = width * 0.35d;
            double x3 = width * 0.5d;

            for (int i = 0; i < 1000; i++)
            {
                double t = i / 1000d;
                double xt = (-x0 + 3 * x1 - 3 * x2 + x3) * (t * t * t) + 3 * (x0 - 2 * x1 + x2) * (t * t) + 3 * (-x0 + x1) * t + x0;
                double yt = (-y0 + 3 * y1 - 3 * y2 + y3) * (t * t * t) + 3 * (y0 - 2 * y1 + y2) * (t * t) + 3 * (-y0 + y1) * t + y0;


                points.Add(new Point((int)xt, 255 - (int)yt));

            }

            y0 = height * 0.5d;
            y1 = height * 0.25d;
            y2 = 0;
            y3 = 0;

            x0 = width * 0.5d;
            x1 = width * 0.65d;
            x2 = width * 0.75d;
            x3 = width;

            for (int i = 0; i < 1000; i++)
            {
                double t = i / 1000d;

                double xt = (-x0 + 3 * x1 - 3 * x2 + x3) * (t * t * t) + 3 * (x0 - 2 * x1 + x2) * (t * t) + 3 * (-x0 + x1) * t + x0;
                double yt = (-y0 + 3 * y1 - 3 * y2 + y3) * (t * t * t) + 3 * (y0 - 2 * y1 + y2) * (t * t) + 3 * (-y0 + y1) * t + y0;

                points.Add(new Point((int)xt, 255 - (int)yt));
            }
            return points.ToArray();
        }

        public static WriteableBitmap Contrast(WriteableBitmap b, int nContrast)
        {
            if (nContrast < -100 || nContrast > 100)
            {
                throw new ArgumentException("Contrast must be between -100 and 100");
            }

            double contrast = (100.0 + nContrast) / 100.0;
            contrast *= contrast;

            int stride = b.PixelWidth * 4; //brga32 is 32            
            int bytes = Math.Abs(stride) * b.PixelHeight;

            byte[] p = b.ToByteArray();

            int i = 0;

            int nOffset = stride - b.PixelWidth * 4;

            for (int y = 0; y < b.PixelHeight; ++y)
            {
                for (int x = 0; x < b.PixelWidth; ++x)
                {
                    p[i + 2] = ApplyContrast(p[i + 2], contrast);
                    p[i + 1] = ApplyContrast(p[i + 1], contrast);
                    p[i + 0] = ApplyContrast(p[i + 0], contrast);

                    i += 4;
                }
                i += nOffset;
            }

            WriteableBitmap finalImg = new WriteableBitmap(b.PixelWidth, b.PixelHeight);
            return finalImg.FromByteArray(p);
        }

        private static byte ApplyContrast(byte p, double contrast)
        {
            double pixel = p / 255.0;
            pixel -= 0.5;
            pixel *= contrast;
            pixel += 0.5;
            pixel *= 255;
            if (pixel < 0) pixel = 0;
            if (pixel > 255) pixel = 255;
            return (byte)pixel;
        }        
    }
}
