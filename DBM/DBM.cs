// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
using SBFile = Microsoft.SmallBasic.Library.File;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
/*
	Completed Items:
		Settings ; EULA; Logs ;Main;
	Todo :
		Emulators 
		SQLITE setup
		Set it up so that if no table exists in a database then revert to sqlite_master or equivalent
		
	//TODO
		Replace all instances of LDLIST with an actual List?
		StackTrace implemented across everything. (implemented as a method)
		StrackGetLast or somethign as a function
		Replace all instances of GlobalStatic.List_DB_* ASAP.
		Start to use System.Version instead of an int and replace instances of GlobablStatic.VersionID to it as well
		
 */

//Complete Implements and Localize
namespace DBM
{
	[SmallBasicType]
	public static class UI 
	{
		private static Stopwatch StartUpStopWatch = Stopwatch.StartNew();
        private static Dictionary<string, string> _Buttons = new Dictionary<string, string>(); 
        private static Dictionary<string, string> _TextBox = new Dictionary<string, string>();  //TODO Implement this
        private static Dictionary<string, string> _CheckBox = new Dictionary<string, string>(); //TODO Implement this
        private static Dictionary<string, string> _ComboBox = new Dictionary<string, string>(); //TODO Implement this
        private static List<string> _HideDisplay = new List<string>();

        public static IReadOnlyDictionary<string,string> Buttons
        {get { return _Buttons; } }

        public static IReadOnlyDictionary<string,string> TextBox
        { get { return _TextBox; } }

        public static IReadOnlyDictionary<string,string> CheckBox
        { get { return _CheckBox; } }

		public static void Main()
		{
			Utilities.AddtoStackTrace( "UI.Main()");

			LDUtilities.ShowErrors = false;
			LDUtilities.ShowFileErrors = false;
			LDUtilities.ShowNoShapeErrors = false;
			LDGraphicsWindow.ExitOnClose = false;
			LDGraphicsWindow.CancelClose = true;

			LDGraphicsWindow.Closing += Events.Closing;
			LDEvents.Error += Events.LogEvents;

			GlobalStatic.Ping = LDNetwork.Ping(GlobalStatic.IP_Ping_Address, 500);
			if (GlobalStatic.Ping != -1) //Represents Network Working
			{
				LDNetwork.DownloadFile(GlobalStatic.EULA_Text_File, GlobalStatic.Online_EULA_URI);
			}

			string EulaVersion = LDText.Replace(SBFile.ReadLine(GlobalStatic.EULA_Text_File, 1), " ", "");
			for (int i = 1; i <= Text.GetLength(EulaVersion); i++)
			{
				if (Text.GetSubText(EulaVersion, i, 1) != "\t")
				{
					GlobalStatic.EULA_Newest_Version = GlobalStatic.EULA_Newest_Version + Text.GetSubText(EulaVersion, i, 1);
				}
			}
			Startup();
		}

		public static void Startup()
		{
			Utilities.AddtoStackTrace( "UI.Startup()");
			Settings.LoadSettings(GlobalStatic.RestoreSettings); //Load Application Settings from text file

			Settings.Paths
			        (
				GlobalStatic.AssetPath,
			    GlobalStatic.PluginPath,
			    GlobalStatic.LocalizationFolder,
			    GlobalStatic.AutoRunPluginPath,
			    GlobalStatic.Localization_LanguageCodes_Path,
			    GlobalStatic.LogCSVpath,
			    GlobalStatic.AutoRunPluginMessage
					); //Makes sure passed paths are valid and creates them if they are not
			
			Settings.IniateDatabases();
			//Plugin.FindAll();
			GlobalStatic.DefaultWidth = GraphicsWindow.Width;
			GlobalStatic.DefaultHeight = GraphicsWindow.Height;

			Utilities.LocalizationXML(Path.Combine( GlobalStatic.LocalizationFolder , GlobalStatic.LanguageCode + ".xml"));
			Events.LogMessage(Utilities.Localization["PRGM Start"], Utilities.Localization["Application"]);

			if (Program.ArgumentCount == 1)
			{
				Engines.Load_DB( Engines.EnginesModes.SQLITE, GetPath(4) );
			}
			if (GlobalStatic.EULA_Acceptance == true && GlobalStatic.EULA_Username == LDFile.UserName && GlobalStatic.LastVersion == (int)LDText.Replace( GlobalStatic.VersionID.ToString(),".","") && GlobalStatic.EulaTest == false)
			{ 
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
				EULA.UI(GlobalStatic.EULA_Text_File,GlobalStatic.Ping);
			}
			StartUpStopWatch.Stop();
			Events.LogMessage("Startup Time: " +StartUpStopWatch.ElapsedMilliseconds + " (ms)",Utilities.Localization["UI"]);
		}

