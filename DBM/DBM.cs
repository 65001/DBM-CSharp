// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBFile = Microsoft.SmallBasic.Library.File;

/*
	Todo :
		Emulators 
		SQLITE setup
		Set it up so that if no table exists in a database then revert to sqlite_master or equivalent
		
	//TODO
		Replace all instances of LDLIST with an actual List?
		Replace all instances of GlobalStatic.List_DB_* ASAP.
		Start to use System.Version instead of an int and replace instances of GlobablStatic.VersionID to it as well
        Emulator stuff?
        //Primitive GlobalStatics to Dictionaries in the future and move to UI.
 */
//Complete Implements and Localize
namespace DBM
{
    public static class UI
    {
        private static Stopwatch StartUpStopWatch = Stopwatch.StartNew();
        private static Dictionary<string, string> _Buttons = new Dictionary<string, string>();
        private static Dictionary<string, string> _TextBox = new Dictionary<string, string>();  //TODO Implement this
        private static Dictionary<string, string> _CheckBox = new Dictionary<string, string>(); //TODO Implement this
        private static Dictionary<string, string> _ComboBox = new Dictionary<string, string>(); //TODO Implement this
        private static List<string> _HideDisplay = new List<string>();

        static readonly string IP_Address = "8.8.8.8";
        static int Ping;

        public static IReadOnlyDictionary<string, string> Buttons
        { get { return _Buttons; } }

        public static IReadOnlyDictionary<string, string> TextBox
        { get { return _TextBox; } }

        public static IReadOnlyDictionary<string, string> CheckBox
        { get { return _CheckBox; } }

        [STAThread]
        public static void Main()
        {
            Utilities.AddtoStackTrace("UI.Main()");
            LDUtilities.ShowErrors = false;
            LDUtilities.ShowFileErrors = false;
            LDUtilities.ShowNoShapeErrors = false;
            LDGraphicsWindow.ExitOnClose = false;
            LDGraphicsWindow.CancelClose = true;
            /*
            try
            {
                using (Ping PingSender = new Ping())
                {
                    PingReply PingReply = PingSender.Send(IP_Address, 100);
                    Ping = (int)PingReply.RoundtripTime;

                    if (PingReply.Status == IPStatus.Success) //Represents Network Working
                    {
                        /*
                        using (WebClient Client = new WebClient())
                        {
                            Client.DownloadFile(GlobalStatic.Online_EULA_URI, @GlobalStatic.EULA_Text_File);
                        }
                        
                    }
                }
               // GlobalStatic.EULA_Newest_Version = ((string)SBFile.ReadLine(GlobalStatic.EULA_Text_File, 1)).Trim();
            }
            catch (Exception ex)
            {
                Events.LogMessage(ex.ToString(), "System");
            }
            */
            
            LDGraphicsWindow.Closing += Events.Closing;
            LDEvents.Error += Events.LogEvents;
            Startup();
        }

        public static void Startup()
        {
            Utilities.AddtoStackTrace("UI.Startup()");
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

            Utilities.LocalizationXML(Path.Combine(GlobalStatic.LocalizationFolder, GlobalStatic.LanguageCode + ".xml"));
            Events.LogMessage(Utilities.Localization["PRGM Start"], Utilities.Localization["Application"]);

            if (Program.ArgumentCount == 1)
            {
                Engines.Load.Sqlite(GetPath(Engines.EnginesMode.SQLITE));
            }
            if (GlobalStatic.EULA_Acceptance == true && GlobalStatic.EULA_UserName == LDFile.UserName && GlobalStatic.LastVersion == int.Parse(GlobalStatic.VersionID.Replace(".", "")) && GlobalStatic.EulaTest == false)
            {
                StartupGUI();
            }
            else
            {
                Events.LogMessage("Run EULA", "Debug");
                if (GlobalStatic.DebugMode == true)
                {
                    Console.WriteLine("EULA Acceptance: {0} \n EULA UserName: {1}\n ", GlobalStatic.EULA_Acceptance, GlobalStatic.EULA_UserName);
                    //Console.WriteLine(GlobalStatic.EULA_Newest_Version + "v" + GlobalStatic.EULA_Accepted_Version);
                    Console.WriteLine("Version ID : {0} \n Eula Test {1} \n ", GlobalStatic.VersionID, GlobalStatic.EulaTest);
                }
                Settings.SaveSettings();
                EULA.UI(GlobalStatic.EULA_Text_File, Ping, GlobalStatic.Title, GlobalStatic.Copyright,GlobalStatic.ProductID);
            }
            StartUpStopWatch.Stop();
            Events.LogMessage("Startup Time: " + StartUpStopWatch.ElapsedMilliseconds + " (ms)", Utilities.Localization["UI"]);
        }

