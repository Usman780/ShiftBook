using Shiftbook.DAL;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.BL
{
    public class EquipmentCodeBL
    {
        public List<EquipmentCode> GetAllEquipmentCodesList(DatabaseEntities de)
        {
            return new EquipmentCodeDAL().GetAllEquipmentCodesList(de);
        }

        public List<EquipmentCode> GetActiveEquipmentCodesList(DatabaseEntities de)
        {
            return new EquipmentCodeDAL().GetActiveEquipmentCodesList(de);
        }

        public EquipmentCode GetEquipmentCodeById(int id, DatabaseEntities de)
        {
            return new EquipmentCodeDAL().GetEquipmentCodeById(id, de);
        }

        public EquipmentCode GetActiveEquipmentCodeById(int id, DatabaseEntities de)
        {
            return new EquipmentCodeDAL().GetActiveEquipmentCodeById(id, de);
        }

        public List<EquipmentCode> GetEquipmentCodeListByCode(string code, DatabaseEntities de)
        {
            return new EquipmentCodeDAL().GetEquipmentCodeListByCode(code, de);
        }

        public bool AddEquipmentCode(EquipmentCode EquipmentCode, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(EquipmentCode.Code))
            {
                return false;
            }
            else
            {
                return new EquipmentCodeDAL().AddEquipmentCode(EquipmentCode, de);
            }
        }

        public int AddEquipmentCode2(EquipmentCode EquipmentCode, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(EquipmentCode.Code))
            {
                return -1;
            }
            else
            {
                return new EquipmentCodeDAL().AddEquipmentCode2(EquipmentCode, de);
            }
        }


        public bool UpdateEquipmentCode(EquipmentCode EquipmentCode, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(EquipmentCode.Code))
            {
                return false;
            }
            else
            {
                return new EquipmentCodeDAL().UpdateEquipmentCode(EquipmentCode, de);
            }
        }

        public bool DeleteEquipmentCode(int id, DatabaseEntities de)
        {
            return new EquipmentCodeDAL().DeleteEquipmentCode(id, de);
        }
    }
}