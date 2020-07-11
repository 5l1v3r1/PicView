﻿using PicView.FileHandling;
using PicView.ImageHandling;
using PicView.SystemIntegration;
using PicView.UI;
using PicView.UI.PicGallery;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using static PicView.ChangeImage.Error_Handling;
using static PicView.FileHandling.ArchiveExtraction;
using static PicView.FileHandling.FileLists;
using static PicView.ImageHandling.Thumbnails;
using static PicView.Library.Fields;
using static PicView.UI.SetTitle;
using static PicView.UI.Sizing.ScaleImage;
using static PicView.UI.Tooltip;
using static PicView.UI.TransformImage.Scroll;
using static PicView.UI.UserControls.UC;

namespace PicView.ChangeImage
{
    internal static class Navigation
    {
        /// <summary>
        /// List of file paths to supported files
        /// </summary>
        internal static System.Collections.Generic.List<string> Pics { get; set; }

        /// <summary>
        /// Counter used to get/set current index
        /// </summary>
        internal static int FolderIndex { get; set; }


        #region Update Image values

        /// <summary>
        /// Loads a picture from a given file path and does extra error checking
        /// </summary>
        /// <param name="path"></param>
        internal static async void Pic(string path)
        {
            // Set Loading
            TheMainWindow.Title = TheMainWindow.TitleText.Text = Application.Current.Resources["Loading"] as string;
            TheMainWindow.TitleText.ToolTip = Application.Current.Resources["Loading"] as string;

            // Handle if from web
            if (!File.Exists(path))
            {
                if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
                {
                    LoadFromWeb.PicWeb(path);
                    return;
                }
                else if (Directory.Exists(path))
                {
                    ChangeFolder(true);
                    GetValues(path);
                }
                else
                {
                    Unload();
                    return;
                }
            }

            // If count not correct or just started, get values
            if (Pics.Count <= FolderIndex || FolderIndex < 0 || FreshStartup)
            {
                GetValues(path);
            }
            // If the file is in the same folder, navigate to it. If not, start manual loading procedure.
            else if (!string.IsNullOrWhiteSpace(Pics[FolderIndex]) && Path.GetDirectoryName(path) != Path.GetDirectoryName(Pics[FolderIndex]))
            {
                // Reset old values and get new
                ChangeFolder(true);
                GetValues(path);
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
                        ShowTooltipMessage("Archive could not be processed"); // TODO add to translation
                        Reload(true);
                        return;
                    }
                    else
                    {
                        TheMainWindow.TitleText.Text = "Unzipping..."; // TODO add translation
                        TheMainWindow.TitleText.ToolTip = TheMainWindow.TitleText.Text;
                    }
                    TheMainWindow.Focus();
                }
            }
            else
            {
                Reload(true);
                return;
            }

            if (!FreshStartup)
            {
                Preloader.Clear();
            }

#if DEBUG
            if (FreshStartup)
                Trace.WriteLine("Pic(string path) entering Pic(int x)");
#endif

            // Navigate to picture using obtained index
            Pic(FolderIndex);

            // Load new gallery values, if changing folder
            if (GetPicGallery != null && Properties.Settings.Default.PicGallery == 2 && !GalleryFunctions.IsLoading)
            {
                await GalleryLoad.Load().ConfigureAwait(false);
            }