		public static void StartupGUI()
		{
            Utilities.AddtoStackTrace( "UI.StartupGUI()");
			GraphicsWindow.Clear(); GraphicsWindow.Hide(); GraphicsWindow.Show();
			LDScrollBars.Add(GlobalStatic.Listview_Width + 200, GlobalStatic.Listview_Height);
			LDGraphicsWindow.State = 2;
			PreMainMenu();
			MainMenu();
		}

		public static void PreMainMenu()
		{
			Utilities.AddtoStackTrace( "UI.PreMainMenu()");
			GlobalStatic.DefaultFontSize = GraphicsWindow.FontSize;
            
			//Main
			GlobalStatic.MenuList[Utilities.Localization["File"]] = "Main";
			GlobalStatic.MenuList[Utilities.Localization["Edit"]] = "Main";
			GlobalStatic.MenuList[Utilities.Localization["View"]+" "] = "Main";
			GlobalStatic.MenuList[Utilities.Localization["Save"]] = "Main";
			GlobalStatic.MenuList[Utilities.Localization["Import"]] = "Main";
			GlobalStatic.MenuList[Utilities.Localization["Export"]] = "Main";
			GlobalStatic.MenuList[Utilities.Localization["Settings"]] = "Main";
			GlobalStatic.MenuList[Utilities.Localization["Developer"]] = "Main"; 
			//GlobalStatic.MenuList["Plugin"] = "Main"; //Localize
			//File
			GlobalStatic.MenuList[Utilities.Localization["New"]] = Utilities.Localization["File"];
			GlobalStatic.MenuList[Utilities.Localization["Open"]] = Utilities.Localization["File"];
			GlobalStatic.MenuList[Utilities.Localization["Define New Table"]] = Utilities.Localization["File"];
			GlobalStatic.MenuList["-"] = Utilities.Localization["File"];
			//Import
			GlobalStatic.MenuList[Utilities.Localization["CSV"]] = Utilities.Localization["Import"];
			GlobalStatic.MenuList[Utilities.Localization["SQL"]] = Utilities.Localization["Import"];
			GlobalStatic.MenuList["Converter"] = Utilities.Localization["Import"]; //Localize
			GlobalStatic.MenuList["HTML to CSV"] = "Converter"; //Localize
			GlobalStatic.MenuList["-"] = "Converter";
			GlobalStatic.MenuList["-"] = "Import";
			//Export
			GlobalStatic.MenuList[Utilities.Localization["CSV"]+" "] = Utilities.Localization["Export"];
			GlobalStatic.MenuList[Utilities.Localization["SQL"] + " "]= Utilities.Localization["Export"];
			GlobalStatic.MenuList[Utilities.Localization["PXML"] + " "]= Utilities.Localization["Export"];
			GlobalStatic.MenuList[Utilities.Localization["HTML"] + " "]= Utilities.Localization["Export"];
			//GlobalStatic.MenuList[Utilities.Localization["Export UI"]]= Utilities.Localization["Export"];
			GlobalStatic.MenuList["-"] = Utilities.Localization["Export"];
			//Settings
			GlobalStatic.MenuList[Utilities.Localization["Help"]] = Utilities.Localization["Settings"];
			GlobalStatic.MenuList[Utilities.Localization["About"]] = Utilities.Localization["Help"];
			GlobalStatic.MenuList[Utilities.Localization["Show Help"]] = Utilities.Localization["Help"];
			GlobalStatic.MenuList["-"] = Utilities.Localization["Help"];
			GlobalStatic.MenuList[Utilities.Localization["Settings Editor"]] = Utilities.Localization["Settings"];
			GlobalStatic.MenuList[Utilities.Localization["Toggle Debug"]] = Utilities.Localization["Settings"];
			GlobalStatic.MenuList[Utilities.Localization["Toggle Transaction Log"]] = Utilities.Localization["Settings"];
			GlobalStatic.MenuList["-"] = Utilities.Localization["Toggle Transaction Log"];
			GlobalStatic.MenuList[Utilities.Localization["Refresh Schema"]] = Utilities.Localization["Settings"];
			GlobalStatic.MenuList[Utilities.Localization["Check for Updates"]] = Utilities.Localization["Settings"];
			GlobalStatic.MenuList["-"] = Utilities.Localization["Settings"];

			//Developer
			GlobalStatic.MenuList[Utilities.Localization["Stack Trace"]] = Utilities.Localization["Developer"];
			GlobalStatic.MenuList[Utilities.Localization["Close TW"]] = Utilities.Localization["Developer"];
			GlobalStatic.MenuList[Utilities.Localization["Create Statistics Page"]] = Utilities.Localization["Developer"];

			//Plugin Section
			GlobalStatic.MenuList["SB Backup Script"] = Utilities.Localization["Plugin"];
			GlobalStatic.MenuList["Scan"] = "SB Backup Script";
			GlobalStatic.MenuList["View"] = "SB Backup Script";
			GlobalStatic.MenuList["ICF"] = Utilities.Localization["Plugin"];
		}

