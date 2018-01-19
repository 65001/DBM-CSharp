// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using System.Text;
using System.Diagnostics;
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
        static List<DateTime> _StackIniationTime = new List<DateTime>();
        static List<DateTime> _StackExitTime = new List<DateTime>();
        static List<TimeSpan> _StackDuration = new List<TimeSpan>();
        static List<string> _ISO_Text = new List<string>();
        static List<string> _ISO_LangCode = new List<string>();
        static List<string> _UI_Name = new List<string>();
        static List<string> _UI_Action = new List<string>();
        static List<string> _UI_Handler = new List<string>();
        static string UpdaterDB = null;
        static Stopwatch SW = new Stopwatch();

        public static IReadOnlyDictionary<string, string> Localization
        {
            get { return _Localization; }
        }

        public static IReadOnlyList<string> StackTrace
        {
            get { return _StackTrace.AsReadOnly(); }
        }

        public static IReadOnlyList<DateTime> StackIniationTime
        {
            get { return _StackIniationTime.AsReadOnly(); }
        }

        public static IReadOnlyList<DateTime> StackExitTime
        {
            get { return _StackExitTime.AsReadOnly(); }
        }

        public static IReadOnlyList<TimeSpan> StackDuration
        {
            get { return _StackDuration.AsReadOnly(); }
        }

        public static IReadOnlyList<string> ISO_Text
        {
            get { return _ISO_Text.AsReadOnly(); }
        }

        public static IReadOnlyList<string> ISO_LangCode
        {
            get { return _ISO_LangCode.AsReadOnly(); }
        }

        public static long GetStackOverHead()
        {
            return SW.ElapsedMilliseconds;
        }

        public static long GetStackOverHeadTicks()
        {
            return SW.ElapsedTicks;
        }

        /// <summary>
        /// Loads localized text from a XML File
        /// </summary>
        /// <param name="XMLPath"></param>
        public static void LocalizationXML(string XMLPath,string DataPath)
		{
			int StackReference = AddtoStackTrace("Utilities.LocalizationXML()");
           
			string XMLDoc = LDxml.Open(XMLPath);
			if (System.IO.File.Exists(XMLPath) && System.IO.File.Exists(DataPath))
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
                throw new FileNotFoundException("Localization File not found!"); //DO NOT LOCALIZE
			}
            Utilities.AddExit(StackReference);
		}

        static void AddLocalization()
        {
            Primitive XML_Array = XMLAttributes();
            string key = ((string)XML_Array[4]).Replace("_"," ");
            string value = XML_Array[6];
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value) )
            {
                throw new ArgumentException("The key or value is null." + XML_Array,$"{key} = {value};");
            }
            else if (_Localization.ContainsKey(key) == false)
            {
                _Localization.Add(key, value);
            }
            else
            {
                throw new Exception("The key : " + key +" already exists in the Localization Dictionary. " + XML_Array);
            }
        }

	    public static string XMLAttributes() { return "1= ;2=" + LDxml.AttributesCount + ";3=" + LDxml.ChildrenCount + ";4=" + LDxml.NodeName + ";5=" + LDxml.NodeType + ";6=" + LDxml.NodeInnerText + ";"; }

        public static int AddtoStackTrace(string Data)
        {
            SW.Start();
            _StackTrace.Add(Data);
            _StackIniationTime.Add(DateTime.UtcNow);
            _StackExitTime.Add(DateTime.MinValue);
            _StackDuration.Add(TimeSpan.MinValue);
            SW.Stop();
            return _StackTrace.Count - 1;
        }

        public static void AddExit(int Index)
        {

            SW.Start();
            _StackExitTime[Index] = DateTime.UtcNow;
            TimeSpan Span = _StackExitTime[Index] - _StackIniationTime[Index];
            SW.Stop();
            _StackDuration[Index] = Span;
        }

		// Reads File and Parses it
		public static string[] ReadFile(string URI) //Reads a file and ignores certain types of data
		{
			int StackReference = AddtoStackTrace("Utilities.ReadFile()");
            List<string> File_Read = new List<string>();

			if (System.IO.File.Exists(URI) == true)
			{
				string[] CNTS = System.IO.File.ReadAllLines(URI);
				int itemCount = CNTS.Length;
				for (int i = 0; i < itemCount; i++)
				{
					if (Text.StartsWith(CNTS[i], "#") == false && !string.IsNullOrWhiteSpace( CNTS[i] ) )
					{
                        File_Read.Add(CNTS[i]);
					}
						CNTS[i] = null;
				}
				string[] CNTS2 = new string[File_Read.Count];
				for (int i = 1; i < File_Read.Count; i++)
				{
					CNTS2[i] = File_Read[i];
				}
                Utilities.AddExit(StackReference);
				return CNTS2;
			}
			else 
			{
				Events.LogMessage("URI isn't accessable or incorrect Parameters given.", "Exception");
			}

            AddExit(StackReference);
			return null;
		}

        public class Updater
        {
            public static void CheckForUpdates(string downloadlocation, string URI = GlobalStatic.OnlineDB_Refrence_Location, bool UI = true)
            {
                int StackReference = AddtoStackTrace($"Utilities.Updater.CheckForUpdates({UI})");
                if (string.IsNullOrWhiteSpace(UpdaterDB) == false || LDNetwork.DownloadFile(downloadlocation, URI) != -1)
                {
                    int LatestVersion = LatestUpdate();
                    int.TryParse(GlobalStatic.VersionID, out int CurrentVersion);

                    string[] Locations = DownloadLinks();
                    string DownloadLocation = Locations[0];
                    string DownloadLocation2 = Locations[1];

                    if (CurrentVersion == LatestVersion && UI == true)
                    {
                        GraphicsWindow.ShowMessage("There are no updates available", Localization["NoUpdates"] ?? "No Updates"); //TODO LOCALIZE
                    }
                    else if (CurrentVersion > LatestVersion && UI == true)
                    {
                        GraphicsWindow.ShowMessage("You have a more recent edition of the program than that offered to the public.\nYou have version " + CurrentVersion + " while the most recent public release is version " + LatestVersion, Localization["NoUpdates"] ?? "No Updates");
                    }
                    else if (CurrentVersion < LatestVersion)
                    {
                        if (LDDialogs.Confirm($"Do you wish to download Version {LatestVersion }? You have Version {CurrentVersion}.", "Download Update") == "Yes") //TODO LOCALIZE
                        {
                            if (Download(DownloadLocation) == false)
                            {
                                Download(DownloadLocation2);
                            }
                        }
                    }
                    Primitive Temp = GlobalStatic.Settings["Updates"];
                    Temp["LastCheck"] = DateTime.Now.ToString("yyyy-MM-dd");
                    GlobalStatic.Settings["Updates"] = Temp;
                    Settings.Save();
                    Settings.Load(GlobalStatic.RestoreSettings,GlobalStatic.SettingsPath);
                }
                else
                {
                    GraphicsWindow.ShowMessage(Localization["Check Log"], Localization["Error"]);
                }
                Utilities.AddExit(StackReference);
            }

            public static int LatestUpdate()
            {
                int StackReference = Utilities.AddtoStackTrace("Updater.LatestUpdate()");
                if (string.IsNullOrWhiteSpace(UpdaterDB))
                {
                    UpdaterDB = LDDataBase.ConnectSQLite(GlobalStatic.UpdaterDBpath);
                }
                Primitive QueryItems = LDDataBase.Query(UpdaterDB, $"SELECT * FROM updates WHERE PRODUCTID = '{ GlobalStatic.ProductID }';", null, true);
                int.TryParse(QueryItems[1]["VERSION"], out int LatestVersion);
                Utilities.AddExit(StackReference);
                return LatestVersion;
            }

            public static string[] DownloadLinks()
            {
                int Stack = Utilities.AddtoStackTrace("Updater.DownloadLinks");
                if (string.IsNullOrWhiteSpace(UpdaterDB))
                {
                    UpdaterDB = LDDataBase.ConnectSQLite(GlobalStatic.UpdaterDBpath);
                }
                Primitive QueryItems = LDDataBase.Query(UpdaterDB, $"SELECT * FROM updates WHERE PRODUCTID = '{ GlobalStatic.ProductID }';", null, true);
                string[] Locations = new string[2];
                Locations[0] = QueryItems[1]["URL"];
                Locations[1] = QueryItems[1]["URL2"];
                Utilities.AddExit(Stack);
                return Locations;
            }

            static bool Download(string URL)
            {
                int Stack = AddtoStackTrace("Utilities.DownloadUpdate(" + URL + ")");
                string DownloadFolder = string.Empty;
                while (string.IsNullOrWhiteSpace(DownloadFolder) || string.IsNullOrWhiteSpace(LDFile.GetExtension(DownloadFolder)))
                {
                    GraphicsWindow.ShowMessage("You will be prompted to select the download location.", "Download Location");
                    DownloadFolder = LDDialogs.SaveFile("1=zip;", Program.Directory);
                }
                int UpdaterSize = LDNetwork.DownloadFile(DownloadFolder, URL);
                switch (UpdaterSize)
                {
                    case -1:
                        GraphicsWindow.ShowMessage(Localization["Check Log"], Localization["Error"]);
                        AddExit(Stack);
                        return false;
                    default:
                        GraphicsWindow.ShowMessage("SUCCESS", "Update Downloaded");
                        AddExit(Stack);
                        return true;
                }
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
            int Stack = Utilities.AddtoStackTrace("Transform.ListToPrimitiveArray");
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i].ToString();
            }
            Utilities.AddExit(Stack);
            return _return;
        }

        public static Primitive ToPrimitiveArray<T>(this ReadOnlyCollection<T> List)
        {
            int Stack = Utilities.AddtoStackTrace("Transform.ReadOnlyCollectionToPrimitiveArray");
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i].ToString();
            }
            Utilities.AddExit(Stack);
            return _return;
        }

        public static Primitive ToPrimitiveArray<T>(this IReadOnlyList<T> List)
        {
            int Stack = Utilities.AddtoStackTrace("Transform.IReadOnlyListToPrimitiveArray");
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i].ToString();
            }
            Utilities.AddExit(Stack);
            return _return;
        }

        public static Primitive ToPrimitiveArray<T>(this Dictionary<T, T> Dictionary)
        {
            int Stack = Utilities.AddtoStackTrace("Transform.DictionaryToPrimitiveArray");
            StringBuilder Exporter = new StringBuilder();
            foreach (KeyValuePair<T, T> entry in Dictionary)
            {
                Exporter.Append(entry.Key + "=" + entry.Value + ";");
            }
            Utilities.AddExit(Stack);
            return Exporter.ToString();
        }

        public static void AddOrReplace<T>(this Dictionary<T,T> Dictionary,T Key,T Value)
        {
            int Stack = Utilities.AddtoStackTrace("Transform.AddorReplaceDictionary");
            if (Dictionary.ContainsKey(Key)==true)
            {
                Dictionary[Key] = Value;
                Utilities.AddExit(Stack);
                return;
            }
            Dictionary.Add(Key, Value);
            Utilities.AddExit(Stack);
        }

        public static void Print<T>(this List<T> List)
        {
            int Stack = Utilities.AddtoStackTrace("Transform.PrintList");
            for (int i = 0; i < List.Count; i++)
            {
                Console.WriteLine("{0} : {1}", i, List[i]);
            }
            Utilities.AddExit(Stack);
        }

        public static void Print<T>(this IReadOnlyList<T> List)
        {
            int Stack = Utilities.AddtoStackTrace("Transform.PrintIReadOnlyList");
            for (int i = 0; i < List.Count; i++)
            {
                Console.WriteLine("{0} : {1}", i, List[i]);
            }
            Utilities.AddExit(Stack);
        }

        public static void Print<T>(this Dictionary<T, T> Dictionary)
        {
            int Stack = Utilities.AddtoStackTrace("Transform.PrintDictionary");
            foreach (KeyValuePair<T,T> entry in Dictionary)
            {
                Console.WriteLine("{0} : {1}", entry.Key, entry.Value);
            }
            Utilities.AddExit(Stack);
        }

        public static void Print<T>(this IReadOnlyDictionary<T, T> Dictionary)
        {
            int Stack = Utilities.AddtoStackTrace("Transform.PrintIReadOnlyDictionary");
            foreach (KeyValuePair<T, T> entry in Dictionary)
            {
                Console.WriteLine("{0} : {1}", entry.Key, entry.Value);
            }
            Utilities.AddExit(Stack);
        }

        public static string SanitizeFieldName(this string String)
        {
            return String?.Replace("\"", "").Replace("[", "").Replace("]", "");
        }

        public static bool IsInteger(this string text)
        {
            return int.TryParse(text, out int test);
        }

        public static bool IsDouble(this string text)
        {
            return double.TryParse(text, out double test);
        }

        public static bool IsFloat(this string text)
        {
            return float.TryParse(text, out float test);
        }
    }
}