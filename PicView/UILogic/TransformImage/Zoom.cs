﻿using PicView.UILogic.Loading;
using PicView.UILogic.PicGallery;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static PicView.ChangeImage.Navigation;
using static PicView.Library.Utilities;

namespace PicView.UILogic.TransformImage
{
    internal static class ZoomLogic
    {
        private static ScaleTransform scaleTransform;
        private static TranslateTransform translateTransform;
        private static Point origin;
        private static Point start;

        /// Used to determine final point when zooming,
        /// since DoubleAnimation changes value of
        /// TranslateTransform continuesly.
        internal static double ZoomValue { get; set; }

        internal const int zoomSpeed = 45;

        /// <summary>
        /// Returns zoom percentage. if 100%, return empty string
        /// </summary>
        internal static string ZoomPercentage
        {
            get
            {
                if (scaleTransform == null || ZoomValue <= 1)
                {
                    return string.Empty;
                }

                var zoom = Math.Round(ZoomValue * 100);

                return zoom + "%";
            }
        }

        /// <summary>
        /// Returns aspect ratio as a formatted string
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        internal static string StringAspect(int width, int height)
        {
            var gcd = GCD(width, height);
            var x = width / gcd;
            var y = height / gcd;

            if (x == width && y == height || x > 16 || y > 9)
            {
                return ") ";
            }

            return ", " + x + ":" + y + ") ";
        }

        /// <summary>
        /// Manipulates the required elements to allow zooming
        /// by modifying ScaleTransform and TranslateTransform
        /// </summary>
        internal static void InitializeZoom()
        {
            // Initialize transforms
            LoadWindows.GetMainWindow.MainImage.RenderTransform = new TransformGroup
            {
                Children = new TransformCollection {
                            new ScaleTransform(),
                            new TranslateTransform()
                        }
            };

            LoadWindows.GetMainWindow.Scroller.ClipToBounds = LoadWindows.GetMainWindow.MainImage.ClipToBounds = true;

            // Set transforms to UI elements
            scaleTransform = (ScaleTransform)((TransformGroup)LoadWindows.GetMainWindow.MainImage.RenderTransform).Children.First(tr => tr is ScaleTransform);
            translateTransform = (TranslateTransform)((TransformGroup)LoadWindows.GetMainWindow.MainImage.RenderTransform).Children.First(tr => tr is TranslateTransform);
        }

        internal static void PreparePanImage(object sender, MouseButtonEventArgs e)
        {
            // Report position for image drag
            LoadWindows.GetMainWindow.MainImage.CaptureMouse();
            start = e.GetPosition(LoadWindows.GetMainWindow.MainImageBorder);
            origin = new Point(translateTransform.X, translateTransform.Y);
        }

        internal static void PanImage(object sender, MouseEventArgs e)
        {
            // Don't drag when full scale
            // and don't drag it if mouse not held down on image
            if (!LoadWindows.GetMainWindow.MainImage.IsMouseCaptured || scaleTransform.ScaleX == 1)
            {
                return;
            }

            // Drag image by modifying X,Y coordinates
            var dragMousePosition = start - e.GetPosition(LoadWindows.GetMainWindow);

            var newXproperty = origin.X - dragMousePosition.X;
            var newYproperty = origin.Y - dragMousePosition.Y;

            var isXOutOfBorder = LoadWindows.GetMainWindow.MainImageBorder.ActualWidth < (LoadWindows.GetMainWindow.MainImage.ActualWidth * scaleTransform.ScaleX);
            var isYOutOfBorder = LoadWindows.GetMainWindow.MainImageBorder.ActualHeight < (LoadWindows.GetMainWindow.MainImage.ActualHeight * scaleTransform.ScaleY);
            if ((isXOutOfBorder && newXproperty > 0) || (!isXOutOfBorder && newXproperty < 0))
            {
                newXproperty = 0;
            }
            if ((isYOutOfBorder && newYproperty > 0) || (!isYOutOfBorder && newYproperty < 0))
            {
                newYproperty = 0;
            }
            var maxX = LoadWindows.GetMainWindow.MainImageBorder.ActualWidth - (LoadWindows.GetMainWindow.MainImage.ActualWidth * scaleTransform.ScaleX);
            if ((isXOutOfBorder && newXproperty < maxX) || (!isXOutOfBorder && newXproperty > maxX))
            {
                newXproperty = maxX;
            }
            var maxY = LoadWindows.GetMainWindow.MainImageBorder.ActualHeight - (LoadWindows.GetMainWindow.MainImage.ActualHeight * scaleTransform.ScaleY);
            if ((isXOutOfBorder && newYproperty < maxY) || (!isXOutOfBorder && newYproperty > maxY))
            {
                newYproperty = maxY;
            }

            translateTransform.X = newXproperty;
            translateTransform.Y = newYproperty;

            e.Handled = true;
        }

