﻿// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using Microsoft.SmallBasic.Library;
using System;
using System.IO;
namespace DBM
{
		public static class GlobalStatic //List of Variables shared across everything through this class
		{
			public static readonly string ProgramDirectory = Program.Directory;
			public static readonly string UserName =  Environment.UserName;
			public static bool  EulaTest, LoadedFile;

        #if DEBUG
           public static bool DebugMode = true;
        #else
        public static bool DebugMode;
        #endif

            //Self Aware Data
            public const string Copyright = "2016 through 2017";
			public const string ProductID = "DBM C#";

			public static readonly string VersionID = typeof(GlobalStatic).Assembly.GetName().Version.ToString().Replace(".","");
			public static int DefaultFontSize;
        
			//URI
			public static string AssetPath = Path.Combine( ProgramDirectory , "Assets");
			public static string LocalizationFolder = Path.Combine( ProgramDirectory , "Localization");
			public static string Localization_LanguageCodes_Path = Path.Combine(LocalizationFolder, "Lang");
			public static string PluginPath = Path.Combine( ProgramDirectory ,"Plugin");
			public static string LogDBpath = Path.Combine(AssetPath , "Log.db");
            public static string UpdaterDBpath = Path.Combine(AssetPath, "Updater.db");
			public static string TransactionDBPath = Path.Combine(AssetPath , "Transactions.db");
			public static string EULA_Text_File = Path.Combine(AssetPath , "EULA.txt");
			public static string SettingsPath = Path.Combine(AssetPath ,"setting.txt");
			public static string AutoRunPluginPath = Path.Combine(AssetPath , "Auto Run Plugin.txt");
			public static string HelpPath = Path.Combine(AssetPath ,"Help Table.html");
			public static string External_Menu_Items_Path = Path.Combine(AssetPath , "Menu.txt");
       
			public const string OnlineDB_Refrence_Location = "https://github.com/65001/DBM-CSharp/raw/master/DBM/bin/Release/Assets/Updater.db";

			//Settings Data
			public static bool RestoreSettings;
			public static Primitive Settings,Extensions;
			public static int Listview_Width,Listview_Height,UIx,Timeout,SortBy;

			public static bool Transaction_Query,Transaction_Commands, EULA_Acceptance;
			public static string Deliminator;
			public static string LastFolder;
			public static string EULA_UserName;
			public static string LanguageCode;

        //Lists
        /*
        public const string ExportT2 = "Export T2",ExportT1 = "Export T1";
        public const string TrackDefaultTable = "TrackDefaultTable";
        public const string List_Mod_Name = "Mod_Name";
        public const string List_Mod_Path = "Mod_Path";
        public const string List_Command_Parser = "Command_SQL_Parser";
        public const string List_Command_Parser_Status = "Command_SQL_Parser_Status";
        public const string List_Command_Parser_OnFail = "Command_SQL_Parser_Fail";
        public const string List_Command_Parser_OnFail_Index = "Command_SQL_Parser_Fail_Index";
        */

            public static Primitive TextBox,CheckBox,ComboBox; //Change to a Dictionaries in the future and move to UI.

			public static readonly string Title = "Database Manager (" + ProductID + ") v" + VersionID + " ";

			public static string LogDB, TransactionDB, Dataview, ListView;

            public const string AutoRunPluginMessage = "# This file designates the Mod and the subroutine the main program should call on the start of the program.\n# Use this to run your program at start up.\n# The Program accepts any of the following:\n#\tMod Name.Sub Name\n#\t1=Mod Name;2=Sub Name;\n# The character # marks the line as commented.\n# To add UI Elements without starting up your program (Please do this if you can) alter the MENU.txt file.";
            public const string LOGSQL = "CREATE TABLE IF NOT EXISTS Log (ID Integer PRIMARY KEY,\"UTC DATE\" TEXT,\"UTC TIME\" TEXT,Date TEXT,Time TEXT,USER TEXT,ProductID TEXT,ProductVersion INTEGER,Type TEXT,Event TEXT);";
			public const string TransactionsSQL = "CREATE TABLE IF NOT EXISTS Transactions (ID INTEGER PRIMARY KEY,\"UTC DATE\" TEXT,\"UTC TIME\" TEXT,USER TEXT,PATH TEXT,DB TEXT,SNAME TEXT,SQL TEXT,Type TEXT,Reason TEXT);";
			public const string LOGSQLVIEW = "CREATE VIEW IF NOT EXISTS \"LOCAL TIME\" AS SELECT ID,DATE,TIME,USER,PRODUCTID,PRODUCTVERSION,Type,Event From Log;CREATE VIEW IF NOT EXISTS \"UTC TIME\" AS Select ID,\"UTC DATE\",\"UTC TIME\",USER,ProductID,ProductVersion,Type,Event From Log;DROP VIEW IF EXISTS LOCAL_TIME;DROP VIEW IF EXISTS UTC_TIME;";
	    }
}