﻿using PicView.lib;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PicView.Windows
{
    /// <summary>
    /// Interaction logic for YesNoDialogWindow.xaml
    /// </summary>
    public partial class YesNoDialogWindow : Window
    {
        string RbtnName;
        public string NameForRename { get { return TxtNewName.Text; } }
        public string Counter2 { get; set; }
        public int NewPicWidth { get; set; }
        public int NewPicHeight { get; set; }
        public string ChosenRbtn { get { return RbtnName; } }
        public string Picname { get; set; }

        public YesNoDialogWindow(string Message)
        {
            InitializeComponent();

            TxtNewName.Focus();
            RenameLabel.Content = Message;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CloseButton.Click += (s, x) => Close();

            CloseButton.MouseEnter += (s, x) =>
            {
                AnimationHelper.MouseEnterColorEvent(0, 0, 0, 0, CloseButtonBrush, true);
            };

            CloseButton.MouseLeave += (s, x) =>
            {
                AnimationHelper.MouseLeaveColorEvent(0, 0, 0, 0, CloseButtonBrush, true);
            };

            CloseButton.PreviewMouseLeftButtonDown += (s, x) =>
            {
                AnimationHelper.PreviewMouseLeftButtonDownColorEvent(CloseButtonBrush, true);
            };
        }

        #region Eventhandlers

        // Close Button
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Closing -= Window_Closing;
            
            AnimationHelper.FadeWindow(this, 0, TimeSpan.FromSeconds(.5));
            FocusManager.SetFocusedElement(Application.Current.MainWindow, this);
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            
            if(rbRename.IsChecked == true)
            {
                DialogResult = true;
                RbtnName = "rbRename";
                this.Close();
            }
            else if(rbBulkRename.IsChecked == true)
            {
                DialogResult = true;
                RbtnName = "rbBulkRename";
                Counter2 = txtcounter.Text;
                this.Close();
            }
            else if (rbBulkRenameEx.IsChecked == true)
            {
                DialogResult = true;
                RbtnName = "rbBulkRenameEx";
                this.Close();
            }
            else if(rbBulkResize.IsChecked == true)
            {
                DialogResult = true;
                RbtnName = "rbBulkResize";
                NewPicHeight = int.Parse(txtHeight.Text);
                NewPicWidth = int.Parse(txtWidth.Text);
                this.Close();
            }
           
         
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }


        #endregion

        private void rbRename_Click(object sender, RoutedEventArgs e)
        {
            RadioButton rbtn = sender as RadioButton;

            if(rbtn != null && rbtn.IsChecked == true)
            {
                switch(rbtn.Name)
                {
                    case "rbRename":
                        RenameLabel.Content = "Are you sure you wanna rename \r\n" + Picname + " to ";
                        TxtNewName.ToolTip = "Write what you wanna rename the file to.";
                        TxtNewName.Visibility = Visibility.Visible;
                        txtcounter.Visibility = Visibility.Collapsed;
                        lbCounter.Visibility = Visibility.Collapsed;
                        lbHeight.Visibility = Visibility.Collapsed;
                        lbWidth.Visibility = Visibility.Collapsed;
                        txtHeight.Visibility = Visibility.Collapsed;
                        txtWidth.Visibility = Visibility.Collapsed;
                        break;

                    case "rbBulkRename":
                        RenameLabel.Content = "Are you sure you wanna rename all \r\n" + " files in the folder.";
                        TxtNewName.ToolTip = "Write what you wanna call all files in the folder with extension.";
                        txtcounter.ToolTip = "Write where you wanna start the count from (Default: 1)";
                        TxtNewName.Visibility = Visibility.Visible;
                        txtcounter.Visibility = Visibility.Visible;
                        lbCounter.Visibility = Visibility.Visible;
                        lbHeight.Visibility = Visibility.Collapsed;
                        lbWidth.Visibility = Visibility.Collapsed;
                        txtHeight.Visibility = Visibility.Collapsed;
                        txtWidth.Visibility = Visibility.Collapsed;
                        break;

                    case "rbBulkRenameEx":
                        RenameLabel.Content = "Are you sure you wanna change all \r\n" + " extension in the folder.";
                        TxtNewName.ToolTip = "Write the extension you wanna convert all files in folder to";
                        TxtNewName.Visibility = Visibility.Visible;
                        txtcounter.Visibility = Visibility.Collapsed;
                        lbCounter.Visibility = Visibility.Collapsed;
                        lbHeight.Visibility = Visibility.Collapsed;
                        lbWidth.Visibility = Visibility.Collapsed;
                        txtHeight.Visibility = Visibility.Collapsed;
                        txtWidth.Visibility = Visibility.Collapsed;
                        break;

                    case "rbBulkResize":
                        RenameLabel.Content = "Are you sure you wanna resize all \r\n" + " pictures in the folder. (This may take time.)";
                        txtWidth.ToolTip = "Write the width you wanna resize all images in folder to";
                        txtHeight.ToolTip = "Write the height you wanna resize all images in folder to";
                        txtcounter.Visibility = Visibility.Collapsed;
                        lbCounter.Visibility = Visibility.Collapsed;
                        TxtNewName.Visibility = Visibility.Collapsed;
                        lbHeight.Visibility = Visibility.Visible;
                        lbWidth.Visibility = Visibility.Visible;
                        txtHeight.Visibility = Visibility.Visible;
                        txtWidth.Visibility = Visibility.Visible;
                        break;
                }


            }
        }

        
    }
}
