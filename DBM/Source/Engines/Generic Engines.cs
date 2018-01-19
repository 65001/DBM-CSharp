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
        public abstract class Engine
        {
            public abstract string GetSchema();
            public abstract string GetColumnsOfTable(string Table);
            public abstract List<string> GetColumnsOfTable(Primitive Data);
        }

        public class SQLITE : Engine
        {
            public override string GetSchema()
            {
                return "SELECT tbl_name,name,type FROM sqlite_master UNION Select tbl_name,name,type From SQLite_Temp_Master;";
            }

            public override string GetColumnsOfTable(string Table)
            {
                return string.Format("PRAGMA table_info(\"{0}\");", Table.SanitizeFieldName());
            }

            public override List<string> GetColumnsOfTable(Primitive Data)
            {
                List<string> Schema = new List<string>();
                for (int i = 1; i <= Data.GetItemCount(); i++)
                {
                    Schema.Add(Data[i]["name"]);
                }
                return Schema;
            }
        }

        private class OLDEB
        {

        }

        private class ODBC
        {

        }

        private class OLEDB
        {

        }

        private class SQLServer
        {

        }

        private class MySQL
        {

        }
    }
}