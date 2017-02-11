using System;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
using SBFile = Microsoft.SmallBasic.Library.File;
using System.Collections.Generic;
using System.IO;
/*
	Completed Items:
		Settings ; EULA; Logs ;Main;
	To do :
		Load 
		Startup GUI
		Pre Main Menu
		Plugin Stuff
		Emulators 
		SQLITE setup
 */

//Complete Implements and Localize
namespace DBM
{
	[SmallBasicType]
	public static class UI
	{
		public static String StartUpStopWatch = LDStopwatch.Add();
		public static Primitive StartTime = Clock.ElapsedMilliseconds;

		public static void Main()
		{
			/* //Example Code Code
			GraphicsWindow.Show();Controls.AddButton(" ", 1, 1);
			Controls.ButtonClicked += Events.BC;
			*/
			LDStopwatch.Start(StartUpStopWatch);
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
			Utilities.LocalizationXML();
			Events.LogMessage(GlobalStatic.LangList["PRGM Start"], GlobalStatic.LangList["Application"]);
			if (Program.ArgumentCount == 1)
			{
				Engines.Load_DB(4, GetPath(4) );
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
					Console.WriteLine(GlobalStatic.EULA_Acceptance);
					Console.WriteLine(GlobalStatic.EULA_Username);
					Console.WriteLine(GlobalStatic.EULA_Newest_Version + "v" + GlobalStatic.EULA_Accepted_Version);
					Console.WriteLine(GlobalStatic.VersionID);
					Console.WriteLine(GlobalStatic.EulaTest);
				}
				Settings.SaveSettings();
				EULA.UI(GlobalStatic.EULA_Text_File);
			}

			LDStopwatch.Stop(StartUpStopWatch);
			GraphicsWindow.ShowMessage(LDStopwatch.ElapsedMilliseconds(StartUpStopWatch), "");
		}

