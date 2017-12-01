using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.SmallBasic.Library;
using Google_Charts;

namespace DBM
{
    public static partial class Engines
    {
        public static void Emulator(EnginesMode Mode, string Database, string SQL, string Username, string Listview) //TODO Implement Emulator atleast for sqlite for DBM
        {
            //Attempts to emulate some if not all commands of a database engine by aliasing it to SQL
            Utilities.AddtoStackTrace($"Engines.Emulator({Mode},{Database},{SQL}");
            StringBuilder Emulator_Sql = new StringBuilder();
            string EmulatorTable = null;

            StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;

            switch (Mode)
            {
                case EnginesMode.SQLITE:
                    if (SQL.StartsWith(".help", Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_Help";
                        List<string> Arguments = new List<string>()
                        {
                            ".help",".databases",".tables",".index",".pragma",".querytimes",".filesystem $Path",".stacktrace",".localizations",".functions"
                        };
                        Dictionary<string, string> Description = new Dictionary<string, string>
                        {
                            { ".help", "Displays this table.Note: The following commands are being interpreted by the DBM CLI Emulator not the SQLITE CLI." },
                            { ".databases", "Displays a list of connected databases." },
                            { ".tables","Lists all tables and views in the current database."},
                            { ".index","Lists all the indexes in the current database."},
                            { ".pragma","Lists most pragma settings for sqlite db."},
                            { ".querytimes","Lists all queries executed by the program and the time in miliseconds."},
                            { ".filesystem $Path","Lists all the folders and files in the argumented path. Defaults to MyDocuments if no argument is given." },
                            { ".stacktrace","Lists the functions and methods and the like and the order they were called in." },
                            { ".localizations","List of the key value pairs of the current languages localization"},
                            { ".functions","List of all custom functions."},
                            { ".charts","A List of all charts currentley implemented"}
                        };
                        Arguments.Sort();
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (Arguments TEXT,Description TEXT);", EmulatorTable);

                        for (int i = 0; i < Arguments.Count; i++)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('{1}','{2}');", EmulatorTable, Arguments[i], Description[Arguments[i]]);
                        }
                        break;
                    }
                    else if (SQL.StartsWith(".databases", Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_Databases";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,NAME TEXT,\"DBM NAME\" TEXT,FILE TEXT);", EmulatorTable);
                        for (int i = 0; i < DB_Name.Count; i++)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} (NAME,\"DBM NAME\",FILE) VALUES ('{1}','{2}','{3}');", EmulatorTable, DB_Name[i].Replace("'", "''"), DB_ShortName[i].Replace("'", "''"), DB_Path[i].Replace("'", "''"));
                        }
                    }
                    else if (SQL.StartsWith(".tables", Comparison) || SQL.StartsWith(".views", Comparison))
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
                    else if (SQL.StartsWith(".index", Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_Index";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,NAME TEXT,Type TEXT);", EmulatorTable);
                        for (int i = 0; i < Indexes.Count; i++)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} (Name,Type) VALUES ('{1}','index');", EmulatorTable, Indexes[i].Replace("'", "''"));
                        }
                    }
                    else if (SQL.StartsWith(".pragma", Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_PRAGMA";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (\"PRAGMA\" TEXT,VALUE TEXT);", EmulatorTable);
                        List<string> PRAGMA = new List<string>()
                        {
                            "Application_ID","Auto_Vacuum","Automatic_Index","Busy_TimeOut","Cache_Size","Cache_Spill","Case_Sensitive_Like","Cell_Size_Check","Checkpoint_fullfsync","data_version","defer_foreign_keys","Encoding","Foreign_Keys","Freelist_count","fullfsync","ignore_check_constraints",
                            "incremental_vacuum","integrity_check","journal_mode","journal_size_limit","legacy_file_format","locking_mode","max_page_count","mmap_size","page_count","page_size","query_only","read_uncommitted","recursive_triggers","reverse_unordered_selects","secure_delete","soft_heap_limit","synchronous","temp_store","threads","user_version",
                            "wal_autocheckpoint","writable_schema"
                        };

                        PRAGMA.Sort();
                        /*The following were removed due to them displaying 1+ rows of data
                         * Collation_List
                         * Compile_Options
                         * database_list
                         * wal_checkpoint
                         * Foreign_Key_Check
                         */
                        for (int i = 0; i < PRAGMA.Count; i++)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('{1}','{2}');", EmulatorTable, PRAGMA[i].Replace("_", " "), Query(Engines.CurrentDatabase, "PRAGMA " + PRAGMA[i] + ";", null, true, Username, "Emulator")[1][PRAGMA[i]]);
                        }
                    }
                    else if (SQL.StartsWith(".filesystem", Comparison))
                    {
                        try
                        {
                            string _Path = SQL.Substring(".filesystem".Length);
                            if (string.IsNullOrWhiteSpace(_Path))
                            {
                                _Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            }
                            List<string> DirectoryList = new List<string>(System.IO.Directory.EnumerateDirectories(_Path));
                            List<string> FileList = new List<string>(System.IO.Directory.EnumerateFiles(_Path));

                            EmulatorTable = "DBM_SQLITE_Filesystem";
                            Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (Name TEXT,\"Date Modified\" TEXT,TYPE TEXT,SIZE INT);", EmulatorTable);

                            for (int i = 0; i < DirectoryList.Count; i++)
                            {
                                string Directory = DirectoryList[i];
                                DirectoryInfo DI = new DirectoryInfo(Directory);
                                Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('{1}','{2}','File Folder',NULL);", EmulatorTable, Directory.Replace("'","''"), DI.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            }

                            for (int i = 0; i < FileList.Count; i++)
                            {
                                string File = FileList[i];
                                FileInfo FI = new FileInfo(File);

                                Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('{1}','{2}','{3}','{4}');", EmulatorTable, File.Replace("'","''"), FI.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"), FI.Extension, FI.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            Events.LogMessage(ex.Message, Utilities.Localization["System"]);
                        }
                    }
                    else if (SQL.StartsWith(".querytimes", Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_QueryTimes";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,SQL TEXT,\"Execution Time (ms)\" INTEGER,Explanation TEXT,User TEXT,\"Start Time (UTC)\" TEXT);", EmulatorTable);

                        int ii = 0;
                        LastQuery.Print();
                        for (int i = 0; i < _Type_Referer.Count; i++)
                        {
                            if (_Type_Referer[i] == Type.Query)
                            {
                                Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES('{1}','{2}','{3}','{4}','{5}','{6}');", EmulatorTable, ii, LastQuery[ii].Replace("'", "''"), _Timer[i], _Explanation[i], _User[i], _UTC_Start[i]);
                                ii++;
                            }
                        #if DEBUG
                            else
                            {
                                Console.WriteLine("#{0} is a {1} with a time of {2}(ms)", i, _Type_Referer[i], _Timer[i]);
                            }
                        #endif
                        }
                    }
                    else if (SQL.StartsWith(".stacktrace", Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_StackTrace";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,Item TEXT,\"Start Time (UTC)\" TEXT);", EmulatorTable);
                        for (int i = 0; i < Utilities.StackTrace.Count; i++)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES('{1}','{2}','{3}');", EmulatorTable, i, Utilities.StackTrace[i], Utilities.StackIniationTime[i]);
                        }
                    }
                    else if (SQL.StartsWith(".localization", Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_Localizations";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,KEY TEXT,VALUE TEXT);", EmulatorTable);
                        foreach (KeyValuePair<string, string> entry in Utilities.Localization)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} (Key,Value) VALUES('{1}','{2}');", EmulatorTable, entry.Key, entry.Value);
                        }
                    }
                    else if (SQL.StartsWith(".function", Comparison))
                    {
                        EmulatorTable = "DBM_Sqlite_Functions";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,Function TEXT,Description TEXT);", EmulatorTable);
                        Dictionary<string, string> Functions = new Dictionary<string, string>()
                        {
                            {"REGEXP(input,pattern)","Pattern matching" },
                            {"POWER(b,x)","b^x" },
                            {"Sqrt(x)","x^(1/2)" },
                            {"Sin(x)","" },
                            {"Sinh(x)","" },
                            {"Cos(x)","" },
                            {"Cosh(x)","" },
                            {"Tan(x)","" },
                            {"Tanh(x)","" },
                            {"e()","" },
                            {"pi()","" },
                            {"log(x)","" },
                            {"encrypt(text,password)","Encrypts the text given the password using AES encryption." },
                            {"decrypt(text,password)","Decrypts the text given the password using AES encryption." },
                            {"hash(text)","Hashes an item using SHA512." },
                        };

                        foreach (KeyValuePair<string, string> entry in Functions)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} (Function,Description) VALUES('{1}','{2}');", EmulatorTable, entry.Key, entry.Value);
                        }
                    }
                    else if (SQL.StartsWith(".charts", Comparison))
                    {
                        //TODO Make a list of all available charts
                        EmulatorTable = "DBM_Charts";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,Chart TEXT,Arguments TEXT);", EmulatorTable);
                        Dictionary<string, string> Charts = new Dictionary<string, string>()
                        {
                            {"bar","Column(s)?,Values,Title,SubTitle,Xaxis,Yaxis" },
                            {"column","Column(s)?,Values,Title,SubTitle,Xaxis,Yaxis" },
                            {"pie","Column(s)?,Values,Title,SubTitle,Xaxis,Yaxis" },
                            {"sankey","From,To,Weight,Title,SubTitle,Xaxis,Yaxis"},
                            {"org","Position,Manager,Name,Title,SubTitle,Xaxis,Yaxis"},
                            {"scatter" ,"Column(s)?,Values,Title,SubTitle,Xaxis,Yaxis"},
                            {"histogram","Column(s)?,Values,Title,SubTitle,Xaxis,Yaxis"},
                            {"table","Column1,Column2,Column3,Title,SubTitle,Xaxis,Yaxis"},
                            {"line","Column1,Column2,Column3,Title,SubTitle,Xaxis,Yaxis"}
                        };
                        foreach (KeyValuePair<string, string> entry in Charts)
                        {
                            Emulator_Sql.AppendFormat("INSERT INTO {0} (Chart,Arguments) VALUES('{1}','{2}');", EmulatorTable, entry.Key, entry.Value);
                        }
                    }
                    else if (SQL.StartsWith(".bar", Comparison))
                    {
                        //Column(s)?,Values,Title,SubTitle,Xaxis,Yaxis
                        Chart chart = new Chart.Bar();
                        string[] Arguments = SQL.Substring(0, SQL.Length - 1).Remove(0, 5).Split(',');
                        Import(chart, Arguments);
                        Write(chart, "Bar");
                    }
                    else if (SQL.StartsWith(".pie", Comparison))
                    {
                        //Column(s)?,Values,Title,SubTitle,Xaxis,Yaxis
                        Chart chart = new Chart.Pie();
                        string[] Arguments = SQL.Substring(0, SQL.Length - 1).Remove(0, 5).Split(',');
                        Import(chart, Arguments);
                        Write(chart, "Pie");
                    }
                    else if (SQL.StartsWith(".column", Comparison))
                    {
                        //Column(s)?,Values,Title,SubTitle,Xaxis,Yaxis
                        Chart chart = new Chart.Column();
                        string[] Arguments = SQL.Substring(0, SQL.Length - 1).Remove(0, 7).Split(',');
                        Import(chart, Arguments);
                        Write(chart, "Column");
                    }
                    else if (SQL.StartsWith(".line", Comparison))
                    {
                        //Column(s)?,Values,Title,SubTitle,Xaxis,Yaxis
                        Chart chart = new Chart.Line();
                        string[] Arguments = SQL.Substring(0, SQL.Length - 1).Remove(0, 5).Split(',');
                        Import(chart, Arguments);
                        Write(chart, "Line");
                    }
                    else if (SQL.StartsWith(".sankey", Comparison))
                    {
                        //Columns(From,To,..),Title,SubTitle,Xaxis,Yaxis
                        Chart chart = new Chart.SanKey();
                        string[] Arguments = SQL.Substring(0, SQL.Length - 1).Remove(0, 7).Split(',');
                        Import(chart, Arguments);
                        Write(chart, "Sankey");
                    }
                    else if (SQL.StartsWith(".org", Comparison))
                    {
                        //Columns(From,To,..),Title,SubTitle,Xaxis,Yaxis
                        Chart chart = new Chart.OrgChart();
                        string[] Arguments = SQL.Substring(0, SQL.Length - 1).Remove(0, 4).Split(',');
                        Import(chart, Arguments);
                        Write(chart, "Org");
                    }
                    else if (SQL.StartsWith(".histogram", Comparison))
                    {
                        Chart chart = new Chart.Histograms();
                        string[] Arguments = SQL.Substring(0, SQL.Length - 1).Remove(0, 10).Split(',');
                        Import(chart, Arguments);
                        Write(chart, "Histogram");
                    }
                    else if (SQL.StartsWith(".scatter", Comparison))
                    {
                        Chart chart = new Chart.Scatter();
                        string[] Arguments = SQL.Substring(0, SQL.Length - 1).Remove(0, 8).Split(',');
                        Import(chart, Arguments);
                        Write(chart, "Scatter");
                    }
                    else if (SQL.StartsWith(".table", Comparison))
                    {
                        Chart chart = new Chart.Table();
                        string[] Arguments = SQL.Substring(0, SQL.Length - 1).Remove(0, 6).Split(',');
                        Import(chart, Arguments);
                        Write(chart, "Table");
                    }
                    else if (SQL.StartsWith(".geo", Comparison))
                    {
                        Chart chart = new Chart.GeoCharts();
                        string[] Arguments = SQL.Substring(0, SQL.Length - 1).Remove(0, 4).Split(',');
                        Import(chart, Arguments);
                        Write(chart, "GeoChart");
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (EmulatorTable != null)
            {
                string _SQL = Emulator_Sql.ToString();
                Command(Database,$"DROP TABLE IF EXISTS {EmulatorTable};" + _SQL, Utilities.Localization["User Requested"] + ": CLI EMULATOR");
                Query(Database, $"SELECT * FROM {EmulatorTable};", Listview, false, Username, Utilities.Localization["User Requested"] + ": CLI EMULATOR");

                GetSchema(Database);
                SetDefaultTable(EmulatorTable);
                GetColumnsofTable(Database, EmulatorTable);
            }
        }

        static void Write(Chart chart,string ChartType)
        {
            string OutPut = Path.GetDirectoryName(DB_Path[GetDataBaseIndex(CurrentDatabase)]) + string.Format("\\{0} {1} Chart.html",CurrentTable.SanitizeFieldName(), ChartType);
            chart.Write(OutPut);
            LitDev.LDProcess.Start(OutPut, null);
        }

        static void Import(Chart chart, string[] Arguments)
        {
            //TODO add support for multiple columns and value types..
            Arguments[0] = Arguments[0].Remove(0,0)?.Replace("(","")?.Replace(")","");
            string SQLQuery = string.Empty;
            if (chart.MinColumns == 1)
            {
                SQLQuery = string.Format("SELECT {0},{1} FROM \"{2}\" GROUP BY {0};", Arguments[0], Arguments[1], CurrentTable.SanitizeFieldName());
            }
            else if (chart.MinColumns == 2)
            {
                SQLQuery = string.Format("SELECT {0},{1},{2} FROM \"{3}\";", Arguments[0], Arguments[1],Arguments[2], CurrentTable.SanitizeFieldName());
            }
            Console.WriteLine(SQLQuery);
            Primitive Data = Query(CurrentDatabase,
                SQLQuery,
                null,
                true,
                "App",
                "Chart"
                );
            Console.WriteLine((string)Data);
            Import(chart, Data, Arguments);
        }

        static void Import(Chart chart,Primitive Data,string[] Arguments)
        {
            Primitive Columns = Data[1].GetAllIndices();
            //TODO add better type determination for charts...
            //Current Assumption is the last value will always be a number
            //and all previous columns are string data types..
            for (int i = 1; i <= Columns.GetItemCount(); i++)
            {
                if (i < Columns.GetItemCount())
                {
                    chart.AddColumn(Columns[i]);
                }
                else
                {
                    chart.AddColumn(Columns[i], Chart.DataType.number);
                }
            }

            for (int i = 1; i <= Data.GetItemCount(); i++)
            {
                for (int ii = 1; ii <= Data[i].GetItemCount(); ii++)
                {
                    chart.AddRowData(i - 1, Data[i][Columns[ii]]);
                }
            }

            if (Arguments.Length > chart.MinColumns + 1)
            {
                chart.Title = (Arguments[chart.MinColumns + 1] ?? CurrentTable)?.Replace("'", "");
            }
            else
            {
                chart.Title = CurrentTable.Replace("'","").Replace("\"","");
            }

            if (Arguments.Length > chart.MinColumns + 2)
            {
                chart.SubTitle = (Arguments[chart.MinColumns + 2] ?? string.Empty)?.Replace("'", "");
            }

            if (Arguments.Length > chart.MinColumns + 3)
            {
                chart.Xaxis = (Arguments[chart.MinColumns + 3] ?? string.Empty)?.Replace("'", "");
            }

            if (Arguments.Length > chart.MinColumns + 4)
            {
                chart.Yaxis = (Arguments[chart.MinColumns + 4] ?? string.Empty)?.Replace("'", "");
            }

        }
    }
}
