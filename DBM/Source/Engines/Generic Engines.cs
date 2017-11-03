using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SmallBasic.Library;

namespace DBM
{
    public partial class Engines
    {
        private static class SQLITE
        {
            public static string GetSchema()
            {
                return "SELECT tbl_name,name,type FROM sqlite_master UNION Select tbl_name,name,type From SQLite_Temp_Master;";
            }

            public static string GetColumnsOfTable(string Table)
            {
                return string.Format("PRAGMA table_info(\"{0}\");", Table.SanitizeFieldName());
            }

            public static List<string> GetColumnsOfTable(Primitive Data)
            {
                List<string> Schema = new List<string>();
                for (int i = 1; i <= Data.GetItemCount(); i++)
                {
                    Schema.Add(Data[i]["name"]);
                }
                return Schema;
            }
        }

        private static class OLDEB
        {

        }

        private static class ODBC
        {

        }

        private static class OLEDB
        {

        }

        private static class SQLServer
        {

        }

        private static class MySQL
        {

        }
    }
}