		public static void StartupGUI()
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.StartupGUI()");
			GraphicsWindow.Clear(); GraphicsWindow.Hide(); GraphicsWindow.Show();
			LDScrollBars.Add(GlobalStatic.Listview_Width + 500, GlobalStatic.Listview_Height);
			LDGraphicsWindow.State = 2;
			PreMainMenu();
			//Plugin.AutoRun( Plugin.AutoRunFile(GlobalStatic.AutoRunPluginPath) );
			Events.LogMessage("Startup Time: " + (Clock.ElapsedMilliseconds - UI.StartTime) + " (ms)", GlobalStatic.LangList["UI"]);
			MainMenu();
		}

		public static void PreMainMenu() //Implement //Defines Buttons
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.PreMainMenu()");
			GlobalStatic.DefaultFontSize = GraphicsWindow.FontSize;
			//Main
			GlobalStatic.MenuList[GlobalStatic.LangList["File"]] = "Main";
			GlobalStatic.MenuList[GlobalStatic.LangList["Edit"]] = "Main";
			GlobalStatic.MenuList[GlobalStatic.LangList["View"]] = "Main";
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

			//Plugin.Menu(GlobalStatic.External_Menu_Items_Path);
		}

		public static void MainMenu() //Implement
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
			Sorts = "1=" + GlobalStatic.LangList["Table"] + ";2= " + GlobalStatic.LangList["View"] + ";3=" + GlobalStatic.LangList["Index"] + ";4=" +GlobalStatic.LangList["Master Table"] + ";";
			if (GlobalStatic.CurrentDatabase != null)
			{
				Engines.GetSchema(GlobalStatic.CurrentDatabase);
			}
			GraphicsWindow.FontSize = 20;
			string Menu = LDControls.AddMenu(Desktop.Width * 1.5, 30, GlobalStatic.MenuList, null, GlobalStatic.CheckList);
			Shapes.Move(Shapes.AddText(GlobalStatic.LangList["Sort"] + ":"), 990, 1) ;
			            
			int SortOffset = LDText.GetWidth(GlobalStatic.LangList["Sort"] + ":");

			GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;
			LDControls.MenuClicked += Events.MC;

			GlobalStatic.ComboBox["Table"] = LDControls.AddComboBox(LDList.ToArray(GlobalStatic.List_SCHEMA_Table), 100, 100);


			Controls.Move(GlobalStatic.ComboBox["Table"], 1075 + 185 + SortOffset, 1);

			//Test Code
			//string Listview = LDDataBase.AddListView( GlobalStatic.Listview_Width, GlobalStatic.Listview_Height);
			//string Path = "C:\\Users\\Abhishek\\Documents\\Proggraming\\SB\\Projects\\DB Manager\\Assets\\Test DB\\Test 2.db";
			//string TestDB = Engines.Load_DB(4,Path);
			//TextWindow.WriteLine(TestDB+":"+ Path +":" + LDFile.Exists(Path));
			//Primitive start = Clock.ElapsedMilliseconds;
			//Engines.Query(TestDB, "Select * From Majestic_million", Listview, false, null, null);
			//TextWindow.WriteLine("Completed : " + ( Clock.ElapsedMilliseconds - start ) );
		}

		static string GetPath(int EngineMode)
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.GetPath()");
			if (Program.ArgumentCount == 1 && GlobalStatic.LoadedFile == false)
			{ GlobalStatic.LoadedFile = true; return Program.GetArgument(1); }
			{
				if (EngineMode == 4)
				{ return LDDialogs.OpenFile(GlobalStatic.Extensions, GlobalStatic.LastFolder + "\\"); }
				return "Currently not Supported";
			}
		}

		static void HideDisplayResults() //Implement
		{
		LDList.Add(GlobalStatic.List_Stack_Trace, "UI.HideDisplayResults()");
		}
		static void ShowDisplayResults()//Implement
		{
		LDList.Add(GlobalStatic.List_Stack_Trace, "UI.ShowDisplayResults()");
		}
		static void DisplayResults()//Implement
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "UI.DisplayResults()");
		}

		static void SettingsUI()//Implement
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "UI.SettingsUI()");
		}

		static void CreateTableUI()//Implement
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "UI.CreateTableUI()");
		}

		static void CreateTableHandler()//Implement
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "UI.CreateTableHandler()");
		}

		static void Title()//Implement
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "UI.Title()");
		}

	}

	public static class Events
	{
		public static void LogEvents() //Error Handler
		{
			LogMessage(LDEvents.LastError, GlobalStatic.LangList["System"]);
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.LogEvents()");
		}

		public static void LogMessage(Primitive Message, string Type) //Logs Message to all applicable locations
		{
			if (GlobalStatic.DebugMode == true && Type != GlobalStatic.LangList["System"]) //Writes Log Message to TextWindow
			{
				TextWindow.WriteLine(LDList.GetAt(GlobalStatic.List_Stack_Trace, LDList.Count(GlobalStatic.List_Stack_Trace)) + ":" + Type + ":" + Message);
			}

			if (Type.Equals("Debug") == true && GlobalStatic.DebugMode == false) { }
			else
			{
				if (string.IsNullOrEmpty(Type)) { Type = "Unknown"; }
				if (GlobalStatic.DebugMode == true)
				{
					if (Text.IsSubText(Message, "LDDataBase.Query") == true || Text.IsSubText(Message, "LDDataBase.Command") == true) 
					{
						TextWindow.WriteLine(GlobalStatic.CurrentDatabase + "\n" + Message);
					}
				}
			}
			GlobalStatic.LogNumber = GlobalStatic.LogNumber + 1;
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.LogMessage()");

			SBFile.AppendContents(GlobalStatic.LogCSVpath,GlobalStatic.LogNumber +"," + Clock.Date + "," + Clock.Time +"," + "\"" + LDText.Replace( GlobalStatic.Username , "\"" , "\"" + "\"" ) + "\"" +"," + GlobalStatic.ProductID +","+ GlobalStatic.VersionID+"," + "\"" +  LDText.Replace( Type, "\"" , "\"" + "\"") + "\"" +","+  "\"" +  LDText.Replace(Message ,"\"" , "\"" + "\"" ) + "\"");
			string LogCMD;
			LogCMD = "INSERT INTO LOG ([UTC DATE],[UTC TIME],DATE,TIME,USER,ProductID,ProductVersion,Event,Type) VALUES(DATE(),TIME(),DATE('now','localtime'),TIME('now','localtime'),'";
				LogCMD = LogCMD + GlobalStatic.Username + "','" + GlobalStatic.ProductID + "','" + GlobalStatic.VersionID + "','" + Message + "','" + Type + "');";
			Engines.Command(GlobalStatic.LogDB, LogCMD, GlobalStatic.LangList["App"], GlobalStatic.LangList["Auto Log"],false);

		}
		public static void BC()
		{
			TextWindow.WriteLine(Controls.LastClickedButton);
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.BC()");
		}

		public static void MC() //Menu Clicked Event Handler
		{
			Handlers.Menu(LDControls.LastMenuItem);
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.BC()");
		}

		public static void Closing() //Implement
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.Closing()");
		}

	}
}