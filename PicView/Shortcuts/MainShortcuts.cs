﻿using PicView.Editing;
using PicView.Editing.Crop;
using PicView.ImageHandling;
using PicView.UI;
using PicView.UI.PicGallery;
using System.Windows;
using System.Windows.Input;
using static PicView.ChangeImage.Error_Handling;
using static PicView.ChangeImage.Navigation;
using static PicView.FileHandling.Copy_Paste;
using static PicView.FileHandling.DeleteFiles;
using static PicView.FileHandling.Open_Save;
using static PicView.Library.Fields;
using static PicView.UI.Loading.LoadWindows;
using static PicView.UI.PicGallery.GalleryScroll;
using static PicView.UI.PicGallery.GalleryToggle;
using static PicView.UI.Sizing.WindowLogic;
using static PicView.UI.TransformImage.Rotation;
using static PicView.UI.TransformImage.Scroll;
using static PicView.UI.TransformImage.ZoomLogic;
using static PicView.UI.UserControls.UC;

namespace PicView.Shortcuts
{
    internal static class MainShortcuts
    {
        internal static void MainWindow_KeysDown(object sender, KeyEventArgs e)
        {
            // Don't allow keys when typing in text
            if (TheMainWindow.TitleText.IsKeyboardFocusWithin) { return; }

            var ctrlDown = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            var altDown = (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt;
            var shiftDown = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

            #region CroppingKeys

            if (cropppingTool != null)
            {
                if (cropppingTool.IsVisible)
                {
                    if (e.Key == Key.Escape)
                    {
                        if (Pics.Count == 0)
                        {
                            SetTitle.SetTitleString((int)TheMainWindow.MainImage.Source.Width, (int)TheMainWindow.MainImage.Source.Height);
                        }
                        else
                        {
                            SetTitle.SetTitleString((int)TheMainWindow.MainImage.Source.Width, (int)TheMainWindow.MainImage.Source.Height, FolderIndex);
                        }
                        TheMainWindow.ParentContainer.Children.Remove(cropppingTool);
                        CanNavigate = true;
                        e.Handled = true;
                        return;
                    }

                    if (e.Key == Key.Enter)
                    {
                        if (Pics.Count == 0)
                        {
                            SetTitle.SetTitleString((int)TheMainWindow.MainImage.Source.Width, (int)TheMainWindow.MainImage.Source.Height);
                        }
                        else
                        {
                            SetTitle.SetTitleString((int)TheMainWindow.MainImage.Source.Width, (int)TheMainWindow.MainImage.Source.Height, FolderIndex);
                        }

                        CropFunctions.SaveCrop();
                        CanNavigate = true;
                        e.Handled = true;
                        return;
                    }
                    e.Handled = true;
                    return;
                }
            }

            #endregion CroppingKeys

            #region Keys where it can be held down

            switch (e.Key)
            {
                case Key.BrowserForward:
                case Key.Right:
                case Key.D:
                    if (picGallery != null)
                    {
                        if (GalleryFunctions.IsOpen)
                        {
                            if (Properties.Settings.Default.PicGallery == 1)
                            {
                                return;
                            }
                        }
                    }
                    if (!e.IsRepeat)
                    {
                        // Go to first if Ctrl held down
                        if (ctrlDown)
                        {
                            Pic(true, true);
                        }
                        else
                        {
                            Pic();
                        }
                    }
                    else if (CanNavigate)
                    {
                        FastPic(true);
                    }
                    return;

                case Key.BrowserBack:
                case Key.Left:
                case Key.A:
                    if (picGallery != null)
                    {
                        if (GalleryFunctions.IsOpen)
                        {
                            if (Properties.Settings.Default.PicGallery == 1)
                            {
                                return;
                            }
                        }
                    }
                    if (!e.IsRepeat)
                    {
                        // Go to last if Ctrl held down
                        if (ctrlDown)
                        {
                            Pic(false, true);
                        }
                        else
                        {
                            Pic(false);
                        }
                    }
                    else if (CanNavigate)
                    {
                        FastPic(false);
                    }
                    return;

                case Key.PageUp:
                    if (picGallery != null)
                    {
                        if (GalleryFunctions.IsOpen)
                        {
                            ScrollTo(true, ctrlDown);
                            return;
                        }
                    }
                    if (Properties.Settings.Default.ScrollEnabled)
                    {
                        TheMainWindow.Scroller.ScrollToVerticalOffset(TheMainWindow.Scroller.VerticalOffset - 30);
                    }

                    return;

                case Key.PageDown:
                    if (picGallery != null)
                    {
                        if (GalleryFunctions.IsOpen)
                        {
                            ScrollTo(false, (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
                            return;
                        }
                    }
                    if (Properties.Settings.Default.ScrollEnabled)
                    {
                        TheMainWindow.Scroller.ScrollToVerticalOffset(TheMainWindow.Scroller.VerticalOffset + 30);
                    }

                    return;

                case Key.Up:
                case Key.W:
                    if (Properties.Settings.Default.ScrollEnabled)
                    {
                        if (ctrlDown)
                        {
                            return;
                        }
                        else
                        {
                            TheMainWindow.Scroller.ScrollToVerticalOffset(TheMainWindow.Scroller.VerticalOffset - 30);
                        }
                    }
                    else if (picGallery != null)
                    {
                        if (GalleryFunctions.IsOpen)
                        {
                            if (Properties.Settings.Default.PicGallery == 1)
                            {
                                ScrollTo(false, (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
                                return;
                            }
                            if (ctrlDown)
                            {
                                Rotate(false);
                            }
                            else
                            {
                                Pic(false);
                            }
                        }
                        else
                        {
                            Rotate(false);
                        }
                    }
                    else
                    {
                        Rotate(false);
                    }

                    return;

                case Key.Down:
                case Key.S:
                    if (ctrlDown && picGallery != null && !GalleryFunctions.IsOpen)
                    {
                        SaveFiles();
                    }
                    else if (Properties.Settings.Default.ScrollEnabled)
                    {
                        TheMainWindow.Scroller.ScrollToVerticalOffset(TheMainWindow.Scroller.VerticalOffset + 30);
                    }
                    else if (picGallery != null)
                    {
                        if (GalleryFunctions.IsOpen)
                        {
                            if (Properties.Settings.Default.PicGallery == 1)
                            {
                                ScrollTo(true, ctrlDown);
                                return;
                            }
                            if (ctrlDown)
                            {
                                Rotate(true);
                            }
                            else
                            {
                                Pic();
                            }
                        }
                        else
                        {
                            Rotate(true);
                        }
                    }
                    else
                    {
                        Rotate(true);
                    }

                    return;

                // Zoom
                case Key.Add:
                case Key.OemPlus:
                    Zoom(true);
                    return;

                case Key.Subtract:
                case Key.OemMinus:
                    Zoom(false);
                    return;
                default: break; // Please automatic code analyzers...
            }

            #endregion Keys where it can be held down

            #region Key is not held down

            if (!e.IsRepeat)
            {
                switch (e.Key)
                {
                    // Esc
                    case Key.Escape:
                        if (UserControls_Open())
                        {
                            Close_UserControls();
                            return;
                        }
                        if (Properties.Settings.Default.Fullscreen)
                        {
                            if (Slideshow.SlideTimer != null)
                            {
                                if (Slideshow.SlideTimer.Enabled)
                                {
                                    Slideshow.StopSlideshow();
                                }
                            }
                            else
                            {
                                Fullscreen_Restore();
                            }
                            return;
                        }
                        if (GalleryFunctions.IsOpen)
                        {
                            Toggle();
                            return;
                        }
                        if (IsDialogOpen)
                        {
                            IsDialogOpen = false;
                            return;
                        }
                        if (infoWindow != null)
                        {
                            if (infoWindow.IsVisible)
                            {
                                infoWindow.Hide();
                                return;
                            }
                        }
                        if (settingsWindow != null)
                        {
                            if (settingsWindow.IsVisible)
                            {
                                settingsWindow.Hide();
                                return;
                            }
                        }
                        if (!cm.IsVisible)
                        {
                            SystemCommands.CloseWindow(TheMainWindow);
                        }
                        break;

                    // Ctrl + Q
                    case Key.Q:
                        if (ctrlDown)
                        {
                            SystemCommands.CloseWindow(TheMainWindow);
                        }

                        break;

                    // O, Ctrl + O
                    case Key.O:
                        Open();
                        break;

                    // X, Ctrl + X
                    case Key.X:
                        if (ctrlDown)
                        {
                            Cut(Pics[FolderIndex]);
                        }
                        else
                        {
                            UpdateUIValues.SetScrolling(sender, e);
                        }
                        break;
                    // F
                    case Key.F:
                        Flip();
                        break;

                    // Delete, Shift + Delete
                    case Key.Delete:
                        DeleteFile(Pics[FolderIndex], !shiftDown);
                        break;

                    // Ctrl + C, Ctrl + Shift + C, Ctrl + Alt + C
                    case Key.C:
                        if (ctrlDown)
                        {
                            if (resizeAndOptimize != null)
                            {
                                if (resizeAndOptimize.IsVisible)
                                {
                                    return; // Prevent paste errors
                                }
                            }

                            if (shiftDown)
                            {
                                Base64.SendToClipboard();
                            }
                            else if (altDown)
                            {
                                CopyBitmap();
                            }
                            else
                            {
                                Copyfile();
                            }
                        }
                        else
                        {
                            CropFunctions.StartCrop();
                        }
                        break;

                    // Ctrl + V
                    case Key.V:
                        if (ctrlDown)
                        {
                            Paste();
                        }
                        break;

                    // Ctrl + I
                    case Key.I:
                        if (ctrlDown)
                        {
                            SystemIntegration.NativeMethods.ShowFileProperties(Pics[FolderIndex]);
                        }
                        break;

                    // Ctrl + P
                    case Key.P:
                        if (ctrlDown)
                        {
                            Print(Pics[FolderIndex]);
                        }
                        break;

                    // Ctrl + R
                    case Key.R:
                        if (ctrlDown)
                        {
                            Reload();
                        }
                        break;

                    // L
                    case Key.L:
                        UpdateUIValues.SetLooping(sender, e);
                        break;

                    // E
                    case Key.E:
                        OpenWith(Pics[FolderIndex]);
                        break;

                    // T
                    case Key.T:
                        ConfigColors.ChangeBackground(sender, e);
                        break;

                    // Space
                    case Key.Space:
                        if (picGallery != null)
                        {
                            if (GalleryFunctions.IsOpen)
                            {
                                ScrollTo();
                                return;
                            }
                        }
                        CenterWindowOnScreen();
                        break;

                    // 1
                    case Key.D1:
                        if (QuickSettingsMenuOpen || GalleryFunctions.IsOpen) { break; }
                        Tooltip.ShowTooltipMessage("Set to center image in window"); // TODO add to translation
                        UpdateUIValues.SetScalingBehaviour(false, false);
                        break;

                    // 2
                    case Key.D2:
                        if (QuickSettingsMenuOpen || GalleryFunctions.IsOpen) { break; }
                        Tooltip.ShowTooltipMessage("Center image in window, fill height"); // TODO add to translation
                        UpdateUIValues.SetScalingBehaviour(false, true);
                        break;

                    // 3
                    case Key.D3:
                        if (QuickSettingsMenuOpen || GalleryFunctions.IsOpen) { break; }
                        Tooltip.ShowTooltipMessage("Center application to window"); // TODO add to translation
                        UpdateUIValues.SetScalingBehaviour(true, false);
                        break;

                    // 4
                    case Key.D4:
                        if (QuickSettingsMenuOpen || GalleryFunctions.IsOpen) { break; }
                        Tooltip.ShowTooltipMessage("Center application to window, fill height"); // TODO add to translation
                        UpdateUIValues.SetScalingBehaviour(true, true);
                        break;

                    // F1
                    case Key.F1:
                        InfoDialogWindow();
                        break;

                    // F2
                    case Key.F2:
                        EditTitleBar.EditTitleBar_Text();
                        break;

                    // F3
                    case Key.F3:
                        Open_In_Explorer();
                        break;

                    // F4
                    case Key.F4:
                        AllSettingsWindow();
                        break;

                    // F5
                    case Key.F5:
                        Slideshow.StartSlideshow();
                        break;

                    // F6
                    case Key.F6:
                        ResetZoom();
                        break;

#if DEBUG
                    // F8
                    case Key.F8:
                        Unload();
                        break;
#endif
                    // F11
                    case Key.F11:
                        Fullscreen_Restore();
                        break;

                    // Home
                    case Key.Home:
                        TheMainWindow.Scroller.ScrollToHome();
                        break;

                    // End
                    case Key.End:
                        TheMainWindow.Scroller.ScrollToEnd();
                        break;

                    default: break;
                }
            }

            #endregion Key is not held down

            #region Alt + keys

            // Alt doesn't work in switch? Waiting for key up is confusing in this case
            // Alt + Z
            if (altDown && (e.SystemKey == Key.Z))
            {
                if (!e.IsRepeat)
                {
                    HideInterfaceLogic.ToggleInterface();
                }
            }

            // Alt + Enter
            else if (altDown && (e.SystemKey == Key.Enter))
            {
                if (!e.IsRepeat)
                {
                    Fullscreen_Restore();
                }
            }

            #endregion Alt + keys
        }

        internal static void MainWindow_KeysUp(object sender, KeyEventArgs e)
        {
            // Don't allow keys when typing in text
            if (TheMainWindow.TitleText.IsKeyboardFocusWithin) { return; }

            switch (e.Key)
            {
                #region FastPicUpdate()

                case Key.A:
                case Key.Right:
                case Key.D:
                    FastPicUpdate();
                    break;

                #endregion FastPicUpdate()

                default: break;
            }
        }

        internal static void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Right:
                case MouseButton.Left:
                    if (IsAutoScrolling)
                    {
                        StopAutoScroll();
                    }
                    break;

                case MouseButton.Middle:
                    if (!IsAutoScrolling)
                    {
                        StartAutoScroll(e);
                    }
                    else
                    {
                        StopAutoScroll();
                    }

                    break;

                case MouseButton.XButton1:
                    Pic(false);
                    break;

                case MouseButton.XButton2:
                    Pic();
                    break;

                default: break;
            }
        }
    }
}