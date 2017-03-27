// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using LitDev;
using Microsoft.SmallBasic.Library;
namespace DBM
{
	public static class Handlers
	{
		public static Primitive CurrentSchema;static string CorrectList;
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
				Engines.Load_DB(Engines.EnginesModes.SQLITE, UI.GetPath(4));
				Settings.SaveSettings();
				UI.PreMainMenu();
				UI.MainMenu();
                return;
			}
			else if (Item == Utilities.Localization["Define New Table"]) //TODO ADD UI
			{

			}
			//Main
			else if (Item == Utilities.Localization["View"] || Item == Utilities.Localization["View"] + " ")
			{
				Controls.HideControl(GlobalStatic.Dataview);
				if (GlobalStatic.ListView == null)
				{
					GlobalStatic.ListView = LDDataBase.AddListView(GlobalStatic.Listview_Width, GlobalStatic.Listview_Height);
					Controls.Move(GlobalStatic.ListView, 10, 35);
					UI.DisplayResults();
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
								GlobalStatic.Dataview = LDControls.AddDataView(GlobalStatic.Listview_Width, GlobalStatic.Listview_Height,null);
								Controls.Move(GlobalStatic.Dataview, 10, 35);
							}
							else 
							{
								Controls.ShowControl(GlobalStatic.Dataview);
							}
							Engines.EditTable(Engines.CurrentTable, GlobalStatic.Dataview);
							UI.HideDisplayResults();
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
			else if (Item == Utilities.Localization["CSV"]) //TODO
			{
               string SQL = Import.CSV(LDDialogs.OpenFile("csv", null));
               Engines.Command(Engines.CurrentDatabase, SQL, GlobalStatic.UserName, "", false); //TODO
            } 
			else if (Item == Utilities.Localization["SQL"]) //TODO
			{ }
			else if (Item == Utilities.Localization["SQL"]) //TODO
			{ }
			else if (Item == Utilities.Localization["HTML to CSV"]) //Plugin //TODO
			{ }
			//Export
			else if (Item == Utilities.Localization["PXML"] + " ") //TODO
			{ }
			else if (Item == Utilities.Localization["HTML"] + " ") //TODO
			{ }
			//else if (Item == Utilities.Localization["Export UI"]) //TODO
			//{ }
			else if (Item == Utilities.Localization["SQL"] + " ") //TODO
			{ } 
			else if (Item == Utilities.Localization["CSV"] + " ") //TODO
			{ }
			//Settings
			else if (Item == Utilities.Localization["About"]) //TODO
			{ }
			else if (Item == Utilities.Localization["Show Help"]) //TODO
			{ }
			else if (Item == Utilities.Localization["Settings Editor"]) //TODO
			{ }
			else if (Item == Utilities.Localization["Toggle Debug"])
			{
				GlobalStatic.DebugMode = !GlobalStatic.DebugMode;
			}
			else if (Item == Utilities.Localization["Toggle Transaction Log"]) 
			{
				GlobalStatic.Transactions = !GlobalStatic.Transactions;
			}
			else if (Item == Utilities.Localization["Refresh Schema"]) //TODO
			{
				Engines.GetSchema(Engines.CurrentDatabase);
				Engines.GetColumnsofTable(Engines.CurrentDatabase,Engines.CurrentTable);
                return;
			}
			else if (Item == Utilities.Localization["Check for Updates"]) //TODO
			{ }
			//Developer
			else if (Item == Utilities.Localization["Stack Trace"]) 
			{
				GlobalStatic.DebugMode = true;
				Console.WriteLine("Debug Mode turned on due to current action.");
                //TODO Print Utilities.StackTrace
				//LDList.Print(GlobalStatic.List_Stack_Trace);
			}
			else if (Item == Utilities.Localization["Close TW"])
			{
				TextWindow.Hide();
			}
			else if (Item == Utilities.Localization["Create Statistics Page"]) //TODO
			{

            }
			//Plugins

			else
			{
				GraphicsWindow.ShowMessage(Item + " does not exist in context or is not yet implemented", "Error Handlers.Menu");
			}
		}

