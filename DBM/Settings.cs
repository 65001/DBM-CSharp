// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
using SBFile = Microsoft.SmallBasic.Library.File;
using System.IO;

namespace DBM
{
	[SmallBasicType]
	public static class Settings
	{
		public static void LoadSettings(bool RestoreSettings)
		{
			//GlobalStatic.SettingsPath = "C:\\Users\\Abhishek\\Documents\\Proggraming\\SB\\Projects\\DB Manager\\Assets\\setting.txt"; //@Dev 
			LDList.Add(GlobalStatic.List_Stack_Trace, "Settings.LoadSettings()");
            if (RestoreSettings == false)
			{
				GlobalStatic.Settings = SBFile.ReadContents(GlobalStatic.SettingsPath);
			}
			GlobalStatic.Listview_Width = GlobalStatic.Settings["Listview_Width"];
			GlobalStatic.Listview_Height = GlobalStatic.Settings["Listview_Height"];
			GlobalStatic.LastVersion = GlobalStatic.Settings["VersionID"];
			GlobalStatic.LastFolder = GlobalStatic.Settings["LastFolder"];
			GlobalStatic.Extensions = GlobalStatic.Settings["Extensions"];
			GlobalStatic.Deliminator = GlobalStatic.Settings["Deliminator"];
			GlobalStatic.Transactions = GlobalStatic.Settings["Transactions"];
			GlobalStatic.LanguageCode = GlobalStatic.Settings["Language"];

			GlobalStatic.EULA_Acceptance = GlobalStatic.Settings["EULA"];
			GlobalStatic.EULA_Username = GlobalStatic.Settings["EULA_By"];
			GlobalStatic.EULA_Accepted_Version = GlobalStatic.Settings["EULA_Version"];

			GlobalStatic.DebugMode = GlobalStatic.Settings["debug_mode"];
			GlobalStatic.DebugParser = GlobalStatic.Settings["debug_parser"];

			GlobalStatic.AssetPath = GlobalStatic.Settings["Asset_Dir"];
			GlobalStatic.LogCSVpath = GlobalStatic.Settings["Log_Path"];
			GlobalStatic.LogDBpath = GlobalStatic.Settings["Log_DB_Path"];
			GlobalStatic.TransactionDBPath = GlobalStatic.Settings["Transaction_DB"];
			GlobalStatic.Transaction_Query = GlobalStatic.Settings["Transaction_Query"];
			GlobalStatic.Transaction_Commands = GlobalStatic.Settings["Transaction_Commands"];

			Primitive NullSettings = "1=Listview_Width;2=Listview_Height;3=VersionID;4=Extensions;5=Language;6=Transactions;7=Transaction_Query;8=Transaction_Commands;9=debug_parser;10=debug_mode;11=Deliminator;12=TimeOut;";
			NullSettings = NullSettings + "13=LastFolder;14=OS_Dir;15=Asset_Dir;16=Log_Path;17=Log_DB_Path;18=Transaction_DB;";

			Primitive Setting_Default = "1=" + (Desktop.Width - 400) + ";2=" + (Desktop.Height - 150) + ";3=" + GlobalStatic.VersionID + ";5=en;6=0;7=0;8=0;9=0;10=0;11=,;12=10000;";
			Setting_Default[4] = "1=db;2=sqlite;3=sqlite3;4=db3;5=*;";
			Setting_Default[13] = LDFile.DocumentsFolder; Setting_Default[14] = Environment.SystemDirectory;
			Setting_Default[15] = Program.Directory + "\\Assets\\";
			Setting_Default[16] = Setting_Default[15] + "Log.csv";
			Setting_Default[17] = Setting_Default[15] + "Log.db";
			Setting_Default[18] = Setting_Default[15] + "Transactions.db";
			Primitive Setting_Files = "15=1;16=1;17=1;18=1;";

			for (int i = 1; i <= SBArray.GetItemCount(NullSettings); i++) //Sets Default Settings for files if they do not yet exist!!
			{
				if (GlobalStatic.Settings[NullSettings[i]] == null || GlobalStatic.Settings[NullSettings[i]] == "")
				{
					GlobalStatic.Settings[NullSettings[i]] = Setting_Default[i];
                    GlobalStatic.RestoreSettings = true; RestoreSettings = true;
				}
				if (Setting_Files[i] == 1 && LDFile.Exists(GlobalStatic.Settings[NullSettings[i]]) == false)
				{
					GlobalStatic.Settings[NullSettings[i]] = Setting_Default[i];
					GlobalStatic.RestoreSettings = true;RestoreSettings = true;
				}


			}
            if (RestoreSettings == true)
			{
				GlobalStatic.Listview_Width = GlobalStatic.Settings["Listview_Width"]; GlobalStatic.Listview_Height = GlobalStatic.Settings["Listview_Height"]; GlobalStatic.LastVersion = GlobalStatic.Settings["VersionID"]; GlobalStatic.LastFolder = GlobalStatic.Settings["LastFolder"]; GlobalStatic.Extensions = GlobalStatic.Settings["Extensions"];
				GlobalStatic.Deliminator = GlobalStatic.Settings["Deliminator"]; GlobalStatic.Transactions = GlobalStatic.Settings["Transactions"]; GlobalStatic.LanguageCode = GlobalStatic.Settings["Language"];

				GlobalStatic.EULA_Acceptance = GlobalStatic.Settings["EULA"]; GlobalStatic.EULA_Username = GlobalStatic.Settings["EULA_By"]; GlobalStatic.EULA_Accepted_Version = GlobalStatic.Settings["EULA_Version"];

				GlobalStatic.DebugMode = GlobalStatic.Settings["debug_mode"]; GlobalStatic.DebugParser = GlobalStatic.Settings["debug_parser"];

				GlobalStatic.AssetPath = GlobalStatic.Settings["Asset_Dir"]; GlobalStatic.LogCSVpath = GlobalStatic.Settings["Log_Path"]; GlobalStatic.LogDBpath = GlobalStatic.Settings["Log_DB_Path"];
				GlobalStatic.TransactionDBPath = GlobalStatic.Settings["Transaction_DB"];
				GlobalStatic.Transaction_Query = GlobalStatic.Settings["Transaction_Query"]; 
				GlobalStatic.Transaction_Commands = GlobalStatic.Settings["Transaction_Commands"];
			}
			SaveSettings();
		}

