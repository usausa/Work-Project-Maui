namespace SideMenuTest
{
    using System;
    using System.Diagnostics;

    using Xamarin.Forms;

    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private bool showSideMenu;
        private double lastPosition;

        private void Anchor_OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Debug.WriteLine($"{e.StatusType} {e.TotalX} {e.TotalY} {SideMenu.X},{SideMenu.Y},{SideMenu.Width},{SideMenu.Height}");

            if (e.StatusType == GestureStatus.Started)
            {
                ViewExtensions.CancelAnimations(SideMenu);

                var rect = AbsoluteLayout.GetLayoutBounds(SideMenu);
                showSideMenu = rect.X > -SideMenu.Width;
                lastPosition = rect.X;
            }
            else if (e.StatusType == GestureStatus.Running)
            {
                //SideMenu.TranslateTo(pos, SideMenu.Y, 20);
                var rect = AbsoluteLayout.GetLayoutBounds(SideMenu);
                rect.X = Math.Max(Math.Min(0, e.TotalX - SideMenu.Width), -SideMenu.Width);
                AbsoluteLayout.SetLayoutBounds(SideMenu, rect);

                showSideMenu = rect.X > lastPosition;
                lastPosition = rect.X;
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                var rect = AbsoluteLayout.GetLayoutBounds(SideMenu);

                if (showSideMenu)
                {
                    //SideMenu.TranslateTo(0, SideMenu.Y, 250, Easing.SpringOut);
                    rect.X = 0;
                    AbsoluteLayout.SetLayoutBounds(SideMenu, rect);
                }
                else
                {
                    //SideMenu.TranslateTo(-SideMenu.Width, SideMenu.Y, 250, Easing.SpringIn);
                    rect.X = -SideMenu.Width;
                    AbsoluteLayout.SetLayoutBounds(SideMenu, rect);
                }
            }

            Debug.WriteLine($"TranslateTo {AbsoluteLayout.GetLayoutBounds(SideMenu).X}");
        }

        private void SideMenu_OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Debug.WriteLine($"{e.StatusType} {e.TotalX} {e.TotalY} {SideMenu.X},{SideMenu.Y},{SideMenu.Width},{SideMenu.Height}");

            if (e.StatusType == GestureStatus.Started)
            {
                ViewExtensions.CancelAnimations(SideMenu);

                var rect = AbsoluteLayout.GetLayoutBounds(SideMenu);
                showSideMenu = rect.X > -SideMenu.Width;
                lastPosition = rect.X;
            }
            else if (e.StatusType == GestureStatus.Running)
            {
                var rect = AbsoluteLayout.GetLayoutBounds(SideMenu);
                //SideMenu.TranslateTo(pos, SideMenu.Y, 20);
                rect.X = Math.Max(Math.Min(0, rect.X + e.TotalX), -SideMenu.Width);
                AbsoluteLayout.SetLayoutBounds(SideMenu, rect);

                showSideMenu = rect.X > lastPosition;
                lastPosition = rect.X;
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                var rect = AbsoluteLayout.GetLayoutBounds(SideMenu);

                if (showSideMenu)
                {
                    //SideMenu.TranslateTo(0, SideMenu.Y, 250, Easing.SpringOut);
                    rect.X = 0;
                    AbsoluteLayout.SetLayoutBounds(SideMenu, rect);
                }
                else
                {
                    //SideMenu.TranslateTo(-SideMenu.Width, SideMenu.Y, 250, Easing.SpringIn);
                    rect.X = -SideMenu.Width;
                    AbsoluteLayout.SetLayoutBounds(SideMenu, rect);

                    showSideMenu = e.TotalX > 0;
                }
            }

            Debug.WriteLine($"TranslateTo {AbsoluteLayout.GetLayoutBounds(SideMenu).X}");
        }
    }
}
