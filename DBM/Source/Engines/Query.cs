using System;
using System.Diagnostics;
using Microsoft.SmallBasic.Library;
using LitDev;

namespace DBM
{
    public partial class Engines
    {
        public struct QuerySettings
        {
            public string Database;
            public string SQL;
            public string User;
            public string Explanation;
            public bool FetchRecords;
            public string ListView;
        }

        public static Primitive Query(QuerySettings QS)
        {
            return Query(
                QS.Database,
                QS.SQL,
                QS.ListView,
                QS.FetchRecords,
                QS.User,
                QS.Explanation
                );
        }

        static Primitive Query(string DataBase, string SQL, string ListView, bool FetchRecords, string UserName, string Explanation) //Expand
        {
            int StackPointer = Stack.Add("Engines.Query()");

            _UTC_Start.Add(DateTime.UtcNow.ToString("hh:mm:ss tt"));
            Stopwatch QueryTime = Stopwatch.StartNew();

            if (SQL.StartsWith("."))
            {
                Emulator(DB_Info[DataBase].Engine , DataBase, SQL, UserName, ListView);
                Stack.Exit(StackPointer);
                return null;
            }

            if (GlobalStatic.Transaction_Query == true)
            {
                TransactionRecord(UserName, DataBase, SQL, Type.Query, Explanation);
            }

            if (UseCache == false)
            {
                _CacheStatus.Add("Disabled");
            }
            else if (FetchRecords == false)
            {
                //The Cache can never be hit :(
                _CacheStatus.Add("NA");
            }

            Primitive QueryResults;

            if (UseCache == false && FetchRecords == true && string.IsNullOrWhiteSpace(ListView) == true && _Cache.ContainsKey(SQL) == true)
            {
                //Read data back from Cache
                _CacheStatus.Add("Hit!");
                QueryResults = _Cache[SQL].Results;
                _Cache[SQL].LifeTime -= 1;
                if (_Cache[SQL].LifeTime <= 0)
                {
                    _Cache.Remove(SQL);
                }
            }
            else
            {
                //Data is not in Cache :(
                QueryResults = LDDataBase.Query(DataBase, SQL, ListView, FetchRecords);
                if (UseCache == true && FetchRecords == true && _Cache.ContainsKey(SQL) == false)
                {
                    CacheEntry cache = new CacheEntry
                    {
                        LifeTime = 10,
                        Results = QueryResults
                    };
                    _Cache.Add(SQL, cache);
                    _CacheStatus.Add("Results added to cache");
                }
                else if (UseCache == true && _Cache.ContainsKey(SQL))
                {
                    _CacheStatus.Add("Error");
                }
            }

            _Type_Referer.Add(Type.Query);

            switch (Explanation)
            {
                case "SCHEMA-PRIVATE":
                case "SCHEMA":
                    break;
                default:
                    _Last_NonSchema_Query.Add(SQL);
                    break;
            }

            _Last_Query.Add(SQL);
            _Timer.Add(QueryTime.ElapsedMilliseconds);
            _Explanation.Add(Explanation);
            _User.Add(UserName);
            Stack.Exit(StackPointer);
            return QueryResults;
        }
    }
}
