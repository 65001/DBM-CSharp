using System;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
using SBFile = Microsoft.SmallBasic.Library.File;
using System.Collections.Generic;
using System.IO;
/*
	Completed Items:
		Settings ; EULA;
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
		public static Primitive StartTime = Clock.ElapsedMilliseconds;

		public static void Main()
		{
			/* //Example Code Code
			GraphicsWindow.Show();Controls.AddButton(" ", 1, 1);
			Controls.ButtonClicked += Events.BC;
			*/
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.Main()");
			Primitive[] Startime2 = new Primitive[10];
			Startime2[0] = Clock.ElapsedMilliseconds;

			LDUtilities.ShowErrors = false;
			LDUtilities.ShowFileErrors = false;
			LDUtilities.ShowNoShapeErrors = false;
			LDGraphicsWindow.ExitOnClose = false;
			LDGraphicsWindow.CancelClose = true;

			LDGraphicsWindow.Closing += Events.Closing;
			LDEvents.Error += DBM.Events.LogEvents;

			GlobalStatic.Ping = LDNetwork.Ping(GlobalStatic.IP_Ping_Address, 500);
			if (GlobalStatic.Ping != -1)
			{
				Startime2[1] = Clock.ElapsedMilliseconds;
				LDNetwork.DownloadFile(GlobalStatic.EULA_Text_File, GlobalStatic.Online_EULA_URI);
				//TextWindow.WriteLine("Download Time : " + (Clock.ElapsedMilliseconds - Startime2[1]));
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
			Plugin.FindAll();
			Utilities.LocalizationXML();
			Events.LogMessage(GlobalStatic.LangList["PRGM Start"], GlobalStatic.LangList["Application"]);
			if (Program.ArgumentCount == 1)
			{
				Engines.Load_DB(4, GetPath(4));
			}
			if (GlobalStatic.EULA_Acceptance == true && GlobalStatic.EULA_Username == LDFile.UserName && GlobalStatic.LastVersion == GlobalStatic.VersionID && GlobalStatic.EulaTest == false)
			{
				Events.LogMessage("Start up GUI", "Debug"); StartupGUI();
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
		}

		public static void StartupGUI()
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.StartupGUI()");
			GraphicsWindow.Clear(); GraphicsWindow.Hide(); GraphicsWindow.Show();
			LDScrollBars.Add(GlobalStatic.Listview_Width + 500, GlobalStatic.Listview_Height);
			LDGraphicsWindow.State = 2;
			PreMainMenu();
			Plugin.AutoRun(Plugin.AutoRunFile(GlobalStatic.AutoRunPluginPath));
			Events.LogMessage("Startup Time: " + (Clock.ElapsedMilliseconds - UI.StartTime) + " (ms)", GlobalStatic.LangList["UI"]);
			MainMenu();
		}

		public static void PreMainMenu() //Implement //Defines Buttons
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.PreMainMenu()");
			//string Button_View = GlobalStatic.LangList["View"];
		}

		public static void MainMenu() //Implement
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "UI.MainMenu()");
			LDGraphicsWindow.ExitButtonMode(GraphicsWindow.Title, "Enabled");
			GraphicsWindow.CanResize = true;

			if (GlobalStatic.DebugMode == true) //Implement
				{ }
			else if (GlobalStatic.DebugMode == false)//Implement
				{ }
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
		public static void LogEvents()
		{
			//TextWindow.WriteLine("Error : " + LDEvents.LastError);
			Events.LogMessage(LDEvents.LastError, "Error");
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.LogEvents()");
		}
		public static void LogMessage(Primitive Message, Primitive Type) //Logs Message to all applicable locations
		{
			
			TextWindow.WriteLine(LDList.GetAt(GlobalStatic.List_Stack_Trace, LDList.Count(GlobalStatic.List_Stack_Trace))+":"+ Type + ":" + Message);
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.LogMessage()");
		}
		public static void BC()
		{
			TextWindow.WriteLine(Controls.LastClickedButton);
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.BC()");
		}
		public static void Closing() //Implement
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Events.Closing()");
		}
	}
}