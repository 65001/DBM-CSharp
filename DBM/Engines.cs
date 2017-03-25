// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
namespace DBM
{
	[SmallBasicType]
	public static class Engines
	{
		public enum EnginesModes { NONE, MySQL = 1, ODBC = 2, OLEDB = 3, SQLITE = 4, SQLSERVER = 5 }
		public static string CurrentDatabase { get; private set; }
		public static string CurrentTable { get; private set; }
		public static string Database_Shortname { get; private set; }
		public static Primitive Schema { get; private set; }
		public static string GQ_CMD { get; private set; } //Auto Generated Query SQL Statements

		static List<string> _DB_Path = new List<string>();
		static List<string> _DB_Name = new List<string>();
		static List<string> _DB_ShortName = new List<string>();
		static List<EnginesModes> _DB_Engine = new List<EnginesModes>();
		

		public static int Command(string Database, string SQL, string User, string Explanation, bool RunParser)
		{
			Console.WriteLine( _DB_ShortName.ToArray() );
            Utilities.AddtoStackTrace( "Engines.Command()");
			if (RunParser == false)
			{
				EnginesModes EngineMode = Engine_Type(Database);
				TransactionRecord(User, Database, SQL, "CMD", Explanation);
				switch (EngineMode)
				{
					case EnginesModes.SQLITE:
						return LDDataBase.Command(Database, SQL);
					default:
						return 0;
				}
			}

			Console.WriteLine("Database type currently not supported!");
			return 0;
		}

		public static void Parser()  //TODO: Implement Parser
		{
            Utilities.AddtoStackTrace( "Engines.Parser()");
		}

		public static Primitive Query(string DataBase, string SQL, string ListView, bool FetchRecords, string UserName, string Explanation) //Expand
		{
			Stopwatch QueryTime = Stopwatch.StartNew();
            Utilities.AddtoStackTrace( "Engines.Query()");
			TransactionRecord(UserName, DataBase, SQL, "Query", Explanation);
			Primitive QueryResults = LDDataBase.Query(DataBase, SQL, ListView, FetchRecords);

			LDList.Add(GlobalStatic.List_Time_Refer, "Query");
			LDList.Add(GlobalStatic.List_Query_Time, QueryTime.ElapsedMilliseconds.ToString());
			return QueryResults;

		}

		public static void Emulator() //TODO Implement Emulator atleast for sqlite for DBM
		{
            //Attempts to emulate some if not all commands of a database engine by aliasing it to SQL
            Utilities.AddtoStackTrace( "Engines.Emulator()");
		}

		public static void TransactionRecord(string UserName, string DataBase, string SQL, string Type, string Reason) //TODO Transactions 
		{
            //This method should only run when the correct global Paramters are rig//Current Storage only supports SQLite
            Utilities.AddtoStackTrace( "Engines.TransactionRecord()");
		}

		public static string Load_DB(EnginesModes Mode, Primitive Data) //Tasked with connecting to a Database and adding the DB Connection Name to a list.
		{
            //MAKE SURE The CurrentMode is always currently changed.
            Utilities.AddtoStackTrace( "Engines.Load_DB()");
			switch (Mode)
			{
				case EnginesModes.MySQL:  //TODO
					return null;
				case EnginesModes.ODBC:  //TODO
					return null;
				case EnginesModes.OLEDB: //TODO
					return null;
				case EnginesModes.SQLITE: 
					if (LDFile.Exists(Data) == true)
					{
						int Index = _DB_Path.IndexOf(Data);
                        if (Index == 0) //New Database
						{
							string Database = LDDataBase.ConnectSQLite(Data);
							AddToList(Data, Database, LDFile.GetFile(Data), EnginesModes.SQLITE);
							GlobalStatic.Settings["LastFolder"] = LDFile.GetFolder(Data);
							Settings.SaveSettings();
							CurrentDatabase = Database;
							return Database;
						}
						else //Database already exists as a connection so set the primary connection to that
						{
							string Database =_DB_Name[Index];
							Database_Shortname = _DB_ShortName[Index];
                            _DB_ShortName.Add(Database_Shortname);
							CurrentDatabase = Database;
							return Database;
						}
					}
					return null;
				case EnginesModes.SQLSERVER: //SQLServer
					return null;
				default:
					return "Incorrect Paramters";
			}
		}

		public static void GetSchema(string Database)
		{
            Utilities.AddtoStackTrace( "Engines.GetSchema()");
			if (string.IsNullOrEmpty(Database))//Prevents Prevents Application from querying a nonexistent db 
			{ 
				return; 
			}

			EnginesModes EngineMode = Engine_Type(Database);
			switch (EngineMode)
				{
					case EnginesModes.SQLITE:	
						LDList.Clear(GlobalStatic.List_SCHEMA_Table);
						LDList.Clear(GlobalStatic.List_SCHEMA_View);
						LDList.Clear(GlobalStatic.List_Schema_Index);

						Primitive Master_Schema_List = Query(Database, "SELECT tbl_name,name,type FROM sqlite_master UNION Select tbl_name,name,type From SQLite_Temp_Master;", null, true, Utilities.Localization["App"], "SCHEMA");
						Primitive Master_Schema_Lists = "table=" + GlobalStatic.List_SCHEMA_Table + ";view=" + GlobalStatic.List_SCHEMA_View + ";index=" + GlobalStatic.List_Schema_Index + ";";
						for (int i = 1; i <= SBArray.GetItemCount(Master_Schema_List); i++)
						{
							LDList.Add(Master_Schema_Lists[Master_Schema_List[i]["type"]], Master_Schema_List[i]["tbl_name"]);
						}
						CurrentTable = LDList.GetAt(GlobalStatic.List_SCHEMA_Table, 1);
						LDList.Add(GlobalStatic.TrackDefaultTable, Database + "." + CurrentTable);
						GetColumnsofTable(Database, CurrentTable);
						break;
				}
		}

