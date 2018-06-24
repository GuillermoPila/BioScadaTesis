using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Activities;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.Runtime;
using System.CodeDom.Compiler;





namespace WpfComponent.Property
{
   
    public class PropertyGrid : Control
    {
        // Fields
        public static readonly DependencyProperty InstanceProperty =
            DependencyProperty.Register("Instance", typeof(object), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnInstanceChanged),
                                                    new CoerceValueCallback(PropertyGrid.CoerceInstance)));
        public static readonly DependencyProperty NullInstanceProperty =
            DependencyProperty.Register("NullInstance", typeof(object), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PropertyGrid.OnNullInstanceChanged)));

        public static readonly DependencyProperty PropertiesProperty =
            DependencyProperty.Register("Properties", typeof(ObservableCollection<PropertyBase>), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(new ObservableCollection<PropertyBase>(),
                                                new PropertyChangedCallback(PropertyGrid.OnPropertiesChanged)));

        // Methods
        static PropertyGrid()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        private static object CoerceInstance(DependencyObject d, object value)
        {
            PropertyGrid propertyGrid = d as PropertyGrid;
            if (value == null)
            {
                return propertyGrid.NullInstance;
            }
            return value;
        }

        private static void OnInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = d as PropertyGrid;
            if (e.NewValue == null)
            {
                propertyGrid.Properties = new ObservableCollection<PropertyBase>();
            }
            else
            {
                propertyGrid.Properties = propertyGrid.Parse(e.NewValue);
            }
        }

        private static void OnNullInstanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void OnPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid propertyGrid = d as PropertyGrid;

            ObservableCollection<PropertyBase> properties = e.OldValue as ObservableCollection<PropertyBase>;
            foreach (PropertyBase item in properties)
            {
                item.Dispose();
            }
        }

        // Properties
        public object Instance
        {
            get
            {
                return base.GetValue(InstanceProperty);
            }
            set
            {
                base.SetValue(InstanceProperty, value);
            }
        }

        public object NullInstance
        {
            get
            {
                return base.GetValue(NullInstanceProperty);
            }
            set
            {
                base.SetValue(NullInstanceProperty, value);
            }
        }

        public ObservableCollection<PropertyBase> Properties
        {
            get
            {
                return (ObservableCollection<PropertyBase>)base.GetValue(PropertiesProperty);
            }
            set
            {
                base.SetValue(PropertiesProperty, value);
            }
        }


        private ObservableCollection<PropertyBase> Parse(object instance)
        {
            ObservableCollection<PropertyBase> Items = new ObservableCollection<PropertyBase>();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);

            Dictionary<string, PropertyCategory> groups = new Dictionary<string, PropertyCategory>();
            List<PropertyItem> propertyCollection = new List<PropertyItem>();

            foreach (PropertyDescriptor propertyDescriptor in properties)
            {
                this.CollectProperties(instance, propertyDescriptor, propertyCollection);
            }

            foreach (PropertyItem property in propertyCollection)
            {
                PropertyCategory propertyCategory;
                if (groups.ContainsKey(property.Category))
                {
                    propertyCategory = groups[property.Category];
                }
                else
                {
                    propertyCategory = new PropertyCategory(property.Category);
                    groups[property.Category] = propertyCategory;
                    Items.Add(propertyCategory);
                }

                propertyCategory.Items.Add(property);
            }

            return Items;
        }

        private void CollectProperties(object instance, PropertyDescriptor descriptor, List<PropertyItem> propertyCollection)
        {
            if (descriptor.Attributes[typeof(FlatAttribute)] == null)
            {
              //  if (descriptor.Attributes[typeof(CLSCompliantAttribute)] != null)
                if (descriptor.IsBrowsable)
                {
                    PropertyItem property = new PropertyItem(instance, descriptor);
                    propertyCollection.Add(property);
                }
            }
            else
            {
                instance = descriptor.GetValue(instance);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);
                foreach (PropertyDescriptor propertyDescriptor in properties)
                {
                    this.CollectProperties(instance, propertyDescriptor, propertyCollection);
                }
            }
        }
    }

    public class PropertyTemplateSelector : DataTemplateSelector
    {
        // Methods
        private DataTemplate FindDataTemplate(PropertyItem property, FrameworkElement element)
        {
            Type propertyType = property.PropertyType;
            DataTemplate template = TryFindDataTemplate(element, propertyType);
            while ((template == null) && (propertyType.BaseType != null))
            {
                propertyType = propertyType.BaseType;
                template = TryFindDataTemplate(element, propertyType);
            }
            if (template == null)
            {
                template = TryFindDataTemplate(element, "default");
            }
            return template;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            PropertyItem property = item as PropertyItem;
            if (property == null)
            {
                throw new ArgumentException("item must be of type Property");
            }
            FrameworkElement element = container as FrameworkElement;
            if (element == null)
            {
                return base.SelectTemplate(property.Value, container);
            }
            return this.FindDataTemplate(property, element);
        }

        private static DataTemplate TryFindDataTemplate(FrameworkElement element, object dataTemplateKey)
        {
            object dataTemplate = element.TryFindResource(dataTemplateKey);
            if (dataTemplate == null)
            {
                dataTemplateKey = new ComponentResourceKey(typeof(PropertyGrid), dataTemplateKey);
                dataTemplate = element.TryFindResource(dataTemplateKey);
            }
            return (dataTemplate as DataTemplate);
        }
    }

    public class PropertyItem : PropertyBase, IDisposable
    {
        // Fields
        private object _instance;
        private PropertyDescriptor _property;

        // Methods
        public PropertyItem(object instance, PropertyDescriptor property)
        {
            this._instance = instance;
            this._property = property;
            this._property.AddValueChanged(this._instance, new EventHandler(this.instance_PropertyChanged));
        }

        protected override void Dispose(bool disposing)
        {
            if (!base.Disposed)
            {
                if (disposing)
                {
                    this._property.RemoveValueChanged(this._instance, new EventHandler(this.instance_PropertyChanged));
                }
                base.Dispose(disposing);
            }
        }

        private void instance_PropertyChanged(object sender, EventArgs e)
        {
            base.NotifyPropertyChanged("Value");
        }

        // Properties
        public string Description
        {
            get
            {
                return this._property.Description;
            }
        }

        public string Category
        {
            get
            {
                return this._property.Category;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this._property.IsReadOnly;
            }
        }

        public bool IsWriteable
        {
            get
            {
                return !this.IsReadOnly;
            }
        }

        public string Name
        {
            get
            {
                return this._property.Name;
            }
        }

        public Type PropertyType
        {
            get
            {
                return this._property.PropertyType;
            }
        }

        public object Value
        {
            get
            {
                return this._property.GetValue(this._instance);
            }
            set
            {
                object currentValue = this._property.GetValue(this._instance);
                if ((value == null) || !value.Equals(currentValue))
                {
                    Type propertyType = this._property.PropertyType;
                    if (((propertyType == typeof(object)) || ((value == null) && propertyType.IsClass)) || ((value != null) && propertyType.IsAssignableFrom(value.GetType())))
                    {
                        this._property.SetValue(this._instance, value);
                    }
                    else
                    {
                        object convertedValue = TypeDescriptor.GetConverter(this._property.PropertyType).ConvertFrom(value);
                        this._property.SetValue(this._instance, convertedValue);
                    }
                }
            }
        }
    }

    public class PropertyBase : INotifyPropertyChanged, IDisposable
    {
        // Fields
        private bool _disposed = false;

        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        // Methods
        protected PropertyBase()
        {
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.Disposed)
            {
                this._disposed = true;
            }
        }

        ~PropertyBase()
        {
            this.Dispose(false);
        }

        protected void NotifyPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        // Properties
        protected bool Disposed
        {
            get
            {
                return this._disposed;
            }
        }
    }


    public class PropertyCategory : PropertyBaseList
    {
        // Fields
        private readonly string _categoryName;

        // Methods
        public PropertyCategory()
        {
            this._categoryName = "Misc";
        }

        public PropertyCategory(string categoryName)
        {
            this._categoryName = categoryName;
        }

        // Properties
        public string Category
        {
            get
            {
                return this._categoryName;
            }
        }
    }

    public class PropertyBaseList : PropertyBase
    {
        // Fields
        private readonly ObservableCollection<PropertyBase> _items = new ObservableCollection<PropertyBase>();

        // Methods
        protected PropertyBaseList()
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (!base.Disposed)
            {
                if (disposing)
                {
                    foreach (PropertyBase item in this.Items)
                    {
                        item.Dispose();
                    }
                }
                base.Dispose(disposing);
            }
        }

        // Properties
        public ObservableCollection<PropertyBase> Items
        {
            get
            {
                return this._items;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class FlatAttribute : Attribute
    {
    }

    public class EnumTypeConverter : IValueConverter
    {
        // Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetValues(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
