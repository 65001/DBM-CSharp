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
using LitDev;
using Microsoft.SmallBasic.Library;

/*
	TODO : SQLITE setup
	TODO : Set it up so that if no table exists in a database then revert to sqlite_master or equivalent
		
	//TODO Replace all instances of LDLIST with an actual List?
	  TODO Replace all instances of GlobalStatic.List_DB_* ASAP.
      TODO Remove dependecey on LitDev?
	  TODO Start to use System.Version instead of an int and replace instances of GlobablStatic.VersionID to it as well
        Emulator stuff?
            Make Emulator use 
            Add entry for PRAGMA for sqlite3
        TODO Dataview for PRAGMA (ie Change Settings?)
        
        Bug Fixes:
        //TODO Investigate Export.SQL for errors?
        //TODO Investigate switiching from View to edit multiple times and crashes and the like

        //Primitive GlobalStatics to Dictionaries in the future and move to UI.
 */
//Complete Implements and Localize

namespace DBM
{
    public static class UI
    {
        private static Stopwatch StartUpStopWatch = Stopwatch.StartNew();
        private static Dictionary<string, string> _Buttons = new Dictionary<string, string>();
        private static Dictionary<string, string> _TextBox = new Dictionary<string, string>();  //TODO Implement _TextBox
        //private static Dictionary<string, string> _CheckBox = new Dictionary<string, string>(); //TODO Implement _CheckBox
        //private static Dictionary<string, string> _ComboBox = new Dictionary<string, string>(); //TODO Implement _ComboBox
        private static List<string> _HideDisplay = new List<string>();

        static Primitive MenuList,IconList;
        
        public static IReadOnlyDictionary<string, string> Buttons
        {
            get { return _Buttons; }
        }

        public static IReadOnlyDictionary<string, string> TextBox
        { get { return _TextBox; } }

        /*
        public static IReadOnlyDictionary<string, string> CheckBox
        { get { return _CheckBox; } }

        public static IReadOnlyDictionary<string, string> ComboBox
        { get { return _ComboBox; }}
        */

        public static void Main()
        {
            Utilities.AddtoStackTrace("UI.Main()");
            LDUtilities.ShowErrors = false;
            LDUtilities.ShowFileErrors = false;
            LDUtilities.ShowNoShapeErrors = false;
            LDGraphicsWindow.ExitOnClose = false;
            LDGraphicsWindow.CancelClose = true;
            
            LDGraphicsWindow.Closing += Events.Closing;
            LDEvents.Error += Events.LogEvents;
            Engines.OnGetColumnsofTable += ColumnsChanged;
            Engines.OnSchemaChange += SchemaChanged;
            Startup();
        }

        static void ColumnsChanged(object sender, EventArgs e)
        {
            LDControls.ComboBoxContent(GlobalStatic.ComboBox["Sort"], Engines.Schema);
            LDControls.ComboBoxContent(GlobalStatic.ComboBox["Search"], Engines.Schema);
            LDControls.ComboBoxContent(GlobalStatic.ComboBox["ColumnList"], Engines.Schema);
        }

        static void SchemaChanged(object sender, EventArgs e)
        {
            switch (GlobalStatic.SortBy)
            {
                case 1:
                    LDControls.ComboBoxContent(GlobalStatic.ComboBox["Table"], Engines.Tables.ToPrimitiveArray() );
                    break;
                case 2:
                    LDControls.ComboBoxContent(GlobalStatic.ComboBox["Table"], Engines.Views.ToPrimitiveArray());
                    break;
                case 3:
                    LDControls.ComboBoxContent(GlobalStatic.ComboBox["Table"], Engines.Indexes.ToPrimitiveArray());
                    break;
            }
            Title();
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
            if (GlobalStatic.EULA_Acceptance == true && GlobalStatic.EULA_UserName == GlobalStatic.UserName && GlobalStatic.EulaTest == false)
            {
                StartupGUI();
            }
            else
            {
                Settings.SaveSettings();
                EULA.UI(GlobalStatic.EULA_Text_File, 0, GlobalStatic.Title, GlobalStatic.Copyright,GlobalStatic.ProductID);
            }
            StartUpStopWatch.Stop();
            Events.LogMessage("Startup Time : " + StartUpStopWatch.ElapsedMilliseconds + " (ms)", Utilities.Localization["UI"]);
        }

