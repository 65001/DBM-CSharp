// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using LitDev;
using Microsoft.SmallBasic.Library;

namespace DBM
{
	public static partial class Engines
	{
		public enum EnginesMode { NONE, MySQL, ODBC, OLEDB, SQLITE, SQLSERVER}
        public enum Type { Command,Query}

		public static string CurrentDatabase { get; private set; }
		public static string CurrentTable { get; private set; }
		public static string DatabaseShortname { get; private set; }

        /// <summary>
        /// Returns the current EngineMode or EngineModes.NONE if no data is currently in the List
        /// </summary>
        public static EnginesMode CurrentEngine
        {
            get
            {
                try
                {
                    return DB_Engine[DB_Name.IndexOf(CurrentDatabase)];
                }
                catch (Exception)
                {
                    return EnginesMode.NONE;
                }
            }
        }

		public static string GQ_CMD { get; private set; } //Auto Generated Query SQL Statements 

		static List<string> _DB_Path = new List<string>();
		static List<string> _DB_Name = new List<string>();
		static List<string> _DB_ShortName = new List<string>();
		static List<EnginesMode> _DB_Engine = new List<EnginesMode>();
        static Dictionary<string, string> _DB_Hash = new Dictionary<string, string>();
        static List<string> _TrackingDefaultTable = new List<string>();

        static List<string> _Tables = new List<string>();
        static List<string> _Views = new List<string>();
        static List<string> _Indexes = new List<string>();

        public static Primitive Schema { get; private set; }
        static List<string> _Schema = new List<string>();
        
        static List<Type> _Type_Referer = new List<Type>();
        static List<long> _Timer = new List<long>();
        static List<string> _Last_Query = new List<string>();
        static List<string> _Last_NonSchema_Query = new List<string>();

        public static int Command(string Database, string SQL,string Explanation)
        {
            return Command(Database, SQL, GlobalStatic.UserName, Explanation, false);
        }

        /// <summary>
        /// Executes a Command against a database.
        /// </summary>
        /// <param name="Database">Database. Use the Database name you recieve from LoadDB</param>
        /// <param name="SQL"></param>
        /// <param name="User">UserName of the requested username</param>
        /// <param name="Explanation">Any notes for transactions</param>
        /// <param name="RunParser">Run Custom Parser... Yet to be implemented</param>
        /// <returns></returns>
        public static int Command(string Database, string SQL, string User, string Explanation, bool RunParser)
		{
            Utilities.AddtoStackTrace("Engines.Command()");
            Stopwatch CommandTime = Stopwatch.StartNew();
			if (RunParser == false)
			{
				TransactionRecord(User, Database, SQL, Type.Command, Explanation);
                return LDDataBase.Command(Database, SQL);
			}

            _Type_Referer.Add(Type.Command);
            _Timer.Add(CommandTime.ElapsedMilliseconds);

            //TODO Implement Parser Stuff 
			return 0;
		}

		public static Primitive Query(string DataBase, string SQL, string ListView, bool FetchRecords, string UserName, string Explanation) //Expand
		{
            Utilities.AddtoStackTrace("Engines.Query()");

            Stopwatch QueryTime = Stopwatch.StartNew();
            
            if (SQL.StartsWith("."))
            {
                Emulator(DB_Engine[DB_Name.IndexOf(DataBase)],DataBase, SQL,UserName,ListView);
                return null;
            }

			TransactionRecord(UserName, DataBase, SQL, Type.Query, Explanation);
			Primitive QueryResults = LDDataBase.Query(DataBase, SQL, ListView, FetchRecords);

            _Type_Referer.Add(Type.Query);

            switch (Explanation)
            {
                case "SCHEMA-PRIVATE":
                case "SCHEMA":
                    break;
                default:
                    _Last_NonSchema_Query.Add(SQL);
                    break;
            }

            _Last_Query.Add(SQL);
            _Timer.Add(QueryTime.ElapsedMilliseconds);
			return QueryResults;
		}

