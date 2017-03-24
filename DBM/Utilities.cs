// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.Text;
using System.Collections.Generic;
using LitDev;
using Microsoft.SmallBasic.Library;
namespace DBM
{
	public class Utilities
	{
		public static void LocalizationXML(string XMLPath)  // Loads localized text from XML File
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Utilities.LocalizationXML()");
			//string XMLPath = GlobalStatic.LocalizationFolder + GlobalStatic.LanguageCode + ".xml";
			string XMLDoc = LDxml.Open(XMLPath);
			if (LDFile.Exists(XMLPath))
			{
				LDxml.FirstNode(); LDxml.FirstChild(); LDxml.LastChild();
				Primitive XML_Array = XMLAttributes();
				if (XML_Array[1]["language"] == GlobalStatic.LanguageCode)
				{
					GlobalStatic.LangList[LDText.Replace(XML_Array[4], "_", " ")] = XML_Array[6];
				}
				else if (GlobalStatic.DebugMode == true)
				{ TextWindow.WriteLine("Rejected : " + XML_Array); }
				while (LDxml.PreviousSibling() == "SUCCESS")
				{
					XML_Array = XMLAttributes();
					if (XML_Array[1]["language"] == GlobalStatic.LanguageCode)
					{
						GlobalStatic.LangList[LDText.Replace(XML_Array[4], "_", " ")] = XML_Array[6];
					}
					else if (GlobalStatic.DebugMode == true)
					{ TextWindow.WriteLine("Rejected : " + XML_Array); }
				}
			}
			else
			{
				Events.LogMessage("Localization XML Missing", "Application");
			}
		}

		static string XMLAttributes() { return "1=" + LDText.Replace(LDText.Replace(LDxml.Attributes, "=", "\\="), ";", "\\;") + ";2=" + LDxml.AttributesCount + ";3=" + LDxml.ChildrenCount + ";4=" + LDxml.NodeName + ";5=" + LDxml.NodeType + ";6=" + LDxml.NodeInnerText + ";"; }

		// Reads File and Parses it
		public static string[] ReadFile(string URI) //Reads a file and ignores certain types of data
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Utilities.ReadFile()");
			string List_File_Read = "File_Read";

			if (System.IO.File.Exists(URI) == true)
			{
				LDList.Clear(List_File_Read);
				string[] CNTS = System.IO.File.ReadAllLines(URI);
				int itemCount = CNTS.Length;
				for (int i = 0; i < itemCount; i++)
				{
					if (Text.StartsWith(CNTS[i], "#") == false && !string.IsNullOrWhiteSpace( CNTS[i] ) )
					{
						LDList.Add(List_File_Read, CNTS[i]);
					}
						CNTS[i] = null;
				}
				string[] CNTS2 = new string[LDList.Count(List_File_Read)];
				for (int i = 1; i < LDList.Count(List_File_Read); i++)
				{
					CNTS2[i] = LDList.GetAt(List_File_Read, i);
				}
				LDList.Clear(List_File_Read);
				return CNTS2;
			}
			else 
			{
				Events.LogMessage("URI isn't accessable or incorrect Parameters given.", "Exception");
			}

			return null;
		}

		public static void Updater()  //TODO Update Functionality. Possibly make this a function?
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "Utilities.Updater()");
		}

		public static void AddMenuItem(string Caption,string Handler,string PreviousNode,string Action)
		{
			Add_UI_Controls("Menu", Caption,Handler, PreviousNode, Action);
		}

		public static void AddControl() 
		{ 
		
		}

		public static Primitive toArray(List<string> List)
		{
			StringBuilder data = new StringBuilder();
			for (int i = 0; i < List.Count; i++)
			{
				data.Append(i.ToString() + "=" + List[i] + ";");
			}
			return data.ToString();
		}

		static void Add_UI_Controls(string Type, string Caption_OR_Name, string Handler, string PreviousNode_OR_ToolTip, string Action) 
		{
			int index = LDList.IndexOf(GlobalStatic.List_UI_Name, Caption_OR_Name);
			if (Type == "Menu")
			{
				GlobalStatic.MenuList[Caption_OR_Name] = PreviousNode_OR_ToolTip;
				if (index == 0 || Caption_OR_Name == "-")
				{
					AddToList(Caption_OR_Name, Handler, Action);
				}
				else
				{
					LDList.SetAt(GlobalStatic.List_UI_Handler, index, Handler);
					LDList.SetAt(GlobalStatic.List_UI_Action, index, Action);
				}
			}
			else if (Type == "Register")
			{
				if (index == 0)
				{
					AddToList(Caption_OR_Name, Handler, Action);
				}
				else
				{
					LDList.SetAt(GlobalStatic.List_UI_Handler, index, Handler);
					LDList.SetAt(GlobalStatic.List_UI_Action, index, Action);
				}

				if (!string.IsNullOrWhiteSpace(PreviousNode_OR_ToolTip))
				{
					LDDialogs.ToolTip(Caption_OR_Name, PreviousNode_OR_ToolTip);
				}
			}
			else 
				{
				Events.LogMessage("Invalid Arguments",GlobalStatic.LangList["App"]);
				}
		}

		static void AddToList(string Name, string Handler, string Action)
		{
			LDList.Add(GlobalStatic.List_UI_Name, Name);
			LDList.Add(GlobalStatic.List_UI_Handler, Handler);
			LDList.Add(GlobalStatic.List_UI_Action, Action);
		}

	}
}