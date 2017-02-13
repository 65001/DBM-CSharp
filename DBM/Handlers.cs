// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using LitDev;
using Microsoft.SmallBasic.Library;
namespace DBM
{
	public class Handlers
	{
		public static void Menu(string Item) //Handles Main Menu  //Implement
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Handlers.Menu(" + Item + ")");

			//File Menu Items
			if (Item == GlobalStatic.LangList["New"])
			{
				string Path = LDDialogs.SaveFile(GlobalStatic.Extensions, GlobalStatic.LastFolder);
				if (!string.IsNullOrWhiteSpace(Path))
				{
					GlobalStatic.LastFolder = LDFile.GetFolder(Path);
					Settings.SaveSettings();
					Events.LogMessage("Created DB :" + Path, GlobalStatic.LangList["Application"]);
					UI.PreMainMenu();
					UI.MainMenu();
					LDDataBase.Connection = null;
				}
			}
			else if (Item == GlobalStatic.LangList["Open"])
			{
				GlobalStatic.ListView = null; GlobalStatic.Dataview = null;
				Settings.LoadSettings(); //Reloads Settings
				Engines.Load_DB(Engines.EnginesModes.SQLITE, UI.GetPath(4));
				Settings.SaveSettings();
				UI.PreMainMenu();
				UI.MainMenu();
			}
			else if (Item == GlobalStatic.LangList["Define New Table"])
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
			}
			else if (Item == GlobalStatic.LangList["Save"])
			{
				if (!string.IsNullOrEmpty(GlobalStatic.CurrentDatabase) && !string.IsNullOrEmpty(GlobalStatic.Dataview))
				{
					string SaveStatus = LDDataBase.SaveTable(GlobalStatic.CurrentDatabase, GlobalStatic.Dataview);
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
				if (!string.IsNullOrEmpty(GlobalStatic.CurrentDatabase))
				{
				
				}
				else
				{
					Events.LogMessage(GlobalStatic.LangList["Error No DB"], GlobalStatic.LangList["UI"]);
					GraphicsWindow.ShowMessage(GlobalStatic.LangList["Error No DB"], GlobalStatic.LangList["UI"]);
				}
			}
			//Import
			else if (Item == GlobalStatic.LangList["CSV"])
			{ }
			else if (Item == GlobalStatic.LangList["SQL"])
			{ }
			else if (Item == GlobalStatic.LangList["SQL"])
			{ }
			else if (Item == GlobalStatic.LangList["HTML to CSV"]) //Plugin
			{ }
			//Export
			else if (Item == GlobalStatic.LangList["PXML"] + " ")
			{ }
			else if (Item == GlobalStatic.LangList["HTML"] + " ")
			{ }
			//else if (Item == GlobalStatic.LangList["Export UI"])
			//{ }
			else if (Item == GlobalStatic.LangList["SQL"] + " ")
			{ }
			else if (Item == GlobalStatic.LangList["CSV"] + " ")
			{ }
			//Settings
			else if (Item == GlobalStatic.LangList["About"])
			{ }
			else if (Item == GlobalStatic.LangList["Show Help"])
			{ }
			else if (Item == GlobalStatic.LangList["Settings Editor"])
			{ }
			else if (Item == GlobalStatic.LangList["Toggle Debug"])
			{ }
			else if (Item == GlobalStatic.LangList["Toggle Transaction Log"])
			{ }
			else if (Item == GlobalStatic.LangList["Refresh Schema"])
			{ }
			else if (Item == GlobalStatic.LangList["Check for Updates"])
			{ }
			//Developer
			else if (Item == GlobalStatic.LangList["Stack Trace"])
			{ }
			else if (Item == GlobalStatic.LangList["Close TW"])
			{ }
			else if (Item == GlobalStatic.LangList["Create Statistics Page"])
			{ }
			//Plugins

			else
			{
				GraphicsWindow.ShowMessage(Item + " does not exist in context or is not yet implemented", "Error Handlers.Menu");
			}
		}

		public static void Buttons(string LastButton) //Implement
		{
			if (LastButton == GlobalStatic.Buttons["Search"])
			{
				Engines.GenerateQuery(true, true, false);
			}
			else if (LastButton == GlobalStatic.Buttons["Sort"])
			{
				Engines.GenerateQuery(false, true, false);
			}
			else if (LastButton == GlobalStatic.Buttons["RunFunction"])
			{
				Engines.GenerateQuery(false, true, true);
			}
			else if (LastButton == GlobalStatic.Buttons["CustomQuery"])
			{
				Engines.Query(GlobalStatic.CurrentDatabase, Controls.GetTextBoxText(GlobalStatic.TextBox["CustomQuery"]), GlobalStatic.ListView, false, GlobalStatic.Username, GlobalStatic.LangList["User Requested"]);
			}
			else if (LastButton == GlobalStatic.Buttons["Command"])
			{

			}
			else
			{
				GraphicsWindow.ShowMessage(Controls.GetButtonCaption(LastButton) + " does not exist in context or is not yet implemented", "Error Handlers.Buttons");
			}
		}

		public static void ComboBox(string ComboBox, string Index) //Sets GlobalStatic.SortBy
		{

		}
	}
}
