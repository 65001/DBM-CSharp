// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
// Created : 3/14/2017 5:55 PM 2017314 17:55:44
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Microsoft.SmallBasic.Library;
using LitDev;
namespace DBM
{
    public static class Export
    {
        public static Primitive Generate2DArray(string Database, string SQL)
        {
            return Engines.Query(Database, SQL, null, true, GlobalStatic.UserName, ""); 
        }

        public static Primitive Generate2DArrayFromTable(string Database, string Table)
        {
            return Generate2DArray(Database, "Select * From " + Table);
        }

        public static Primitive Generate2DArrayFromCurrentTable()
        {
            return Generate2DArrayFromTable(Engines.CurrentDatabase, Engines.CurrentTable);
        }

        /// <summary>
        /// Generates a 2D Primitive Array from the Last nonschema Query
        /// </summary>
        public static Primitive Generate2DArrayFromLastQuery()
        {
            return Generate2DArray(Engines.CurrentDatabase, Engines.LastNonSchemaQuery.Last());
        }

        public static Primitive GenerateSchemaFromQueryData(Primitive Data)
        {
            return Data[1].GetAllIndices();
        }

        public static Primitive GenerateSchemaFromLastQuery()
        {
            return GenerateSchemaFromQueryData(Generate2DArrayFromLastQuery());
        }

        public static void CSV(Primitive Data, Primitive Schema, string FilePath, string deliminator)
        {
            Primitive _Data = null;
            Data[0] = Schema;

            for (int i = 0; i <= Data.GetItemCount() + 1; i++)
            {
                _Data[(i + 1)] = Data[i];
            }

            if (!string.IsNullOrWhiteSpace(deliminator))
            {
                LDUtilities.CSVDeliminator = deliminator;
            }
            LDFile.WriteCSV(FilePath, _Data);
        }

        public static void SQL(Primitive Data, Primitive Schema, Dictionary<string, bool> PK, Dictionary<string, string> Types, string TableName, string FilePath)
        {
            Stopwatch SQL_Time = new Stopwatch();
            SQL_Time.Start();
            string _SQL = SQL(Data, Schema, PK, Types, Engines.CurrentTable);
            System.IO.File.WriteAllText(FilePath, _SQL);
            Console.WriteLine("SQL void time {0} ms", SQL_Time.ElapsedMilliseconds);
        }

        public static string SQL(Primitive Data, Primitive Schema, Dictionary<string, bool> PK, Dictionary<string, string> Types, string TableName)
        {
            Utilities.AddtoStackTrace("Export.SQL()");
            Stopwatch SQL_Time = new Stopwatch();
            SQL_Time.Start();

            if (string.IsNullOrWhiteSpace(Data))
            {
                throw new ArgumentNullException(Data);
            }

            if (string.IsNullOrWhiteSpace(Schema))
            {
                throw new ArgumentNullException(Schema);
            }

            if (string.IsNullOrWhiteSpace(TableName))
            {
                throw new ArgumentNullException(TableName);
            }

            if (PK == null)
            {
                throw new ArgumentNullException("PK");
            }
            if (Types == null)
            {
                throw new ArgumentNullException("Types");
            }

            StringBuilder SQL = new StringBuilder();
            SQL.Append("CREATE TABLE " + TableName + "(");
            //Header Stuff
            for (int i = 1; i <= Schema.GetItemCount(); i++)
            {
                SQL.Append("\"" + (string)Schema[i] + "\" " + Types[Schema[i]]);
                if (PK[Schema[i]])
                {
                    SQL.Append("  PRIMARY KEY");
                }
                if (i < Schema.GetItemCount())
                {
                    SQL.Append(",");
                }
            }
            SQL.Append(");\n");
            //Data Extraction
            for (int i = 1; i <= Data.GetItemCount(); i++)
            {
                SQL.Append("INSERT INTO " + TableName + " VALUES ('");
                for (int ii = 1; ii <= Data[i].GetItemCount(); ii++)
                {
                    SQL.Append(Data[i][Schema[ii]].ToString().Replace("'", "''"));
                    if (ii < Data[i].GetItemCount())
                    {
                        SQL.Append("','");
                    }
                }
                SQL.Append("');\n");
            }
            SQL.Replace("' '", "NULL").Replace("''", "NULL");
            Console.WriteLine("SQL string time {0} ms", SQL_Time.ElapsedMilliseconds);
            return SQL.ToString();
        }


        /// <summary>
        ///  Fetches the Primary Key Information on everything
        /// </summary>
        public static Dictionary<string, bool> SQL_Fetch_PK(Primitive SchemaQuery, Primitive Schema, Engines.EnginesMode CurrentEngine)
        {
            Dictionary<string, bool> _Dictionary = new Dictionary<string, bool>();
            switch (CurrentEngine)
            {
                case Engines.EnginesMode.SQLITE:
                    for (int i = 1; i <= SchemaQuery.GetItemCount(); i++)
                    {
                        for (int ii = 1; ii <= Schema.GetItemCount(); ii++)
                        {
                            if (Schema[ii] == SchemaQuery[i]["name"])
                            {
                                if (SchemaQuery[i]["pk"] == 1)
                                {
                                    _Dictionary.Add(SchemaQuery[i]["name"], true);
                                }
                                else
                                {
                                    _Dictionary.Add(SchemaQuery[i]["name"], false);
                                }
                            }
                        }
                    }
                    return _Dictionary;
                default:
                    throw new NotImplementedException("Current Engine is not supported");
            }
        }

