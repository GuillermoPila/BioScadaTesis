using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioScadaServer.Drivers
{
    public interface Driver
    {
        void Initialize();
        bool Write(IConnector connect, object value);
        List<IConnector> Connectors { get; set; }

        void ServerRequestStopDriver();
        void ServerRequestStartDriver();
        
        void Fin();

    }
}
