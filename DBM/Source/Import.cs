// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
// Created : 3/14/2017 5:55 PM 2017314 17:55:30
using System;
using System.Text;
using System.Linq;
using System.IO;
using System.Data.SQLite;
using System.Collections.Generic;
using LitDev;
using System.Diagnostics;
namespace DBM
{
	public static class Import
	{
		static string Data; //Refers to an Instance of LDFASTARRAY
		static List<int> CSV_Length = new List<int>();
		static List<bool> CSV_IsString = new List<bool>();
		static string HeaderSQL;
		static string HeaderWOType;

        public static void CSV(string InputFilePath, string OutPutFilePath)
        {
            File.WriteAllText(OutPutFilePath, CSV(InputFilePath));
        }

		public static string CSV(string FilePath)
		{			
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

			try
			{
				string CSV_SQL = ArrayToSql(Standard_Size,Name);
                CSVHeaders(Standard_Size,Name);
				//Appending
				CSV_SQL = (HeaderSQL +"\n" + CSV_SQL).Replace("'NULL'","NULL").Replace("<<HEADERS>>", HeaderWOType);
                LDFastArray.Remove(Data);
                Console.WriteLine("Import.CSV time {0} ms", Elappsed.ElapsedMilliseconds);
                return CSV_SQL;
			}
			catch (Exception ex)
			{
                Events.LogMessage(ex.Message, "System");
			}

			//Drops The FastArray
			LDFastArray.Remove(Data);

        #if DEBUG
            Console.WriteLine("Import.CSV time {0} ms", Elappsed.ElapsedMilliseconds);
        #endif
            return string.Empty;
		}

		static string ArrayToSql(int Standard_Size,string TableName)
		{
			StringBuilder CSV_SQL = new StringBuilder();

			for (int i = 2; i <= LDFastArray.Size1(Data); i++)
			{
				if (CSV_Length[(i-1)] == Standard_Size) 
					//To Prevent out of bound eras
					//The Minus one is for the difference in data sets. C# counts from Zero and the LD from one
					//The pain of mixing these systems.
				{
                    CSV_SQL.Append("INSERT INTO \"" + TableName + "\" <<HEADERS>> VALUES('");
					for (int ii = 1; ii <= LDFastArray.Size2(Data, i); ii++)
					{
                        string Temp = LDFastArray.Get2D(Data, i, ii).ToString().Replace("'", "''").Replace("\n"," ");
                        if (string.IsNullOrWhiteSpace(Temp))
						{
							Temp = "NULL";
						}
						CSV_SQL.Append(Temp); //Adds the value of Temp to the Current SB
						Temp = null;

						if (CSV_IsString[(ii - 1)] == true)
						{
							if (double.TryParse(Temp, out double double2) == true) //Tests a String to see if its a number
							{
								CSV_IsString[ii] = false;
							}
						}

						if (ii < Standard_Size)
						{
                            CSV_SQL.Append("','");
						}
					}
				}

				CSV_SQL.AppendLine("');");
			}
			return CSV_SQL.ToString();
		}

		static void CSVHeaders(int Standard_Size,string TableName) 
		{
            HeaderSQL = "CREATE TABLE IF NOT EXISTS \"" + TableName + "\" (";
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
		}

        public static void SQL(string database,string Path)
        {
           string[] SQL = File.ReadAllLines(Path);
           var cnn = Engines.GetConnection(database);
           var cmd = new SQLiteCommand(cnn);

            SQLiteTransaction transaction;
            transaction = cnn.BeginTransaction();
            for (int i = 0; i < SQL.Length; i++)
            {
                //Every 100 lines commit and start a new transaction.
                if (i % 100 == 0)
                {
                    transaction.Commit();
                    transaction = cnn.BeginTransaction();
                }
                if (SQL[i] != "');")
                {
                    cmd.CommandText = SQL[i];
                    cmd.ExecuteNonQuery();
                }
            }
           transaction.Commit();
        }
	}
}