        public static void StartupGUI()
        {
            Utilities.AddtoStackTrace("UI.StartupGUI()");
            ClearWindow();
            LDScrollBars.Add(GlobalStatic.Listview_Width + 200, GlobalStatic.Listview_Height);
            LDGraphicsWindow.State = 2;
            PreMainMenu();
            MainMenu();
        }

        public static void PreMainMenu()
        {
            Utilities.AddtoStackTrace("UI.PreMainMenu()");
            GraphicsWindow.FontName = "Segoe UI";

            //Main
            MenuList[Utilities.Localization["File"]] = "Main";
            MenuList[Utilities.Localization["Edit"]] = "Main";
            MenuList[Utilities.Localization["View"] + " "] = "Main";
            MenuList[Utilities.Localization["Save"]] = "Main";
            MenuList[Utilities.Localization["Import"]] = "Main";
            MenuList[Utilities.Localization["Export"]] = "Main";
            MenuList[Utilities.Localization["Settings"]] = "Main";

            //File
            MenuList[Utilities.Localization["New"]] = Utilities.Localization["File"];
            MenuList[Utilities.Localization["Open"]] = Utilities.Localization["File"];
            MenuList["Other"] = Utilities.Localization["File"]; // TODO Localize
            MenuList[Utilities.Localization["Define New Table"]] = "Other";
            MenuList[Utilities.Localization["New in Memory Db"]] = "Other";
            MenuList[Utilities.Localization["Create Statistics Page"]] = "Other";
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
            MenuList[Utilities.Localization["HTML"] + " "] = Utilities.Localization["Export"];
            MenuList["JSON"] = Utilities.Localization["Export"]; //TODO Localize
            MenuList[Utilities.Localization["SQL"] + " "] = Utilities.Localization["Export"];
            MenuList[Utilities.Localization["PXML"] + " "]= Utilities.Localization["Export"];
            MenuList["-"] = Utilities.Localization["Export"];

            //Settings
            MenuList[Utilities.Localization["Help"]] = Utilities.Localization["Settings"];
            MenuList[Utilities.Localization["About"]] = Utilities.Localization["Help"];
            MenuList[Utilities.Localization["Show Help"]] = Utilities.Localization["Help"];
            MenuList["-"] = Utilities.Localization["Help"];
            MenuList[Utilities.Localization["Settings Editor"]] = Utilities.Localization["Settings"];
            //MenuList["DB Settings"] = Utilities.Localization["Settings"]; //TODO LOCALIZE //TODO Implement
            MenuList[Utilities.Localization["Refresh Schema"]] = Utilities.Localization["Settings"];
            MenuList[Utilities.Localization["Check for Updates"]] = Utilities.Localization["Settings"];
            MenuList["-"] = Utilities.Localization["Settings"];

            //IconList[Utilities.Localization["Settings Editor"]] = LDImage.LoadSVG( GlobalStatic.AssetPath + "\\Images\\settings.svg");

            //Plugin Section
            /*
			GlobalStatic.MenuList["SB Backup Script"] = Utilities.Localization["Plugin"];
			GlobalStatic.MenuList["Scan"] = "SB Backup Script";
			GlobalStatic.MenuList["View"] = "SB Backup Script";
			GlobalStatic.MenuList["ICF"] = Utilities.Localization["Plugin"];
            */


        }

