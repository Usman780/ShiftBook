using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.DAL
{
    public class EquipmentHistoryDAL
    {
        public List<EquipmentCodeHistory> GetAllEquipmentCodeHistoriesList(DatabaseEntities de)
        {
            return de.EquipmentCodeHistories.ToList();
        }

        public List<EquipmentCodeHistory> GetActiveEquipmentCodeHistoriesList(DatabaseEntities de)
        {
            return de.EquipmentCodeHistories.Where(x => x.IsActive == 1).ToList();
        }

        public EquipmentCodeHistory GetEquipmentCodeHistoryById(int id, DatabaseEntities de)
        {
            return de.EquipmentCodeHistories.Where(x => x.Id == id).FirstOrDefault();
        }

        public EquipmentCodeHistory GetActiveEquipmentCodeHistoryById(int id, DatabaseEntities de)
        {
            return de.EquipmentCodeHistories.Where(x => x.Id == id).FirstOrDefault(x => x.IsActive == 1);
        }

        public bool AddEquipmentCodeHistory(EquipmentCodeHistory EquipmentCodeHistory, DatabaseEntities de)
        {
            try
            {
                de.EquipmentCodeHistories.Add(EquipmentCodeHistory);
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int AddEquipmentCodeHistory2(EquipmentCodeHistory EquipmentCodeHistory, DatabaseEntities de)
        {
            try
            {
                de.EquipmentCodeHistories.Add(EquipmentCodeHistory);
                de.SaveChanges();

                return EquipmentCodeHistory.Id;
            }
            catch
            {
                return -1;
            }
        }

        public bool UpdateEquipmentCodeHistory(EquipmentCodeHistory EquipmentCodeHistory, DatabaseEntities de)
        {
            try
            {
                de.Entry(EquipmentCodeHistory).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteEquipmentCodeHistory(int id, DatabaseEntities de)
        {
            try
            {
                EquipmentCodeHistory EquipmentCodeHistory = de.EquipmentCodeHistories.Remove(de.EquipmentCodeHistories.Where(x => x.Id == id).FirstOrDefault());
                EquipmentCodeHistory.IsActive = 0;
                de.Entry(EquipmentCodeHistory).State = System.Data.Entity.EntityState.Modified;
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