		public static void MainMenu()
		{

            Utilities.AddtoStackTrace( "UI.MainMenu()");
			LDGraphicsWindow.ExitButtonMode(GraphicsWindow.Title, "Enabled");
			GraphicsWindow.CanResize = true;
			GlobalStatic.CheckList[Utilities.Localization["Toggle Debug"]] = GlobalStatic.DebugMode;
			GlobalStatic.CheckList[Utilities.Localization["Toggle Transaction Log"]] = GlobalStatic.Transactions;

			LDGraphicsWindow.State = 2;
			GraphicsWindow.Title = GlobalStatic.Title + " ";
			GlobalStatic.DefaultFontSize = GraphicsWindow.FontSize;
			if (GlobalStatic.DebugMode == true) //Implement 
			{
				Events.LogMessage("Debug Mode is ON", "UI"); //Localize
			}
			 Primitive Sorts;
			Sorts = "1=" + Utilities.Localization["Table"] + ";2=" + Utilities.Localization["View"] + ";3=" + Utilities.Localization["Index"] + ";4=" +Utilities.Localization["Master Table"] + ";";
			if (Engines.CurrentDatabase != null)
			{
				Engines.GetSchema(Engines.CurrentDatabase);
			}
			GraphicsWindow.FontSize = 20;
			string Menu = LDControls.AddMenu(Desktop.Width * 1.5, 30, GlobalStatic.MenuList, null, GlobalStatic.CheckList);
			Shapes.Move(Shapes.AddText(Utilities.Localization["Sort"] + ":"), 990, 1) ;
			            
			int SortOffset = LDText.GetWidth(Utilities.Localization["Sort"] + ":") - LDText.GetWidth("Sort:"); //Offsets some controls when not using the default English encoding

			GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;
			GlobalStatic.ComboBox["Table"] = LDControls.AddComboBox( LDList.ToArray(GlobalStatic.List_SCHEMA_Table), 100, 100);
			GlobalStatic.ComboBox["Sorts"] = LDControls.AddComboBox(Sorts,100,100);
			GlobalStatic.ComboBox["Database"] = LDControls.AddComboBox(Engines.DB_ShortName.ToPrimitiveArray(),100,100);
			Controls.Move(GlobalStatic.ComboBox["Sorts"], 970 + 185 + SortOffset, 5);
			Controls.Move(GlobalStatic.ComboBox["Table"], 1075 + 185 + SortOffset,5);
			Controls.Move(GlobalStatic.ComboBox["Database"], 1050 + SortOffset, 5);

			Handlers.Menu(Utilities.Localization["View"]); //Virtual Call to Handler
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
            Utilities.AddtoStackTrace( "UI.GetPath()");
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
            Utilities.AddtoStackTrace( "UI.HideDisplayResults()");
			string Default_Brush = GraphicsWindow.BrushColor;
			GraphicsWindow.BrushColor = "WHITE";
			GraphicsWindow.FillRectangle(GlobalStatic.UIx - 5, 45, 320, 350);
			GraphicsWindow.BrushColor = Default_Brush;
			for (int i = 0; i < _HideDisplay.Count; i++)
			{
				Controls.HideControl(_HideDisplay[i]);
			}
		}

