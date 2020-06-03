﻿using PicView.PreLoading;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static PicView.AjaxLoader;
using static PicView.ArchiveExtraction;
using static PicView.Error_Handling;
using static PicView.Fields;
using static PicView.FileLists;
using static PicView.ImageDecoder;
using static PicView.Resize_and_Zoom;
using static PicView.Scroll;
using static PicView.SetTitle;
using static PicView.Thumbnails;
using static PicView.Tooltip;
using static PicView.Utilities;

namespace PicView
{
    internal static class Navigation
    {
        #region Update Pic
        /// <summary>
        /// Loads a picture from a given file path and does extra error checking
        /// </summary>
        /// <param name="path"></param>
        internal static async void Pic(string path)
        {
            // Set Loading
            mainWindow.Title = mainWindow.Bar.Text = Loading;
            mainWindow.Bar.ToolTip = Loading;
            if (mainWindow.img.Source == null)
            {
                AjaxLoadingStart();
            }

            // Handle if from web
            if (!File.Exists(path))
            {
                if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
                {
                    LoadFromWeb.PicWeb(path);
                }
                else
                {
                    Unload();
                }

                return;
            }

            // If count not correct or just started, get values
            if (Pics.Count <= FolderIndex || FolderIndex < 0 || freshStartup)
            {
                await GetValues(path).ConfigureAwait(true);
            }
            // If the file is in the same folder, navigate to it. If not, start manual loading procedure.
            else if (!string.IsNullOrWhiteSpace(Pics[FolderIndex]) && Path.GetDirectoryName(path) != Path.GetDirectoryName(Pics[FolderIndex]))
            {
                //// Reset zipped values
                //if (!string.IsNullOrWhiteSpace(TempZipPath))
                //{
                //    DeleteTempFiles();
                //    TempZipPath = string.Empty;
                //    RecentFiles.SetZipped(string.Empty, false);
                //}

                // Reset old values and get new
                ChangeFolder(true);
                await GetValues(path).ConfigureAwait(true);
            }

            // If no need to reset values, get index
            else if (Pics != null)
            {
                FolderIndex = Pics.IndexOf(path);
            }

            if (Pics != null)
            {
                // Fix large archive extraction error
                if (Pics.Count == 0)
                {
                    var recovery = await RecoverFailedArchiveAsync().ConfigureAwait(true);
                    if (!recovery)
                    {
                        ToolTipStyle("Archive could not be processed");
                        Reload(true);
                        return;
                    }
                    else
                    {
                        mainWindow.Bar.Text = "Unzipping...";
                        mainWindow.Bar.ToolTip = mainWindow.Bar.Text;
                    }
                    mainWindow.Focus();
                }
            }
            else
            {
                Reload(true);
                return;
            }

            if (!freshStartup)
            {
                Preloader.Clear();
            }

            // Navigate to picture using obtained index
            Pic(FolderIndex);

            if (Pics.Count > 1)
            {
                if (!GalleryMisc.IsLoading)
                {
                    await GalleryLoad.Load().ConfigureAwait(true);
                }
            }

            prevPicResource = null; // Make sure to not waste memory

            AjaxLoadingEnd();
        }

