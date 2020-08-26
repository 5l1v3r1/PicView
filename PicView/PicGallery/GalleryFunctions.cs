﻿using PicView.UILogic.Loading;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static PicView.UILogic.UC;

namespace PicView.UILogic.PicGallery
{
    internal static class GalleryFunctions
    {
        internal static int picGalleryItem_Size;
        internal static int picGalleryItem_Size_s;

        private static bool Open;

        internal static bool IsOpen
        {
            get { return Open; }
            set
            {
                Open = value;
#if DEBUG
                Trace.WriteLine("IsOpen changed value to: " + IsOpen);
#endif
            }
        }

        internal static async Task Add(BitmapSource pic, int id)
        {
            await ConfigureWindows.GetMainWindow.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                var selected = id == ChangeImage.Navigation.FolderIndex;
                var item = new Views.UserControls.PicGalleryItem(pic, id, selected);
                item.MouseLeftButtonDown += delegate
                {
                    GalleryClick.Click(id);
                };
                GetPicGallery.Container.Children.Add(item);
            }));
        }

        internal static void Clear()
        {
            if (GetPicGallery == null)
            {
                return;
            }

            GetPicGallery.Container.Children.Clear();

#if DEBUG
            Trace.WriteLine("Cleared Gallery children");
#endif
        }

        internal static void SetSelected(int x)
        {
            if (x > GetPicGallery.Container.Children.Count) { return; }

            // Select next item
            var nextItem = GetPicGallery.Container.Children[x] as Views.UserControls.PicGalleryItem;
            nextItem.innerborder.BorderBrush = Application.Current.Resources["ChosenColorBrush"] as SolidColorBrush;
            nextItem.innerborder.Width = nextItem.innerborder.Height = picGalleryItem_Size;
        }

        internal static void SetUnselected(int x)
        {
            if (x > GetPicGallery.Container.Children.Count) { return; }

            // Deselect current item
            var prevItem = GetPicGallery.Container.Children[x] as Views.UserControls.PicGalleryItem;
            prevItem.innerborder.BorderBrush = Application.Current.Resources["BorderBrush"] as SolidColorBrush;
            prevItem.innerborder.Width = prevItem.innerborder.Height = picGalleryItem_Size_s;
        }
    }
}