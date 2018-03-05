using System;
using LitDev;
using Microsoft.SmallBasic.Library;

namespace DBM
{
    public class Updater
    {
        static string UpdaterDB = null;
        public static void CheckForUpdates(string downloadlocation, string URI = GlobalStatic.OnlineDB_Refrence_Location, bool UI = true)
        {
            int StackReference = Stack.Add($"Utilities.Updater.CheckForUpdates({UI})");
            if (string.IsNullOrWhiteSpace(UpdaterDB) == false || LDNetwork.DownloadFile(downloadlocation, URI) != -1)
            {
                int LatestVersion = LatestUpdate();
                int.TryParse(GlobalStatic.VersionID, out int CurrentVersion);

                string[] Locations = FetchLinks();
                string DownloadLocation = Locations[0];
                string DownloadLocation2 = Locations[1];

                if (CurrentVersion == LatestVersion && UI == true)
                {
                    GraphicsWindow.ShowMessage("There are no updates available", Language.Localization["NoUpdates"] ?? "No Updates"); //TODO LOCALIZE
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
                Settings.Load(GlobalStatic.RestoreSettings, GlobalStatic.SettingsPath);
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

        static string[] FetchLinks()
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
}
