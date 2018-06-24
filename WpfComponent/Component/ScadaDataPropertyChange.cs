using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using WpfComponent.Common;

namespace WpfComponent.Component
{
    public class ScadaDataPropertyChange : ObjectTransmitter, IObjectReceiver
    {
        

        private Collection<Item> items = new Collection<Item>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<Item> Items { get { return items; } set { items = value; } }

        
        public void Receive(Dictionary<string, object> Objects)
        {
            
            var query = from obj in Objects
                        join item in Items on obj.Key equals item.VariableName
                        select new { item, obj.Value };

            foreach (var item in query)
            {
                item.item.VariableChanged(item.Value);
            }
            
        }

        public ScadaDataPropertyChange()
        {
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            Typeface typeface = new Typeface("Times New Roman");
            FormattedText text = new FormattedText("DataPropertyChange", CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight, typeface, 12, Brushes.Black);

            drawingContext.DrawText(text, new Point(0, 0));
        }
    }

    public class Item : IVariableChangeReceptor
    {

        private static object lckObj = new object();
        public FrameworkElement Control { get; set; }
        public string PropertyName { get; set; }
        public string VariableName { get; set; }
        public override string ToString()
        {
            if (Control != null)
                return string.Format("{0}->{1}.{2}", VariableName, Control.Name, PropertyName);
            else
                return "Unasigned";
        }

        public void VariableChanged(object newValue)
        {
            lock (lckObj)
            {

                FrameworkElement control = Control;
                if (control == null)
                    return;
                PropertyInfo property = control.GetType().GetProperty(PropertyName);
                if (property == null)
                    return;


                object previousValue = null;

                control.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                          (ThreadStart)(() => previousValue = property.GetValue(control, null)));

                if (previousValue != null && previousValue.Equals(newValue))
                    return;
                if (newValue == null)
                    return;
                object convertedValue = Convert.ChangeType(newValue, property.PropertyType);

                control.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                          (ThreadStart)(() => property.SetValue(Control, convertedValue, null)));

            }
        }

    }
}
