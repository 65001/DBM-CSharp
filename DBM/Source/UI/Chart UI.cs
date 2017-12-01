﻿using LitDev;
using Microsoft.SmallBasic.Library;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DBM
{
    public static partial class UI
    {
        public static class Charts
        {
            static string XListBox;
            static string YListBox;
            static string SchemaListBox;

            static string Left1;
            static string Left2;
            static string Right1;
            static string Right2;
            static string GenerateChartButton;
            static string Escape;

            static string TitleTB;
            static string SubTitleTB;
            static string XaxisCaptionTB;
            static string YaxisCaptionTB;

            static string DataView;

            static List<string> XColumns = new List<string>();
            static List<string> YColumns = new List<string>();
            static List<string> Schema = new List<string>();
            static Google_Charts.Chart chart;

            public static void Display(Google_Charts.Chart chart)
            {
                Utilities.AddtoStackTrace("UI.Charts.Display()");
                Charts.chart = chart;
                ClearWindow();
                GraphicsWindow.Title = "Charts";

                LDGraphicsWindow.CancelClose = true;
                LDGraphicsWindow.ExitOnClose = false;
                LDGraphicsWindow.Closing += Events.Closing;
                LDGraphicsWindow.ExitButtonMode(GraphicsWindow.Title, "Disabled");

                XColumns.Clear();
                YColumns.Clear();
                Schema = Export.GenerateSchemaListFromLastQuery();

                //UI Stuff
                GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize + 12;

                GraphicsWindow.DrawText(5, 10,string.Format("Min Columns {0}",chart.MinColumns + 1));
                GraphicsWindow.DrawText(5, 30,string.Format("Max Columns {0}", chart.MaxColumns));
                GraphicsWindow.DrawText(5, 70, "X:");
                GraphicsWindow.DrawText(300 + 100, 70, "Columns:");
                GraphicsWindow.DrawText(600 + 200, 70,"Y:");

                GraphicsWindow.DrawText(5, 450, "Title");
                GraphicsWindow.DrawText(5, 500, "SubTitle");
                GraphicsWindow.DrawText(5, 550, "X axis");
                GraphicsWindow.DrawText(5, 600, "Y axis");

                GraphicsWindow.FontSize = GlobalStatic.DefaultFontSize;

                XListBox = LDControls.AddListBox("", 300, 300);
                SchemaListBox = LDControls.AddListBox(Schema.ToPrimitiveArray(), 300, 300);
                YListBox = LDControls.AddListBox("", 300, 300);

                DataView = LDControls.AddDataView(300, 300, "1=Column;2=Type;");
                LDControls.DataViewSetColumnComboBox(DataView, 2, "1=Text;2=Number;");

                if (Desktop.Width < 1500)
                {
                    LDScrollBars.Add(1920, 0);
                }
            
                Controls.Move(XListBox, 5, 100);
                Controls.Move(SchemaListBox, 300 + 100, 100);
                Controls.Move(YListBox, 600 +200, 100);
                Controls.Move(DataView, 1200, 100);
                
                Left1 = Controls.AddButton("<", 325, 200);
                Right1 = Controls.AddButton(">", 325, 250);

                Left2 = Controls.AddButton("<", 725, 200);
                Right2 = Controls.AddButton(">", 725, 250);

                GenerateChartButton = Controls.AddButton("Generate Chart", 300, 500);
                Controls.Move(GenerateChartButton, 800, 550);
                Controls.SetSize(GenerateChartButton, 100, 30);

                //TODO Add TextBoxes for Title,SubTitle,X axis caption, and y axis caption
                TitleTB = Controls.AddTextBox(130, 450);
                SubTitleTB = Controls.AddTextBox(130, 500);
                XaxisCaptionTB = Controls.AddTextBox(130, 550);
                YaxisCaptionTB = Controls.AddTextBox(130, 600);

                Controls.SetSize(TitleTB, 200, 30);
                Controls.SetSize(SubTitleTB, 200, 30);
                Controls.SetSize(XaxisCaptionTB, 200, 30);
                Controls.SetSize(YaxisCaptionTB, 200, 30);

                Escape = Controls.AddButton("Esc", 50, 500);
                Controls.Move(Escape, 800, 500);
                Controls.SetSize(Escape, 100, 30);

                //TODO Add Region options for GeoCharts
                //TODO Allow users to specify data type (Number,string,eventually Datetime etc).
                //Otherwise attempt to grab that data from the db?
                Controls.SetSize(Left1, 50, 30);
                Controls.SetSize(Right1, 50, 30);
                Controls.SetSize(Left2, 50, 30);
                Controls.SetSize(Right2, 50, 30);

                //Event Handler Unhooking
                Controls.ButtonClicked -= Events.BC;
                //Event Handler Hooking
                Controls.ButtonClicked += Handler;
                AutoHide();
            }

            static void Binder()
            {
                LDControls.ListBoxContent(XListBox, XColumns.ToPrimitiveArray());
                LDControls.ListBoxContent(SchemaListBox, Schema.ToPrimitiveArray());
                LDControls.ListBoxContent(YListBox, YColumns.ToPrimitiveArray());
                List<string> Columns = new List<string>();
                Columns.AddRange(XColumns);
                Columns.AddRange(YColumns);

                for (int i = 1; i <= LDControls.DataViewRowCount(DataView); i++)
                {
                    string CurrentColumn = LDControls.DataViewGetValue(DataView, i, 1);
                    if (Columns.Contains(CurrentColumn) == false)
                    {
                        LDControls.DataViewDeleteRow(DataView, i);
                    }
                    else
                    {
                        Columns.Remove(CurrentColumn);
                    }
                }

                for (int i = 0; i < Columns.Count; i++)
                {
                    LDControls.DataViewSetRow(DataView, LDControls.DataViewRowCount(DataView) + 1, string.Format("1={0};2=Text;",Columns[i]));
                }
            }

            public static void Clear()
            {
                //Remove Chart Handlers
                Controls.ButtonClicked -= Handler;
                GlobalStatic.ListView = null;
                GlobalStatic.Dataview = null;
                ClearWindow();
                PreMainMenu();
                HideDisplayResults();
                MainMenu();
                //Add DBM Handlers
                Controls.ButtonClicked += Events.BC;
            }

            static void Handler()
            {
                string LastListBox = LDControls.LastListBox;
                string LCB = Controls.LastClickedButton;
                int Node = LDControls.ListBoxGetSelected(LastListBox) - 1;
                List<string> CurrentList = null;
                string Item = null;

                if (LastListBox == SchemaListBox)
                {
                    CurrentList = Schema;
                }
                else if (LastListBox == XListBox)
                {
                    CurrentList = XColumns;
                }
                else if (LastListBox == YListBox)
                {
                    CurrentList = YColumns;
                }

                if (Node >= 0)
                { 
                    Item = CurrentList[Node];
                }

                if (LCB == GenerateChartButton)
                {
                    List<string> Columns = new List<string>();
                    Columns.AddRange(XColumns);
                    Columns.AddRange(YColumns);
                    Dictionary<string, string> Types = new Dictionary<string, string>();
                    for (int i = 1; i <= LDControls.DataViewRowCount(DataView); i++)
                    {
                        Types.Add(LDControls.DataViewGetValue(DataView, i, 1), LDControls.DataViewGetValue(DataView, i, 2));
                    }
                    GenerateChart(Columns, Types , Engines.Query(Engines.CurrentDatabase, Engines.NonSchemaQuery.Last(), null, true, GlobalStatic.UserName, "Generating Chart"));
                    Clear();
                    GraphicsWindow.ShowMessage("Exported Chart!", "Success");
                }
                else if (LCB == Escape)
                {
                    Clear();
                }
                else if (CurrentList?.Count >= Node)
                {
                    if (LCB == Left1)
                    {
                        Schema.RemoveAt(Node);
                        XColumns.Add(Item);
                    }
                    else if (LCB == Right1)
                    {
                        XColumns.RemoveAt(Node);
                        Schema.Add(Item);
                    }
                    else if (LCB == Left2)
                    {
                        YColumns.RemoveAt(Node);
                        Schema.Add(Item);
                    }
                    else if (LCB == Right2)
                    {
                        Schema.RemoveAt(Node);
                        YColumns.Add(Item);
                    }
                }
                Binder();
                AutoHide();
            }

            static void AutoHide()
            {
                Controls.ShowControl(Left1);
                Controls.ShowControl(Left2);
                Controls.ShowControl(Right1);
                Controls.ShowControl(Right2);
                int Count = XColumns.Count + YColumns.Count;
                //Left 1 //Only one X axis
                if (XColumns.Any() && XColumns.Count == 1)
                {
                    Controls.HideControl(Left1);
                }

                if (XColumns.Count == 0)
                {
                    Controls.HideControl(Right1);
                }
                if (YColumns.Count == 0)
                {
                    Controls.HideControl(Left2);
                }
                if (Schema.Count == 0)
                {
                    Controls.HideControl(Left1);
                    Controls.HideControl(Right2);
                }

                if ( Count > (chart.MaxColumns))
                {
                    Controls.HideControl(Right2);
                }
            }

            static void GenerateChart(List<string> Columns,Dictionary<string,string> Types, Primitive QueryData)
            {
                //Process Columns
                for (int i = 0; i < Columns.Count; i++)
                {
                    try
                    {
                        switch (Types[Columns[i]])
                        {
                            case "Number":
                                chart.AddColumn(Columns[i], Google_Charts.Chart.DataType.number);
                                break;
                            case "Text":
                            default:
                                chart.AddColumn(Columns[i]);
                                break;
                        }
                    }
                    catch (KeyNotFoundException){}
                }
                
                Primitive Indices = QueryData[1].GetAllIndices();
                //Tranforms Data
                for (int i = 1; i <= QueryData.GetItemCount(); i++)
                {
                    for (int ii = 1; ii <= QueryData[i].GetItemCount(); ii++)
                    {
                        if (Columns.Contains(Indices[ii]))
                        {
                            chart.AddRowData(i - 1, QueryData[i][Indices[ii]]);
                        }
                    }
                }

                chart.Title = Controls.GetTextBoxText(TitleTB);
                chart.SubTitle = Controls.GetTextBoxText(SubTitleTB);
                chart.Xaxis = Controls.GetTextBoxText(XaxisCaptionTB);
                chart.Yaxis = Controls.GetTextBoxText(YaxisCaptionTB);
                
                string OutPut = Path.GetDirectoryName(Engines.DB_Path[Engines.GetDataBaseIndex(Engines.CurrentDatabase)]) + string.Format("\\{0} {1} Chart.html", Engines.CurrentTable.SanitizeFieldName(), chart.ChartType);
                chart.Write(OutPut);
                LDProcess.Start(OutPut, null);
            }
        }
    }
}