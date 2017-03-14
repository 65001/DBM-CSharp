// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
using SBFile = Microsoft.SmallBasic.Library.File;
using System.Threading.Tasks;
using System.Diagnostics;
/*
	Completed Items:
		Settings ; EULA; Logs ;Main;
	To do :
		Emulators 
		SQLITE setup
 */

//Complete Implements and Localize
namespace DBM
{
	[SmallBasicType]
	public static class UI 
	{
		private static string SetTitle;
		private static Stopwatch StartUpStopWatch = Stopwatch.StartNew();
		public static Primitive StartTime = Clock.ElapsedMilliseconds;

		public static void Main()
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.Main()");
			Primitive[] Startime2 = new Primitive[10];
			Startime2[0] = Clock.ElapsedMilliseconds;

			LDUtilities.ShowErrors = false;
			LDUtilities.ShowFileErrors = false;
			LDUtilities.ShowNoShapeErrors = false;
			LDGraphicsWindow.ExitOnClose = false;
			LDGraphicsWindow.CancelClose = true;

			LDGraphicsWindow.Closing += Events.Closing;
			LDEvents.Error += Events.LogEvents;


			GlobalStatic.Ping = LDNetwork.Ping(GlobalStatic.IP_Ping_Address, 500);
			if (GlobalStatic.Ping != -1)
			{
				Startime2[1] = Clock.ElapsedMilliseconds;
				LDNetwork.DownloadFile(GlobalStatic.EULA_Text_File, GlobalStatic.Online_EULA_URI);
			}

			string EulaVersion = LDText.Replace(SBFile.ReadLine(GlobalStatic.EULA_Text_File, 1), " ", "");
			for (int i = 1; i <= Text.GetLength(EulaVersion); i++)
			{
				if (Text.GetSubText(EulaVersion, i, 1) != GlobalStatic.TabKey)
				{
					GlobalStatic.EULA_Newest_Version = GlobalStatic.EULA_Newest_Version + Text.GetSubText(EulaVersion, i, 1);
				}
			}

			Startup();
		}

