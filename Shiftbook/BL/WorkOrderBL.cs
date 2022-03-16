using Shiftbook.DAL;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.BL
{
    public class WorkOrderBL
    {
        public List<WorkOrder> GetAllWorkOrdersList(DatabaseEntities de)
        {
            return new WorkOrderDAL().GetAllWorkOrdersList(de);
        }

        public List<WorkOrder> GetActiveWorkOrdersList(DatabaseEntities de)
        {
            return new WorkOrderDAL().GetActiveWorkOrdersList(de);
        }

        public WorkOrder GetWorkOrderById(int id, DatabaseEntities de)
        {
            return new WorkOrderDAL().GetWorkOrderById(id, de);
        }

        public WorkOrder GetActiveWorkOrderById(int id, DatabaseEntities de)
        {
            return new WorkOrderDAL().GetActiveWorkOrderById(id, de);
        }

        public bool AddWorkOrder(WorkOrder WorkOrder, DatabaseEntities de)
        {
            return new WorkOrderDAL().AddWorkOrder(WorkOrder, de);
        }

        public int AddWorkOrder2(WorkOrder WorkOrder, DatabaseEntities de)
        {
            return new WorkOrderDAL().AddWorkOrder2(WorkOrder, de);
        }

        public bool UpdateWorkOrder(WorkOrder WorkOrder, DatabaseEntities de)
        {
            return new WorkOrderDAL().UpdateWorkOrder(WorkOrder, de);
        }

        public bool DeleteWorkOrder(int id, DatabaseEntities de)
        {
            return new WorkOrderDAL().DeleteWorkOrder(id, de);
        }
    }
}