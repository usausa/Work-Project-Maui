using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace WorkList
{
    public class ListViewEx : ListView
    {
        protected override void SetupContent(Cell content, int index)
        {
            base.SetupContent(content, index);

            //var currentViewCell = content as ViewCell;

            //if (currentViewCell != null)
            //{
            //    currentViewCell.View.BackgroundColor = index % 2 == 0 ? Color.White : Color.SkyBlue;
            //}
        }
    }
}
