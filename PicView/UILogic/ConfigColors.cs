﻿using PicView.Library.Resources;
using PicView.UI.Animations;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using static PicView.Library.Fields;

namespace PicView.UI
{
    public static class ConfigColors
    {
        /// <summary>
        /// Helper for user color settings
        /// </summary>
        internal static Color backgroundBorderColor;

        /// <summary>
        /// Helper for user color settings
        /// </summary>
        internal static Color mainColor;

        /// <summary>
        /// Update color values for brushes and window border
        /// </summary>
        /// <param name="remove">Remove border?</param>
        public static void UpdateColor(bool remove = false)
        {
            if (remove)
            {
                Application.Current.Resources["WindowBorderColorBrush"] = Brushes.Black;
                return;
            }

            var getColor = AnimationHelper.GetPrefferedColorOver();
            var getColorBrush = new SolidColorBrush(getColor);

            Application.Current.Resources["ChosenColor"] = getColor;
            Application.Current.Resources["ChosenColorBrush"] = getColorBrush;

            if (Properties.Settings.Default.WindowBorderColorEnabled)
            {
                try
                {
                    Application.Current.Resources["WindowBorderColorBrush"] = getColorBrush;
                }
                catch (Exception e)
                {
#if DEBUG
                    Trace.WriteLine(nameof(UpdateColor) + " threw exception:  " + e.Message);
#endif
                }
            }
        }

        public static void SetColors()
        {
            mainColor = (Color)Application.Current.Resources["IconColor"];

            switch (Properties.Settings.Default.BgColorChoice)
            {
                case 0:
                    TheMainWindow.ParentContainer.Background = Brushes.Transparent;
                    break;

                case 1:
                    var brush = Properties.Settings.Default.LightTheme ? Brushes.Black : Brushes.White;
                    TheMainWindow.ParentContainer.Background = brush;
                    break;

                case 2:
                    TheMainWindow.ParentContainer.Background = DrawingBrushes.CheckerboardDrawingBrush(Colors.White);
                    break;
            }

            backgroundBorderColor = (Color)Application.Current.Resources["BackgroundColorAlt"];
        }

        public static void ChangeBackground(object sender, RoutedEventArgs e)
        {
            if (TheMainWindow.ParentContainer == null)
            {
                return;
            }

            Properties.Settings.Default.BgColorChoice++;

            if (Properties.Settings.Default.BgColorChoice > 2)
            {
                Properties.Settings.Default.BgColorChoice = 0;
            }

            switch (Properties.Settings.Default.BgColorChoice)
            {
                case 0:
                    var x = new SolidColorBrush(Colors.Transparent);
                    if (TheMainWindow.ParentContainer.Background == x)
                    {
                        goto case 1;
                    }
                    TheMainWindow.ParentContainer.Background = x;
                    break;

                case 1:
                    var brush = Properties.Settings.Default.LightTheme ? Brushes.Black : Brushes.White;
                    TheMainWindow.ParentContainer.Background = brush;
                    break;

                case 2:
                    var _ = DrawingBrushes.CheckerboardDrawingBrush(Colors.White);
                    if (_ != null)
                    {
                        TheMainWindow.ParentContainer.Background = _;
                    }
                    break;

                default:
                    TheMainWindow.ParentContainer.Background = Brushes.Transparent;
                    break;
            }
        }

        public static Brush GetBackgroundColorBrush()
        {
            return Properties.Settings.Default.BgColorChoice switch
            {
                0 => Brushes.Transparent,
                1 => Properties.Settings.Default.LightTheme ? Brushes.Black : Brushes.White,
                2 => DrawingBrushes.CheckerboardDrawingBrush(Colors.White),
                _ => Brushes.Transparent,
            };
        }

        internal static void ChangeToLightTheme()
        {
            Application.Current.Resources.MergedDictionaries[1] = new ResourceDictionary
            {
                Source = new Uri(@"/PicView;component/Themes/Styles/ColorThemes/Light.xaml", UriKind.Relative)
            };

            Properties.Settings.Default.LightTheme = true;
        }


        internal static void ChangeToDarkTheme()
        {
            Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary 
            {
                Source = new Uri(@"/PicView;component/Themes/Styles/ColorThemes/Dark.xaml", UriKind.Relative)
            };

            Properties.Settings.Default.LightTheme = false;
        }


    }
}