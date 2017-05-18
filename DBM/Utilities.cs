// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.Xml;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
namespace DBM
{
	public static class Utilities
	{
        static Dictionary<string, string> _Localization = new Dictionary<string, string>();
        static List<string> _StackTrace = new List<string>();
        static List<string> _ISO_Text = new List<string>();
        static List<string> _ISO_LangCode = new List<string>();
        static List<string> _UI_Name = new List<string>();
        static List<string> _UI_Action = new List<string>();
        static List<string> _UI_Handler = new List<string>();
        
        public static IReadOnlyDictionary<string, string> Localization
        {
            get { return _Localization; }
        }

        public static IReadOnlyList<string> StackTrace
        {
            get { return _StackTrace.AsReadOnly(); }
        }

        public static IReadOnlyList<string> ISO_Text
        {
            get { return _ISO_Text.AsReadOnly(); }
        }

        public static IReadOnlyList<string> ISO_LangCode
        {
            get { return _ISO_LangCode.AsReadOnly(); }
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
                string DataPath =  Path.Combine( GlobalStatic.Localization_LanguageCodes_Path , GlobalStatic.LanguageCode + ".txt");

                Primitive Localization_Temp = System.IO.File.ReadAllText(DataPath);
                string[] LocalizationFiles =  System.IO.Directory.GetFiles(LDFile.GetFolder(XMLPath));
                _ISO_Text.Clear();
                _ISO_LangCode.Clear();

                foreach (string FilePath in LocalizationFiles)
                {
                    string LanguageFile = LDFile.GetFile(FilePath);
                    _ISO_LangCode.Add(LanguageFile);
                    _ISO_Text.Add(Localization_Temp[ LanguageFile ]);
                }
			}
			else
			{
				Events.LogMessage("Localization XML Missing", "Application"); //DO NOT LOCALIZE
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

	    public static string XMLAttributes() { return "1=" + LDText.Replace(LDText.Replace(LDxml.Attributes, "=", "\\="), ";", "\\;") + ";2=" + LDxml.AttributesCount + ";3=" + LDxml.ChildrenCount + ";4=" + LDxml.NodeName + ";5=" + LDxml.NodeType + ";6=" + LDxml.NodeInnerText + ";"; }

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

		static void Add_UI_Controls(string Type, string Caption_OR_Name, string Handler, string PreviousNode_OR_ToolTip, string Action) 
		{
			int index = _UI_Name.IndexOf(Caption_OR_Name);
			if (Type == "Menu")
			{
				GlobalStatic.MenuList[Caption_OR_Name] = PreviousNode_OR_ToolTip;
				if (index == 0 || Caption_OR_Name == "-")
				{
					AddToList(Caption_OR_Name, Handler, Action);
				}
				else
				{
                    _UI_Handler[index] = Handler;
                    _UI_Action[index] = Action;
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
                    _UI_Handler[index] = Handler;
                    _UI_Action[index] = Action;
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
            _UI_Name.Add(Name);
            _UI_Action.Add(Action);
            _UI_Handler.Add(Handler);
		}
	}

    public static class Transform
    {
        public static Primitive ToPrimitiveArray<T>(this List<T> List)
        {
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i].ToString();
            }
            return _return;
        }

        public static Primitive ToPrimitiveArray(this ReadOnlyCollection<string> List)
        {
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i];
            }
            return _return;
        }

        public static Primitive ToPrimitiveArray(this IReadOnlyList<string> List)
        {
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i];
            }
            return _return;
        }

        public static Primitive ToPrimitiveArray<T>(this Dictionary<T, T> Dictionary)
        {
            StringBuilder Exporter = new StringBuilder();
            foreach (KeyValuePair<T, T> entry in Dictionary)
            {
                Exporter.Append(entry.Key + "=" + entry.Value + ";");
            }
            return Exporter.ToString();
        }

        public static void Print<T>(this List<T> List)
        {
            for (int i = 0; i < List.Count; i++)
            {
                Console.WriteLine("{0} : {1}", i, List[i]);
            }
        }

        public static void Print<T>(this IReadOnlyList<T> List)
        {
            for (int i = 0; i < List.Count; i++)
            {
                Console.WriteLine("{0} : {1}", i, List[i]);
            }
        }

        public static void Print<T>(this Dictionary<T, T> Dictionary)
        {
            foreach (KeyValuePair<T,T> entry in Dictionary)
            {
                Console.WriteLine("{0} : {1}", entry.Key, entry.Value);
            }
        }
    }
}