using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace WpfComponent.Common
{
    public class ObjectTransmitter : Component
    {
        public class ReceiverItem
        {
            public FrameworkElement Receiver { get; set; }
        }

        private Collection<ReceiverItem> receivers = new Collection<ReceiverItem>();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Collection<ReceiverItem> Receivers
        {
            get { return receivers; }
            set { receivers = value; }
        }

        protected void TransmitTo(IObjectReceiver receiver, Dictionary<string, object> Objects)
        {

            
            
                receiver.Receive(Objects);
            /*
            if (ControllerFrom.IsMasteredBy(ControllerDest, out FullPrefix))
            {
                //put header
                Dictionary<string, object> toSend = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> o in Objects)
                    toSend.Add(FullPrefix + o.Key, o.Value);
                receiver.Receive(toSend);
            }
            else if (ControllerDest.IsMasteredBy(ControllerFrom, out FullPrefix))
            {
                //delete header
                int fullPrefixLength = FullPrefix.Length;
                Dictionary<string, object> toSend = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> o in Objects)
                {
                    if (o.Key.IndexOf(FullPrefix) == 0)
                        toSend.Add(o.Key.Substring(fullPrefixLength), o.Value);
                    else
                        toSend.Add(o.Key, o.Value);
                }
                receiver.Receive(toSend);
            }
            else
                throw new Exception("Error. Components located at unrelated Controllers can not communicate.");*/
        }

        protected void Transmit(Dictionary<string, object> Objects)
        {
            foreach (ReceiverItem item in Receivers)
                if (item.Receiver is IObjectReceiver)
                    TransmitTo(item.Receiver as IObjectReceiver, Objects);
        }

        protected void Transmit(string Name, object Value)
        {
            Transmit(new Dictionary<string, object>() { { Name, Value } });
        }
    }
}
