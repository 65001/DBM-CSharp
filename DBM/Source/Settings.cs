// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using Microsoft.SmallBasic.Library;
using LitDev;
using System.Text;
using System.IO;

namespace DBM
{
    public static class Settings
    {
        /// <summary>
        /// Loads Application Settings from text/xml file
        /// </summary>
        /// <param name="RestoreSettings"></param>
        /// <param name="SettingsPath"></param>
        public static void Load(bool RestoreSettings, string SettingsPath)
        {
            int StackReference = Stack.Add($"Settings.LoadSettings({RestoreSettings},{SettingsPath})");
            if (string.IsNullOrWhiteSpace(SettingsPath))
            {
                throw new ArgumentNullException("The Settings Path was null or whitespace");
            }

            string XMLPath = SettingsPath?.Replace(".txt", ".xml");

            if (RestoreSettings == false && (System.IO.File.Exists(SettingsPath) || System.IO.File.Exists(XMLPath)) )
            {
                if (Path.GetExtension(SettingsPath) == ".txt") //Auto Converts Text file to an XML Settings file
                {
                    if (System.IO.File.Exists(XMLPath) == false)
                    {
                        ConverttoXML(System.IO.File.ReadAllText(SettingsPath), XMLPath);
                    }
                }
            }

            //Reads Setttings from XML 
            GlobalStatic.Settings = XML(XMLPath);
            SettingsToFields(); //Bind Settings.
            Defaults();
            SettingsToFields(); //Binds Settings in the event a default has been set.
            Save(); //Save Settings in the event a value has been changed.
            Stack.Exit(StackReference);
        }

