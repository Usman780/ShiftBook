using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shiftbook.Models;
using Shiftbook.BL;
using Shiftbook.Helping_Classes;

namespace Shiftbook.Controllers
{
    public class WordController : Controller
    {
        private GeneralPurpose gp = new GeneralPurpose();
        private DatabaseEntities de = new DatabaseEntities();
        public ActionResult ShiftBookTemplate(int id)
        {
            try
            {
                WorkOrder wo = new WorkOrderBL().GetActiveWorkOrderById(id, de);
                //Create an instance for word app  
                Microsoft.Office.Interop.Word.Application winword = new Microsoft.Office.Interop.Word.Application();

                //Set animation status for word application  
                winword.ShowAnimation = false;

                //Set status for word application is to be visible or not.  
                winword.Visible = false;

                //Create a missing variable for missing value  
                object missing = System.Reflection.Missing.Value;

                //Create a new document  
                Microsoft.Office.Interop.Word.Document document = winword.Documents.Add(ref missing, ref missing, ref missing, ref missing);

                //This loop will be used to create Header of Doc file
                foreach (Microsoft.Office.Interop.Word.Section section in document.Sections)
                {
                    //Get the header range and add the header details.  
                    Microsoft.Office.Interop.Word.Range headerRange = section.Headers[Microsoft.Office.Interop.Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.Fields.Add(headerRange, Microsoft.Office.Interop.Word.WdFieldType.wdFieldPage);
                    headerRange.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    headerRange.Font.ColorIndex = Microsoft.Office.Interop.Word.WdColorIndex.wdBlack;
                    headerRange.Font.Size = 10;
                    headerRange.Text = "Shiftbook Work Order";
                }

                #region Para + Heading

                Microsoft.Office.Interop.Word.Paragraph para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "Work Order # " + wo.Id, 14, "underline", "bold", "center");
                para.Range.InsertParagraphAfter();

                para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "Date/Time: ", 12, "noUnderline", "bold", "left");
                para.Range.InsertParagraphAfter();

                para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "      " + wo.OrderDateTime, 10, "noUnderline", "noBold", "left");
                para.Range.InsertParagraphAfter();

                para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "Type: ", 12, "noUnderline", "bold", "left");
                para.Range.InsertParagraphAfter();

