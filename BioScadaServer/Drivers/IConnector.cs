using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioScadaServer.Drivers
{
    public interface IConnector
    {
        string ConnectorName { get; set; }
        object ConnectValue { get; set; }

        Driver Driver { get; }

        //int ShowPeriod { get; set; }
        bool Read();
        bool Write(object obj);
    }
}
