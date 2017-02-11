using System;
using Microsoft.SmallBasic.Library;
namespace DBM
{
	public class Handlers
	{
		public static void Menu(string Item) //Handles Main Menu 
		{
			//File Menu Items
			if (Item == GlobalStatic.LangList["New"])
			{ }
			else if (Item == GlobalStatic.LangList["Open"])
			{ }
			else if (Item == GlobalStatic.LangList["Define New Table"])
			{ }
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
			{}
			//Plugins

			else
			{
				GraphicsWindow.ShowMessage(Item + " does not exist in context or is not yet implemented", "Error Handlers.Menu");
			}                                       
		}

		public static void Buttons(string LastButton)
		{ 
		GraphicsWindow.ShowMessage(LastButton + " does not exist in context or is not yet implemented", "Error Handlers.Buttons");
		}
	}
}
