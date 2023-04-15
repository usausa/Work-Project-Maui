namespace SideMenuTest
{
    using System;

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
                rect.X = Math.Max(Math.Min(0, e.TotalX - SideMenu.Width), -SideMenu.Width);
                AbsoluteLayout.SetLayoutBounds(SideMenu, rect);

                if (rect.X > lastPosition)
                {
                    showSideMenu = true;
                }
                else if (rect.X < lastPosition)
                {
                    showSideMenu = false;
                }
                lastPosition = rect.X;
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                var rect = AbsoluteLayout.GetLayoutBounds(SideMenu);
                var pos = showSideMenu ? 0 : -SideMenu.Width;
                var length = (uint)(250 * Math.Abs(pos - rect.X) / SideMenu.Width);

                SideMenu.Animate("Slide", x =>
                {
                    rect.X = x;
                    AbsoluteLayout.SetLayoutBounds(SideMenu, rect);
                }, rect.X, pos, length: length);
            }
        }

        private void SideMenu_OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
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
                rect.X = Math.Max(Math.Min(0, rect.X + e.TotalX), -SideMenu.Width);
                AbsoluteLayout.SetLayoutBounds(SideMenu, rect);

                if (rect.X > lastPosition)
                {
                    showSideMenu = true;
                }
                else if (rect.X < lastPosition)
                {
                    showSideMenu = false;
                }
                lastPosition = rect.X;
            }
            else if (e.StatusType == GestureStatus.Completed)
            {
                var rect = AbsoluteLayout.GetLayoutBounds(SideMenu);
                var pos = showSideMenu ? 0 : -SideMenu.Width;
                var length = (uint)(250 * Math.Abs(pos - rect.X) / SideMenu.Width);

                SideMenu.Animate("Slide", x =>
                {
                    rect.X = x;
                    AbsoluteLayout.SetLayoutBounds(SideMenu, rect);
                }, rect.X, pos, length: length);
            }
        }
    }
}