		public static void ShowDisplayResults()
		{
            Utilities.AddtoStackTrace( "UI.ShowDisplayResults()");
			GraphicsWindow.DrawRectangle(GlobalStatic.UIx, 50, 310, 240);
			GraphicsWindow.FontSize = 15;
			GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 52, Utilities.Localization["Display Settings"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 73, Utilities.Localization["Sort by"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 150, Utilities.Localization["Search Settings"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 180, Utilities.Localization["Search in"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 210,Utilities.Localization["Search"] + ":");
			GraphicsWindow.FontSize = 13;
			GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;
			for (int i = 0; i < _HideDisplay.Count;i++)
			{
				Controls.ShowControl(_HideDisplay[i]);
			}
		}

		public static void DisplayResults()
		{
            _Buttons.Clear(); //Clears the Dictionary to prevent errors
            Utilities.AddtoStackTrace( "UI.DisplayResults()");
			LDGraphicsWindow.Width = Desktop.Width;
			LDGraphicsWindow.Height = Desktop.Height;
			GraphicsWindow.Left = 0;
			GraphicsWindow.Top = 0;
			GlobalStatic.UIx = GlobalStatic.Listview_Width + 50;

			GraphicsWindow.DrawRectangle(GlobalStatic.UIx, 50, 310, 340);
			GraphicsWindow.FontSize = 15;
			GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 52, Utilities.Localization["Display Settings"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 73, Utilities.Localization["Sort by"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 150, Utilities.Localization["Search Settings"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 180, Utilities.Localization["Search in"]);
			GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 210, Utilities.Localization["Search"] + ":");
			GraphicsWindow.DrawText(GlobalStatic.UIx + 127, 290, Utilities.Localization["Functions"] + ":");

            //TODO implement LOG CB?

            //Sort
            GraphicsWindow.FontSize = 13;
			string AscDesc = "1=" + Utilities.Localization["Asc"] + ";2=" + Utilities.Localization["Desc"] + ";";
			GlobalStatic.ComboBox["Sort"] = LDControls.AddComboBox(Engines.Schema, 100, 100);
			GlobalStatic.ComboBox["ASCDESC"] = LDControls.AddComboBox(AscDesc, 110, 100);

            _Buttons.Add("Sort", Controls.AddButton(Utilities.Localization["Sort"], GlobalStatic.UIx + 10, 120));
           // GlobalStatic.Buttons["Sort"] = Controls.AddButton(Utilities.Localization["Sort"], GlobalStatic.UIx + 10, 120);

			Controls.Move(GlobalStatic.ComboBox["Sort"], GlobalStatic.UIx + 80, 72);
			Controls.Move(GlobalStatic.ComboBox["ASCDESC"], GlobalStatic.UIx+ 190, 72);
			Controls.SetSize(_Buttons["Sort"], 290, 25);

			LDDialogs.ToolTip(_Buttons["Sort"], "Iniates a sort based on user set parameters"); //Localize
			LDDialogs.ToolTip(GlobalStatic.ComboBox["ASCDESC"], "Sorts Ascending and Decending based on position"); //Localize

			//Search
			GlobalStatic.ComboBox["Search"] = LDControls.AddComboBox(Engines.Schema, 200, 120);
			GlobalStatic.TextBox["Search"] = Controls.AddTextBox(GlobalStatic.UIx + 100, 210);
			GlobalStatic.CheckBox["StrictSearch"] = LDControls.AddCheckBox(Utilities.Localization["Strict Search"]);
			GlobalStatic.CheckBox["InvertSearch"] = LDControls.AddCheckBox("Invert"); //Localize
            _Buttons.Add("Search", Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Search"]), GlobalStatic.UIx + 10, 260));
			//GlobalStatic.Buttons["Search"] = Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Search"]), GlobalStatic.UIx + 10, 260);

			Controls.Move(GlobalStatic.CheckBox["StrictSearch"], GlobalStatic.UIx + 20, 240);
			Controls.Move(GlobalStatic.CheckBox["InvertSearch"], GlobalStatic.UIx + 150, 240);
			Controls.Move(GlobalStatic.ComboBox["Search"], GlobalStatic.UIx + 100, 180);
			Controls.SetSize(GlobalStatic.TextBox["Search"], 200, 25);
			Controls.SetSize(_Buttons["Search"], 290, 25);

			//Functions
			GlobalStatic.ComboBox["FunctionList"] = LDControls.AddComboBox(GlobalStatic.SQLFunctionsList, 130, 100);
			Controls.Move(GlobalStatic.ComboBox["FunctionList"], GlobalStatic.UIx + 10, 310);
			GlobalStatic.ComboBox["ColumnList"] = LDControls.AddComboBox(Engines.Schema, 135, 100);
			Controls.Move(GlobalStatic.ComboBox["ColumnList"], GlobalStatic.UIx + 160, 310);

            _Buttons.Add("RunFunction", Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Run Function"]), GlobalStatic.UIx + 10, 340));
			//GlobalStatic.Buttons["RunFunction"] = Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Run Function"]), GlobalStatic.UIx + 10, 340);
			Controls.SetSize(_Buttons["RunFunction"], 290, 25);

			//Custom Query
			GlobalStatic.TextBox["CustomQuery"] = Controls.AddMultiLineTextBox(GlobalStatic.UIx, 420);
			Controls.SetSize(GlobalStatic.TextBox["CustomQuery"], 310, 150);

            _Buttons.Add("CustomQuery", Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Query"]), GlobalStatic.UIx, 580));
  			//GlobalStatic.Buttons["CustomQuery"] = Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Query"]), GlobalStatic.UIx, 580);
  			Controls.SetSize(_Buttons["CustomQuery"], 310, 25);

            _Buttons.Add("Command", Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Command"]), GlobalStatic.UIx, 580 + 35));
  			//GlobalStatic.Buttons["Command"] = Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Command"]), GlobalStatic.UIx, 580 + 35);
			Controls.SetSize(_Buttons["Command"] , 310, 25);
			LDDialogs.ToolTip(_Buttons["Command"], "Executes customized SQL command statements onto the database"); //Localize
			string CustomQueryData = "This Textbox allows you to use Custom\nSQL Queries. Remove this and type in an SQL \nstatement. \nYou also use it to export data";//Localize
			Controls.SetTextBoxText(GlobalStatic.TextBox["CustomQuery"],CustomQueryData);

            //Hide Display Results
            _HideDisplay.Clear();
            _HideDisplay.Add(GlobalStatic.ComboBox["Sort"]);
            _HideDisplay.Add(GlobalStatic.ComboBox["ASCDESC"]);
            _HideDisplay.Add(_Buttons["Sort"]);
            _HideDisplay.Add(GlobalStatic.ComboBox["Search"]);
            _HideDisplay.Add(GlobalStatic.TextBox["Search"]);
            _HideDisplay.Add(GlobalStatic.CheckBox["StrictSearch"]);
            _HideDisplay.Add(GlobalStatic.CheckBox["InvertSearch"]);
            _HideDisplay.Add(_Buttons["Search"]);
            _HideDisplay.Add(GlobalStatic.ComboBox["FunctionList"]);
            _HideDisplay.Add(GlobalStatic.ComboBox["ColumnList"]);
            _HideDisplay.Add(_Buttons["RunFunction"]);
            _HideDisplay.Add(GlobalStatic.TextBox["CustomQuery"]);
            _HideDisplay.Add(_Buttons["CustomQuery"]);
            _HideDisplay.Add(_Buttons["Command"]);
		}

		public static void Title()
		{
            Utilities.AddtoStackTrace( "UI.Title()");
			string TimeRef,SetTitle;
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

		public static void SettingsUI()//TODO: Make the Settings UI
		{
            Utilities.AddtoStackTrace( "UI.SettingsUI()");
            GraphicsWindow.Clear();
            GraphicsWindow.Title = Utilities.Localization["Settings"];

            GraphicsWindow.CanResize = false;
            LDGraphicsWindow.CancelClose = true;
            LDGraphicsWindow.ExitOnClose = false;
            LDGraphicsWindow.Closing += Events.Closing;
            LDGraphicsWindow.ExitButtonMode(Utilities.Localization["Settings"], "Disabled");

            GraphicsWindow.FontSize = 20;

            GraphicsWindow.DrawText(10, 30, Utilities.Localization["Listview Width"]);
            GlobalStatic.TextBox["Settings_Width"] = Controls.AddTextBox(200, 25);

            GraphicsWindow.DrawText(10, 65, Utilities.Localization["Listview Height"]);
            GlobalStatic.TextBox["Settings_Heigth"] = Controls.AddTextBox(200, 60);

            GraphicsWindow.DrawText(10, 100, Utilities.Localization["Extensions"]);
            GlobalStatic.TextBox["Settings_Extensions"] = Controls.AddTextBox(200, 95);

            GraphicsWindow.DrawText(10, 135, Utilities.Localization["Deliminator"]);
            GlobalStatic.TextBox["Settings_Deliminator"] = Controls.AddTextBox(200, 130);

            GraphicsWindow.DrawText(10, 165, Utilities.Localization["Language"]);
            GlobalStatic.ComboBox["Language"] = LDControls.AddComboBox(LDList.ToArray("ISO_Text"), 200, 120);
		}

		public static void CreateTableUI()//TODO: Create the "Create Table UI"
		{
            Utilities.AddtoStackTrace( "UI.CreateTableUI()");
		}

		public static void CreateTableHandler()//TODO Create the Create Table Handler
		{
            Utilities.AddtoStackTrace( "UI.CreateTableHandler()");
		}
	}

	public static class Events
	{
		public static void LogEvents() //Error Handler
		{
			LogMessage(LDEvents.LastError, Utilities.Localization["System"]);
            Utilities.AddtoStackTrace( "Events.LogEvents()");
		}
		public static void LogMessagePopUp(Primitive MessagePopUp,Primitive MessageLog, Primitive Title, Primitive Type)
		{
			GraphicsWindow.ShowMessage(MessagePopUp, Title);
			LogMessage(MessageLog, Type);
		}

		public static void LogMessage(Primitive Message, string Type) //Logs Message to all applicable locations
		{
            LogMessage(Message,Type, Utilities.StackTrace[Utilities.StackTrace.Count- 1] );
            Utilities.AddtoStackTrace( "Events.LogMessage");
		}

        static void LogMessage(Primitive Message, string Type, string Caller)
        {
            if (GlobalStatic.DebugMode == true && Type != Utilities.Localization["System"]) //Writes Log Message to TextWindow
            {
                Console.WriteLine("Log : Caller was {0}; Type {1}; Message {2} ", Caller, Type, Message);
            }

            if (Type.Equals("Debug") == true && GlobalStatic.DebugMode == false)
            {
                return;
            }
            else if (Type.Equals("PopUp") == true)
            {
                GraphicsWindow.ShowMessage(Message, Caller + "REVERT!");
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
                        Console.WriteLine("{0}",Engines.CurrentDatabase + "\n" + Message);
                    }
                }
            }
            GlobalStatic.LogNumber++;
            Utilities.AddtoStackTrace( "Events.LogMessage()");

            SBFile.AppendContents(GlobalStatic.LogCSVpath, GlobalStatic.LogNumber + "," + Clock.Date + "," + Clock.Time + "," + "\"" + LDText.Replace(GlobalStatic.Username, "\"", "\"" + "\"") + "\"" + "," + GlobalStatic.ProductID + "," + GlobalStatic.VersionID + "," + "\"" + LDText.Replace(Type, "\"", "\"" + "\"") + "\"" + "," + "\"" + LDText.Replace(Message, "\"", "\"" + "\"") + "\"");
            string LogCMD = "INSERT INTO LOG ([UTC DATE],[UTC TIME],DATE,TIME,USER,ProductID,ProductVersion,Event,Type) VALUES(DATE(),TIME(),DATE('now','localtime'),TIME('now','localtime'),'";
            LogCMD = LogCMD + GlobalStatic.Username + "','" + GlobalStatic.ProductID + "','" + GlobalStatic.VersionID + "','" + Message + "','" + Type + "');";
            Engines.Command(GlobalStatic.LogDB, LogCMD, Utilities.Localization["App"], Utilities.Localization["Auto Log"], false);
        }

		public static void Closing()
		{
            Utilities.AddtoStackTrace( "Events.Closing()");
			if (string.IsNullOrEmpty(Engines.CurrentDatabase))
			{
                LogMessage("Program Closing", Utilities.Localization["Application"]); //Localize
            } 
			else 
			{
                LogMessage("Program Closing - Closing : " + Engines.Database_Shortname , Utilities.Localization["Application"]); //Localize
            }

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
            Utilities.AddtoStackTrace( "Events.BC()");
		}

		public async static void MC() //Menu Clicked Event Handler
		{
			await Task.Run(() => { Handlers.Menu(LDControls.LastMenuItem); });
            Utilities.AddtoStackTrace( "Events.MC()");
		}

		public async static void CB()
		{
			await Task.WhenAll(Task.Run(() => { Handlers.ComboBox(LDControls.LastComboBox, LDControls.LastComboBoxIndex); }));
            Utilities.AddtoStackTrace( "Events.CB()");
		}

	}
}