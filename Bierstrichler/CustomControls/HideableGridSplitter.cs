using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Bierstrichler.CustomControls
{
    /// <summary>
    /// Grid splitter that show or hides the following row when the visibility of the splitter is changed. 
    /// </summary>
    public class HidableGridSplitter : GridSplitter
    {

        GridLength width;

        public HidableGridSplitter()
        {
            this.IsVisibleChanged += HideableGridSplitter_IsVisibleChanged;
            this.Initialized += HideableGridSplitter_Initialized;
        }

        void HideableGridSplitter_Initialized(object sender, EventArgs e)
        {
            //Cache the initial RowDefinition height,
            //so it is not always assumed to be "Auto"
            Grid parent = base.Parent as Grid;
            if (parent == null) return;
            int colIndex = Grid.GetColumn(this);
            if (colIndex - 1 <= 0) return;
            var leftNeighbor = parent.ColumnDefinitions[colIndex - 1];
            width = leftNeighbor.Width;
        }

        void HideableGridSplitter_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Grid parent = base.Parent as Grid;
            if (parent == null) return;

            int columnIndex = Grid.GetColumn(this);

            if (columnIndex - 1 <= 0) return;

            var leftNeighbor = parent.ColumnDefinitions[columnIndex - 1];

            if (this.Visibility == Visibility.Visible)
                leftNeighbor.Width = width;
            else
            {
                width = leftNeighbor.Width;
                leftNeighbor.Width = new GridLength(0);
            }

        }
    }
}
