using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.DAL
{
    public class TypeDAL
    {
        public List<Models.Type> GetAllTypesList(DatabaseEntities de)
        {
            return de.Types.ToList();
        }

        public List<Models.Type> GetActiveTypesList(DatabaseEntities de)
        {
            return de.Types.Where(x => x.IsActive == 1).ToList();
        }

        public Models.Type GetTypeById(int id, DatabaseEntities de)
        {
            return de.Types.Where(x => x.Id == id).FirstOrDefault();
        }

        public Models.Type GetActiveTypeById(int id, DatabaseEntities de)
        {
            return de.Types.Where(x => x.Id == id).FirstOrDefault(x => x.IsActive == 1);
        }

        public bool AddType(Models.Type Type, DatabaseEntities de)
        {
            try
            {
                de.Types.Add(Type);
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int AddType2(Models.Type Type, DatabaseEntities de)
        {
            try
            {
                de.Types.Add(Type);
                de.SaveChanges();

                return Type.Id;
            }
            catch
            {
                return -1;
            }
        }

        public bool UpdateType(Models.Type Type, DatabaseEntities de)
        {
            try
            {
                de.Entry(Type).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();

                if (Type.IsActive == 0)
                {
                    List<WorkOrder> OrderList = new WorkOrderDAL().GetActiveWorkOrdersList(de).Where(x => x.Id == Type.Id).ToList();
                    foreach (WorkOrder o in OrderList)
                    {
                        o.IsActive = 0;
                        new WorkOrderDAL().UpdateWorkOrder(o, de);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteType(int id, DatabaseEntities de)
        {
            try
            {
                Models.Type type = de.Types.Remove(de.Types.Where(x => x.Id == id).FirstOrDefault());
                type.IsActive = 0;
                de.Entry(type).State = System.Data.Entity.EntityState.Modified;
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