        public static void MainMenu()
        {
            Utilities.AddtoStackTrace("UI.MainMenu()");
            LDGraphicsWindow.ExitButtonMode(GraphicsWindow.Title, "Enabled");
            GraphicsWindow.CanResize = true;


            LDGraphicsWindow.State = 2;
            GraphicsWindow.Title = GlobalStatic.Title + " ";
            Primitive Sorts = "1=" + Utilities.Localization["Table"] + ";2=" + Utilities.Localization["View"] + ";3=" + Utilities.Localization["Index"] + ";4=" + Utilities.Localization["Master Table"] + ";";
            if (Engines.CurrentDatabase != null && Engines.CurrentDatabase != null)
            {
                Engines.GetSchema(Engines.CurrentDatabase);
            }
            GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize + 8;
            string Menu = LDControls.AddMenu(Desktop.Width * 1.5, 30, MenuList, IconList, null);
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

            GlobalStatic.ComboBox["Sorts"] =LDControls.AddComboBox(Sorts, 100, 100);
            GlobalStatic.ComboBox["Database"]= LDControls.AddComboBox(Engines.DB_ShortName.ToPrimitiveArray(), 100, 100);
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
            Utilities.AddtoStackTrace("UI.GetPath(" + EngineMode + ")");
            if (Program.ArgumentCount == 1 && GlobalStatic.LoadedFile == false)
            { GlobalStatic.LoadedFile = true; return Program.GetArgument(1); }
            {
                switch (EngineMode)
                {
                    case Engines.EnginesMode.SQLITE:
                        return LDDialogs.OpenFile(GlobalStatic.Extensions, GlobalStatic.LastFolder + "\\");
                    default:
                        throw new NotImplementedException();
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
            string Default_Brush = GraphicsWindow.BrushColor;
            GraphicsWindow.BrushColor = "WHITE";
            GraphicsWindow.FillRectangle(GlobalStatic.UIx - 5, 45, 320, 350);
            GraphicsWindow.BrushColor = Default_Brush;


            GraphicsWindow.DrawRectangle(GlobalStatic.UIx, 50, 310, 340);
            GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize + 3;
            GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 52, Utilities.Localization["Display Settings"]);
            GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 78, Utilities.Localization["Sort by"]);
            GraphicsWindow.DrawText(GlobalStatic.UIx + 100, 150, Utilities.Localization["Search Settings"]);
            GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 180, Utilities.Localization["Search in"]);
            GraphicsWindow.DrawText(GlobalStatic.UIx + 20, 210, Utilities.Localization["Search"] + ":");
            GraphicsWindow.DrawText(GlobalStatic.UIx + 127, 290, Utilities.Localization["Functions"] + ":");
        }

