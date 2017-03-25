// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using LitDev;
using Microsoft.SmallBasic.Library;
namespace DBM
{
	public class Utilities
	{
        static Dictionary<string, string> _Localization = new Dictionary<string, string>();
        static List<string> _StackTrace = new List<string>();
        
        public static IReadOnlyDictionary<string, string> Localization
        {
            get { return _Localization; }
        }

        public static IReadOnlyList<string> StackTrace
        {
            get { return _StackTrace.AsReadOnly(); }
        }

        public static void LocalizationXML(string XMLPath)  // Loads localized text from XML File
		{
			AddtoStackTrace( "Utilities.LocalizationXML()");
			string XMLDoc = LDxml.Open(XMLPath);
			if (LDFile.Exists(XMLPath))
			{
                _Localization.Clear();
                LDxml.FirstNode();
                LDxml.FirstChild();
                LDxml.LastChild();

                AddLocalization();
				while (LDxml.PreviousSibling() == "SUCCESS")
				{
                    AddLocalization();
				}
			}
			else
			{
				Events.LogMessage("Localization XML Missing", "Application");
			}
		}

        static void AddLocalization()
        {
            Primitive XML_Array = XMLAttributes();
            if (XML_Array[1]["language"] == GlobalStatic.LanguageCode)
            {
                string key = LDText.Replace(XML_Array[4], "_", " ");
                string value = XML_Array[6];
                if (_Localization.ContainsKey(key) == false)
                {
                    _Localization.Add(LDText.Replace(XML_Array[4], "_", " "), XML_Array[6]);
                }
                else
                {
                    throw new Exception("The key : " + key +" already exists in the Localization Dictionary.");
                }
            }
            else if (GlobalStatic.DebugMode == true)
            {
                Console.WriteLine("Rejected: {0}", XML_Array);
            }
        }

		static string XMLAttributes() { return "1=" + LDText.Replace(LDText.Replace(LDxml.Attributes, "=", "\\="), ";", "\\;") + ";2=" + LDxml.AttributesCount + ";3=" + LDxml.ChildrenCount + ";4=" + LDxml.NodeName + ";5=" + LDxml.NodeType + ";6=" + LDxml.NodeInnerText + ";"; }

        public static void AddtoStackTrace(string Data)
        {
            _StackTrace.Add(Data);
        }


		// Reads File and Parses it
		public static string[] ReadFile(string URI) //Reads a file and ignores certain types of data
		{
			AddtoStackTrace( "Utilities.ReadFile()");
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
			AddtoStackTrace( "Utilities.Updater()");
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
            Primitive _return =null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i];
            }
            return _return;
		}

        public static Primitive toArray(ReadOnlyCollection<string> List)
        {
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i];
            }
            return _return;
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
				Events.LogMessage("Invalid Arguments",Utilities.Localization["App"]);
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