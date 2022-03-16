using Shiftbook.BL;
using Shiftbook.Helping_Classes;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.DAL
{
    public class EquipmentCodeDAL
    {
        GeneralPurpose gp = new GeneralPurpose();

        public List<EquipmentCode> GetAllEquipmentCodesList(DatabaseEntities de)
        {
            return de.EquipmentCodes.ToList();
        }

        public List<EquipmentCode> GetActiveEquipmentCodesList(DatabaseEntities de)
        {
            return de.EquipmentCodes.Where(x => x.IsActive == 1).ToList();
        }

        public EquipmentCode GetEquipmentCodeById(int id, DatabaseEntities de)
        {
            return de.EquipmentCodes.Where(x => x.Id == id).FirstOrDefault();
        }

        public EquipmentCode GetActiveEquipmentCodeById(int id, DatabaseEntities de)
        {
            return de.EquipmentCodes.Where(x => x.Id == id).FirstOrDefault(x => x.IsActive == 1);
        }

        public List<EquipmentCode> GetEquipmentCodeListByCode(string code, DatabaseEntities de)
        {
            return de.EquipmentCodes.Where(x => x.Code.ToLower().Contains(code.ToLower())).ToList();
        }

        public bool AddEquipmentCode(EquipmentCode EquipmentCode, DatabaseEntities de)
        {
            try
            {
                de.EquipmentCodes.Add(EquipmentCode);
                de.SaveChanges();
                EquipmentCodeHistory history = new EquipmentCodeHistory();
                history.EquipmentCodeId = EquipmentCode.Id;
                history.Status = "Equipment Code is Added";
                history.IsActive = 1;
                history.UpdatedBy = gp.ValidateLoggedinUser().Id;
                history.CreatedAt = GeneralPurpose.DateTimeNow();
                new EquipmentCodeHistoryBL().AddEquipmentCodeHistory(history, de);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int AddEquipmentCode2(EquipmentCode EquipmentCode, DatabaseEntities de)
        {
            try
            {
                de.EquipmentCodes.Add(EquipmentCode);
                de.SaveChanges();

                EquipmentCodeHistory history = new EquipmentCodeHistory();
                history.EquipmentCodeId = EquipmentCode.Id;
                history.Status = "Equipment Code is Added";
                history.IsActive = 1;
                history.UpdatedBy = gp.ValidateLoggedinUser().Id;
                history.CreatedAt = GeneralPurpose.DateTimeNow();
                new EquipmentCodeHistoryBL().AddEquipmentCodeHistory(history, de);

                return EquipmentCode.Id;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public bool UpdateEquipmentCode(EquipmentCode EquipmentCode, DatabaseEntities de)
        {
            try
            {
                de.Entry(EquipmentCode).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();

                EquipmentCodeHistory history = new EquipmentCodeHistory();
                string Status = "";
                if (EquipmentCode.IsActive == 0)
                {
                    List<MaintenanceTask> TaskList = new MaintenanceTaskDAL().GetActiveMaintenanceTasksList(de).Where(x => x.EquipmentCodeId == EquipmentCode.Id).ToList();
                    foreach(MaintenanceTask mt in TaskList)
                    {
                        mt.IsActive = 0;
                        new MaintenanceTaskDAL().UpdateMaintenanceTask(mt, de);
                    }
                    Status = "Equipment Code is Deleted";
                }
                else
                {
                    Status = "Equipment Code is Updated";
                }
                
                history.EquipmentCodeId = EquipmentCode.Id;
                history.Status = Status;
                history.IsActive = 1;
                history.UpdatedBy = gp.ValidateLoggedinUser().Id;
                history.CreatedAt = GeneralPurpose.DateTimeNow();
                new EquipmentCodeHistoryBL().AddEquipmentCodeHistory(history, de);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteEquipmentCode(int id, DatabaseEntities de)
        {
            try
            {
                de.EquipmentCodes.Remove(de.EquipmentCodes.Where(x => x.Id == id).FirstOrDefault());
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}