// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;

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
            return Command(Database, SQL, LDFile.UserName, Explanation, false);
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
		public static void TransactionRecord(string UserName, string DataBase, string SQL, Type Type, string Reason) //TODO Transactions 
		{
            Utilities.AddtoStackTrace( "Engines.TransactionRecord()");
            //Escapes function when conditions are correct
            if (GlobalStatic.Transactions == false || DataBase == GlobalStatic.TransactionDB )
            {
                return;
            }

            if (GlobalStatic.Transaction_Commands == true && Type == Type.Command)
            {

            }

            if (GlobalStatic.Transaction_Query == true && Type == Type.Query)
            {
                int Index = _DB_Name.IndexOf(CurrentDatabase);
                if (Index >= 0) //Prevents Out of bound errors
                {
                        string URI = _DB_Path[Index];
                        string _SQL = "INSERT INTO Transactions (USER,DB,SQL,TYPE,Reason,\"UTC DATE\",\"UTC TIME\",PATH,SNAME) VALUES('" + UserName + "','" + DataBase + "','" + SQL.Replace("'", "''") + "','Query','" + Reason.Replace("'", "''") + "',Date(),TIME(),'" + URI + "','" + LDFile.GetFolder(URI) + "');";
                        LDDataBase.Command(GlobalStatic.TransactionDB, _SQL);
                }
            }
		}

        public static void Emulator() //TODO Implement Emulator atleast for sqlite for DBM
        {
            //Attempts to emulate some if not all commands of a database engine by aliasing it to SQL
            Utilities.AddtoStackTrace("Engines.Emulator()");
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
            if (!string.IsNullOrEmpty(table))
            {
                CurrentTable = "\"" + table + "\"";
                _TrackingDefaultTable.Add(CurrentDatabase + "." + CurrentTable);
                return;
            }
            Events.LogMessagePopUp("Table does not exist in context", "Table does not exist in context", "Error", Utilities.Localization["System"]);
        }

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

        public static Primitive Functions(EnginesMode Mode)
        {
            switch (Mode)
            {
                case EnginesMode.SQLITE:
                    return "1=Avg;2=Count;3=Max;4=Min;5=Sum;6=Total;7=Hex;8=Length;9=Lower;10=round;11=Trim;12=Upper;";
                default:
                    return "1=Avg;2=Count;3=Max;4=Min;5=Sum;6=Total;7=Hex;8=Length;9=Lower;10=round;11=Trim;12=Upper;";
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

        public static class Load
        {
            /// <summary>
            /// Old Method. Kept in for backwards compatability.
            /// </summary>
            /// <param name="compatability"></param>
            /// <returns></returns>
            public static string DB(EnginesMode Mode, Primitive Data)
            {
                Dictionary<string, string> _Data = new Dictionary<string, string>
                {
                    ["URI"] = Data
                };
                return DB(Mode, _Data);
            }

            public static string DB(EnginesMode Mode, Dictionary<string, string> Data) //Tasked with connecting to a Database and adding the DB Connection Name to a list.
            {
                switch (Mode)
                {
                    case EnginesMode.SQLITE:
                        return DB(Mode, Data, LDFile.GetFile(Data["URI"]));
                    default:
                        throw new NotImplementedException();
                }
            }

            public static string DB(EnginesMode Mode, Dictionary<string, string> Data, string ShortName)
            {
                //MAKE SURE The CurrentMode is always currently changed.
                Utilities.AddtoStackTrace("Engines.Load.DB()");
                string HashCode = Data.ToPrimitiveArray();
                //If DB is already in the list...
                if (_DB_Hash.ContainsKey(HashCode))
                {
                    CurrentDatabase = _DB_Hash[HashCode];
                    return CurrentDatabase;
                }

                //New Database creation code
                switch (Mode)
                {
                    case EnginesMode.MySQL:
                        CurrentDatabase = LDDataBase.ConnectMySQL(Data["Server"], Data["User"], Data["Password"], Data["Database"]);
                        _DB_Hash.Add(HashCode, CurrentDatabase);
                        return CurrentDatabase;
                    case EnginesMode.ODBC:
                        CurrentDatabase = LDDataBase.ConnectOdbc(Data["Driver"], Data["Server"], Data["Port"], Data["User"], Data["Password"], Data["Option"], Data["Database"]);
                        _DB_Hash.Add(HashCode, CurrentDatabase);
                        return CurrentDatabase;
                    case EnginesMode.OLEDB:
                        CurrentDatabase = LDDataBase.ConnectOleDb(Data["Provider"], Data["Server"], Data["Database"]);
                        _DB_Hash.Add(HashCode, CurrentDatabase);
                        return CurrentDatabase;
                    case EnginesMode.SQLITE:
                        if (System.IO.Directory.Exists(LDFile.GetFolder(Data["URI"])))
                        {
                            string Database = LDDataBase.ConnectSQLite(Data["URI"]);
                            AddToList(Data["URI"], Database, ShortName, EnginesMode.SQLITE);
                            Settings.SaveSettings();
                            CurrentDatabase = Database;
                            _DB_Hash.Add(HashCode, CurrentDatabase);
                            return Database;
                        }
                        return null;
                    case EnginesMode.SQLSERVER:
                        CurrentDatabase = LDDataBase.ConnectSqlServer(Data["Server"], Data["Database"]);
                        _DB_Hash.Add(HashCode, CurrentDatabase);
                        return CurrentDatabase;
                    default:
                        return "Incorrect Paramters";
                }
            }

            public static void MemoryDB(EnginesMode Mode)
            {
                switch (Mode)
                {
                    case EnginesMode.SQLITE:
                        AddToList("", LDDataBase.ConnectSQLite(""), "SQLITE MEM DB", EnginesMode.SQLITE, "URI=;");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            public static string Sqlite(string FilePath)
            {
                Dictionary<string, string> _Data = new Dictionary<string, string>
                {
                    ["URI"] = FilePath
                };
                return DB(EnginesMode.SQLITE, _Data);
            }

            public static string Sqlite(string FilePath, string ShortName)
            {
                Dictionary<string, string> _Data = new Dictionary<string, string>
                {
                    ["URI"] = FilePath
                };
                return DB(EnginesMode.SQLITE, _Data, ShortName);
            }

            public static string SQLServer(string Server, string Database)
            {
                Dictionary<string, string> Data = new Dictionary<string, string>
                {
                    ["Server"] = Server,
                    ["Database"] = Database
                };
                return DB(EnginesMode.SQLSERVER, Data);
            }

            public static string MySQL(string Server, string Database, string User, string Password)
            {
                Dictionary<string, string> Data = new Dictionary<string, string>
                {
                    ["Server"] = Server,
                    ["User"] = User,
                    ["Password"] = Password,
                    ["Database"] = Database
                };
                return DB(EnginesMode.MySQL, Data);
            }

            public static string OLEDB(string Server, string Database, string Provider)
            {
                Dictionary<string, string> Data = new Dictionary<string, string>
                {
                    ["Provider"] = Provider,
                    ["Server"] = Server,
                    ["Database"] = Database
                };
                return DB(EnginesMode.OLEDB, Data);
            }

            public static string ODBC(string Server, string Database, string User, string Password, int Port, string Driver, string Option)
            {
                Dictionary<string, string> Data = new Dictionary<string, string>
                {
                    ["Driver"] = Driver,
                    ["Server"] = Server,
                    ["Port"] = Port.ToString(CultureInfo.InvariantCulture),
                    ["User"] = User,
                    ["Password"] = Password,
                    ["Option"] = Option,
                    ["Database"] = Database
                };
                return DB(EnginesMode.ODBC, Data);
            }
        }
        public static class Transform
        {
            public static void CreateStatisticsTable(string Database,string Table,string StatTableName,Primitive Schema)
            {
                Utilities.AddtoStackTrace("Engines.Transform.CreateStatisticsPage()");
                StringBuilder SQL = new StringBuilder();
                switch (Engines.DB_Engine[Engines.DB_Name.IndexOf(Database)])
                {
                    case EnginesMode.SQLITE:
                        SQL.AppendLine("DROP TABLE IF EXISTS " + StatTableName + ";");
                        SQL.AppendLine("CREATE TEMP TABLE " + StatTableName + " (Row Text,SUM INT,AVG INT,COUNT INT,\"COUNT DISTINCT\" INT,MAX INT,MIN INT,TYPE Text,Length TEXT,\"UTC DATE\" TEXT,\"UTC TIME\" TEXT);");
                        for (int i = 1; i <= Schema.GetItemCount(); i++)
                        {
                            string Row = "\"" + Schema[i] + "\"";
                            SQL.AppendFormat("INSERT INTO {0} Select '{1}',SUM({1}),AVG({1}),COUNT({1}),COUNT(DISTINCT {1}),MAX({1}),MIN({1}),typeOf({1}),Length({1}),DATE(),TIME() FROM {2};;",StatTableName,Row,Table);
                        }
                        Command(Database, SQL.ToString(),Utilities.Localization["Application"] + ":" + Utilities.Localization["User Requested"] + ":" + Utilities.Localization["Statistics Page"]);
                        break;
                }
            }
        }

        //Read Only Collections of Private Data
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

            public static ReadOnlyCollection<EnginesMode> DB_Engine
            {
                get { return _DB_Engine.AsReadOnly(); }
            }

            public static IReadOnlyList<string> Tables
            {
                get { return _Tables.AsReadOnly(); }
            }

            public static IReadOnlyList<string> Views
            {
                get { return _Views.AsReadOnly(); }
            }

            public static IReadOnlyList<string> Indexes
            {
                get { return _Indexes.AsReadOnly(); }
            }

            public static IReadOnlyList<Type> Types
            {
                get { return _Type_Referer.AsReadOnly(); }
            }

            public static IReadOnlyList<long> Timer
            {
                get { return _Timer.AsReadOnly(); }
            }

            public static IReadOnlyList<string> LastQuery
            {
                get { return _Last_Query.AsReadOnly(); }
            }

            public static IReadOnlyList<string> LastNonSchemaQuery
            {
                get { return _Last_NonSchema_Query.AsReadOnly(); }
            }
    }
}