        public static Dictionary<string, string> SQL_Fetch_Type(Primitive SchemaQuery, Primitive Schema, Engines.EnginesMode CurrentEngine)
        {
            Dictionary<string, string> _Dictionary = new Dictionary<string, string>();
            switch (CurrentEngine)
            {
                case Engines.EnginesMode.SQLITE:
                    for (int i = 1; i <= SchemaQuery.GetItemCount(); i++)
                    {
                        for (int ii = 1; ii <= Schema.GetItemCount(); ii++)
                        {
                            if (Schema[ii] == SchemaQuery[i]["name"])
                            {
                                _Dictionary.Add(SchemaQuery[i]["name"], SchemaQuery[i]["type"]);
                            }
                        }
                    }
                    return _Dictionary;
                default:
                    throw new PlatformNotSupportedException("Current Engine is not supported");
            }
        }
        
        public static void XML(Primitive Data,Primitive Schema,string Title,string FilePath)
        {
            Utilities.AddtoStackTrace("Export.XML");

            const string XML_Creation_Statement = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\n";

            Title = Title.Replace(" ", "_").Replace("\"", "").Replace("&", "").Replace("<", "").Replace(">", "").Replace("'","");
            StringBuilder _XML = new StringBuilder();
            _XML.Append(XML_Creation_Statement);
            _XML.Append("<root>\n");
            _XML.AppendFormat("\t<{0}>\n", Title);

            for (int i = 1; i <= Data.GetItemCount(); i++)
            {
                _XML.Append("\t\t<row>\n");
                for (int ii = 1; ii <= Schema.GetItemCount(); ii++)
                {
                    string Node = ((string)Schema[ii]).Replace(" ", "_").Replace("\"", "").Replace("&","").Replace("<", "").Replace(">", "").Replace("'", "");
                    string Item = ((string)Data[i][Schema[ii]]).Replace("&", "&amp;").Replace("<", "&lt;");
                    _XML.AppendFormat("\t\t\t<{0}>{1}</{0}>\n", Node, Item);
                }
                _XML.Append("\t\t</row>\n");
            }
            _XML.AppendFormat("\t</{0}>\n", Title);
            _XML.Append("</root>");
            System.IO.File.WriteAllText(FilePath, _XML.ToString());
        }

        public static void HTML(Primitive Data, Primitive Schema, string Title, string FilePath, string Generator) 
        {
            Utilities.AddtoStackTrace("Export.HTML");
            string Output = HTML(Data, Schema, Title, Generator);
            System.IO.File.WriteAllText(FilePath, Output);
        }

        public static string HTML(Primitive Data, Primitive Schema, string Title, string Generator)
        {
            Utilities.AddtoStackTrace("Export.HTML");
            Stopwatch HTML_Timer = new Stopwatch();
            HTML_Timer.Start();
            if (string.IsNullOrWhiteSpace(Data) || string.IsNullOrWhiteSpace(Schema) || string.IsNullOrWhiteSpace(Title))
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
            HTML_Statement.Append(Schema.GetItemCount().ToString());
            HTML_Statement.Append("\">" + Title + "</td>\n\t\t\t\t");
            HTML_Statement.Append("</tr>\n\t\t\t\t<tr>\n");

            //Converts Primitive Data to FastArray
            string FastArray = LDFastArray.Add();
            for (int i = 1; i <= Data.GetItemCount(); i++)
            {
                Primitive Temp_HTML = Data[i];
                for (int ii = 1; ii <= Schema.GetItemCount(); ii++)
                {
                    LDFastArray.Set2D(FastArray, i, ii, Temp_HTML[Schema[ii]]);
                }
            }
            //Header Data
            for (int i = 1; i <= Schema.GetItemCount(); i++)
            {
                string Temp_Schema = Schema[i].ToString().Replace("_", " ");
                Temp_Schema = Text.ConvertToUpperCase(Text.GetSubText(Temp_Schema, 1, 1)) + Text.GetSubTextToEnd(Temp_Schema, 2);
                HTML_Statement.Append("\t\t\t\t\t<th>" + Temp_Schema + "</th>\n");
            }

            HTML_Statement.Append("\t\t\t\t</tr>\n");

            for (int i = 1; i <= Data.GetItemCount(); i++)
            {
                HTML_Statement.Append("\t\t\t\t<tr>\n");
                for (int ii = 1; ii <= Schema.GetItemCount(); ii++)
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

        public static void JSON(Primitive Data, Primitive Schema, string Title, string Path)
        {
            Utilities.AddtoStackTrace("Export.JSON");
            System.IO.File.WriteAllText(Path, JSON(Data, Schema, Title) );
        }

        public static string JSON(Primitive Data, Primitive Schema, string Title)
        {
            Utilities.AddtoStackTrace("Export.JSON");
            StringBuilder _JSON = new StringBuilder();
            _JSON.Append("{\"" + Title +"\": {");
            _JSON.Append("\"Record\": [");
            for (int i = 1; i <= Data.GetItemCount(); i++)
            {
                _JSON.Append("{");
                for (int ii = 1; ii <= Schema.GetItemCount(); ii++)
                {
                    _JSON.Append("\"" + (string)Schema[ii] + "\" : \"" + ((string)Data[i][Schema[ii]]).Replace(Environment.NewLine,string.Empty) + "\"");
                    if (ii < Schema.GetItemCount())
                    {
                        _JSON.Append(",");
                    }
                }
                _JSON.AppendLine("}");
                if (i < Data.GetItemCount())
                {
                    _JSON.Append(",");
                }
            }
            _JSON.Append("]}}");
            return _JSON.ToString();
        }

    }
}
