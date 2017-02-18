using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Drawing;
using System.Text;

using Novacode;
using XkSystem.Areas.Course.Models.Schedule;

namespace XkSystem.Code
{
    public class DocxHelper
    {
        public static bool DataTableToWord(string filePath, DataTable dt, string headerText = "", string tableCaption = "")
        {
            using (var doc = DocX.Create(filePath, DocumentTypes.Document))
            {
                int rows = dt.Rows.Count;
                int columns = dt.Columns.Count;

                if (!String.IsNullOrWhiteSpace(headerText))
                {
                    Formatting fmt = new Formatting();
                    fmt.Bold = true;
                    fmt.Size = 18;
                    doc.InsertParagraph(headerText, false, fmt).Alignment = Alignment.center;
                    doc.InsertParagraph("");
                }

                if (!String.IsNullOrWhiteSpace(tableCaption))
                {
                    rows = rows + 1;
                }

                Table tbl = doc.InsertTable(rows, columns);

                int startRowIndex = 0;
                if (!String.IsNullOrWhiteSpace(tableCaption))
                {
                    startRowIndex = 1;

                    tbl.Rows[0].Cells[0].Paragraphs[0].Append(tableCaption);
                    tbl.Rows[0].Cells[0].Width = 1;
                    tbl.Rows[0].MergeCells(0, columns);
                    tbl.Rows[0].MinHeight = 30;

                    foreach (var p in tbl.Rows[0].Cells[0].Paragraphs)
                    {
                        tbl.Rows[0].Cells[0].RemoveParagraph(p);
                    }

                    tbl.Rows[0].Cells[0].InsertParagraph(tableCaption);
                    tbl.Rows[0].Cells[0].VerticalAlignment = VerticalAlignment.Center;
                    tbl.Rows[0].Cells[0].Paragraphs[0].Alignment = Alignment.center;
                }

                for (int i = startRowIndex; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (!String.IsNullOrWhiteSpace(dt.Rows[i - startRowIndex][j].ToString()))
                        {
                            var arrText = dt.Rows[i - startRowIndex][j].ToString().Split(new char[] { '|' });
                            tbl.Rows[i].Cells[j].Paragraphs[0].Append(arrText[0])
                                .Alignment = Alignment.center;
                            if (arrText.Length > 1)
                            {
                                for (int k = 1; k < arrText.Length; k++)
                                {
                                    tbl.Rows[i].Cells[j].InsertParagraph(arrText[k])
                                        .Alignment = Alignment.center;
                                }
                            }
                        }
                        else
                        {
                            tbl.Rows[i].Cells[j].Paragraphs[0].Append(dt.Rows[i - startRowIndex][j].ToString())
                                .Alignment = Alignment.center;
                        }
                        tbl.Rows[i].Cells[j].Width = 1;
                        tbl.Rows[i].Cells[j].VerticalAlignment = VerticalAlignment.Center;
                    }
                    tbl.Rows[i].MinHeight = 30;
                }

                tbl.Design = TableDesign.TableNormal;
                tbl.AutoFit = AutoFit.Window;

                Border border = new Border(BorderStyle.Tcbs_single, BorderSize.two, 0, Color.Black);
                tbl.SetBorder(TableBorderType.Bottom, border);
                tbl.SetBorder(TableBorderType.Left, border);
                tbl.SetBorder(TableBorderType.Right, border);
                tbl.SetBorder(TableBorderType.Top, border);
                tbl.SetBorder(TableBorderType.InsideV, border);
                tbl.SetBorder(TableBorderType.InsideH, border);

                doc.Save();
            }

            return true;
        }

