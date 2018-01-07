using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using LitDev;


namespace DBM
{
    public partial class Engines
    {
        static List<SQLiteFunction> FunctionList = new List<SQLiteFunction>();

        static void Bind(this SQLiteConnection connection, SQLiteFunction function)
        {
            var attributes = function.GetType().GetCustomAttributes(typeof(SQLiteFunctionAttribute), true).Cast<SQLiteFunctionAttribute>().ToArray();
            if (attributes.Length == 0)
            {
                throw new InvalidOperationException("SQLiteFunction doesn't have SQLiteFunctionAttribute");
            }
            connection.BindFunction(attributes[0], function);
        }

        public static void AutoBind(this SQLiteConnection connection)
        {
            for (int i = 0; i < FunctionList.Count; i++)
            {
                try
                {
                    connection.Bind(FunctionList[i]);
                }
                catch (Exception ex)
                {
                    Events.LogMessage(ex.Message, Utilities.Localization["System"]);
                }
            }
        }

        public static void AddToBindList(SQLiteFunction function)
        {
            FunctionList.Add(function);
        }

        /// <summary>
        /// Automatically adds a list of custom functions to be binded on database creation :)
        /// </summary>
        public static void CreateBindList()
        {
            AddToBindList(new CustomFunctions.RegEx());
            AddToBindList(new CustomFunctions.Power());
            AddToBindList(new CustomFunctions.Sqrt());
            AddToBindList(new CustomFunctions.e());
            AddToBindList(new CustomFunctions.PI());
            AddToBindList(new CustomFunctions.Log());

            AddToBindList(new CustomFunctions.Sin());
            AddToBindList(new CustomFunctions.Sinh());
            AddToBindList(new CustomFunctions.Cos());
            AddToBindList(new CustomFunctions.Cosh());
            AddToBindList(new CustomFunctions.Tan());
            AddToBindList(new CustomFunctions.Tanh());
            AddToBindList(new CustomFunctions.Encrypt());
            AddToBindList(new CustomFunctions.Decrypt());
            AddToBindList(new CustomFunctions.Hash());
            AddToBindList(new CustomFunctions.Translate());
        }

        public static SQLiteConnection GetConnection(string DataBase)
        {
            for (int i = 0; i < LDDataBase.dataBases.Count; i++)
            {
                if (DataBase == LDDataBase.dataBases[i].name)
                {
                    return LDDataBase.dataBases[i].cnnSQLite;
                }
            }
            return null;
        }
    }

    public static class CustomFunctions
    {
        [SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class RegEx : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(args[1]), Convert.ToString(args[0]));
            }
        }

        [SQLiteFunction(Name = "POW", Arguments = 2, FuncType = FunctionType.Scalar)]
        [SQLiteFunction(Name = "POWER", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class Power : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                
                return Math.Pow( Double.Parse( args[0].ToString() ) , Double.Parse( args[1].ToString() ) );
            }
        }

        [SQLiteFunction(Arguments = 1, FuncType = FunctionType.Scalar, Name = "SQRT")]
        public class Sqrt : SQLiteFunction
        {
            public override object Invoke(object[] args)
            { 
                return Math.Sqrt(Double.Parse(args[0].ToString()));
            }
        }

        [SQLiteFunction(Arguments = 1, FuncType = FunctionType.Scalar, Name = "SIN")]
        public class Sin : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.Sin(Double.Parse(args[0].ToString()));
            }
        }

        [SQLiteFunction(Arguments = 1, FuncType = FunctionType.Scalar, Name = "SINH")]
        public class Sinh : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.Sinh(Double.Parse(args[0].ToString()));
            }
        }

        [SQLiteFunction(Arguments = 1, FuncType = FunctionType.Scalar, Name = "cos")]
        public class Cos : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.Cos(Double.Parse(args[0].ToString()));
            }
        }

        [SQLiteFunction(Arguments = 1, FuncType = FunctionType.Scalar, Name = "cosh")]
        public class Cosh : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.Cosh(Double.Parse(args[0].ToString()));
            }
        }

        [SQLiteFunction(Arguments = 1, FuncType = FunctionType.Scalar, Name = "tan")]
        public class Tan : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.Tan(Double.Parse(args[0].ToString()));
            }
        }

        [SQLiteFunction(Arguments = 1, FuncType = FunctionType.Scalar, Name = "tanh")]
        public class Tanh : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.Tanh(Double.Parse(args[0].ToString()));
            }
        }


        [SQLiteFunction(Arguments = 0, FuncType = FunctionType.Scalar, Name = "E")]
        public class e : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.E;
            }
        }

        [SQLiteFunction(Arguments = 0, FuncType = FunctionType.Scalar, Name = "pi")]
        public class PI : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.PI;
            }
        }

        [SQLiteFunction(Arguments = 1, FuncType = FunctionType.Scalar, Name = "log")]
        public class Log : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return Math.Log10(Double.Parse( args[0].ToString() ));
            }
        }

        [SQLiteFunction(Arguments = 2, FuncType = FunctionType.Scalar, Name = "encrypt")]
        public class Encrypt : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return LDEncryption.AESEncrypt(args[0].ToString(),args[1].ToString());
            }
        }

        [SQLiteFunction(Arguments = 2, FuncType = FunctionType.Scalar, Name = "decrypt")]
        public class Decrypt : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return LDEncryption.AESDecrypt(args[0].ToString(),args[1].ToString());
            }
        }

        [SQLiteFunction(Arguments = 1, FuncType = FunctionType.Scalar, Name = "hash")]
        public class Hash : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return LDEncryption.SHA512Hash(args[0].ToString() );
            }
        }

        [SQLiteFunction(Arguments = 3, FuncType = FunctionType.Scalar, Name = "translate")]
        public class Translate : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return LDTranslate.Translate(args[0].ToString(),args[1].ToString(),args[2].ToString());
            }
        }

    }
}