// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.Threading;
using System.Threading.Tasks;
using LitDev;
using Microsoft.SmallBasic.Library;
namespace DBM
{
	public static class Handlers
	{
		public static Primitive CurrentSchema;static string CorrectList;
		public static Primitive TypeofSorts ="1="+ GlobalStatic.LangList["Table"] +";2=" + GlobalStatic.LangList["View"] +"3="+ GlobalStatic.LangList["Index"] + "4="+ GlobalStatic.LangList["Master Table"]+";";

		public static void Menu(string Item) //Handles Main Menu  //TODO 
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Handlers.Menu(" + Item + ")");

			//Switch and Enum cannot be used because values can change
			//File Menu Items
			if (Item == GlobalStatic.LangList["New"])
			{
				string Path = LDDialogs.SaveFile(GlobalStatic.Extensions, GlobalStatic.LastFolder);
				if (!string.IsNullOrWhiteSpace(Path))
				{
					GlobalStatic.ListView = null;
					GlobalStatic.Dataview = null;
					GlobalStatic.LastFolder = LDFile.GetFolder(Path);
					Settings.LoadSettings();
					Settings.SaveSettings();
					Events.LogMessage("Created DB :" + Path, GlobalStatic.LangList["Application"]);
					UI.PreMainMenu();
					UI.MainMenu();
					LDDataBase.Connection = null; 
				}
			}
			else if (Item == GlobalStatic.LangList["Open"])
			{
				GlobalStatic.ListView = null; 
				GlobalStatic.Dataview = null;
				Settings.LoadSettings(); //Reloads Settings
				Engines.Load_DB(Engines.EnginesModes.SQLITE, UI.GetPath(4));
				Settings.SaveSettings();
				UI.PreMainMenu();
				UI.MainMenu();
			}
			else if (Item == GlobalStatic.LangList["Define New Table"]) //TODO
			{

			}
			//Main
			else if (Item == GlobalStatic.LangList["View"] || Item == GlobalStatic.LangList["View"] + " ")
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
					Engines.Query(Engines.CurrentDatabase, "SELECT * FROM " + Engines.CurrentTable + ";", GlobalStatic.ListView, false, GlobalStatic.LangList["App"], GlobalStatic.LangList["View Function"]);
				}
			}
			else if (Item == GlobalStatic.LangList["Save"])
			{
				if (!string.IsNullOrEmpty(Engines.CurrentDatabase) && !string.IsNullOrEmpty(GlobalStatic.Dataview))
				{
					string SaveStatus = LDDataBase.SaveTable(Engines.CurrentDatabase, GlobalStatic.Dataview);
					Events.LogMessage("The save was : " + SaveStatus, GlobalStatic.LangList["UI"]);
					GraphicsWindow.ShowMessage("The save was : " + SaveStatus, "Save Status");
				}
				else
				{
					Events.LogMessage(GlobalStatic.LangList["Dataview Error"], GlobalStatic.LangList["UI"]);
					GraphicsWindow.ShowMessage(GlobalStatic.LangList["Error"] + ":" + GlobalStatic.LangList["Dataview Error"], "Save Error");
				}
			}
			else if (Item == GlobalStatic.LangList["Edit"])
			{
				if (!string.IsNullOrEmpty(Engines.CurrentDatabase))
				{
					switch (GlobalStatic.SortBy)
					{
						case 2:
							Events.LogMessage(GlobalStatic.LangList["Views Read Only"], GlobalStatic.LangList["UI"]);
							GraphicsWindow.ShowMessage(GlobalStatic.LangList["Error"] + ":" + GlobalStatic.LangList["Views Read Only"], GlobalStatic.LangList["Access Denied"]);
							break;
						case 4:
							Events.LogMessage(GlobalStatic.LangList["Master Table Protected"], GlobalStatic.LangList["UI"]);
							GraphicsWindow.ShowMessage(GlobalStatic.LangList["Error"] + ":" + GlobalStatic.LangList["Master Table Protected"], GlobalStatic.LangList["Access Denied"]);
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
					Events.LogMessage(GlobalStatic.LangList["Error No DB"], GlobalStatic.LangList["UI"]);
					GraphicsWindow.ShowMessage(GlobalStatic.LangList["Error No DB"], GlobalStatic.LangList["UI"]);
				}
			}
			//Import
			else if (Item == GlobalStatic.LangList["CSV"]) //TODO
			{ } 
			else if (Item == GlobalStatic.LangList["SQL"]) //TODO
			{ }
			else if (Item == GlobalStatic.LangList["SQL"]) //TODO
			{ }
			else if (Item == GlobalStatic.LangList["HTML to CSV"]) //Plugin //TODO
			{ }
			//Export
			else if (Item == GlobalStatic.LangList["PXML"] + " ") //TODO
			{ }
			else if (Item == GlobalStatic.LangList["HTML"] + " ") //TODO
			{ }
			//else if (Item == GlobalStatic.LangList["Export UI"]) //TODO
			//{ }
			else if (Item == GlobalStatic.LangList["SQL"] + " ") //TODO
			{ } 
			else if (Item == GlobalStatic.LangList["CSV"] + " ") //TODO
			{ }
			//Settings
			else if (Item == GlobalStatic.LangList["About"]) //TODO
			{ }
			else if (Item == GlobalStatic.LangList["Show Help"]) //TODO
			{ }
			else if (Item == GlobalStatic.LangList["Settings Editor"]) //TODO
			{ }
			else if (Item == GlobalStatic.LangList["Toggle Debug"])
			{
				GlobalStatic.DebugMode = !GlobalStatic.DebugMode;
			}
			else if (Item == GlobalStatic.LangList["Toggle Transaction Log"]) 
			{
				GlobalStatic.Transactions = !GlobalStatic.Transactions;
			}
			else if (Item == GlobalStatic.LangList["Refresh Schema"]) //TODO
			{
				Engines.GetSchema(Engines.CurrentDatabase);
				Engines.GetSchemaofTable(Engines.CurrentDatabase,Engines.CurrentTable);
			}
			else if (Item == GlobalStatic.LangList["Check for Updates"]) //TODO
			{ }
			//Developer
			else if (Item == GlobalStatic.LangList["Stack Trace"]) 
			{
				GlobalStatic.DebugMode = true;
				Console.WriteLine("Debug Mode turned on due to current action.");
				LDList.Print(GlobalStatic.List_Stack_Trace);
			}
			else if (Item == GlobalStatic.LangList["Close TW"])
			{
				TextWindow.Hide();
			}
			else if (Item == GlobalStatic.LangList["Create Statistics Page"]) //TODO
			{ }
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
				Engines.Query(Engines.CurrentDatabase, Controls.GetTextBoxText(GlobalStatic.TextBox["CustomQuery"]), GlobalStatic.ListView, false, GlobalStatic.Username, GlobalStatic.LangList["User Requested"]);
			}
			else if (LastButton == GlobalStatic.Buttons["Command"]) //Custom Command
			{
				Engines.Command(Engines.CurrentDatabase, Controls.GetTextBoxText(GlobalStatic.TextBox["Command"]), GlobalStatic.UserName, GlobalStatic.LangList["User Requested"], false);
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
			LDList.Add(GlobalStatic.List_DB_Tracker, LDList.GetAt(GlobalStatic.List_DB_ShortName, Index));
			Engines.Load_DB(Engines.EnginesModes.SQLITE, LDList.GetAt(GlobalStatic.List_DB_Path, Index));
			Engines.GetSchema(Engines.CurrentDatabase);
			Engines.GetSchemaofTable(Engines.CurrentDatabase, Engines.CurrentTable);

			SortsComboBox(1); LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Sorts"], 1);
			TableComboBox(1); LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Table"], 1);
			if (GlobalStatic.SortBy == 4)
			{
				Engines.SetDefaultTable("sqlite_master");
				Engines.GetSchemaofTable(Engines.CurrentDatabase, Engines.CurrentTable);
				LDControls.ComboBoxContent(GlobalStatic.ComboBox["Table"], "1=" + Engines.CurrentTable + ";2=sqlite_temp_master;");
			}
			else
			{
				LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Table"], CurrentSchema);
				SortsComboBox(1); LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Sorts"], 1);
				Engines.GetSchemaofTable(Engines.CurrentDatabase, Engines.CurrentTable);
			}
		}

		static void SetComboBox()
		{ 
			LDControls.ComboBoxContent(GlobalStatic.ComboBox["Sort"], Engines.Schema);
			LDControls.ComboBoxContent(GlobalStatic.ComboBox["ColumnList"], Engines.Schema);
			LDControls.ComboBoxContent(GlobalStatic.ComboBox["Search"], Engines.Schema);
			UI.Title();
			Handlers.Menu(GlobalStatic.LangList["View"]); //Tasks
		}

		static void TableComboBox(int Index)
		{ 
			Engines.SetDefaultTable(LDList.GetAt(CorrectList, Index));
			Engines.GetSchemaofTable(Engines.CurrentDatabase, Engines.CurrentTable);
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
					Engines.GetSchemaofTable(Engines.CurrentDatabase, Engines.CurrentTable);
					break;
			}

			if (Index != 4)
			{
				Engines.SetDefaultTable(LDList.GetAt(CorrectList, 1));
				LDControls.ComboBoxContent(GlobalStatic.ComboBox["Table"], LDList.ToArray(CorrectList));
				Engines.GetSchemaofTable(Engines.CurrentDatabase, Engines.CurrentTable);
			}
			if (!string.IsNullOrEmpty(Engines.CurrentTable))
			{
				UI.HideDisplayResults();
				SetComboBox();
				UI.Title();
			}
			else
			{
				string Message = "In the current database no " + GlobalStatic.LangList[Handlers.TypeofSorts[GlobalStatic.SortBy]] + "s can be found.";
				Events.LogMessagePopUp(Message, Message,GlobalStatic.LangList["Error"], GlobalStatic.LangList["UI"]);
			}
		
		}
	}
}