        /// <summary>
        /// This method should only run when the correct global Paramters are set.
        /// //Current Storage only supports SQLite db file.
        /// </summary>
		public static void TransactionRecord(string UserName, string DataBase, string SQL, Type Type, string Reason) 
		{
            Utilities.AddtoStackTrace( "Engines.TransactionRecord()");
            //Escapes function when conditions are correct
            if (GlobalStatic.Transactions == false || DataBase == GlobalStatic.TransactionDB )
            {
                return;
            }

            if (GlobalStatic.Transaction_Commands == true && Type == Type.Command)
            {
                string _Type = "Command";
                _TransactionRecord(UserName, DataBase, SQL, _Type, Reason);
            }

            if (GlobalStatic.Transaction_Query == true && Type == Type.Query)
            {
                string _Type = "Query";
                _TransactionRecord(UserName, DataBase, SQL, _Type, Reason);
            }
		}

        static void _TransactionRecord(string UserName, string DataBase, string SQL, string Type, string Reason)
        {
            int Index = _DB_Name.IndexOf(DataBase);
            if (Index >= 0) //Prevents Out of bound errors
            {
                string URI = _DB_Path[Index];
                string _SQL = "INSERT INTO Transactions (USER,DB,SQL,TYPE,Reason,\"UTC DATE\",\"UTC TIME\",PATH,SNAME) VALUES('" + UserName + "','" + DataBase + "','" + SQL.Replace("'", "''") + "','" +Type+ "','" + Reason.Replace("'", "''") + "',Date(),TIME(),'" + URI + "','" + Path.GetDirectoryName(URI) + "');";
                LDDataBase.Command(GlobalStatic.TransactionDB, _SQL);
            }
        }

