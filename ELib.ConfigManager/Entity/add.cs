using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ELib.ConfigManager.Entity
{
    [Serializable]
    public class add
    {
        [XmlAttribute("key")]
        public string key { get; set; }
        [XmlAttribute("value")]
        public string value { get; set; }
    }
}
