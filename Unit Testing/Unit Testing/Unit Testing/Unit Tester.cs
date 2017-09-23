using System;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using DBM;
using LitDev;
using Microsoft.SmallBasic.Library;

namespace Unit_Testing
{
    class Program
    {
        static void Main(string[] args)
        {
        } 
    }

    [TestFixture]
    class ImportTest
    {
        [Test]
        public void CSV()
        {
            Assert.That( () => Import.CSV(""), Throws.TypeOf<FileNotFoundException>());
        }
    }

    [TestFixture]
    class ExportTest
    {
        static Primitive Schema;
        static Primitive Data;

        [TestCase("C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.db", "Select * FROM \"Sacramento realestate transactions\";","Sacramento realestate transactions", "C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.json")]
        public void JSON(string URI,string Query,string Title,string JSONPath)
        {
            Load(URI, Query);
            string JSON = Export.JSON(Data, Schema, Title);
            Assert.AreEqual(JSON, System.IO.File.ReadAllText(JSONPath));
        }

        public void XML()
        {

        }

        [TestCase("C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.db", "Select * FROM \"Sacramento realestate transactions\";", "Sacramento realestate transactions", "C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.html")]
        public void HTML(string URI,string Query,string Title,string HTMLPath)
        {
            Load(URI, Query);
            string HTML = Export.HTML(Data, Schema, Title, "DBM C# V1240"); //TODO
            Assert.AreEqual(HTML, System.IO.File.ReadAllText(HTMLPath));
        }

        public void CSV()
        {

        }

        public void MarkDown() { }

        public void MarkUp() { }

        public void Load(string URI, string Query)
        {
            string DB = LDDataBase.ConnectSQLite(URI);
            Data = LDDataBase.Query(DB, Query, null, true);
            Schema = Export.GenerateSchemaFromQueryData(Data);
        }


    }
}
