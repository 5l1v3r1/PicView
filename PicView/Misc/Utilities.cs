﻿using Microsoft.WindowsAPICodePack.Taskbar;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using static PicView.Fields;

namespace PicView
{
    internal static class Helper
    {
        /// <summary>
        /// Greatest Common Divisor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        internal static int GCD(int x, int y)
        {
            return y == 0 ? x : GCD(y, x % y);
        }

        /// <summary>
        /// Show progress on taskbar
        /// </summary>
        /// <param name="i">index</param>
        /// <param name="ii">size</param>
        internal static void Progress(int i, int ii)
        {
            TaskbarManager prog = TaskbarManager.Instance;
            prog.SetProgressState(TaskbarProgressBarState.Normal);
            prog.SetProgressValue(i, ii);
        }

        /// <summary>
        /// Stop showing taskbar progress, return to default
        /// </summary>
        internal static void NoProgress()
        {
            TaskbarManager prog = TaskbarManager.Instance;
            prog.SetProgressState(TaskbarProgressBarState.NoProgress);
        }

        /// <summary>
        /// Sends the file to Windows print system
        /// </summary>
        /// <param name="path">The file path</param>
        internal static void Print(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            var p = new Process();
            p.StartInfo.FileName = path;
            p.StartInfo.Verb = "print";
            p.Start();
        }

        internal static void UpdateColor()
        {
            Application.Current.Resources["ChosenColor"] = AnimationHelper.GetPrefferedColorOver();

            if (Properties.Settings.Default.WindowBorderColorEnabled)
            {
                var bgBrush = Application.Current.Resources["WindowBackgroundColorBrush"] as System.Windows.Media.SolidColorBrush;
                bgBrush.Color = AnimationHelper.GetPrefferedColorOver();
            }
        }

        internal static void RemoveBorderColor()
        {
            if (Properties.Settings.Default.WindowBorderColorEnabled)
            {
                var bgBrush = Application.Current.Resources["WindowBackgroundColorBrush"] as System.Windows.Media.SolidColorBrush;
                bgBrush.Color = Colors.Black;
            }
        }

        internal static void ChangeBackground(object sender, RoutedEventArgs e)
        {
            if (mainWindow.imgBorder == null)
                return;

            if (!(mainWindow.imgBorder.Background is SolidColorBrush cc))
                return;

            if (cc.Color == Colors.White)
            {
                mainWindow.imgBorder.Background = new SolidColorBrush(Colors.Transparent);
                Properties.Settings.Default.BgColorWhite = false;
            }

            else
            {
                mainWindow.imgBorder.Background = new SolidColorBrush(Colors.White);
                Properties.Settings.Default.BgColorWhite = true;
            }

        }


    }
}
