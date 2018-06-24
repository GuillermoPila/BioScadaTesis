using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfComponent.ToolBox
{
    public class Toolbox : ItemsControl
    {
        // Defines the ItemHeight and ItemWidth properties of
        // the WrapPanel used for this Toolbox
        public Size ItemSize
        {
            get { return itemSize; }
            set { itemSize = value; }
        }
        private Size itemSize = new Size(50,50);

        // Creates or identifies the element that is used to display the given item.        
        protected override DependencyObject GetContainerForItemOverride()
        {
            ToolboxItem temp = new ToolboxItem();
//            temp.ItemNameG = "GUille";
            return temp;
        }

        //Prepares the specified element to display the specified item. 
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            ToolboxItem aux = (element as ToolboxItem);
            aux.ItemName = item.GetType().Name;
            base.PrepareContainerForItemOverride(aux, item);
            
        }

        // Determines if the specified item is (or is eligible to be) its own container.        
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is ToolboxItem);
        }
    }
}
