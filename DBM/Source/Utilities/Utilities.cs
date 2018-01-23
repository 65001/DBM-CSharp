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
        static List<string> _UI_Name = new List<string>();
        static List<string> _UI_Action = new List<string>();
        static List<string> _UI_Handler = new List<string>();
        static string UpdaterDB = null;
        
	    public static string XMLAttributes() { return "1= ;2=" + LDxml.AttributesCount + ";3=" + LDxml.ChildrenCount + ";4=" + LDxml.NodeName + ";5=" + LDxml.NodeType + ";6=" + LDxml.NodeInnerText + ";"; }

		// Reads File and Parses it
		public static string[] ReadFile(string URI) //Reads a file and ignores certain types of data
		{
			int StackReference = Stack.Add("Utilities.ReadFile()");
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
                Stack.Exit(StackReference);
				return CNTS2;
			}
			else 
			{
				Events.LogMessage("URI isn't accessable or incorrect Parameters given.", "Exception");
			}

            Stack.Exit(StackReference);
			return null;
		}

        public class Updater
        {
            public static void CheckForUpdates(string downloadlocation, string URI = GlobalStatic.OnlineDB_Refrence_Location, bool UI = true)
            {
                int StackReference = Stack.Add($"Utilities.Updater.CheckForUpdates({UI})");
                if (string.IsNullOrWhiteSpace(UpdaterDB) == false || LDNetwork.DownloadFile(downloadlocation, URI) != -1)
                {
                    int LatestVersion = LatestUpdate();
                    int.TryParse(GlobalStatic.VersionID, out int CurrentVersion);

                    string[] Locations = DownloadLinks();
                    string DownloadLocation = Locations[0];
                    string DownloadLocation2 = Locations[1];

                    if (CurrentVersion == LatestVersion && UI == true)
                    {
                        GraphicsWindow.ShowMessage("There are no updates available",Language.Localization["NoUpdates"] ?? "No Updates"); //TODO LOCALIZE
                    }
                    else if (CurrentVersion > LatestVersion && UI == true)
                    {
                        GraphicsWindow.ShowMessage("You have a more recent edition of the program than that offered to the public.\nYou have version " + CurrentVersion + " while the most recent public release is version " + LatestVersion, 
                            Language.Localization["NoUpdates"] ?? "No Updates");
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
                    GraphicsWindow.ShowMessage(
                        Language.Localization["Check Log"],
                        Language.Localization["Error"]);
                }
                Stack.Exit(StackReference);
            }

            public static int LatestUpdate()
            {
                int StackReference = Stack.Add("Updater.LatestUpdate()");
                if (string.IsNullOrWhiteSpace(UpdaterDB))
                {
                    UpdaterDB = LDDataBase.ConnectSQLite(GlobalStatic.UpdaterDBpath);
                }
                Primitive QueryItems = LDDataBase.Query(UpdaterDB, $"SELECT * FROM updates WHERE PRODUCTID = '{ GlobalStatic.ProductID }';", null, true);
                int.TryParse(QueryItems[1]["VERSION"], out int LatestVersion);
                Stack.Exit(StackReference);
                return LatestVersion;
            }

            public static string[] DownloadLinks()
            {
                int StackPointer = Stack.Add("Updater.DownloadLinks");
                if (string.IsNullOrWhiteSpace(UpdaterDB))
                {
                    UpdaterDB = LDDataBase.ConnectSQLite(GlobalStatic.UpdaterDBpath);
                }
                Primitive QueryItems = LDDataBase.Query(UpdaterDB, $"SELECT * FROM updates WHERE PRODUCTID = '{ GlobalStatic.ProductID }';", null, true);
                string[] Locations = new string[2];
                Locations[0] = QueryItems[1]["URL"];
                Locations[1] = QueryItems[1]["URL2"];
                Stack.Exit(StackPointer);
                return Locations;
            }

            static bool Download(string URL)
            {
                int StackPointer = Stack.Add("Utilities.DownloadUpdate(" + URL + ")");
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
                        GraphicsWindow.ShowMessage(
                            Language.Localization["Check Log"],
                            Language.Localization["Error"]);
                        Stack.Exit(StackPointer);
                        return false;
                    default:
                        GraphicsWindow.ShowMessage("SUCCESS", "Update Downloaded");
                        Stack.Exit(StackPointer);
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
            int StackPointer = Stack.Add("Transform.ListToPrimitiveArray");
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i].ToString();
            }
            Stack.Exit(StackPointer);
            return _return;
        }

        public static Primitive ToPrimitiveArray<T>(this ReadOnlyCollection<T> List)
        {
            int StackPointer = Stack.Add("Transform.ReadOnlyCollectionToPrimitiveArray");
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i].ToString();
            }
            Stack.Exit(StackPointer);
            return _return;
        }

        public static Primitive ToPrimitiveArray<T>(this IReadOnlyList<T> List)
        {
            int StackPointer = Stack.Add("Transform.IReadOnlyListToPrimitiveArray");
            Primitive _return = null;
            for (int i = 0; i < List.Count; i++)
            {
                _return[i + 1] = List[i].ToString();
            }
            Stack.Exit(StackPointer);
            return _return;
        }

        public static Primitive ToPrimitiveArray<T>(this Dictionary<T, T> Dictionary)
        {
            int StackPointer = Stack.Add("Transform.DictionaryToPrimitiveArray");
            StringBuilder Exporter = new StringBuilder();
            foreach (KeyValuePair<T, T> entry in Dictionary)
            {
                Exporter.Append(entry.Key + "=" + entry.Value + ";");
            }
            Stack.Exit(StackPointer);
            return Exporter.ToString();
        }

        public static void AddOrReplace<T>(this Dictionary<T,T> Dictionary,T Key,T Value)
        {
            int StackPointer = Stack.Add("Transform.AddorReplaceDictionary");
            if (Dictionary.ContainsKey(Key)==true)
            {
                Dictionary[Key] = Value;
                Stack.Exit(StackPointer);
                return;
            }
            Dictionary.Add(Key, Value);
            Stack.Exit(StackPointer);
        }

        public static void Print<T>(this List<T> List)
        {
            int StackPointer = Stack.Add("Transform.PrintList");
            for (int i = 0; i < List.Count; i++)
            {
                Console.WriteLine("{0} : {1}", i, List[i]);
            }
            Stack.Exit(StackPointer);
        }

        public static void Print<T>(this IReadOnlyList<T> List)
        {
            int StackPointer = Stack.Add("Transform.PrintIReadOnlyList");
            for (int i = 0; i < List.Count; i++)
            {
                Console.WriteLine("{0} : {1}", i, List[i]);
            }
            Stack.Exit(StackPointer);
        }

        public static void Print<T>(this Dictionary<T, T> Dictionary)
        {
            int StackPointer = Stack.Add("Transform.PrintDictionary");
            foreach (KeyValuePair<T,T> entry in Dictionary)
            {
                Console.WriteLine("{0} : {1}", entry.Key, entry.Value);
            }
            Stack.Exit(StackPointer);
        }

        public static void Print<T>(this IReadOnlyDictionary<T, T> Dictionary)
        {
            int StackPointer = Stack.Add("Transform.PrintIReadOnlyDictionary");
            foreach (KeyValuePair<T, T> entry in Dictionary)
            {
                Console.WriteLine("{0} : {1}", entry.Key, entry.Value);
            }
            Stack.Exit(StackPointer);
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