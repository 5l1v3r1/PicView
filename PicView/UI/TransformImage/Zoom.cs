﻿using PicView.UI.PicGallery;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using static PicView.ChangeImage.Navigation;
using static PicView.Library.Fields;
using static PicView.Library.Utilities;
using static PicView.UI.Sizing.WindowLogic;
using static PicView.UI.TransformImage.Scroll;

namespace PicView.UI.TransformImage
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
        private static double zoomValue; 

        /// <summary>
        /// Returns zoom percentage. if 100%, return empty string
        /// </summary>
        internal static string ZoomPercentage
        {
            get
            {
                if (scaleTransform == null || zoomValue <= 1)
                {
                    return string.Empty;
                }

                var zoom = Math.Round(zoomValue * 100);

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
            TheMainWindow.MainImage.RenderTransform = new TransformGroup
            {
                Children = new TransformCollection {
                            new ScaleTransform(),
                            new TranslateTransform()
                        }
            };

            TheMainWindow.Scroller.ClipToBounds = TheMainWindow.MainImage.ClipToBounds = true;

            // Set transforms to UI elements
            scaleTransform = (ScaleTransform)((TransformGroup)TheMainWindow.MainImage.RenderTransform).Children.First(tr => tr is ScaleTransform);
            translateTransform = (TranslateTransform)((TransformGroup)TheMainWindow.MainImage.RenderTransform).Children.First(tr => tr is TranslateTransform);
        }

        // Zoom
        /// <summary>
        /// Pan and Zoom, reset zoom and double click to reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Move window when Shift is being held down
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                Move(sender, e);
                return;
            }

            // Fix focus
            EditTitleBar.Refocus();

            // Logic for auto scrolling
            if (AutoScrolling)
            {
                // Report position and enable autoscrolltimer
                AutoScrollOrigin = e.GetPosition(TheMainWindow);
                AutoScrollTimer.Enabled = true;
                return;
            }
            // Reset zoom on double click
            if (e.ClickCount == 2)
            {
                ResetZoom();
                return;
            }
            // Drag logic
            if (!Scroll.IsScrollEnabled)
            {
                // Report position for image drag
                TheMainWindow.MainImage.CaptureMouse();
                start = e.GetPosition(TheMainWindow);
                origin = new Point(translateTransform.X, translateTransform.Y);
            }
        }

        internal static void Bg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TheMainWindow.Bar.Bar.IsKeyboardFocusWithin)
            {
                // Fix focus
                EditTitleBar.Refocus();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void Zoom_img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Stop autoscrolling or dragging image
            if (AutoScrolling)
            {
                Scroll.StopAutoScroll();
            }
            else
            {
                TheMainWindow.MainImage.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Used to drag image
        /// or getting position for autoscrolltimer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void Zoom_img_MouseMove(object sender, MouseEventArgs e)
        {
            if (AutoScrolling)
            {
                // Start automainWindow.Scroller and report position
                AutoScrollPos = e.GetPosition(TheMainWindow.Scroller);
                AutoScrollTimer.Start();
            }

            // Don't drag when full scale
            // and don't drag it if mouse not held down on image
            if (!TheMainWindow.MainImage.IsMouseCaptured || scaleTransform.ScaleX == 1)
            {
                return;
            }

            // Drag image by modifying X,Y coordinates
            var dragMousePosition = start - e.GetPosition(TheMainWindow);

            var newXproperty = origin.X - dragMousePosition.X;
            var newYproperty = origin.Y - dragMousePosition.Y;

            if (newXproperty < 0)
            {
                newXproperty = 0;
            }

            if (newYproperty < 0)
            {
                newYproperty = 0;
            }

            var rightEdge = TheMainWindow.bg.ActualWidth / zoomValue;
            var bottomEdge = (xHeight + translateTransform.Y + origin.Y) * zoomValue;

            if (rightEdge > TheMainWindow.bg.ActualWidth)
            {
                newXproperty = newXproperty - 1 < -1 ? 0 : newXproperty - 1;
            }

            if (bottomEdge > TheMainWindow.bg.ActualHeight)
            {
                newYproperty = newYproperty - 1 < -1 ? 0 : newYproperty - 1;
            }

            translateTransform.X = newXproperty;
            translateTransform.Y = newYproperty;

            e.Handled = true;
        }

        /// <summary>
        /// Zooms, scrolls or changes image with mousewheel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void Zoom_img_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Disable normal scroll, so we can use our own values
            e.Handled = true;

            if (Properties.Settings.Default.ScrollEnabled && !AutoScrolling)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    Pic(e.Delta > 0);
                }
                else
                {
                    // Scroll vertical when scroll enabled
                    var zoomSpeed = 45;
                    if (e.Delta > 0)
                    {
                        TheMainWindow.Scroller.ScrollToVerticalOffset(TheMainWindow.Scroller.VerticalOffset - zoomSpeed);
                    }
                    else if (e.Delta < 0)
                    {
                        TheMainWindow.Scroller.ScrollToVerticalOffset(TheMainWindow.Scroller.VerticalOffset + zoomSpeed);
                    }
                }
            }
            // Change image with shift being held down
            else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                Pic(e.Delta > 0);
            }
            // Zoom
            else if (!AutoScrolling)
            {
                Zoom(e.Delta > 0);
            }
        }

        /// <summary>
        /// Resets element values to their loaded values
        /// </summary>
        internal static void ResetZoom(bool animate = true)
        {
            if (TheMainWindow.MainImage.Source == null) { return; }

            // Scale to default
            translateTransform.X = translateTransform.Y = 1;

            if (animate)
            {
                BeginZoomAnimation(1);
            }
            else
            {
                scaleTransform.ScaleX = scaleTransform.ScaleY = 1.0;
            }

            Tooltip.CloseToolTipMessage();
            IsZoomed = false;
            zoomValue = 1;

            // Display non-zoomed values
            if (Pics.Count == 0)
            {
                /// Display values from web
                SetTitle.SetTitleString((int)TheMainWindow.MainImage.Source.Width, (int)TheMainWindow.MainImage.Source.Height);
            }
            else
            {
                SetTitle.SetTitleString((int)TheMainWindow.MainImage.Source.Width, (int)TheMainWindow.MainImage.Source.Height, FolderIndex);
            }
        }

        /// <summary>
        /// Scales or zooms, depending on given values
        /// </summary>
        /// <param name="i">increment</param>
        internal static void Zoom(bool increment)
        {
            /// Don't zoom when gallery is open
            if (UserControls.UC.picGallery != null)
            {
                if (GalleryFunctions.IsOpen)
                {
                    return;
                }
            }

            /// Get position where user points cursor
            var position = Mouse.GetPosition(TheMainWindow.MainImage);

            /// Use our position as starting point for zoom
            TheMainWindow.MainImage.RenderTransformOrigin = new Point(position.X / xWidth, position.Y / xHeight);

            zoomValue = scaleTransform.ScaleX;

            /// Determine zoom speed
            var zoomSpeed = .095;

            if (increment)
            {
                // Increase speed determined by how much is zoomed in
                if (zoomValue > 1.3)
                {
                    zoomSpeed += .11;
                }
                if (zoomValue > 1.5)
                {
                    zoomSpeed += .13;
                }
            }
            else
            {
                // Zoom out faster
                if (zoomValue > 1.3)
                {
                    zoomSpeed += .3;
                }
                if (zoomValue > 1.5)
                {
                    zoomSpeed += .35;
                }
                // Make it go negative
                zoomSpeed = -zoomSpeed;
            }

            // Set speed
            zoomValue += zoomSpeed;

            if (zoomValue < 1.0)
            {
                /// Don't zoom less than 1.0,
                zoomValue = 1.0;
            }

            IsZoomed = true;

            BeginZoomAnimation(zoomValue);

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
                SetTitle.SetTitleString((int)TheMainWindow.MainImage.Source.Width, (int)TheMainWindow.MainImage.Source.Height);
            }
            else
            {
                SetTitle.SetTitleString((int)TheMainWindow.MainImage.Source.Width, (int)TheMainWindow.MainImage.Source.Height, FolderIndex);
            }
        }

        private static void BeginZoomAnimation(double zoomValue)
        {
            var duration = new Duration(TimeSpan.FromSeconds(.35));

            var anim = new DoubleAnimation(zoomValue, duration)
            {
                // Set stop to make sure animation doesn't hold ownership of scaletransform
                FillBehavior = FillBehavior.Stop
            };

            anim.Completed += delegate {
                // Hack it to keep the intended value
                scaleTransform.ScaleX = scaleTransform.ScaleY = zoomValue;

                // Make sure value stays correct
                IsZoomed = true;
            };

            scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }
    }
}