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

        static SQLITE Sqlite = new SQLITE();
        static OLDEB Oldeb = new OLDEB();
        static ODBC Odbc = new ODBC();
        static MySQL Mysql = new MySQL();
        static SQLServer SQLserver = new SQLServer();

        public static int GetDataBaseIndex(string Database)
        {
            return DB_Name.IndexOf(Database);
        }

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

        /// <summary>
        /// Auto Generated Query SQL Statements 
        /// </summary>
		public static string GQ_CMD { get; private set; } 

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
        static List<string> _Explanation = new List<string>();
        static List<string> _User = new List<string>();
        static List<string> _Last_Query = new List<string>();
        static List<string> _Last_NonSchema_Query = new List<string>();
        static List<string> _UTC_Start = new List<string>();
        public static event EventHandler OnSchemaChange;
        public static event EventHandler OnGetColumnsofTable;
        
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
            Utilities.AddtoStackTrace($"Engines.Command({Database})");
            _UTC_Start.Add(DateTime.UtcNow.ToString("hh:mm:ss tt"));
            Stopwatch CommandTime = Stopwatch.StartNew();
			if (RunParser == false)
			{
                if (GlobalStatic.Transaction_Commands == true)
                {
                    TransactionRecord(User, Database, SQL, Type.Command, Explanation);
                 }
                return LDDataBase.Command(Database, SQL);
			}

            _Type_Referer.Add(Type.Command);
            _Timer.Add(CommandTime.ElapsedMilliseconds);
            _Explanation.Add(Explanation);
            _User.Add(User);
            //TODO Implement Parser Stuff 
			return 0;
		}

		public static Primitive Query(string DataBase, string SQL, string ListView, bool FetchRecords, string UserName, string Explanation) //Expand
		{
            Utilities.AddtoStackTrace("Engines.Query()");

            _UTC_Start.Add(DateTime.UtcNow.ToString("hh:mm:ss tt"));
            Stopwatch QueryTime = Stopwatch.StartNew();
            
            if (SQL.StartsWith("."))
            {
                Emulator(DB_Engine[DB_Name.IndexOf(DataBase)],DataBase, SQL,UserName,ListView);
                return null;
            }

            if (GlobalStatic.Transaction_Query == true)
            {
                TransactionRecord(UserName, DataBase, SQL, Type.Query, Explanation);
            }
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
            _Explanation.Add(Explanation);
            _User.Add(UserName);
			return QueryResults;
		}

        /// <summary>
        /// This method should only run when the correct global Paramters are set.
        /// //Current Storage only supports SQLite db file.
        /// </summary>
		public static void TransactionRecord(string UserName, string DataBase, string SQL, Type Type, string Reason) 
		{
            Utilities.AddtoStackTrace( "Engines.TransactionRecord("+DataBase+")");
            //Escapes function when conditions are correct
            
            if (DataBase == GlobalStatic.TransactionDB ) //This is done to prevent a stackoverflow. 
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

        public static void Parser()  //TODO: Implement Parser
        {
            Utilities.AddtoStackTrace("Engines.Parser()");
        }

        public static void GetSchema(string Database)
        {
            Utilities.AddtoStackTrace($"Engines.GetSchema({Database})");
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
                    Primitive Master_Schema_List = Query(Database,Sqlite.GetSchemaQuery(), null, true, Utilities.Localization["App"], "SCHEMA");
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
                    break;
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
            OnSchemaChange?.Invoke(null, EventArgs.Empty);
        }

        public static void GetColumnsofTable(string database, string table)
        {
            Utilities.AddtoStackTrace($"Engines.GetSchemaofTable({database},{table})");

            if (string.IsNullOrEmpty(database)) //Prevents calls to nonexistent Databases
            {
                throw new ArgumentException("The Database Paramater cannot be null or empty", database);
            }

            EnginesMode EngineMode = Engine_Type(database);
            string SchemaQuery;
            switch (EngineMode)
            {
                case EnginesMode.SQLITE:
                    _Schema.Clear();
                    SchemaQuery = Sqlite.GetSchemaQuery();
                    Primitive QSchema = Query(database, SchemaQuery, null, true, Utilities.Localization["App"], Utilities.Localization["SCHEMA PRIVATE"]);
                    _Schema = Sqlite.GetColumnsOfTable(QSchema);
                    break;
                default:
                    throw new NotImplementedException();
            }
            Schema = _Schema.ToPrimitiveArray();
            OnGetColumnsofTable?.Invoke(null, EventArgs.Empty);
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
                    CurrentTable = $"\"{table.SanitizeFieldName()}\"";
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
        #if DEBUG
            Console.WriteLine("Generated Query :{0}",GQ_CMD);
        #endif
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
            if (string.IsNullOrWhiteSpace(OrderBy) == false)
            {
                OrderBy = "\"" + OrderBy.SanitizeFieldName() + "\"";
            }
            return string.Format("ORDER BY {0} {1};",OrderBy,ASCDESC);
        }
        static string GenerateFunction(string Function, string Column)
        {
            return string.Format( "SELECT {0} (\"{1}\") FROM {2} ",Function, Column.SanitizeFieldName(), CurrentTable);
        }

        public static List<string> Functions(EnginesMode Mode)
        {
            switch (Mode)
            {
                case EnginesMode.SQLITE:
                    List<string> Func = new List<string> { "Avg", "Count", "Max", "Min", "Sum", "Total", "Hex", "Length", "Lower", "Round", "Trim", "Upper","Sqrt","Abs" };
                    Func.Sort();
                    return Func;
                default:
                    throw new NotImplementedException();
            }
        }

        static EnginesMode Engine_Type(string Database) //Fetches Engine Mode/Type associated with the Database 
        {
            int Index = _DB_Name.IndexOf(Database);
            if (Index >= 0)
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