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

		public static int Command(Primitive Database, string SQL, string User, string Explanation, bool RunParser)
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.Command()");
			if (RunParser == false) 
			{
				int EngineMode = Engine_Type(Database);
				Engines.TransactionRecord(User, Database, SQL, "CMD", Explanation);
				switch (EngineMode)
				{
					case 1:
						return 0;
					case 2:
						return 0;
					case 3:
						return 0;
					case 4:
						return LDDataBase.Command(Database, SQL);
					case 5:
						return 0;
					default:
						return 0;
				}
			}
			else if (RunParser == true) 
			{
				Console.WriteLine("Currently not Supported!");
			}
			return 0;
		}

		public static void Parser() 
		{
		LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.Parser()");
		}

		public static Primitive Query(string DataBase,string SQL,string ListView,bool FetchRecords,string UserName,string Explanation) 
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.Query()");
			int EngineMode = Engine_Type(DataBase);
			TransactionRecord(UserName, DataBase, SQL, "Query", Explanation);

			switch (EngineMode) 
			{
				case 1:
					return 0;
				case 2:
					return 0;
				case 3: 
					return 0;
				case 4: 
					return LDDataBase.Query(DataBase, SQL, ListView, FetchRecords);
				case 5: 
					return 0;
				default: 
					return 0;
			}
		}

		public static void Emulator() 
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.Emulator()");
		}

		public static void TransactionRecord(string UserName,string DataBase,string SQL,string Type,string Reason) 
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.TransactionRecord()");
		}

		public static string Load_DB(int EngineMode,Primitive Data) 
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.Load_DB()");
			switch (EngineMode)
			{
				case 1 : //MySQL
					return "";
				case 2 : //ODBC
					return "";
				case 3 : //OLEDB
					return "";
				case 4 : //SQLITE
					if (LDFile.Exists(Data) == true)
					{
						int Index = LDList.GetAt(GlobalStatic.List_DB_Path, Data);
						TextWindow.WriteLine("LOAD DB INDEX : " + Index);

						if (Index == 0) //New Database
						{
							string Database = LDDataBase.ConnectSQLite(Data);
							AddToList(Data,Database, LDFile.GetFile(Data),4);
							GlobalStatic.Settings["LastFolder"] = LDFile.GetFolder(Data);
							DBM.Settings.SaveSettings();
							return Database;
						}
						else 
						{
							LDList.Add(GlobalStatic.List_DB_ShortName, LDList.GetAt(GlobalStatic.List_DB_ShortName,Index));
							return LDList.GetAt(GlobalStatic.List_DB_Name,Index);
						}
					}
					return "";
				case 5 : //SQLServer
					return "";
				default:
					return "Incorrect Paramters";;
			}
		}

		public static void GetSchema() //Implement
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.GetSchema()");
		}

		public static void GetSchemaofTable() //Implement
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.GetSchemaofTable()");
		}

		public static void GenerateQuery() //Implement
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.GenerateQuery()");
		}

		public static void CreateStatisticsPage() //Implement
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "Engines.CreateStatisticsPage()");
		}

		static int Engine_Type(string Database) 
		{
			int Index = LDList.IndexOf(GlobalStatic.List_DB_Name,Database);
			if (Index != 0)
			{
				return LDList.GetAt(GlobalStatic.List_DB_Engine, Index);
			}
			else 
			{ return 0;}
		}

		static void AddToList(string Path, string Name, string ShortName, int Engine)
		{
			LDList.Add(GlobalStatic.List_DB_Path, Path);
			LDList.Add(GlobalStatic.List_DB_Name, Name);
			LDList.Add(GlobalStatic.List_DB_ShortName, ShortName);
			LDList.Add(GlobalStatic.List_DB_Engine, Engine);
		}
	}
}
