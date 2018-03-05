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
        public interface IEngine
        {
            string GetSchema();
            string GetColumnsOfTable(string Table);
            List<string> GetColumnsOfTable(Primitive Data);
            Dictionary<string, string> GetTypes(Primitive SchemaQuery, Primitive Schema);
        }

        public class SQLite : IEngine
        {
            public string GetSchema()
            {
                return "SELECT tbl_name,name,type FROM sqlite_master UNION Select tbl_name,name,type From SQLite_Temp_Master;";
            }

            public string GetColumnsOfTable(string Table)
            {
                return string.Format("PRAGMA table_info(\"{0}\");", Table.SanitizeFieldName());
            }

            public List<string> GetColumnsOfTable(Primitive Data)
            {
                List<string> Schema = new List<string>();
                for (int i = 1; i <= Data.GetItemCount(); i++)
                {
                    Schema.Add(Data[i]["name"]);
                }
                return Schema;
            }

            public Dictionary<string, string> GetTypes(Primitive SchemaQuery,Primitive Schema)
            {
                int SchemaQueryCount = SchemaQuery.GetItemCount();
                int SchemaCount = Schema.GetItemCount();
                Dictionary<string, string> _Dictionary = new Dictionary<string, string>();

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