using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfComponent.Common
{
    public interface IVariableChangeReceptor
    {
        void VariableChanged(object newValue);
    }
}
