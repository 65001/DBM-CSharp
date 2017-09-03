using System;
using System.Threading.Tasks;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using LitDev;
using Microsoft.SmallBasic.Library;

namespace Localization
{
    class Localization
    {
        static void Main(string[] args)
        {
            LDUtilities.ShowErrors = false;
            Console.Title = "Localizer";

            Stopwatch SW = new Stopwatch();
            SW.Start();

            //TextFiles(Program.Directory + "\\Lang\\Array\\");
            TranslateDB(Program.Directory + "\\Localization.db", Microsoft.SmallBasic.Library.Program.Directory+"\\Lang\\");

            Console.WriteLine("Elapsed Time {0}", SW.ElapsedMilliseconds);
            Console.ReadLine();
        }

        static void TranslateDB(string DBPath,string LocalizationWritePath)
        {
            if (System.IO.File.Exists(DBPath) == false) throw new ArgumentException("The DBPath does not exist or is invalid");
            if(System.IO.Directory.Exists(LocalizationWritePath) == false) throw new ArgumentException("The LocalizationWritePath does not exist or is invalid");
            string Database = LDDataBase.ConnectSQLite(DBPath);
            Primitive Records = LDDataBase.Query(Database, "SELECT * FROM Localizations ORDER BY KEY ASC;", null, true);

            Dictionary<string, string> Data = new Dictionary<string, string>();
            
            //Convert Primitive Data to a Dictionary
            for (int i = 1; i <= Records.GetItemCount(); i++)
            {
                Data.Add(Records[i]["Key"], Records[i]["Localization"]);
            }

            Primitive Languages = LDTranslate.Languages();
            Primitive Index = Languages.GetAllIndices();
            List<string> SortedIndex = new List<string>();

            //Converts the Primitive Index to a List<string>
            for (int i = 1; i <= Index.GetItemCount(); i++)
            {
                SortedIndex.Add(Index[i]);
            }
            SortedIndex.Sort();

            for (int i = 0; i < SortedIndex.Count; i++)
            {
                Stopwatch SW = new Stopwatch();
                SW.Start();
                string Path = string.Format("{0}\\{1}.xml", LocalizationWritePath, SortedIndex[i]);
                TranslateLoop("en", SortedIndex[i], Data, Path);
                SW.Stop();
                Console.WriteLine(SW.ElapsedMilliseconds);
            }
        }

        static void TranslateLoop(string From, string To, Dictionary<string, string> Original, string Path)
        {
            Dictionary<string, string> Translated = new Dictionary<string, string>();

            Translated = TranslateDictionary(Original, "en", To); //Translates Dictionary

            Stopwatch IO = new Stopwatch();
            IO.Start();
            Write(Path, DictionaryToXml(Translated));
            IO.Stop();

            Console.Write("To {0} Completed IO:{1} ", To,IO.ElapsedMilliseconds);  
        }

        static Task Write(string Path, string Contents)
        {
            return Task.Run(() => System.IO.File.WriteAllText(Path, Contents));
        }

        static string DictionaryToXml(Dictionary<string, string> Translation)
        {
            const string XML_Creation_Statement = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\n";
            StringBuilder SB = new StringBuilder(3000);

            SB.Append(XML_Creation_Statement);
            SB.Append("<root>\n");
            SB.AppendFormat("\t<{0}>\n", "Language");
            foreach (var Key in Translation)
            {
                string Node = (Key.Key).Replace(" ", "_").Replace("\"", "").Replace("&", "").Replace("<", "").Replace(">", "").Replace("'", "");
                string Item = Key.Value.Replace("&", "&amp;").Replace("<", "&lt;");
                SB.AppendFormat("\t\t<{0}>{1}</{0}>\n", Node, Item);
            }
            SB.AppendFormat("\t</{0}>\n", "Language");
            SB.Append("</root>");
            return SB.ToString();
        }

        static Dictionary<string, string> TranslateDictionary(Dictionary<string,string> Data,string From,string To)
        {
            Dictionary<string, string> Results = new Dictionary<string, string>();
            var Tasks = new List<Task<string>>();
            foreach (var Type in Data)
            {
                Results.Add(Type.Key, Translate(Type.Value, From, To) );
            }
            return Results;
        }

        static string Translate(string Input, string From, string To,bool AutoHide = true)
        {
            if (From == To) return Input;
            string translation = null;
            translation = LDTranslate.Translate(Input, From, To);
            if (AutoHide == false && string.IsNullOrWhiteSpace(translation) == true) { return Input; }
            return translation;
        }

        
        static void TextFiles(string Path)
        {
            Primitive Languages = LDTranslate.Languages();
            Primitive Index = Languages.GetAllIndices();
            Dictionary<string, string> Translation = new Dictionary<string, string>();

            for (int i = 1; i < Index.GetItemCount(); i++)
            {
                string data = Translate(Languages[Index[i]], "en", Index[i],false) ;
                if (Index[i] == "en" || string.IsNullOrWhiteSpace(data) == false)
                {
                    Console.WriteLine(data);
                    Translation.Add(Index[i], data);
                }
                else
                {
                    Console.WriteLine("Hiding {0}",Index[i]);
                }
            }

            for (int i = 1; i < Index.GetItemCount(); i++)
            {
                Console.Title = Index[i] + "(" + i + "/" + Index.GetItemCount() + ")";
                StringBuilder SB = new StringBuilder();
                for (int ii = 1; ii < Index.GetItemCount(); ii++)
                {
                    if (Translation.ContainsKey(Index[ii]))
                    {
                        string Formatted = string.Format("{0}={1} {2};", Index[ii], Translation[Index[ii]], Translate(Languages[Index[ii]], "en", Index[i]));
                        SB.Append(Formatted);
                        Console.WriteLine(Formatted);
                    }
                }
                System.IO.File.WriteAllText(Path + Index[i] + ".txt", SB.ToString());
            }
        }
        
    }
}
