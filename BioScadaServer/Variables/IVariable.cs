using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioScadaServer.Variables
{
    public interface IVariable
    {
        string Name { get; set; }
        object Value { get; set; }
    }
}