		public static void SaveSettings()
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Settings.SaveSettings()");
			if (GlobalStatic.Settings.EqualTo(SBFile.ReadContents(GlobalStatic.SettingsPath)) == false)
			{
				string status = SBFile.WriteContents(GlobalStatic.SettingsPath, GlobalStatic.Settings);
				if (status == "FAILED") //Settings could not be saved for some reason!
				{
					Events.LogMessage(GlobalStatic.LangList["Failed Save Settings"], GlobalStatic.LangList["UI"]); 
					GraphicsWindow.ShowMessage(GlobalStatic.LangList["Failed Save Settings"], GlobalStatic.LangList["Error"]);
				}
			}
		}

		public static void Paths(string AssetPath,string PluginPath,string LocalizationFolder,string AutoRunPluginPath,string Localization_LanguageCodes_Path,string LogCSVPath,string AutoRunPluginMessage)
		{ 
			LDList.Add(GlobalStatic.List_Stack_Trace, "Settings.Paths()");
			if (LDFile.Exists(AssetPath) == false || LDFile.Exists(PluginPath) == false || LDFile.Exists(LocalizationFolder) == false) //Creates Folders if one is missing
			{
				Directory.CreateDirectory(AssetPath);
				Directory.CreateDirectory(PluginPath);
				Directory.CreateDirectory(LocalizationFolder);
				Directory.CreateDirectory(Localization_LanguageCodes_Path);
			}

			if (LDFile.Exists(AutoRunPluginPath) == false)
			{
				System.IO.File.WriteAllText(AutoRunPluginPath, AutoRunPluginMessage);
			}

			if (LDFile.Exists(LogCSVPath) == false)
			{
				System.IO.File.WriteAllText(LogCSVPath, "id,Local Date,Local Time,Username,Product ID,Version,Type,Event");
			}
		}


		public static void IniateDatabases()
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Settings.IniateDatabases()");
			GlobalStatic.LogDB = LDDataBase.ConnectSQLite(GlobalStatic.LogDBpath);
			GlobalStatic.TransactionDB = LDDataBase.ConnectSQLite(GlobalStatic.TransactionDBPath);

			AddToList("<none>", "<none>", "<none>",0);
			AddToList(GlobalStatic.LogDBpath, GlobalStatic.LogDB, "Master Log",4);
			AddToList(GlobalStatic.TransactionDBPath, GlobalStatic.TransactionDB, "Transaction Log",4);

			Engines.Command(GlobalStatic.LogDB, GlobalStatic.LOGSQL, GlobalStatic.UserName, "Auto Creation Statements", false);
			Engines.Command(GlobalStatic.LogDB, GlobalStatic.LOGSQLVIEW , GlobalStatic.UserName, "Auto Creation Statements", false);
			Engines.Command(GlobalStatic.TransactionDB, GlobalStatic.TransactionsSQL , GlobalStatic.UserName, "Auto Creation Statements", false);

			GlobalStatic.LogNumber = Engines.Query(GlobalStatic.LogDB,"SELECT COUNT(ID) FROM LOG;",null,true,GlobalStatic.UserName,"Fetch Log")[1]["COUNT(ID)"];
		}

		private static void AddToList(string Path,string Name,string ShortName,int Engine)  
		{
			LDList.Add(GlobalStatic.List_DB_Path, Path);
			LDList.Add(GlobalStatic.List_DB_Name, Name);
			LDList.Add(GlobalStatic.List_DB_ShortName, ShortName);
			LDList.Add(GlobalStatic.List_DB_Engine, Engine);
		}

	}
}