        public static bool ScheduleToWord(string filePath, List<ScheduleExportWord> scheduleList)
        {
            using (var doc = DocX.Create(filePath, DocumentTypes.Document))
            {
                for (int index = 0; index < scheduleList.Count; index++)
                {
                    var schedule = scheduleList[index];

                    int rows = schedule.ScheduleList.Rows.Count;
                    int columns = schedule.ScheduleList.Columns.Count;

                    if (!String.IsNullOrWhiteSpace(schedule.HeaderText))
                    {
                        Formatting fmt = new Formatting();
                        fmt.Bold = true;
                        fmt.Size = 18;
                        doc.InsertParagraph(schedule.HeaderText, false, fmt).Alignment = Alignment.center;
                        doc.InsertParagraph("");
                    }

                    if (!String.IsNullOrWhiteSpace(schedule.TableCaption))
                    {
                        rows = rows + 1;
                    }

                    Table tbl = doc.InsertTable(rows, columns);

                    int startRowIndex = 0;
                    if (!String.IsNullOrWhiteSpace(schedule.TableCaption))
                    {
                        startRowIndex = 1;

                        tbl.Rows[0].Cells[0].Paragraphs[0].Append(schedule.TableCaption);
                        tbl.Rows[0].Cells[0].Width = 1;
                        tbl.Rows[0].MergeCells(0, columns);
                        tbl.Rows[0].MinHeight = 30;

                        foreach (var p in tbl.Rows[0].Cells[0].Paragraphs)
                        {
                            tbl.Rows[0].Cells[0].RemoveParagraph(p);
                        }

                        tbl.Rows[0].Cells[0].InsertParagraph(schedule.TableCaption);
                        tbl.Rows[0].Cells[0].VerticalAlignment = VerticalAlignment.Center;
                        tbl.Rows[0].Cells[0].Paragraphs[0].Alignment = Alignment.center;
                    }

                    for (int i = startRowIndex; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            string[] arrData = schedule.ScheduleList.Rows[i - startRowIndex][j].ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            if (arrData.Length <= 1)
                            {
                                tbl.Rows[i].Cells[j].Paragraphs[0].Append(schedule.ScheduleList.Rows[i - startRowIndex][j].ToString())
                                    .Alignment = Alignment.center;
                            }
                            else
                            {
                                for (int k = 0; k < arrData.Length; k++)
                                {
                                    if (k == 0)
                                    {
                                        tbl.Rows[i].Cells[j].Paragraphs[0].Append(arrData[k]).Alignment = Alignment.center;
                                    }
                                    else
                                    {
                                        tbl.Rows[i].Cells[j].InsertParagraph(arrData[k]).Alignment = Alignment.center;
                                    }
                                }
                            }
                            tbl.Rows[i].Cells[j].Width = 1;
                            tbl.Rows[i].Cells[j].VerticalAlignment = VerticalAlignment.Center;
                        }
                        tbl.Rows[i].MinHeight = 30;
                    }

                    tbl.Design = TableDesign.TableNormal;
                    tbl.AutoFit = AutoFit.Window;

                    Border border = new Border(BorderStyle.Tcbs_single, BorderSize.two, 0, Color.Black);
                    tbl.SetBorder(TableBorderType.Bottom, border);
                    tbl.SetBorder(TableBorderType.Left, border);
                    tbl.SetBorder(TableBorderType.Right, border);
                    tbl.SetBorder(TableBorderType.Top, border);
                    tbl.SetBorder(TableBorderType.InsideV, border);
                    tbl.SetBorder(TableBorderType.InsideH, border);

                    if (scheduleList.Count > 1 && index < scheduleList.Count - 1)
                    {
                        tbl.InsertPageBreakAfterSelf();
                    }
                }

                doc.Save();
            }
            return true;
        }

        public static List<ClassSetImportWordSchedule> WordToImportSchedule(string filePath)
        {
            using (var doc = DocX.Load(filePath))
            {
                List<ClassSetImportWordSchedule> scheduleList = new List<Areas.Course.Models.Schedule.ClassSetImportWordSchedule>();

                foreach (var tbl in doc.Tables)
                {
                    var schedule = new ClassSetImportWordSchedule();

                    var classRow = tbl.Rows[0];
                    schedule.ClassName = GetCellText(classRow.Cells[0]);

                    var columnRow = tbl.Rows[1];
                    DataTable dt = new DataTable();
                    foreach (var cell in columnRow.Cells)
                    {
                        dt.Columns.Add(new DataColumn(GetCellText(cell)));
                    }

                    for (var i = 2; i < tbl.Rows.Count(); i++)
                    {
                        var row = tbl.Rows[i];
                        if (String.IsNullOrWhiteSpace(GetCellText(row.Cells[0])))
                        {
                            continue;
                        }

                        var dr = dt.NewRow();
                        for (var j = 0; j < columnRow.Cells.Count(); j++)
                        {
                            dr[j] = GetCellText(row.Cells[j]);
                        }
                        dt.Rows.Add(dr);
                    }

                    schedule.ClassScheduleList = dt;
                    scheduleList.Add(schedule);
                }

                return scheduleList;
            }
        }

        private static string GetCellText(Cell cell)
        {
            StringBuilder text = new StringBuilder();
            if (cell.Paragraphs.Count == 1)
            {
                text.Append(cell.Paragraphs[0].Text);
            }
            else if (cell.Paragraphs.Count > 1)
            {
                foreach (var p in cell.Paragraphs)
                {
                    if (!String.IsNullOrWhiteSpace(p.Text.Trim()))
                    {
                        text.AppendFormat("{0}|", p.Text.Trim());
                    }
                }

                if (text.Length > 1)
                {
                    text.Remove(text.Length - 1, 1);
                }
            }

            return text.ToString();
        }
    }
}