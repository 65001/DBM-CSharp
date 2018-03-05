// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.IO;
using System.Text;
using System.Data.SQLite;
using System.Collections.Generic;
using System.Linq;
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
        /// Feature Flag
        /// </summary>
        public static bool UseCache = false;

        static SQLite Sqlite = new SQLite();
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
                    return DB_Info[CurrentDatabase].Engine;
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

        public struct Connections
        {
            public SQLiteConnection SQLITE;
        }

        public struct DatabaseInfo
        {
            public string Path;
            public string ID;
            public string ShortName;
            public EnginesMode Engine;
            public string Hash;
            public Connections Connections;
        }

        static Dictionary<string, DatabaseInfo> _DBInfo = new Dictionary<string, DatabaseInfo>();
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

        static Dictionary<string, CacheEntry> _Cache = new Dictionary<string, CacheEntry>();
        static List<string> _CacheStatus = new List<string>();

        static List<string> _Last_NonSchema_Query = new List<string>();
        static List<string> _UTC_Start = new List<string>();
        public static event EventHandler OnSchemaChange;
        public static event EventHandler OnGetColumnsofTable;

        public class CacheEntry
        {
            public Primitive Results;
            /// <summary>
            /// Lifetime decrements every time the cache hit til it hits zero where it should be invalidated.
            /// </summary>
            public int LifeTime;
        }

        public static class Cache
        {
            /// <summary>
            /// Forces the invalidation of the query cache
            /// </summary>
            public static void Clear()
            {
                _Cache.Clear();
                _Cache = new Dictionary<string, CacheEntry>();
            }
        }

        /// <summary>
        /// This method should only run when the correct global Paramters are set.
        /// //Current Storage only supports SQLite db file.
        /// </summary>
		public static void TransactionRecord(string UserName, string DataBase, string SQL, Type Type, string Reason) 
		{
            int StackPointer = Stack.Add( "Engines.TransactionRecord("+DataBase+")");
            //Escapes function when conditions are correct
            
            if (DataBase == GlobalStatic.TransactionDB ) //This is done to prevent a stackoverflow. 
            {
                Stack.Exit(StackPointer);
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
            Stack.Exit(StackPointer);
		}

        static void _TransactionRecord(string UserName, string DataBase, string SQL, string Type, string Reason)
        {
            if (DB_Info.ContainsKey(DataBase)) //Prevents Out of bound errors
            {
                string URI = _DBInfo[DataBase].Path;
                
                string _SQL = "INSERT INTO Transactions (USER,DB,SQL,TYPE,Reason,\"UTC DATE\",\"UTC TIME\",PATH,SNAME) VALUES('" + UserName + "','" + DataBase + "','" + SQL.Replace("'", "''") + "','" +Type+ "','" + Reason.Replace("'", "''") + "',Date(),TIME(),'" + URI + "','" + Path.GetDirectoryName(URI) + "');";
                LDDataBase.Command(GlobalStatic.TransactionDB, _SQL);
            }
        }

        public static void GetSchema(string Database)
        {
            int StackPointer = Stack.Add($"Engines.GetSchema({Database})");
            if (string.IsNullOrEmpty(Database))//Prevents Prevents Application from querying a nonexistent db 
            {
                Stack.Exit(StackPointer);
                return;
            }
            
            EnginesMode EngineMode = Engine_Type(Database);
            switch (EngineMode)
            {
                case EnginesMode.SQLITE:
                    _Tables.Clear();
                    _Views.Clear();
                    _Indexes.Clear();

                    QuerySettings QS = new QuerySettings
                    {
                        Database = Database,
                        SQL = Sqlite.GetSchema(),
                        FetchRecords = true,
                        User = Language.Localization["App"],
                        Explanation = "SCHEMA"
                    };

                    Primitive Master_Schema_List = Query(QS);
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
            Stack.Exit(StackPointer);
        }

        public static void GetColumnsofTable(string database, string table)
        {
            int StackPointer = Stack.Add($"Engines.GetSchemaofTable({database},{table})");

            if (string.IsNullOrEmpty(database)) //Prevents calls to nonexistent Databases
            {
                throw new ArgumentException("The Database Paramater cannot be null or empty", database);
            }

            EnginesMode EngineMode = Engine_Type(database);
            switch (EngineMode)
            {
                case EnginesMode.SQLITE:
                    _Schema.Clear();
                    QuerySettings QS = new QuerySettings
                    {
                        Database = database,
                        SQL = Sqlite.GetColumnsOfTable(table),
                        FetchRecords = true,
                        User = Language.Localization["App"],
                        Explanation = Language.Localization["SCHEMA PRIVATE"]
                    };

                    Primitive QSchema = Query(QS);
                    _Schema = Sqlite.GetColumnsOfTable(QSchema);
                    break;
                default:
                    throw new NotImplementedException();
            }
            Schema = _Schema.ToPrimitiveArray();
            OnGetColumnsofTable?.Invoke(null, EventArgs.Empty);
            Stack.Exit(StackPointer);
        }

        public static void EditTable(string Table, string Control)
        {
            LDDataBase.EditTable(CurrentDatabase, Table, Control);
        }

        public static void SetDefaultTable(string table)
        {
            int StackPointer = Stack.Add($"Enginges.SetDefaultTable({table})");
                string Characters = "\"[]";

                if (!string.IsNullOrEmpty(table))
                {
                    if (table.IndexOfAny(Characters.ToCharArray()) >= 0)
                    {
                        Events.LogMessage("The presence of the one of the following characters has been detected : \"[] in Engines.SetDefaultTable(" + table + "). This may cause unexpected behaviour.", "Warning");
                    }
                    CurrentTable = $"\"{table.SanitizeFieldName()}\"";
                    _TrackingDefaultTable.Add(CurrentDatabase + "." + CurrentTable);
                    Stack.Exit(StackPointer);
                    return;
                }
                Events.LogMessagePopUp("Table does not exist in context", "Table does not exist in context", "Error", Language.Localization["System"]);
            Stack.Exit(StackPointer);
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

        /// <summary>
        /// Fetches Engine Mode/Type associated with the Database 
        /// </summary>
        static EnginesMode Engine_Type(string Database) 
        {
            if (DB_Info.ContainsKey(Database))
            {
                return _DBInfo[Database].Engine;
            }
            return EnginesMode.NONE;
        }

        static void AddToList(string path,string Name,string ShortName,EnginesMode Mode,string Hash)
        {
            AddToList(path, Name, ShortName, Mode);
            _DB_Hash.Add(Hash, Name);

            var Info = _DBInfo[Name];
            Info.Hash = Hash;
            _DBInfo[Name] = Info;
        }

        static void AddToList(string path, string Name, string ShortName, EnginesMode Engine)
        {
            DatabaseShortname = ShortName;
            var Connections = new Connections
            {
                SQLITE = GetConnection(Name),
            };

            var Info = new DatabaseInfo
            {
                ID = Name,
                Path = path,
                ShortName = ShortName,
                Engine = Engine,
                Connections = Connections
            };

            _DBInfo.Add(Name, Info);
        }
    }
}