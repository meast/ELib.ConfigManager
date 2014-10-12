using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ELib.ConfigManager.Entity
{
    [Serializable]
    [XmlRoot]
    public class config
    {
        [XmlArray]
        public List<machine> machines { get; set; }
    }
}
