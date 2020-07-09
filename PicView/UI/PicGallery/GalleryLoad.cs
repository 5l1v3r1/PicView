﻿using PicView.UI.Sizing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static PicView.Library.Fields;
using static PicView.UI.HideInterfaceLogic;
using static PicView.UI.PicGallery.GalleryFunctions;
using static PicView.UI.PicGallery.GalleryScroll;
using static PicView.UI.UserControls.UC;

namespace PicView.UI.PicGallery
{
    internal static class GalleryLoad
    {
        internal static void PicGallery_Loaded(object sender, RoutedEventArgs e)
        {
            // Add events and set fields, when it's loaded.
            GetPicGallery.Scroller.PreviewMouseWheel += ScrollTo;
            GetPicGallery.Scroller.ScrollChanged += (s, x) => TheMainWindow.Focus(); // Maintain window focus when scrolling manually
            GetPicGallery.grid.MouseLeftButtonDown += (s, x) => TheMainWindow.Focus();
            GetPicGallery.x2.MouseLeftButtonDown += delegate { GalleryToggle.CloseContainedGallery(); };
        }

        internal static void LoadLayout()
        {
            if (GetPicGallery == null)
            {
                GetPicGallery = new UserControls.PicGallery
                {
                    Opacity = 0,
                    Visibility = Visibility.Collapsed
                };

                TheMainWindow.ParentContainer.Children.Add(GetPicGallery);
                Panel.SetZIndex(GetPicGallery, 999);
            }

            // TODO Make this code more clean and explain what's going on?
            if (Properties.Settings.Default.PicGallery == 1)
            {
                if (Properties.Settings.Default.Fullscreen)
                {
                    GetPicGallery.Width = SystemParameters.PrimaryScreenWidth;
                    GetPicGallery.Height = SystemParameters.PrimaryScreenHeight;
                }
                else if (Properties.Settings.Default.ShowInterface)
                {
                    GetPicGallery.Width = TheMainWindow.Width - 15;
                    GetPicGallery.Height = TheMainWindow.ActualHeight - 70;
                }
                else
                {
                    GetPicGallery.Width = TheMainWindow.Width - 2;
                    GetPicGallery.Height = TheMainWindow.Height - 2; // 2px for borders
                }

                GetPicGallery.HorizontalAlignment = HorizontalAlignment.Stretch;
                GetPicGallery.Scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                GetPicGallery.Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                GetPicGallery.x2.Visibility = Visibility.Visible;
                GetPicGallery.Container.Margin = new Thickness(0, 65, 0, 0);
            }
            else
            {
                GetPicGallery.Width = picGalleryItem_Size + 14; // 17 for scrollbar width + 2 for borders
                GetPicGallery.Height = MonitorInfo.Height;

                GetPicGallery.HorizontalAlignment = HorizontalAlignment.Right;
                GetPicGallery.Scroller.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                GetPicGallery.Scroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                GetPicGallery.x2.Visibility = Visibility.Collapsed;
                GetPicGallery.Container.Margin = new Thickness(0, 0, 0, 0);

                ShowNavigation(false);
                ShowTopandBottom(false);
                ConfigColors.UpdateColor(true);

#if DEBUG
                if (GetGalleryMenu == null)
                {
                    GetGalleryMenu = new UserControls.Gallery.GalleryMenu
                    {
                    };

                    if (WindowLogic.fakeWindow == null)
                    {
                        WindowLogic.fakeWindow = new Windows.FakeWindow();
                    }

                    WindowLogic.fakeWindow.grid.Children.Add(GetGalleryMenu);
                }
#endif
            }


            GetPicGallery.Visibility = Visibility.Visible;
            GetPicGallery.Opacity = 1;
            GetPicGallery.Container.Orientation = Orientation.Vertical;

            IsOpen = true;
        }

        internal static Task Load()
        {
            IsLoading = true;
            return Task.Run(async () =>
            {
                /// TODO Maybe make this start at at folder index
                /// and get it work with a real sorting method?

                for (int i = 0; i < ChangeImage.Navigation.Pics.Count; i++)
                {
                    var pic = ImageHandling.Thumbnails.GetBitmapSourceThumb(ChangeImage.Navigation.Pics[i]);
                    if (pic != null)
                    {
                        if (!pic.IsFrozen)
                        {
                            pic.Freeze();
                        }

                        await Add(pic, i).ConfigureAwait(false);
                    }
                    // TODO find a placeholder for null images?
                }
                IsLoading = false;
            });
        }
    }
}