        /// <summary>
        /// Resets element values to their loaded values
        /// </summary>
        internal static void ResetZoom(bool animate = true)
        {
            if (LoadWindows.GetMainWindow.MainImage.Source == null) { return; }

            if (animate)
            {
                BeginZoomAnimation(1);
            }
            else
            {
                scaleTransform.ScaleX = scaleTransform.ScaleY = 1.0;
                translateTransform.X = translateTransform.Y = 0.0;
            }

            Tooltip.CloseToolTipMessage();
            ZoomValue = 1;

            // Display non-zoomed values
            if (Pics.Count == 0)
            {
                /// Display values from web
                SetTitle.SetTitleString((int)LoadWindows.GetMainWindow.MainImage.Source.Width, (int)LoadWindows.GetMainWindow.MainImage.Source.Height);
            }
            else
            {
                SetTitle.SetTitleString((int)LoadWindows.GetMainWindow.MainImage.Source.Width, (int)LoadWindows.GetMainWindow.MainImage.Source.Height, FolderIndex);
            }

            UC.GetQuickSettingsMenu.ZoomSlider.Value = 1.0;
        }

        /// <summary>
        /// Determine zoom direction and speed
        /// </summary>
        /// <param name="i">increment</param>
        internal static void Zoom(bool increment)
        {
            /// Don't zoom when gallery is open
            if (UC.GetPicGallery != null)
            {
                if (GalleryFunctions.IsOpen)
                {
                    return;
                }
            }

            ZoomValue = scaleTransform.ScaleX;

            /// Determine zoom speed
            var zoomSpeed = .095;

            if (increment)
            {
                // Increase speed determined by how much is zoomed in
                if (ZoomValue > 1.2)
                {
                    zoomSpeed += .135;
                }
                if (ZoomValue > 1.5)
                {
                    zoomSpeed += .16;
                }
                if (ZoomValue > 1.8)
                {
                    zoomSpeed += .19;
                }
            }
            else
            {
                // Zoom out faster
                if (ZoomValue > 1.2)
                {
                    zoomSpeed += .4;
                }
                if (ZoomValue > 1.5)
                {
                    zoomSpeed += .55;
                }
                // Make it go negative
                zoomSpeed = -zoomSpeed;
            }

            // Set speed
            ZoomValue += zoomSpeed;

            if (ZoomValue < 1.0)
            {
                /// Don't zoom less than 1.0,
                ZoomValue = 1.0;
            }

            Zoom(ZoomValue);
        }

        /// <summary>
        /// Zooms to given value
        /// </summary>
        /// <param name="value"></param>
        internal static void Zoom(double value)
        {
            if (value > UC.GetQuickSettingsMenu.ZoomSlider.Maximum)
            {
                return;
            }

            ZoomValue = value;

            BeginZoomAnimation(ZoomValue);

            /// Displays zoompercentage in the center window
            if (!string.IsNullOrEmpty(ZoomPercentage))
            {
                Tooltip.ShowTooltipMessage(ZoomPercentage, true);
            }
            else
            {
                Tooltip.CloseToolTipMessage();
            }

            /// Display updated values
            if (Pics.Count == 0)
            {
                /// Display values from web
                SetTitle.SetTitleString((int)LoadWindows.GetMainWindow.MainImage.Source.Width, (int)LoadWindows.GetMainWindow.MainImage.Source.Height);
            }
            else
            {
                SetTitle.SetTitleString((int)LoadWindows.GetMainWindow.MainImage.Source.Width, (int)LoadWindows.GetMainWindow.MainImage.Source.Height, FolderIndex);
            }

            if (UC.GetQuickSettingsMenu.ZoomSlider.Value != value)
            {
                UC.GetQuickSettingsMenu.ZoomSlider.Value = value;
            }
        }

        private static void BeginZoomAnimation(double zoomValue)
        {
            Point relative = Mouse.GetPosition(LoadWindows.GetMainWindow.MainImage);

            // Calculate new position
            double absoluteX = relative.X * scaleTransform.ScaleX + translateTransform.X;
            double absoluteY = relative.Y * scaleTransform.ScaleY + translateTransform.Y;

            // Reset to zero if value is one, which is reset
            double newTranslateValueX = zoomValue > 1 ? absoluteX - relative.X * zoomValue : 0;
            double newTranslateValueY = zoomValue > 1 ? absoluteY - relative.Y * zoomValue : 0;

            var duration = new Duration(TimeSpan.FromSeconds(.3));

            var scaleAnim = new DoubleAnimation(zoomValue, duration)
            {
                // Set stop to make sure animation doesn't hold ownership of scaletransform
                FillBehavior = FillBehavior.Stop
            };

            scaleAnim.Completed += delegate
            {
                // Hack it to keep the intended value
                scaleTransform.ScaleX = scaleTransform.ScaleY = zoomValue;

                // Make sure value stays correct
                ZoomValue = 1.0;
            };

            var translateAnimX = new DoubleAnimation(translateTransform.X, newTranslateValueX, duration)
            {
                // Set stop to make sure animation doesn't hold ownership of translateTransform
                FillBehavior = FillBehavior.Stop
            };

            translateAnimX.Completed += delegate
            {
                // Hack it to keep the intended value
                translateTransform.X = newTranslateValueX;
            };

            var translateAnimY = new DoubleAnimation(translateTransform.Y, newTranslateValueY, duration)
            {
                // Set stop to make sure animation doesn't hold ownership of translateTransform
                FillBehavior = FillBehavior.Stop
            };

            translateAnimY.Completed += delegate
            {
                // Hack it to keep the intended value
                translateTransform.Y = newTranslateValueY;
            };

            // Start animations

            translateTransform.BeginAnimation(TranslateTransform.XProperty, translateAnimX);
            translateTransform.BeginAnimation(TranslateTransform.YProperty, translateAnimY);

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
        }
    }
}