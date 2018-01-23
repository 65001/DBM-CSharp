using System.IO;
using Microsoft.SmallBasic.Library;
using LitDev;

namespace DBM
{
    public static partial class UI
    {
        public static class Settings
        {
            public static void Display()
            {
                int StackPointer = Stack.Add("UI.Settings.Display()");
                ClearWindow();
                GraphicsWindow.Title = Language.Localization["Settings"];

                GraphicsWindow.CanResize = false;
                LDGraphicsWindow.CancelClose = true;
                LDGraphicsWindow.ExitOnClose = false;
                LDGraphicsWindow.Closing += Events.Closing;
                LDGraphicsWindow.ExitButtonMode(Language.Localization["Settings"], "Disabled");

                GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize + 8;

                GraphicsWindow.DrawText(10, 10, Language.Localization["Listview Width"]);
                _TextBox["Settings_Width"] = Controls.AddTextBox(200, 10);

                GraphicsWindow.DrawText(10, 50, Language.Localization["Listview Height"]);
                _TextBox["Settings_Height"] = Controls.AddTextBox(200, 50);

                GraphicsWindow.DrawText(10, 90, Language.Localization["Extensions"]);
                _TextBox["Settings_Extensions"] = Controls.AddTextBox(200, 90);

                GraphicsWindow.DrawText(10, 130, Language.Localization["Deliminator"]);
                _TextBox["Settings_Deliminator"] = Controls.AddTextBox(200, 130);

                GraphicsWindow.DrawText(10, 175, Language.Localization["Language"]);

                GlobalStatic.ComboBox["Language"] = LDControls.AddComboBox(Language.ISO_Text.ToPrimitiveArray(), 200, 120);
                Controls.Move(GlobalStatic.ComboBox["Language"], 200, 175);

                GraphicsWindow.DrawText(10, 280, Language.Localization["LOG CSV Path"]);
                _Buttons.AddOrReplace("Log_CSV", Controls.AddButton(Language.Localization["Browse"], 320, 280));

                GraphicsWindow.DrawText(10, 330, Language.Localization["LOG DB PATH"]);
                _Buttons.AddOrReplace("Log_DB", Controls.AddButton(Language.Localization["Browse"], 320, 330));

                GraphicsWindow.DrawText(10, 380, Language.Localization["Transaction DB Path"]);
                _Buttons.AddOrReplace("Transaction_DB", Controls.AddButton(Language.Localization["Browse"], 320, 380));

                for (int i = 0; i < Language.ISO_LangCode.Count; i++)
                {
                    if (Language.ISO_LangCode[i] == GlobalStatic.LanguageCode)
                    {
                        int Index = i + 1;
                        LDControls.ComboBoxSelect(GlobalStatic.ComboBox["Language"], Index);
                    }
                }

                _Buttons.AddOrReplace("Settings Save", Controls.AddButton(Language.Localization["Save and Close"], 50, 450));
                _Buttons.AddOrReplace("Settings Close", Controls.AddButton(Language.Localization["Close wo saving"], 50, 500));

                Controls.SetSize(_Buttons["Settings Save"], 280, 40);
                Controls.SetSize(_Buttons["Settings Close"], 280, 40);

                Controls.SetTextBoxText(_TextBox["Settings_Width"], GlobalStatic.Listview_Width);
                Controls.SetTextBoxText(_TextBox["Settings_Height"], GlobalStatic.Listview_Height);
                Controls.SetTextBoxText(_TextBox["Settings_Extensions"], GlobalStatic.Extensions);
                Controls.SetTextBoxText(_TextBox["Settings_Deliminator"], GlobalStatic.Deliminator);

                GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;

                LDControls.ComboBoxItemChanged -= Events.CB;
                Controls.ButtonClicked -= Events.BC;
                Controls.ButtonClicked += Handler;
                Stack.Exit(StackPointer);
            }

            public static void Clear()
            {
                int StackPointer = Stack.Add("UI.Settings.Clear()");
                GlobalStatic.ListView = null;
                GlobalStatic.Dataview = null;
                MenuList = null;

                Controls.ButtonClicked -= Handler;
                Controls.ButtonClicked += Events.BC;
                ClearWindow();
                PreMainMenu();
                HideDisplayResults();
                LDControls.ComboBoxItemChanged += Events.CB;
                MainMenu();
                Stack.Exit(StackPointer);
            }

            static void Handler()
            {
                int StackPointer = Stack.Add("UI.Settings.Handler()");
                Button(Controls.LastClickedButton);
                Stack.Exit(StackPointer);
            }

            static void Button(string LastClickedButton)
            {
                int StackPointer = Stack.Add($"UI.Settings.Button({LastClickedButton})");
                if (LastClickedButton == _Buttons["Settings Save"])
                {
                    GlobalStatic.Settings["Listview"] = string.Format("Width={0};Height={1};", Controls.GetTextBoxText(_TextBox["Settings_Width"]), Controls.GetTextBoxText(_TextBox["Settings_Height"]));
                    GlobalStatic.Settings["Extensions"] = Controls.GetTextBoxText(_TextBox["Settings_Extensions"]);
                    GlobalStatic.Settings["Deliminator"] = Controls.GetTextBoxText(_TextBox["Settings_Deliminator"]);
                    GlobalStatic.Settings["Language"] = Language.ISO_LangCode[LDControls.ComboBoxGetSelected(GlobalStatic.ComboBox["Language"]) - 1];
                    GlobalStatic.LanguageCode = GlobalStatic.Settings["Language"];
                    DBM.Settings.Save();
                    DBM.Settings.Load(GlobalStatic.RestoreSettings, GlobalStatic.SettingsPath);

                    Language.Load(
                        Path.Combine(GlobalStatic.LocalizationFolder, GlobalStatic.LanguageCode + ".xml"),
                        Path.Combine(GlobalStatic.Localization_LanguageCodes_Path, GlobalStatic.LanguageCode + ".txt")
                        );

                    Button(_Buttons["Settings Close"]);
                    Stack.Exit(StackPointer);
                    return;
                }
                else if (LastClickedButton == _Buttons["Settings Close"])
                {
                    Clear();
                    Stack.Exit(StackPointer);
                    return;
                }
            }
        }
    }
}