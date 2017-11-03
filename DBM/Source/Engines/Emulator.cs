using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.SmallBasic.Library;

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
                            { ".functions","List of all custom functions."}
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
                                Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('{1}','{2}','File Folder',NULL);", EmulatorTable, Directory, DI.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            }

                            for (int i = 0; i < FileList.Count; i++)
                            {
                                string File = FileList[i];
                                FileInfo FI = new FileInfo(File);

                                Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('{1}','{2}','{3}','{4}');", EmulatorTable, File, FI.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"), FI.Extension, FI.Length);
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

                    }
                    else if (SQL.StartsWith(".bar", Comparison))
                    {
                        //Column(s)?
                        //Values

                        //Title
                        //SubTitle
                        //Haxis Title
                        //Yaxis Title
                        Chart chart = new Chart.Bar();
                        SQL = SQL.Remove(0, 5);
                        string[] Arguments = SQL.Split(',');
                        string SQLQuery = string.Format("SELECT {0},{1} FROM {2} GROUP BY {0};", Arguments[0].Replace("(", "").Replace(")", ""), Arguments[1], CurrentTable.SanitizeFieldName());
                        Primitive Data = Query(CurrentDatabase, 
                            SQLQuery,
                            null,
                            true,
                            Username,
                            "Chart"
                            );
                        Primitive Columns = Data[1].GetAllIndices();
                        //Console.WriteLine((string)Arguments.ToPrimitiveArray());
                        //Console.WriteLine((string)Columns);
                        //Console.WriteLine((string)Data);

                        chart.AddColumn(Columns[1]);
                        chart.AddColumn(Columns[2],Chart.DataType.number);

                        for (int i = 1; i <= Data.GetItemCount(); i++)
                        {
                            for (int ii = 1; ii <= Data[i].GetItemCount(); ii++) {
                                chart.AddRowData(i - 1, Data[i][Columns[ii]]);
                            }
                        }

                        chart.Title = (Arguments[2] ?? Engines.CurrentTable).Replace("'","");
                        chart.SubTitle = (Arguments[3] ?? string.Empty).Replace("'","");
                        chart.Xaxis = (Arguments[4] ?? string.Empty).Replace("'", "");
                        chart.Yaxis = (Arguments[5] ?? string.Empty).Replace("'", "");

                        Console.WriteLine(chart.Export());
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

                Engines.GetSchema(Database);
                Engines.SetDefaultTable(EmulatorTable);
                Engines.GetColumnsofTable(Database, EmulatorTable);
            }

        }
    }
}
