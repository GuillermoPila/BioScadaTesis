using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioScadaScript
{
    public class VariableCalculated
    {
        private string name;
        public string Name { get { return name; } set { name = value; } }

        private string code;
        public string Code { get { return code; } set { code = value; } }

        private string expresion;
        public string Expresion { get { return expresion; } set { expresion = value; } }

        private object value;
        public object Value { get { return value; } set { this.value = value; } }
    }
}
