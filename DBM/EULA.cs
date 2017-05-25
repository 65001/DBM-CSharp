using LitDev;
using Microsoft.SmallBasic.Library;
//using SBFile = Microsoft.SmallBasic.Library.File;
namespace DBM
{
    public static class EULA
	{
		static string CheckBox, Accept, Decline;

		public static void UI(string FilePath,decimal Ping,string Title,string CopyrightDate,string ProductID)
		{
            Utilities.AddtoStackTrace( "EULA.UI()");
			GraphicsWindow.Show();
			GraphicsWindow.Left = Desktop.Width / 3;
			GraphicsWindow.Top = Desktop.Height / 4;
			GraphicsWindow.Title = Title + "EULA";

			LDControls.RichTextBoxReadOnly = true;
			string EulaTextBox = LDControls.AddRichTextBox(600, 350);
			LDControls.RichTextBoxReadOnly = false;
            Controls.Move(EulaTextBox, 10, 10);
            string CNTS = System.IO.File.ReadAllText(FilePath).Replace("<date>", CopyrightDate).Replace("<product>",ProductID);

			if (Ping == -1) // DEV //TODO
			{
                Events.LogMessage(Utilities.Localization["Failed Load Online EULA"], Utilities.Localization["Error"]);
			}

			if (string.IsNullOrWhiteSpace(CNTS)) //TODO
			{
				Program.End();
			}
		
			LDControls.RichTextBoxSetText(EulaTextBox, CNTS, false);
			CheckBox = LDControls.AddCheckBox("I have read and agree to this EULA.");
			Accept = Controls.AddButton("Accept", 235, 390);
			Decline = Controls.AddButton("Decline", 315, 390);
			Controls.Move(CheckBox, 190, 365);
			Controls.SetSize(Accept, 70, 30);
			Controls.SetSize(Decline, 70, 30);
			Controls.ButtonClicked += Handler;
		}

		public static void Handler()
		{
            Utilities.AddtoStackTrace( "EULA.Handler()");
			string lastButton = Controls.LastClickedButton;
			GlobalStatic.Settings["EULA_By"] = GlobalStatic.UserName;
			//GlobalStatic.Settings["EULA_Version"] = GlobalStatic.EULA_Newest_Version;
			GlobalStatic.Settings["VersionID"] = GlobalStatic.VersionID.Replace(".","");

            Controls.ButtonClicked -= Handler; //Unsubcribes the event Handler from the event

            //Forced to use Else If due to non-constant values
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
				Events.LogMessage("EULA Declined", Utilities.Localization["UI"]); //Localize
				Program.End();
			}
		}
	}
}