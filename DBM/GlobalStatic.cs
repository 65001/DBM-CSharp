using System;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
using SBFile = Microsoft.SmallBasic.Library.File;
using System.Collections.Generic;
using System.IO;
namespace DBM
{
		public static class GlobalStatic //List of Variables shared across everything through this class
		{
			public static string ProgramDirectory = Program.Directory;
			public static string Username = LDFile.UserName;
			public static float Ping;
			public static string TabKey = Text.GetCharacter(9);
			public static bool DebugMode;
			public static bool DebugParser;
			public static bool EulaTest = false;

			public const string IP_Ping_Address = "8.8.8.8";
			public static bool LoadedFile = false;

			//Self Aware Data
			public const string Copyright = "2016 through 2017";
			public const string ProductID = "DBM C#";
			public const int VersionID = 1122;

			public static int DefaultWidth, DefaultHeight,DefaultFontSize,LogNumber;

			public static string UserName = LDFile.UserName,CurrentDatabase;
			//URI
			public static string AssetPath = ProgramDirectory + "\\Assets\\";
			public static string LocalizationFolder = ProgramDirectory + "\\Localization\\";
			public static string Localization_LanguageCodes_Path = LocalizationFolder + "Lang\\";
			public static string PluginPath = ProgramDirectory + "Plugin\\";
			public static Primitive LogCSVpath = AssetPath + "Log.csv";
			public static Primitive LogDBpath = AssetPath + "Log.db";
			public static string TransactionDBPath = AssetPath + "Transactions.db";
			public static string EULA_Text_File = AssetPath + "EULA.txt";
			public static string SettingsPath = AssetPath + "setting.txt";
			public static string AutoRunPluginPath = AssetPath + "Auto Run Plugin.txt";
			public static string HelpPath = AssetPath + "Help Table.html";
			public static string External_Menu_Items_Path = AssetPath + "Menu.txt";
			public static string Online_EULA_URI = "https://drive.google.com/uc?export=download&id=0B2v4xbFnpKvRNTFKckFKLVNNUDg";
			public static string OnlineDB_Refrence_Location = "https://docs.google.com/uc?id=0B2v4xbFnpKvRVmNVODZ4bnppd3c&export=download";
			//Settings Data
			public static bool RestoreSettings = false;
			public static Primitive Settings;
			public static int Listview_Width;
			public static int Listview_Height;
			public static int LastVersion;
			public static Primitive Extensions;
			public static bool Transactions,Transaction_Query,Transaction_Commands;
			public static string Deliminator;
			public static int TimeOut;
			public static string LastFolder;
			public static string EULA_Username;
			public static string EULA_Accepted_Version;
			public static bool EULA_Acceptance;
			public static string LanguageCode;
			public static string EULA_Newest_Version;
			public static Primitive LangList,Schema;
			//Lists
			public static string ExportT2 = "Export T2",ExportT1 = "Export T1";

			public static string TrackDefaultTable = "TrackDefaultTable";
			public static string List_Mod_Name = "Mod_Name";
			public static string List_Mod_Path = "Mod_Path";
			public static string List_Command_Parser = "Command_SQL_Parser";
			public static string List_Command_Parser_Status = "Command_SQL_Parser_Status";
			public static string List_Command_Parser_OnFail = "Command_SQL_Parser_Fail";
			public static string List_Command_Parser_OnFail_Index = "Command_SQL_Parser_Fail_Index";
			public static string List_UI_Name = "UI_Name";
			public static string List_UI_Handler = "UI_Handler";
			public static string List_UI_Action = "UI_Action";
			public static string List_Stack_Trace = "Stack_Trace";
			public static string List_Stack_Time = "Stack_Time";
			public static string List_SCHEMA_Table = "SCHEMA_TABLE";
			public static string List_SCHEMA_View = "SCHEMA_VIEW";
			public static string List_Schema_Index = " SCHEMA_INDEX";
			public static string List_DB_Path = "DB_Path";
			public static string List_DB_Name = "DB_Name";
			public static string List_DB_ShortName = "DB_SName";
			public static string List_DB_Engine = "DB_Engine";
			public static string List_DB_Tracker = "DB_Tracking";
			public static string List_Query_Time = "Query_Time";
			public static string List_CMD_Time = "CMD_Time";
			public static string List_Time_Refer = "Time_Ref";
			public static string List_ISO_Lang = "ISO_Lang";
			public static string List_ISO_Text = "ISO_Text";
			public static Primitive MenuList,CheckList,Buttons;
			public static string Title = "Database Manager (" + ProductID + ") v" + VersionID + " ";
			public const string AutoRunPluginMessage = "# This file designates the Mod and the subroutine the main program should call on the start of the program.\n# Use this to run your program at start up.\n# The Program accepts any of the following:\n#\tMod Name.Sub Name\n#\t1=Mod Name;2=Sub Name;\n# The character # marks the line as commented.\n# To add UI Elements without starting up your program (Please do this if you can) alter the MENU.txt file.";

			public static string LogDB, TransactionDB;
			//public static string TransactionDB;

			static string DQC = "\"";
			public static string LOGSQL = "CREATE TABLE IF NOT EXISTS Log (ID Integer PRIMARY KEY,\"UTC DATE\" TEXT," + DQC + "UTC TIME" + DQC + " TEXT,Date TEXT,Time TEXT,USER TEXT,ProductID TEXT,ProductVersion INTEGER,Type TEXT,Event TEXT);";
			public static string TransactionsSQL = "CREATE TABLE IF NOT EXISTS Transactions (ID INTEGER PRIMARY KEY,\"UTC DATE\" TEXT," + DQC + "UTC TIME" + DQC + " TEXT,USER TEXT,PATH TEXT,DB TEXT,SNAME TEXT,SQL TEXT,Type TEXT,Reason TEXT);";
			public static string LOGSQLVIEW = "CREATE VIEW IF NOT EXISTS " + DQC + "LOCAL TIME" + DQC + " AS SELECT ID,DATE,TIME,USER,PRODUCTID,PRODUCTVERSION,Type,Event From Log;CREATE VIEW IF NOT EXISTS " + DQC + "UTC TIME" + DQC + " AS Select ID," + DQC + "UTC DATE" + DQC + "," + DQC + "UTC TIME" + DQC + ",USER,ProductID,ProductVersion,Type,Event From Log;DROP VIEW IF EXISTS LOCAL_TIME;DROP VIEW IF EXISTS UTC_TIME;";
	}

}
