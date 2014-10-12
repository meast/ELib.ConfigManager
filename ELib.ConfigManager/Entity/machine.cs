using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ELib.ConfigManager.Entity
{
    [Serializable]
    public class machine
    {
        [XmlAttribute]
        public string name { get; set; }
        
        [XmlArray]
        public List<add> configs { get; set; }
        
        [XmlIgnore]
        public bool IsDebug
        {
            get
            {
                string s = this.configs.Find(p => p.key.ToLower() == "isdebug").value;
                if (s.ToLower() == "true") return true;
                return false;
            }
        }
    }
}
