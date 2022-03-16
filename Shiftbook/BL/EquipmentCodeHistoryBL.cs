using Shiftbook.DAL;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.BL
{
    public class EquipmentCodeHistoryBL
    {
        public List<EquipmentCodeHistory> GetAllEquipmentCodeHistoriesList(DatabaseEntities de)
        {
            return new EquipmentHistoryDAL().GetAllEquipmentCodeHistoriesList(de);
        }

        public List<EquipmentCodeHistory> GetActiveEquipmentCodeHistoriesList(DatabaseEntities de)
        {
            return new EquipmentHistoryDAL().GetActiveEquipmentCodeHistoriesList(de);
        }

        public EquipmentCodeHistory GetEquipmentCodeHistoryById(int id, DatabaseEntities de)
        {
            return new EquipmentHistoryDAL().GetEquipmentCodeHistoryById(id, de);
        }

        public EquipmentCodeHistory GetActiveEquipmentCodeHistoryById(int id, DatabaseEntities de)
        {
            return new EquipmentHistoryDAL().GetActiveEquipmentCodeHistoryById(id, de);
        }

        public bool AddEquipmentCodeHistory(EquipmentCodeHistory EquipmentCodeHistory, DatabaseEntities de)
        {
            if (EquipmentCodeHistory.IsActive == null)
            {
                return false;
            }
            else
            {
                return new EquipmentHistoryDAL().AddEquipmentCodeHistory(EquipmentCodeHistory, de);
            }
        }

        public int AddEquipmentCodeHistory2(EquipmentCodeHistory EquipmentCodeHistory, DatabaseEntities de)
        {
            if (EquipmentCodeHistory.IsActive == null)
            {
                return -1;
            }
            else
            {
                return new EquipmentHistoryDAL().AddEquipmentCodeHistory2(EquipmentCodeHistory, de);
            }
        }


        public bool UpdateEquipmentCodeHistory(EquipmentCodeHistory EquipmentCodeHistory, DatabaseEntities de)
        {
            if (EquipmentCodeHistory.IsActive == null)
            {
                return false;
            }
            else
            {
                return new EquipmentHistoryDAL().UpdateEquipmentCodeHistory(EquipmentCodeHistory, de);
            }
        }

        public bool DeleteEquipmentCodeHistory(int id, DatabaseEntities de)
        {
            return new EquipmentHistoryDAL().DeleteEquipmentCodeHistory(id, de);
        }
    }
}