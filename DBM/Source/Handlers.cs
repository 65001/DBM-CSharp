// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.Diagnostics;
using System.Collections.Generic;
using LitDev;
using Microsoft.SmallBasic.Library;

namespace DBM
{
    public static class Handlers
    {
        static IReadOnlyList<string> CorrectList;
        static List<string> Tracker = new List<string>();
        public static Primitive TypeofSorts = "1=" + Utilities.Localization["Table"] + ";2=" + Utilities.Localization["View"] + ";3=" + Utilities.Localization["Index"] + ";4=" + Utilities.Localization["Master Table"] + ";";

        /// <summary>
        /// Handles Main Menu
        /// </summary>
        /// <param name="Item">Localized Menu Item</param>
        public static void Menu(string Item)
        {
            int StackPointer = Stack.Add($"Handlers.Menu({Item})");

            //Switch and Enum cannot be used because values can change
            //File Menu Items
            if (Item == Utilities.Localization["New"])
            {
                string Path = LDDialogs.SaveFile(GlobalStatic.Extensions, GlobalStatic.LastFolder);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    GlobalStatic.ListView = null;
                    GlobalStatic.Dataview = null;
                    GlobalStatic.LastFolder = System.IO.Path.GetDirectoryName(Path);
                    Settings.Load(GlobalStatic.RestoreSettings, GlobalStatic.SettingsPath);
                    Settings.Save();
                    LDDataBase.ConnectSQLite(Path);
                    Engines.Load.Sqlite(Path);

                    Events.LogMessage("Created DB :" + Path, Utilities.Localization["Application"]);
                    UI.PreMainMenu();
                    UI.MainMenu();
                    LDDataBase.Connection = null;
                }
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["Open"])
            {
                GlobalStatic.ListView = null;
                GlobalStatic.Dataview = null;
                Settings.Load(GlobalStatic.RestoreSettings, GlobalStatic.SettingsPath); //Reloads Settings
                string Path = UI.GetPath(Engines.EnginesMode.SQLITE);

                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Engines.Load.Sqlite(Path);
                    GlobalStatic.Settings["LastFolder"] = System.IO.Path.GetDirectoryName(Path);
                    Settings.Save();
                    UI.PreMainMenu();
                    UI.MainMenu();

                    int Index = Engines.DB_Name.IndexOf(Engines.CurrentDatabase) + 1;
                    Handlers.ComboBox.CB(GlobalStatic.ComboBox["Database"], Index);
                    LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Database"], Index);
                }
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["Define New Table"])
            {
                Engines.InvalidateCache();
                UI.CreateTableUI();
            }
            else if (Item == Utilities.Localization["New in Memory Db"])
            {
                Engines.Load.MemoryDB(Engines.EnginesMode.SQLITE);
                UI.PreMainMenu();
                UI.MainMenu();
            }
            else if (Item == Utilities.Localization["Create Statistics Page"])
            {
                string Name = string.Format("\"Statistics of {0}\"", Engines.CurrentTable.SanitizeFieldName());
                Engines.Transform.CreateStatisticsTable(Engines.CurrentDatabase, Engines.CurrentTable, Name, Export.GenerateSchemaFromLastQuery());
                Engines.Query(Engines.CurrentDatabase, $"SELECT * FROM {Name};", GlobalStatic.ListView, false, GlobalStatic.UserName, Utilities.Localization["Statistics Page"]);

                Engines.GetSchema(Engines.CurrentDatabase);
                Engines.SetDefaultTable(Name.SanitizeFieldName());
                Engines.GetColumnsofTable(Engines.CurrentDatabase, Name);
                ComboBox.Bind();
            }
            //Main
            else if (Item == Utilities.Localization["View"] || Item == Utilities.Localization["View"] + " ")
            {
                Controls.HideControl(GlobalStatic.Dataview);
                GlobalStatic.Dataview = null;
                if (GlobalStatic.ListView == null)
                {
                    bool Bold = GraphicsWindow.FontBold;
                    GraphicsWindow.FontBold = false;
                    GlobalStatic.ListView = LDControls.AddListView(GlobalStatic.Listview_Width, GlobalStatic.Listview_Height, null);

                    LDControls.AddContextMenu(GlobalStatic.ListView, "1=Ascend;2=Descend;3=Random();", null);
                    Controls.Move(GlobalStatic.ListView, 10, 35);
                    UI.DisplayResults();
                    GraphicsWindow.FontBold = Bold;
                }
                else
                {
                    UI.ShowDisplayResults();
                    Controls.ShowControl(GlobalStatic.ListView);
                }
                if (!string.IsNullOrEmpty(Engines.CurrentTable))
                {
                    Engines.Query(Engines.CurrentDatabase, "SELECT * FROM " + Engines.CurrentTable + ";", GlobalStatic.ListView, false, Utilities.Localization["App"], Utilities.Localization["View Function"]);
                }
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["Save"])
            {
                if (!string.IsNullOrEmpty(Engines.CurrentDatabase) && !string.IsNullOrEmpty(GlobalStatic.Dataview))
                {
                    string SaveStatus = LDDataBase.SaveTable(Engines.CurrentDatabase, GlobalStatic.Dataview);
                    Events.LogMessagePopUp("The save was : " + SaveStatus, Utilities.Localization["UI"], "Save Status");
                }
                else
                {
                    Events.LogMessagePopUp(Utilities.Localization["Dataview Error"], Utilities.Localization["UI"], "Save Error");
                }
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["Edit"])
            {
                if (!string.IsNullOrEmpty(Engines.CurrentDatabase))
                {
                    switch (GlobalStatic.SortBy)
                    {
                        case 2:
                            Events.LogMessagePopUp(Utilities.Localization["Views Read Only"], Utilities.Localization["UI"], Utilities.Localization["Access Denied"]);
                            break;
                        case 4:
                            Events.LogMessagePopUp(Utilities.Localization["Master Table Protected"], Utilities.Localization["UI"], Utilities.Localization["Access Denied"]);
                            break;
                        default:
                            Controls.HideControl(GlobalStatic.ListView);
                            if (String.IsNullOrEmpty(GlobalStatic.Dataview))
                            {
                                GlobalStatic.Dataview = LDControls.AddDataView(GlobalStatic.Listview_Width, GlobalStatic.Listview_Height, null);
                                Controls.Move(GlobalStatic.Dataview, 10, 35);
                            }
                            else
                            {
                                Controls.ShowControl(GlobalStatic.Dataview);
                            }
                            UI.HideDisplayResults();
                            Engines.EditTable(Engines.CurrentTable, GlobalStatic.Dataview);
                            break;
                    }
                }
                else
                {
                    Events.LogMessagePopUp(Utilities.Localization["Error No DB"], Utilities.Localization["UI"], Utilities.Localization["Edit"]);
                }
                Stack.Exit(StackPointer);
                return;
            }
            //Import
            else if (Item == Utilities.Localization["CSV"])
            {
                string Path = LDDialogs.OpenFile("csv", null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    //Outputted to a temporary path in the event the import doesn't work...
                    string TempPath = System.IO.Path.GetTempFileName().Replace(".tmp", ".sql");
                    Stopwatch CSV = new Stopwatch();
                    CSV.Start();
                    Import.CSV(Path, TempPath);
                    CSV.Stop();

                    Stopwatch SQL = new Stopwatch();
                    SQL.Start();
                    Import.SQL(Engines.CurrentDatabase, TempPath);
                    SQL.Stop();

                    string ToolTip = string.Format("CSV Import Completed!\n CSV:{0}(ms)\nSQL:{1}(ms)",
                        CSV.ElapsedMilliseconds,
                        SQL.ElapsedMilliseconds);
                    Events.LogMessagePopUp(ToolTip, Utilities.Localization["UI"], Utilities.Localization["Importer"]); //TODO Localize

                    Stack.Exit(StackPointer);
                    return;
                }
                Events.LogMessagePopUp(Utilities.Localization["Error Generic"], Utilities.Localization["UI"], "Import.CSV");//TODO Localize
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["SQL"])
            {
                string Path = LDDialogs.OpenFile("sql", null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Stopwatch SQL = new Stopwatch();
                    SQL.Start();
                    Import.SQL(Engines.CurrentDatabase, Path);
                    SQL.Stop();
                    Events.LogMessagePopUp("SQL Import Completed in " + SQL.ElapsedMilliseconds + "(ms)", Utilities.Localization["UI"], Utilities.Localization["Importer"]); //TODO Localize
                    Stack.Exit(StackPointer);
                    return;
                }
                Events.LogMessagePopUp(Utilities.Localization["Error Generic"], Utilities.Localization["UI"], "Import.SQL");//TODO Localize
                Stack.Exit(StackPointer);
                return;
            }
            //Export
            else if (Item == Utilities.Localization["PXML"] + " ")
            {
                string Path = LDDialogs.SaveFile("xml", null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Primitive Data = Export.Generate2DArrayFromLastQuery();
                    Export.XML(Data, Export.GenerateSchemaFromQueryData(Data), Engines.CurrentTable, Path);
                    Events.LogMessagePopUp("XML export of " + Engines.CurrentTable + " completed!", "Export", "Success");
                    Stack.Exit(StackPointer);
                    return;
                }
                Events.LogMessagePopUp(Utilities.Localization["Error Generic"], Utilities.Localization["UI"], "Export.XML");//TODO Localize
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["HTML"] + " ")
            {
                string Path = LDDialogs.SaveFile("html", null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Primitive Data = Export.Generate2DArrayFromLastQuery();
                    Export.HTML(Data, Export.GenerateSchemaFromQueryData(Data), Engines.CurrentTable.SanitizeFieldName(), Path, GlobalStatic.ProductID + " V" + GlobalStatic.VersionID);
                    Events.LogMessagePopUp("HTML export of " + Engines.CurrentTable + " completed!", Utilities.Localization["Export"], Utilities.Localization["Success"]);
                    Stack.Exit(StackPointer);
                    return;
                }
                Events.LogMessagePopUp(Utilities.Localization["Error Generic"], Utilities.Localization["UI"], "Export.HTML");//TODO Localize
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["SQL"] + " ")
            {
                string Path = LDDialogs.SaveFile("sql", null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Primitive Data = Export.Generate2DArrayFromLastQuery();
                    Primitive Schema = Export.GenerateSchemaFromQueryData(Data);
                    Primitive SchemaQuery;
                    switch (Engines.CurrentEngine)
                    {
                        case Engines.EnginesMode.SQLITE:
                            SchemaQuery = Engines.Query(Engines.CurrentDatabase, "PRAGMA table_info(" + Engines.CurrentTable.SanitizeFieldName() + ");", null, true, GlobalStatic.UserName, "SCHEMA");
                            break;
                        default:
                            throw new PlatformNotSupportedException("Currently database is not supported");
                    }
                    Dictionary<string, bool> PK = Export.SQL_Fetch_PK(SchemaQuery, Schema, Engines.CurrentEngine);
                    Dictionary<string, string> Types = Export.SQL_Fetch_Type(SchemaQuery, Schema, Engines.CurrentEngine);
                    Export.SQL(Data, Schema, PK, Types, Engines.CurrentTable, Path);
                    LDProcess.Start(Path, null);
                    Events.LogMessagePopUp("SQL export of " + Engines.CurrentTable + " completed!", Utilities.Localization["Export"], Utilities.Localization["Success"]); //TODO Localize
                    Stack.Exit(StackPointer);
                    return;
                }
                Events.LogMessagePopUp(Utilities.Localization["Error Generic"], Utilities.Localization["UI"], "Export.SQL");//TODO Localize
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["CSV"] + " ")
            {
                string Path = LDDialogs.SaveFile("csv", null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Primitive Data = Export.Generate2DArrayFromLastQuery();
                    Export.CSV(Data, Export.GenerateSchemaFromQueryData(Data), Path, GlobalStatic.Deliminator);
                    Events.LogMessagePopUp("CSV export of " + Engines.CurrentTable + " completed!", Utilities.Localization["Export"], Utilities.Localization["Success"]); //TODO Localize
                    Stack.Exit(StackPointer);
                    return;
                }
                Events.LogMessagePopUp(Utilities.Localization["Error Generic"], Utilities.Localization["UI"], "Export.CSV");//TODO Localize
                Stack.Exit(StackPointer);
            }
            else if (Item == "JSON") //TODO Localize
            {
                string Path = LDDialogs.SaveFile("json", null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Primitive Data = Export.Generate2DArrayFromLastQuery();
                    Export.JSON(Data, Export.GenerateSchemaFromQueryData(Data), Engines.CurrentTable.SanitizeFieldName(), Path);
                    Events.LogMessagePopUp("JSON export of " + Engines.CurrentTable + " completed!", Utilities.Localization["Export"], Utilities.Localization["Success"]); //TODO Localize
                    Stack.Exit(StackPointer);
                    return;
                }
                Events.LogMessagePopUp(Utilities.Localization["Error Generic"], Utilities.Localization["UI"], "Export.JSON");//TODO Localize
                Stack.Exit(StackPointer);
            }
            else if (Item == "MarkDown") //TODO Localize
            {
                string Path = LDDialogs.SaveFile("md", null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Primitive Data = Export.Generate2DArrayFromLastQuery();
                    Export.MarkDown(Data, Export.GenerateSchemaFromQueryData(Data), Path);
                    Events.LogMessagePopUp("MarkDown export is now complete", Utilities.Localization["Export"], Utilities.Localization["Success"]); //TODO Localize
                    Stack.Exit(StackPointer);
                    return;
                }
                Events.LogMessagePopUp(Utilities.Localization["Error Generic"], Utilities.Localization["UI"], "Export.MarkDown");//TODO Localize
                Stack.Exit(StackPointer);
            }
            else if (Item == "Wiki MarkUp") //TODO Localize
            {
                string Path = LDDialogs.SaveFile("markup", null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Primitive Data = Export.Generate2DArrayFromLastQuery();
                    Export.MarkUp(Data, Export.GenerateSchemaFromQueryData(Data), Path);
                    Events.LogMessagePopUp("Wiki Markup export is now complete", Utilities.Localization["Export"], Utilities.Localization["Success"]); //TODO Localize
                    Stack.Exit(StackPointer);
                    return;
                }
                Events.LogMessagePopUp(Utilities.Localization["Error Generic"], Utilities.Localization["UI"], "Export.Wiki Markup");//TODO Localize
                Stack.Exit(StackPointer);
            }
            //Settings
            else if (Item == Utilities.Localization["About"])
            {
                Primitive About_Data = Engines.Query(Engines.CurrentDatabase, "SELECT SQLITE_VERSION(),sqlite_source_id();", null, true, GlobalStatic.UserName, Utilities.Localization["User Requested"] + ":" + Utilities.Localization["App"]);
                string About_Msg = string.Format("DBM C# is a Database Mangement Program developed by Abhishek Sathiabalan. (C){0}. All rights reserved.\n\nYou are running : {1} v{2}\n\n", GlobalStatic.Copyright, GlobalStatic.ProductID, GlobalStatic.VersionID);
                About_Msg += string.Format("SQLite Version : {0}\nSQLITE Source ID : {1}", About_Data[1]["SQLITE_VERSION()"], About_Data[1]["sqlite_source_id()"]);
                Events.LogMessagePopUp(About_Msg, "Debug", "About");//DO NOT LOCALIZE
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["Show Help"])
            {
                string Path = System.IO.Path.Combine(GlobalStatic.AssetPath, "HELP Table.html");
                LDProcess.Start(Path, null);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["Settings Editor"])
            {
                UI.Settings.Display();
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["Refresh Schema"])
            {
                Engines.GetSchema(Engines.CurrentDatabase);
                Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == Utilities.Localization["Check for Updates"])
            {
                Utilities.Updater.CheckForUpdates(GlobalStatic.UpdaterDBpath);
                Stack.Exit(StackPointer);
                return;
            }
            //Charts
            else if (Item == "Bar") //TODO Localize 
            {
                Google_Charts.Chart.Bar Bar = new Google_Charts.Chart.Bar();
                UI.Charts.Display(Bar);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == "Column") //TODO Localize 
            {
                Google_Charts.Chart.Column Column = new Google_Charts.Chart.Column();
                UI.Charts.Display(Column);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == "Geo") //TODO Localize
            {
                Google_Charts.Chart.GeoCharts Geo = new Google_Charts.Chart.GeoCharts();
                UI.Charts.Display(Geo);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == "Histogram") //TODO Localize 
            {
                Google_Charts.Chart.Histograms Histo = new Google_Charts.Chart.Histograms();
                UI.Charts.Display(Histo);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == "Line") //TODO Localize
            {
                Google_Charts.Chart.Line Line = new Google_Charts.Chart.Line();
                UI.Charts.Display(Line);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == "Org") //TODO Localize
            {
                Google_Charts.Chart.OrgChart Org = new Google_Charts.Chart.OrgChart();
                UI.Charts.Display(Org);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == "Pie") //TODO: Localize
            {
                Google_Charts.Chart.Pie Pie = new Google_Charts.Chart.Pie();
                UI.Charts.Display(Pie);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == "Sankey") //TODO Localize
            {
                Google_Charts.Chart.SanKey SanKey = new Google_Charts.Chart.SanKey();
                UI.Charts.Display(SanKey);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == "Scatter Plot") //TODO Localize
            {
                Google_Charts.Chart.Scatter Scatter = new Google_Charts.Chart.Scatter();
                UI.Charts.Display(Scatter);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == "Sortable Table") //TODO Localize
            {
                Google_Charts.Chart.Table Table = new Google_Charts.Chart.Table();
                UI.Charts.Display(Table);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item == "TimeLine") //TODO Localize
            {
                Google_Charts.Chart.TimeLine TL = new Google_Charts.Chart.TimeLine();
                UI.Charts.Display(TL);
                Stack.Exit(StackPointer);
                return;
            }
            else if (Item != null)
            {
                Events.LogMessage(Item + " does not exist in context or is not yet implemented", Utilities.Localization["UI"]);
            }
            Stack.Exit(StackPointer);
        }

        public static void ContextMenu(string Control, int Index)
        {
            int StackPointer = Stack.Add($"Handlers.ContextMenu({Control},{Index})");
            if (Control == GlobalStatic.ListView)
            {
                if (Index <= 3)
                {
                    Primitive Schema = Export.GenerateSchemaFromLastQuery();
                    LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Sort"], LDControls.LastListViewColumn);
                    LDControls.ComboBoxSelect(GlobalStatic.ComboBox["ASCDESC"], Index);
                    Buttons(UI.Buttons["Sort"]);
                }
            }
            Stack.Exit(StackPointer);
        }

        public static void Buttons(string LastButton)
        {
            int StackPointer = Stack.Add($"Handlers.Buttons({LastButton})");
            try
            {
                if (LastButton == UI.Buttons["Search"] || LastButton == UI.Buttons["Sort"] || LastButton == UI.Buttons["RunFunction"])
                {
                    Primitive ASCDESC_Sorts = "1=ASC;2=DESC;3=RANDOM();";
                    bool Search = false, Sort = true, Function = false;

                    bool.TryParse(LDControls.CheckBoxGetState(GlobalStatic.CheckBox["StrictSearch"]), out bool StrictSearch);
                    bool.TryParse(LDControls.CheckBoxGetState(GlobalStatic.CheckBox["InvertSearch"]), out bool InvertSearch);

                    string SearchIn = "\"" + Engines.Schema[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["Search"])] + "\"";
                    string SearchText = Controls.GetTextBoxText(UI.TextBox["Search"]).ToString().Replace("'", "''");
                    string FunctionIn = "\"" + Engines.Schema[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["ColumnList"])] + "\"";
                    string FunctionCalled = Engines.Functions(Engines.CurrentEngine)[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["FunctionList"]) - 1];

                    string SortBy = "\"" + Engines.Schema[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["Sort"])] + "\"";
                    string ASCDESC = ASCDESC_Sorts[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["ASCDESC"])];

                    if (LastButton == UI.Buttons["Search"])
                    {
                        Search = true;
                    }
                    else if (LastButton == UI.Buttons["RunFunction"])
                    {
                        Function = true;
                    }
                    /*
                    Console.WriteLine();
                    Console.WriteLine("Search: {0} Sort : {1} Function :{2}", Search, Sort, Function);
                    Console.WriteLine("Strict Search : {0} Invert Search : {1}", StrictSearch, InvertSearch);
                    Console.WriteLine("SearchIn : {0} Search Text : {1} FunctionIn : {2} FunctionCalled : {3} SortBy : {4} ASCDESC : {5} LastButton : {6}", SearchIn, SearchText, FunctionIn, FunctionCalled, SortBy, ASCDESC, Controls.GetButtonCaption(LastButton));
                    */
                    var GQS = new Engines.GenerateQuerySettings
                    {
                        //bool
                        Search = Search,
                        Sort = Sort,
                        RunFunction = Function,

                        SearchBy = SearchIn,
                        OrderBy = SortBy,

                        StrictSearch = StrictSearch,
                        InvertSearch = InvertSearch,
                        FunctionSelected = FunctionCalled,
                        FunctionColumn = FunctionIn,
                        SearchText = SearchText
                    };
                    switch (ASCDESC)
                    {
                        case "ASC":
                            GQS.Order = Engines.GenerateQuerySettings.SortOrder.Ascending;
                            break;
                        case "DESC":
                            GQS.Order = Engines.GenerateQuerySettings.SortOrder.Descding;
                            break;
                        case "RANDOM()":
                            GQS.Order = Engines.GenerateQuerySettings.SortOrder.Random;
                            break;
                    }
                    string Query = Engines.GenerateQuery(GQS, Engines.CurrentTable.SanitizeFieldName());
                    Engines.Query(Engines.CurrentDatabase, Query, GlobalStatic.ListView, false, GlobalStatic.UserName, "Auto Generated Query on behalf of " + GlobalStatic.UserName);

                }
                else if (LastButton == UI.Buttons["Query"])
                {
                    Engines.InvalidateCache();
                    Engines.Query(Engines.CurrentDatabase, Controls.GetTextBoxText(UI.TextBox["CustomQuery"]), GlobalStatic.ListView, false, GlobalStatic.UserName, Utilities.Localization["User Requested"]);
                }
                else if (LastButton == UI.Buttons["Command"]) //Custom Command
                {
                    Engines.InvalidateCache();
                    Engines.Command(Engines.CurrentDatabase, Controls.GetTextBoxText(UI.TextBox["CustomQuery"]), GlobalStatic.UserName, Utilities.Localization["User Requested"]);
                }
            }
            catch (KeyNotFoundException)
            {
                string Message = Controls.GetButtonCaption(LastButton) + "(" + LastButton + ") |" + UI.Buttons.ContainsKey(LastButton) + "|" + Controls.GetButtonCaption(LastButton) == Utilities.Localization["Query"].ToUpper() + "| does not exist in context or has not yet implemented.";
                Events.LogMessage(Message, "System");
            }
            Stack.Exit(StackPointer);
        }

        public static class ComboBox
        {
            static Dictionary<int, IReadOnlyList<string>> Schema = new Dictionary<int, IReadOnlyList<string>>
            {
                { 1, Engines.Tables },
                { 2, Engines.Views },
                { 3, Engines.Indexes },
                { 4, new List<string>() {"sqlite_master"}.AsReadOnly() }
            };

            public static void CB(string ComboBox, int Index)
            {
                int StackPointer = Stack.Add($"Handlers.ComboBox({ComboBox},{Index})");
                try
                {
                    if (ComboBox == GlobalStatic.ComboBox["Table"])
                    {
                        Table(Index);
                        Stack.Exit(StackPointer);
                        return;
                    }
                    else if (ComboBox == GlobalStatic.ComboBox["Sorts"])
                    {
                        Sorts(Index);
                        Stack.Exit(StackPointer);
                        return;
                    }
                    else if (ComboBox == GlobalStatic.ComboBox["Database"])
                    {
                        Database(Index);
                        Stack.Exit(StackPointer);
                        return;
                    }
                }
                catch (KeyNotFoundException)
                {
                    string Message = string.Format("{0} at {1} does not exist in context or has not yet implemented.", ComboBox, Index);
                    Events.LogMessage(Message, "System");
                }
                Stack.Exit(StackPointer);
            }

            static void Database(int Index)
            {
                Tracker.Add(Engines.DB_ShortName[Index - 1]);

                Engines.Load.Sqlite(Engines.DB_Path[Index - 1]);

                Engines.GetSchema(Engines.CurrentDatabase);
                Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);

                LDControls.ComboBoxContent(GlobalStatic.ComboBox["FunctionList"], Engines.Functions(Engines.EnginesMode.SQLITE).ToPrimitiveArray());

                Sorts(1);
                Table(1);
                LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Sorts"], 1);
                LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Table"], 1);

                if (GlobalStatic.SortBy == 4)
                {
                    Engines.SetDefaultTable("sqlite_master");
                    Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
                    LDControls.ComboBoxContent(GlobalStatic.ComboBox["Table"], "1=" + Engines.CurrentTable + ";2=sqlite_temp_master;");
                    return;
                }

                Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
                return;
            }

            public static void Bind()
            {
                LDControls.ComboBoxContent(GlobalStatic.ComboBox["Sort"], Engines.Schema);
                LDControls.ComboBoxContent(GlobalStatic.ComboBox["ColumnList"], Engines.Schema);
                LDControls.ComboBoxContent(GlobalStatic.ComboBox["Search"], Engines.Schema);
                UI.Title();
                Menu(Utilities.Localization["View"]); //Tasks
            }

            static void Table(int Index)
            {
                //Prevents OutofBound Exceptions
                if (CorrectList.Count >= Index)
                {
                    Engines.SetDefaultTable(CorrectList[Index - 1]);
                    Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
                    Bind();
                }
            }

            static void Sorts(int Index)
            {
                GlobalStatic.SortBy = Index; //Sets GlobalStatic.SortBy. Count by 1 instead of zero

                if (Schema.ContainsKey(Index))
                {
                    CorrectList = Schema[Index];
                    if (CorrectList.Count > 0)
                    {
                        Engines.SetDefaultTable(CorrectList[0]);
                        LDControls.ComboBoxContent(GlobalStatic.ComboBox["Table"], CorrectList.ToPrimitiveArray());
                        Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
                    }
                }

                if (!string.IsNullOrWhiteSpace(Engines.CurrentTable))
                {
                    UI.HideDisplayResults();
                    Bind();
                    UI.Title();
                    return;
                }
                Events.LogMessage("In the current database no " + Utilities.Localization[TypeofSorts[GlobalStatic.SortBy]] + "s can be found.", Utilities.Localization["UI"]);
            }
        }
	}
}