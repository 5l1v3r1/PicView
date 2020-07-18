﻿using System.Windows.Controls;
using static PicView.UI.Animations.MouseOverAnimations;

namespace PicView.UI.UserControls
{
    public partial class BackGroundButton : UserControl
    {
        public BackGroundButton()
        {
            InitializeComponent();

            Loaded += delegate
            {
                TheButton.PreviewMouseLeftButtonDown += (s, x) => PreviewMouseButtonDownAnim(bgBrush);
                TheButton.MouseEnter += (s, x) => ButtonMouseOverAnim(bgBrush, true);
                TheButton.MouseLeave += (s, x) => ButtonMouseLeaveAnimBgColor(bgBrush, false);
                TheButton.Click += ConfigColors.ChangeBackground;

                ToolTip = "Toggle background color"; // TODO add translation
            };
        }
    }
}