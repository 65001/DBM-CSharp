// asathiabalan@gmail.com
// Author : Abhishek Sathiabalan
// (C) 2016 - 2017. All rights Reserved. Goverened by Included EULA
using System;
using Microsoft.SmallBasic.Library;
using NUnit.Framework;
namespace DBM
{
	[TestFixture]
	public class UnitTests
	{

		[Test]
		public void A_Load_Settings()
		{
			Settings.LoadSettings();
			GraphicsWindow.ShowMessage(GlobalStatic.SettingsPath, "");
			Assert.Warn(GlobalStatic.SettingsPath);
		}

		[Test]
		public void StartUp()
		{
			UI.Main();
			Assert.True(true);
		}

		[Test]
		public void Connect()
		{
			Engines.Load_DB(Engines.EnginesModes.SQLITE, LitDev.LDDialogs.OpenFile("db", ""));
			Assert.True(true);    
		}

		[Test]
		public void End()
		{
			Assert.Fail();
		}
	}
}
