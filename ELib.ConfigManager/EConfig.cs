using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace ELib.ConfigManager
{
    /// <summary>
    /// config values by different machinename
    /// </summary>
    public class EConfig
    {
        public string ConfigFile = "";
        public string MachineName = "";
        public Entity.config Configs;
        public Entity.machine Config;

        public static EConfig obj;

        /// <summary>
        /// do nothing
        /// </summary>
        public EConfig()
        {
        }

        /// <summary>
        /// init with config file
        /// </summary>
        /// <param name="CFile"></param>
        public EConfig(string CFile)
        {
            this.MachineName = System.Environment.MachineName;
            CFile = this.ParseCFile(CFile);
            if (!System.IO.File.Exists(CFile))
            {
                try
                {
                    string DefaultText = this.Test1();
                    System.IO.File.WriteAllText(CFile, DefaultText, Encoding.UTF8);
                }
                catch { }
            }
            if (System.IO.File.Exists(CFile))
            {
                this.ConfigFile = CFile;
                XmlSerializer xs = new XmlSerializer(typeof(Entity.config));
                FileStream fs = new FileStream(CFile, FileMode.Open);
                this.Configs = (Entity.config)xs.Deserialize(fs);
                fs.Close();
                this.Config = this.Configs.machines.Find(p => p.name == this.MachineName);
            }
        }

        /// <summary>
        /// get exists or create new instance
        /// </summary>
        /// <returns></returns>
        public static EConfig GetInstance()
        {
            if (EConfig.obj == null) EConfig.obj = new EConfig();
            return EConfig.obj;
        }

        /// <summary>
        /// instance creator
        /// </summary>
        /// <param name="CFile"></param>
        /// <returns></returns>
        public static EConfig GetInstance(string CFile)
        {
            if (EConfig.obj == null) EConfig.obj = new EConfig(CFile);
            return obj;
        }

        /// <summary>
        /// set config file,you must confirm the config file is exists and can be read.
        /// using: EConfig.GetInstance().SetConfigFile(ConfigFile)
        /// </summary>
        /// <param name="CFile">config file path</param>
        /// <returns>EConfig</returns>
        public EConfig SetConfigFile(string CFile)
        {
            if (EConfig.obj == null) EConfig.obj = new EConfig(CFile);
            else
            {
                if (EConfig.obj.ConfigFile != CFile)
                {
                    CFile = this.ParseCFile(CFile);
                    if (System.IO.File.Exists(CFile))
                    {
                        this.ConfigFile = CFile;
                        XmlSerializer xs = new XmlSerializer(typeof(Entity.config));
                        FileStream fs = new FileStream(CFile, FileMode.Open);
                        this.Configs = (Entity.config)xs.Deserialize(fs);
                        fs.Close();
                        this.Config = this.Configs.machines.Find(p => p.name == this.MachineName);
                    }
                }
            }
            return EConfig.obj;
        }

        /// <summary>
        /// get config value by key,using this machinename or default
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public string Get(string Key)
        {
            string Result = "";
            Entity.add a = new Entity.add();
            if (this.Config != null)
                a = this.Config.configs.Find(p => p.key == Key);
            if (a == null || String.IsNullOrEmpty(a.key))
            {
                try { a = this.Configs.machines.Find(p => p.name.ToLower() == "default").configs.Find(r => r.key == Key); }
                catch { }
            }
            if (a != null) Result = a.value;
            return Result;
        }

        /// <summary>
        /// get config value by key and machinename
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="MachineName"></param>
        /// <returns></returns>
        public string Get(string Key, string MachineName)
        {
            if (this.Configs == null) return "";
            Entity.add a = this.Configs.machines.Find(p => p.name == MachineName).configs.Find(r => r.key == Key);
            if (a != null && !String.IsNullOrEmpty(a.key)) return a.value;
            return "";
        }

        /// <summary>
        /// build the default sample xml content
        /// </summary>
        /// <returns></returns>
        public string Test1()
        {
            string Result = "";
            try
            {
                Entity.add a1 = new Entity.add();
                a1.key = "a1";
                a1.value = "v1";
                Entity.add a2 = new Entity.add();
                a2.key = "a2";
                a2.value = "v2";
                Entity.add a3 = new Entity.add();
                a3.key = "isdebug";
                a3.value = "true";

                Entity.machine m1 = new Entity.machine();
                m1.name = "default";
                m1.configs = new List<Entity.add>();
                m1.configs.Add(a1);
                m1.configs.Add(a2);

                Entity.machine m2 = new Entity.machine();
                m2.name = "host-1";
                m2.configs = new List<Entity.add>();
                m2.configs.Add(a1);
                m2.configs.Add(a2);
                m2.configs.Add(a3);

                Entity.machine m3 = new Entity.machine();
                m3.name = this.MachineName;
                m3.configs = new List<Entity.add>();
                m3.configs.Add(a1);

                Entity.config c = new Entity.config();
                c.machines = new List<Entity.machine>();
                c.machines.Add(m1);
                c.machines.Add(m2);
                c.machines.Add(m3);

                XmlSerializer xs = new XmlSerializer(typeof(Entity.config));
                MemoryStream ms = new MemoryStream();
                XmlWriterSettings xws = new XmlWriterSettings();
                // UTF-8 encoding without BOM
                xws.Encoding = new UTF8Encoding(false);
                xws.Indent = true;
                using (XmlWriter xw = XmlWriter.Create(ms, xws))
                {
                    xs.Serialize(xw, c);
                }
                Result = Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                Result = ex.Message + "\n" + ex.Source + "\n" + ex.StackTrace;
            }
            return Result;
        }

        /// <summary>
        /// parse special chars such as "|datadirectory|" and "~/" in web application 
        /// </summary>
        /// <param name="CFile"></param>
        /// <returns></returns>
        private string ParseCFile(string CFile)
        {
            if (!String.IsNullOrEmpty(CFile))
            {
                if (CFile.StartsWith("|DataDirectory|", StringComparison.OrdinalIgnoreCase))
                {
                    CFile = "~/app_data/" + CFile.Substring(15);
                }
            }
            if (CFile.StartsWith("~/"))
            {
                CFile = System.AppDomain.CurrentDomain.BaseDirectory + CFile.Substring(2);
            }
            return CFile;
        }

    }
}
