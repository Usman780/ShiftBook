using Shiftbook.DAL;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.BL
{
    public class WorkOrderHistoryBL
    {
        public List<WorkOrderHistory> GetAllWorkOrderHistorysList(DatabaseEntities de)
        {
            return new WorkOrderHistoryDAL().GetAllWorkOrderHistorysList(de);
        }

        public List<WorkOrderHistory> GetActiveWorkOrderHistorysList(DatabaseEntities de)
        {
            return new WorkOrderHistoryDAL().GetActiveWorkOrderHistorysList(de);
        }

        public WorkOrderHistory GetWorkOrderHistoryById(int id, DatabaseEntities de)
        {
            return new WorkOrderHistoryDAL().GetWorkOrderHistoryById(id, de);
        }

        public WorkOrderHistory GetActiveWorkOrderHistoryById(int id, DatabaseEntities de)
        {
            return new WorkOrderHistoryDAL().GetActiveWorkOrderHistoryById(id, de);
        }

        public bool AddWorkOrderHistory(WorkOrderHistory WorkOrderHistory, DatabaseEntities de)
        {
            return new WorkOrderHistoryDAL().AddWorkOrderHistory(WorkOrderHistory, de);
        }

        public int AddWorkOrderHistory2(WorkOrderHistory WorkOrderHistory, DatabaseEntities de)
        {
            return new WorkOrderHistoryDAL().AddWorkOrderHistory2(WorkOrderHistory, de);
        }

        public bool UpdateWorkOrderHistory(WorkOrderHistory WorkOrderHistory, DatabaseEntities de)
        {
            return new WorkOrderHistoryDAL().UpdateWorkOrderHistory(WorkOrderHistory, de);
        }

        public bool DeleteWorkOrderHistory(int id, DatabaseEntities de)
        {
            return new WorkOrderHistoryDAL().DeleteWorkOrderHistory(id, de);
        }
    }
}