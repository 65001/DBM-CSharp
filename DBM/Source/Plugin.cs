/*
 * using System;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
using SBFile = Microsoft.SmallBasic.Library.File;
using System.Collections.Generic;
using System.IO;
namespace DBM
{
	
	public class Plugin
	{
		public static void FindAll()
		{
			AddtoStackTrace( "Plugin.FindAll()");
			Primitive Plugin_Files = SBFile.GetFiles(GlobalStatic.PluginPath);
			LDList.Clear(GlobalStatic.List_Mod_Path);LDList.Clear(GlobalStatic.List_Mod_Name);
			for (int i = 1; i <= SBArray.GetItemCount(Plugin_Files); i++)
			{
				if (LDFile.GetExtension(Plugin_Files) == "exe")
				{
					LDList.Add(GlobalStatic.List_Mod_Path, Plugin_Files[i]);
					LDList.Add(GlobalStatic.List_Mod_Name, LDFile.GetFile(Plugin_Files[i]));
				}
			}
		}
		public static string[] AutoRunFile(string URI) //IMPLEMENT 
		{
			AddtoStackTrace( "Plugin. AutoRunFile()");
				return Utilities.ReadFile(URI);
		}
		public static void AutoRun(string[] ListOfPluginsToExecute) //Implement
		{
			for (int i = 0; i < ListOfPluginsToExecute.Length; i++)
			{
				Console.WriteLine(ListOfPluginsToExecute[i]);
			}
		//	GraphicsWindow.ShowMessage("Currently not supported!", "");
		AddtoStackTrace( "Plugin.AutoRun()");
		}


		public static void Menu(string URI) //IMPLEMENT 
		{
		AddtoStackTrace( "Plugin.Menu)");
			Console.WriteLine("External Menu from {0} and it exists? {1}",URI,LDFile.Exists(URI));
			if (LDFile.Exists(URI) == true) 
			{
				string[] CNTS = Utilities.ReadFile(URI);
				string[][] CNTS2= new string[CNTS.Length][];
				for (int i = 0; i < CNTS.Length; i++)
				{
					CNTS2[i] = new string[1] { CNTS[i]};
					//CNTS2[i] = CNTS[i];
					for (int ii = 1; ii < CNTS2[i].Length; i++) 
					{ 
						Console.WriteLine("MENU : {0} : {1}", ii, CNTS2[i][ii]);
					}
				}
				//Array.GetItemCount(CNTS);
				//CNTS.GetLength();
				/*for (int i = 1; i <= SBArray.GetItemCount(CNTS); i++)
				{
					if ( (CNTS[i][6] == 1 || CNTS[i][6] == string.Empty) && Utilities.Localization[CNTS[i][2]] != string.Empty )
					{
						Primitive Temp = CNTS[i]; Temp = Utilities.Localization[Utilities.Localization[i][2]];
						CNTS[i] = Temp; 
					}
					if (!string.IsNullOrEmpty(Utilities.Localization[CNTS[i][4]]))
					{
						CNTS[i][4] = Utilities.Localization[CNTS[i][4]];
					}

				}
				GlobalStatic.MenuList = CNTS;

			}

		}
	}
}*/