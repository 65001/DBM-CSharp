// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
using SBFile = Microsoft.SmallBasic.Library.File;
using System.Collections.Generic;
using System.IO;
namespace DBM
{
	[SmallBasicType]
	public static class Engines
	{
		public enum EnginesModes {MySQL=1, ODBC=2,OLEDB=3,SQLITE=4,SQLSERVER=5}

		public static int Command(string Database, string SQL, string User, string Explanation, bool RunParser)
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.Command()");
			if (RunParser == false)
			{
				int EngineMode = Engine_Type(Database);
				TransactionRecord(User, Database, SQL, "CMD", Explanation);
				switch (EngineMode)
				{
					case 1:
						return 0;
					case 2:
						return 0;
					case 3:
						return 0;
					case (int)EnginesModes.SQLITE:
						return LDDataBase.Command(Database, SQL);
					case 5:
						return 0;
					default:
						return 0;
				}
			}
			else if (RunParser == true)
			{
				Console.WriteLine("Database type currently not supported!");
			}
			return 0;
		}

		public static void Parser()
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.Parser()");
		}

		public static Primitive Query(string DataBase, string SQL, string ListView, bool FetchRecords, string UserName, string Explanation) //Expand
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.Query()");
			int EngineMode = Engine_Type(DataBase);
			TransactionRecord(UserName, DataBase, SQL, "Query", Explanation);
			return LDDataBase.Query(DataBase, SQL, ListView, FetchRecords);
		}

		public static void Emulator() //Implement
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.Emulator()");
		}

		public static void TransactionRecord(string UserName, string DataBase, string SQL, string Type, string Reason) //Implement
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.TransactionRecord()");
		}

		public static string Load_DB(EnginesModes Mode, Primitive Data)
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.Load_DB()");
			switch (Mode)
			{
				case EnginesModes.MySQL: //MySQL
					return "";
				case EnginesModes.ODBC: //ODBC
					return "";
				case EnginesModes.OLEDB: //OLEDB
					return "";
				case EnginesModes.SQLITE: //SQLITE
					if (LDFile.Exists(Data) == true)
					{
						int Index = LDList.GetAt(GlobalStatic.List_DB_Path, Data);
						Events.LogMessage("LOAD DB INDEX : " + Index, "Debug");
						Events.LogMessage("LOAD DB : " + Data , "Debug");
						if (Index == 0) //New Database
						{
							string Database = LDDataBase.ConnectSQLite(Data);
							AddToList(Data, Database, LDFile.GetFile(Data), 4);
							GlobalStatic.Settings["LastFolder"] = LDFile.GetFolder(Data);
							Settings.SaveSettings();
							GlobalStatic.CurrentDatabase = Database;
							return Database;
						}
						else
						{
							string Database = LDList.GetAt(GlobalStatic.List_DB_Name, Index);
							LDList.Add(GlobalStatic.List_DB_ShortName, LDList.GetAt(GlobalStatic.List_DB_ShortName, Index));
							GlobalStatic.CurrentDatabase = Database;
							return Database;
						}
					}
					return "";
				case EnginesModes.SQLSERVER: //SQLServer
					return "";
				default:
					return "Incorrect Paramters"; ;
			}
		}

		public static void GetSchema(string Database)
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.GetSchema()");
			Events.LogMessage(Database, "Debug");
			if (!string.IsNullOrEmpty(Database)) //Prevents Prevents Application from querying a nonexistent db 
			{
				LDList.Clear(GlobalStatic.List_SCHEMA_Table);
				LDList.Clear(GlobalStatic.List_SCHEMA_View);
				LDList.Clear(GlobalStatic.List_Schema_Index);

				Primitive Master_Schema_List = Query(Database, "SELECT tbl_name,name,type FROM sqlite_master UNION Select tbl_name,name,type From SQLite_Temp_Master;", null, true, GlobalStatic.LangList["App"], "SCHEMA");
				Primitive Master_Schema_Lists = "table=" + GlobalStatic.List_SCHEMA_Table + ";view=" + GlobalStatic.List_SCHEMA_View + ";index=" + GlobalStatic.List_Schema_Index + ";";
				for (int i = 1; i <= SBArray.GetItemCount(Master_Schema_List); i++)
				{
					LDList.Add(Master_Schema_Lists[Master_Schema_List[i]["type"]], Master_Schema_List[i]["tbl_name"]);
				}
				string Default_Table = LDList.GetAt(GlobalStatic.List_SCHEMA_Table, 1);
				LDList.Add(GlobalStatic.TrackDefaultTable,Database + "." + Default_Table);
				GetSchemaofTable(Database, Default_Table);
			}
		}

		public static void GetSchemaofTable(string Database, string Table)
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.GetSchemaofTable()");
			if (!string.IsNullOrEmpty(Database) && !string.IsNullOrEmpty(Table)) //Prevents calls to nonexistent tables or Databases
			{
				LDList.Clear("SCHEMA");
				Primitive Schema = Query(Database, "PRAGMA table_info(\"" + Table + "\");", null, true,GlobalStatic.LangList["App"], GlobalStatic.LangList["SCHEMA-PRIVATE"]);
				for (int i = 1;i <= SBArray.GetItemCount(Schema); i++)
				{
					LDList.Add("SCHEMA", Schema[i]["name"]);
				}
				GlobalStatic.Schema = LDList.ToArray("SCHEMA");
			}
		}

		public static void GenerateQuery(bool Search,bool Sort,bool Function) //Implement //Interface to private classes
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.GenerateQuery()");
			if (Search)
			{ }
			if (Sort)
			{ }
			if (Function)
			{ }
		}

		static void GenerateSearch() //Implement
		{ }
		static void GenerateSort() //Implement
		{ }
		static void GenerateFunction() //Implement
		{ }

		public static void CreateStatisticsPage() //Implement
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.CreateStatisticsPage()");
		}

		private static int Engine_Type(string Database) //Fetches Engine Mode/Type associated with the Database 
		{
			int Index = LDList.IndexOf(GlobalStatic.List_DB_Name, Database);
			if (Index != 0)
			{
				return LDList.GetAt(GlobalStatic.List_DB_Engine, Index);
			}
			else
			{ return 0; }
		}

		static void AddToList(string path, string Name, string ShortName, int Engine)
		{
			LDList.Add(GlobalStatic.List_DB_Path, path);
			LDList.Add(GlobalStatic.List_DB_Name, Name);
			LDList.Add(GlobalStatic.List_DB_ShortName, ShortName);
			LDList.Add(GlobalStatic.List_DB_Engine, Engine);
		}
	}
}