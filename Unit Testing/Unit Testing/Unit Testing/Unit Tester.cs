using System;
using System.IO;
using DBM;
using LitDev;
using Microsoft.SmallBasic;
using NUnit.Framework;

namespace Unit_Testing
{
    class Program
    {
        static void Main(string[] args)
        {
        } 
    }

    [TestFixture]
    class Import
    {
        [Test]
        public void CSV()
        {
            Assert.That( () => DBM.Import.CSV(""), Throws.TypeOf<FileNotFoundException>());
        }
    }

    [TestFixture]
    class Export
    {

    }
}
