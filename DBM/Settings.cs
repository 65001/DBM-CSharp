// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.Collections.Generic;
using Microsoft.SmallBasic.Library;
using System.IO;

namespace DBM
{
	[SmallBasicType]
	public static class Settings
	{
        static List<string> NullSettings = new List<string>()
        {
            "Listview_Width","Listview_Height","VersionID","Extensions","Language","Transaction_Query","Transaction_Commands","Deliminator","TimeOut","LastFolder"
            ,"OS_Dir","Asset_Dir","Log_DB_Path","Transaction_DB","Font_Size"
        };

		public static void LoadSettings(bool RestoreSettings)
		{
            Utilities.AddtoStackTrace("Settings.LoadSettings(" + RestoreSettings+ ")");
            if (RestoreSettings == false && System.IO.File.Exists(GlobalStatic.SettingsPath))
			{
				GlobalStatic.Settings = System.IO.File.ReadAllText(GlobalStatic.SettingsPath);
			}
            SettingsToFields();

            Dictionary<string, Primitive> Defaults = new Dictionary<string, Primitive>
            {
                { "Listview_Width", Desktop.Width - 400 },
                { "Listview_Height", Desktop.Height - 150 },
                { "VersionID", GlobalStatic.VersionID },
                { "Extensions", "1=db;2=sqlite;3=sqlite3;4=db3;5=*;" },
                { "Language", "en" },
                { "Transaction_Query", false },
                { "Transaction_Commands", false },
                { "Deliminator", "," },
                { "TimeOut", 10000 },
                { "LastFolder", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
                { "OS_Dir", Environment.SystemDirectory },
                { "Asset_Dir", Program.Directory + "\\Assets\\" },
                { "Log_DB_Path", Program.Directory + "\\Assets\\Log.db" },
                { "Transaction_DB", Program.Directory + "\\Assets\\Transactions.db" },
                { "Font_Size", 12 }
            };
            Primitive Setting_Files = "13=1;14=1;";
            Primitive Settings_Directories = "10=1;11=1;12=1;";

            if(NullSettings.Count != Defaults.Count) { throw new ApplicationException("Disparity between NullSettings Values and Default Values. Could indicate Settings Corruption."); }

			for (int i = 1; i <= NullSettings.Count; i++) //Sets Default Settings for files if they do not yet exist!!
			{
                string Key = NullSettings[i - 1];
                string Value = GlobalStatic.Settings[Key];

                if (ReplaceWithDefault(Value, Setting_Files[i], Settings_Directories[i]))
                {
                    GlobalStatic.Settings[Key] = Defaults[Key];
                    GlobalStatic.RestoreSettings = true; RestoreSettings = true;
                }
			}

            if (RestoreSettings)
			{
                SettingsToFields();
			}
			SaveSettings();
		}

        static bool ReplaceWithDefault(string Value, int isFile, int isDirectory)
        {
            if (string.IsNullOrWhiteSpace(Value)) return true;
            if (isFile == 1 && System.IO.File.Exists(Value) == false) return true;
            else if (isDirectory == 1 && Directory.Exists(Value) == false) return true;
            return false;
        }

        static void SettingsToFields()
        {
            GlobalStatic.Listview_Width = GlobalStatic.Settings["Listview_Width"];
            GlobalStatic.Listview_Height = GlobalStatic.Settings["Listview_Height"];
            GlobalStatic.LastVersion = GlobalStatic.Settings["VersionID"];
            GlobalStatic.LastFolder = GlobalStatic.Settings["LastFolder"];
            GlobalStatic.Extensions = GlobalStatic.Settings["Extensions"];
            GlobalStatic.Deliminator = GlobalStatic.Settings["Deliminator"];
            GlobalStatic.LanguageCode = GlobalStatic.Settings["Language"];

            GlobalStatic.EULA_Acceptance = GlobalStatic.Settings["EULA"];
            GlobalStatic.EULA_UserName = GlobalStatic.Settings["EULA_By"];

            GlobalStatic.AssetPath = GlobalStatic.Settings["Asset_Dir"];
            GlobalStatic.LogDBpath = GlobalStatic.Settings["Log_DB_Path"];
            GlobalStatic.TransactionDBPath = GlobalStatic.Settings["Transaction_DB"];
            GlobalStatic.Transaction_Query = GlobalStatic.Settings["Transaction_Query"];
            GlobalStatic.Transaction_Commands = GlobalStatic.Settings["Transaction_Commands"];
            GlobalStatic.DefaultFontSize = GlobalStatic.Settings["Font_Size"];
        }

		public static void SaveSettings()
		{
            Utilities.AddtoStackTrace("Settings.SaveSettings()");
			if (System.IO.File.Exists(GlobalStatic.SettingsPath) == false ||  GlobalStatic.Settings.EqualTo(System.IO.File.ReadAllText(GlobalStatic.SettingsPath)) == false)
			{
                try
                {
                    System.IO.File.WriteAllText(GlobalStatic.SettingsPath, GlobalStatic.Settings);
                }
                catch (Exception) //Settings could not be saved for some reason!
                {
                    Events.LogMessage(Utilities.Localization["Failed Save Settings"], Utilities.Localization["UI"]);
                    GraphicsWindow.ShowMessage(Utilities.Localization["Failed Save Settings"], Utilities.Localization["Error"]);
                }
			}
		}

		public static void Paths(string AssetPath,string PluginPath,string LocalizationFolder,string AutoRunPluginPath,string Localization_LanguageCodes_Path,string AutoRunPluginMessage)
		{
            Utilities.AddtoStackTrace("Settings.Paths()");
			if (Directory.Exists(AssetPath) == false || Directory.Exists(LocalizationFolder) == false) //Creates Folders if one is missing
			{
				Directory.CreateDirectory(AssetPath);
				Directory.CreateDirectory(LocalizationFolder);
				Directory.CreateDirectory(Localization_LanguageCodes_Path);
			}

			if (System.IO.File.Exists(AutoRunPluginPath) == false)
			{
				System.IO.File.WriteAllText(AutoRunPluginPath, AutoRunPluginMessage);
			}
		}


		public static void IniateDatabases()
		{
            Utilities.AddtoStackTrace( "Settings.IniateDatabases()");
            /*
			GlobalStatic.LogDB = LDDataBase.ConnectSQLite(GlobalStatic.LogDBpath);
			GlobalStatic.TransactionDB = LDDataBase.ConnectSQLite(GlobalStatic.TransactionDBPath);
            */

            GlobalStatic.TransactionDB =Engines.Load.Sqlite(GlobalStatic.TransactionDBPath,"Transaction Log");
            GlobalStatic.LogDB = Engines.Load.Sqlite(GlobalStatic.LogDBpath, "Master Log");
            
			Engines.Command(GlobalStatic.LogDB, GlobalStatic.LOGSQL, GlobalStatic.UserName, "Auto Creation Statements", false);
			Engines.Command(GlobalStatic.LogDB, GlobalStatic.LOGSQLVIEW , GlobalStatic.UserName, "Auto Creation Statements", false);
			Engines.Command(GlobalStatic.TransactionDB, GlobalStatic.TransactionsSQL , GlobalStatic.UserName, "Auto Creation Statements", false);
		}
	}
}