		public static void DisplayResults()
		{
            Utilities.AddtoStackTrace( "UI.DisplayResults()");
            //Clears the Dictionary to prevent errors
            _Buttons.Clear();
            _TextBox.Clear();
            /*
            _CheckBox.Clear();
            _ComboBox.Clear();
            */

            LDGraphicsWindow.Width = Desktop.Width;
			LDGraphicsWindow.Height = Desktop.Height;
			GraphicsWindow.Left = 0;
			GraphicsWindow.Top = 0;
			GlobalStatic.UIx = GlobalStatic.Listview_Width + 50;
            DisplayHelper();
            //TODO Implement LOG CB?

            //Sort
            GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize + 1;
			string AscDesc = "1=" + Utilities.Localization["Asc"] + ";2=" + Utilities.Localization["Desc"] + ";";
            GlobalStatic.ComboBox["Sort"]= LDControls.AddComboBox(Engines.Schema, 100, 100);
            GlobalStatic.ComboBox["ASCDESC"]= LDControls.AddComboBox(AscDesc, 110, 100);

            _Buttons.AddOrReplace("Sort", Controls.AddButton(Utilities.Localization["Sort"], GlobalStatic.UIx + 10, 120));

			Controls.Move(GlobalStatic.ComboBox["Sort"], GlobalStatic.UIx + 80, 77);
			Controls.Move(GlobalStatic.ComboBox["ASCDESC"], GlobalStatic.UIx+ 190, 77);
			Controls.SetSize(_Buttons["Sort"], 290, 30);

			LDDialogs.ToolTip(_Buttons["Sort"], "Iniates a sort based on user set parameters"); //Localize
			LDDialogs.ToolTip(GlobalStatic.ComboBox["ASCDESC"], "Sorts Ascending and Decending based on position"); //Localize

            //Search
            GlobalStatic.ComboBox["Search"]=LDControls.AddComboBox(Engines.Schema, 200, 120);
			_TextBox.AddOrReplace("Search",  Controls.AddTextBox(GlobalStatic.UIx + 100, 210));
            GlobalStatic.CheckBox["StrictSearch"]= LDControls.AddCheckBox(Utilities.Localization["Strict Search"]);
            GlobalStatic.CheckBox["InvertSearch"]=LDControls.AddCheckBox(Utilities.Localization["Invert"]);
            _Buttons.AddOrReplace("Search", Controls.AddButton(Utilities.Localization["Search"].ToUpper(), GlobalStatic.UIx + 10, 260));

			Controls.Move(GlobalStatic.CheckBox["StrictSearch"], GlobalStatic.UIx + 20, 240);
			Controls.Move(GlobalStatic.CheckBox["InvertSearch"], GlobalStatic.UIx + 150, 240);
			Controls.Move(GlobalStatic.ComboBox["Search"], GlobalStatic.UIx + 100, 180);

			Controls.SetSize(_TextBox["Search"], 200, 25);
			Controls.SetSize(_Buttons["Search"], 290, 30);

            //Functions
            GlobalStatic.ComboBox["FunctionList"]= LDControls.AddComboBox(Engines.Functions(Engines.CurrentEngine).ToPrimitiveArray(), 130, 100);
            GlobalStatic.ComboBox["ColumnList"] = LDControls.AddComboBox(Engines.Schema, 135, 100);

            Controls.Move(GlobalStatic.ComboBox["FunctionList"], GlobalStatic.UIx + 10, 315);
			Controls.Move(GlobalStatic.ComboBox["ColumnList"], GlobalStatic.UIx + 160, 315);

            _Buttons.AddOrReplace("RunFunction", Controls.AddButton(Utilities.Localization["Run Function"].ToUpper(), GlobalStatic.UIx + 10, 345));
			Controls.SetSize(_Buttons["RunFunction"], 290, 30);

			//Custom Query
			_TextBox["CustomQuery"] = Controls.AddMultiLineTextBox(GlobalStatic.UIx, 420);
			Controls.SetSize(_TextBox["CustomQuery"], 310, 150);

            _Buttons.AddOrReplace("Query", Controls.AddButton(Utilities.Localization["Query"].ToUpper(), GlobalStatic.UIx, 580) );
  			Controls.SetSize(_Buttons["Query"], 310, 30);

            _Buttons.AddOrReplace("Command", Controls.AddButton(Utilities.Localization["Command"].ToUpper(), GlobalStatic.UIx, 615));

			Controls.SetSize(_Buttons["Command"] , 310, 30);
			LDDialogs.ToolTip(_Buttons["Command"], "Executes customized SQL command statements onto the database"); //Localize
			string CustomQueryData = "This Textbox allows you to use Custom\nSQL Queries. Remove this and type in an SQL \nstatement. \nYou also use it to export data";//Localize
			Controls.SetTextBoxText(_TextBox["CustomQuery"],CustomQueryData);

            //Hide Display Results
            _HideDisplay.Clear();
            _HideDisplay.Add(GlobalStatic.ComboBox["Sort"]);
            _HideDisplay.Add(GlobalStatic.ComboBox["ASCDESC"]);
            _HideDisplay.Add(Buttons["Sort"]);
            _HideDisplay.Add(GlobalStatic.ComboBox["Search"]);
            _HideDisplay.Add(_TextBox["Search"]);
            _HideDisplay.Add(GlobalStatic.CheckBox["StrictSearch"]);
            _HideDisplay.Add(GlobalStatic.CheckBox["InvertSearch"]);
            _HideDisplay.Add(Buttons["Search"]);
            _HideDisplay.Add(GlobalStatic.ComboBox["FunctionList"]);
            _HideDisplay.Add(GlobalStatic.ComboBox["ColumnList"]);
            _HideDisplay.Add(Buttons["RunFunction"]);
            _HideDisplay.Add(_TextBox["CustomQuery"]);
            _HideDisplay.Add(Buttons["Query"]);
            _HideDisplay.Add(Buttons["Command"]);
		}

