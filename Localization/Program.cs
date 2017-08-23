using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using LitDev;
using Microsoft.SmallBasic.Library;

namespace Localization
{
    class Program
    {
        static void Main(string[] args)
        {
            TextFiles(Microsoft.SmallBasic.Library.Program.Directory + "\\Lang\\Array\\");
            //TranslateDB(Microsoft.SmallBasic.Library.Program.Directory + "\\Localization.db", Microsoft.SmallBasic.Library.Program.Directory+"\\Lang\\");
        }

        static void TranslateDB(string DBPath,string LocalizationWritePath)
        {
            if (System.IO.File.Exists(DBPath) == false) throw new ArgumentException("The DBPath does not exist or is invalid");
            if(System.IO.Directory.Exists(LocalizationWritePath) == false) throw new ArgumentException("The LocalizationWritePath does not exist or is invalid");
            string Database = LDDataBase.ConnectSQLite(DBPath);
            Console.WriteLine(Database);
            Primitive Records = LDDataBase.Query(Database, "SELECT * FROM Localizations;", null, true);

            Dictionary<string, string> Data = new Dictionary<string, string>();
            //Convert Primitive Data to a Dictionary
            for (int i = 1; i < Records.GetItemCount(); i++)
            {
                Data.Add(Records[i]["Key"], Records[i]["Localization"]);
            }

            Primitive Languages = LDTranslate.Languages();
            Primitive Index = Languages.GetAllIndices();
            //Translate all languages into a in memory dictionary
            List<Dictionary<string, string>> TranslationDictionary = new List<Dictionary<string, string>>();

            for (int i = 1; i < Index.GetItemCount(); i++)
            {
                Console.Write((string)Languages[Index[i]]);
                Stopwatch SW = new Stopwatch();
                SW.Start();
                TranslationDictionary.Add(TranslateDictionary(Data, "en", Index[i]));
                SW.Stop();
                Console.WriteLine($" {SW.ElapsedMilliseconds}");
            }

            for (int i = 0; i < TranslationDictionary.Count; i++)
            {
                Console.WriteLine("Writing {0}", Languages[Index[i + 1]]);
                string Path = LocalizationWritePath + "\\" + Index[i + 1] + ".xml";
                string TranslationXML = DictionaryToXml(TranslationDictionary[i]);
                System.IO.File.WriteAllText(Path, TranslationXML);
            }

            Console.ReadKey();
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
            foreach (var Type in Data)
            {
                Results.Add(Type.Key, Translate(Type.Value, From, To));
            }
            return Results;
        }

        static string Translate(string Input, string From, string To)
        {
            string translation = null;
            translation = LDTranslate.Translate(Input, From, To);
            if (string.IsNullOrWhiteSpace(translation)) { return Input; }
            return translation;
        }

        
        static void TextFiles(string Path)
        {
            Primitive Languages = LDTranslate.Languages();
            Primitive Index = Languages.GetAllIndices();
            Dictionary<string, string> Translation = new Dictionary<string, string>();

            for (int i = 1; i < Index.GetItemCount(); i++)
            {
                Translation.Add(Index[i], Translate(Languages[Index[i]], "en", Index[i]));
            }

            for (int i = 1; i < Index.GetItemCount(); i++)
            {
                Console.Title = Index[i] + "(" + i + "/" + Index.GetItemCount() + ")";
                StringBuilder SB = new StringBuilder();
                for (int ii = 1; ii < Index.GetItemCount(); ii++)
                {
                    string Formatted = string.Format("{0}={1} {2};", Index[ii], Translation[Index[ii]] , Translate( Languages[Index[ii]],"en",Index[i] )   );
                    SB.Append(Formatted);
                    Console.WriteLine(Formatted);
                }
                System.IO.File.WriteAllText(Path + Index[i] + ".txt", SB.ToString());
            }
        }
        
    }
}
