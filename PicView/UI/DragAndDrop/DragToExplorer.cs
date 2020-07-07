﻿using PicView.Library;
using PicView.UI.TransformImage;
using PicView.UI.UserControls;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static PicView.ChangeImage.Navigation;
using static PicView.Library.Fields;
using static PicView.SystemIntegration.NativeMethods;

namespace PicView.UI.DragAndDrop
{
    internal static class DragToExplorer
    {
        internal static Window dragdropWindow;

        internal static void DragFile(object sender, MouseButtonEventArgs e)
        {
            if (TheMainWindow.MainImage.Source == null
                || Keyboard.Modifiers != ModifierKeys.Control
                || Keyboard.Modifiers == ModifierKeys.Shift
                || Keyboard.Modifiers == ModifierKeys.Alt
                || Properties.Settings.Default.PicGallery == 2
                || Properties.Settings.Default.Fullscreen
                || Scroll.IsAutoScrolling)
            {
                return;
            }

            if (UC.GetColorPicker != null)
            {
                if (UC.GetColorPicker.Opacity == 1)
                {
                    return;
                }
            }

            if (TheMainWindow.TitleText.IsFocused)
            {
                EditTitleBar.Refocus();
                return;
            }

            string file;

            if (Pics.Count == 0)
            {
                string url = Utilities.GetURL(TheMainWindow.TitleText.Text);
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute)) // Check if from web
                {
                    // Create temp directory
                    var tempPath = Path.GetTempPath();
                    var fileName = Path.GetFileName(url);

                    // Download to it
                    using var webClient = new System.Net.WebClient();
                    Directory.CreateDirectory(tempPath);
                    webClient.DownloadFileAsync(new Uri(url), tempPath + fileName);

                    file = tempPath + fileName;
                }
                else
                {
                    return;
                }

            }
            else if (Pics.Count > FolderIndex)
            {
                file = Pics[FolderIndex];
            }
            else
            {
                return;
            }

            FrameworkElement senderElement = sender as FrameworkElement;
            DataObject dragObj = new DataObject();
            dragObj.SetFileDropList(new StringCollection() { file });
            DragDrop.DoDragDrop(senderElement, dragObj, DragDropEffects.Copy);

            e.Handled = true;
        }
    }
}
