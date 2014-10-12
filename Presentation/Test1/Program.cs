using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Test1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine(System.Environment.MachineName);
            /*
            ELib.ConfigManager.EConfig ec = new ELib.ConfigManager.EConfig();
            string s = ec.Test1();
            Console.WriteLine(s);
            */

            ELib.ConfigManager.EConfig ec = new ELib.ConfigManager.EConfig("test.config");
            
            string s1 = ec.Get("a1");
            Console.WriteLine(s1);
            string RL = Console.ReadLine();
        }
    }
}