        /// <summary>
        /// Loads image based on overloaded int.
        /// </summary>
        /// <param name="x">The index of file to load from Pics</param>
        internal static async void Pic(int x)
        {
            BitmapSource pic;

            // Clear unsupported image window, if shown
            if (unsupported)
            {
                mainWindow.topLayer.Children.Remove(badImage);
                badImage = null;
                unsupported = false;
            }

            // Additional error checking
            if (x == -1)
            {
                await GetValues(Pics[0]).ConfigureAwait(true);
            }
            
            if (Pics.Count <= x)
            {
                pic = await PicErrorFix(x).ConfigureAwait(true);
                if (pic == null)
                {
                    Reload(true);
                    return;
                }
            }
            else
            {
                /// Add "pic" as local variable used for the image.
                /// Use the Load() function load image from memory if available
                /// if not, it will be null
                pic = Preloader.Load(Pics[x]);
            }

            if (pic == null)
            {
                mainWindow.Title = Loading;
                mainWindow.Bar.Text = Loading;
                mainWindow.Bar.ToolTip = Loading;               

                var thumb = GetThumb(x, true);

                if (thumb != null)
                {
                    mainWindow.img.Source = thumb;
                }

                // Dissallow changing image while loading
                canNavigate = false;

                if (freshStartup)
                {
                    TryZoomFit(Pics[x]);

                    // Load new value manually
                    pic = await RenderToBitmapSource(Pics[x]).ConfigureAwait(true);
                }
                else
                {
                    AjaxLoadingStart();

                    do
                    {
                        // Try again while loading                                             
                        pic = Preloader.Load(Pics[x]);
                        await Task.Delay(50).ConfigureAwait(true);
                    } while (Preloader.IsLoading);

                    AjaxLoadingEnd();
                }

                // If pic is still null, image can't be rendered
                if (pic == null)
                {
                    // Attempt to load new image
                    pic = await PicErrorFix(x).ConfigureAwait(true);
                    if (pic == null)
                    {
                        if (Pics.Count <= 1)
                        {
                            Unload();
                            return;
                        }

                        DisplayBrokenImage();
                        canNavigate = true;
                        return;
                    }
                }
            }

            // Show the image! :)
            mainWindow.img.Source = pic;

            ZoomFit(pic.PixelWidth, pic.PixelHeight);

            // Scroll to top if scroll enabled
            if (IsScrollEnabled)
            {
                mainWindow.Scroller.ScrollToTop();
            }

            if (Flipped)
               Rotate_and_Flip.Flip();

            // Update values
            canNavigate = true;
            SetTitleString(pic.PixelWidth, pic.PixelHeight, x);
            FolderIndex = x;

            if (Pics.Count > 1)
            {
                Progress(x, Pics.Count);

                // Preload images \\
                if (Preloader.StartPreload())
                {
                    if (!Preloader.Contains(Pics[x]))
                    {
                        Preloader.Add(pic, Pics[x]);
                    }

                    await Preloader.PreLoad(x).ConfigureAwait(false);
                }
            }

            if (!freshStartup)
            {
                RecentFiles.Add(Pics[x]);

                if (prevPicResource != null)
                {
                    prevPicResource = null;
                }
            }
            
            freshStartup = false;
        }

        /// <summary>
        /// Load a picture from a prepared bitmap
        /// </summary>
        /// <param name="pic"></param>
        /// <param name="imageName"></param>
        internal static void Pic(BitmapSource pic, string imageName)
        {
            /// Old method, might need updates?

            Unload();

            if (IsScrollEnabled)
            {
                mainWindow.Scroller.ScrollToTop();
            }

            mainWindow.img.Source = pic;

            ZoomFit(pic.PixelWidth, pic.PixelHeight);
            CloseToolTipStyle();

            SetTitleString(pic.PixelWidth, pic.PixelHeight, imageName);

            NoProgress();

            canNavigate = false;
        }

        /// <summary>
        /// Load a picture from a base64
        /// </summary>
        /// <param name="pic"></param>
        /// <param name="imageName"></param>
        internal static async void Pic64(string base64string)
        {
            var pic = await Base64.Base64StringToBitmap(base64string).ConfigureAwait(true);

            Unload();

            if (IsScrollEnabled)
            {
                mainWindow.Scroller.ScrollToTop();
            }

            mainWindow.img.Source = pic;

            ZoomFit(pic.PixelWidth, pic.PixelHeight);
            CloseToolTipStyle();

            SetTitleString(pic.PixelWidth, pic.PixelHeight, "Base64 image");

            NoProgress();

            canNavigate = false;
        }

        #endregion

        #region Change Image

