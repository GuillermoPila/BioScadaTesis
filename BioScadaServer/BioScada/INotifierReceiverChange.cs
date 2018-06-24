using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioScadaServer.BioScada
{
    public interface INotifierReceiverChange
    {
        void ReceiverChange(ChangeNotification notifier);
    }
}
