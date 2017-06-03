﻿// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using LitDev;
using Microsoft.SmallBasic.Library;
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

        /// <summary>
        /// Loads localized text from XML File
        /// </summary>
        /// <param name="XMLPath"></param>
        public static void LocalizationXML(string XMLPath)
		{
			AddtoStackTrace( "Utilities.LocalizationXML()");
			string XMLDoc = LDxml.Open(XMLPath);
			if (System.IO.File.Exists(XMLPath))
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
                string[] LocalizationFiles  = Directory.GetFiles(Path.GetDirectoryName(XMLPath));

                _ISO_Text.Clear();
                _ISO_LangCode.Clear();

                foreach (string FilePath in LocalizationFiles)
                {
                    string LanguageFile = Path.GetFileNameWithoutExtension(FilePath);
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
                string key = XML_Array[4].ToString().Replace("_"," ");
                string value = XML_Array[6];
                if (_Localization.ContainsKey(key) == false)
                {
                    _Localization.Add(key, value);
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

	    public static string XMLAttributes() { return "1=" + LDxml.Attributes.ToString().Replace("=", "\\=").Replace(";", "\\;") + ";2=" + LDxml.AttributesCount + ";3=" + LDxml.ChildrenCount + ";4=" + LDxml.NodeName + ";5=" + LDxml.NodeType + ";6=" + LDxml.NodeInnerText + ";"; }

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


		public static void AddControl() 
		{ 
		
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

        public static void AddOrReplace(this Dictionary<string,string> Dictionary,string Key,string Value)
        {
            if (Dictionary.ContainsKey(Key)==true)
            {
                Dictionary[Key] = Value;
                return;
            }
            Dictionary.Add(Key, Value);
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

        public static string SanitizeFieldName(this string String)
        {
            return String.Replace("\"", "").Replace("[", "").Replace("]", "");
        }

    }
}