		public static void Title()
		{
            Utilities.AddtoStackTrace( "UI.Title()");
			string SetTitle;
			SetTitle = GlobalStatic.Title + " " + Engines.DatabaseShortname + "(" + Engines.CurrentDatabase + ") : " + Handlers.TypeofSorts[(GlobalStatic.SortBy)] + " : " + Engines.CurrentTable;
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

		public static void SettingsUI()
		{
            Utilities.AddtoStackTrace( "UI.SettingsUI()");
            ClearWindow();
            GraphicsWindow.Title = Utilities.Localization["Settings"];

            GraphicsWindow.CanResize = false;
            LDGraphicsWindow.CancelClose = true;
            LDGraphicsWindow.ExitOnClose = false;
            LDGraphicsWindow.Closing += Events.Closing;
            LDGraphicsWindow.ExitButtonMode(Utilities.Localization["Settings"], "Disabled");

            GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize + 8;

            GraphicsWindow.DrawText(10, 10, Utilities.Localization["Listview Width"]);
            _TextBox["Settings_Width"] = Controls.AddTextBox(200, 10);

            GraphicsWindow.DrawText(10, 50, Utilities.Localization["Listview Height"]);
            _TextBox["Settings_Height"] = Controls.AddTextBox(200, 50);

            GraphicsWindow.DrawText(10, 90, Utilities.Localization["Extensions"]);
            _TextBox["Settings_Extensions"] = Controls.AddTextBox(200, 90);

            GraphicsWindow.DrawText(10, 130, Utilities.Localization["Deliminator"]);
            _TextBox["Settings_Deliminator"] = Controls.AddTextBox(200, 130);

            GraphicsWindow.DrawText(10, 175, Utilities.Localization["Language"]);

            GlobalStatic.ComboBox["Language"]= LDControls.AddComboBox(Utilities.ISO_Text.ToPrimitiveArray(), 200, 120);
            Controls.Move(GlobalStatic.ComboBox["Language"], 200, 175);

            GraphicsWindow.DrawText(10, 280, Utilities.Localization["LOG CSV Path"]);
            _Buttons.AddOrReplace("Log_CSV",Controls.AddButton(Utilities.Localization["Browse"], 320, 280));

            GraphicsWindow.DrawText(10, 330, Utilities.Localization["LOG DB PATH"]);
            _Buttons.AddOrReplace("Log_DB", Controls.AddButton(Utilities.Localization["Browse"], 320, 330));

            GraphicsWindow.DrawText(10, 380, Utilities.Localization["Transaction DB Path"]);
            _Buttons.AddOrReplace("Transaction_DB", Controls.AddButton(Utilities.Localization["Browse"], 320, 380));

            for (int i = 0; i < Utilities.ISO_LangCode.Count; i++)
            {
                if (Utilities.ISO_LangCode[i] == GlobalStatic.LanguageCode)
                {
                    int Index = i + 1;
                    LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Language"], Index );
                }
            }

            _Buttons.AddOrReplace("Settings Save", Controls.AddButton(Utilities.Localization["Save and Close"], 50, 450));
            _Buttons.AddOrReplace("Settings Close", Controls.AddButton(Utilities.Localization["Close wo saving"], 50, 500));

            Controls.SetSize(_Buttons["Settings Save"], 280, 40);
            Controls.SetSize(_Buttons["Settings Close"], 280, 40);

            Controls.SetTextBoxText(_TextBox["Settings_Width"], GlobalStatic.Listview_Width);
            Controls.SetTextBoxText(_TextBox["Settings_Height"], GlobalStatic.Listview_Height);
            Controls.SetTextBoxText(_TextBox["Settings_Extensions"], GlobalStatic.Extensions);
            Controls.SetTextBoxText(_TextBox["Settings_Deliminator"], GlobalStatic.Deliminator);

            GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;

            Controls.ButtonClicked -= Events.BC;
            Controls.ButtonClicked += SettingsUIHandler;
        }

        static void SettingsUIHandler()
        {
            SettingsUIButton(Controls.LastClickedButton);
        }

        /// <summary>
        /// Clears the current window and associated UI dictionaries
        /// </summary>
        static void ClearWindow()
        {
            GraphicsWindow.Clear();
            _Buttons.Clear();
            _TextBox.Clear();
            /*
            _CheckBox.Clear();
            _ComboBox.Clear();
            */
        }

        static void SettingsUIButton(string LastClickedButton)
        {
            if (LastClickedButton == _Buttons["Settings Save"])
            {
                GlobalStatic.Settings["Listview_Width"] = Controls.GetTextBoxText(_TextBox["Settings_Width"]);
                GlobalStatic.Settings["Listview_Height"] = Controls.GetTextBoxText(_TextBox["Settings_Height"]);
                GlobalStatic.Settings["Extensions"] = Controls.GetTextBoxText(_TextBox["Settings_Extensions"]);
                GlobalStatic.Settings["Deliminator"] = Controls.GetTextBoxText(_TextBox["Settings_Deliminator"]);
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
                MenuList = null;

                Controls.ButtonClicked -= SettingsUIHandler;
                Controls.ButtonClicked += Events.BC;
                ClearWindow();
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
            ClearWindow();

            LDGraphicsWindow.CancelClose = true;
            LDGraphicsWindow.ExitOnClose = false;

            GlobalStatic.Dataview = LDControls.AddDataView((Desktop.Width-10),(Desktop.Height-100), "1=Field;2=Type;3=PK;4=AI;5=Unique;6=Not_Null;");
            GraphicsWindow.DrawText(1, 4, "Name: ");
            Controls.Move(GlobalStatic.Dataview, 1, 30);

            _TextBox["Table_Name"] = Controls.AddTextBox(50, 1);
            Controls.SetTextBoxText(_TextBox["Table_Name"], "Table1");

            _Buttons.AddOrReplace("Commit", Controls.AddButton("Commit", 250, 1));
            _Buttons.AddOrReplace("Exit", Controls.AddButton("Exit", 350, 1));

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
            string Name = Controls.GetTextBoxText(_TextBox["Table_Name"]);
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
                ClearWindow();
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
            Utilities.AddtoStackTrace("Events.LogEvents()");
		}
        
        /// <param name="UI_Message">Message Shown on the popup</param>
        /// <param name="Message">Message passed to the low</param>
        /// <param name="Title">Title of the popup</param>
        /// <param name="Type">Type</param>
		public static void LogMessagePopUp(Primitive UI_Message,Primitive Message, Primitive Title, Primitive Type)
		{
			GraphicsWindow.ShowMessage(UI_Message, Title);
			LogMessage(Message, Type);
		}

		public static void LogMessage(string Message, string Type) //Logs Message to all applicable locations
		{
            LogMessage(Message,Type, Utilities.StackTrace[Utilities.StackTrace.Count- 1] );
            Utilities.AddtoStackTrace("Events.LogMessage()");
		}

        static void LogMessage(string Message, string Type, string Caller)
        {
            Console.WriteLine("Log : Caller was : {0}; Type: {1}; Message: {2} ;", Caller, Type, Message);

            if (string.IsNullOrWhiteSpace(Type))
            {
                Type = "Unknown";
            }

            if (Type == "Debug" && GlobalStatic.DebugMode == false)
            {
                return;
            }

            if (Type == "PopUp")
            {
                GraphicsWindow.ShowMessage(Message, Caller);
            }
            else if(Message.Contains("LDDataBase.Query") == true || Message.Contains("LDDataBase.Command") == true)
            {
                Console.WriteLine("Event Logger: {0}:{1}",Engines.CurrentDatabase, Message);
                return;
            }

            Utilities.AddtoStackTrace("Events.LogMessage(" + Message +","+ Type +"," + Caller +")");

            string LogCMD = "INSERT INTO LOG ([UTC DATE],[UTC TIME],DATE,TIME,USER,ProductID,ProductVersion,Event,Type) VALUES(DATE(),TIME(),DATE('now','localtime'),TIME('now','localtime'),'" + GlobalStatic.UserName + "','"+ GlobalStatic.ProductID + "','" + GlobalStatic.VersionID + "','" + Message.Replace("\n","") + "','" + Type + "');"; ;

            Task.Run( ()=> { Engines.Command(GlobalStatic.LogDB, LogCMD, Utilities.Localization["App"], Utilities.Localization["Auto Log"], false); } );
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
            GraphicsWindow.Clear();
            GraphicsWindow.Hide();
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

        public async static void CB(string LastComboBox,int ComboBoxIndex)
        {
            await Task.Run(() => { Handlers.ComboBox(LastComboBox, ComboBoxIndex); });
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