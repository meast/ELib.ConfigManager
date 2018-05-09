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

        public static Dictionary<string, EConfig> obj = new Dictionary<string,EConfig>();

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
            CFile = ParseCFile(CFile);
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
            return new EConfig();
        }

        /// <summary>
        /// instance creator
        /// </summary>
        /// <param name="CFile"></param>
        /// <returns></returns>
        public static EConfig GetInstance(string CFile)
        {
            CFile = ParseCFile(CFile);
            string hashkey = MD5(CFile);
            if (!EConfig.obj.ContainsKey(hashkey) || EConfig.obj[hashkey] == null) EConfig.obj[hashkey] = new EConfig(CFile);
            return EConfig.obj[hashkey];
        }

        /// <summary>
        /// set config file,you must confirm the config file is exists and can be read.
        /// using: EConfig.GetInstance().SetConfigFile(ConfigFile)
        /// </summary>
        /// <param name="CFile">config file path</param>
        /// <returns>EConfig</returns>
        public EConfig SetConfigFile(string CFile)
        {
            CFile = ParseCFile(CFile);
            string hashkey = MD5(CFile);
            if (!EConfig.obj.ContainsKey(hashkey) || EConfig.obj[hashkey] == null) EConfig.obj[hashkey] = new EConfig(CFile);
            return EConfig.obj[hashkey];
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

                Entity.section sec1 = new Entity.section();
                sec1.name = "sec-name";
                sec1.id = "sec-id-1";
                sec1.value = "sec-val-1";

                Entity.section sec2 = new Entity.section();
                sec2.name = "sec-name";
                sec2.id = "sec-id-2";
                sec2.value = "sec-val-2";

                Entity.sections sec = new Entity.sections();
                sec.name = "sec-names";
                sec.sectionitems = new List<Entity.section>();
                sec.sectionitems.Add(sec1);
                sec.sectionitems.Add(sec2);

                Entity.sections secs = sec;
                secs.name = "sec-namess";


                Entity.machine m1 = new Entity.machine();
                m1.name = "default";
                m1.configs = new List<Entity.add>();
                m1.configs.Add(a1);
                m1.configs.Add(a2);
                m1.sectionslist = new List<Entity.sections>();
                m1.sectionslist.Add(sec);
                m1.sectionslist.Add(secs);         

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
        public static string ParseCFile(string CFile)
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

        /// <summary>
        /// MD5 hash
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5(string str)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(str));
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// SHA1 hash
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SHA1(string str)
        {
            System.Security.Cryptography.SHA1CryptoServiceProvider sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] data = sha1.ComputeHash(System.Text.Encoding.Default.GetBytes(str));
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("X2"));
            }
            return sb.ToString();
            //return System.Text.Encoding.GetEncoding("GB2312").GetString(new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(System.Text.Encoding.GetEncoding("GB2312").GetBytes(str)));
        }
    }
}
