using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WpfComponent.Common
{
    public interface IObjectReceiver
    {
        void Receive(Dictionary<string, object> Objects);
        //Controller GetController();
    }
}
