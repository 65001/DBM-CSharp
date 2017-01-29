using System;
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
			LDList.Add(GlobalStatic.List_Stack_Trace, "Plugin.FindAll()");
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
		public static Primitive AutoRunFile(string URI) //IMPLEMENT 
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Plugin. AutoRunFile()");
				return Utilities.ReadFile(URI);
		}
		public static void AutoRun(Primitive ListOfPluginsToExecute) //Implement
		{ 
		LDList.Add(GlobalStatic.List_Stack_Trace, "Plugin.AutoRun()");
		}

		public static void Menu(string URI) //IMPLEMENT 
		{
		LDList.Add(GlobalStatic.List_Stack_Trace, "Plugin.Menu)");
			if (LDFile.Exists(URI) == true) 
			{
				Primitive CNTS = Utilities.ReadFile(URI);
				for (int i = 1; i < SBArray.GetItemCount(CNTS); i++)
				{

				}
			}
		}
	}
}