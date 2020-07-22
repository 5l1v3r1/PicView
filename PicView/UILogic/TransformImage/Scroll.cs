﻿using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static PicView.Library.Fields;
using static PicView.UILogic.Sizing.ScaleImage;
using static PicView.UILogic.Tooltip;
using static PicView.UILogic.UC;

namespace PicView.UILogic.TransformImage
{
    internal static class Scroll
    {
        /// <summary>
        /// Starting point of AutoScroll
        /// </summary>
        internal static Point? AutoScrollOrigin;

        /// <summary>
        /// Current point of AutoScroll
        /// </summary>
        internal static Point AutoScrollPos;

        /// <summary>
        /// Timer used to continously scroll with AutoScroll
        /// </summary>
        internal static Timer AutoScrollTimer;

        internal static bool IsAutoScrolling { get; set; }

        /// <summary>
        /// Toggles scroll and displays it with TooltipStle
        /// </summary>
        internal static bool IsScrollEnabled
        {
            get { return Properties.Settings.Default.ScrollEnabled; }
            set
            {
                Properties.Settings.Default.ScrollEnabled = value;
                TheMainWindow.Scroller.VerticalScrollBarVisibility =
                    value ? ScrollBarVisibility.Auto : ScrollBarVisibility.Disabled;

                if (ChangeImage.Navigation.Pics != null)
                {
                    TryFitImage();
                    ShowTooltipMessage(value ? Application.Current.Resources["ScrollingEnabled"] : Application.Current.Resources["ScrollingDisabled"]);
                }
            }
        }

        // AutoScrollSign

        internal static void HideAutoScrollSign()
        {
            GetAutoScrollSign.Visibility = Visibility.Collapsed;
            GetAutoScrollSign.Opacity = 0;
        }

        internal static void ShowAutoScrollSign()
        {
            Canvas.SetTop(GetAutoScrollSign, AutoScrollOrigin.Value.Y);
            Canvas.SetLeft(GetAutoScrollSign, AutoScrollOrigin.Value.X);
            GetAutoScrollSign.Visibility = Visibility.Visible;
            GetAutoScrollSign.Opacity = 1;
        }

        // Auto scroll

        /// <summary>
        /// Starts the auto scroll feature and shows the sign on the ui
        /// </summary>
        /// <param name="e"></param>
        internal static void StartAutoScroll(MouseButtonEventArgs e)
        {
            // Don't scroll if not scrollable
            if (TheMainWindow.Scroller.ComputedVerticalScrollBarVisibility == Visibility.Collapsed)
            {
                return;
            }

            IsAutoScrolling = true;
            AutoScrollOrigin = e.GetPosition(TheMainWindow.Scroller);

            ShowAutoScrollSign();
        }

        /// <summary>
        /// Stop auto scroll feature and remove sign from the ui
        /// </summary>
        internal static void StopAutoScroll()
        {
            AutoScrollTimer.Stop();
            //window.ReleaseMouseCapture();
            AutoScrollTimer.Enabled = false;
            IsAutoScrolling = false;
            AutoScrollOrigin = null;
            HideAutoScrollSign();
        }

        /// <summary>
        /// Uses timer to scroll vertical up/down every seventh milisecond
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="E"></param>
        internal static async void AutoScrollTimerEvent(object sender, ElapsedEventArgs E)
        {
            // Error checking
            if (AutoScrollPos == null || AutoScrollOrigin == null)
            {
                return;
            }

            // Start in dispatcher because timer is threaded
            await TheMainWindow.Dispatcher.BeginInvoke((Action)(() =>
            {
                if (AutoScrollOrigin.HasValue)
                {
                    // Calculate offset by Y coordinate
                    var offset = (AutoScrollPos.Y - AutoScrollOrigin.Value.Y) / 15;

                    if (IsAutoScrolling)
                    {
                        // Tell the scrollviewer to scroll to calculated offset
                        TheMainWindow.Scroller.ScrollToVerticalOffset(
                            TheMainWindow.Scroller.VerticalOffset + offset);
                    }
                }
            }));
        }
    }
}