		public static void GetColumnsofTable(string Database, string Table)
		{
            Utilities.AddtoStackTrace( "Engines.GetSchemaofTable()");

			if (string.IsNullOrEmpty(Database) || string.IsNullOrEmpty(Table)) //Prevents calls to nonexistent tables or Databases
			{
				return;
			}

			EnginesModes EngineMode = Engine_Type(Database);
			switch (EngineMode)
			{
				case EnginesModes.SQLITE:
					LDList.Clear("SCHEMA");
					Primitive LSchema = Query(Database, "PRAGMA table_info(" + Table + ");", null, true, Utilities.Localization["App"], Utilities.Localization["SCHEMA PRIVATE"]);
					for (int i = 1; i <= SBArray.GetItemCount(LSchema); i++)
					{
						LDList.Add("SCHEMA", LSchema[i]["name"]);
					}
					Schema = LDList.ToArray("SCHEMA");
					break;
			}
		}

		public static void EditTable(string Table,string Control)
		{
			LDDataBase.EditTable(CurrentDatabase,Table, Control);
		}

		public static void SetDefaultTable(string Table)
		{
			if (!string.IsNullOrEmpty(Table))
			{
				CurrentTable = "\"" + Table + "\"";
				LDList.Add(GlobalStatic.TrackDefaultTable, CurrentDatabase + "." + CurrentTable);
			}
			else
			{
				Events.LogMessagePopUp("Table does not exist in context", "Table does not exist in context", "Error", Utilities.Localization["System"]);
			}
		}

		public static void GenerateQuery(bool Search,bool Sort,bool Function,string SearchBy,string OrderBy,string SortOrder,bool StrictSearch,bool InvertSearch,string FunctionSelected,string FunctionColumn,string SearchText) 
		{
			//Interface to private classes
			if (!string.IsNullOrEmpty(CurrentTable))
			{
				GQ_CMD = null;
				GQ_CMD = "SELECT * FROM " + CurrentTable + " ";
                Utilities.AddtoStackTrace( "Engines.GenerateQuery()");
				if (Search)
				{
					GQ_CMD += GenerateSearch(SearchBy,SearchText,InvertSearch,StrictSearch);
				}
				if (Function) 
				{
					GQ_CMD = GenerateFunction(FunctionSelected, FunctionColumn);
				}

				if (Sort)
				{
					GQ_CMD += GenerateSort(OrderBy,SortOrder );
				}
			}
			Query(CurrentDatabase, GQ_CMD, GlobalStatic.ListView, false, GlobalStatic.UserName, "Auto Generated Query on behalf of " + GlobalStatic.Username);
			GQ_CMD = null;
		}

		static string GenerateSearch(string SearchColumn,string SearchText,bool InvertSearch,bool StrictSearch)
		{
			string CMD;
			CMD = "WHERE " + SearchColumn;
			if (InvertSearch == true && StrictSearch == false)
			{ CMD += " NOT"; }

			if (StrictSearch == false)
			{
				CMD += " LIKE '%" + SearchText + "%' ";
			}
			else
			{
				if (InvertSearch) 
				{
					CMD += "!='" + SearchText + "' ";
				}
				else 
				{
					CMD += "='" + SearchText + "' ";
				}
			}
			return CMD;
		}


		static string GenerateSort(string OrderBy,string ASCDESC) 
		{
			string CMD;
			CMD = "ORDER BY \"" + OrderBy + "\" " + ASCDESC + ";";
			return CMD;
		}
		static string GenerateFunction(string Function,string Column) 
		{
			string CMD;
			CMD = "SELECT " + Function + "(\"" + Column + "\") FROM " + CurrentTable + " ";
			return CMD;
		}

		public static void CreateStatisticsPage(string Table) //TODO
		{
            Utilities.AddtoStackTrace( "Engines.CreateStatisticsPage()");
		}

		static EnginesModes Engine_Type(string Database) //Fetches Engine Mode/Type associated with the Database 
		{
            int Index = _DB_Name.IndexOf(Database);
			if (Index != 0)
			{
                return _DB_Engine[Index];
			}
			return EnginesModes.NONE; 
		}

		public static void AddToList(string path, string Name, string ShortName, EnginesModes Engine)
		{
			Database_Shortname = ShortName;

			_DB_Name.Add(Name);
			_DB_Path.Add(path);
			_DB_ShortName.Add(ShortName);
			_DB_Engine.Add(Engine);
		}

		public static ReadOnlyCollection<string> DB_Name 
		{ 
			get { return _DB_Name.AsReadOnly(); }
		}

        public static ReadOnlyCollection<string> DB_Path
        {
            get { return _DB_Path.AsReadOnly(); }
        }

        public static ReadOnlyCollection<string> DB_ShortName
        {
            get { return _DB_ShortName.AsReadOnly(); }
        }

        public static ReadOnlyCollection<EnginesModes> DB_Engine
        {
            get { return _DB_Engine.AsReadOnly(); }
        }
	}
}