                para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "      " + wo.OrderType, 10, "noUnderline", "noBold", "left");
                para.Range.InsertParagraphAfter();

                para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "Entry: ", 12, "underline", "bold", "center");
                para.Range.InsertParagraphAfter();

                para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "Location: ", 12, "noUnderline", "bold", "left");
                para.Range.InsertParagraphAfter();

                para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "      " + wo.Location, 10, "noUnderline", "noBold", "left");
                para.Range.InsertParagraphAfter();

                para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "Work Order: ", 12, "noUnderline", "bold", "left");
                para.Range.InsertParagraphAfter();

                para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "      " + wo.MaintenanceTask.TaskDescription, 10, "noUnderline", "noBold", "left");
                para.Range.InsertParagraphAfter();

                para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "Author: ", 12, "noUnderline", "bold", "left");
                para.Range.InsertParagraphAfter();

                para = Styles(document.Content.Paragraphs.Add(ref missing),
                    "      " + wo.User.FName + " " + wo.User.LName, 10, "noUnderline", "noBold", "left");
                para.Range.InsertParagraphAfter();


                Microsoft.Office.Interop.Word.InlineShape line = document.Paragraphs.Last.Range.InlineShapes.AddHorizontalLineStandard(ref missing);
                line.Height = 2;
                line.Fill.Solid();
                line.HorizontalLineFormat.NoShade = true;
                line.Fill.ForeColor.RGB = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                line.HorizontalLineFormat.PercentWidth = 90;
                line.Height = 1;
                line.HorizontalLineFormat.Alignment = WdHorizontalLineAlignment.wdHorizontalLineAlignCenter;

                #endregion

                //para = Styles(document.Content.Paragraphs.Add(ref missing),
                //    "Followups: ", 12, "underline", "bold", "center");
                //para.Range.InsertParagraphAfter();

                ////Create a 5X5 table and insert some dummy record  
                //Table firstTable = document.Tables.Add(para.Range, 6, 5, ref missing, ref missing);

                //firstTable.Borders.Enable = 1;
                //foreach (Row row in firstTable.Rows)
                //{
                //    foreach (Cell cell in row.Cells)
                //    {
                //        //Header row  
                //        if (cell.RowIndex == 1)
                //        {
                //            cell.Range.Font.Bold = 1;
                //            cell.Range.Font.Size = 12;
                //            cell.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                //            cell.Shading.BackgroundPatternColor = WdColor.wdColorGray25; //setting background color of cell
                //            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                //            cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;

                //            switch (cell.ColumnIndex)
                //            {
                //                case 1:
                //                    cell.Range.Text = "Nr.";
                //                    break;
                //                case 2:
                //                    cell.Range.Text = "Date/Time";
                //                    break;
                //                case 3:
                //                    cell.Range.Text = "Type";
                //                    break;
                //                case 4:
                //                    cell.Range.Text = "Entry";
                //                    break;
                //                case 5:
                //                    cell.Range.Text = "Author";
                //                    break;
                //            }
                //        }
                //        else //Data row  
                //        {
                //            cell.Range.Font.Bold = 0;
                //            cell.Range.Font.Size = 10;
                //            cell.Range.Font.Underline = WdUnderline.wdUnderlineNone;
                //            cell.VerticalAlignment = WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                //            cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                //            switch (cell.ColumnIndex)
                //            {
                //                case 1:
                //                    cell.Range.Text = (cell.RowIndex - 1).ToString();
                //                    break;
                //                case 2:
                //                    cell.Range.Text = DateTime.Now.AddDays(cell.RowIndex - 1).ToString("dd/MM/yyyy hh:mm tt");
                //                    break;
                //                case 3:
                //                    cell.Range.Text = "Electrical";
                //                    break;
                //                case 4:

                //                    string cellText = "Location:" + Environment.NewLine + "      Location" + (cell.RowIndex - 1) + Environment.NewLine +
                //                        "Work Order:" + Environment.NewLine + "      Order" + (cell.RowIndex - 1) + Environment.NewLine;
                //                    cell.Range.Text = cellText;

                //                    //setting the heading bold
                //                    object objStart = cell.Range.Start;
                //                    object objEnd = cell.Range.Start + cellText.IndexOf(":");

                //                    Microsoft.Office.Interop.Word.Range rngBold = document.Range(ref objStart, ref objEnd);
                //                    rngBold.Bold = 1;

                //                    //setting this cell to left aligned
                //                    cell.Range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                //                    break;
                //                case 5:
                //                    cell.Range.Text = "User " + (cell.RowIndex - 1);
                //                    break;
                //            }
                //        }
                //    }
                //}


                string filename = "work_order.docx";
                filename = Path.Combine(Server.MapPath("../Content/ErrorLogs/"), filename);


                document.SaveAs(filename);
                document.Close(ref missing, ref missing, ref missing);
                document = null;
                winword.Quit(ref missing, ref missing, ref missing);
                winword = null;

                return File(filename, "application/force-download", Path.GetFileName(filename));

                //return RedirectToAction("Index", "Home", new { msg = "1" });
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Auth", new { msg = ex.Message });
            }
        }

        public static Microsoft.Office.Interop.Word.Paragraph Styles(Microsoft.Office.Interop.Word.Paragraph para, string text, int textSize, string isUnderline, string isBold, string textAlignment)
        {
            para.Range.Font.Name = "Times New Roman";
            para.Range.Font.ColorIndex = Microsoft.Office.Interop.Word.WdColorIndex.wdBlack;
            para.Range.Text = text;
            if (isUnderline == "underline")
            {
                para.Range.Font.Underline = WdUnderline.wdUnderlineSingle;
            }
            else
            {
                para.Range.Font.Underline = WdUnderline.wdUnderlineNone;
            }

            if (textAlignment == "left")
            {
                para.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
            }
            else if (textAlignment == "center")
            {
                para.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
            }
            else
            {
                para.Alignment = WdParagraphAlignment.wdAlignParagraphRight;
            }

            if (isBold == "bold")
            {
                para.Range.Font.Bold = 1;
            }
            else
            {
                para.Range.Font.Bold = 0;
            }

            para.Range.Font.Size = textSize;

            return para;
        }
    }
}