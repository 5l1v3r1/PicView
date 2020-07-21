﻿using PicView.Editing;
using PicView.Editing.Crop;
using PicView.ImageHandling;
using PicView.UILogic;
using PicView.UILogic.PicGallery;
using System.Windows;
using System.Windows.Input;
using static PicView.ChangeImage.Error_Handling;
using static PicView.ChangeImage.Navigation;
using static PicView.FileHandling.Copy_Paste;
using static PicView.FileHandling.DeleteFiles;
using static PicView.FileHandling.Open_Save;
using static PicView.Library.Fields;
using static PicView.UILogic.Loading.LoadWindows;
using static PicView.UILogic.PicGallery.GalleryScroll;
using static PicView.UILogic.PicGallery.GalleryToggle;
using static PicView.UILogic.Sizing.WindowLogic;
using static PicView.UILogic.TransformImage.Rotation;
using static PicView.UILogic.TransformImage.Scroll;
using static PicView.UILogic.TransformImage.ZoomLogic;
using static PicView.UILogic.UserControls.UC;

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

            if (GetCropppingTool != null)
            {
                if (GetCropppingTool.IsVisible)
                {
                    if (e.Key == Key.Escape)
                    {
                        CropFunctions.CloseCrop();
                        e.Handled = true;
                        return;
                    }

                    if (e.Key == Key.Enter)
                    {
                        CropFunctions.PerformCrop();
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
                    if (GetPicGallery != null)
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
                    if (GetPicGallery != null)
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
                    if (GetPicGallery != null)
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
                    if (GetPicGallery != null)
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
                    else if (GetPicGallery != null)
                    {
                        if (GalleryFunctions.IsOpen)
                        {
                            if (Properties.Settings.Default.PicGallery == 1)
                            {
                                ScrollTo(false, (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control);
                                return;
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
                    if (ctrlDown)
                    {
                        SaveFiles();
                    }
                    else if (Properties.Settings.Default.ScrollEnabled)
                    {
                        TheMainWindow.Scroller.ScrollToVerticalOffset(TheMainWindow.Scroller.VerticalOffset + 30);
                    }
                    else if (GetPicGallery != null)
                    {
                        if (GalleryFunctions.IsOpen)
                        {
                            if (Properties.Settings.Default.PicGallery == 1)
                            {
                                ScrollTo(true, ctrlDown);
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
                default: break;
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
                                if (GalleryFunctions.IsOpen)
                                {
                                    Toggle();
                                }
                                else
                                {
                                    Fullscreen_Restore();
                                }
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
                        if (InfoWindow != null)
                        {
                            if (InfoWindow.IsVisible)
                            {
                                InfoWindow.Hide();
                                return;
                            }
                        }
                        if (GetSettingsWindow != null)
                        {
                            if (GetSettingsWindow.IsVisible)
                            {
                                GetSettingsWindow.Hide();
                                return;
                            }
                        }
                        if (Color_Picking.IsRunning)
                        {
                            Color_Picking.StopRunning(false);
                            return;
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
                            ConfigureSettings.UpdateUIValues.SetScrolling(sender, e);
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
                            if (GetResizeAndOptimize != null)
                            {
                                if (GetResizeAndOptimize.IsVisible)
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
                        ConfigureSettings.UpdateUIValues.SetLooping(sender, e);
                        break;

                    // E
                    case Key.E:
                        OpenWith(Pics[FolderIndex]);
                        break;

                    // T
                    case Key.T:
                        ConfigureSettings.ConfigColors.ChangeBackground(sender, e);
                        break;

                    // Space
                    case Key.Space:
                        if (GetPicGallery != null)
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
                        if (QuickSettingsMenuOpen || GalleryFunctions.IsOpen
                        || Properties.Settings.Default.Fullscreen) { break; }

                        Tooltip.ShowTooltipMessage(Application.Current.Resources["CenterImageInWindow"]);
                        ConfigureSettings.UpdateUIValues.SetScalingBehaviour(false, false);
                        break;

                    // 2
                    case Key.D2:
                        if (QuickSettingsMenuOpen || GalleryFunctions.IsOpen
                        || Properties.Settings.Default.Fullscreen) { break; }

                        Tooltip.ShowTooltipMessage(Application.Current.Resources["CenterImageInWindowFillHeight"]);
                        ConfigureSettings.UpdateUIValues.SetScalingBehaviour(false, true);
                        break;

                    // 3
                    case Key.D3:
                        if (QuickSettingsMenuOpen || GalleryFunctions.IsOpen
                        || Properties.Settings.Default.Fullscreen) { break; }

                        Tooltip.ShowTooltipMessage(Application.Current.Resources["CenterApplicationToWindow"]);
                        ConfigureSettings.UpdateUIValues.SetScalingBehaviour(true, false);
                        break;

                    // 4
                    case Key.D4:
                        if (QuickSettingsMenuOpen || GalleryFunctions.IsOpen
                        || Properties.Settings.Default.Fullscreen) { break; }

                        Tooltip.ShowTooltipMessage(Application.Current.Resources["CenterApplicationToWindowFillHeight"]);
                        ConfigureSettings.UpdateUIValues.SetScalingBehaviour(true, true);
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

            if (altDown && !e.IsRepeat)
            {
                // Alt + Z
                if ((e.SystemKey == Key.Z))
                {
                    HideInterfaceLogic.ToggleInterface();
                }

                // Alt + Enter
                else if ((e.SystemKey == Key.Enter))
                {
                    if (Properties.Settings.Default.PicGallery != 2)
                    {
                        Fullscreen_Restore();
                    }
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
                    // Stop running color picking when right clicking
                    if (Color_Picking.IsRunning)
                    {
                        Color_Picking.StopRunning(false);
                    }
                    else if (IsAutoScrolling)
                    {
                        StopAutoScroll();
                        return;
                    }
                    break;
                case MouseButton.Left:
                    if (IsAutoScrolling)
                    {
                        StopAutoScroll();
                        return;
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
            if (IsAutoScrolling)
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
            if (!IsScrollEnabled)
            {
                PreparePanImage(sender, e);
            }
        }

        internal static void Bg_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TheMainWindow.TitleText.InnerTextBox.IsKeyboardFocusWithin)
            {
                // Fix focus
                EditTitleBar.Refocus();
                return;
            }

            // Move window when Shift is being held down
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                Move(sender, e);
                return;
            }

            if (Color_Picking.IsRunning)
            {
                Color_Picking.StopRunning(true);
            }

            // Reset zoom on double click
            if (e.ClickCount == 2)
            {
                ResetZoom();
                return;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void MainImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Stop autoscrolling or dragging image
            if (IsAutoScrolling)
            {
                StopAutoScroll();
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
        internal static void MainImage_MouseMove(object sender, MouseEventArgs e)
        {   
            if (IsAutoScrolling)
            {
                // Start automainWindow.Scroller and report position
                AutoScrollPos = e.GetPosition(TheMainWindow.Scroller);
                AutoScrollTimer.Start();
            }

            if (Color_Picking.IsRunning)
            {
                if (GetColorPicker.Opacity == 1)
                {
                    Color_Picking.StartRunning();
                }

                return;
            }

            PanImage(sender, e);
        }

        /// <summary>
        /// Zooms, scrolls or changes image with mousewheel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void MainImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Disable normal scroll, so we can use our own values
            e.Handled = true;

            if (Properties.Settings.Default.ScrollEnabled && !IsAutoScrolling)
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
            else if (!IsAutoScrolling)
            {
                Zoom(e.Delta > 0);
            }
        }

    }
}