		public static void Startup()
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.Startup()");
			Settings.LoadSettings(); //Load Application Settings from text file
			Settings.IniateDatabases();
			//Plugin.FindAll();
			Utilities.LocalizationXML(GlobalStatic.LocalizationFolder + GlobalStatic.LanguageCode + ".xml");
			Events.LogMessage(GlobalStatic.LangList["PRGM Start"], GlobalStatic.LangList["Application"]);
			if (Program.ArgumentCount == 1)
			{
				Engines.Load_DB( Engines.EnginesModes.SQLITE, GetPath(4) );
			}
			if (GlobalStatic.EULA_Acceptance == true && GlobalStatic.EULA_Username == LDFile.UserName && GlobalStatic.LastVersion == GlobalStatic.VersionID && GlobalStatic.EulaTest == false)
			{
				Events.LogMessage("Start up GUI", "Debug"); 
				StartupGUI();
			}
			else
			{
				Events.LogMessage("Run EULA", "Debug");
				if (GlobalStatic.DebugMode == true)
				{
					Console.WriteLine("EULA Acceptance: {0} \n EULA Username: {1}\n ",GlobalStatic.EULA_Acceptance,GlobalStatic.EULA_Username);
					Console.WriteLine(GlobalStatic.EULA_Newest_Version + "v" + GlobalStatic.EULA_Accepted_Version);
					Console.WriteLine("Version ID : {0} \n Eula Test {1} \n ", GlobalStatic.VersionID,GlobalStatic.EulaTest);
				}
				Settings.SaveSettings();
				EULA.UI(GlobalStatic.EULA_Text_File);
			}
			StartUpStopWatch.Stop();
			Events.LogMessage("Startup Time: " +StartUpStopWatch.ElapsedMilliseconds + " (ms)",GlobalStatic.LangList["UI"]);
		}

		public static void StartupGUI()
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.StartupGUI()");
			GraphicsWindow.Clear(); GraphicsWindow.Hide(); GraphicsWindow.Show();
			LDScrollBars.Add(GlobalStatic.Listview_Width + 200, GlobalStatic.Listview_Height);
			LDGraphicsWindow.State = 2;
			PreMainMenu();
			MainMenu();
		}

		public static void PreMainMenu() //Defines Buttons
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.PreMainMenu()");
			GlobalStatic.DefaultFontSize = GraphicsWindow.FontSize;
			//Main
			GlobalStatic.MenuList[GlobalStatic.LangList["File"]] = "Main";
			GlobalStatic.MenuList[GlobalStatic.LangList["Edit"]] = "Main";
			GlobalStatic.MenuList[GlobalStatic.LangList["View"]+" "] = "Main";
			GlobalStatic.MenuList[GlobalStatic.LangList["Save"]] = "Main";
			GlobalStatic.MenuList[GlobalStatic.LangList["Import"]] = "Main";
			GlobalStatic.MenuList[GlobalStatic.LangList["Export"]] = "Main";
			GlobalStatic.MenuList[GlobalStatic.LangList["Settings"]] = "Main";
			GlobalStatic.MenuList[GlobalStatic.LangList["Developer"]] = "Main"; 
			//GlobalStatic.MenuList["Plugin"] = "Main"; //Localize
			//File
			GlobalStatic.MenuList[GlobalStatic.LangList["New"]] = GlobalStatic.LangList["File"];
			GlobalStatic.MenuList[GlobalStatic.LangList["Open"]] = GlobalStatic.LangList["File"];
			GlobalStatic.MenuList[GlobalStatic.LangList["Define New Table"]] = GlobalStatic.LangList["File"];
			GlobalStatic.MenuList["-"] = GlobalStatic.LangList["File"];
			//Import
			GlobalStatic.MenuList[GlobalStatic.LangList["CSV"]] = GlobalStatic.LangList["Import"];
			GlobalStatic.MenuList[GlobalStatic.LangList["SQL"]] = GlobalStatic.LangList["Import"];
			GlobalStatic.MenuList["Converter"] = GlobalStatic.LangList["Import"]; //Localize
			GlobalStatic.MenuList["HTML to CSV"] = "Converter"; //Localize
			GlobalStatic.MenuList["-"] = "Converter";
			GlobalStatic.MenuList["-"] = "Import";
			//Export
			GlobalStatic.MenuList[GlobalStatic.LangList["CSV"]+" "] = GlobalStatic.LangList["Export"];
			GlobalStatic.MenuList[GlobalStatic.LangList["SQL"] + " "]= GlobalStatic.LangList["Export"];
			GlobalStatic.MenuList[GlobalStatic.LangList["PXML"] + " "]= GlobalStatic.LangList["Export"];
			GlobalStatic.MenuList[GlobalStatic.LangList["HTML"] + " "]= GlobalStatic.LangList["Export"];
			//GlobalStatic.MenuList[GlobalStatic.LangList["Export UI"]]= GlobalStatic.LangList["Export"];
			GlobalStatic.MenuList["-"] = GlobalStatic.LangList["Export"];
			//Settings
			GlobalStatic.MenuList[GlobalStatic.LangList["Help"]] = GlobalStatic.LangList["Settings"];
			GlobalStatic.MenuList[GlobalStatic.LangList["About"]] = GlobalStatic.LangList["Help"];
			GlobalStatic.MenuList[GlobalStatic.LangList["Show Help"]] = GlobalStatic.LangList["Help"];
			GlobalStatic.MenuList["-"] = GlobalStatic.LangList["Help"];
			GlobalStatic.MenuList[GlobalStatic.LangList["Settings Editor"]] = GlobalStatic.LangList["Settings"];
			GlobalStatic.MenuList[GlobalStatic.LangList["Toggle Debug"]] = GlobalStatic.LangList["Settings"];
			GlobalStatic.MenuList[GlobalStatic.LangList["Toggle Transaction Log"]] = GlobalStatic.LangList["Settings"];
			GlobalStatic.MenuList["-"] = GlobalStatic.LangList["Toggle Transaction Log"];
			GlobalStatic.MenuList[GlobalStatic.LangList["Refresh Schema"]] = GlobalStatic.LangList["Settings"];
			GlobalStatic.MenuList[GlobalStatic.LangList["Check for Updates"]] = GlobalStatic.LangList["Settings"];
			GlobalStatic.MenuList["-"] = GlobalStatic.LangList["Settings"];

			//Developer
			GlobalStatic.MenuList[GlobalStatic.LangList["Stack Trace"]] = GlobalStatic.LangList["Developer"];
			GlobalStatic.MenuList[GlobalStatic.LangList["Close TW"]] = GlobalStatic.LangList["Developer"];
			GlobalStatic.MenuList[GlobalStatic.LangList["Create Statistics Page"]] = GlobalStatic.LangList["Developer"];

			//Plugin Section
			GlobalStatic.MenuList["SB Backup Script"] = GlobalStatic.LangList["Plugin"];
			GlobalStatic.MenuList["Scan"] = "SB Backup Script";
			GlobalStatic.MenuList["View"] = "SB Backup Script";
			GlobalStatic.MenuList["ICF"] = GlobalStatic.LangList["Plugin"];
		}

		public static void MainMenu()
		{
			
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.MainMenu()");
			LDGraphicsWindow.ExitButtonMode(GraphicsWindow.Title, "Enabled");
			GraphicsWindow.CanResize = true;
			GlobalStatic.CheckList[GlobalStatic.LangList["Toggle Debug"]] = GlobalStatic.DebugMode;
			GlobalStatic.CheckList[GlobalStatic.LangList["Toggle Transaction Log"]] = GlobalStatic.Transactions;

			LDGraphicsWindow.State = 2;
			GraphicsWindow.Title = GlobalStatic.Title + " ";
			GlobalStatic.DefaultFontSize = GraphicsWindow.FontSize;
			if (GlobalStatic.DebugMode == true) //Implement 
			{
				Events.LogMessage("Debug Mode is ON", "UI"); //Localize
			}
			 Primitive Sorts;
			Sorts = "1=" + GlobalStatic.LangList["Table"] + ";2=" + GlobalStatic.LangList["View"] + ";3=" + GlobalStatic.LangList["Index"] + ";4=" +GlobalStatic.LangList["Master Table"] + ";";
			if (Engines.CurrentDatabase != null)
			{
				Engines.GetSchema(Engines.CurrentDatabase);
			}
			GraphicsWindow.FontSize = 20;
			string Menu = LDControls.AddMenu(Desktop.Width * 1.5, 30, GlobalStatic.MenuList, null, GlobalStatic.CheckList);
			Shapes.Move(Shapes.AddText(GlobalStatic.LangList["Sort"] + ":"), 990, 1) ;
			            
			int SortOffset = LDText.GetWidth(GlobalStatic.LangList["Sort"] + ":") - LDText.GetWidth("Sort:"); //Offsets some controls when not using the default English encoding

			GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;

			GlobalStatic.ComboBox["Table"] = LDControls.AddComboBox(LDList.ToArray(GlobalStatic.List_SCHEMA_Table), 100, 100);
			GlobalStatic.ComboBox["Sorts"] = LDControls.AddComboBox(Sorts,100,100);
			GlobalStatic.ComboBox["Database"] = LDControls.AddComboBox( LDList.ToArray(GlobalStatic.List_DB_ShortName),100,100);

			Controls.Move(GlobalStatic.ComboBox["Sorts"], 970 + 185 + SortOffset, 5);
			Controls.Move(GlobalStatic.ComboBox["Table"], 1075 + 185 + SortOffset,5);
			Controls.Move(GlobalStatic.ComboBox["Database"], 1050 + SortOffset, 5);

			Handlers.Menu(GlobalStatic.LangList["View"]); //Virtual Call to Handler
			Title();

			Controls.ButtonClicked += Events.BC;
			LDControls.MenuClicked += Events.MC;
			LDControls.ComboBoxItemChanged += Events.CB;
			//TextWindow.WriteLine("Debug Mode:" + GlobalStatic.DebugMode + " Parser : " + GlobalStatic.DebugParser);
			if (GlobalStatic.DebugMode == false && GlobalStatic.DebugParser == false)
			{
				TextWindow.Hide();
			}
		}

		public static string GetPath(int EngineMode)
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.GetPath()");
			if (Program.ArgumentCount == 1 && GlobalStatic.LoadedFile == false)
			{ GlobalStatic.LoadedFile = true; return Program.GetArgument(1); }
			{
				switch (EngineMode)
				{ 
					case (int)Engines.EnginesModes.SQLITE:
						return LDDialogs.OpenFile(GlobalStatic.Extensions, GlobalStatic.LastFolder + "\\");
					default:
						return "Currently not Supported";
				}
			}
		}

		public static void HideDisplayResults() 
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.HideDisplayResults()");
			string Default_Brush = GraphicsWindow.BrushColor;
			GraphicsWindow.BrushColor = "WHITE";
			GraphicsWindow.FillRectangle(GlobalStatic.UIx - 5, 45, 320, 350);
			GraphicsWindow.BrushColor = Default_Brush;
			for (int i = 1; i <= SBArray.GetItemCount(GlobalStatic.HideDisplayResults); i++)
			{
				Controls.HideControl(GlobalStatic.HideDisplayResults[i]);
			}
		}

		public static void ShowDisplayResults()
		{
		LDList.Add(GlobalStatic.List_Stack_Trace, "UI.ShowDisplayResults()");
			GraphicsWindow.DrawRectangle(GlobalStatic.UIx, 50, 310, 240);
			GraphicsWindow.FontSize = 15;
			GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 52, GlobalStatic.LangList["Display Settings"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 73, GlobalStatic.LangList["Sort by"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 150, GlobalStatic.LangList["Search Settings"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 180, GlobalStatic.LangList["Search in"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 210,GlobalStatic.LangList["Search"] + ":");
			GraphicsWindow.FontSize = 13;
			GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;
			for (int i = 1; i <= SBArray.GetItemCount(GlobalStatic.HideDisplayResults);i++)
			{
				Controls.ShowControl(GlobalStatic.HideDisplayResults[i]);
			}
		}

		public static void DisplayResults()
		{ 
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.DisplayResults()");
			LDGraphicsWindow.Width = Desktop.Width;
			LDGraphicsWindow.Height = Desktop.Height;
			GraphicsWindow.Left = 0;
			GraphicsWindow.Top = 0;
			GlobalStatic.UIx = GlobalStatic.Listview_Width + 50;

			GraphicsWindow.DrawRectangle(GlobalStatic.UIx, 50, 310, 340);
			GraphicsWindow.FontSize = 15;
			GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 52, GlobalStatic.LangList["Display Settings"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx+ 20, 73, GlobalStatic.LangList["Sort by"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 150, GlobalStatic.LangList["Search Settings"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 180, GlobalStatic.LangList["Search in"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 210, GlobalStatic.LangList["Search"] + ":");
			GraphicsWindow.DrawText(GlobalStatic.UIx + 127, 290, GlobalStatic.LangList["Functions"] + ":");

			//Did not implement LOG CB //TODO

			//Sort
			GraphicsWindow.FontSize = 13;
			string AscDesc = "1=" + GlobalStatic.LangList["Asc"] + ";2=" + GlobalStatic.LangList["Desc"] + ";";
			GlobalStatic.ComboBox["Sort"] = LDControls.AddComboBox(Engines.Schema, 100, 100);
			GlobalStatic.ComboBox["ASCDESC"] = LDControls.AddComboBox(AscDesc, 110, 100);
			GlobalStatic.Buttons["Sort"] = Controls.AddButton(GlobalStatic.LangList["SORT"], GlobalStatic.UIx + 10, 120);

			Controls.Move(GlobalStatic.ComboBox["Sort"], GlobalStatic.UIx + 80, 72);
			Controls.Move(GlobalStatic.ComboBox["ASCDESC"], GlobalStatic.UIx+ 190, 72);
			Controls.SetSize(GlobalStatic.Buttons["Sort"], 290, 25);

			LDDialogs.ToolTip(GlobalStatic.Buttons["Sort"], "Iniates a sort based on user set parameters"); //Localize
			LDDialogs.ToolTip(GlobalStatic.ComboBox["ASCDESC"], "Sorts Ascending and Decending based on position"); //Localize

			//Search
			GlobalStatic.ComboBox["Search"] = LDControls.AddComboBox(Engines.Schema, 200, 120);
			GlobalStatic.TextBox["Search"] = Controls.AddTextBox(GlobalStatic.UIx + 100, 210);
			GlobalStatic.CheckBox["StrictSearch"] = LDControls.AddCheckBox(GlobalStatic.LangList["Strict Search"]);
			GlobalStatic.CheckBox["InvertSearch"] = LDControls.AddCheckBox("Invert"); //Localize
			GlobalStatic.Buttons["Search"] = Controls.AddButton(Text.ConvertToUpperCase(GlobalStatic.LangList["Search"]), GlobalStatic.UIx + 10, 260);

			Controls.Move(GlobalStatic.CheckBox["StrictSearch"], GlobalStatic.UIx + 20, 240);
			Controls.Move(GlobalStatic.CheckBox["InvertSearch"], GlobalStatic.UIx + 150, 240);
			Controls.Move(GlobalStatic.ComboBox["Search"], GlobalStatic.UIx + 100, 180);
			Controls.SetSize(GlobalStatic.TextBox["Search"], 200, 25);
			Controls.SetSize(GlobalStatic.Buttons["Search"], 290, 25);

			//Functions
			GlobalStatic.ComboBox["FunctionList"] = LDControls.AddComboBox(GlobalStatic.SQLFunctionsList, 130, 100);
			Controls.Move(GlobalStatic.ComboBox["FunctionList"], GlobalStatic.UIx + 10, 310);
			GlobalStatic.ComboBox["ColumnList"] = LDControls.AddComboBox(Engines.Schema, 135, 100);
			Controls.Move(GlobalStatic.ComboBox["ColumnList"], GlobalStatic.UIx + 160, 310);

			GlobalStatic.Buttons["RunFunction"] = Controls.AddButton(Text.ConvertToUpperCase(GlobalStatic.LangList["Run Function"]), GlobalStatic.UIx + 10, 340);
			Controls.SetSize(GlobalStatic.Buttons["RunFunction"], 290, 25);

			//Custom Query
			GlobalStatic.TextBox["CustomQuery"] = Controls.AddMultiLineTextBox(GlobalStatic.UIx, 420);
			Controls.SetSize(GlobalStatic.TextBox["CustomQuery"], 310, 150);

  			GlobalStatic.Buttons["CustomQuery"] = Controls.AddButton(Text.ConvertToUpperCase(GlobalStatic.LangList["Query"]), GlobalStatic.UIx, 580);
  			Controls.SetSize(GlobalStatic.Buttons["CustomQuery"], 310, 25);
  			
  			GlobalStatic.Buttons["Command"] = Controls.AddButton(Text.ConvertToUpperCase(GlobalStatic.LangList["Command"]), GlobalStatic.UIx, 580 + 35);
			Controls.SetSize(GlobalStatic.Buttons["Command"], 310, 25);
			LDDialogs.ToolTip(GlobalStatic.Buttons["Command"], "Executes customized SQL command statements onto the database"); //Localize
			string CustomQueryData = "This Textbox allows you to use Custom\nSQL Queries. Remove this and type in an SQL \nstatement. \nYou also use it to export data";//Localize
			Controls.SetTextBoxText(GlobalStatic.TextBox["CustomQuery"],CustomQueryData);

			//Hide Display Results
			GlobalStatic.HideDisplayResults[1] = GlobalStatic.ComboBox["Sort"];
			GlobalStatic.HideDisplayResults[2] = GlobalStatic.ComboBox["ASCDESC"];
			GlobalStatic.HideDisplayResults[3] = GlobalStatic.Buttons["Sort"];
			GlobalStatic.HideDisplayResults[4] = GlobalStatic.ComboBox["Search"];
			GlobalStatic.HideDisplayResults[5] = GlobalStatic.TextBox["Search"];
			GlobalStatic.HideDisplayResults[6] = GlobalStatic.CheckBox["StrictSearch"];
			GlobalStatic.HideDisplayResults[7] = GlobalStatic.CheckBox["InvertSearch"];
			GlobalStatic.HideDisplayResults[8] = GlobalStatic.Buttons["Search"];
			GlobalStatic.HideDisplayResults[9] = GlobalStatic.ComboBox["FunctionList"];
			GlobalStatic.HideDisplayResults[10] = GlobalStatic.ComboBox["ColumnList"];
			GlobalStatic.HideDisplayResults[11] = GlobalStatic.Buttons["RunFunction"];
			GlobalStatic.HideDisplayResults[12] = GlobalStatic.TextBox["CustomQuery"];
			GlobalStatic.HideDisplayResults[13] = GlobalStatic.Buttons["CustomQuery"];
			GlobalStatic.HideDisplayResults[14] = GlobalStatic.Buttons["Command"];
		}

		public static void Title()
		{ 
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.Title()");
			string TimeRef;
			SetTitle = GlobalStatic.Title + " " + Engines.Database_Shortname + "(" + Engines.CurrentDatabase + ") :" + Handlers.TypeofSorts[GlobalStatic.SortBy] + ":" + Engines.CurrentTable;
			TimeRef = LDList.GetAt(GlobalStatic.List_Time_Refer, LDList.Count(GlobalStatic.List_Time_Refer)); //Time of Last Query or Command
			if (string.IsNullOrEmpty(Engines.CurrentDatabase))
			{
				SetTitle = GlobalStatic.Title;
			}
			else 
			{
				switch (TimeRef)
				{
					case "CMD":
						SetTitle += "( CMD TIME : " + LDList.GetAt(GlobalStatic.List_CMD_Time, LDList.Count(GlobalStatic.List_CMD_Time)) + ")";
						break;
					case "Query":
						SetTitle += "( Query Time : " + LDList.GetAt(GlobalStatic.List_Query_Time, LDList.Count(GlobalStatic.List_Query_Time)) + ")";
						break;
				}
			}
			GraphicsWindow.Title = SetTitle;
		}


		public static void SettingsUI()//TODO
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.SettingsUI()");
		}

		public static void CreateTableUI()//TODO
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.CreateTableUI()");
		}

		public static void CreateTableHandler()//TODO
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.CreateTableHandler()");
		}
	}

	public static class Events
	{
		public static void LogEvents() //Error Handler
		{
			LogMessage(LDEvents.LastError, GlobalStatic.LangList["System"]);
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.LogEvents()");
		}
		public static void LogMessagePopUp(Primitive MessagePopUp,Primitive MessageLog, Primitive Title, Primitive Type)
		{
			GraphicsWindow.ShowMessage(MessagePopUp, Title);
			LogMessage(MessageLog, Type);
		}

		public static void LogMessage(Primitive Message, string Type) //Logs Message to all applicable locations
		{
			if (GlobalStatic.DebugMode == true && Type != GlobalStatic.LangList["System"]) //Writes Log Message to TextWindow
			{
				TextWindow.WriteLine(LDList.GetAt(GlobalStatic.List_Stack_Trace, LDList.Count(GlobalStatic.List_Stack_Trace)) + ":" + Type + ":" + Message);
			}

			if (Type.Equals("Debug") == true && GlobalStatic.DebugMode == false) 
			{
				return;
			}
			else if (Type.Equals("PopUp") == true) 
			{
				GraphicsWindow.ShowMessage(Message, LDList.GetAt(GlobalStatic.List_Stack_Trace, LDList.Count(GlobalStatic.List_Stack_Trace) + "REVERT!" ));
			}
			else
			{
				if (string.IsNullOrEmpty(Type))
					{
						Type = "Unknown"; 
					}
				if (GlobalStatic.DebugMode == true)
				{
					if (Text.IsSubText(Message, "LDDataBase.Query") == true || Text.IsSubText(Message, "LDDataBase.Command") == true) 
					{
						TextWindow.WriteLine(Engines.CurrentDatabase + "\n" + Message);
					}
				}
			}
			GlobalStatic.LogNumber = GlobalStatic.LogNumber + 1;
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.LogMessage()");

			SBFile.AppendContents(GlobalStatic.LogCSVpath,GlobalStatic.LogNumber +"," + Clock.Date + "," + Clock.Time +"," + "\"" + LDText.Replace( GlobalStatic.Username , "\"" , "\"" + "\"" ) + "\"" +"," + GlobalStatic.ProductID +","+ GlobalStatic.VersionID+"," + "\"" +  LDText.Replace( Type, "\"" , "\"" + "\"") + "\"" +","+  "\"" +  LDText.Replace(Message ,"\"" , "\"" + "\"" ) + "\"");
			string LogCMD = "INSERT INTO LOG ([UTC DATE],[UTC TIME],DATE,TIME,USER,ProductID,ProductVersion,Event,Type) VALUES(DATE(),TIME(),DATE('now','localtime'),TIME('now','localtime'),'";
			LogCMD = LogCMD + GlobalStatic.Username + "','" + GlobalStatic.ProductID + "','" + GlobalStatic.VersionID + "','" + Message + "','" + Type + "');";
			Engines.Command(GlobalStatic.LogDB, LogCMD, GlobalStatic.LangList["App"], GlobalStatic.LangList["Auto Log"],false);
		}

		public static void Closing()
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.Closing()");
			if (string.IsNullOrEmpty(Engines.CurrentDatabase))
			{ LogMessage("Program Closing", GlobalStatic.LangList["Application"]); } //Localize
			else 
			{ LogMessage("Program Closing - Closing : " + Engines.Database_Shortname , GlobalStatic.LangList["Application"]); } //Localize

			if (LDWindows.CurrentID == 0)
			{ Program.End(); }
			else
			{ 
				GraphicsWindow.Clear();
				GraphicsWindow.Hide();
			}
		}

		//The following async the Handlers class to make the code faster! Warning ! Can cause bugs!!!
		public async static void BC()
		{
			await Task.Run(() => { Handlers.Buttons(Controls.LastClickedButton); });
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.BC()");
		}

		public async static void MC() //Menu Clicked Event Handler
		{
			await Task.Run(() => { Handlers.Menu(LDControls.LastMenuItem); });
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.MC()");
		}

		public async static void CB()
		{
			await Task.WhenAll(Task.Run(() => { Handlers.ComboBox(LDControls.LastComboBox, LDControls.LastComboBoxIndex); }));
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.CB()");
		}

	}
}