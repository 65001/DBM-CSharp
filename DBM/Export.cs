// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
// Created : 3/14/2017 5:55 PM 2017314 17:55:44
using System;
using System.Text;
using System.IO;
using System.Diagnostics;
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

        public static Primitive Generate2DArrayFromLastQuery()
        {
            return Generate2DArray(Engines.CurrentDatabase, Engines.LastQuery[Engines.LastQuery.Count - 1]);
        }
    
        public static void CSV(Primitive Data,Primitive Schema,string FilePath,string deliminator) //TODO
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
        public static void HTML(Primitive Data,Primitive Schema,string Title,string FilePath,string Generator) //TODO
        {
            Utilities.AddtoStackTrace("Export.HTML");
            string Output =  HTML(Data, Schema, Title,Generator);
            System.IO.File.WriteAllText(FilePath, Output);
        }

        public static string HTML(Primitive Data, Primitive Schema,string Title,string Generator)
        {
            Utilities.AddtoStackTrace("Export.HTML");
            Stopwatch HTML_Timer = new Stopwatch();
            HTML_Timer.Start();
            if(string.IsNullOrWhiteSpace(Data) || string.IsNullOrWhiteSpace(Schema) || string.IsNullOrWhiteSpace(Title))
            {
                throw new ArgumentException("DBM.Export.HTML : Data , Schema , or Title are null or are composed of whitespace characters");
            }

            StringBuilder HTML_Statement = new StringBuilder();
            HTML_Statement.Append("<!DOCTYPE html>\n<html>\n\t");

            HTML_Statement.Append("<head>\n\t\t");

            HTML_Statement.Append("<title>" + Title + "</title>\n\t\t");
            HTML_Statement.Append("<meta charset = \"UTF-8\">\n\t\t");
            HTML_Statement.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n\t\t");
            HTML_Statement.Append("<meta name=\"generator\" content=\"" + Generator + "\">\n\t\t");
            HTML_Statement.Append("<!-- DBM is develeoped by Abhishek Sathiabalan https://github.com/65001/DBM-CSharp -->\n\t\t");

            HTML_Statement.Append("<style>\n\t\t\t");
            HTML_Statement.Append("table, td, th {border: 1px solid #ddd; text-align: left;}\n\t\t\t");
            HTML_Statement.Append("table {border-collapse: collapse; width: 100%;}\n\t\t\tth,td {padding: 5px;}\n\t\t\t");
            HTML_Statement.Append("tr:hover{background-color:#f5f5f5}\n\t\t\tth,td#Main {background-color: #4CAF50;color: white;font-size:120%;border:0px;text-align:center;}\n\t\t");
            HTML_Statement.Append("</style>\n\t");

            HTML_Statement.Append("</head>\n\n\t");
            
            HTML_Statement.Append("<body>\n\t\t");

            HTML_Statement.Append("<div style=\"overflow-x:auto;\">\n\t\t\t");
            HTML_Statement.Append("<table>\n\t\t\t\t");
            HTML_Statement.Append("<tr>\n\t\t\t\t\t<td id=\"Main\" colspan = \"");
            HTML_Statement.Append(SBArray.GetItemCount(Schema).ToString());
            HTML_Statement.Append("\">" + Title + "</td>\n\t\t\t\t");
            HTML_Statement.Append("</tr>\n\t\t\t\t<tr>\n");

            //Converts Primitive Data to FastArray
            string FastArray = LDFastArray.Add();
            for (int i = 1; i <= SBArray.GetItemCount(Data); i++)
            {
                Primitive Temp_HTML = Data[i];
                for(int ii =1;ii <= SBArray.GetItemCount(Schema);ii++)
                {
                    LDFastArray.Set2D(FastArray, i, ii, Temp_HTML[Schema[ii]]);
                }
            }
            //Header Data
            for (int i = 1; i <= SBArray.GetItemCount(Schema); i++)
            {
                string Temp_Schema = LDText.Replace(Schema[i], "_", " ");
                Temp_Schema = Text.ConvertToUpperCase(Text.GetSubText(Temp_Schema, 1, 1)) + Text.GetSubTextToEnd(Temp_Schema, 2);
                HTML_Statement.Append("\t\t\t\t\t<th>" + Temp_Schema + "</th>\n");
            }

            HTML_Statement.Append("\t\t\t\t</tr>\n");

            for (int i = 1; i <= SBArray.GetItemCount(Data); i++)
            {
                HTML_Statement.Append("\t\t\t\t<tr>\n");
                for (int ii = 1; ii <= SBArray.GetItemCount(Schema); ii++)
                {
                    HTML_Statement.Append("\t\t\t\t\t<td>" + LDFastArray.Get2D(FastArray, i, ii).ToString() + "</td>\n");
                }
                HTML_Statement.Append("\t\t\t\t</tr>\n");
            }

            LDFastArray.Remove(FastArray);
            HTML_Statement.Append("\t\t\t</table>\n\t\t</div>\n\t</body>\n</html>");

            HTML_Timer.Stop();
            Console.WriteLine("Export.HTML Run time {0} ms", HTML_Timer.ElapsedMilliseconds);
            return HTML_Statement.ToString();
        }
	}
}
