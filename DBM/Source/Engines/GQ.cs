using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBM
{
    public static partial class Engines
    {
        public struct GenerateQuerySettings
        {
            public bool Search;
            public bool StrictSearch;
            public bool InvertSearch;
            public bool Sort;
            public bool RunFunction;

            /// <summary>
            /// Column
            /// </summary>
            public string SearchBy;
            public string SearchText;

            /// <summary>
            /// Column
            /// </summary>
            public string OrderBy;
            /// <summary>
            /// ASC,DESC,Random etc
            /// </summary>
            
            public enum SortOrder { Ascending,Descding,Random };
            public SortOrder Order;

            public string FunctionSelected;
            public string FunctionColumn;
        }


        public static string GenerateQuery(GenerateQuerySettings GQS, string CurrentTable)
        {
            int StackReference = Stack.Add("Engines.GenerateQuery()");
            if (string.IsNullOrEmpty(CurrentTable))
            {
                throw new ArgumentNullException();
            }

            GQ_CMD = string.Format("SELECT * FROM \"{0}\" ", CurrentTable.SanitizeFieldName());
            
            if (GQS.Search)
            {
                GQ_CMD += GenerateSearch(GQS.SearchBy, GQS.SearchText, GQS.InvertSearch, GQS.StrictSearch);
            }
            if (GQS.RunFunction)
            {
                GQ_CMD = GenerateFunction(GQS.FunctionSelected, GQS.FunctionColumn,CurrentTable);
            }
            if (GQS.Sort)
            {
                GQ_CMD += GenerateSort(GQS.OrderBy, GQS.Order);
            }
            Console.WriteLine("Generated Query :{0}", GQ_CMD);
            Stack.Exit(StackReference);
            return GQ_CMD;
        }

        static string GenerateSearch(string SearchColumn, string SearchText, bool InvertSearch, bool StrictSearch)
        {
            string CMD;
            CMD = string.Format("WHERE \"{0}\"", SearchColumn.SanitizeFieldName());
            if (InvertSearch == true && StrictSearch == false)
            {
                CMD += " NOT";
            }

            if (StrictSearch == false)
            {
                CMD += string.Format(" LIKE '%{0}%' ",SearchText);
            }
            else
            {
                if (InvertSearch)
                {
                    CMD += "!='" + SearchText + "' ";
                }
                else
                {
                    CMD += "='" + SearchText + "' ";
                }
            }
            return CMD;
        }

        static string GenerateSort(string OrderBy,GenerateQuerySettings.SortOrder SO2)
        {
            if (string.IsNullOrWhiteSpace(OrderBy))
            {
                return ";";
            }

            switch (SO2)
            {
                case GenerateQuerySettings.SortOrder.Ascending:
                    return string.Format("ORDER BY \"{0}\" ASC;",OrderBy.SanitizeFieldName());
                case GenerateQuerySettings.SortOrder.Descding:
                    return string.Format("ORDER BY \"{0}\" DESC;", OrderBy.SanitizeFieldName());
                case GenerateQuerySettings.SortOrder.Random:
                    return "ORDER BY RANDOM();";
                default:
                    throw new ArgumentException();
            }
        }

        static string GenerateFunction(string Function, string Column,string CurrentTable)
        {
            return string.Format("SELECT {0}(\"{1}\") FROM \"{2}\"", Function, Column.SanitizeFieldName(), CurrentTable.SanitizeFieldName());
        }
    }
}
