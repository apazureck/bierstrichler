using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace DiagramDesigner
{
    public class MoveThumb : Thumb
    {
        private RotateTransform rotateTransform;
        private ContentControl designerItem;

        public MoveThumb()
        {
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as ContentControl;

            if (this.designerItem != null)
            {
                this.rotateTransform = this.designerItem.RenderTransform as RotateTransform;
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null)
            {
                Point dragDelta = new Point(e.HorizontalChange, e.VerticalChange);

                if (this.rotateTransform != null)
                {
                    dragDelta = this.rotateTransform.Transform(dragDelta);
                }

                Canvas canvas = this.designerItem.Parent as Canvas;
                if (canvas == null)
                    return;

                Canvas.SetLeft(this.designerItem, Canvas.GetLeft(this.designerItem) + dragDelta.X);
                Canvas.SetTop(this.designerItem, Canvas.GetTop(this.designerItem) + dragDelta.Y);

                if (Canvas.GetLeft(this.designerItem) < 0)
                    Canvas.SetLeft(this.designerItem, 0);
 
                if (Canvas.GetLeft(this.designerItem) + this.ActualWidth > canvas.ActualWidth)
                    Canvas.SetLeft(this.designerItem, canvas.ActualWidth - this.ActualWidth);
 
                if (Canvas.GetTop(this.designerItem) < 0)
                    Canvas.SetTop(this.designerItem, 0);
 
                if (Canvas.GetTop(this.designerItem) + this.ActualHeight > canvas.ActualHeight)
                    Canvas.SetTop(this.designerItem, canvas.ActualHeight - this.ActualHeight);

            }
        }
    }
}
