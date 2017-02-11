using System;
using NUnit.Framework;
namespace DBM
{
	[TestFixture]
	public class UnitTests
	{
		[Test]
		public void Connect()
		{
			DBM.Engines.Load_DB(4, LitDev.LDDialogs.OpenFile("db", ""));
			Assert.True(true);
			    
		}
	}
}
