using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;
using BioScadaServer.Variables;
using BioScadaServer.Tools;


namespace BioScadaServer.BioScada
{
    [Serializable]
    [XmlRoot("Variables")]
    public class Experiment
    {
        public Experiment()
        {
            _Variables = new List<Variable>();
        }

        public Experiment(Variable[] var)
        {
            _Variables = var.ToList();
            for (int i = 0; i < _Variables.Count; i++)
            {
                _Variables[i].Experiment.Add(this);
            }
        }


        private List<Variable> _Variables;
        [XmlArrayAttribute("Items")]
        public List<Variable> Variables { get { return _Variables; } set { _Variables = value; } }

        private string name;
        public string Name { get { return name; } set { name = value; } }

        private bool isEnable = false;
        public void Start()
        {
            if (!isEnable)
            {
                for (int i = 0; i < _Variables.Count; i++)
                {
                    _Variables[i].Start();
                }

                isEnable = true;
            }
        }
        public void Stop()
        {
            if (isEnable)
            {
                isEnable = false;
                for (int i = 0; i < _Variables.Count; i++)
                {
                    _Variables[i].Stop();
                }
            }
        }
    }
}
