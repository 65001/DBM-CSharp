// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
// Created : 3/14/2017 5:55 PM 2017314 17:55:30

using LitDev;
using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;
using System.Collections.Generic;

namespace DBM
{
	public static partial class Import
	{
		static string Data; //Refers to an Instance of LDFASTARRAY
		static List<int> CSV_Length = new List<int>();
		static List<bool> CSV_IsString = new List<bool>();
		static string HeaderSQL;
		static string HeaderWOType;
        static int CSVInterval = GlobalStatic.CSVInterval;
        static int SQLInterval = GlobalStatic.SQLInterval;
        
        public static void CSV(string InputFilePath, string OutPutFilePath)
        {
            int StackPointer = Stack.Add($"Import.CSV({InputFilePath},{OutPutFilePath})");
            StreamWriter SW = new StreamWriter(OutPutFilePath,false);
            
            CSV(InputFilePath,ref SW);
            Stack.Exit(StackPointer);
        }
        

		public static void CSV(string FilePath,ref StreamWriter SW)
		{
            int StackPointer = Stack.Add($"Utilities.CSV({FilePath})");
			//TODO Make sure comment's are universal across SQL.Then use them to insert data such as how long it took to generate the SQL and how many rows were skipped if any?
			if (File.Exists(FilePath) == false)
			{
                throw new FileNotFoundException();
			}

			Stopwatch Elappsed = Stopwatch.StartNew();
            Elappsed.Start();

			CSV_Length.Clear();
			CSV_IsString.Clear();

			string Name = Path.GetFileNameWithoutExtension(FilePath).Trim();
			Data = LDFastArray.ReadCSV(FilePath); 

			//Calculate Lengths of Data
			for (int i = 1; i <= LDFastArray.Size1(Data); i++)
			{
				CSV_Length.Add(LDFastArray.Size2(Data, i));
			}
			int Standard_Size = CSV_Length.First();

			//Sets IsInteger to true by default
			for (int i = 1; i <= Standard_Size; i++)
			{
				CSV_IsString.Add(true);
			}

            //Tests all Data to see if it is a integer type
            for (int i = 2; i <= Standard_Size; i++)
            {
                //This prevent out of bound errors.
                //The Minus one is for the difference in data sets. C# counts from Zero and the LD from one
                if (CSV_Length[(i - 1)] != Standard_Size)
                {
                    continue;
                }

                for (int ii = 1; ii <= LDFastArray.Size2(Data, i); ii++)
                {
                    if (CSV_IsString[(ii - 1)] == false)
                    {
                        continue;
                    }

                    string Temp = LDFastArray.Get2D(Data, i, ii).ToString().Replace("'", "''").Replace("\n", " ");
                    if (double.TryParse(Temp, out double double2) == true) //Tests a String to see if its a number
                    {
                        CSV_IsString[ii] = false;
                     }
                }
            }

            try
			{
                CSVHeaders(Standard_Size, Name,ref SW);
                ArrayToSql(Standard_Size,Name,HeaderWOType,ref SW);
                LDFastArray.Remove(Data);
                Stack.Exit(StackPointer);
                SW.Close();
                return;
			}
			catch (Exception ex)
			{
                
                Events.LogMessagePopUp(ex.Message +"\n" + ex.StackTrace, "System","CSV Conversion Error");
			}

			//Drops The FastArray
			LDFastArray.Remove(Data);
            Stack.Exit(StackPointer);
            SW.Close();
            return;
		}

		static void ArrayToSql(int Standard_Size,string TableName,string Headers,ref StreamWriter SW)
		{
            int StackPointer = Stack.Add("Import.ArrayToSql");

			for (int i = 2; i <= LDFastArray.Size1(Data); i++)
			{
                //This prevent out of bound errors.
                //The Minus one is for the difference in data sets. C# counts from Zero and the LD from one
                if (CSV_Length[(i - 1)] != Standard_Size)
                {
                    continue;
                }

                SW.Write($"INSERT INTO \"{TableName}\" {Headers} VALUES(");
				for (int ii = 1; ii <= LDFastArray.Size2(Data, i); ii++)
				{
                    string Temp = "'" + LDFastArray.Get2D(Data, i, ii).ToString().Replace("'", "''").Replace("\n"," ");
                    if (string.IsNullOrWhiteSpace(Temp))
					{
						Temp = "NULL";
					}

					if (ii < Standard_Size)
					{
                        Temp += "',";
					}

                    if (Temp == "'NULL','")
                    {
                        SW.Write("NULL");
                    }
                    else
                    {
                        SW.Write(Temp);
                    }
				}

                SW.WriteLine("');");
                if (i % CSVInterval == 0)
                {
                    SW.Flush();
                }
			}
            SW.Flush();
            Stack.Exit(StackPointer);
		}

		static void CSVHeaders(int Standard_Size,string TableName,ref StreamWriter SW) 
		{
            int StackPointer = Stack.Add("Import.CSVHeaders");
            HeaderSQL = $"CREATE TABLE IF NOT EXISTS \"{TableName}\" (";
			HeaderWOType = "(";
			for (int i = 1; i <= Standard_Size; i++)
			{
				HeaderSQL += "\"" + LDFastArray.Get2D(Data, 1, i) + "\" ";
				HeaderWOType += "\"" + LDFastArray.Get2D(Data, 1, i) + "\"";

                switch (CSV_IsString[(i - 1)])
                {
                    case true:
                        HeaderSQL += "TEXT";
                        break;
                    case false:
                        HeaderSQL += "INTEGER";
                        break;
                }

				if (i < Standard_Size)
				{
					HeaderSQL += ",";
					HeaderWOType += ",";
				}
			}
			HeaderSQL += ");\n";
			HeaderWOType += ")";
            SW.WriteLine(HeaderSQL);
            SW.Flush();
            Stack.Exit(StackPointer);
		}

        public static void SQL(string database,string Path)
        {
            int StackPointer = Stack.Add($"Import.SQL({database},{Path})");
            StreamReader SR = new StreamReader(Path);

           var cnn = Engines.DB_Info[database].Connections.SQLITE;
           var cmd = new SQLiteCommand(cnn);

            SQLiteTransaction transaction;
            transaction = cnn.BeginTransaction();
            int Lines = 0;
            while (SR.EndOfStream == false)
            {
                string SQL = SR.ReadLine();
                //Every Interval commit and start a new transaction.
                if (Lines % SQLInterval == 0)
                {
                    transaction.Commit();
                    transaction = cnn.BeginTransaction();
                }
                if (SQL != "');")
                {
                    cmd.CommandText = SQL;
                    cmd.CommandTimeout = 20;
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    Console.WriteLine($"Error on {Lines} due to bad SQL");
                }
                Lines = Lines + 1;
            }
            transaction.Commit();
            //Disposing of appropriate resources
            //Do NOT dispose of the connection.
            transaction.Dispose();
            cmd.Dispose();
            SR.Dispose();
            Stack.Exit(StackPointer);
        }
	}
}