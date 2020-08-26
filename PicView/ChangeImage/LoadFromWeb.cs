﻿using PicView.FileHandling;
using PicView.UILogic;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static PicView.ChangeImage.Error_Handling;
using static PicView.ChangeImage.Navigation;
using static PicView.ImageHandling.ImageDecoder;
using static PicView.UILogic.SetTitle;
using static PicView.UILogic.Tooltip;

namespace PicView.ChangeImage
{
    internal static class LoadFromWeb
    {
        /// <summary>
        /// Attemps to download image and display it
        /// </summary>
        /// <param name="path"></param>
        internal static async void PicWeb(string path)
        {
            ConfigureWindows.GetMainWindow.TitleText.Text = Application.Current.Resources["Loading"] as string;

            BitmapSource pic;
            if (Pics != null && Pics.Count > 0)
            {
                BackupPath = Pics[FolderIndex];
            }

            try
            {
                pic = await LoadImageWebAsync(path).ConfigureAwait(true);
            }
            catch (Exception e)
            {
#if DEBUG
                Trace.WriteLine("PicWeb caught exception, message = " + e.Message);
#endif
                ShowTooltipMessage(e.Message, true);
                pic = null;
            }

            if (pic == null)
            {
                Reload(true);
                return;
            }

            Pic(pic, path);
            RecentFiles.Add(path);
            CanNavigate = false;

            // Fix not having focus after drag and drop
            if (!ConfigureWindows.GetMainWindow.IsFocused)
            {
                ConfigureWindows.GetMainWindow.Focus();
            }
        }

        /// <summary>
        /// Downloads image from web and returns as BitmapSource
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        internal static async Task<BitmapSource> LoadImageWebAsync(string address)
        {
            CanNavigate = false;
            BitmapSource pic = null;
            await Task.Run(async () =>
            {
                var client = new WebClient();
                client.DownloadProgressChanged += (sender, e) =>
                ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    ConfigureWindows.GetMainWindow.Title = ConfigureWindows.GetMainWindow.TitleText.Text =
                        $"{e.BytesReceived} / {e.TotalBytesToReceive} {e.ProgressPercentage} {Application.Current.Resources["PercentComplete"]}";
                    ConfigureWindows.GetMainWindow.TitleText.ToolTip = ConfigureWindows.GetMainWindow.Title;
                }));
                client.DownloadDataCompleted += (sender, e) =>
                ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    if (pic != null)
                    {
                        SetTitleString(pic.PixelWidth, pic.PixelHeight, address);
                    }

                    CanNavigate = false;
                }));

                var bytes = await client.DownloadDataTaskAsync(new Uri(address)).ConfigureAwait(true);
                pic = GetMagickImage(bytes);
            }).ConfigureAwait(true);
            return pic;
        }
    }
}