		public static void Buttons(string LastButton)
		{
			if (LastButton == GlobalStatic.Buttons["Search"] || LastButton == GlobalStatic.Buttons["Sort"] || LastButton == GlobalStatic.Buttons["RunFunction"])
			{
				Primitive ASCDESC_Sorts = "1=ASC;2=DESC;";
				bool Search =false,Sort=true,Function=false;
				bool StrictSearch = LDControls.CheckBoxGetState(GlobalStatic.CheckBox["StrictSearch"]);
				bool InvertSearch = LDControls.CheckBoxGetState(GlobalStatic.CheckBox["InvertSearch"]);

				string SearchIn = Engines.Schema[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["Search"])];
				string SearchText =LDText.Replace( Controls.GetTextBoxText(GlobalStatic.TextBox["Search"]),"'","''");
				string FunctionIn = Engines.Schema[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["ColumnList"])];
				string FunctionCalled = GlobalStatic.SQLFunctionsList[ LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["FunctionList"]) ];

				string SortBy = Engines.Schema[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["Sort"])];
				string ASCDESC = ASCDESC_Sorts[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["ASCDESC"])];
				//Console.WriteLine("{0} {1}", LDControls.CheckBoxGetState(GlobalStatic.ComboBox["Search"]),GlobalStatic.ComboBox);
				if (LastButton == GlobalStatic.Buttons["Search"])
				{
					Search = true;
				}
				else if (LastButton == GlobalStatic.Buttons["RunFunction"])
				{
					Function = true;
				}
				//Console.WriteLine("Search :{0} Sort : {1} Function : {2}\n Search Text : {3} Function Column : {4} Sort Order : {5} Sorts : {6}", Search, Sort, Function, SearchText,FunctionIn,ASCDESC,SortBy);
				Engines.GenerateQuery(Search, Sort, Function, SearchIn, SortBy, ASCDESC, StrictSearch, InvertSearch, FunctionCalled, FunctionIn, SearchText);
			}
			else if (LastButton == GlobalStatic.Buttons["CustomQuery"])
			{
				Engines.Query(Engines.CurrentDatabase, Controls.GetTextBoxText(GlobalStatic.TextBox["CustomQuery"]), GlobalStatic.ListView, false, GlobalStatic.Username, Utilities.Localization["User Requested"]);
			}
			else if (LastButton == GlobalStatic.Buttons["Command"]) //Custom Command
			{
				Engines.Command(Engines.CurrentDatabase, Controls.GetTextBoxText(GlobalStatic.TextBox["CustomQuery"]), GlobalStatic.UserName, Utilities.Localization["User Requested"], false);
			}
			else
			{
				GraphicsWindow.ShowMessage(Controls.GetButtonCaption(LastButton) + " does not exist in context or is not yet implemented", "Error Handlers.Buttons");
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
				SortsComboBox(1); LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Sorts"], 1);
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
			Engines.SetDefaultTable(LDList.GetAt(CorrectList, Index));
			Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
			SetComboBox();
		}

		static void SortsComboBox(int Index)
		{ 
		GlobalStatic.SortBy = Index; //Sets GlobalStatic.SortBy

			switch (Index)
			{
				case 1:
					CorrectList = GlobalStatic.List_SCHEMA_Table;
					break;
				case 2:
					CorrectList = GlobalStatic.List_SCHEMA_View;
					break;
				case 3:
					CorrectList = GlobalStatic.List_Schema_Index;
					break;
				case 4:
					Engines.SetDefaultTable("sqlite_master");
					Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
					break;
			}

			if (Index != 4)
			{
				Engines.SetDefaultTable(LDList.GetAt(CorrectList, 1));
				LDControls.ComboBoxContent(GlobalStatic.ComboBox["Table"], LDList.ToArray(CorrectList));
				Engines.GetColumnsofTable(Engines.CurrentDatabase, Engines.CurrentTable);
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
