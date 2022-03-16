using Shiftbook.DAL;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.BL
{
    public class TypeBL
    {
        public List<Models.Type> GetAllTypesList(DatabaseEntities de)
        {
            return new TypeDAL().GetAllTypesList(de);
        }

        public List<Models.Type> GetActiveTypesList(DatabaseEntities de)
        {
            return new TypeDAL().GetActiveTypesList(de);
        }

        public Models.Type GetTypeById(int id, DatabaseEntities de)
        {
            return new TypeDAL().GetTypeById(id, de);
        }

        public Models.Type GetActiveTypeById(int id, DatabaseEntities de)
        {
            return new TypeDAL().GetActiveTypeById(id, de);
        }

        public bool AddType(Models.Type Type, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(Type.TypeName))
            {
                return false;
            }
            else
            {
                return new TypeDAL().AddType(Type, de);
            }
        }

        public int AddType2(Models.Type Type, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(Type.TypeName))
            {
                return -1;
            }
            else
            {
                return new TypeDAL().AddType2(Type, de);
            }
        }


        public bool UpdateType(Models.Type Type, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(Type.TypeName))
            {
                return false;
            }
            else
            {
                return new TypeDAL().UpdateType(Type, de);
            }
        }

        public bool DeleteType(int id, DatabaseEntities de)
        {
            return new TypeDAL().DeleteType(id, de);
        }
    }
}