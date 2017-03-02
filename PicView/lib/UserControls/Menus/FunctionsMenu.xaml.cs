﻿using System.Windows.Controls;
using System.Windows.Input;
using static PicView.lib.Variables;

namespace PicView.lib.UserControls.Menus
{
    /// <summary>
    /// Menu to open functions
    /// </summary>
    public partial class FunctionsMenu : UserControl
    {
        public FunctionsMenu()
        {
            InitializeComponent();

            //CloseButton
            CloseButton.MouseEnter += CloseButtonMouseOver;
            CloseButton.MouseLeave += CloseButtonMouseLeave;
            CloseButton.PreviewMouseLeftButtonDown += CloseButtonMouseButtonDown;

            // FileDetailsButton
            FileDetailsButton.MouseEnter += FileDetailsButtonMouseOver;
            FileDetailsButton.MouseLeave += FileDetailsButtonMouseLeave;
            FileDetailsButton.PreviewMouseLeftButtonDown += FileDetailsButtonMouseButtonDown;

            //RenameFileButton
            RenameFileButton.MouseEnter += RenameFileButtonMouseOver;
            RenameFileButton.MouseLeave += RenameFileButtonMouseLeave;
            RenameFileButton.PreviewMouseLeftButtonDown += RenameFileButtonMouseButtonDown;

            //HelpButton
            Help.MouseEnter += HelpButtonMouseOver;
            Help.MouseLeave += HelpButtonMouseLeave;
            Help.PreviewMouseLeftButtonDown += HelpButtonMouseButtonDown;

            //AboutButton
            About.MouseEnter += AboutButtonMouseOver;
            About.MouseLeave += AboutButtonMouseLeave;
            About.PreviewMouseLeftButtonDown += AboutButtonMouseButtonDown;

            // ClearButton
            ClearButton.MouseEnter += ClearButtonMouseOver;
            ClearButton.MouseLeave += ClearButtonMouseLeave;
            ClearButton.PreviewMouseLeftButtonDown += ClearButtonMouseButtonDown;

            // DeleteButton
            DeleteButton.MouseEnter += DeleteButtonMouseOver;
            DeleteButton.MouseLeave += DeleteButtonMouseLeave;
            DeleteButton.PreviewMouseLeftButtonDown += DeleteButtonMouseButtonDown;

            //DeletePermButton
            DeletePermButton.MouseEnter += DeletePermButtonMouseOver;
            DeletePermButton.MouseLeave += DeleteButtonMousePermLeave;
            DeletePermButton.PreviewMouseLeftButtonDown += DeleteButtonMousePermButtonDown;

            // ReloadButton
            ReloadButton.MouseEnter += ReloadButtonMouseOver;
            ReloadButton.MouseLeave += ReloadButtonMouseLeave;
            ReloadButton.PreviewMouseLeftButtonDown += ReloadButtonMouseButtonDown;

            // ResetZoomButton
            ResetZoomButton.MouseEnter += ResetZoomButtonMouseOver;
            ResetZoomButton.MouseLeave += ResetZoomButtonMouseLeave;
            ResetZoomButton.PreviewMouseLeftButtonDown += ResetZoomButtonMouseButtonDown;
        }

        #region Mouseover Events

        // Close Button
        private void CloseButtonMouseOver(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseEnterColorEvent(0, 0, 0, 0, CloseButtonBrush, false);
        }

        private void CloseButtonMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationHelper.PreviewMouseLeftButtonDownColorEvent(CloseButtonBrush, false);
        }

        private void CloseButtonMouseLeave(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseLeaveColorEvent(0, 0, 0, 0, CloseButtonBrush, false);
        }

        //Help Button
        private void HelpButtonMouseOver(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseEnterColorEvent(0, 0, 0, 0, HelpBrush, false);
        }

        private void HelpButtonMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationHelper.PreviewMouseLeftButtonDownColorEvent(HelpBrush, false);
        }

        private void HelpButtonMouseLeave(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseLeaveColorEvent(0, 0, 0, 0, HelpBrush, false);
        }

        // About Button
        private void AboutButtonMouseOver(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseEnterColorEvent(0, 0, 0, 0, AboutBrush, false);
        }
        private void AboutButtonMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationHelper.PreviewMouseLeftButtonDownColorEvent(AboutBrush, false);
        }

        private void AboutButtonMouseLeave(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseLeaveColorEvent(0, 0, 0, 0, AboutBrush, false);
        }

        // Reload Button
        private void ReloadButtonMouseOver(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseEnterColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                ReloadBrush,
                false
            );
        }

        private void ReloadButtonMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationHelper.PreviewMouseLeftButtonDownColorEvent(ReloadBrush, false);
        }

        private void ReloadButtonMouseLeave(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseLeaveColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                ReloadBrush,
                false
            );
        }

        // ResetZoom Button
        private void ResetZoomButtonMouseOver(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseEnterColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                ResetZoomBrush,
                false
            );
        }

        private void ResetZoomButtonMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationHelper.PreviewMouseLeftButtonDownColorEvent(ResetZoomBrush, false);
        }

        private void ResetZoomButtonMouseLeave(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseLeaveColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                ResetZoomBrush,
                false
            );
        }

        // Clear Button
        private void ClearButtonMouseOver(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseEnterColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                ClearBrush,
                false
            );
        }

        private void ClearButtonMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationHelper.PreviewMouseLeftButtonDownColorEvent(ClearBrush, false);
        }

        private void ClearButtonMouseLeave(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseLeaveColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                ClearBrush,
                false
            );
        }

        // Delete Button
        private void DeleteButtonMouseOver(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseEnterColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                DeleteBrush,
                false
            );
        }

        private void DeleteButtonMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationHelper.PreviewMouseLeftButtonDownColorEvent(DeleteBrush, false);
        }

        private void DeleteButtonMouseLeave(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseLeaveColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                DeleteBrush,
                false
            );
        }


        // Delete Permanent Button
        private void DeletePermButtonMouseOver(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseEnterColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                DeletePermBrush,
                false
            );
        }

        private void DeleteButtonMousePermButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationHelper.PreviewMouseLeftButtonDownColorEvent(DeletePermBrush, false);
        }

        private void DeleteButtonMousePermLeave(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseLeaveColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                DeletePermBrush,
                false
            );
        }

        //RenameFile Button
        private void RenameFileButtonMouseOver(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseEnterColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                RenameFileBrush,
                false
            );
        }

        private void RenameFileButtonMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationHelper.PreviewMouseLeftButtonDownColorEvent(RenameFileBrush, false);
        }

        private void RenameFileButtonMouseLeave(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseLeaveColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                RenameFileBrush,
                false
            );
        }

        // File Details Button
        private void FileDetailsButtonMouseOver(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseEnterColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                FileDetailsBrush,
                false
            );
        }

        private void FileDetailsButtonMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            AnimationHelper.PreviewMouseLeftButtonDownColorEvent(FileDetailsBrush, false);
        }

        private void FileDetailsButtonMouseLeave(object sender, MouseEventArgs e)
        {
            AnimationHelper.MouseLeaveColorEvent(
                mainColor.A,
                mainColor.R,
                mainColor.G,
                mainColor.B,
                FileDetailsBrush,
                false
            );
        }
        #endregion
    }
}
