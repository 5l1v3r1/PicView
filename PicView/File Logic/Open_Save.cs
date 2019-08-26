﻿using System;
using System.Diagnostics;
using System.IO;
using static PicView.Error_Handling;
using static PicView.Fields;
using static PicView.ImageManager;
using static PicView.Navigation;
using static PicView.Interface;
using static PicView.ToggleMenus;

namespace PicView
{
    internal static class Open_Save
    {
        /// <summary>
        /// Opens image in File Explorer
        /// </summary>
        internal static void Open_In_Explorer()
        {
            if (!File.Exists(PicPath) || mainWindow.img.Source == null)
            {
                ToolTipStyle("Error, File does not exist, or something went wrong...");
                return;
            }
            try
            {
                Close_UserControls();
                ToolTipStyle(ExpFind);
                Process.Start("explorer.exe", "/select,\"" + PicPath + "\"");
            }
            catch (InvalidCastException e)
            {
#if DEBUG
                Trace.WriteLine("Open_In_Explorer exception \n" + e.Message);
#endif
            }
        }

        /// <summary>
        /// Open a file dialog where user can select a supported file
        /// </summary>
        internal static void Open()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = FilterFiles,
                Title = "Open image - PicView"
            };
            if (dlg.ShowDialog() == true)
            {
                
                Pic(dlg.FileName);

                if (string.IsNullOrWhiteSpace(PicPath))
                    PicPath = dlg.FileName;
            }
            else return;

            Close_UserControls();
        }

        /// <summary>
        /// Open a File Dialog, where the user can save a supported file type.
        /// </summary>
        internal static void SaveFiles()
        {
            var Savedlg = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = FilterFiles,
                Title = "Save image - PicView",
                FileName = Path.GetFileName(PicPath)
            };

            if (!string.IsNullOrEmpty(PicPath))
            {
                if (Savedlg.ShowDialog() == true)
                {
                    if (!TrySaveImage(Rotateint, Flipped, PicPath, Savedlg.FileName))
                        ToolTipStyle("Error, File didn't get saved");
                }
                else return;

                //Refresh the list of pictures.
                Reload();

                Close_UserControls();
            }
        }
    }
}
