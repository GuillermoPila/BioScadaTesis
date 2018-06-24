using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BioScadaServer.BioScada
{
    [Serializable]
    public class ChangeNotification
    {
        [Serializable]
        public class ItemVar
        {
            public string Name;
            public object NewValue;
        }

        public ChangeNotification(DateTime when, ItemVar item)
        {
            this.when = when;
            this.item = item;

        }

        private DateTime when;
        public DateTime When
        {
            get { return when; }
            set { when = value; }
        }

        private ItemVar item;
        public ItemVar Item
        {
            get { return item; }
            set { item = value; }
        }
    }
}
