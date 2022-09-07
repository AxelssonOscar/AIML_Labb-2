using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AI_Labb_2.Utils
{
    static class Utilities
    {
        static public Random rand = new Random();
        static public float Lerp(float first, float second, float amount)
        {
            return first + amount * (second - first);
        }

        static public byte Lerp(byte first, byte second, byte amount)
        {
            return (byte)(first + amount * (second - first));
        }

        static public List<Color> RandomColorsInList(int num)
        {
            List<Color> list = new List<Color>();
            for(int i = 0; i < num; i++)
            {
                list.Add(new Color(rand.Next(255), rand.Next(255), rand.Next(255), 255));
            }
            return list;
        }

        static int filenr = 0;


#pragma warning disable CA1416 // Validate platform compatibility
        static public void ResizeImageToThumbnail(string filepath, string filename)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            var srcImage = System.Drawing.Image.FromFile(filepath);
            
            var newWidth = (int)(srcImage.Width * 0.5);
            var newHeight = (int)(srcImage.Height * 0.5);
            var newImage = new System.Drawing.Bitmap(newWidth, newHeight);
            var graphics = System.Drawing.Graphics.FromImage(newImage);

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphics.DrawImage(srcImage, new System.Drawing.Rectangle(0, 0, newWidth, newHeight));

            newImage.Save("thumbnail" + filename, System.Drawing.Imaging.ImageFormat.Png);


            watch.Stop();
            Console.WriteLine("Execution Time: " + watch.ElapsedMilliseconds + "ms");
        }

        static public bool ResizeImage(string filepath, string filename)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var srcImage = System.Drawing.Image.FromFile(filepath);

            int max = Math.Max(srcImage.Width, srcImage.Height);

            if (max > 800)
            {
                float min = Math.Min((float)(800 / (float)srcImage.Width), (float)(800 / (float)srcImage.Height));
                 

                var newWidth = (int)(srcImage.Width * min);
                var newHeight = (int)(srcImage.Height * min);
                var newImage = new System.Drawing.Bitmap(newWidth, newHeight);
                var graphics = System.Drawing.Graphics.FromImage(newImage);

                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.DrawImage(srcImage, new System.Drawing.Rectangle(0, 0, newWidth, newHeight));

                newImage.Save("resized" + filename, System.Drawing.Imaging.ImageFormat.Png);

                return true;
            }
            else
            {
                watch.Stop();
                Console.WriteLine("No image resize. Execution Time: " + watch.ElapsedMilliseconds + "ms");
                return false;
            }

        }
#pragma warning restore CA1416 // Validate platform compatibility


        



    }
}
