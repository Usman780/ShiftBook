using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.DAL
{
    public class DepartmentDAL
    {
        public List<Department> GetAllDepartmentsList(DatabaseEntities de)
        {
            return de.Departments.ToList();
        }

        public List<Department> GetActiveDepartmentsList(DatabaseEntities de)
        {
            return de.Departments.Where(x => x.IsActive == 1).ToList();
        }

        public Department GetDepartmentById(int id, DatabaseEntities de)
        {
            return de.Departments.Where(x => x.Id == id).FirstOrDefault();
        }

        public Department GetActiveDepartmentById(int id, DatabaseEntities de)
        {
            return de.Departments.Where(x => x.Id == id).FirstOrDefault(x => x.IsActive == 1);
        }

        public bool AddDepartment(Department Department, DatabaseEntities de)
        {
            try
            {
                de.Departments.Add(Department);
                de.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public int AddDepartment2(Department Department, DatabaseEntities de)
        {
            try
            {
                de.Departments.Add(Department);
                de.SaveChanges();

                return Department.Id;
            }
            catch
            {
                return -1;
            }
        }

        public bool UpdateDepartment(Department Department, DatabaseEntities de)
        {
            try
            {
                de.Entry(Department).State = System.Data.Entity.EntityState.Modified;
                de.SaveChanges();

                if (Department.IsActive == 0)
                {
                    List<User> userList = new UserDAL().GetActiveUsersListByDep(Department.Id, de);
                    foreach (User u in userList)
                    {
                        u.IsActive = 0;
                        new UserDAL().UpdateUser(u, de);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteDepartment(int id, DatabaseEntities de)
        {
            try
            {
                de.Departments.Remove(de.Departments.Where(x => x.Id == id).FirstOrDefault());
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