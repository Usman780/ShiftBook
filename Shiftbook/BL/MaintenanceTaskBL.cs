using Shiftbook.DAL;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.BL
{
    public class MaintenanceTaskBL
    {
        public List<MaintenanceTask> GetAllMaintenanceTasksList(DatabaseEntities de)
        {
            return new MaintenanceTaskDAL().GetAllMaintenanceTasksList(de);
        }

        public List<MaintenanceTask> GetActiveMaintenanceTasksList(DatabaseEntities de)
        {
            return new MaintenanceTaskDAL().GetActiveMaintenanceTasksList(de);
        }

        public MaintenanceTask GetMaintenanceTaskById(int id, DatabaseEntities de)
        {
            return new MaintenanceTaskDAL().GetMaintenanceTaskById(id, de);
        }

        public MaintenanceTask GetActiveMaintenanceTaskById(int id, DatabaseEntities de)
        {
            return new MaintenanceTaskDAL().GetActiveMaintenanceTaskById(id, de);
        }

        public bool AddMaintenanceTask(MaintenanceTask MaintenanceTask, DatabaseEntities de)
        {
            return new MaintenanceTaskDAL().AddMaintenanceTask(MaintenanceTask, de);
        }

        public int AddMaintenanceTask2(MaintenanceTask MaintenanceTask, DatabaseEntities de)
        {
            return new MaintenanceTaskDAL().AddMaintenanceTask2(MaintenanceTask, de);
        }


        public bool UpdateMaintenanceTask(MaintenanceTask MaintenanceTask, DatabaseEntities de)
        {
            return new MaintenanceTaskDAL().UpdateMaintenanceTask(MaintenanceTask, de);
        }

        public bool DeleteMaintenanceTask(int id, DatabaseEntities de)
        {
            return new MaintenanceTaskDAL().DeleteMaintenanceTask(id, de);
        }
    }
}