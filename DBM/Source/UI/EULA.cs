using LitDev;
using Microsoft.SmallBasic.Library;
namespace DBM
{
    public static class EULA
	{
		static string CheckBox, Accept, Decline;

		public static void UI(string FilePath,decimal Ping,string Title,string CopyrightDate,string ProductID)
		{
            int StackPointer = Stack.Add("EULA.UI()");
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
                Events.LogMessage(Language.Localization["Failed Load Online EULA"], Language.Localization["Error"]);
			}

			if (string.IsNullOrWhiteSpace(CNTS)) //TODO
			{
                System.Environment.Exit(2);
			}
		
			LDControls.RichTextBoxSetText(EulaTextBox, CNTS, false);
			CheckBox = LDControls.AddCheckBox("I have read and agree to this EULA.");
			Accept = Controls.AddButton("Accept", 235, 390);
			Decline = Controls.AddButton("Decline", 315, 390);
			Controls.Move(CheckBox, 190, 365);
			Controls.SetSize(Accept, 70, 30);
			Controls.SetSize(Decline, 70, 30);
			Controls.ButtonClicked += Handler;
            Stack.Exit(StackPointer);
		}

		public static void Handler()
		{
            int StackPointer = Stack.Add("EULA.Handler()");
			string lastButton = Controls.LastClickedButton;
			GlobalStatic.Settings["VersionID"] = GlobalStatic.VersionID;

            Controls.ButtonClicked -= Handler; //Unsubcribes the event Handler from the event

            //Forced to use Else If due to non-constant values
            if (lastButton == Accept)
			{
                GlobalStatic.Settings["EULA"] = string.Format("Signed=true;Signer={0};", GlobalStatic.UserName);
				Settings.Save();
				DBM.UI.StartupGUI();
                Stack.Exit(StackPointer);
			}
			else if (lastButton == Decline)
			{
				GlobalStatic.Settings["EULA"] = false;
				Settings.Save();
				Events.LogMessage("EULA Declined", Language.Localization["UI"]); //Localize
                Stack.Exit(StackPointer);
                System.Environment.Exit(5);
			}
		}
	}
}