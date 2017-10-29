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
    class LocalizationTest
    {
        [Test]
        public void Localization()
        {
            Assert.That(() => DBM.Utilities.LocalizationXML(null, null), Throws.TypeOf<FileNotFoundException>());
        }

        [TestCase("C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\DBM\\bin\\Release\\Localization\\en.xml", "C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\DBM\\bin\\Release\\Localization\\Lang\\en.txt")]
        public void Localization(string XML, string DataFile)
        {
            DBM.Utilities.LocalizationXML(XML, DataFile);
            Assert.IsNotNull(DBM.Utilities.Localization);
        }
    }

    [TestFixture]
    class SettingsTest
    {
        [Test]
        public void Load()
        {
            Assert.That(() => Settings.LoadSettings(false, null), Throws.TypeOf < ArgumentNullException>());
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

        [TestCase("C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.db", "Select * FROM \"Sacramento realestate transactions\";", "Sacramento realestate transactions", "C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.html")]
        public void HTML(string URI,string Query,string Title,string HTMLPath)
        {
            Load(URI, Query);
            string HTML = Export.HTML(Data, Schema, Title,  "DBM C# V1240"); //TODO
            Assert.AreEqual(HTML, System.IO.File.ReadAllText(HTMLPath));
        }

        [TestCase("C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.db", "Select * FROM \"Sacramento realestate transactions\";", "Sacramento realestate transactions", "C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.csv")]
        public void CSV(string URI, string Query, string Title, string TestPath)
        {
            Load(URI, Query);
            string CSV = Microsoft.SmallBasic.Library.File.GetTemporaryFilePath();
            Export.CSV(Data, Schema, CSV, ",");
            Assert.AreEqual(System.IO.File.ReadAllText(CSV), System.IO.File.ReadAllText(TestPath));

        }

        [TestCase("C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.db", "Select * FROM \"Sacramento realestate transactions\";", "Sacramento realestate transactions", "C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.xml")]
        public void XML(string URI, string Query, string Title, string TestPath)
        {
            Load(URI, Query);
            string XML = Microsoft.SmallBasic.Library.File.GetTemporaryFilePath();
            Export.XML(Data, Schema, Title, XML);
            Assert.AreEqual(System.IO.File.ReadAllText(XML), System.IO.File.ReadAllText(TestPath));
        }

        [TestCase("C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.db", "Select * FROM \"Sacramento realestate transactions\";", "Sacramento realestate transactions", "C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.md")]
        public void MarkDown(string URI, string Query, string Title, string TestPath)
        {
            Load(URI, Query);
            string MD = Export.MarkDown(Data, Schema);
            Assert.AreEqual(MD, System.IO.File.ReadAllText(TestPath));
        }

        [TestCase("C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.db", "Select * FROM \"Sacramento realestate transactions\";", "Sacramento realestate transactions", "C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\Test Files\\Sacramento realestate transactions.markup")]
        public void MarkUp(string URI, string Query, string Title, string TestPath)
        {
            Load(URI, Query);
            string MU = Export.MarkUp(Data, Schema);
            Assert.AreEqual(MU, System.IO.File.ReadAllText(TestPath));
        }

        public void Load(string URI, string Query)
        {
            string DB = LDDataBase.ConnectSQLite(URI);
            Data = LDDataBase.Query(DB, Query, null, true);
            Schema = Export.GenerateSchemaFromQueryData(Data);
        }
    }
}
