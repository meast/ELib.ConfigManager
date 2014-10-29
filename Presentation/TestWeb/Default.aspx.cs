using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestWeb
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            /*
             * usage:
             *  1. ELib.ConfigManager.EConfig ec = new ELib.ConfigManager.EConfig("~/bin/elib.config");
             *  2. ELib.ConfigManager.EConfig.GetInstance("~/bin/elib.config").Get("a1")
             *     ELib.ConfigManager.EConfig.GetInstance("|DataDirectorY|elib.config").Get("a2")
             *  3. ELib.ConfigManager.EConfig.GetInstance().SetConfigFile("~/bin/elib.config").Get("a2");
             * */

            string a1 = ELib.ConfigManager.EConfig.GetInstance("~/bin/elib.config").Get("a1");
            string a2 = ELib.ConfigManager.EConfig.GetInstance().SetConfigFile("~/bin/elib.config").Get("a2");
            this.Label1.Text = String.Format("the value of a1 is : {0}, the value of a2 is : {1}", a1, a2);
            
            
            // test the |DataDirectory|
            string a3 = ELib.ConfigManager.EConfig.GetInstance("|DataDirectory|elib.config").Get("a1");
            string a4 = ELib.ConfigManager.EConfig.GetInstance("|DataDirectorY|elib.config").Get("a2");
            this.Label2.Text = String.Format("the value of a1 is : {0}, the value of a2 is : {1}", a3, a4);
            
        }
    }
}