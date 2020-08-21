﻿using PicView.UILogic.Loading;
using PicView.UILogic.Sizing;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static PicView.SystemIntegration.NativeMethods;
using static PicView.UILogic.PicGallery.GalleryScroll;

namespace PicView.Views.Windows
{
    public partial class FakeWindow : Window
    {
        public FakeWindow()
        {
            Focusable = false;
            Topmost = false;
            InitializeComponent();
            Width = WindowLogic.MonitorInfo.Width;
            Height = WindowLogic.MonitorInfo.Height;
            Width = WindowLogic.MonitorInfo.Width;
            Height = WindowLogic.MonitorInfo.Height;
            ContentRendered += FakeWindow_ContentRendered;
        }

        private void FakeWindow_ContentRendered(object sender, EventArgs e)
        {
            PreviewMouseLeftButtonDown += FakeWindow_MouseLeftButtonDown;
            PreviewMouseRightButtonDown += FakeWindow_MouseLeftButtonDown;
            Application.Current.MainWindow.StateChanged += MainWindow_StateChanged;
            StateChanged += FakeWindow_StateChanged;
            MouseWheel += FakeWindow_MouseWheel;
            Application.Current.MainWindow.Focus();
            LostFocus += FakeWindow_LostFocus;
            GotFocus += FakeWindow_LostFocus;

            EnableBlur(this);

            // Hide from alt tab
            var helper = new WindowInteropHelper(this).Handle;
            _ = SetWindowLong(helper, GWL_EX_STYLE, (GetWindowLong(helper, GWL_EX_STYLE) | WS_EX_TOOLWINDOW) & ~WS_EX_APPWINDOW);

            LoadWindows.GetMainWindow.BringIntoView();
        }

        private void FakeWindow_LostFocus(object sender, RoutedEventArgs e)
        {
            LoadWindows.GetMainWindow.Focus();
        }

        private void FakeWindow_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                LoadWindows.GetMainWindow.Focus();
            }
        }

        private void FakeWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ScrollTo(e.Delta > 0, true);
            }
            else
            {
                ScrollTo(e.Delta > 0, false, true);
            }
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (LoadWindows.GetMainWindow.WindowState == WindowState.Normal)
            {
                if (Properties.Settings.Default.PicGallery == 1) { return; }

                Show();
                LoadWindows.GetMainWindow.BringIntoView();
            }
            else if (LoadWindows.GetMainWindow.WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        private void FakeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LoadWindows.GetMainWindow.Focus();
        }
    }
}