using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shiftbook.Models;
using Shiftbook.BL;
using Shiftbook.Helping_Classes;
using OfficeOpenXml;

namespace Shiftbook.Controllers
{
    public class ExcelController : Controller
    {
        private DatabaseEntities de = new DatabaseEntities();
        private GeneralPurpose gp = new GeneralPurpose();


        [HttpPost]
        public ActionResult PostImportMaintenanceTask(FormCollection formCollection, string way = "")
        {
            DatabaseEntities dc = new DatabaseEntities();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["PostedFile"];
                if ((file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(file.FileName)))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] filebytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(filebytes, 0, Convert.ToInt32(file.ContentLength));

                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfColumns = workSheet.Dimension.End.Column;
                        var noOfRows = workSheet.Dimension.End.Row;

                        for (int rowIterator = 2; rowIterator <= noOfRows; rowIterator++)
                        {

                            List<MaintenanceTask> listTask = new MaintenanceTaskBL().GetActiveMaintenanceTasksList(de).ToList();
                            List<EquipmentCode> codes = new EquipmentCodeBL().GetActiveEquipmentCodesList(de).ToList();
                            if (workSheet.Cells[rowIterator, 1] == null)//can this be null?
                            {
                                break; //if it's null exit from the for loop
                            }

                            var EquipCode = workSheet.Cells[rowIterator, 1].Value.ToString();
                            EquipCode = EquipCode.Trim(' ');
                            var valueChecking = workSheet.Cells[rowIterator, 6].Value.ToString();
                            valueChecking = valueChecking.Trim(' ');
                            int CodeId = -1;
                            if (workSheet.Cells[rowIterator, 2] != null)
                            {
                                EquipmentCode e = codes.Where(x => x.Code.Trim() == EquipCode).FirstOrDefault();

                                if (e != null)
                                {
                                    CodeId = e.Id;
                                }
                                else
                                {
                                    var equip = new EquipmentCode();
                                    equip.Code = EquipCode;
                                    equip.IsActive = 1;
                                    equip.CreatedAt = GeneralPurpose.DateTimeNow();
                                    new EquipmentCodeBL().AddEquipmentCode2(equip, de);
                                    CodeId = equip.Id;
                                }

                                MaintenanceTask v = listTask.Where(x => x.TaskDescription.ToLower().Trim() == valueChecking.ToLower()).FirstOrDefault();

                                if (v == null)
                                {
                                    var claim = new MaintenanceTask();

                                    claim.EquipmentCodeId = CodeId;
                                    claim.AssetDescription = workSheet.Cells[rowIterator, 2].Value == null ? string.Empty : workSheet.Cells[rowIterator, 2].Value.ToString();
                                    claim.Pid = workSheet.Cells[rowIterator, 3].Value == null ? string.Empty : workSheet.Cells[rowIterator, 3].Value.ToString();
                                    claim.SystemDescription = workSheet.Cells[rowIterator, 4].Value == null ? string.Empty : workSheet.Cells[rowIterator, 4].Value.ToString();
                                    claim.MaintenanceType = workSheet.Cells[rowIterator, 5].Value == null ? string.Empty : workSheet.Cells[rowIterator, 5].Value.ToString();
                                    claim.TaskDescription = workSheet.Cells[rowIterator, 6].Value == null ? string.Empty : workSheet.Cells[rowIterator, 6].Value.ToString();
                                    claim.Comment = workSheet.Cells[rowIterator, 7].Value == null ? string.Empty : workSheet.Cells[rowIterator, 7].Value.ToString();
                                    claim.Interval = workSheet.Cells[rowIterator, 8].Value == null ? string.Empty : workSheet.Cells[rowIterator, 8].Value.ToString();
                                    claim.Unit = workSheet.Cells[rowIterator, 9].Value == null ? string.Empty : workSheet.Cells[rowIterator, 9].Value.ToString();
                                    claim.PlantShutDownJob = workSheet.Cells[rowIterator, 10].Value == null ? string.Empty : workSheet.Cells[rowIterator, 10].Value.ToString();
                                    claim.AtexZone = workSheet.Cells[rowIterator, 11].Value == null ? string.Empty : workSheet.Cells[rowIterator, 11].Value.ToString();
                                    claim.AuxiliaryMaterial = workSheet.Cells[rowIterator, 12].Value == null ? string.Empty : workSheet.Cells[rowIterator, 12].Value.ToString();
                                    claim.GreaseOilManufacturer = workSheet.Cells[rowIterator, 13].Value == null ? string.Empty : workSheet.Cells[rowIterator, 13].Value.ToString();
                                    claim.GreaseOilType = workSheet.Cells[rowIterator, 14].Value == null ? string.Empty : workSheet.Cells[rowIterator, 14].Value.ToString();
                                    claim.TopupAmount = workSheet.Cells[rowIterator, 15].Value == null ? string.Empty : workSheet.Cells[rowIterator, 15].Value.ToString();
                                    claim.RoutineType = null;
                                    claim.IsActive = 1;
                                    claim.CreatedAt = DateTime.Now;

                                    if (!new MaintenanceTaskBL().AddMaintenanceTask(claim, de))
                                    {
                                        return RedirectToAction("ImportMaintenanceTask", "Admin", new { msg = "Something is Wrong", color = "red" });
                                    }
                                }
                            }
                        }
                    }
                }
                return RedirectToAction("ImportMaintenanceTask", "Admin", new { msg = "File Uploaded Successfully", color = "green" });
            }
            return RedirectToAction("ImportMaintenanceTask", "Admin", new { msg = "Something is Wrong", color = "red" });
        }

        public static string GetCellValue(SpreadsheetDocument document, DocumentFormat.OpenXml.Spreadsheet.Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = "";
            if(cell.CellValue != null)
            {
                value = cell.CellValue.InnerXml;
            }

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                string a = stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

    }
}