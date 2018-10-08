﻿using ImageMagick;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PicView.lib
{
    internal static class ImageManager
    {
        /// <summary>
        /// Decodes image from file to BitMapSource
        /// </summary>
        /// <param name="file">Full path of the file</param>
        /// <param name="extension">file extension beggining with dot</param>
        /// <returns></returns>
        internal static BitmapSource RenderToBitmapSource(string file, string extension)
        {
            if (string.IsNullOrWhiteSpace(file) || file.Length < 2)
                return null;

            BitmapSource pic;

            using (MagickImage magick = new MagickImage())
            {
                // Set values for maximum quality
                magick.Quality = 100;
                magick.ColorSpace = ColorSpace.Transparent;

                var mrs = new MagickReadSettings()
                {
                    Density = new Density(300, 300),
                };

                if (extension.ToLower() == ".svg")
                {
                    // Make background transparent
                    mrs.Format = MagickFormat.Svg;
                    mrs.BackgroundColor = MagickColors.Transparent;
                    try
                    {
                        magick.Read(file, mrs);
                    }
                    catch (MagickException)
                    {
                        return null;
                    }
                }
                else
                {
                    try
                    {
                        magick.Read(file);
                    }
                    catch (MagickException)
                    {
                        return null;
                    }
                }

                pic = magick.ToBitmapSource();
                pic.Freeze();
                return pic;
            }

        }

        internal static BitmapSource GetBitmapSourceThumb(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            switch (ext)
            {
                // Non-standards
                case ".svg":
                case ".psd":
                case ".psb":
                case ".orf":
                case ".cr2":
                case ".crw":
                case ".dng":
                case ".raf":
                case ".raw":
                case ".mrw":
                case ".nef":
                case ".x3f":
                case ".arw":
                case ".webp":
                case ".aai":
                case ".ai":
                case ".art":
                case ".bgra":
                case ".bgro":
                case ".canvas":
                case ".cin":
                case ".cmyk":
                case ".cmyka":
                case ".cur":
                case ".cut":
                case ".dcm":
                case ".dcr":
                case ".dcx":
                case ".dds":
                case ".dfont":
                case ".dlib":
                case ".dpx":
                case ".dxt1":
                case ".dxt5":
                case ".emf":
                case ".epi":
                case ".eps":
                case ".ept":
                case ".ept2":
                case ".ept3":
                case ".exr":
                case ".fax":
                case ".fits":
                case ".flif":
                case ".g3":
                case ".g4":
                case ".gif87":
                case ".gradient":
                case ".gray":
                case ".group4":
                case ".hald":
                case ".hdr":
                case ".hrz":
                case ".icb":
                case ".icon":
                case ".ipl":
                case ".jc2":
                case ".j2k":
                case ".jng":
                case ".jnx":
                case ".jpm":
                case ".jps":
                case ".jpt":
                case ".kdc":
                case ".label":
                case ".map":
                case ".nrw":
                case ".otb":
                case ".otf":
                case ".pbm":
                case ".pcd":
                case ".pcds":
                case ".pcl":
                case ".pct":
                case ".pcx":
                case ".pfa":
                case ".pfb":
                case ".pfm":
                case ".picon":
                case ".pict":
                case ".pix":
                case ".pjpeg":
                case ".png00":
                case ".png24":
                case ".png32":
                case ".png48":
                case ".png64":
                case ".png8":
                case ".pnm":
                case ".ppm":
                case ".ps":
                case ".radialgradient":
                case ".ras":
                case ".rgb":
                case ".rgba":
                case ".rgbo":
                case ".rla":
                case ".rle":
                case ".scr":
                case ".screenshot":
                case ".sgi":
                case ".srf":
                case ".sun":
                case ".svgz":
                case ".tiff64":
                case ".ttf":
                case ".vda":
                case ".vicar":
                case ".vid":
                case ".viff":
                case ".vst":
                case ".vmf":
                case ".wpg":
                case ".xbm":
                case ".xcf":
                case ".yuv":
                    return GetMagickImage(path, 60, 55);

                // Standards
                case ".jpg":
                case ".jpeg":
                case ".jpe":
                case ".png":
                case ".bmp":
                case ".tif":
                case ".tiff":
                case ".gif":
                case ".ico":
                case ".wdp":
                    return Helper.GetWindowsThumbnail(path);

                // Non supported
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns BitmapSource at specified quality and pixel size
        /// </summary>
        /// <param name="file"></param>
        /// <param name="quality"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        internal static BitmapSource GetMagickImage(string file, byte quality, short size)
        {
            BitmapSource pic;

            using (MagickImage magick = new MagickImage())
            {
                magick.Quality = quality;
                magick.ColorSpace = ColorSpace.Transparent;
                try
                {
                    magick.AdaptiveResize(size, size);
                    magick.Read(file);
                }
                catch (MagickException)
                {
                    return null;
                }
                pic = magick.ToBitmapSource();
                pic.Freeze();
                return pic;
            }
        }

        internal static BitmapSource GetMagickImage(Stream s)
        {
            BitmapSource pic;

            using (MagickImage magick = new MagickImage())
            {
                magick.Quality = 100;
                var mrs = new MagickReadSettings()
                {
                    Density = new Density(300)
                };

                magick.Read(s);
                magick.ColorSpace = ColorSpace.Transparent;
                pic = magick.ToBitmapSource();

                pic.Freeze();
                return pic;
            }
        }

        /// <summary>
        /// Tries to save image to the specified destination,
        /// returns false if unsuccesful
        /// </summary>
        /// <param name="rotate">Degrees image is rotated by</param>
        /// <param name="flipped">Whether to flip image or not</param>
        /// <param name="path">The path of the source file</param>
        /// <param name="destination">Where to save image to</param>
        /// <returns></returns>
        internal static bool TrySaveImage(int rotate, bool flipped, string path, string destination)
        {
            if (File.Exists(path))
            {
                try
                {
                    using (var SaveImage = new MagickImage())
                    {
                        // Set maximum quality
                        var mrs = new MagickReadSettings()
                        {
                            Density = new Density(300, 300),
                        };
                        SaveImage.Quality = 100;

                        SaveImage.Read(path, mrs);

                        // Apply transformation values
                        if (flipped)
                            SaveImage.Flop();
                        SaveImage.Rotate(rotate);

                        SaveImage.Write(destination);
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        internal static Size ImageSize (string file)
        {
            using (MagickImage magick = new MagickImage())
            {
                magick.Read(file);
                return new Size(magick.Width, magick.Height);
            }
        }

        internal static void ResizeImage(string Pic, int NewWidth, int NewHeight)
        {
            try
            {
                using (MagickImage magick = new MagickImage(Pic))
                {
                    MagickGeometry size = new MagickGeometry(NewWidth, NewHeight);
                    size.IgnoreAspectRatio = true;
                    magick.Resize(size);
                    magick.Quality = 100;
                    magick.Write(Pic);
                }
            }
            catch (MagickException)
            {
                return;
            }          
        }
    }
}
