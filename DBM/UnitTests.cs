using System;
using Microsoft.SmallBasic.Library;
using NUnit.Framework;
namespace DBM
{
	[TestFixture]
	public class UnitTests
	{
		[Test]
		public void Load_Settings()
		{
			GraphicsWindow.ShowMessage(GlobalStatic.SettingsPath, "");
			Assert.Warn(GlobalStatic.SettingsPath);
			Settings.LoadSettings();
		}
		[Test]
		public void Connect()
		{
			Engines.Load_DB(4, LitDev.LDDialogs.OpenFile("db", ""));
			Assert.True(true);
			    
		}
	}
}
