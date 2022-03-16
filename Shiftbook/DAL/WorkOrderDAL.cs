using Shiftbook.BL;
using Shiftbook.Helping_Classes;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.DAL
{
    public class WorkOrderDAL
    {
        GeneralPurpose gp = new GeneralPurpose();
        public List<WorkOrder> GetAllWorkOrdersList(DatabaseEntities de)
        {
            return de.WorkOrders.ToList();
        }

        public List<WorkOrder> GetActiveWorkOrdersList(DatabaseEntities de)
        {
            return de.WorkOrders.Where(x => x.IsActive == 1).ToList();
        }

        public WorkOrder GetWorkOrderById(int id, DatabaseEntities de)
        {
            return de.WorkOrders.Where(x => x.Id == id).FirstOrDefault();
        }

        public WorkOrder GetActiveWorkOrderById(int id, DatabaseEntities de)
        {
            return de.WorkOrders.Where(x => x.Id == id).FirstOrDefault(x => x.IsActive == 1);
        }

        public bool AddWorkOrder(WorkOrder WorkOrder, DatabaseEntities de)
        {
            try
            {
                de.WorkOrders.Add(WorkOrder);
                de.SaveChanges();

                EquipmentCodeHistory history = new EquipmentCodeHistory();
                history.EquipmentCodeId = WorkOrder.EquipmentCodeId;
                history.TaskId = WorkOrder.MaintenanceTaskId;
                history.WorkOrderId = WorkOrder.Id;
                history.Status = "Work Order is Added";
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

        public int AddWorkOrder2(WorkOrder WorkOrder, DatabaseEntities de)
        {
            try
            {
                de.WorkOrders.Add(WorkOrder);
                de.SaveChanges();

                EquipmentCodeHistory history = new EquipmentCodeHistory();
                history.EquipmentCodeId = WorkOrder.EquipmentCodeId;
                history.TaskId = WorkOrder.MaintenanceTaskId;
                history.WorkOrderId = WorkOrder.Id;
                history.Status = "Work Order is Added";
                history.IsActive = 1;
                history.UpdatedBy = gp.ValidateLoggedinUser().Id;
                history.CreatedAt = GeneralPurpose.DateTimeNow();
                new EquipmentCodeHistoryBL().AddEquipmentCodeHistory(history, de);

                return WorkOrder.Id;
            }
            catch
            {
                return -1;
            }
        }

        public bool UpdateWorkOrder(WorkOrder WorkOrder, DatabaseEntities de)
        {
            try
            {
                de.Entry(WorkOrder).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();
                EquipmentCodeHistory history = new EquipmentCodeHistory();
                string Status = "";
                if (WorkOrder.IsActive == 0)
                {
                    List<WorkOrderHistory> historyList = new WorkOrderHistoryDAL().GetActiveWorkOrderHistorysList(de).Where(x => x.WorkOrderId == WorkOrder.Id).ToList();
                    foreach (WorkOrderHistory h in historyList)
                    {
                        h.IsActive = 0;
                        new WorkOrderHistoryDAL().UpdateWorkOrderHistory(h, de);
                    }

                    List<WorkOrder> FollowOrderList = new WorkOrderDAL().GetActiveWorkOrdersList(de).Where(x => x.FollowupParentId == WorkOrder.Id).ToList();
                    foreach (WorkOrder o in FollowOrderList)
                    {
                        o.IsActive = 0;
                        de.Entry(o).State = System.Data.Entity.EntityState.Modified;
                        de.SaveChanges();
                    }

                    Status = "Work Order is Deleted";
                }
                else
                {
                    Status = "Work Order is Updated";
                }
                
                history.EquipmentCodeId = WorkOrder.EquipmentCodeId;
                history.TaskId = WorkOrder.MaintenanceTaskId;
                history.WorkOrderId = WorkOrder.Id;
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

        public bool DeleteWorkOrder(int id, DatabaseEntities de)
        {
            try
            {
                de.WorkOrders.Remove(de.WorkOrders.Where(x => x.Id == id).FirstOrDefault());
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