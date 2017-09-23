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
        /// Generates a 2D Small Basic Primitive Array from the Last nonschema Query.
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

            Data[0] = Schema; //Sets the Schema at Indicie zero
            //Shift all indicies by one to meet implicit expectations of the Primitive[] Datatype from SmallBasic
            int DataCount = Data.GetItemCount();
            for (int i = 0; i <= DataCount + 1; i++)
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

            string[] SchemArray = ConvertSchema(Schema);
            string[,] DataArray = ConvertData(Schema, Data);
            int DataCount = Data.GetItemCount();
            int SchemaCount = Schema.GetItemCount();

            StringBuilder SQL = new StringBuilder();
            SQL.Append("CREATE TABLE " + TableName + "(");
            //Header Stuff
            for (int i = 0; i < SchemArray.Length; i++)
            {
                SQL.Append("\"" + SchemArray[i] + "\" " + Types[SchemArray[i]]);
                if (PK[SchemArray[i]])
                {
                    SQL.Append("  PRIMARY KEY");
                }
                if (i < (SchemArray.Length - 1) )
                {
                    SQL.Append(",");
                }
            }
            SQL.Append(");\n");

            //Data Extraction
            for (int i = 0; i < DataCount; i++)
            {
                SQL.Append("INSERT INTO " + TableName + " VALUES ('");
                for (int ii = 0; ii < SchemaCount; ii++)
                {
                    SQL.Append( DataArray[i,ii].Replace("'", "''") );
                    if (ii < (SchemArray.Length - 1) )
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
                    int SchemaQueryCount = SchemaQuery.GetItemCount();
                    int SchemaCount = Schema.GetItemCount();

                    for (int i = 1; i <= SchemaQueryCount; i++)
                    {
                        for (int ii = 1; ii <= SchemaCount; ii++)
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

        /// <summary>
        /// StringBuilder based XML document creator.
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Schema"></param>
        /// <param name="Title"></param>
        /// <param name="FilePath"></param>
        public static void XML(Primitive Data,Primitive Schema,string Title,string FilePath)
        {
            Utilities.AddtoStackTrace("Export.XML");

            const string XML_Creation_Statement = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\n";

            Title = Title.Replace(" ", "_").Replace("\"", "").Replace("&", "").Replace("<", "").Replace(">", "").Replace("'","");
            StringBuilder _XML = new StringBuilder();
            _XML.Append(XML_Creation_Statement);
            _XML.Append("<root>\n");
            _XML.AppendFormat("\t<{0}>\n", Title);

            string[] SchemArray = ConvertSchema(Schema);
            string[,] DataArray = ConvertData(Schema, Data);
            int DataCount = Data.GetItemCount();
            int SchemaCount = Schema.GetItemCount();

            for (int i = 0; i < DataCount; i++)
            {
                _XML.Append("\t\t<row>\n");
                for (int ii = 0; ii < SchemaCount; ii++)
                {
                    string Node =  SchemArray[ii].Replace(" ", "_").Replace("\"", "").Replace("&","").Replace("<", "").Replace(">", "").Replace("'", "");
                    string Item =  DataArray[i,ii].Replace("&", "&amp;").Replace("<", "&lt;");
                    _XML.AppendFormat("\t\t\t<{0}>{1}</{0}>\n", Node, Item);
                }
                _XML.Append("\t\t</row>\n");
            }
            _XML.AppendFormat("\t</{0}>\n", Title);
            _XML.Append("</root>");
            System.IO.File.WriteAllText(FilePath, _XML.ToString());
        }

        /// <summary>
        /// For Wikipedia Tables
        /// </summary>
        public static void MarkUp(Primitive Data, Primitive Schema, string FilePath)
        {
            Utilities.AddtoStackTrace("Export.MarkUp");
            System.IO.File.WriteAllText(FilePath, MarkUp(Data, Schema) );
        }

        /// <summary>
        /// For Wikipedia Tables. Currently does not deal with Pipe characters.
        /// </summary>
        public static string MarkUp(Primitive Data, Primitive Schema)
        {
            Utilities.AddtoStackTrace("Export.MarkUp");
            Primitive Index = Schema.GetAllIndices();

            int DataCount = Data.GetItemCount();
            int SchemaCount = Schema.GetItemCount();
            string[] SchemaArray = ConvertSchema(Schema);
            string[,] DataArray = ConvertData(Schema, Data);

            StringBuilder SB = new StringBuilder();
            SB.AppendLine("{| class=\"wikitable sortable\"");
            SB.AppendLine("|-");
            SB.Append("! ");
            //Headers
            for (int i = 0; i < SchemaCount; i++)
            {
                if (i != 0)
                {
                    SB.AppendFormat("!! {0}", SchemaArray[i]);
                }
                else
                {
                    SB.AppendFormat("{0}", SchemaArray[i]);
                }
            }
            SB.AppendLine();

            for(int i = 0; i < DataCount; i++)
            {
                SB.Append("|-\n|");
                for (int ii = 0; ii < SchemaCount; ii++)
                {
                    if (ii < (SchemaCount - 1 ) )
                    {
                        SB.AppendFormat("{0} ||", DataArray[i,ii]);
                    }
                    else
                    {
                        SB.Append(DataArray[i,ii]);
                    }
                }
                SB.AppendLine();
            }

            SB.AppendLine("|}");
            return SB.ToString();
        }

        /// <summary>
        /// Support for Github and Reddit Markdown
        /// </summary>
        public static void MarkDown(Primitive Data, Primitive Schema, string FilePath)
        {
            Utilities.AddtoStackTrace("Export.MarkDown");
            string Output = MarkDown(Data, Schema);
            System.IO.File.WriteAllText(FilePath, Output);
        }

        /// <summary>
        /// Support for Github and Reddit Markdown
        /// </summary>
        public static string MarkDown(Primitive Data, Primitive Schema)
        {
            Utilities.AddtoStackTrace("Export.MarkDown");
            Primitive Index = Schema.GetAllIndices();
            Stopwatch MD = new Stopwatch();
            MD.Start();

            StringBuilder SB = new StringBuilder();

            string[] SchemaArray = ConvertSchema(Schema);
            string[,] DataArray = ConvertData(Schema, Data);
            int SchemaCount = Schema.GetItemCount();
            int DataCount = Data.GetItemCount();

            //Create Header stuff
            SB.Append("|");
            for(int i = 0; i < SchemaArray.Length; i++)
            {
                SB.AppendFormat(" {0} |", SchemaArray[i].Replace("|","`|") );
            }
            SB.AppendLine();

            //Cell allignment
            SB.Append("|");
            for (int i = 0; i < SchemaArray.Length; i++)
            {
                SB.Append(":---|");
            }
            SB.AppendLine();

            for(int i = 0; i < DataCount; i++)
            {
                SB.Append("|");
                for (int ii = 0; ii < SchemaCount; ii++)
                {
                    SB.AppendFormat("{0}|", DataArray[i,ii]);
                }
                SB.AppendLine();
            }

            Console.WriteLine("Markdown completed in {0} ms",MD.ElapsedMilliseconds);
            return SB.ToString();
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
                throw new ArgumentException("DBM.Export.HTML : Data, Schema, or Title are null or are composed of whitespace characters");
            }

            StringBuilder HTML_Statement = new StringBuilder();
            HTML_Statement.Append("<!DOCTYPE html>\n<html>\n\t");

            HTML_Statement.Append("<head>\n\t\t");

            HTML_Statement.Append("<title>" + Title + "</title>\n\t\t");
            HTML_Statement.Append("<meta charset = \"UTF-8\">\n\t\t");
            HTML_Statement.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n\t\t");
            HTML_Statement.Append("<meta name=\"generator\" content=\"" + Generator + "\">\n\t\t");
            HTML_Statement.Append("<!-- DBM is developed by Abhishek Sathiabalan https://github.com/65001/DBM-CSharp -->\n\t\t");

            HTML_Statement.Append("<style>\n\t\t\t");
            HTML_Statement.Append("table, td, th {border: 1px solid #ddd; text-align: left;}\n\t\t\t");
            HTML_Statement.Append("table {border-collapse: collapse; width: 100%;}\n\t\t\tth,td {padding: 5px;}\n\t\t\t");
            HTML_Statement.Append("tr:hover{background-color:#f5f5f5}\n\t\t\tth,td#Main {background-color: #4CAF50;color: white;font-size:120%;border:0px;text-align:center;}\n\t\t");
            HTML_Statement.Append("</style>\n\t");

            HTML_Statement.Append("</head>\n\n\t");

            HTML_Statement.Append("<body>\n\t\t");

            HTML_Statement.Append("<div style=\"overflow-x:auto;\">\n\t\t\t");
            HTML_Statement.Append("<table>\n\t\t\t\t");
            HTML_Statement.Append("<thead>\n\t\t\t\t\t<tr>\n\t\t\t\t\t\t<td id=\"Main\" colspan = \"");
            HTML_Statement.Append(Schema.GetItemCount().ToString());
            HTML_Statement.Append("\">" + Title + "</td>\n\t\t\t\t\t");
            HTML_Statement.Append("</tr>\n\t\t\t\t\t<tr>\n");

            //Converts Primitive Data Type to String[,].
            string[,] DataArray = ConvertData(Schema, Data);
            string[] SchemaArray = ConvertSchema(Schema);
            int DataCount = Data.GetItemCount();
            int SchemaCount = Schema.GetItemCount();

            //Converts Column Names in the database to Columns in a html document
            for (int i = 0; i < SchemaCount; i++)
            {
                string Temp_Schema = SchemaArray[i].Replace("_", " ");
                Temp_Schema = Text.GetSubText(Temp_Schema, 1, 1).ToString().ToUpper() + Text.GetSubTextToEnd(Temp_Schema, 2);
                HTML_Statement.Append("\t\t\t\t\t\t<th>" + Temp_Schema + "</th>\n");
            }

            HTML_Statement.Append("\t\t\t\t\t</tr>\n\t\t\t\t</thead>\n");
            //Convert Table Data into HTML table rows.
            for (int i = 0; i < DataCount; i++)
            {
                HTML_Statement.Append("\t\t\t\t<tr>\n");
                for (int ii = 0; ii < SchemaCount; ii++)
                {
                    HTML_Statement.Append("\t\t\t\t\t<td>" + DataArray[i,ii]  + "</td>\n");
                }
                HTML_Statement.Append("\t\t\t\t</tr>\n");
            }

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

        /// <summary>
        /// StringBuilder based JSON writer.
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="Schema"></param>
        /// <param name="Title"></param>
        /// <returns></returns>
        public static string JSON(Primitive Data, Primitive Schema, string Title)
        {
            Utilities.AddtoStackTrace("Export.JSON");
            StringBuilder _JSON = new StringBuilder();
            _JSON.Append("{\"" + Title +"\": {");
            _JSON.Append("\"Record\": [");

            string[]  SchemaArray = ConvertSchema(Schema);
            string[,] DataArray = ConvertData(Schema, Data);
            int SchemaCount = Schema.GetItemCount();
            int DataCount = Data.GetItemCount();

            for (int i = 0; i < DataCount; i++)
            {
                _JSON.Append("{");
                for (int ii = 0; ii < SchemaCount; ii++)
                {
                    _JSON.Append("\"" + SchemaArray[ii] + "\" : \"" +  DataArray[i,ii].Replace(Environment.NewLine,string.Empty) + "\"");
                    if (ii < (SchemaCount - 1 ))
                    {
                        _JSON.Append(",");
                    }
                }
                _JSON.AppendLine("}");
                if (i < (DataCount - 1 ))
                {
                    _JSON.Append(",");
                }
            }
            _JSON.Append("]}}");
            return _JSON.ToString();
        }

        public static string[,] ConvertData(Primitive Schema, Primitive Data)
        {
            int DataCount = Data.GetItemCount();
            int SchemaCount = Schema.GetItemCount();
            string[,] DataArray = new string[DataCount, SchemaCount];

            for (int i = 1; i <= DataCount; i++)
            {
                Primitive Temp_HTML = Data[i];
                for (int ii = 1; ii <= SchemaCount; ii++)
                {
                    DataArray[i - 1, ii - 1] = Temp_HTML[Schema[ii]];
                }
            }
            return DataArray;
        }

        public static string[] ConvertSchema(Primitive Schema)
        {
            int SchemaCount = Schema.GetItemCount();
            string[] SchemaArray = new string[SchemaCount];
            for (int i = 1; i <= SchemaCount; i++)
            {
                SchemaArray[i - 1] = Schema[i];
            }
            return SchemaArray;
        }

    }
}
