using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.DAL
{
    public class WorkOrderHistoryDAL
    {
        public List<WorkOrderHistory> GetAllWorkOrderHistorysList(DatabaseEntities de)
        {
            return de.WorkOrderHistories.ToList();
        }

        public List<WorkOrderHistory> GetActiveWorkOrderHistorysList(DatabaseEntities de)
        {
            return de.WorkOrderHistories.Where(x => x.IsActive == 1).ToList();
        }

        public WorkOrderHistory GetWorkOrderHistoryById(int id, DatabaseEntities de)
        {
            return de.WorkOrderHistories.Where(x => x.Id == id).FirstOrDefault();
        }

        public WorkOrderHistory GetActiveWorkOrderHistoryById(int id, DatabaseEntities de)
        {
            return de.WorkOrderHistories.Where(x => x.Id == id).FirstOrDefault(x => x.IsActive == 1);
        }

        public bool AddWorkOrderHistory(WorkOrderHistory WorkOrderHistory, DatabaseEntities de)
        {
            try
            {
                de.WorkOrderHistories.Add(WorkOrderHistory);
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int AddWorkOrderHistory2(WorkOrderHistory WorkOrderHistory, DatabaseEntities de)
        {
            try
            {
                de.WorkOrderHistories.Add(WorkOrderHistory);
                de.SaveChanges();

                return WorkOrderHistory.Id;
            }
            catch
            {
                return -1;
            }
        }

        public bool UpdateWorkOrderHistory(WorkOrderHistory WorkOrderHistory, DatabaseEntities de)
        {
            try
            {
                de.Entry(WorkOrderHistory).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteWorkOrderHistory(int id, DatabaseEntities de)
        {
            try
            {
                de.WorkOrderHistories.Remove(de.WorkOrderHistories.Where(x => x.Id == id).FirstOrDefault());
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