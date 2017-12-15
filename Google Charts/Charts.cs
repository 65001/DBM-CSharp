using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google_Charts
{
    public abstract class Chart
    {
        List<string> Columns = new List<string>();
        List<DataType> Types = new List<DataType>();
        List<List<string>> Data = new List<List<string>>();

        public enum ChartTypes { Bar, Bubble, Column, Donut, Pie, Table, Org, Sankey, Histogram, Scatter, Line, GeoChart,TimeLine };
        public enum DataType { number, text };

        public abstract ChartTypes ChartType { get; }
        public abstract string Function { get; }
        public abstract string Package { get; }
        public abstract bool[] RespectNumbers { get; }

        public string Title;
        public string SubTitle;
        public string Xaxis;
        public string Yaxis;
        string MapKey = "AIzaSyDlBNT9yXQlCK18gRyYM1mBDhuKdb2ZXQA";

        /// <summary>
        /// Counts from zero and not one!!!
        /// </summary>
        public abstract int MinColumns { get; }
        public abstract int MaxColumns { get; }

        private int CurrentColumns { get { return Columns.Count; } }

        public void AddColumn(string Column, DataType Type = DataType.text)
        {
            if (CurrentColumns <= MaxColumns)
            {
                Columns.Add(Column);
                Types.Add(Type);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public void BulkAddColumn(string[] Column, DataType[] Type)
        {
            Columns.AddRange(Column);
            Types.AddRange(Type);
        }

        public void BulkAddColumn(List<string> Column, List<DataType> Type)
        {
            Columns.AddRange(Column);
            Types.AddRange(Type);
        }

        public void AddRowData<T>(int Index, T Data)
        {            
            int GlobalMax = this.Data.Count;
            int LocalMax = 0;

            if (GlobalMax > Index)
            {
                LocalMax = this.Data[Index].Count;
            }

            if (LocalMax > Columns.Count)
            {
                throw new ArgumentOutOfRangeException("Index", " You have added too many rows but lack sufficient number of columns");
            }

            if (GlobalMax > Index)
            {
                this.Data[Index].Add(Data.ToString());
            }
            else
            {
                List<string> Temp = new List<string>
                {
                    Data.ToString()
                };
                this.Data.Add(Temp);
            }
        }

        private string ListToJsonArray(List<string> Data, bool respectNumber = true)
        {
            StringBuilder SB = new StringBuilder();
            SB.Append("[");
            int Max = Data.Count - 1;
            for (int i = 0; i < Data.Count; i++)
            {
                Data[i] = Data[i]?.Replace("\"", "").Replace("'", "\\'").Replace("\n", " ");
                if (respectNumber == false)
                {
                    SB.AppendFormat("'{0}'", Data[i]);
                }
                else
                {
                    switch (Types[i])
                    {
                        case DataType.number:
                            SB.AppendFormat("{0}", Data[i]);
                            break;
                        case DataType.text:
                        default:
                            SB.AppendFormat("'{0}'", Data[i]);
                            break;
                    }
                }

                if (i < Max)
                {
                    SB.Append(",");
                }
            }
            SB.Append("]");
            return SB.ToString().Replace("' '", "''");
        }

        public virtual string Export()
        {
            if (Title == null) Title = string.Empty;
            if (SubTitle == null) SubTitle = string.Empty;
            if (Xaxis == null) Xaxis = string.Empty;
            if (Yaxis == null) Yaxis = string.Empty;
            if (Function == null) throw new ArgumentNullException("Function");
            if (Package == null) throw new ArgumentNullException("Package");
            if (CurrentColumns > MaxColumns || CurrentColumns < MinColumns)
            {
                throw new ArgumentOutOfRangeException("Current Columns is out of bounds");
            }
            return Export(Function, Package, Title, SubTitle, Xaxis, Yaxis, RespectNumbers);
        }

        protected virtual string Export(string Function, string Package, string Title, string SubTitle, string Xaxis, string Yaxis, bool[] RespectNumbers)
        {
            StringBuilder SB = new StringBuilder();
            string width = "95%";
            string height = "95%";

            SB.AppendLine("<html>");
            SB.AppendLine("\t<head>");
            SB.AppendLine("\t<meta charset=\"utf-8\" />");
            SB.AppendLine("\t\t<script type=\"text/javascript\" src=\"https://www.gstatic.com/charts/loader.js\"></script>");
            SB.AppendLine("\t\t<script type=\"text/javascript\">");
            SB.AppendFormat("\t\t\tgoogle.charts.load('{0}'", "current");
            SB.Append(",{'packages':[\'" + Package + "\']"); //TODO FIX
            if (ChartType == ChartTypes.GeoChart)
            {
                SB.Append(",");
                SB.AppendFormat("'{0}'", "mapsApiKey");
                SB.AppendFormat(":'{0}'", MapKey);
            }

            SB.AppendLine("});");
            SB.AppendLine("\t\t\tgoogle.charts.setOnLoadCallback(drawChart);");
            SB.AppendLine("\t\t\tvar chart;");
            SB.AppendLine();
            SB.AppendLine("\t\t\tfunction drawChart() {");
            SB.AppendLine("\t\t\t\tvar data = google.visualization.arrayToDataTable([");
            //Columns
            SB.Append("\t\t\t\t\t");
            SB.Append(ListToJsonArray(Columns, RespectNumbers[0]));
            SB.AppendLine(",");
            //Data
            for (int i = 0; i < Data.Count; i++)
            {
                SB.Append("\t\t\t\t\t");
                //Data Level 2
                SB.Append(ListToJsonArray(Data[i], RespectNumbers[1]));
                SB.Append("");
                if (i < (Data.Count - 1))
                {
                    SB.AppendLine(",");
                }
            }
            SB.AppendLine("]);");
            SB.AppendLine();
            SB.AppendLine("\t\t\t\tvar options = {");
            SB.AppendFormat("\t\t\t\t\t\ttitle: '{0}',\n", Title);
            SB.AppendFormat("\t\t\t\t\t\tsubtitle:'{0}',\n", SubTitle);
            SB.Append("\t\t\t\t\t\thAxis: {title: '" + Xaxis + "'},\n");
            SB.Append("\t\t\t\t\t\tvAxis: {title: '" + Yaxis + "'}\n");
            SB.AppendFormat("\t\t\t\t\t\t}};\n");

            SB.AppendFormat("\t\t\t\tchart = new google.visualization.{0}(document.getElementById('chart'));\n", Function);
            SB.AppendFormat("\t\t\t\tchart.draw(data, options);\n", Function);

            SB.AppendLine("\t\t\t}");
            if (Package == "corechart" || Package == "geochart")
            {
                SB.AppendLine("\t\t\tfunction NW()");
                SB.AppendLine("\t\t\t{");
                SB.AppendLine("\t\t\t\tvar string = chart.getImageURI();");
                SB.AppendLine("\t\t\t\tvar iframe = \"<img width = '100%' height = '100%' frameBorder = '0' src = '\" + string + \"' ></img> \"");
                SB.AppendLine("\t\t\t\tvar w = window.open();");
                SB.AppendLine("\t\t\t\tw.document.open();");
                SB.AppendLine("\t\t\t\tw.document.write(iframe);");
                SB.AppendLine("\t\t\t\tw.document.close();");
                SB.AppendLine("\t\t\t}");
            }
            SB.AppendLine("\t\t</script>");
            SB.AppendLine("\t</head>");
            SB.AppendLine("\t<body>");
            SB.AppendFormat("\t\t<div id='{0}' style=\"width:{1}; height: {2};\"></div>\n", "chart", width, height);
            if (Package == "corechart")
            {
                SB.AppendLine("\t\t<a href =\"#\" onclick=\"NW()\">PNG Version</a>");
            }
            SB.AppendLine("\t</body>");
            SB.AppendLine("</html>");
            return SB.ToString();
        }

        public void Write(string URI)
        {
            if (string.IsNullOrWhiteSpace(URI)) throw new ArgumentNullException("URI");
            System.IO.File.WriteAllText(URI, Export().ToString());
        }

        public abstract class CoreChart : Chart
        {
            public override int MinColumns { get { return 1; } }
            public override bool[] RespectNumbers { get { return new bool[2] { false, true }; } }
            public override string Package { get { return "corechart"; } }
        }

        public class Bar : CoreChart
        {
            public override ChartTypes ChartType
            {
                get { return ChartTypes.Bar; }
            }
            public override string Function { get { return "BarChart"; } }
            public override int MaxColumns { get { return 10; } }
        }

        public class Column : CoreChart
        {
            public override ChartTypes ChartType
            {
                get { return ChartTypes.Column; }
            }
            public override string Function { get { return "ColumnChart"; } }
            public override int MaxColumns { get { return 10; } }
        }


        public class Pie : CoreChart
        {
            public override ChartTypes ChartType { get { return ChartTypes.Pie; } }
            public override string Function { get { return "PieChart"; } }
            public override int MaxColumns { get { return 2; } }
        }

        public class Histograms : CoreChart
        {
            public override ChartTypes ChartType { get { return ChartTypes.Histogram; } }
            public override string Function { get { return "Histogram"; } }
            public override int MaxColumns { get { return 10; } }
        }

        public class Scatter : CoreChart
        {
            public override ChartTypes ChartType { get { return ChartTypes.Scatter; } }
            public override string Function { get { return "ScatterChart"; } }
            public override int MaxColumns { get { return 10; } }
        }

        public class Line : CoreChart
        {
            public override ChartTypes ChartType { get { return ChartTypes.Line; } }
            public override string Function { get { return "LineChart"; } }
            public override int MaxColumns { get { return 10; } }
        }

        public class Table : Chart
        {
            public override string Function { get { return "Table"; } }
            public override string Package { get { return "table"; } }
            public override ChartTypes ChartType { get { return ChartTypes.Table; } }
            public override bool[] RespectNumbers { get { return new bool[2] { false, false }; } }
            public override int MinColumns { get { return 1; } }
            public override int MaxColumns { get { return 10; } }
        }

        public class OrgChart : Chart
        {
            public override ChartTypes ChartType
            {
                get { return ChartTypes.Org; }
            }
            public override int MinColumns { get { return 1; } }
            public override int MaxColumns { get { return 4; } }
            public override string Package { get { return "orgchart"; } }
            public override string Function { get { return "OrgChart"; } }
            public override bool[] RespectNumbers
            {
                get { return new bool[2] { false, false }; }
            }
        }

        public class SanKey : Chart
        {
            public override ChartTypes ChartType
            {
                get { return ChartTypes.Sankey; }
            }
            public override bool[] RespectNumbers
            {
                get { return new bool[2] { false, true }; }
            }
            public override int MinColumns { get { return 1; } }
            public override int MaxColumns { get { return 3; } }
            public override string Package { get { return "sankey"; } }
            public override string Function { get { return "Sankey"; } }
        }

        public class GeoCharts : Chart
        {
            public override int MinColumns { get { return 1; } }
            public override int MaxColumns { get { return 3; } }
            public override string Function { get { return "GeoChart"; } }
            public override string Package { get { return "geochart"; } }
            public override ChartTypes ChartType { get { return ChartTypes.GeoChart; } }
            public override bool[] RespectNumbers
            {
                get { return new bool[2] { false, true }; }
            }
        }

        public class TimeLine : Chart
        {
            public override int MinColumns { get { return 2; } }
            public override int MaxColumns { get { return 10; } }
            public override string Function {get {return "Timeline";} }
            public override string Package { get { return "timeline"; } }
            public override ChartTypes ChartType { get { return ChartTypes.TimeLine; } }
            public override bool[] RespectNumbers
            {
                get { return new bool[2] { false, true }; }
            }

        }
    }
}
