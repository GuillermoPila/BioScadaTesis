using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfComponent.Common
{
    public class Component : FrameworkElement, IVariableChangeReceptor
    {
        public Component()
        {
          
        }

        protected virtual void ValueChange(object newValue){}

        public void VariableChanged(object newValue)
        {
            ValueChange(newValue);
        }
    }
}
