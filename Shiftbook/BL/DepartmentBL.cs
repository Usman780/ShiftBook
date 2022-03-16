using Shiftbook.DAL;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.BL
{
    public class DepartmentBL
    {
        public List<Department> GetAllDepartmentsList(DatabaseEntities de)
        {
            return new DepartmentDAL().GetAllDepartmentsList(de);
        }

        public List<Department> GetActiveDepartmentsList(DatabaseEntities de)
        {
            return new DepartmentDAL().GetActiveDepartmentsList(de);
        }

        public Department GetDepartmentById(int id, DatabaseEntities de)
        {
            return new DepartmentDAL().GetDepartmentById(id, de);
        }

        public Department GetActiveDepartmentById(int id, DatabaseEntities de)
        {
            return new DepartmentDAL().GetActiveDepartmentById(id, de);
        }

        public bool AddDepartment(Department Department, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(Department.Name))
            {
                return false;
            }
            else
            {
                return new DepartmentDAL().AddDepartment(Department, de);
            }
        }

        public int AddDepartment2(Department Department, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(Department.Name))
            {
                return -1;
            }
            else
            {
                return new DepartmentDAL().AddDepartment2(Department, de);
            }
        }


        public bool UpdateDepartment(Department Department, DatabaseEntities de)
        {
            if (String.IsNullOrEmpty(Department.Name))
            {
                return false;
            }
            else
            {
                return new DepartmentDAL().UpdateDepartment(Department, de);
            }
        }

        public bool DeleteDepartment(int id, DatabaseEntities de)
        {
            return new DepartmentDAL().DeleteDepartment(id, de);
        }
    }
}