        /// <summary>
        /// Goes to next, previous, first or last file in folder
        /// </summary>
        /// <param name="next">Whether it's forward or not</param>
        /// <param name="end">Whether to go to last or first,
        /// depending on the next value</param>
        internal static void Pic(bool next = true, bool end = false)
        {

#if DEBUG
            var stopWatch = new Stopwatch();
            stopWatch.Start();
#endif

            // Exit if not intended to change picture
            if (!canNavigate)
            {
                return;
            }

            // exit if browsing PicGallery
            if (picGallery != null)
            {
                if (Properties.Settings.Default.PicGallery == 1)
                {
                    if (GalleryMisc.IsOpen)
                    {
                        return;
                    }
                }
            }

            // Make backup
            var x = FolderIndex;

            // Go to first or last
            if (end)
            {
                FolderIndex = next ? Pics.Count - 1 : 0;
                x = FolderIndex;

                // Reset preloader values to prevent errors
                if (Pics.Count > 20)
                {
                    Preloader.Clear();
                }

                PreloadCount = 4;
            }
            // Go to next or previous
            else
            {
                if (next)
                {
                    // loop next
                    if (Properties.Settings.Default.Looping)
                    {
                        FolderIndex = FolderIndex == Pics.Count - 1 ? 0 : FolderIndex + 1;
                    }
                    else
                    {
                        // Go to next if able
                        if (FolderIndex + 1 == Pics.Count)
                        {
                            return;
                        }

                        FolderIndex++;
                    }

                    PreloadCount++;
                    reverse = false;
                }
                else
                {
                    // Loop prev
                    if (Properties.Settings.Default.Looping)
                    {
                        FolderIndex = FolderIndex == 0 ? Pics.Count - 1 : FolderIndex - 1;
                    }
                    else
                    {
                        // Go to prev if able
                        if (FolderIndex - 1 < 0)
                        {
                            return;
                        }

                        FolderIndex--;
                    }

                    PreloadCount--;
                    reverse = true;
                }
            }

            // Go to the image!
            Pic(FolderIndex);

            // Update PicGallery selected item, if needed
            if (picGallery != null)
            {
                if (picGallery.Container.Children.Count > FolderIndex && picGallery.Container.Children.Count > x)
                {
                    if (x != FolderIndex)
                    {
                        GalleryMisc.SetUnselected(x);
                    }

                    GalleryMisc.SetSelected(FolderIndex);
                    GalleryScroll.ScrollTo();
                }
                else
                {
                    // TODO Find way to get PicGalleryItem an alternative way...
                }
            }

#if DEBUG
            stopWatch.Stop();
            var s = "Pic(); executed in " + stopWatch.Elapsed.TotalMilliseconds + " milliseconds";
            Trace.WriteLine(s);
            //ToolTipStyle(s);
#endif
        }

        internal static void PicButton(bool arrow , bool right)
        {
            if (arrow)
            {
                if (!canNavigate)
                {
                    return;
                }

                if (right)
                {
                    clickArrowRightClicked = true;
                    Pic();
                }
                else
                {
                    clickArrowLeftClicked = true;
                    Pic(false, false);
                }
            }
            else
            {
                if (GalleryMisc.IsOpen)
                {
                    GalleryScroll.ScrollTo(!right);
                    return;
                }

                if (!canNavigate)
                {
                    return;
                }

                if (right)
                {
                    RightbuttonClicked = true;
                    Pic();
                }
                else
                {
                    LeftbuttonClicked = true;
                    Pic(false, false);
                }
            }
        }

        /// <summary>
        /// Only load image without resizing
        /// </summary>
        /// <param name="forwards">The direction</param>
        internal static void FastPic(bool forwards)
        {
            /// TODO FastPic Changes...
            /// Need solution for slowing down this thing to something useful
            /// await task delay only works once, it seems
            /// Timers doesn't deliver a proper result in my experience
            /// 

            FastPicRunning = true;

            if (forwards)
            {
                if (FolderIndex == Pics.Count - 1)
                {
                    FolderIndex = 0;
                }
                else
                {
                    FolderIndex++;
                }
            }
            else
            {
                if (FolderIndex == 0)
                {
                    FolderIndex = Pics.Count - 1;
                }
                else
                {
                    FolderIndex--;
                }
            }

            mainWindow.img.Width = xWidth;
            mainWindow.img.Height = xHeight;

            mainWindow.Bar.ToolTip =
            mainWindow.Title =
            mainWindow.Bar.Text = "Image " + (FolderIndex + 1) + " of " + Pics.Count;

            var thumb = GetThumb(FolderIndex);

            if (thumb != null)
            {
                mainWindow.img.Source = thumb;
            }

            Progress(FolderIndex, Pics.Count);
        }

        /// <summary>
        /// Update after FastPic() was used
        /// </summary>
        internal static void FastPicUpdate()
        {
            if (!FastPicRunning)
            {
                return;
            }

            //fastPicTimer.Stop();
            FastPicRunning = false;

            if (!Preloader.Contains(Pics[FolderIndex]))
            {
                PreloadCount = 0;
                Preloader.Clear();
            }

            Pic(FolderIndex);
        }

        #endregion
    }
}
