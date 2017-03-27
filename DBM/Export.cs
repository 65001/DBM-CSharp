// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
// Created : 3/14/2017 5:55 PM 2017314 17:55:44
using System;
using Microsoft.SmallBasic.Library;
namespace DBM
{
    public class Export
    {
        public static Primitive Generate2DArray(string Database, string SQL)
        {
            return Engines.Query(Database, SQL, null, true, GlobalStatic.UserName, ""); //TODO
        }

        public static Primitive Generate2DArrayFromTable(string Database, string Table)
        {
            return Generate2DArray(Database, "Select * From " + Table);
        }

        public static Primitive Generate2DArrayFromCurrentTable()
        {
            return Generate2DArrayFromTable(Engines.CurrentDatabase, Engines.CurrentTable);
        }
    
        public static void CSV(Primitive Data,Primitive Schema,string FilePath) //TODO
        {

        }

        public static void SQL(Primitive Data,Primitive Schema,string FilePath) //TODO
        {

        }

        public static void XML(Primitive Data,Primitive Schema,string FilePath) //TODO
        {

        }

        public static void HTML(Primitive Data,Primitive Schema,string FilePath) //TODO
        {

        }
	}
}
