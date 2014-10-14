ELib.ConfigManager
==================

dotnet base configuration manager per machine.config diffrent value for diffrent machine by a config file.

find config value by machine name.use the default machine when the config key is not exists in current machine.
the machine name is case sensitive.

if the config file is not exists, it will generate a sample file for you.just confirm the path of the file is exists and writable.

Usage:


ELib.ConfigManager.EConfig ec = new ELib.ConfigManager.EConfig("~/bin/elib.config");

string a1 = ec.Get("a1");

  the first two character  "~/" will be parse as System.Web.HttpContext.Current.Server.MapPath("/").

string a1 = ELib.ConfigManager.EConfig.GetInstance("~/bin/elib.config").Get("a1");

  if the config key a1 is not exists in current machine , it will return the a1 in the default machine.
  
string a2 = ELib.ConfigManager.EConfig.GetInstance("|DataDirectorY|elib.config").Get("a2");

  the first 15 character "|DataDirectorY|" ignore case will be parse as "~/app_data/" in lower case,and then parse the "~/" as  System.AppDomain.CurrentDomain.BaseDirectory + CFile.Substring(2).
  
  
Name your host as default is not recommended.
