// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
// Created : 3/14/2017 5:55 PM 2017314 17:55:30
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using LitDev;
using System.Diagnostics;
using Microsoft.SmallBasic.Library;
namespace DBM
{
	public static class Import
	{
		static string Data; //Refers to an Instance of LDFASTARRAY
		static List<int> CSV_Length = new List<int>();
		static List<bool> CSV_IsString = new List<bool>();
		static string HeaderSQL;
		static string HeaderWOType;

		public static string CSV(string FilePath) //TODO
		{			
			//TODO Make sure comment's are universal across SQL.Then use them to insert data such as how long it took to generate the SQL and how many rows were skipped if any

			if (LDFile.Exists(FilePath) == false)
			{
                return string.Empty;
			}

			Stopwatch Elappsed =	Stopwatch.StartNew();
			Elappsed.Start();
			        
			CSV_Length.Clear();
			CSV_IsString.Clear();

			string Name = LDText.Trim(LDFile.GetFile(FilePath));
			Data = LDFastArray.ReadCSV(FilePath); 

			//Calculate Lengths of Data
			for (int i = 1; i <= LDFastArray.Dim1(Data); i++)
			{
				CSV_Length.Add(LDFastArray.Dim2(Data, i));
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
				CSV_SQL = "BEGIN;\n" + HeaderSQL + CSV_SQL + "COMMIT;";
				CSV_SQL = LDText.Replace(CSV_SQL, "'NULL'", "NULL");
				CSV_SQL = LDText.Replace(CSV_SQL, "<<HEADERS>>", HeaderWOType);
                return CSV_SQL;
			}
			catch (Exception ex)
			{
				GraphicsWindow.ShowMessage(ex.StackTrace, ex.Message);
			}

			//Drops The FastArray
			LDFastArray.Remove(Data);
            return string.Empty;            
		}

		static string ArrayToSql(int Standard_Size,string TableName)
		{
			double double2;
			StringBuilder CSV_SQL = new StringBuilder();

			for (int i = 2; i <= LDFastArray.Dim1(Data); i++)
			{
				if (CSV_Length[(i-1)] == Standard_Size) 
					//To Prevent out of bound eras
					//The Minus one is for the difference in data sets. One Counts from Zero and the other from one
					//The pain of mixing these systems.
				{
                    CSV_SQL.Append("INSERT INTO \"" + TableName + "\" <<HEADERS>> VALUES('");
					for (int ii = 1; ii <= LDFastArray.Dim2(Data, i); ii++)
					{
						string Temp = LDText.Replace(LDFastArray.Get2D(Data, i, ii), "'", "''");
						if (string.IsNullOrWhiteSpace(Temp))
						{
							Temp = "NULL";
						}
						CSV_SQL.Append(Temp); //Adds the value of Temp to the Current SB
						Temp = null;

						//Console.WriteLine("List Index {0} of {1} @ {2}", ii, CSV_IsString.Count,i);
						if (CSV_IsString[(ii - 1)] == true)
						{
							if (double.TryParse(Temp, out double2) == true) //Tests a String to see if its a number
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
				if (CSV_IsString[(i-1)])
				{
					HeaderSQL += "TEXT";
				}
				else
				{
					HeaderSQL += "INTEGER";
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

		public static void SQL(string FilePath) //TODO Import.SQL
		{ 
		
		}
	}
}
