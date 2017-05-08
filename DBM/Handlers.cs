// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LitDev;
using Microsoft.SmallBasic.Library;
namespace DBM
{
	public static class Handlers
	{
		public static Primitive CurrentSchema;
        static IReadOnlyList<string> CorrectList;
		public static Primitive TypeofSorts ="1="+ Utilities.Localization["Table"] +";2=" + Utilities.Localization["View"] +"3="+ Utilities.Localization["Index"] + "4="+ Utilities.Localization["Master Table"]+";";

		public static void Menu(string Item) //Handles Main Menu
		{
            Utilities.AddtoStackTrace( "Handlers.Menu(" + Item + ")");

            //Switch and Enum cannot be used because values can change
            //File Menu Items
            if (Item == Utilities.Localization["New"])
            {
                string Path = LDDialogs.SaveFile(GlobalStatic.Extensions, GlobalStatic.LastFolder);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    GlobalStatic.ListView = null;
                    GlobalStatic.Dataview = null;
                    GlobalStatic.LastFolder = LDFile.GetFolder(Path);
                    Settings.LoadSettings(GlobalStatic.RestoreSettings);
                    Settings.SaveSettings();
                    LDDataBase.ConnectSQLite(Path);
                    Engines.Load_DB(Engines.EnginesModes.SQLITE, Path);

                    Events.LogMessage("Created DB :" + Path, Utilities.Localization["Application"]);
                    UI.PreMainMenu();
                    UI.MainMenu();
                    LDDataBase.Connection = null;
                }
                return;
            }
            else if (Item == Utilities.Localization["Open"])
            {
                GlobalStatic.ListView = null;
                GlobalStatic.Dataview = null;
                Settings.LoadSettings(GlobalStatic.RestoreSettings); //Reloads Settings
                Engines.Load_DB(Engines.EnginesModes.SQLITE, UI.GetPath(Engines.EnginesModes.SQLITE));
                Settings.SaveSettings();
                UI.PreMainMenu();
                UI.MainMenu();


                int Index = Engines.DB_Name.IndexOf(Engines.CurrentDatabase)+ 1;
                Handlers.ComboBox(GlobalStatic.ComboBox["Database"], Index);
                LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Database"], Index);
                return;
            }
            else if (Item == Utilities.Localization["Define New Table"]) //TODO ADD UI
            {
                UI.CreateTableUI();
            }
            //Main
            else if (Item == Utilities.Localization["View"] || Item == Utilities.Localization["View"] + " ")
            {
                Controls.HideControl(GlobalStatic.Dataview);
                if (GlobalStatic.ListView == null)
                {
                    bool Bold = GraphicsWindow.FontBold;
                    GraphicsWindow.FontBold = false;
                    GlobalStatic.ListView = LDControls.AddListView(GlobalStatic.Listview_Width, GlobalStatic.Listview_Height, null);

                    LDControls.AddContextMenu(GlobalStatic.ListView, "1=Ascend;2=Descend;", null);
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
                return;
            }
            else if (Item == Utilities.Localization["Save"])
            {
                if (!string.IsNullOrEmpty(Engines.CurrentDatabase) && !string.IsNullOrEmpty(GlobalStatic.Dataview))
                {
                    string SaveStatus = LDDataBase.SaveTable(Engines.CurrentDatabase, GlobalStatic.Dataview);
                    Events.LogMessage("The save was : " + SaveStatus, Utilities.Localization["UI"]);
                    GraphicsWindow.ShowMessage("The save was : " + SaveStatus, "Save Status");
                }
                else
                {
                    Events.LogMessage(Utilities.Localization["Dataview Error"], Utilities.Localization["UI"]);
                    GraphicsWindow.ShowMessage(Utilities.Localization["Error"] + ":" + Utilities.Localization["Dataview Error"], "Save Error");
                }
                return;
            }
            else if (Item == Utilities.Localization["Edit"])
            {
                if (!string.IsNullOrEmpty(Engines.CurrentDatabase))
                {
                    switch (GlobalStatic.SortBy)
                    {
                        case 2:
                            Events.LogMessage(Utilities.Localization["Views Read Only"], Utilities.Localization["UI"]);
                            GraphicsWindow.ShowMessage(Utilities.Localization["Error"] + ":" + Utilities.Localization["Views Read Only"], Utilities.Localization["Access Denied"]);
                            break;
                        case 4:
                            Events.LogMessage(Utilities.Localization["Master Table Protected"], Utilities.Localization["UI"]);
                            GraphicsWindow.ShowMessage(Utilities.Localization["Error"] + ":" + Utilities.Localization["Master Table Protected"], Utilities.Localization["Access Denied"]);
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
                    Events.LogMessage(Utilities.Localization["Error No DB"], Utilities.Localization["UI"]);
                    GraphicsWindow.ShowMessage(Utilities.Localization["Error No DB"], Utilities.Localization["UI"]);
                }
                return;
            }
            //Import
            else if (Item == Utilities.Localization["CSV"])
            {
                Engines.Command(Engines.CurrentDatabase, Import.CSV(LDDialogs.OpenFile("csv", null)), GlobalStatic.UserName, "", false);
            }
            else if (Item == Utilities.Localization["SQL"])
            {
                string SQL = System.IO.File.ReadAllText(LDDialogs.OpenFile("sql", null));
                Engines.Command(Engines.CurrentDatabase, SQL, GlobalStatic.UserName, null, false);
            }
            else if (Item == Utilities.Localization["HTML to CSV"]) //Plugin //TODO
            { }
            //Export
            else if (Item == Utilities.Localization["PXML"] + " ") //TODO XML
            {
                string Path = LDDialogs.SaveFile("xml", null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Primitive Data = Export.Generate2DArrayFromLastQuery();
                    //Export.XML(Data,Export.GenerateSchemaFromQueryData(Data),Path);
                    GraphicsWindow.ShowMessage("Export Completed!", "Success");//TODO Localize
                    return;
                }
                GraphicsWindow.ShowMessage("Oh no something went wrong :(", "Error"); //TODO Localize
            }
            else if (Item == Utilities.Localization["HTML"] + " ")
            {
                string Path = LDDialogs.SaveFile("html",null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Primitive Data = Export.Generate2DArrayFromLastQuery();
                    Export.HTML(Data, Export.GenerateSchemaFromQueryData(Data), Engines.CurrentTable.Replace("\"", ""), Path, GlobalStatic.ProductID + " V" + GlobalStatic.VersionID);
                    GraphicsWindow.ShowMessage("Export Completed!", "Success");//TODO Localize
                    return;
                }
                GraphicsWindow.ShowMessage("Oh no something went wrong :(", "Error");
            }
            else if (Item == Utilities.Localization["SQL"] + " ")
            {
                string Path = LDDialogs.SaveFile("sql",null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Primitive Data = Export.Generate2DArrayFromLastQuery();
                    Primitive Schema = Export.GenerateSchemaFromQueryData(Data);
                    Primitive SchemaQuery;
                    switch (Engines.CurrentEngine)
                    {
                        case Engines.EnginesModes.SQLITE:
                            SchemaQuery = Engines.Query(Engines.CurrentDatabase, "PRAGMA table_info(" + Engines.CurrentTable + ");", null, true, GlobalStatic.UserName, "SCHEMA");
                            break;
                        default:
                            throw new Exception("Currently database is not supported");
                    }
                    Dictionary<string, bool> PK = Export.SQL_Fetch_PK(SchemaQuery, Schema, Engines.CurrentEngine);
                    Dictionary<string, string> Types = Export.SQL_Fetch_Type(SchemaQuery, Schema, Engines.CurrentEngine);
                    Export.SQL(Data, Schema, PK, Types, Engines.CurrentTable, Path);
                    LDProcess.Start(Path, null);
                    GraphicsWindow.ShowMessage("Export Completed!", "Success");//TODO Localize
                    return;
                }
                GraphicsWindow.ShowMessage("Oh no something went wrong :(", "Error");
            }
            else if (Item == Utilities.Localization["CSV"] + " ")
            {
                string Path = LDDialogs.SaveFile("csv", null);
                if (!string.IsNullOrWhiteSpace(Path))
                {
                    Primitive Data = Export.Generate2DArrayFromLastQuery();
                    Export.CSV(Data, Export.GenerateSchemaFromQueryData(Data), Path, GlobalStatic.Deliminator);
                    GraphicsWindow.ShowMessage("Export Completed!", "Success");//TODO Localize
                    return;
                }
                GraphicsWindow.ShowMessage("Oh no something went wrong :(", "Error");
            }
            //Settings
            else if (Item == Utilities.Localization["About"])
            {
                Primitive About_Data =  Engines.Query(Engines.CurrentDatabase, "SELECT SQLITE_VERSION(),sqlite_source_id();", null, true, GlobalStatic.UserName, Utilities.Localization["User Requested"] + ":" + Utilities.Localization["App"]);
                string About_Msg = "DBM C# is a Database Mangement Program developed by Abhishek Sathiabalan. (C)" + GlobalStatic.Copyright + ". All rights reserved.\n\nYou are running : " +GlobalStatic.ProductID + " v" + GlobalStatic.VersionID + "\n\n";
                About_Msg += "SQLite Version : " + About_Data[1]["SQLITE_VERSION()"] + "\n" + "SQLITE Source ID : " + About_Data[1]["sqlite_source_id()"];
                GraphicsWindow.ShowMessage(About_Msg, "About"); //DO NOT LOCALIZE
            }
            else if (Item == Utilities.Localization["Show Help"]) //TODO
            {
                string Path = System.IO.Path.Combine(GlobalStatic.AssetPath, "HELP Table.html");
                LDProcess.Start(Path, null);
            }
            else if (Item == Utilities.Localization["Settings Editor"]) //TODO
            {
                UI.SettingsUI();
            }
            else if (Item == Utilities.Localization["Toggle Debug"])
            {
                GlobalStatic.DebugMode = !GlobalStatic.DebugMode;
            }
            else if (Item == Utilities.Localization["Toggle Transaction Log"])
            {
                GlobalStatic.Transactions = !GlobalStatic.Transactions;
            }
            else if (Item == Utilities.Localization["Refresh Schema"])
            {
                Engines.GetSchema(Engines.CurrentDatabase);
                Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
                return;
            }
            else if (Item == Utilities.Localization["Check for Updates"]) //TODO
            {

            }
            //Developer
            else if (Item == Utilities.Localization["Stack Trace"])
            {
                GlobalStatic.DebugMode = true;
                Console.WriteLine("\nStack Trace:");
                Utilities.StackTrace.Print();
            }
            else if (Item == Utilities.Localization["Close TW"])
            {
                TextWindow.Hide();
            }
            else if (Item == Utilities.Localization["Create Statistics Page"]) //TODO
            {

            }
            else
            {
                GraphicsWindow.ShowMessage(Item + " does not exist in context or is not yet implemented", "Error Handlers.Menu");
            }
		}

        public static void ContextMenu(string Control,int Index)
        {
            if (Control == GlobalStatic.ListView)
            {
                if (Index <= 2)
                {
                    Primitive Schema = Export.GenerateSchemaFromQueryData(Export.Generate2DArrayFromLastQuery());
                    LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Sort"], LDControls.LastListViewColumn);
                    LDControls.ComboBoxSelect(GlobalStatic.ComboBox["ASCDESC"], Index);
                    Buttons(UI.Buttons["Sort"]);
                }
            }
        }

        public static void Buttons(string LastButton)
		{
            try
            {
                if (LastButton == UI.Buttons["Search"] || LastButton == UI.Buttons["Sort"] || LastButton == UI.Buttons["RunFunction"])
                {
                    Primitive ASCDESC_Sorts = "1=ASC;2=DESC;";
                    bool Search = false, Sort = true, Function = false;
                    bool StrictSearch = LDControls.CheckBoxGetState(GlobalStatic.CheckBox["StrictSearch"]);
                    bool InvertSearch = LDControls.CheckBoxGetState(GlobalStatic.CheckBox["InvertSearch"]);

                    string SearchIn = Engines.Schema[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["Search"])];
                    string SearchText = LDText.Replace(Controls.GetTextBoxText(GlobalStatic.TextBox["Search"]), "'", "''");
                    string FunctionIn = Engines.Schema[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["ColumnList"])];
                    string FunctionCalled = GlobalStatic.SQLFunctionsList[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["FunctionList"])];

                    string SortBy = Engines.Schema[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["Sort"])];
                    string ASCDESC = ASCDESC_Sorts[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["ASCDESC"])];
                    if (LastButton == UI.Buttons["Search"])
                    {
                        Search = true;
                    }
                    else if (LastButton == UI.Buttons["RunFunction"])
                    {
                        Function = true;
                    }
                    Engines.GenerateQuery(Search, Sort, Function, SearchIn, SortBy, ASCDESC, StrictSearch, InvertSearch, FunctionCalled, FunctionIn, SearchText);
                }
                else if (LastButton == UI.Buttons["CustomQuery"])
                {
                    Engines.Query(Engines.CurrentDatabase, Controls.GetTextBoxText(GlobalStatic.TextBox["CustomQuery"]), GlobalStatic.ListView, false, GlobalStatic.Username, Utilities.Localization["User Requested"]);
                }
                else if (LastButton == UI.Buttons["Command"]) //Custom Command
                {
                    Engines.Command(Engines.CurrentDatabase, Controls.GetTextBoxText(GlobalStatic.TextBox["CustomQuery"]), GlobalStatic.UserName, Utilities.Localization["User Requested"], false);
                }
            }
            catch(KeyNotFoundException)
            {
                GraphicsWindow.ShowMessage(Controls.GetButtonCaption(LastButton) + " does not exist in context or has not yet implemented", "Error Handlers.Buttons");
            }
		}

		public static void ComboBox(string ComboBox, int Index)
		{
			if (ComboBox == GlobalStatic.ComboBox["Table"])
			{
				TableComboBox(Index);
			}
			else if (ComboBox == GlobalStatic.ComboBox["Sorts"])
			{
				SortsComboBox(Index);
			}
			else if (ComboBox == GlobalStatic.ComboBox["Database"])
			{
				DatabaseComboBox(Index);
			}
		}

		static void DatabaseComboBox(int Index)
		{
			LDList.Add(GlobalStatic.List_DB_Tracker, Engines.DB_ShortName[Index-1]);
			Engines.Load_DB(Engines.EnginesModes.SQLITE,Engines.DB_Path[Index-1]);
			Engines.GetSchema(Engines.CurrentDatabase);
			Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);

			SortsComboBox(1); LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Sorts"], 1);
			TableComboBox(1); LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Table"], 1);
			if (GlobalStatic.SortBy == 4)
			{
				Engines.SetDefaultTable("sqlite_master");
				Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
				LDControls.ComboBoxContent(GlobalStatic.ComboBox["Table"], "1=" + Engines.CurrentTable + ";2=sqlite_temp_master;");
			}
			else
			{
				LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Table"], CurrentSchema);
				SortsComboBox(1);
                LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Sorts"], 1);
				Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
			}
		}

		static void SetComboBox()
		{
            Primitive Schema = Engines.Schema;

            LDControls.ComboBoxContent(GlobalStatic.ComboBox["Sort"], Schema);
			LDControls.ComboBoxContent(GlobalStatic.ComboBox["ColumnList"], Schema);
			LDControls.ComboBoxContent(GlobalStatic.ComboBox["Search"], Schema);
			UI.Title();
			Menu(Utilities.Localization["View"]); //Tasks
		}

		static void TableComboBox(int Index)
		{ 
			Engines.SetDefaultTable(CorrectList[Index - 1]);
			Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
			SetComboBox();
		}

		static void SortsComboBox(int Index)
		{ 
		GlobalStatic.SortBy = Index; //Sets GlobalStatic.SortBy

			switch (Index)
			{
				case 1:
					CorrectList = Engines.Tables;
					break;
				case 2:
					CorrectList = Engines.Views;
					break;
				case 3:
					CorrectList = Engines.Indexes;
					break;
				case 4:
					Engines.SetDefaultTable("sqlite_master");
					Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
					break;
			}

			if (Index != 4)
			{
                if (CorrectList.Count > 0) //Prevents OutofBound Errors
                {
                    Engines.SetDefaultTable(CorrectList[0]);
                    LDControls.ComboBoxContent(GlobalStatic.ComboBox["Table"], CorrectList.ToPrimitiveArray());
                    Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
                }
			}
			if (!string.IsNullOrEmpty(Engines.CurrentTable))
			{
				UI.HideDisplayResults();
				SetComboBox();
				UI.Title();
			}
			else
			{
				string Message = "In the current database no " + Utilities.Localization[TypeofSorts[GlobalStatic.SortBy]] + "s can be found.";
				Events.LogMessagePopUp(Message, Message,Utilities.Localization["Error"], Utilities.Localization["UI"]);
			}
		}
	}
}
