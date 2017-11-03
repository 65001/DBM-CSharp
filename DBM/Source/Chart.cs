﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DBM
{
    public abstract class Chart
    {
        List<string> Columns = new List<string>();
        List<DataType> Types = new List<DataType>();
        List<List<string>> Data = new List<List<string>>();

        public enum ChartTypes { Bar, Bubble, Column, Donut, Pie, Table };
        public enum DataType { number,text};

        public abstract ChartTypes ChartType { get; }
        public abstract string Function { get; }
        public abstract string Package { get; }

        public string Title;
        public string SubTitle;
        public string Xaxis;
        public string Yaxis;

        public void AddColumn(string Column, DataType Type = DataType.text)
        {
            Columns.Add(Column);
            Types.Add(Type);
        }

        public void BulkAddColumn(string[] Column,DataType[] Type)
        {
            Columns.AddRange(Column);
            Types.AddRange(Type);
        }

        public void BulkAddColumn(List<string> Column,List<DataType> Type)
        {
            Columns.AddRange(Column);
            Types.AddRange(Type);
        }

        public void AddRowData<T>(int Index,T Data)
        {
            int GlobalMax = this.Data.Count;
            int LocalMax = 0;

            if (GlobalMax > Index) {
                LocalMax = this.Data[Index].Count;
            }

            int ColumnMax = this.Columns.Count;
            if (LocalMax + 1 > ColumnMax)
            {
                throw new ArgumentOutOfRangeException("Index"," You have added too many rows but lack sufficient number of columns");
            }

            if (GlobalMax > Index)
            {
                this.Data[Index].Add(Data.ToString());
            }
            else
            {
                List<string> Temp = new List<string>();
                Temp.Add(Data.ToString());
                this.Data.Add(Temp);
            }
        }

        private string ListToJsonArray(List<string> Data,bool respectNumber = true)
        {
            StringBuilder SB = new StringBuilder();
            SB.Append("[");
            for (int i = 0; i< Data.Count; i++)
            {
                if (respectNumber == false)
                {
                    SB.AppendFormat("'{0}'", Data[i]);
                }
                else
                {
                    switch (Types[i])
                    {
                        case DataType.text:
                            SB.AppendFormat("'{0}'", Data[i]);
                            break;
                        case DataType.number:
                            SB.AppendFormat("{0}", Data[i]);
                            break;
                        default:
                            SB.AppendFormat("'{0}'", Data[i]);
                            break;
                    }
                }
                if (i<(Data.Count - 1))
                {
                    SB.Append(",");
                }
            }
            SB.Append("]");
            return SB.ToString();
        }

        public string Export()
        {
            if (Title == null) Title = string.Empty;
            if (SubTitle == null) SubTitle = string.Empty;
            if (Xaxis == null) Xaxis = string.Empty;
            if (Yaxis == null) Yaxis = string.Empty;
            if (Function == null) throw new ArgumentNullException("Function");
            if (Package == null) throw new ArgumentNullException("Package");
            return Export(Function, Package, Title, SubTitle, Xaxis, Yaxis);
        }

        string Export(string Function, string Package, string Title, string SubTitle, string Xaxis, string Yaxis)
        {
            StringBuilder SB = new StringBuilder();
            string width = "95%";
            string height = "95%";

            SB.AppendLine("<html>");
            SB.AppendLine("\t<head>");
            SB.AppendLine("<meta charset=\"utf-8\" />");
            SB.AppendLine("\t\t<script type=\"text/javascript\" src=\"https://www.gstatic.com/charts/loader.js\"></script>");
            SB.AppendLine("\t\t<script type=\"text/javascript\">");
            SB.AppendFormat("\t\t\tgoogle.charts.load('{0}'", "current");
            SB.Append(",{'packages':[\'" + Package + "\']}"); //TODO FIX

            SB.AppendLine(");\n");

            SB.AppendLine("\t\t\tgoogle.charts.setOnLoadCallback(drawChart);");
            SB.AppendLine();
            SB.AppendLine("\t\t\tfunction drawChart() {");
            SB.AppendLine("\t\t\t\tvar data = google.visualization.arrayToDataTable([");
            //Columns
            SB.Append("\t\t\t\t\t");
            SB.Append(ListToJsonArray(Columns,false));
            SB.AppendLine(",");
            //Data
            for (int i = 0; i < Data.Count; i++)
            {
                SB.Append("\t\t\t\t\t");
                //Data Level 2
                SB.Append(ListToJsonArray(Data[i]));
                SB.Append("");
                if (i < (Data.Count - 1))
                {
                    SB.AppendLine(",");
                }
            }
            SB.AppendLine("]);");

            SB.AppendLine("\t\t\tvar options = {");
            SB.AppendFormat("\t\t\t\t\ttitle: '{0}',\n", Title);
            SB.AppendFormat("\t\t\t\t\tsubtitle:'{0}',\n", SubTitle);
            SB.Append("\t\t\t\t\thAxis: {title: '"+Xaxis+"'},\n");
            SB.Append("\t\t\t\t\tyAxis: {title: '" + Yaxis + "'}\n");
            SB.AppendFormat("}};\n");

            SB.AppendFormat("\t\tvar chart = new google.visualization.{0}(document.getElementById('chart'));\n", Function);
            SB.AppendFormat("\t\tchart.draw(data, options);\n", Function);
            SB.AppendLine("\t}\n\t\t</script>");
            SB.AppendLine("\t</head>");
            SB.AppendLine("\t<body>");
            SB.AppendFormat("\t\t<div id='{0}' style=\"width:{1}; height: {2};\"></div>\n", "chart", width, height);
            SB.AppendLine("\t</body>");
            SB.AppendLine("</html>");
            return SB.ToString();
        }

        public void Write(string URI, string Data)
        {
            if (string.IsNullOrWhiteSpace(URI)) throw new ArgumentNullException("URI");
            if (string.IsNullOrWhiteSpace(Data)) throw new ArgumentNullException("Data");
            System.IO.File.WriteAllText(URI, Data);
        }

        public abstract class CoreChart : Chart
        {
            public override string Package { get { return "corechart"; } }
        }

        public class Bar : CoreChart
        {
            public override ChartTypes ChartType
            {
                get { return ChartTypes.Bar; }
            }

            public override string Function { get { return "ColumnChart"; } }
        }

        public class Bubble : CoreChart
        {
            public override ChartTypes ChartType
            {
                get { return ChartTypes.Bubble; }
            }

            public override string Function { get { return "BubbleChart"; } }
        }

        public class Column : CoreChart
        {
            public override ChartTypes ChartType
            {
                get { return ChartTypes.Column; }
            }

            public override string Function { get { return "ColumnChart"; } }
        }


        public class Pie : Chart
        {
            public override ChartTypes ChartType
            {
                get { return ChartTypes.Pie; }
            }

            public override string Function { get { return ""; } }
            public override string Package => throw new NotImplementedException();
        }
    }
}