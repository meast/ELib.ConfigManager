using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ELib.ConfigManager.Entity
{
    [Serializable]
    public class section
    {
        [XmlAttribute]
        public string name { get; set; }

        [XmlAttribute]
        public string id { get; set; }

        [XmlAttribute]
        public string value { get; set; }
    }
}
