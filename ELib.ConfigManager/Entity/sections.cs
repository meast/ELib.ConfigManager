using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ELib.ConfigManager.Entity
{
    [Serializable]
    public class sections
    {
        [XmlAttribute]
        public string name { get; set; }
                       
        [XmlArray]
        public List<section> sectionitems { get; set; }
    }
}
