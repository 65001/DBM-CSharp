// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
// Created : 3/14/2017 5:55 PM 2017314 17:55:44
using System;
using System.Text;
using System.IO;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
using LitDev;
namespace DBM
{
    public class Export
    {
        public static Primitive Generate2DArray(string Database, string SQL)
        {
            return Engines.Query(Database, SQL, null, true, GlobalStatic.UserName, ""); //TODO
        }

        public static Primitive Generate2DArrayFromTable(string Database, string Table)
        {
            return Generate2DArray(Database, "Select * From " + Table);
        }

        public static Primitive Generate2DArrayFromCurrentTable()
        {
            return Generate2DArrayFromTable(Engines.CurrentDatabase, Engines.CurrentTable);
        }
    
        public static void CSV(Primitive Data,Primitive Schema,string FilePath) //TODO
        {

        }

        public static void SQL(Primitive Data,Primitive Schema,string FilePath) //TODO
        {

        }

        public static void XML(Primitive Data,Primitive Schema,string FilePath) //TODO
        {
            /*
            Utilities.AddtoStackTrace("Export.XML");
            const string XML_Creation_Statement = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><root></root>";
            System.IO.File.WriteAllText(FilePath, XML_Creation_Statement);
            string Document = LDxml.Open(FilePath);

            string Parent = LDxml.Parent();
            bool Continue = true;

            Primitive DataArray = Utilities.XMLAttributes();

            while (Continue)
            {
                string Switcher = DataArray[5];
                switch (Switcher)
                {
                    //Ignore Root and XMLDeclaration
                    case "XmlDeclaration":
                    case "root":
                        LDxml.NextSibling();
                        DataArray = Utilities.XMLAttributes();
                        break;
                    default:
                        DataArray = Utilities.XMLAttributes();
                        XML_Write(Data);
                        LDxml.Save(FilePath);
                        Continue = false;
                        break;
                }
            }
            */
        }
        /*
        static void XML_Write(Primitive Data)
        {
            Utilities.AddtoStackTrace("Export.XML_Write");
         //   Primitive ExportData = null;
            for (int i = 1; SBArray.GetItemCount(Data); i++)
            {
               // ExportData["id"] = i;
                LDxml.AddNode("record", i, null, "Append");
                string Child = LDxml.LastChild();
            }
        }
        */
        public static void HTML(Primitive Data,Primitive Schema,string Title,string FilePath) //TODO
        {
            Utilities.AddtoStackTrace("Export.HTML");
            string Output =  HTML(Data, Schema, Title);
            Console.WriteLine(Output);

            System.IO.File.WriteAllText(FilePath, Output);
        }

        public static string HTML(Primitive Data, Primitive Schema,string Title)
        {
            Utilities.AddtoStackTrace("Export.HTML");
            StringBuilder HTML_Statement = new StringBuilder();
            HTML_Statement.Append("<!DOCTYOE html>\n<html>\n\t<title>" + Title + "</title>\n");
            HTML_Statement.Append("\t<meta name=\"viewport\" content=\" width=device-width, initial-scale=1\">)");

             return HTML_Statement.ToString();

        }
	}
}