        public static void Emulator(EnginesMode Mode,string Database,string SQL,string Username,string Listview) //TODO Implement Emulator atleast for sqlite for DBM
        {
            //Attempts to emulate some if not all commands of a database engine by aliasing it to SQL
            Utilities.AddtoStackTrace("Engines.Emulator()");
            StringBuilder Emulator_Sql = new StringBuilder();
            string EmulatorTable = null;

            StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;
            
            switch (Mode)
            {
                case EnginesMode.SQLITE:
                    if (SQL.StartsWith(".help",Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_Help";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (Arguments TEXT,Description TEXT);", EmulatorTable);
                        Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('.help','displays this table. Note: The following commands are being interpreted by the DBM CLI Emulator not the SQLITE CLI.');", EmulatorTable);
                        Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('.databases','Displays a list of connected databases.');", EmulatorTable);
                        Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('.tables','Lists all tables and views in the current database.');", EmulatorTable);
                        Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('.index','Lists all the indexes in the current database.');", EmulatorTable);
                        Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('.pragma','Lists most pragma settings for sqlite db.');", EmulatorTable);
                        break;
                    }
                    else if (SQL.StartsWith(".databases",Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_Databases";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,NAME TEXT,\"DBM NAME\" TEXT,FILE TEXT);", EmulatorTable);
                        for (int i = 0; i < DB_Name.Count; i++)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} (NAME,\"DBM NAME\",FILE) VALUES ('{1}','{2}','{3}');", EmulatorTable, DB_Name[i].Replace("'","''"), DB_ShortName[i].Replace("'", "''"), DB_Path[i].Replace("'", "''"));
                        }
                    }
                    else if (SQL.StartsWith(".tables",Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_Tables";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,NAME TEXT,Type TEXT);", EmulatorTable);
                        for (int i = 0; i < Tables.Count; i++)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} (Name,Type) VALUES ('{1}','table');", EmulatorTable, Tables[i].Replace("'", "''"));
                        }
                        for (int i = 0; i < Views.Count; i++)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} (Name,Type) VALUES ('{1}','view');", EmulatorTable, Views[i].Replace("'", "''"));
                        }
                    }
                    else if (SQL.StartsWith(".index",Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_Index";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,NAME TEXT,Type TEXT);", EmulatorTable);
                        for (int i = 0; i < Indexes.Count; i++)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} (Name,Type) VALUES ('{1}','index');", EmulatorTable, Indexes[i].Replace("'", "''"));
                        }
                    }
                    else if (SQL.StartsWith(".pragma",Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_PRAGMA";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (\"PRAGMA\" TEXT,VALUE TEXT);", EmulatorTable);
                        List<string> PRAGMA = new List<string>()
                        {
                            "Application_ID","Auto_Vacuum","Automatic_Index","Busy_TimeOut","Cache_Size","Cache_Spill","Case_Sensitive_Like","Cell_Size_Check","Checkpoint_fullfsync","data_version","defer_foreign_keys","Encoding","Foreign_Keys","Freelist_count","fullfsync","ignore_check_constraints",
                            "incremental_vacuum","integrity_check","journal_mode","journal_size_limit","legacy_file_format","locking_mode","max_page_count","mmap_size","page_count","page_size","query_only","read_uncommitted","recursive_triggers","reverse_unordered_selects","secure_delete","soft_heap_limit","synchronous","temp_store","threads","user_version",
                            "wal_autocheckpoint","writable_schema"
                        };
                        /*The following were removed due to them displaying 1+ rows of data
                         * Collation_List
                         * Compile_Options
                         * database_list
                         * wal_checkpoint
                         * Foreign_Key_Check
                         */
                        for (int i = 0; i < PRAGMA.Count; i++)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('{1}','{2}');", EmulatorTable, PRAGMA[i].Replace("_"," "), Query(Engines.CurrentDatabase, "PRAGMA " + PRAGMA[i] +";", null, true, Username, "Emulator")[1][PRAGMA[i]]);
                        }
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (EmulatorTable != null)
            {
                string _SQL = Emulator_Sql.ToString();
                Command(Database, "DROP TABLE IF EXISTS " + EmulatorTable + ";" + _SQL, Utilities.Localization["User Requested"] + ": CLI EMULATOR");
                Query(Database, "SELECT * FROM " + EmulatorTable + ";", Listview, false, Username, Utilities.Localization["User Requested"] + ": CLI EMULATOR");
                Engines.SetDefaultTable(EmulatorTable);
                Engines.GetColumnsofTable(Database, EmulatorTable);
            }

        }

        public static void Parser()  //TODO: Implement Parser
        {
            Utilities.AddtoStackTrace("Engines.Parser()");
        }

        public static void GetSchema(string Database)
        {
            Utilities.AddtoStackTrace("Engines.GetSchema()");
            if (string.IsNullOrEmpty(Database))//Prevents Prevents Application from querying a nonexistent db 
            {
                return;
            }

            EnginesMode EngineMode = Engine_Type(Database);
            switch (EngineMode)
            {
                case EnginesMode.SQLITE:
                    _Tables.Clear();
                    _Views.Clear();
                    _Indexes.Clear();

                    Primitive Master_Schema_List = Query(Database, "SELECT tbl_name,name,type FROM sqlite_master UNION Select tbl_name,name,type From SQLite_Temp_Master;", null, true, Utilities.Localization["App"], "SCHEMA");
                    for (int i = 1; i <= Master_Schema_List.GetItemCount(); i++)
                    {
                        string Name = Master_Schema_List[i]["tbl_name"];
                        switch (Master_Schema_List[i]["type"].ToString())
                        {
                            case "table":
                                _Tables.Add(Name);
                                break;
                            case "view":
                                _Views.Add(Name);
                                break;
                            case "index":
                                _Indexes.Add(Name);
                                break;
                        }
                    }
                    try
                    {
                        CurrentTable = _Tables.FirstOrDefault();
                    }
                    catch (Exception ex)
                    {
                        Events.LogMessage(ex.ToString(), "System");
                    }
                    _TrackingDefaultTable.Add(Database + "." + CurrentTable);
                    if (Database != null && CurrentTable != null)
                    {
                        GetColumnsofTable(Database, CurrentTable);
                    }
                    break;
            }
        }

        public static void GetColumnsofTable(string database, string table)
        {
            Utilities.AddtoStackTrace("Engines.GetSchemaofTable()");

            if (string.IsNullOrEmpty(database)) //Prevents calls to nonexistent Databases
            {
                throw new ArgumentException("The Database Paramater cannot be null or empty", database);
            }

            EnginesMode EngineMode = Engine_Type(database);
            switch (EngineMode)
            {
                case EnginesMode.SQLITE:
                    _Schema.Clear();

                    Primitive QSchema = Query(database, "PRAGMA table_info(" + table + ");", null, true, Utilities.Localization["App"], Utilities.Localization["SCHEMA PRIVATE"]);
                    for (int i = 1; i <= QSchema.GetItemCount(); i++)
                    {
                        _Schema.Add(QSchema[i]["name"]);
                    }
                    Schema = _Schema.ToPrimitiveArray(); ;
                    break;
            }
        }

        public static void EditTable(string Table, string Control)
        {
            LDDataBase.EditTable(CurrentDatabase, Table, Control);
        }

        public static void SetDefaultTable(string table)
        { 
                string Characters = "\"[]";

                if (!string.IsNullOrEmpty(table))
                {
                    if (table.IndexOfAny(Characters.ToCharArray()) >= 0)
                    {
                        Events.LogMessage("The presence of the one of the following characters has been detected : \"[] in Engines.SetDefaultTable(" + table + "). This may cause unexpected behaviour.", "Warning");
                    }
                    CurrentTable = "\"" + table.Replace("\"", "").Replace("[", "").Replace("]", "") + "\"";
                    _TrackingDefaultTable.Add(CurrentDatabase + "." + CurrentTable);
                    return;
                }
                Events.LogMessagePopUp("Table does not exist in context", "Table does not exist in context", "Error", Utilities.Localization["System"]);
        }

        public static void OnDefaultTableChanged(EventArgs e) { }


        public static void GenerateQuery(bool Search, bool Sort, bool Function, string SearchBy, string OrderBy, string SortOrder, bool StrictSearch, bool InvertSearch, string FunctionSelected, string FunctionColumn, string SearchText)
        {
            //Interface to private classes
            if (!string.IsNullOrEmpty(CurrentTable))
            {
                GQ_CMD = "SELECT * FROM " + CurrentTable + " ";
                Utilities.AddtoStackTrace("Engines.GenerateQuery()");
                if (Search)
                {
                    GQ_CMD += GenerateSearch(SearchBy, SearchText, InvertSearch, StrictSearch);
                }
                if (Function)
                {
                    GQ_CMD = GenerateFunction(FunctionSelected, FunctionColumn);
                }
                if (Sort)
                {
                    GQ_CMD += GenerateSort(OrderBy, SortOrder);
                }
            }

            Query(CurrentDatabase, GQ_CMD, GlobalStatic.ListView, false, GlobalStatic.UserName, "Auto Generated Query on behalf of " + GlobalStatic.UserName);
            GQ_CMD = null;
        }

        static string GenerateSearch(string SearchColumn, string SearchText, bool InvertSearch, bool StrictSearch)
        {
            string CMD;
            CMD = "WHERE " + SearchColumn;
            if (InvertSearch == true && StrictSearch == false)
            {
                CMD += " NOT";
            }

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

        static string GenerateSort(string OrderBy, string ASCDESC)
        {
            return "ORDER BY \"" + OrderBy + "\" " + ASCDESC + ";";
        }
        static string GenerateFunction(string Function, string Column)
        {
            return "SELECT " + Function + "(\"" + Column + "\") FROM " + CurrentTable + " ";
        }

        public static List<string> Functions(EnginesMode Mode)
        {
            switch (Mode)
            {
                case EnginesMode.SQLITE:
                    return new List<string> {"Avg","Count","Max","Min","Sum","Total","Hex","Length","Lower","Round","Trim","Upper" };
                default:
                    throw new NotImplementedException();
            }
        }

        static EnginesMode Engine_Type(string Database) //Fetches Engine Mode/Type associated with the Database 
        {
            int Index = _DB_Name.IndexOf(Database);
            if (Index != -1)
            {
                return _DB_Engine[Index];
            }
            return EnginesMode.NONE;
        }

        static void AddToList(string path,string Name,string ShortName,EnginesMode Mode,string Hash)
        {
            AddToList(path, Name, ShortName, Mode);
            _DB_Hash.Add(Hash, Name);
        }

        static void AddToList(string path, string Name, string ShortName, EnginesMode Engine)
        {
            DatabaseShortname = ShortName;
            _DB_Name.Add(Name);
            _DB_Path.Add(path);
            _DB_ShortName.Add(ShortName);
            _DB_Engine.Add(Engine);
        }
    }
}