using System;
using System.Globalization;
using LitDev;
using Microsoft.SmallBasic.Library;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBM
{
    public static partial class Engines
    {
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
            public static void CreateStatisticsTable(string Database, string Table, string StatTableName, Primitive Schema)
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
                            SQL.AppendFormat("INSERT INTO {0} Select '{1}',SUM({1}),AVG({1}),COUNT({1}),COUNT(DISTINCT {1}),MAX({1}),MIN({1}),typeOf({1}),Length({1}),DATE(),TIME() FROM {2};;", StatTableName, Row, Table);
                        }
                        Command(Database, SQL.ToString(), Utilities.Localization["Application"] + ":" + Utilities.Localization["User Requested"] + ":" + Utilities.Localization["Statistics Page"]);
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
