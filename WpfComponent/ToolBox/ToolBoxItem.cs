using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace WpfComponent.ToolBox
{
    public class ToolboxItem : ContentControl
    {
        // caches the start point of the drag operation
        private Point? dragStartPoint = null;

        static ToolboxItem()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ToolboxItem), new FrameworkPropertyMetadata(typeof(ToolboxItem)));
        }

        public static readonly DependencyProperty ItemNameProperty =
            DependencyProperty.Register("ItemNameProperty", typeof(string),
            typeof(ToolboxItem), new UIPropertyMetadata("Guiklldsg"));

        public string ItemName
        {
            get { return (string)GetValue(ItemNameProperty); }
            set { SetValue(ItemNameProperty, value); }
        }


         public string ItemNameG
        {
            get { return itemName; }
            set { itemName= value; }
        }
        private string itemName="sdgsdgs"; 

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            this.dragStartPoint = new Point?(e.GetPosition(this));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
                this.dragStartPoint = null;

            if (this.dragStartPoint.HasValue)
            {
                // XamlWriter.Save() has limitations in exactly what is serialized,
                // see SDK documentation; short term solution only;
                string xamlString = XamlWriter.Save(this.Content);
                DragObject dataObject = new DragObject();
                dataObject.Xaml = xamlString;

                FrameworkElement control = this.Content as FrameworkElement;
                if (control != null)
                {
                    double scale = 1;// 1.3;
                    dataObject.DesiredSize = new Size(control.Width * scale, control.Height * scale);
                }

                //WrapPanel panel = VisualTreeHelper.GetParent(this) as WrapPanel;
                //if (panel != null)
                //{
                //    // desired size for DesignerCanvas is the stretched Toolbox item size
                //    double scale = 1.3;
                //    dataObject.DesiredSize = new Size(panel.ItemWidth * scale, panel.ItemHeight * scale);
                //}

                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);

                e.Handled = true;
            }
        }
    }

    public class DragObject
    {
        // Xaml string that represents the serialized content
        public String Xaml { get; set; }

        // Defines width and height of the DesignerItem
        // when this DragObject is dropped on the DesignerCanvas
        public Size? DesiredSize { get; set; }
    }

}
