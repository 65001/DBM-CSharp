using System;
using System.Diagnostics;
using LitDev;

namespace DBM
{
    public partial class Engines
    {
        public struct CommandSettings
        {
            public string Database;
            public string SQL;
            public string User;
            public string Explanation;
        }

        public static int Command(CommandSettings CS)
        {
            return Command(CS.Database, CS.SQL, CS.User, CS.Explanation);
        }

        /// <summary>
        /// Executes a Command against a database.
        /// </summary>
        /// <param name="Database">Database. Use the Database name you recieve from LoadDB</param>
        /// <param name="SQL"></param>
        /// <param name="User">UserName of the requested username</param>
        /// <param name="Explanation">Any notes for transactions</param>
        /// <param name="RunParser">Run Custom Parser... Yet to be implemented</param>
        /// <returns></returns>
        static int Command(string Database, string SQL, string User, string Explanation)
        {
            int StackPointer = Stack.Add($"Engines.Command({Database})");
            _UTC_Start.Add(DateTime.UtcNow.ToString("hh:mm:ss tt"));
            Stopwatch CommandTime = Stopwatch.StartNew();

            if (GlobalStatic.Transaction_Commands == true)
            {
                TransactionRecord(User, Database, SQL, Type.Command, Explanation);
            }

            int Updated = LDDataBase.Command(Database, SQL);
            _Type_Referer.Add(Type.Command);
            _Timer.Add(CommandTime.ElapsedMilliseconds);
            _Explanation.Add(Explanation);
            _User.Add(User);

            Stack.Exit(StackPointer);
            return Updated;
        }
    }
}