        static void Defaults()
        {
            int StackPointer = Stack.Add("Settings.Defaults");
            GlobalStatic.Settings["VersionID"] = GlobalStatic.VersionID;

            if (GlobalStatic.Listview_Width == 0)
            {
                SetSettingsValue("listview", "width", Desktop.Width - 400);
            }

            if (GlobalStatic.Listview_Height == 0)
            {
                SetSettingsValue("listview", "height", Desktop.Height - 150);
            }

            if (GlobalStatic.DefaultFontSize == 0)
            {
                GlobalStatic.Settings["fontsize"] = 12;
            }

            if (string.IsNullOrWhiteSpace(GlobalStatic.LanguageCode))
            {
                GlobalStatic.Settings["language"] = "en";
            }

            if (string.IsNullOrWhiteSpace(GlobalStatic.Deliminator))
            {
                GlobalStatic.Settings["Deliminator"] = ",";
            }

           
            if (GlobalStatic.CSVInterval <= 0)
            {
                SetSettingsValue("Intervals", "CSV", 100);
            }

            if (GlobalStatic.SQLInterval <= 0)
            {
                SetSettingsValue("Intervals", "SQL", 100);
            }

            //Directories
            //Checks to see if directories are accessible or valid by the local computer
            if (Directory.Exists(GlobalStatic.Settings["Paths"]["OS"]) == false)
            {
                SetSettingsValue("Paths", "OS", Environment.SystemDirectory);
            }
            if (Directory.Exists(GlobalStatic.Settings["Paths"]["LastFolder"]) == false)
            {
                SetSettingsValue("Paths", "LastFolder", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }
            if (Directory.Exists(GlobalStatic.Settings["Paths"]["Assets"]) == false)
            {
                SetSettingsValue("Paths", "Assets", Program.Directory + "\\Assets\\");
            }

            //Files. Tests to see if files can be accessed by the local computer.
            if (System.IO.File.Exists(GlobalStatic.Settings["Paths"]["Log"]) == false)
            {
                SetSettingsValue("Paths", "Log", Program.Directory + "\\Assets\\Log.db");
            }

            if (System.IO.File.Exists(GlobalStatic.Settings["Paths"]["Transaction"]) == false)
            {
                SetSettingsValue("Paths", "Transaction", Program.Directory + "\\Assets\\Transactions.db");
            }

            if (string.IsNullOrWhiteSpace(GlobalStatic.Settings["Extensions"]))
            {
                GlobalStatic.Settings["Extensions"] = "1=db;2=sqlite;3=sqlite3;4=db3;5=*;";
            }

            if (string.IsNullOrWhiteSpace(GlobalStatic.Settings["Updates"]["AutoUpdate"]))
            {
                SetSettingsValue("Updates", "AutoUpdate", true);
            }

            if (string.IsNullOrWhiteSpace(GlobalStatic.Settings["Updates"]["LastCheck"]))
            {
                SetSettingsValue("Updates", "LastCheck", DateTime.Now.AddDays(-14).ToString("yyyy-MM-dd"));
            }


            if (string.IsNullOrWhiteSpace(GlobalStatic.Settings["EULA"]["Signed"]))
            {
                SetSettingsValue("EULA", "Signed", false);
            }
            Stack.Exit(StackPointer);
        }

        static void SetSettingsValue<T>(string Key1,string Key2,T Value)
        {
            Primitive Temp = GlobalStatic.Settings[Key1];
            Temp[Key2] = Value.ToString();
            GlobalStatic.Settings[Key1] = Temp;
        }

        /// <summary>
        /// Binds Primitive GlobalStatic.Settings to GlobalStatic.SettingKey. 
        /// </summary>
        static void SettingsToFields()
        {
            int StackPointer = Stack.Add("Settings.SettingsToFields");
            //Intervals
            GlobalStatic.CSVInterval = GlobalStatic.Settings["Intervals"]["CSV"];
            GlobalStatic.SQLInterval = GlobalStatic.Settings["Intervals"]["SQL"];
            //Listview
            GlobalStatic.Listview_Width = (int?)GlobalStatic.Settings["listview"]["Width"] ?? GlobalStatic.Settings["Listview_Width"];
            GlobalStatic.Listview_Height = (int?)GlobalStatic.Settings["listview"]["Height"] ?? GlobalStatic.Settings["Listview_Height"];

            //Paths
            GlobalStatic.LastFolder = (string)GlobalStatic.Settings["Paths"]["LastFolder"] ?? (string)GlobalStatic.Settings["LastFolder"];
            GlobalStatic.AssetPath = (string)GlobalStatic.Settings["Paths"]["Assets"] ?? (string)GlobalStatic.Settings["Asset_Dir"];
            GlobalStatic.LogDBpath = (string)GlobalStatic.Settings["Paths"]["Log"]  ?? (string)GlobalStatic.Settings["Log_DB_Path"];
            GlobalStatic.TransactionDBPath = (string)GlobalStatic.Settings["Paths"]["Transaction"]  ?? (string)GlobalStatic.Settings["Transaction_DB"];
            //Transactions
            GlobalStatic.Transaction_Query = (bool?)GlobalStatic.Settings["Transactions"]["Query"]  ?? GlobalStatic.Settings["Transaction_Query"];
            GlobalStatic.Transaction_Commands = (bool?)GlobalStatic.Settings["Transactions"]["Commands"]  ?? GlobalStatic.Settings["Transaction_Commands"];
            //Updates
            GlobalStatic.AutoUpdate = (bool?)GlobalStatic.Settings["Updates"]["AutoUpdate"]  ?? GlobalStatic.Settings["AutoUpdate"];
            string LastUpdate = (string)GlobalStatic.Settings["Updates"]["LastCheck"]  ?? (string)GlobalStatic.Settings["LastUpdateCheck"] ?? DateTime.Now.AddDays(-14).ToString("yyyy-MM-dd");

            int.TryParse(LastUpdate.ToString().Replace("-", "") , out int LastAutoUpdate);
            int.TryParse(DateTime.Now.ToString("yyyyMMdd"), out int Today);

            GlobalStatic.ISO_Today = Today;
            GlobalStatic.LastUpdateCheck = LastAutoUpdate;

            GlobalStatic.Extensions = (string)GlobalStatic.Settings["Extensions"] ?? "1=db;2=sqlite;3=sqlite3;4=db3;5=*;";
            GlobalStatic.Deliminator = (string)GlobalStatic.Settings["Deliminator"] ?? ",";
            GlobalStatic.LanguageCode = ((string)GlobalStatic.Settings["language"] ?? GlobalStatic.Settings["Language"]);
            GlobalStatic.DefaultFontSize = (int?)GlobalStatic.Settings["fontsize"] ?? GlobalStatic.Settings["Font_Size"];

            GlobalStatic.EULA_Acceptance = (bool?)GlobalStatic.Settings["EULA"]["Signed"] ?? (bool?)GlobalStatic.Settings["EULA"] ?? false;
            GlobalStatic.EULA_UserName = (string)GlobalStatic.Settings["EULA"]["Signer"] ?? (string)GlobalStatic.Settings["EULA_By"] ?? null;
            Stack.Exit(StackPointer);
        }

		public static void Save()
		{
            int StackReference = Stack.Add("Settings.SaveSettings()");
            try
            {
                ConverttoXML(GlobalStatic.Settings, GlobalStatic.SettingsPath.Replace(".txt", ".xml"));
            }
            catch (Exception) //Settings could not be saved for some reason!
            {
                Events.LogMessage(Utilities.Localization["Failed Save Settings"], Utilities.Localization["UI"]);
            }
            Stack.Exit(StackReference);
		}

        /// <summary>
        /// Converts a text settings file to an XML settings file.
        /// </summary>
        static void ConverttoXML(Primitive Settings, string URI)
        {
            int StackPointer = Stack.Add("Settings.ConvertToXML");
            try
            {
                System.IO.File.WriteAllText(URI,ConverttoXML(Settings));
            }
            catch (Exception)
            {
                GraphicsWindow.ShowMessage(Utilities.Localization["Failed Save Settings"], Utilities.Localization["Error"]);
            }
            Stack.Exit(StackPointer);
        }

        /// <summary>
        /// Converts a text settings file to XML.
        /// </summary>
        static string ConverttoXML(Primitive Settings)
        {
            int StackPointer = Stack.Add("Settings.ConvertToXML");
            StringBuilder SB = new StringBuilder();
            SB.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            SB.AppendLine("<root>");
            SB.AppendLine("\t<!-- Details whether or not a transaction is stored in the transaction database -->");
            SB.AppendLine("\t<Transactions>");
            SB.AppendFormat("\t\t<Query>{0}</Query>\n", (bool?)Settings["Transactions"]["Query"] ?? Settings["Transaction_Query"]);
            SB.AppendFormat("\t\t<Commands>{0}</Commands>\n", (bool?)Settings["Transactions"]["Commands"] ?? Settings["Transaction_Commands"]);
            SB.AppendLine("\t</Transactions>\n");
            SB.AppendLine("\t<!-- List of all paths that the Program uses.-->");
            SB.AppendLine("\t<!-- Warning: Change this at your own risk! It may break the application!-->");
            SB.AppendLine("\t<Paths>");
            SB.AppendFormat("\t\t<OS>{0}</OS>\n", (string)Settings["Paths"]["OS"] ?? Settings["OS_Dir"]);

            SB.AppendFormat("\t\t<LastFolder>{0}</LastFolder>\n", (string)Settings["Paths"]["LastFolder"] ?? Settings["LastFolder"]);
            SB.AppendFormat("\t\t<Assets>{0}</Assets>\n", (string)Settings["Paths"]["Assets"] ?? Settings["Asset_Dir"]);
            SB.AppendFormat("\t\t<Log>{0}</Log>\n", (string)Settings["Paths"]["Log"] ?? Settings["Log_DB_Path"]);
            SB.AppendFormat("\t\t<Transaction>{0}</Transaction>\n", (string)Settings["Paths"]["Transaction"] ?? Settings["Transaction_DB"]);
            SB.AppendLine("\t</Paths>\n");

            SB.AppendLine("\t<!-- Set AutoUpdate to False if Internet Access is restricted or unavailable.-->");
            SB.AppendLine("\t<Updates>");
            SB.AppendFormat("\t\t<LastCheck>{0}</LastCheck>\n", (string)Settings["Updates"]["LastCheck"] ?? Settings["LastUpdateCheck"]);
            SB.AppendFormat("\t\t<AutoUpdate>{0}</AutoUpdate>\n", (string)Settings["Updates"]["AutoUpdate"] ?? Settings["AutoUpdate"]);
            SB.AppendLine("\t</Updates>\n");

            SB.AppendLine("\t<EULA>");
            SB.AppendFormat("\t\t<Signer>{0}</Signer>\n", (string)Settings["EULA"]["Signer"] ?? Settings["EULA_By"]);
            SB.AppendFormat("\t\t<Signed>{0}</Signed>\n", (string)Settings["EULA"]["Signed"] ?? Settings["EULA"]);
            SB.AppendLine("\t</EULA>\n");

            SB.AppendFormat("\t<Deliminator>{0}</Deliminator>\n", Settings["Deliminator"]);
            SB.AppendFormat("\t<VersionID>{0}</VersionID>\n", Settings["VersionID"]);
            SB.AppendFormat("\t<Extensions>{0}</Extensions>\n", Settings["Extensions"]);
            SB.AppendLine("	<!-- UI Related Stuff -->");
            SB.AppendFormat("\t<fontsize>{0}</fontsize>\n", (string)Settings["fontsize"] ?? Settings["Font_Size"]);
            SB.AppendFormat("\t<language>{0}</language>\n", (string)Settings["language"] ?? Settings["Language"]);
            SB.AppendLine("\t<listview>");
            SB.AppendFormat("\t\t<width>{0}</width>\n", (int?)Settings["listview"]["width"] ?? Settings["Listview_Width"]);
            SB.AppendFormat("\t\t<height>{0}</height>\n", (int?)Settings["listview"]["height"] ?? Settings["Listview_Height"]);
            SB.AppendLine("\t</listview>");

            SB.AppendLine("\t<Intervals>");
            SB.AppendFormat($"\t\t<CSV>{Settings["Intervals"]["CSV"]}</CSV>\n");
            SB.AppendFormat($"\t\t<SQL>{Settings["Intervals"]["SQL"]}</SQL>\n");
            SB.AppendLine("\t</Intervals>");
            SB.Append("</root>");
            Stack.Exit(StackPointer);
            return SB.ToString();
        }


        static Primitive XML(string URI)
        {
            int StackPointer = Stack.Add("Settings.XML");
            LDxml.Open(URI);
            Primitive Data = LDxml.ToArray();
            Data = Data["root"]["children"];
            Primitive RData = null;
            for (int i = 1; i <= Data.GetItemCount(); i++)
            {
                Primitive indicies = Data[i].GetAllIndices();
                if (Data[i][indicies[1]]["Data"] != string.Empty)
                {
                    RData[indicies[1]] = Data[i][indicies[1]]["Data"];
                }
                else if (Data[i][indicies[1]]["Children"] != string.Empty)
                {
                    Primitive Temp = null;
                    for (int ii = 1; ii <= Data[i][indicies[1]]["Children"].GetItemCount(); ii++)
                    {
                        Primitive Index = Data[i][indicies[1]]["Children"][ii].GetAllIndices();
                        Temp[Index[1]] += Data[i][indicies[1]]["Children"][ii][Index[1]]["Data"];
                    }
                    RData[indicies[1]] = Temp;
                }
                else
                {
                    //GraphicsWindow.ShowMessage(Data[i], "XML");
                }
            }
            Stack.Exit(StackPointer);
            return RData;
        }

        /// <summary>
        /// Automatically creates Directories if they do not exist.
        /// </summary>
		public static void Paths(string AssetPath,string PluginPath,string LocalizationFolder,string AutoRunPluginPath,string Localization_LanguageCodes_Path,string AutoRunPluginMessage)
		{
            int StackPointer = Stack.Add("Settings.Paths()");
			if (Directory.Exists(AssetPath) == false || Directory.Exists(LocalizationFolder) == false) //Creates Folders if one is missing
			{
				Directory.CreateDirectory(AssetPath);
				Directory.CreateDirectory(LocalizationFolder);
				Directory.CreateDirectory(Localization_LanguageCodes_Path);
			}

			if (System.IO.File.Exists(AutoRunPluginPath) == false)
			{
				System.IO.File.WriteAllText(AutoRunPluginPath, AutoRunPluginMessage);
			}
            Stack.Exit(StackPointer);
		}

        /// <summary>
        /// Connects to and creates Log and Transaction databases with default tables.
        /// </summary>
		public static void IniateDatabases()
		{
            int StackPointer = Stack.Add("Settings.IniateDatabases()");
            if (string.IsNullOrWhiteSpace(GlobalStatic.TransactionDBPath) || string.IsNullOrWhiteSpace(GlobalStatic.LogDBpath))
            {
                throw new ArgumentNullException("Transaction or Log Path is null");
            }
            GlobalStatic.TransactionDB =Engines.Load.Sqlite(GlobalStatic.TransactionDBPath,"Transaction Log");
            GlobalStatic.LogDB = Engines.Load.Sqlite(GlobalStatic.LogDBpath, "Master Log");
            
			Engines.Command(GlobalStatic.LogDB, GlobalStatic.LOGSQL, GlobalStatic.UserName, "Auto Creation Statements");
			Engines.Command(GlobalStatic.LogDB, GlobalStatic.LOGSQLVIEW , GlobalStatic.UserName, "Auto Creation Statements");
			Engines.Command(GlobalStatic.TransactionDB, GlobalStatic.TransactionsSQL , GlobalStatic.UserName, "Auto Creation Statements");
            Stack.Exit(StackPointer);
        }
	}
}