        public static void StartupGUI()
        {
            Utilities.AddtoStackTrace("UI.StartupGUI()");
            GraphicsWindow.Show();
            GraphicsWindow.Clear();
            LDScrollBars.Add(GlobalStatic.Listview_Width + 200, GlobalStatic.Listview_Height);
            LDGraphicsWindow.State = 2;
            PreMainMenu();
            MainMenu();
        }

        public static void PreMainMenu()
        {
            Utilities.AddtoStackTrace("UI.PreMainMenu()");
            GlobalStatic.DefaultFontSize = GraphicsWindow.FontSize;
            GraphicsWindow.FontName = "Segoe UI";
            Primitive MenuList = new Primitive();

            //Main
            MenuList[Utilities.Localization["File"]] = "Main";
            MenuList[Utilities.Localization["Edit"]] = "Main";
            MenuList[Utilities.Localization["View"] + " "] = "Main";
            MenuList[Utilities.Localization["Save"]] = "Main";
            MenuList[Utilities.Localization["Import"]] = "Main";
            MenuList[Utilities.Localization["Export"]] = "Main";
            MenuList[Utilities.Localization["Settings"]] = "Main";
            MenuList[Utilities.Localization["Developer"]] = "Main";
            //GlobalStatic.MenuList["Plugin"] = "Main"; //Localize

            //File
            MenuList[Utilities.Localization["New"]] = Utilities.Localization["File"];
            MenuList[Utilities.Localization["Open"]] = Utilities.Localization["File"];
            MenuList[Utilities.Localization["Define New Table"]] = Utilities.Localization["File"];
            MenuList[Utilities.Localization["New in Memory Db"]] = Utilities.Localization["File"];
            MenuList[Utilities.Localization["Create Statistics Page"]] = Utilities.Localization["File"];
            MenuList["-"] = Utilities.Localization["File"];

            //Import
            MenuList[Utilities.Localization["CSV"]] = Utilities.Localization["Import"];
            MenuList[Utilities.Localization["SQL"]] = Utilities.Localization["Import"];
            //MenuList["Converter"] = Utilities.Localization["Import"]; //Localize
            //MenuList["HTML to CSV"] = "Converter"; //Localize
            //MenuList["-"] = "Converter";
            MenuList["-"] = Utilities.Localization["Import"];

            //Export
            MenuList[Utilities.Localization["CSV"] + " "] = Utilities.Localization["Export"];
            MenuList[Utilities.Localization["SQL"] + " "] = Utilities.Localization["Export"];
            //MenuList[Utilities.Localization["PXML"] + " "]= Utilities.Localization["Export"];
            MenuList[Utilities.Localization["HTML"] + " "] = Utilities.Localization["Export"];
            MenuList["-"] = Utilities.Localization["Export"];

            //Settings
            MenuList[Utilities.Localization["Help"]] = Utilities.Localization["Settings"];
            MenuList[Utilities.Localization["About"]] = Utilities.Localization["Help"];
            MenuList[Utilities.Localization["Show Help"]] = Utilities.Localization["Help"];
            MenuList["-"] = Utilities.Localization["Help"];
            MenuList[Utilities.Localization["Settings Editor"]] = Utilities.Localization["Settings"];
            MenuList[Utilities.Localization["Toggle Debug"]] = Utilities.Localization["Settings"];
            //MenuList[Utilities.Localization["Toggle Transaction Log"]] = Utilities.Localization["Settings"];
            //MenuList["-"] = Utilities.Localization["Toggle Transaction Log"];
            MenuList[Utilities.Localization["Refresh Schema"]] = Utilities.Localization["Settings"];
            //GlobalStatic.MenuList[Utilities.Localization["Check for Updates"]] = Utilities.Localization["Settings"];
            MenuList["-"] = Utilities.Localization["Settings"];

            //Developer
            MenuList[Utilities.Localization["Stack Trace"]] = Utilities.Localization["Developer"];
            MenuList[Utilities.Localization["Close TW"]] = Utilities.Localization["Developer"];

            //Plugin Section
            /*
			GlobalStatic.MenuList["SB Backup Script"] = Utilities.Localization["Plugin"];
			GlobalStatic.MenuList["Scan"] = "SB Backup Script";
			GlobalStatic.MenuList["View"] = "SB Backup Script";
			GlobalStatic.MenuList["ICF"] = Utilities.Localization["Plugin"];
            */
            GlobalStatic.MenuList = MenuList;
        }

