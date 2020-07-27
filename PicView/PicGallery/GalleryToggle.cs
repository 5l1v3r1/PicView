﻿using PicView.UILogic.Animations;
using PicView.UILogic.Loading;
using PicView.UILogic.Sizing;
using PicView.Views.Windows;
using System;
using System.Windows;
using System.Windows.Media.Animation;
using static PicView.ChangeImage.Navigation;
using static PicView.UILogic.PicGallery.GalleryFunctions;
using static PicView.UILogic.PicGallery.GalleryLoad;
using static PicView.UILogic.PicGallery.GalleryScroll;
using static PicView.UILogic.Sizing.WindowLogic;
using static PicView.UILogic.UC;

namespace PicView.UILogic.PicGallery
{
    internal static class GalleryToggle
    {
        #region Toggle

        internal static void Toggle(bool change = false)
        {
            if (Pics.Count < 1)
            {
                return;
            }

            var picGallery = Properties.Settings.Default.PicGallery;

            /// Toggle PicGallery, when not changed
            if (!change)
            {
                if (picGallery == 1)
                {
                    if (!IsOpen)
                    {
                        OpenContainedGallery();
                    }
                    else
                    {
                        CloseContainedGallery();
                    }
                }
                else
                {
                    if (!IsOpen)
                    {
                        OpenFullscreenGallery();
                    }
                    else
                    {
                        CloseFullscreenGallery();
                    }
                }
            }
            /// Toggle PicGallery, when changed
            else
            {
                if (picGallery == 1)
                {
                    ChangeToPicGalleryTwo();
                }
                else
                {
                    ChangeToPicGalleryOne();
                }
            }
        }

        #endregion Toggle

        #region Open

        internal static async void OpenContainedGallery()
        {
            if (Pics.Count < 1)
            {
                return;
            }

            Properties.Settings.Default.PicGallery = 1;
            LoadLayout();

            GetPicGallery.Opacity = 1;
            AnimationHelper.Fade(GetPicGallery.Container, TimeSpan.FromSeconds(.5), TimeSpan.Zero, 0, 1);

            GetClickArrowLeft.Visibility =
            GetClickArrowRight.Visibility =
            Getx2.Visibility =
            GetMinus.Visibility =
            GetRestorebutton.Visibility =
            GetGalleryShortcut.Visibility = Visibility.Hidden;

            if (fakeWindow != null)
            {
                if (fakeWindow.grid.Children.Contains(GetPicGallery))
                {
                    fakeWindow.grid.Children.Remove(GetPicGallery);
                    LoadWindows.GetMainWindow.ParentContainer.Children.Add(GetPicGallery);
                }
            }

            ScrollTo();

            if (!IsLoading)
            {
                await Load().ConfigureAwait(false);
            }
        }

        internal static async void OpenFullscreenGallery(bool startup = false)
        {
            if (Pics.Count < 1 && !startup)
            {
                return;
            }

            Properties.Settings.Default.PicGallery = 2;
            LoadLayout();

            if (fakeWindow == null)
            {
                fakeWindow = new FakeWindow();
                fakeWindow.grid.Children.Add(new Views.UserControls.Gallery.PicGalleryTopButtons
                {
                    Margin = new Thickness(1, 12, 0, 0),
                });
            }

            // Switch gallery container to the correct window
            if (LoadWindows.GetMainWindow.ParentContainer.Children.Contains(GetPicGallery))
            {
                LoadWindows.GetMainWindow.ParentContainer.Children.Remove(GetPicGallery);
                fakeWindow.grid.Children.Add(GetPicGallery);
            }

            fakeWindow.Show();
            ScrollTo();
            LoadWindows.GetMainWindow.Focus();

            if (!FreshStartup)
            {
                ScaleImage.TryFitImage();
            }

            // Fix not showing up opacity bug..
            VisualStateManager.GoToElementState(GetPicGallery, "Opacity", false);

            if (!IsLoading)
            {
                await Load().ConfigureAwait(false);
            }
        }

        #endregion Open

        #region Close

        internal static void CloseContainedGallery()
        {
            IsOpen = false;

            // Restore interface elements if needed
            if (!Properties.Settings.Default.ShowInterface || Properties.Settings.Default.Fullscreen)
            {
                HideInterfaceLogic.ShowNavigation(true);
                HideInterfaceLogic.ShowShortcuts(true);
            }

            var da = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(.5),
                From = 1,
                To = 0,
                FillBehavior = FillBehavior.Stop
            };
            da.Completed += delegate
            {
                GetPicGallery.Visibility = Visibility.Collapsed;
                GetPicGallery.Opacity = 1;
            };

            GetPicGallery.BeginAnimation(UIElement.OpacityProperty, da);
        }

        internal static void CloseFullscreenGallery()
        {
            Properties.Settings.Default.PicGallery = 1;
            IsOpen = false;
            fakeWindow.Hide();

            ConfigureSettings.ConfigColors.UpdateColor();

            HideInterfaceLogic.ShowStandardInterface();

            // Restore settings
            AutoFitWindow = Properties.Settings.Default.AutoFitWindow;
        }

        #endregion Close

        #region Change

        internal static void ChangeToPicGalleryOne()
        {
            Properties.Settings.Default.PicGallery = 1;
            LoadLayout();

            if (fakeWindow.grid.Children.Contains(GetPicGallery))
            {
                fakeWindow.grid.Children.Remove(GetPicGallery);
                LoadWindows.GetMainWindow.ParentContainer.Children.Add(GetPicGallery);
            }

            fakeWindow.Hide();
        }

        internal static void ChangeToPicGalleryTwo()
        {
            Properties.Settings.Default.PicGallery = 2;
            LoadLayout();

            if (fakeWindow != null)
            {
                if (!fakeWindow.grid.Children.Contains(GetPicGallery))
                {
                    LoadWindows.GetMainWindow.ParentContainer.Children.Remove(GetPicGallery);
                    fakeWindow.grid.Children.Add(GetPicGallery);
                }
            }
            else
            {
                LoadWindows.GetMainWindow.ParentContainer.Children.Remove(GetPicGallery);
                fakeWindow = new FakeWindow();
                fakeWindow.grid.Children.Add(GetPicGallery);
            }

            fakeWindow.Show();
            ScrollTo();
            LoadWindows.GetMainWindow.Focus();
        }

        #endregion Change
    }
}