﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitDev;

namespace DBM
{
    public static partial class Engines
    {
        public static void Emulator(EnginesMode Mode, string Database, string SQL, string Username, string Listview) //TODO Implement Emulator atleast for sqlite for DBM
        {
            //Attempts to emulate some if not all commands of a database engine by aliasing it to SQL
            int StackPointer = Stack.Add($"Engines.Emulator({Mode},{Database},{SQL}");
            StringBuilder Emulator_Sql = new StringBuilder();
            string EmulatorTable = null;

            StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase;

            switch (Mode)
            {
                case EnginesMode.SQLITE:
                    if (SQL.StartsWith(".help", Comparison))
                    {
                        
                        //EmulatorTable = "DBM_SQLITE_Help";
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
                            var QS = new QuerySettings
                            {
                                Database = CurrentDatabase,
                                SQL = $"PRAGMA {PRAGMA[i]};",
                                ListView = null,
                                FetchRecords = true,
                                User = Username,
                                Explanation = "Emulator"
                            };

                            Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES ('{1}','{2}');", 
                                EmulatorTable, 
                                PRAGMA[i].Replace("_", " "), 
                                Query(QS)[1][PRAGMA[i]]);
                            
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
                            Events.LogMessage(ex.Message, Language.Localization["System"]);
                        }
                    }
                    else if (SQL.StartsWith(".querytimes", Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_QueryTimes";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,SQL TEXT,\"Execution Time (ms)\" INTEGER,Explanation TEXT,User TEXT,\"Start Time (UTC)\" TEXT,Cache TEXT);", EmulatorTable);

                        int ii = 0;
                        for (int i = 0; i < _Type_Referer.Count; i++)
                        {
                            if (_Type_Referer[i] == Type.Query)
                            {
                                Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES('{1}','{2}','{3}','{4}','{5}','{6}','{7}');", EmulatorTable, ii, LastQuery[ii].Replace("'", "''"), _Timer[i], _Explanation[i], _User[i], _UTC_Start[i],_CacheStatus[ii]);
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
                        //"hh:mm:ss ffffff"
                        EmulatorTable = "DBM_SQLITE_StackTrace";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,Item TEXT,\"Start Time (UTC)\" TEXT,\"Exit Time (UTC)\" TEXT,\"Duration (ms)\" TEXT);", EmulatorTable);
                        for (int i = 0; i < Stack.StackEntries.Count; i++)
                        {
                            var SE = Stack.StackEntries[i];
                            Emulator_Sql.AppendFormat("INSERT INTO {0} VALUES('{1}','{2}','{3}','{4}','{5}');", EmulatorTable, i, 
                                SE.Trace,
                                SE.StartTime.ToString("hh:mm:ss ffffff"),
                                SE.ExitTime.ToString("hh:mm:ss ffffff"),
                                SE.Duration.TotalMilliseconds
                                );
                        }
                    }
                    else if (SQL.StartsWith(".localization", Comparison))
                    {
                        EmulatorTable = "DBM_SQLITE_Localizations";
                        Emulator_Sql.AppendFormat("CREATE TEMP TABLE {0} (ID INTEGER PRIMARY KEY,KEY TEXT,VALUE TEXT);", EmulatorTable);
                        foreach (KeyValuePair<string, string> entry in Language.Localization)
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
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (EmulatorTable != null)
            {
                string _SQL = Emulator_Sql.ToString();
                var CS = new Engines.CommandSettings()
                {
                    Database = Database,
                    SQL = $"DROP TABLE IF EXISTS {EmulatorTable};" + _SQL,
                    User = GlobalStatic.UserName,
                    Explanation = Language.Localization["User Requested"] + ": CLI EMULATOR"
                };
                Command(CS);

                QuerySettings QS = new QuerySettings
                {
                    Database = Database,
                    SQL = $"SELECT * FROM {EmulatorTable};",
                    ListView = Listview,
                    FetchRecords = false,
                    User = Username,
                    Explanation = Language.Localization["User Requested"] + ": CLI EMULATOR"
                };
                Query(QS);

                GetSchema(Database);
                SetDefaultTable(EmulatorTable);
                GetColumnsofTable(Database, EmulatorTable);
            }
            Stack.Exit(StackPointer);
        }
    }
}