        public static void MainMenu()
        {
            Utilities.AddtoStackTrace("UI.MainMenu()");
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
            Primitive Sorts = "1=" + Utilities.Localization["Table"] + ";2=" + Utilities.Localization["View"] + ";3=" + Utilities.Localization["Index"] + ";4=" + Utilities.Localization["Master Table"] + ";";
            if (Engines.CurrentDatabase != null && Engines.CurrentDatabase != null)
            {
                Engines.GetSchema(Engines.CurrentDatabase);
            }
            GraphicsWindow.FontSize = 20;
            string Menu = LDControls.AddMenu(Desktop.Width * 1.5, 30, GlobalStatic.MenuList, null, GlobalStatic.CheckList);
            Shapes.Move(Shapes.AddText(Utilities.Localization["Sort"] + ":"), 990, 1);

            int SortOffset = LDText.GetWidth(Utilities.Localization["Sort"] + ":") - LDText.GetWidth("Sort:"); //Offsets some controls when not using the default English encoding

            GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;
            try
            {
                GlobalStatic.ComboBox["Table"] = LDControls.AddComboBox(Engines.Tables.ToPrimitiveArray(), 100, 100);
            }
            catch (Exception ex)
            {
                Events.LogMessage(ex.ToString(), "System");
            }
            GlobalStatic.ComboBox["Sorts"] = LDControls.AddComboBox(Sorts, 100, 100);
            GlobalStatic.ComboBox["Database"] = LDControls.AddComboBox(Engines.DB_ShortName.ToPrimitiveArray(), 100, 100);
            Controls.Move(GlobalStatic.ComboBox["Sorts"], 1155 + SortOffset, 5);
            Controls.Move(GlobalStatic.ComboBox["Table"], 1260 + SortOffset, 5);
            Controls.Move(GlobalStatic.ComboBox["Database"], 1050 + SortOffset, 5);

            Task.Run(()=>  Handlers.Menu(Utilities.Localization["View"])); //Virtual Call to Handler

            Title();

            Controls.ButtonClicked += Events.BC;
            LDControls.MenuClicked += Events.MC;
            LDControls.ComboBoxItemChanged += Events.CB;
            LDControls.ContextMenuClicked += Events.MI;
        }

        public static string GetPath(Engines.EnginesMode EngineMode)
        {
            Utilities.AddtoStackTrace("UI.GetPath()");
            if (Program.ArgumentCount == 1 && GlobalStatic.LoadedFile == false)
            { GlobalStatic.LoadedFile = true; return Program.GetArgument(1); }
            {
                switch (EngineMode)
                {
                    case Engines.EnginesMode.SQLITE:
                        return LDDialogs.OpenFile(GlobalStatic.Extensions, GlobalStatic.LastFolder + "\\");
                    default:
                        return "Currently not Supported";
                }
            }
        }

