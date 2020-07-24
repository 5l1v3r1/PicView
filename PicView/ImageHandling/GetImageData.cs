﻿using PicView.ChangeImage;
using PicView.Library;
using System;
using System.Globalization;
using System.IO;
using System.Windows;

namespace PicView.ImageHandling
{
    internal static class GetImageData
    {
        internal static string[] RetrieveData(string file)
        {
            var fileInfo = new FileInfo(file);
            var image = Preloader.Load(Navigation.Pics[Navigation.FolderIndex]);

            var inchesWidth = image.PixelWidth / image.DpiX;
            var inchesHeight = image.PixelHeight / image.DpiY;
            var cmWidth = inchesWidth * 2.54;
            var cmHeight = inchesHeight * 2.54;

            var firstRatio = image.PixelWidth / Utilities.GCD(image.PixelWidth, image.PixelHeight);
            var secondRatio = image.PixelHeight / Utilities.GCD(image.PixelWidth, image.PixelHeight);
            string ratioText;
            if (firstRatio == secondRatio)
            {
                ratioText = $"{firstRatio}:{secondRatio} ({Application.Current.Resources["Square"]})";
            }
            else if (firstRatio > secondRatio)
            {
                ratioText = $"{firstRatio}:{secondRatio} ({Application.Current.Resources["Landscape"]})";
            }
            else
            {
                ratioText = $"{firstRatio}:{secondRatio} ({Application.Current.Resources["Portrait"]})";
            }

            return new string[]
            {
                // Fileinfo
                fileInfo.Name,
                fileInfo.DirectoryName,
                fileInfo.FullName,
                fileInfo.CreationTime.ToString(CultureInfo.CurrentCulture),
                fileInfo.LastWriteTime.ToString(CultureInfo.CurrentCulture),

                // Resolution
                image.PixelWidth + " x " + image.PixelHeight + " " + Application.Current.Resources["Pixels"],
                // DPI
                Math.Round(image.DpiX) + " x " + Math.Round(image.DpiY) + Application.Current.Resources["Dpi"],
                // Bit dpeth
                image.Format.BitsPerPixel.ToString(CultureInfo.CurrentCulture),
                // Megapixels
                ((float)image.PixelHeight * image.PixelWidth / 1000000)
                    .ToString("0.##", CultureInfo.CurrentCulture) + " " + Application.Current.Resources["MegaPixels"],
                // Print size cm
                cmWidth.ToString("0.##", CultureInfo.CurrentCulture) + " x " + cmHeight.ToString("0.##", CultureInfo.CurrentCulture)
                    + " " + Application.Current.Resources["Centimeters"],
                // Print size inch
                inchesWidth.ToString("0.##", CultureInfo.CurrentCulture) + " x " + inchesHeight.ToString("0.##", CultureInfo.CurrentCulture)
                    + " " + Application.Current.Resources["Inches"],
                // Aspect ratio
                ratioText
            };
        }
    }
}