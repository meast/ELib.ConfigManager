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
        [XmlAttribute("name")]
        public string value { get; set; }
    }
}