        public static void HideDisplayResults()
        {
            Utilities.AddtoStackTrace("UI.HideDisplayResults()");
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
            Utilities.AddtoStackTrace("UI.ShowDisplayResults()");
            DisplayHelper();
            GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;
            for (int i = 0; i < _HideDisplay.Count; i++)
            {
                Controls.ShowControl(_HideDisplay[i]);
            }
        }

        static void DisplayHelper()
        {
            GraphicsWindow.DrawRectangle(GlobalStatic.UIx, 50, 310, 340);
            GraphicsWindow.FontSize = 15;
            GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 52, Utilities.Localization["Display Settings"]);
            GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 73, Utilities.Localization["Sort by"]);
            GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 150, Utilities.Localization["Search Settings"]);
            GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 180, Utilities.Localization["Search in"]);
            GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 210, Utilities.Localization["Search"] + ":");
            GraphicsWindow.DrawText(GlobalStatic.UIx + 127, 290, Utilities.Localization["Functions"] + ":");
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
            DisplayHelper();
            //TODO implement LOG CB?

            //Sort
            GraphicsWindow.FontSize = 13;
			string AscDesc = "1=" + Utilities.Localization["Asc"] + ";2=" + Utilities.Localization["Desc"] + ";";
			GlobalStatic.ComboBox["Sort"] = LDControls.AddComboBox(Engines.Schema, 100, 100);
			GlobalStatic.ComboBox["ASCDESC"] = LDControls.AddComboBox(AscDesc, 110, 100);

            _Buttons.Add("Sort", Controls.AddButton(Utilities.Localization["Sort"], GlobalStatic.UIx + 10, 120));

			Controls.Move(GlobalStatic.ComboBox["Sort"], GlobalStatic.UIx + 80, 72);
			Controls.Move(GlobalStatic.ComboBox["ASCDESC"], GlobalStatic.UIx+ 190, 72);
			Controls.SetSize(_Buttons["Sort"], 290, 25);

			LDDialogs.ToolTip(_Buttons["Sort"], "Iniates a sort based on user set parameters"); //Localize
			LDDialogs.ToolTip(GlobalStatic.ComboBox["ASCDESC"], "Sorts Ascending and Decending based on position"); //Localize

			//Search
			GlobalStatic.ComboBox["Search"] = LDControls.AddComboBox(Engines.Schema, 200, 120);
			GlobalStatic.TextBox["Search"] = Controls.AddTextBox(GlobalStatic.UIx + 100, 210);
			GlobalStatic.CheckBox["StrictSearch"] = LDControls.AddCheckBox(Utilities.Localization["Strict Search"]);
			GlobalStatic.CheckBox["InvertSearch"] = LDControls.AddCheckBox(Utilities.Localization["Invert"]);
            _Buttons.Add("Search", Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Search"]), GlobalStatic.UIx + 10, 260));

			Controls.Move(GlobalStatic.CheckBox["StrictSearch"], GlobalStatic.UIx + 20, 240);
			Controls.Move(GlobalStatic.CheckBox["InvertSearch"], GlobalStatic.UIx + 150, 240);
			Controls.Move(GlobalStatic.ComboBox["Search"], GlobalStatic.UIx + 100, 180);
			Controls.SetSize(GlobalStatic.TextBox["Search"], 200, 25);
			Controls.SetSize(_Buttons["Search"], 290, 25);

			//Functions
			GlobalStatic.ComboBox["FunctionList"] = LDControls.AddComboBox(Engines.Functions(Engines.CurrentEngine), 130, 100);
			Controls.Move(GlobalStatic.ComboBox["FunctionList"], GlobalStatic.UIx + 10, 310);
			GlobalStatic.ComboBox["ColumnList"] = LDControls.AddComboBox(Engines.Schema, 135, 100);
			Controls.Move(GlobalStatic.ComboBox["ColumnList"], GlobalStatic.UIx + 160, 310);

            _Buttons.Add("RunFunction", Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Run Function"]), GlobalStatic.UIx + 10, 340));
			Controls.SetSize(_Buttons["RunFunction"], 290, 25);

			//Custom Query
			GlobalStatic.TextBox["CustomQuery"] = Controls.AddMultiLineTextBox(GlobalStatic.UIx, 420);
			Controls.SetSize(GlobalStatic.TextBox["CustomQuery"], 310, 150);

            _Buttons.Add("CustomQuery", Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Query"]), GlobalStatic.UIx, 580));
  			Controls.SetSize(_Buttons["CustomQuery"], 310, 25);

            _Buttons.Add("Command", Controls.AddButton(Text.ConvertToUpperCase(Utilities.Localization["Command"]), GlobalStatic.UIx, 580 + 35));

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
			string SetTitle;
			SetTitle = GlobalStatic.Title + " " + Engines.DatabaseShortname + "(" + Engines.CurrentDatabase + ") :" + Handlers.TypeofSorts[GlobalStatic.SortBy] + ":" + Engines.CurrentTable;
			if (string.IsNullOrEmpty(Engines.CurrentDatabase))
			{
				SetTitle = GlobalStatic.Title;
			}
			else 
			{
				switch (Engines.Types.Last())
				{
					case Engines.Type.Command:
						SetTitle += "( Command TIME : "  + Engines.Timer.Last()  + ")";
						break;
					case Engines.Type.Query:
						SetTitle += "( Query Time : " + Engines.Timer.Last() + ")";
						break;
				}
			}
			GraphicsWindow.Title = SetTitle;
		}

		public static void SettingsUI()//TODO: Make the Settings UI
		{
            _Buttons.Clear();
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
            GlobalStatic.TextBox["Settings_Height"] = Controls.AddTextBox(200, 60);

            GraphicsWindow.DrawText(10, 100, Utilities.Localization["Extensions"]);
            GlobalStatic.TextBox["Settings_Extensions"] = Controls.AddTextBox(200, 95);

            GraphicsWindow.DrawText(10, 135, Utilities.Localization["Deliminator"]);
            GlobalStatic.TextBox["Settings_Deliminator"] = Controls.AddTextBox(200, 130);

            GraphicsWindow.DrawText(10, 165, Utilities.Localization["Language"]);
            GlobalStatic.ComboBox["Language"] = LDControls.AddComboBox(Utilities.ISO_Text.ToPrimitiveArray(), 200, 120);
            Controls.Move(GlobalStatic.ComboBox["Language"], 200, 165);

            GraphicsWindow.DrawText(10, 280, Utilities.Localization["LOG CSV Path"]);
            _Buttons.Add("Log_CSV",Controls.AddButton(Utilities.Localization["Browse"], 290, 280));

            GraphicsWindow.DrawText(10, 320, Utilities.Localization["LOG DB PATH"]);
            _Buttons.Add("Log_DB", Controls.AddButton(Utilities.Localization["Browse"], 290, 320));

            GraphicsWindow.DrawText(10, 360, Utilities.Localization["Transaction DB Path"]);
            _Buttons.Add("Transaction_DB", Controls.AddButton(Utilities.Localization["Browse"], 290, 360));

            for (int i = 0; i < Utilities.ISO_LangCode.Count; i++)
            {
                if (Utilities.ISO_LangCode[i] == GlobalStatic.LanguageCode)
                {
                    int Index = i + 1;
                    LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Language"], Index );
                }
            }

            _Buttons.Add("Debug_Parser", LDControls.AddCheckBox("Debug Parser"));
            Controls.Move(_Buttons["Debug_Parser"], 10, 220);

            _Buttons.Add("Debug_Mode", LDControls.AddCheckBox("Debug Mode"));
            Controls.Move(_Buttons["Debug_Mode"], 10, 250);

            _Buttons.Add("Settings Save", Controls.AddButton(Utilities.Localization["Save and Close"], 50, 420));
            _Buttons.Add("Settings Close", Controls.AddButton(Utilities.Localization["Close wo saving"], 50, 470));

            Controls.SetSize(_Buttons["Settings Save"], 280, 35);
            Controls.SetSize(_Buttons["Settings Close"], 280, 35);

            Controls.SetTextBoxText(GlobalStatic.TextBox["Settings_Width"], GlobalStatic.Listview_Width);
            Controls.SetTextBoxText(GlobalStatic.TextBox["Settings_Height"], GlobalStatic.Listview_Height);
            Controls.SetTextBoxText(GlobalStatic.TextBox["Settings_Extensions"], GlobalStatic.Extensions);
            Controls.SetTextBoxText(GlobalStatic.TextBox["Settings_Deliminator"], GlobalStatic.Deliminator);
            LDControls.CheckBoxState(_Buttons["Debug_Parser"], GlobalStatic.DebugParser);
            LDControls.CheckBoxState(_Buttons["Debug_Mode"], GlobalStatic.DebugMode);

            GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;

            Controls.ButtonClicked -= Events.BC;
            Controls.ButtonClicked += SettingsUIHandler;
        }

        static void SettingsUIHandler()
        {
            SettingsUIButton(Controls.LastClickedButton);
        }

        static void SettingsUIButton(string LastClickedButton)
        {
            if (LastClickedButton == _Buttons["Settings Save"])
            {
                GlobalStatic.Settings["Listview_Width"] = Controls.GetTextBoxText(GlobalStatic.TextBox["Settings_Width"]);
                GlobalStatic.Settings["Listview_Height"] = Controls.GetTextBoxText(GlobalStatic.TextBox["Settings_Height"]);
                GlobalStatic.Settings["Extensions"] = Controls.GetTextBoxText(GlobalStatic.TextBox["Settings_Extensions"]);
                GlobalStatic.Settings["Deliminator"] = Controls.GetTextBoxText(GlobalStatic.TextBox["Settings_Deliminator"]);
                GlobalStatic.Settings["Language"] = Utilities.ISO_LangCode[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["Language"]) - 1];
                GlobalStatic.Settings["debug_mode"] = LDControls.CheckBoxGetState(_Buttons["Debug_Mode"]);
                GlobalStatic.Settings["debug_parser"] = LDControls.CheckBoxGetState(_Buttons["Debug_Parser"]);
                GlobalStatic.LanguageCode = GlobalStatic.Settings["Language"];
                Settings.SaveSettings();
                Settings.LoadSettings(GlobalStatic.RestoreSettings);
                Utilities.LocalizationXML(Path.Combine(GlobalStatic.LocalizationFolder, GlobalStatic.LanguageCode + ".xml"));
                SettingsUIButton(_Buttons["Settings Close"]);
                return;
            }
            else if (LastClickedButton == _Buttons["Settings Close"])
            {
                GlobalStatic.ListView = null;
                GlobalStatic.Dataview = null;
                GlobalStatic.MenuList = null;

                Controls.ButtonClicked -= SettingsUIHandler;
                Controls.ButtonClicked += Events.BC;
                GraphicsWindow.Clear();
                _Buttons.Clear();
                PreMainMenu();
                HideDisplayResults();
                MainMenu();
                Handlers.Buttons("View");
                return;
            }
        }

		public static void CreateTableUI()
		{
            Utilities.AddtoStackTrace( "UI.CreateTableUI()");
            Controls.HideControl(GlobalStatic.Dataview);
            Controls.HideControl(GlobalStatic.ListView);
            GlobalStatic.ListView = null;

            HideDisplayResults();
            GraphicsWindow.Clear();
            _Buttons.Clear();

            LDGraphicsWindow.CancelClose = true;
            LDGraphicsWindow.ExitOnClose = false;

            GlobalStatic.Dataview = LDControls.AddDataView((Desktop.Width-10),(Desktop.Height-100), "1=Field;2=Type;3=PK;4=AI;5=Unique;6=Not_Null;");
            GraphicsWindow.DrawText(1, 4, "Name: ");
            Controls.Move(GlobalStatic.Dataview, 1, 30);

            GlobalStatic.TextBox["Table_Name"] = Controls.AddTextBox(50, 1);
            Controls.SetTextBoxText(GlobalStatic.TextBox["Table_Name"], "Table1");

            _Buttons.Add("Commit", Controls.AddButton("Commit", 250, 1));
            _Buttons.Add("Exit", Controls.AddButton("Exit", 350, 1));

            LDControls.DataViewSetColumnComboBox(GlobalStatic.Dataview, 2, "1=Integer;2=Text;3=Blob;4=Real;5=Numeric;");
            for (int i = 3; i <= 6; i++)
            {
                LDControls.DataViewSetColumnCheckBox(GlobalStatic.Dataview, i);
            }

            Controls.ButtonClicked -= Events.BC;
            Controls.ButtonClicked += CreateTableHandler;
        }

		public static void CreateTableHandler()
		{
            Utilities.AddtoStackTrace( "UI.CreateTableHandler()");
            string LastButton = Controls.LastClickedButton;
            string Name = Controls.GetTextBoxText(GlobalStatic.TextBox["Table_Name"]);
            if (LastButton == _Buttons["Commit"])
            {
                if (!string.IsNullOrWhiteSpace(Name)) 
                {
                    Name = Name.Replace("[", "").Replace("]", "").Replace("\"", "");
                    int Max = LDControls.DataViewRowCount(GlobalStatic.Dataview);
                    StringBuilder Define_SQL = new StringBuilder();
                    Define_SQL.Append("CREATE TABLE \"" + Name + "\"(");
                    for(int i =1;i <= Max; i++)
                    {
                        Primitive _Data = LDControls.DataViewGetRow(GlobalStatic.Dataview, i);
                        if (!string.IsNullOrWhiteSpace(_Data[1]))
                        {
                            if(_Data[4] == true)
                            {
                                _Data[3] = true;
                            }
                            string Field = ((string)_Data[1]).Replace("[", "").Replace("]", "").Replace("\"", "");
                            Define_SQL.AppendFormat("\"{0}\" {1}",Field,(string)_Data[2]);

                            if (_Data[6] == true)
                            {
                                Define_SQL.Append(" NOT NULL");
                            }
                            if (_Data[3] == true)
                            {
                                Define_SQL.Append(" PRIMARY KEY");
                            }
                            if (_Data[4] == true)
                            {
                                Define_SQL.Append(" AUTOINCREMENT");
                            }
                            if (_Data[5] == true)
                            {
                                Define_SQL.Append(" UNIQUE");
                            }
                            if (i != Max)
                            {
                                Define_SQL.Append(",");
                            }
                            else
                            {
                                Define_SQL.Append(");");
                            }
                        }
                    }
                    //TODO Auto insert code into current database
                    if(!string.IsNullOrWhiteSpace(Engines.CurrentDatabase))
                    {
                        string Confirmation =   LDDialogs.Confirm("Do you wish to commit the following SQL:\n" + Define_SQL.ToString() + "\n to " + Engines.DB_ShortName[Engines.DB_Name.IndexOf(Engines.CurrentDatabase)], "Commit SQL");
                        if (Confirmation == "Yes")
                        {
                            Engines.Command(Engines.CurrentDatabase, Define_SQL.ToString(), GlobalStatic.UserName, "User Defining a Table", false); //Localize
                        }
                    }
                }
                else
                {
                    GraphicsWindow.ShowMessage("Table Name is not empty, please fill it!", "NAME");
                }
                return;
            }

            if (LastButton == _Buttons["Exit"])
            {
                Controls.ButtonClicked -= CreateTableHandler;
                Controls.ButtonClicked += Events.BC;
                GraphicsWindow.Clear();
                _Buttons.Clear();
                DisplayResults();
                ShowDisplayResults();
                MainMenu();
                Handlers.Buttons("View");
                return;
            } 
		}
	}

	public static class Events
	{
		public static void LogEvents() //Error Handler
		{
			LogMessage(LDEvents.LastError, Utilities.Localization["System"]);
            Utilities.AddtoStackTrace( "Events.LogEvents()");
		}
		public static void LogMessagePopUp(Primitive MessagePopup,Primitive MessageLog, Primitive Title, Primitive Type)
		{
			GraphicsWindow.ShowMessage(MessagePopup, Title);
			LogMessage(MessageLog, Type);
		}

		public static void LogMessage(Primitive Message, string Type) //Logs Message to all applicable locations
		{
            LogMessage(Message,Type, Utilities.StackTrace[Utilities.StackTrace.Count- 1] );
            Utilities.AddtoStackTrace( "Events.LogMessage");
		}

        static void LogMessage(Primitive Message, string Type, string Caller)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Log : Caller was : {0}; Type: {1}; Message: {2} ;", Caller, Type, Message);
            Console.ForegroundColor = ConsoleColor.White;
            if (string.IsNullOrEmpty(Type))
            {
                Type = "Unknown";
            }

            if (Type.Equals("Debug") == true && GlobalStatic.DebugMode == false)
            {
                return;
            }

            if (Type.Equals("PopUp") == true)
            {
                GraphicsWindow.ShowMessage(Message, Caller + "REVERT!");
            }
            else
            {
                if (GlobalStatic.DebugMode == true)
                {
                    if (Text.IsSubText(Message, "LDDataBase.Query") == true || Text.IsSubText(Message, "LDDataBase.Command") == true)
                    {
                        Console.WriteLine("{0}",Engines.CurrentDatabase + "\n" + Message);
                    }
                }
            }

            Utilities.AddtoStackTrace("Events.LogMessage()");
            GlobalStatic.LogNumber++;
            string Contents = GlobalStatic.LogNumber + "," + Clock.Date + "," + Clock.Time + "," + "\"" + GlobalStatic.UserName.Replace("\"", "\"\"") + "\"" + "," + GlobalStatic.ProductID + "," + GlobalStatic.VersionID + "," + "\"" + LDText.Replace(Type, "\"", "\"" + "\"") + "\"" + "," + "\"" + LDText.Replace(Message, "\"", "\"" + "\"") + "\"";
            try
            {
                System.IO.File.AppendAllText(GlobalStatic.LogCSVpath, Contents);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message + ex.Source);
            }
            catch (Exception ex)
            {
               LogMessagePopUp(ex.Message, ex.Message, ex.Message, "System");
            }
            string LogCMD = "INSERT INTO LOG ([UTC DATE],[UTC TIME],DATE,TIME,USER,ProductID,ProductVersion,Event,Type) VALUES(DATE(),TIME(),DATE('now','localtime'),TIME('now','localtime'),'";
            LogCMD += GlobalStatic.UserName + "','" + GlobalStatic.ProductID + "','" + GlobalStatic.VersionID + "','" + Message + "','" + Type + "');";
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
                LogMessage("Program Closing - Closing : " + Engines.DatabaseShortname , Utilities.Localization["Application"]); //Localize
            }
            Environment.Exit(0);
        }

		//The following async the Handlers class to make the code faster! Warning ! Can cause bugs!!!

        /// <summary>
        /// Buttom Clicked Event Handler
        /// </summary>
		public async static void BC()
		{
			await Task.Run(() => { Handlers.Buttons(Controls.LastClickedButton); });
            Utilities.AddtoStackTrace( "Events.BC()");
		}

        /// <summary>
        /// Menu Clicked Event Handler
        /// </summary>
        public async static void MC()
		{
			await Task.Run(() => { Handlers.Menu(LDControls.LastMenuItem); });
            Utilities.AddtoStackTrace( "Events.MC()");
		}

        /// <summary>
        /// ComboBox Changed Event Handler
        /// </summary>
		public async static void CB()
		{
			await Task.Run(() => { Handlers.ComboBox(LDControls.LastComboBox, LDControls.LastComboBoxIndex); });
            Utilities.AddtoStackTrace("Events.CB()");
		}

        /// <summary>
        /// Context Menu
        /// </summary>
        public async static void MI()
        {
            await Task.Run(() => { Handlers.ContextMenu(LDControls.LastContextControl,LDControls.LastContextItem); });
        }
	}
}