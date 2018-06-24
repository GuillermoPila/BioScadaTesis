using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace WpfComponent.Common
{
    public interface IObjectTransmitter
    {
        Collection<Component> Receivers { get; }
    }
}
