﻿using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using AForge.Video.FFMPEG;

namespace TimeLapseTool.Helpers
{
    public static class VideoHelper
    {
        public static void CreateTimeLapse(string outputFilePath, int width, int height, int frameRate, bool orderByFileDate, string[] fileList)
        {
            try
            {
                using (var videoWriter = new VideoFileWriter())
                {
                    videoWriter.Open(outputFilePath, width, height, frameRate, VideoCodec.MPEG4, 1000000);

                    foreach (string file in fileList)
                    {
                        // Out of Memory errors are common for incomplete or corrupted images.  Skip over them and continue.
                        try
                        {
                            using (Bitmap image = Image.FromFile(file) as Bitmap)
                            {
                                if (image != null)
                                {
                                    Bitmap bm = ResizeImage(image, width, height);
                                    videoWriter.WriteVideoFrame(bm);
                                }
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    videoWriter.Close();
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