            prevPicResource = null; // Make sure to not waste memory
        }

        /// <summary>
        /// Loads image at specified index
        /// </summary>
        /// <param name="index">The index of file to load from Pics</param>
        internal static async void Pic(int index)
        {
#if DEBUG
            var stopWatch = new Stopwatch();
            stopWatch.Start();
#endif
            // Declare variable to be used to set image source
            BitmapSource bitmapSource;

            // Error checking to fix rare cases of crashing
            if (Pics.Count < index)
            {
                bitmapSource = await PicErrorFix(index).ConfigureAwait(true);
                if (bitmapSource == null)
                {
                    /// Try to recover
                    /// TODO needs testing
                    Reload(true);
                    return;
                }
            }
            else if (File.Exists(Pics[index])) // Checking if file exists fixes rare crashes
            {
                /// Use the Load() function load image from memory if available
                /// if not, it will be null
                bitmapSource = Preloader.Load(Pics[index]);
            }
            else
            {
                /// Try to reload from backup if file does not exist
                /// TODO needs testing
                Reload(true);
                return;
            }

            // Initate loading behavior, if needed
            if (bitmapSource == null)
            {
                // Set loading from translation service
                TheMainWindow.Title = Application.Current.Resources["Loading"] as string;
                TheMainWindow.TitleText.Text = Application.Current.Resources["Loading"] as string;
                TheMainWindow.TitleText.ToolTip = Application.Current.Resources["Loading"] as string;

                // Show a thumbnail while loading
                var thumb = GetThumb(index, true);
                if (thumb != null)
                {
                    TheMainWindow.MainImage.Source = thumb;
                }

                // Dissallow changing image while loading
                CanNavigate = false;

                // Get it!
                await Preloader.Add(Pics[index]).ConfigureAwait(true);

                if (bitmapSource == null)
                {
                    // Attempt to fix it
                    bitmapSource = await PicErrorFix(index).ConfigureAwait(true);

                    // If pic is still null, image can't be rendered
                    if (bitmapSource == null)
                    {
                        // Clean up
                        Pics.RemoveAt(index);
                        Preloader.Remove(index);

                        // Sync with gallery, if needed
                        if (GetPicGallery != null)
                        {
                            if (GetPicGallery.grid.Children.Count > index)
                            {
                                GetPicGallery.grid.Children.RemoveAt(index);
                            }
                        }

                        // Check if images still exists
                        if (Pics.Count == 0)
                        {
                            Unload();
                            return;
                        }

                        /// Retry
                        /// TODO needs testing
                        CanNavigate = true;
                        Pic();
                        return;
                    }
                }
            }

            // Reset transforms if needed
            if (UI.TransformImage.Rotation.Flipped || UI.TransformImage.Rotation.Rotateint != 0)
            {
                UI.TransformImage.Rotation.Flipped = false;
                UI.TransformImage.Rotation.Rotateint = 0;

                TheMainWindow.MainImage.LayoutTransform = null;
            }

            // Show the image! :)
            TheMainWindow.MainImage.Source = bitmapSource;
            FitImage(bitmapSource.PixelWidth, bitmapSource.PixelHeight);
            SetTitleString(bitmapSource.PixelWidth, bitmapSource.PixelHeight, index);

            // Scroll to top if scroll enabled
            if (IsScrollEnabled)
            {
                TheMainWindow.Scroller.ScrollToTop();
            }

            // Update values
            CanNavigate = true;
            FolderIndex = index;

            if (Pics.Count > 1)
            {
                Taskbar.Progress(index, Pics.Count);

                // Preload images \\
                if (Preloader.StartPreload())
                {
                    await Preloader.PreLoad(index).ConfigureAwait(false);
                }
            }

            if (!FreshStartup)
            {
                RecentFiles.Add(Pics[index]);

                if (prevPicResource != null)
                {
                    prevPicResource = null;
                }
            }

            FreshStartup = false;
#if DEBUG
            stopWatch.Stop();
            var s = $"Pic(); executed in {stopWatch.Elapsed.TotalMilliseconds} milliseconds";
            Trace.WriteLine(s);
#endif
        }

        /// <summary>
        /// Load a picture from a prepared bitmap
        /// </summary>
        /// <param name="pic"></param>
        /// <param name="imageName"></param>
        internal static void Pic(BitmapSource bitmap, string imageName)
        {
            /// Old method, might need updates?

            Unload();

            if (IsScrollEnabled)
            {
                TheMainWindow.Scroller.ScrollToTop();
            }

            TheMainWindow.MainImage.Source = bitmap;

            FitImage(bitmap.PixelWidth, bitmap.PixelHeight);
            CloseToolTipMessage();

            SetTitleString(bitmap.PixelWidth, bitmap.PixelHeight, imageName);

            Taskbar.NoProgress();

            CanNavigate = false;
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
                TheMainWindow.Scroller.ScrollToTop();
            }

            TheMainWindow.MainImage.Source = pic;

            FitImage(pic.PixelWidth, pic.PixelHeight);
            CloseToolTipMessage();

            SetTitleString(pic.PixelWidth, pic.PixelHeight, "Base64 image"); // TODO add to translation

            Taskbar.NoProgress();

            CanNavigate = false;
        }

        /// <summary>
        /// Handle logic if user wants to load from a folder
        /// </summary>
        /// <param name="folder"></param>
        internal static void PicFolder(string folder)
        {
            ChangeFolder(true);
            Pics = FileList(folder);

            Pic(Pics[0]);

            GetQuickSettingsMenu.GoToPic.GoToPicBox.Text = (FolderIndex + 1).ToString(CultureInfo.CurrentCulture);

            prevPicResource = null; // Make sure to not waste memory
        }

        #endregion Update Image values

        #region Change navigation values

        /// <summary>
        /// Goes to next, previous, first or last file in folder
        /// </summary>
        /// <param name="next">Whether it's forward or not</param>
        /// <param name="end">Whether to go to last or first,
        /// depending on the next value</param>
        internal static void Pic(bool next = true, bool end = false)
        {
            // Exit if not intended to change picture
            if (!CanNavigate)
            {
                return;
            }

            // exit if browsing PicGallery
            if (GetPicGallery != null)
            {
                if (Properties.Settings.Default.PicGallery == 1)
                {
                    if (GalleryFunctions.IsOpen)
                    {
                        return;
                    }
                }
            }

            // Make backup
            var indexBackup = FolderIndex;

            if (!end) // Go to next or previous
            {
                if (next)
                {
                    // loop next
                    if (Properties.Settings.Default.Looping || Slideshow.SlideTimer != null && Slideshow.SlideTimer.Enabled)
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
                    Reverse = false;
                }
                else
                {
                    // Loop prev
                    if (Properties.Settings.Default.Looping || Slideshow.SlideTimer != null && Slideshow.SlideTimer.Enabled)
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
                    Reverse = true;
                }
            }
            else // Go to first or last
            {
                FolderIndex = next ? Pics.Count - 1 : 0;
                indexBackup = FolderIndex;

                // Reset preloader values to prevent errors
                if (Pics.Count > 20)
                {
                    Preloader.Clear();
                }

                PreloadCount = 4;
            }

            // Go to the image!
            Pic(FolderIndex);

            // Update PicGallery selected item, if needed
            if (GetPicGallery != null)
            {
                if (GetPicGallery.Container.Children.Count > FolderIndex && GetPicGallery.Container.Children.Count > indexBackup)
                {
                    if (indexBackup != FolderIndex)
                    {
                        GalleryFunctions.SetUnselected(indexBackup);
                    }

                    GalleryFunctions.SetSelected(FolderIndex);
                    GalleryScroll.ScrollTo();
                }
                else
                {
                    // TODO Find way to get PicGalleryItem an alternative way...
                }
            }

            CloseToolTipMessage();
        }

        /// <summary>
        /// Extra functionality and error checking when clicking
        /// on the navigation buttons
        /// </summary>
        /// <param name="arrow"></param>
        /// <param name="right"></param>
        internal static void PicButton(bool arrow, bool right)
        {
            if (!arrow) // Normal buttons
            {
                if (GalleryFunctions.IsOpen)
                {
                    GalleryScroll.ScrollTo(!right);
                    return;
                }

                if (!CanNavigate)
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
            else // Alternative interface buttons
            {
                if (!CanNavigate)
                {
                    return;
                }

                if (right)
                {
                    ClickArrowRightClicked = true;
                    Pic();
                }
                else
                {
                    ClickArrowLeftClicked = true;
                    Pic(false, false);
                }
            }
        }

        /// <summary>
        /// Only load thumb without resizing
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

            TheMainWindow.MainImage.Width = xWidth;
            TheMainWindow.MainImage.Height = xHeight;

            TheMainWindow.TitleText.ToolTip =
            TheMainWindow.Title =
            TheMainWindow.TitleText.Text = "Image " + (FolderIndex + 1) + " of " + Pics.Count; // TODO add to translation

            var thumb = GetThumb(FolderIndex);

            if (thumb != null)
            {
                TheMainWindow.MainImage.Source = thumb;
            }

            Taskbar.Progress(FolderIndex, Pics.Count);
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

            FastPicRunning = false;

            if (!Preloader.Contains(Pics[FolderIndex]))
            {
                PreloadCount = 0;
                Preloader.Clear();
            }

            Pic(FolderIndex);
        }

        #endregion Change navigation values
    }
}