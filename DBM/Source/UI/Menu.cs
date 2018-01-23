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
            MenuList[Utilities.Localization["File"]] = "Main";
            MenuList[Utilities.Localization["Edit"]] = "Main";
            MenuList[Utilities.Localization["View"] + " "] = "Main";
            MenuList[Utilities.Localization["Save"]] = "Main";
            MenuList[Utilities.Localization["Import"]] = "Main";
            MenuList[Utilities.Localization["Export"]] = "Main";
            MenuList[Utilities.Localization["Settings"]] = "Main";
            MenuList["Charts"] = "Main"; //TODO Localize

            //File
            MenuList[Utilities.Localization["New"]] = Utilities.Localization["File"];
            MenuList[Utilities.Localization["Open"]] = Utilities.Localization["File"];
            MenuList["Other"] = Utilities.Localization["File"]; // TODO Localize
            MenuList[Utilities.Localization["Define New Table"]] = "Other";
            MenuList[Utilities.Localization["New in Memory Db"]] = "Other";
            MenuList[Utilities.Localization["Create Statistics Page"]] = "Other";
            MenuList["-"] = Utilities.Localization["File"];

            //Import
            MenuList[Utilities.Localization["CSV"]] = Utilities.Localization["Import"];
            MenuList[Utilities.Localization["SQL"]] = Utilities.Localization["Import"];
            //MenuList["Converter"] = Utilities.Localization["Import"]; //Localize
            //MenuList["HTML to CSV"] = "Converter"; //Localize
            //MenuList["-"] = "Converter";
            MenuList["-"] = Utilities.Localization["Import"];

            //Export
            MenuList[Utilities.Localization["CSV"] + " "] = Utilities.Localization["Export"];
            MenuList[Utilities.Localization["HTML"] + " "] = Utilities.Localization["Export"];
            MenuList["JSON"] = Utilities.Localization["Export"]; //TODO Localize
            MenuList[Utilities.Localization["SQL"] + " "] = Utilities.Localization["Export"];
            MenuList[Utilities.Localization["PXML"] + " "] = Utilities.Localization["Export"];
            MenuList["MarkDown"] = Utilities.Localization["Export"]; //TODO Localize
            MenuList["Wiki MarkUp"] = Utilities.Localization["Export"]; //TODO Localize
            MenuList["-"] = Utilities.Localization["Export"];

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
            MenuList[Utilities.Localization["Help"]] = Utilities.Localization["Settings"];
            MenuList[Utilities.Localization["About"]] = Utilities.Localization["Help"];
            MenuList[Utilities.Localization["Show Help"]] = Utilities.Localization["Help"];
            MenuList["-"] = Utilities.Localization["Help"];
            MenuList[Utilities.Localization["Settings Editor"]] = Utilities.Localization["Settings"];

            MenuList[Utilities.Localization["Refresh Schema"]] = Utilities.Localization["Settings"];
            MenuList[Utilities.Localization["Check for Updates"]] = Utilities.Localization["Settings"];
            MenuList["-"] = Utilities.Localization["Settings"];

            Stack.Exit(StackReference);
            //IconList[Utilities.Localization["Settings Editor"]] = LDImage.LoadSVG( GlobalStatic.AssetPath + "\\Images\\settings.svg");
        }

        public static void MainMenu()
        {
            int StackReference = Stack.Add("UI.MainMenu()");
            LDGraphicsWindow.ExitButtonMode(GraphicsWindow.Title, "Enabled");
            GraphicsWindow.CanResize = true;

            LDGraphicsWindow.State = 2;
            GraphicsWindow.Title = GlobalStatic.Title + " ";

            Primitive Sorts = $"1={Utilities.Localization["Table"]};2={Utilities.Localization["View"]};3={Utilities.Localization["Index"]};4={Utilities.Localization["Master Table"]};";
            if (Engines.CurrentDatabase != null && Engines.CurrentDatabase != null)
            {
                Engines.GetSchema(Engines.CurrentDatabase);
            }
            GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize + 8;
            int UIx = GlobalStatic.Listview_Width - 380;

            string Menu = LDControls.AddMenu(Desktop.Width * 1.5, 30, MenuList, IconList, null);
            Shapes.Move(Shapes.AddText(Utilities.Localization["Sort"] + ":"), UIx, 1);

            int TextWidth = LDText.GetHeight(Utilities.Localization["Sort"] + ":");
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
            Events.MC(Utilities.Localization["View"]);

            Title();

            Controls.ButtonClicked += Events.BC;
            LDControls.MenuClicked += Events.MC;
            LDControls.ComboBoxItemChanged += Events.CB;
            LDControls.ContextMenuClicked += Events.MI;

            Stack.Exit(StackReference);
        }
    }
}
