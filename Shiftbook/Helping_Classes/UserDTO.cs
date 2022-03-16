using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Shiftbook.Helping_Classes
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string EncId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string UserName { get; set; }
        public string Language { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public string UserRole { get; set; }
        public int Author { get; set; }
    }
}