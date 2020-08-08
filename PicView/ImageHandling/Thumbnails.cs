﻿using ImageMagick;
using PicView.FileHandling;
using PicView.UILogic.UserControls;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using static PicView.ChangeImage.Navigation;
using static PicView.UILogic.UC;

namespace PicView.ImageHandling
{
    internal static class Thumbnails
    {
        /// <summary>
        /// Returns thumb for common images
        /// </summary>
        /// <param name="x"></param>
        /// <param name="startup"></param>
        /// <returns></returns>
        internal static BitmapSource GetThumb(int x, bool startup)
        {
            if (!startup)
            {
                return GetThumb(x);
            }

            var supported = SupportedFiles.IsSupportedFile(Pics[x]);

            if (!supported.HasValue)
            {
                return null;
            }

            if (supported.Value)
            {
                return GetThumb(x);
            }

            return null;
        }

        /// <summary>
        /// Load thumbnail at provided index
        /// or full image if preloaded.
        /// </summary>
        /// <returns></returns>
        internal static BitmapSource GetThumb(int x)
        {
            BitmapSource pic;

            if (GetPicGallery != null)
            {
                if (GetPicGallery.Container.Children.Count > 0)
                {
                    if (x < GetPicGallery.Container.Children.Count && GetPicGallery.Container.Children.Count == Pics.Count)
                    {
                        var y = GetPicGallery.Container.Children[x] as PicGalleryItem;
                        pic = (BitmapSource)y.img.Source;
                    }
                    else
                    {
                        pic = GetBitmapSourceThumb(Pics[x]);
                    }
                }
                else
                {
                    pic = GetBitmapSourceThumb(Pics[x]);
                }
            }
            else
            {
                pic = GetBitmapSourceThumb(Pics[x]);
            }

            if (pic == null)
            {
                return null;
            }

            if (!pic.IsFrozen)
            {
                pic.Freeze();
            }

            return pic;
        }

        internal static BitmapSource GetBitmapSourceThumb(string path) => (Path.GetExtension(path).ToLower(CultureInfo.CurrentCulture)) switch
        {
            // Return windows thumbnails if applicable, else use imagemagick
            ".jpg" or ".jpeg" or ".jpe" or ".png" or ".bmp" or ".gif" or ".ico" => GetWindowsThumbnail(path),
            _ => GetMagickImageThumb(path),
        };

        /// <summary>
        /// Returns BitmapSource at specified quality and pixel size
        /// </summary>
        /// <param name="file"></param>
        /// <param name="quality"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private static BitmapSource GetMagickImageThumb(string file, byte quality = 100, short size = 500)
        {
            BitmapSource pic;

            using MagickImage magick = new MagickImage
            {
                Quality = quality,
                ColorSpace = ColorSpace.Transparent
            };
            try
            {
                magick.Read(file);
                magick.AdaptiveResize(size, size);
            }
#if DEBUG
            catch (MagickException e)
            {
                Trace.WriteLine("GetMagickImage returned " + file + " null, \n" + e.Message);
                return null;
            }
#else
                catch (MagickException) { return null; }
#endif
            pic = magick.ToBitmapSource();
            pic.Freeze();
            return pic;
        }

        /// <summary>
        /// Returns a Windows Thumbnail
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <returns></returns>
        private static BitmapSource GetWindowsThumbnail(string path, bool extralarge = false)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            if (extralarge)
            {
                return Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(path).Thumbnail.ExtraLargeBitmapSource;
            }

            return Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(path).Thumbnail.BitmapSource;
        }
    }
}