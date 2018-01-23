using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SmallBasic.Library;
using LitDev;

namespace DBM
{
    public static class Language
    {
        static Dictionary<string, string> _Localization = new Dictionary<string, string>();
        static List<string> _ISO_Text = new List<string>();
        static List<string> _ISO_LangCode = new List<string>();

        public static IReadOnlyDictionary<string, string> Localization
        {
            get { return _Localization; }
        }

        public static IReadOnlyList<string> ISO_Text
        {
            get { return _ISO_Text.AsReadOnly(); }
        }

        public static IReadOnlyList<string> ISO_LangCode
        {
            get { return _ISO_LangCode.AsReadOnly(); }
        }

        public static void Load(string File,string DataPath)
        {
            int StackReference = Stack.Add($"Language.Load({File},{DataPath})");

            string XMLDoc = LDxml.Open(File);
            if (System.IO.File.Exists(File) && System.IO.File.Exists(DataPath))
            {
                _Localization.Clear();

                LDxml.FirstNode();
                LDxml.FirstChild();
                LDxml.LastChild();

                Add();
                while (LDxml.PreviousSibling() == "SUCCESS")
                {
                    Add();
                }

                Primitive Localization_Temp = System.IO.File.ReadAllText(DataPath);
                string[] LocalizationFiles = Directory.GetFiles(Path.GetDirectoryName(File));

                _ISO_Text.Clear();
                _ISO_LangCode.Clear();

                foreach (string FilePath in LocalizationFiles)
                {
                    string LanguageFile = Path.GetFileNameWithoutExtension(FilePath);
                    _ISO_LangCode.Add(LanguageFile);
                    _ISO_Text.Add(Localization_Temp[LanguageFile]);
                }
            }
            else
            {
                throw new FileNotFoundException("Localization File not found!"); //DO NOT LOCALIZE
            }
            Stack.Exit(StackReference);
        }

        public static void Add()
        {
            Primitive XML_Array = Utilities.XMLAttributes();
            string key = ((string)XML_Array[4]).Replace("_", " ");
            string value = XML_Array[6];
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The key or value is null." + XML_Array, $"{key} = {value};");
            }

            if (_Localization.ContainsKey(key))
            {
                throw new Exception("The key : " + key + " already exists in the Localization Dictionary. " + XML_Array);
            }

            _Localization.Add(key, value);
        }
    }
}
