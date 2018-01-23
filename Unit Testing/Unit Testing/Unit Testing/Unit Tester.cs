using System;
using System.Diagnostics;
using System.IO;
using Google_Charts;
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
            Assert.That(() => Language.Load(null, null), Throws.TypeOf<FileNotFoundException>());
        }

        [TestCase("C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\DBM\\bin\\Release\\Localization\\en.xml", "C:\\Users\\Abhishek\\Documents\\Projects\\DBM\\DBM\\bin\\Release\\Localization\\Lang\\en.txt")]
        public void Localization(string XML, string DataFile)
        {
            Language.Load(XML, DataFile);
            Assert.IsNotNull(DBM.Language.Localization);
        }
    }

    [TestFixture]
    class GQ
    {
        [Test]
        public void NullGQSTest()
        {
            Assert.That(() => DBM.Engines.GenerateQuery(null, "Test"), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void NullTableTest()
        {
            var GQS = new Engines.GenerateQuerySettings();
            Assert.That(() => Engines.GenerateQuery(GQS, null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void WhiteSpaceTableTest()
        {
            var GQS = new Engines.GenerateQuerySettings();
            Assert.That(() => Engines.GenerateQuery(GQS, string.Empty), Throws.TypeOf<ArgumentNullException>());
        }

        
        [TestFixture]
        public class Search
        {
            [TestCase("ID", "10", "Log")]
            public void LikeSearch(string Column, string Text, string Table)
            {
                var GQS = new Engines.GenerateQuerySettings
                {
                    Search = true,
                    SearchBy = Column,
                    SearchText = Text
                };
                Assert.AreEqual(Engines.GenerateQuery(GQS, Table), string.Format("SELECT * FROM \"{0}\" WHERE \"{1}\" LIKE \'%{2}%\' ",Table,Column,Text));
            }

            [TestCase("ID", "10", "Log")]
            public void StrictSearch(string Column, string Text, string Table)
            {
                var GQS = new Engines.GenerateQuerySettings
                {
                    Search = true,
                    StrictSearch = true,
                    SearchBy = Column,
                    SearchText = Text
                };
                Assert.AreEqual(Engines.GenerateQuery(GQS, Table), string.Format("SELECT * FROM \"{0}\" WHERE \"{1}\"='{2}' ",Table,Column,Text));
            }

            [TestCase("ID","10","Log")]
            public void InvertSearchStrict(string Column, string Text, string Table)
            {
                var GQS = new Engines.GenerateQuerySettings
                {
                    Search = true,
                    StrictSearch = true,
                    InvertSearch = true,
                    SearchBy = Column,
                    SearchText = Text
                };
                Assert.AreEqual(Engines.GenerateQuery(GQS, Table), string.Format( "SELECT * FROM \"{0}\" WHERE \"{1}\"!='{2}' ",Table,Column,Text));
            }

            [TestCase("ID","10","Log")]
            public void InvertSearchLike(string Column,string Text,string Table)
            {
                var GQS = new Engines.GenerateQuerySettings
                {
                    Search = true,
                    InvertSearch = true,
                    SearchBy = Column,
                    SearchText = Text
                };
                Assert.AreEqual(Engines.GenerateQuery(GQS, Table), string.Format("SELECT * FROM \"{0}\" WHERE \"{1}\" NOT LIKE '%{2}%' ",Table.SanitizeFieldName(),Column.SanitizeFieldName(),Text.SanitizeFieldName()));
            }
        }

        [TestFixture]
        public class Sort
        {
            [TestCase("ID", "Log")]
            [TestCase("city","SA Realestate")]
            public void ASC(string Column, string Table)
            {
                var GQS = new Engines.GenerateQuerySettings
                {
                    Sort = true,
                    OrderBy = Column,
                    Order = Engines.GenerateQuerySettings.SortOrder.Ascending
                };
                Assert.AreEqual(Engines.GenerateQuery(GQS, Table),string.Format("SELECT * FROM \"{0}\" ORDER BY \"{1}\" ASC;", Table.SanitizeFieldName(), Column.SanitizeFieldName()));
            }

            [TestCase("ID", "Log")]
            public void DESC(string Column,string Table)
            {
                var GQS = new Engines.GenerateQuerySettings
                {
                    Sort = true,
                    OrderBy = Column,
                    Order = Engines.GenerateQuerySettings.SortOrder.Descding
                };
                Assert.AreEqual(Engines.GenerateQuery(GQS, Table), string.Format("SELECT * FROM \"{0}\" ORDER BY \"{1}\" DESC;",Table.SanitizeFieldName(),Column.SanitizeFieldName()));
            }

            [TestCase("ID","Log")]
            public void Random(string Column,string Table)
            {
                var GQS = new Engines.GenerateQuerySettings
                {
                    Sort = true,
                    OrderBy = Column,
                    Order = Engines.GenerateQuerySettings.SortOrder.Random
                };
                Assert.AreEqual(Engines.GenerateQuery(GQS, Table), string.Format("SELECT * FROM \"{0}\" ORDER BY RANDOM();",Table.SanitizeFieldName()));
            }
        }

        [TestFixture]
        public class Function
        {
            [TestCase("AVG","ID","Log")]
            public void Functions(string Function,string Column,string Table)
            {
                var GQS = new Engines.GenerateQuerySettings
                {
                    RunFunction = true,
                    FunctionSelected = Function,
                    FunctionColumn = Column
                };
                Assert.AreEqual(Engines.GenerateQuery(GQS, Table),string.Format("SELECT {0}(\"{1}\") FROM \"{2}\"",Function,Column.SanitizeFieldName(),Table.SanitizeFieldName()));
            }
        }
    }

    [TestFixture]
    class SettingsTest
    {
        [Test]
        public void Load()
        {
            Assert.That(() => Settings.Load(false, null), Throws.TypeOf < ArgumentNullException>());
        }
    }

    [TestFixture]
    class ImportTest
    {
        [TestFixture]
        public class CSV
        {
        }
    }

    [TestFixture]
    class ChartTest
    {
        [Test]
        public void AddColumns()
        {
            Google_Charts.Chart chart = new Google_Charts.Chart.Bar();
            chart.AddColumn("President");
            chart.AddColumn("Money", Google_Charts.Chart.DataType.number);
            Assert.Pass();
        }

        [Test]
        public void AddData()
        {
            Google_Charts.Chart chart = new Google_Charts.Chart.Bar();
            chart.AddColumn("President");
            chart.AddColumn("Money", Google_Charts.Chart.DataType.number);
            chart.AddRowData(1,"Obama");
            chart.AddRowData(2, "1000");
            Assert.Pass();
        }
    }

    [TestFixture]
    class ExportTest //TODO Add a good Export Test for HTML
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
