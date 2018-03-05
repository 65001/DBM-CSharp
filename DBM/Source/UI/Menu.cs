using System;
using LitDev;
using Microsoft.SmallBasic.Library;

namespace DBM
{
    public partial class UI
    {
        public static void PreMainMenu()
        {
            int StackReference = Stack.Add("UI.PreMainMenu()");
            GraphicsWindow.FontName = "Segoe UI";
            
            //Main
            MenuList[Language.Localization["File"]] = "Main";
            MenuList[Language.Localization["Edit"]] = "Main";
            MenuList[Language.Localization["View"] + " "] = "Main";
            MenuList[Language.Localization["Save"]] = "Main";
            MenuList[Language.Localization["Import"]] = "Main";
            MenuList[Language.Localization["Export"]] = "Main";
            MenuList[Language.Localization["Settings"]] = "Main";
            MenuList["Charts"] = "Main"; //TODO Localize

            //File
            MenuList[Language.Localization["New"]] = Language.Localization["File"];
            MenuList[Language.Localization["Open"]] = Language.Localization["File"];
            MenuList["Other"] = Language.Localization["File"]; // TODO Localize
            MenuList[Language.Localization["Define New Table"]] = "Other";
            MenuList[Language.Localization["New in Memory Db"]] = "Other";
            MenuList[Language.Localization["Create Statistics Page"]] = "Other";
            MenuList["-"] = Language.Localization["File"];

            //Import
            MenuList[Language.Localization["CSV"]] = Language.Localization["Import"];
            MenuList[Language.Localization["SQL"]] = Language.Localization["Import"];
            //MenuList["Converter"] = Language.Localization["Import"]; //Localize
            //MenuList["HTML to CSV"] = "Converter"; //Localize
            //MenuList["-"] = "Converter";
            MenuList["-"] = Language.Localization["Import"];

            //Export
            MenuList[Language.Localization["CSV"] + " "] = Language.Localization["Export"];
            MenuList[Language.Localization["HTML"] + " "] = Language.Localization["Export"];
            MenuList["JSON"] = Language.Localization["Export"]; //TODO Localize
            MenuList[Language.Localization["SQL"] + " "] = Language.Localization["Export"];
            MenuList[Language.Localization["PXML"] + " "] = Language.Localization["Export"];
            MenuList["MarkDown"] = Language.Localization["Export"]; //TODO Localize
            MenuList["Wiki MarkUp"] = Language.Localization["Export"]; //TODO Localize
            MenuList["-"] = Language.Localization["Export"];

            //Charts
            MenuList["Bar"] = "Charts";
            MenuList["Column"] = "Charts";
            MenuList["Geo"] = "Charts";
            MenuList["Histogram"] = "Charts";

            MenuList["Line"] = "Charts";
            MenuList["Org"] = "Charts";
            MenuList["Pie"] = "Charts";

            MenuList["Sankey"] = "Charts";
            MenuList["Scatter Plot"] = "Charts";
            MenuList["Sortable Table"] = "Charts";
            MenuList["TimeLine"] = "Charts";

            //Settings
            MenuList[Language.Localization["Help"]] = Language.Localization["Settings"];
            MenuList[Language.Localization["About"]] = Language.Localization["Help"];
            MenuList[Language.Localization["Show Help"]] = Language.Localization["Help"];
            MenuList["-"] = Language.Localization["Help"];
            MenuList[Language.Localization["Settings Editor"]] = Language.Localization["Settings"];

            MenuList[Language.Localization["Refresh Schema"]] = Language.Localization["Settings"];
            MenuList[Language.Localization["Check for Updates"]] = Language.Localization["Settings"];
            MenuList["-"] = Language.Localization["Settings"];

            Stack.Exit(StackReference);
        }

        public static void MainMenu()
        {
            int StackReference = Stack.Add("UI.MainMenu()");
            LDGraphicsWindow.ExitButtonMode(GraphicsWindow.Title, "Enabled");
            GraphicsWindow.CanResize = true;

            LDGraphicsWindow.State = 2;
            GraphicsWindow.Title = GlobalStatic.Title + " ";

            Primitive Sorts = $"1={Language.Localization["Table"]};2={Language.Localization["View"]};3={Language.Localization["Index"]};4={Language.Localization["Master Table"]};";
            if (Engines.CurrentDatabase != null && Engines.CurrentDatabase != null)
            {
                Engines.GetSchema(Engines.CurrentDatabase);
            }
            GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize + 8;
            int UIx = GlobalStatic.Listview_Width - 380;

            string Menu = LDControls.AddMenu(Desktop.Width * 1.5, 30, MenuList, IconList, null);

            Shapes.Move(Shapes.AddText(Language.Localization["Sort"] + ":"), UIx, 1);

            int TextWidth = LDText.GetHeight(Language.Localization["Sort"] + ":");
            GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;

            try
            {
                GlobalStatic.ComboBox["Table"] = LDControls.AddComboBox(Engines.Tables.ToPrimitiveArray(), 100, 100);
            }
            catch (Exception ex)
            {
                Events.LogMessage(ex.ToString(), "System");
            }

            GlobalStatic.ComboBox["Sorts"] = LDControls.AddComboBox(Sorts, 100, 100);
            GlobalStatic.ComboBox["Database"] = LDControls.AddComboBox(Engines.DB_ShortName.ToPrimitiveArray(), 100, 100);
            Controls.Move(GlobalStatic.ComboBox["Database"], UIx + TextWidth + 35, 5);
            Controls.Move(GlobalStatic.ComboBox["Sorts"], UIx + TextWidth + 150, 5);
            Controls.Move(GlobalStatic.ComboBox["Table"], UIx + TextWidth + 260, 5);

            //Virtual Call to Handler
            Events.MC(Language.Localization["View"]);

            Title();

            Controls.ButtonClicked += Events.BC;
            LDControls.MenuClicked += Events.MC;
            LDControls.ComboBoxItemChanged += Events.CB;
            LDControls.ContextMenuClicked += Events.MI;

            Stack.Exit(StackReference);
        }
    }
}
