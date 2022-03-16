using Shiftbook.BL;
using Shiftbook.Helping_Classes;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.DAL
{
    public class MaintenanceTaskDAL
    {
        GeneralPurpose gp = new GeneralPurpose();
        public List<MaintenanceTask> GetAllMaintenanceTasksList(DatabaseEntities de)
        {
            return de.MaintenanceTasks.ToList();
        }

        public List<MaintenanceTask> GetActiveMaintenanceTasksList(DatabaseEntities de)
        {
            return de.MaintenanceTasks.Where(x => x.IsActive == 1).ToList();
        }

        public MaintenanceTask GetMaintenanceTaskById(int id, DatabaseEntities de)
        {
            return de.MaintenanceTasks.Where(x => x.Id == id).FirstOrDefault();
        }

        public MaintenanceTask GetActiveMaintenanceTaskById(int id, DatabaseEntities de)
        {
            return de.MaintenanceTasks.Where(x => x.Id == id).FirstOrDefault(x => x.IsActive == 1);
        }

        public bool AddMaintenanceTask(MaintenanceTask MaintenanceTask, DatabaseEntities de)
        {
            try
            {
                de.MaintenanceTasks.Add(MaintenanceTask);
                de.SaveChanges();

                EquipmentCodeHistory history = new EquipmentCodeHistory();
                history.EquipmentCodeId = MaintenanceTask.EquipmentCodeId;
                history.TaskId = MaintenanceTask.Id;
                history.Status = "Maintenance Task is Added";
                history.IsActive = 1;
                history.UpdatedBy = gp.ValidateLoggedinUser().Id;
                history.CreatedAt = GeneralPurpose.DateTimeNow();
                new EquipmentCodeHistoryBL().AddEquipmentCodeHistory(history, de);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int AddMaintenanceTask2(MaintenanceTask MaintenanceTask, DatabaseEntities de)
        {
            try
            {
                de.MaintenanceTasks.Add(MaintenanceTask);
                de.SaveChanges();

                EquipmentCodeHistory history = new EquipmentCodeHistory();
                history.EquipmentCodeId = MaintenanceTask.EquipmentCodeId;
                history.TaskId = MaintenanceTask.Id;
                history.Status = "Maintenance Task is Added";
                history.IsActive = 1;
                history.UpdatedBy = gp.ValidateLoggedinUser().Id;
                history.CreatedAt = GeneralPurpose.DateTimeNow();
                new EquipmentCodeHistoryBL().AddEquipmentCodeHistory(history, de);

                return MaintenanceTask.Id;
            }
            catch
            {
                return -1;
            }
        }

        public bool UpdateMaintenanceTask(MaintenanceTask MaintenanceTask, DatabaseEntities de)
        {
            try
            {
                de.Entry(MaintenanceTask).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();

                EquipmentCodeHistory history = new EquipmentCodeHistory();
                string Status = "";
                if (MaintenanceTask.IsActive == 0)
                {
                    List<WorkOrder> OrderList = new WorkOrderDAL().GetActiveWorkOrdersList(de).Where(x => x.MaintenanceTaskId == MaintenanceTask.Id).ToList();
                    foreach (WorkOrder o in OrderList)
                    {
                        o.IsActive = 0;
                        new WorkOrderDAL().UpdateWorkOrder(o, de);
                    }
                    Status = "Maintenance Task is Deleted";
                }
                else
                {
                    Status = "Maintenance Task is Updated";
                }

                history.EquipmentCodeId = MaintenanceTask.EquipmentCodeId;
                history.TaskId = MaintenanceTask.Id;
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

        public bool DeleteMaintenanceTask(int id, DatabaseEntities de)
        {
            try
            {
                de.MaintenanceTasks.Remove(de.MaintenanceTasks.Where(x => x.Id == id).FirstOrDefault());
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