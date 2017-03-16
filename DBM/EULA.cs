using System;
using LitDev;
using Microsoft.SmallBasic.Library;
using SBArray = Microsoft.SmallBasic.Library.Array;
using SBFile = Microsoft.SmallBasic.Library.File;
using System.Collections.Generic;
using System.IO;
namespace DBM
{
	public class EULA
	{
		public static string CheckBox, Accept, Decline;
		public static void UI(string URI)
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "EULA.UI()");
			GraphicsWindow.Show();
			GraphicsWindow.Left = Desktop.Width / 3;
			GraphicsWindow.Top = Desktop.Height / 4;
			GraphicsWindow.Title = GlobalStatic.Title + "EULA";
			GlobalStatic.DefaultWidth = GraphicsWindow.Width; GlobalStatic.DefaultHeight = GraphicsWindow.Height;
			LDControls.RichTextBoxReadOnly = true;
			string EulaTextBox = LDControls.AddRichTextBox(600, 350);
			LDControls.RichTextBoxReadOnly = false;
			Controls.Move(EulaTextBox, 10, 10);
			string CNTS = LDText.Replace(SBFile.ReadContents(URI), "<date>", GlobalStatic.Copyright);

			if (GlobalStatic.Ping == -1) // DEV //TODO
			{

			}
			if (CNTS.Equals(null) == true || CNTS == "") //TODO
			{
				Program.End();
			}
			else
			{
				LDControls.RichTextBoxSetText(EulaTextBox, CNTS, false);
				CheckBox = LDControls.AddCheckBox("I have read and agree to this EULA.");
				Accept = Controls.AddButton("Accept", 235, 390);
				Decline = Controls.AddButton("Decline", 315, 390);
				Controls.Move(CheckBox, 190, 365);
				Controls.SetSize(Accept, 70, 30);
				Controls.SetSize(Decline, 70, 30);
			}
			Controls.ButtonClicked += EULA.Handler;
		}
		public static void Handler()
		{
			LDList.Add(GlobalStatic.List_Stack_Trace, "EULA.Handler()");
			string lastButton = Controls.LastClickedButton;
			GlobalStatic.Settings["EULA_By"] = GlobalStatic.UserName;
			GlobalStatic.Settings["EULA_Version"] = GlobalStatic.EULA_Newest_Version;
			GlobalStatic.Settings["VersionID"] = GlobalStatic.VersionID;
			if (lastButton == Accept)
			{
				GlobalStatic.Settings["EULA"] = true;
				Settings.SaveSettings();
				DBM.UI.StartupGUI();
			}
			else if (lastButton == Decline)
			{
				GlobalStatic.Settings["EULA"] = false;
				Settings.SaveSettings();
				Events.LogMessage("EULA Declined", GlobalStatic.LangList["UI"]); //Localize
				Program.End();
			}

			Controls.ButtonClicked -= EULA.Handler; //Unsubcribes the event Handler from the event
		}
	}
}
