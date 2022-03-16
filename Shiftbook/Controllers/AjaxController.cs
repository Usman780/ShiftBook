using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shiftbook.Models;
using Shiftbook.Helping_Classes;
using Shiftbook.BL;
using System.Text;

namespace Shiftbook.Controllers
{
    public class AjaxController : Controller
    {
        GeneralPurpose gp = new GeneralPurpose();
        DatabaseEntities de = new DatabaseEntities();

        #region User

        [HttpPost]
        public ActionResult GetUserDataTableList(string fName = "", string lName="", string email = "", int role = -1)
        {
            List<User> ulist = new UserBL().GetActiveUsersList(de).Where(x => x.IsPrimary != 1 ).ToList();

            if(gp.ValidateLoggedinUser().IsPrimary != 1)
            {
                ulist = ulist.Where(x => x.Role != 1).ToList();
            }

            if(fName != "")
            {
                ulist = ulist.Where(x => x.FName.ToLower().Contains(fName.ToLower())).ToList();
            }
            if (lName != "")
            {
                ulist = ulist.Where(x => x.LName.ToLower().Contains(lName.ToLower())).ToList();
            }
            if (email != "")
            {
                ulist = ulist.Where(x => x.Email.ToLower().Contains(email.ToLower())).ToList();
            }
            if (role != -1)
            {
                ulist = ulist.Where(x => x.Role == role).ToList();
            }

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null)
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        if (sortColumnName == "Department")
                        {
                            ulist = ulist.OrderByDescending(x => x.Department.Name).ToList();
                        }
                        else
                        {
                            ulist = ulist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                        }
                    }
                    else
                    {
                        if (sortColumnName == "Department")
                        {
                            ulist = ulist.OrderBy(x => x.Department.Name).ToList();
                        }
                        else
                        {
                            ulist = ulist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                        }
                    }
                }
            }

            int totalrows = ulist.Count();

            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                ulist = ulist.Where(x => x.Email.ToLower().Contains(searchValue.ToLower()) ||
                                    x.FName.ToLower().Contains(searchValue.ToLower()) ||
                                    x.LName.ToLower().Contains(searchValue.ToLower()) ||
                                    (x.Contact != null && x.Contact.ToLower().Contains(searchValue.ToLower()))).ToList();
            }

            int totalrowsafterfilterinig = ulist.Count();


            // pagination
            ulist = ulist.Skip(start).Take(length).ToList();

            List<UserDTO> udto = new List<UserDTO>();

            foreach (User u in ulist)
            {
                UserDTO obj = new UserDTO()
                {
                    Id = u.Id,
                    EncId = GeneralPurpose.EncryptId(u.Id),
                    DepartmentId = (int)u.DepartmentId,
                    DepartmentName = u.Department.Name,
                    FName = u.FName,
                    LName = u.LName,
                    UserName = u.UserName,
                    Language = u.Language,
                    Contact = u.Contact,
                    Email = u.Email,
                    //Password = StringCipher.Decrypt(u.Password),
                    Author = u.Author==null?0:1,
                    Role = (int)u.Role
                };

                udto.Add(obj);
            }

            return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetUserList(int role = -1)
        {
            List<User> uList = new UserBL().GetActiveUsersList(de).Where(x=> x.IsPrimary != 1).OrderBy(x=> x.FName).ToList();

            if(role == 1)
            {
                uList = uList.Where(x => x.Role == role).ToList();
            }
            if (role == 2)
            {
                uList = uList.Where(x => x.Role == role).ToList();
            }
            if (role == 3)
            {
                uList = uList.Where(x => x.Role == role).ToList();
            }
            if (role == 4)
            {
                uList = uList.Where(x => x.Role == role).ToList();
            }

            List<UserDTO> udto = new List<UserDTO>();
            string DepartmentName = "";
            foreach (User u in uList)
            {
                if(u.DepartmentId != null)
                {
                    DepartmentName = u.Department.Name;
                }
                else
                {
                    DepartmentName = "";
                }
                UserDTO obj = new UserDTO()
                {
                    Id = u.Id,
                    EncId = GeneralPurpose.EncryptId(u.Id),
                    DepartmentName = DepartmentName,
                    FName = u.FName + " " + u.LName,
                    UserRole = GeneralPurpose.GetUserRole((int)u.Role)
                };

                udto.Add(obj);
            }

            return new JsonResult()
            {
                Data = udto,
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }


        [HttpPost]
        public ActionResult GetUserById(int id)
        {
            User u = new UserBL().GetActiveUserById(id, de);
            if (u == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            UserDTO obj = new UserDTO()
            {
                Id = u.Id,
                EncId = GeneralPurpose.EncryptId(u.Id),
                DepartmentId = (int)u.DepartmentId,
                DepartmentName = u.Department.Name,
                FName = u.FName,
                LName = u.LName,
                UserName = u.UserName,
                Language = u.Language,
                Contact = u.Contact,
                Email = u.Email,
                Password = StringCipher.Decrypt(u.Password),
                Author = u.Author==null? 0 : 1,
                Role = (int)u.Role
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult ValidateEmail(string email, int id = -1)
        {
            return Json(gp.ValidateEmail(email, id), JsonRequestBehavior.AllowGet);
        }

        public bool CheckEmailValidity(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Department

        [HttpPost]
        public ActionResult GetDepartmentDataTableList(string Name = "", string Location = "")
        {
            List<Department> list = new DepartmentBL().GetActiveDepartmentsList(de).ToList();

            if (Name != "")
            {
                list = list.Where(x => x.Name.ToLower().Contains(Name.ToLower())).ToList();
            }
            if (Location != "")
            {
                list = list.Where(x => x.Location != null).ToList();
                list = list.Where(x => x.Location.ToLower().Contains(Location.ToLower())).ToList();
            }

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null)
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        list = list.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                    else
                    {
                        list = list.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                }
            }

            int totalrows = list.Count();

            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                list = list.Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) ||
                                    x.Location.ToLower().Contains(searchValue.ToLower())
                                    ).ToList();
            }

            int totalrowsafterfilterinig = list.Count();


            // pagination
            list = list.Skip(start).Take(length).ToList();

            List<DTO> DtoList = new List<DTO>();
            int count = 0;
            foreach (Department u in list)
            {
                DTO obj = new DTO()
                {
                    count = count++,
                    Id = u.Id,
                    EncId = GeneralPurpose.EncryptId(u.Id),
                    Name = u.Name,
                    Location = u.Location
                };

                DtoList.Add(obj);
            }

            return Json(new { data = DtoList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDepartmentList()
        {
            List<Department> dList = new DepartmentBL().GetActiveDepartmentsList(de);

            List<DTO> ddto = new List<DTO>();

            foreach (Department d in dList)
            {
                DTO obj = new DTO()
                {
                    Id = d.Id,
                    EncId = GeneralPurpose.EncryptId(d.Id),
                    Name = d.Name,
                    Location = d.Location,
                };

                ddto.Add(obj);
            }

            return new JsonResult()
            {
                Data = ddto,
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult GetDepartmentById(int id)
        {
            Department d = new DepartmentBL().GetActiveDepartmentById(id, de);
            if (d == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            DTO obj = new DTO()
            {
                Id = d.Id,
                EncId = GeneralPurpose.EncryptId(d.Id),
                Name = d.Name,
                Location = d.Location,
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValidateDepartment(string name, int id = -1)
        {
            return Json(gp.ValidateDepartment(name, id), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region EquipmentCode

        [HttpPost]
        public ActionResult GetEquipmentCodeDataTableList()
        {
            List<EquipmentCode> clist = new EquipmentCodeBL().GetActiveEquipmentCodesList(de).OrderByDescending(x=> x.Id).ToList();

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null)
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        clist = clist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                    else
                    {
                        clist = clist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                }
            }

            int totalrows = clist.Count();

            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                clist = clist.Where(x => x.Code.ToLower().Contains(searchValue.ToLower()) ||
                                    (x.EquipmentName != null && x.EquipmentName.ToLower().Contains(searchValue.ToLower())) ||
                                    (x.Description != null && x.Description.ToLower().Contains(searchValue.ToLower())) 
                                    ).ToList();
            }

            int totalrowsafterfilterinig = clist.Count();


            // pagination
            clist = clist.Skip(start).Take(length).ToList();

            List<DTO> cdto = new List<DTO>();

            foreach (EquipmentCode c in clist)
            {
                DTO obj = new DTO()
                {
                    EId = c.Id,
                    EEncId = GeneralPurpose.EncryptId(c.Id),
                    ECode = c.Code,
                    EEquipmentName = c.EquipmentName,
                    EDescription = c.Description
                };

                cdto.Add(obj);
            }

            return Json(new { data = cdto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetEquipmentCodeList(string code = "")
        {
            List<EquipmentCode> clist = new EquipmentCodeBL().GetActiveEquipmentCodesList(de);

            if(code != "")
            {
                clist = clist.Where(x => x.Code.ToLower().Contains(code.ToLower())).ToList();
            }

            List<EquipmentCode> cdto = new List<EquipmentCode>();

            foreach (EquipmentCode c in clist)
            {
                EquipmentCode obj = new EquipmentCode()
                {
                    Id = c.Id,
                    Code = c.Code,
                };

                cdto.Add(obj);
            }

            return new JsonResult()
            {
                Data = cdto,
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult GetEquipmentCodeById(int id)
        {
            EquipmentCode c = new EquipmentCodeBL().GetActiveEquipmentCodeById(id, de);
            
            if (c == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            
            EquipmentCode obj = new EquipmentCode()
            {
                Id = c.Id,
                Code = c.Code,
                EquipmentName = c.EquipmentName,
                Description = c.Description
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ValidateEquipmentCode(string code, int id = -1)
        {
            return Json(gp.ValidateEquipmentCode(code, id), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetEquipmentCodeHistory(int id = -1)
        {
            List<EquipmentCodeHistory> history = new EquipmentCodeHistoryBL().GetActiveEquipmentCodeHistoriesList(de).OrderByDescending(x => x.Id).ToList();

            history = history.Where(x => x.EquipmentCodeId == id).ToList();

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null)
            {
                if (sortColumnName != "0")
                {
                    if(sortColumnName  == "User")
                    {
                        if (sortDirection == "asc")
                        {
                            history = history.OrderBy(x => x.User.FName).ToList();
                        }
                        else
                        {
                            history = history.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                        }
                    }
                    else
                    {
                        if (sortDirection == "asc")
                        {
                            history = history.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                        }
                        else
                        {
                            history = history.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                        }
                    }
                }
            }

            int totalrows = history.Count();

            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                history = history.Where(x => x.Status != null && x.Status.ToLower().Contains(searchValue.ToLower()) 
                                    ).ToList();
            }

            int totalrowsafterfilterinig = history.Count();


            // pagination
            history = history.Skip(start).Take(length).ToList();

            List<DTO> cdto = new List<DTO>();
            string User = "";
            foreach (EquipmentCodeHistory c in history)
            {
                if(c.UpdatedBy != null)
                {
                    User = c.User.FName + " " + c.User.LName;
                }
                else
                {
                    User = "";
                }

                DTO obj = new DTO()
                {
                    HId = c.Id,
                    HEncId = GeneralPurpose.EncryptId(c.Id),
                    HStatus = c.Status,
                    HCreated = User,
                    HCreatedAt = c.CreatedAt.Value.ToString("f"),
                };

                cdto.Add(obj);
            }

            return Json(new { data = cdto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region MaintenanceTask

        [HttpPost]
        public ActionResult GetMaintenanceTaskDataTableList(string today = "", string Week = "", string Month = "",
            string code = "", string pId = "", string type = "", string searchdate = "", string searchweek = "",
            string searchmonth = "", string searchperiod_startdate = "", string searchperiod_enddate ="", int tag = -1)
        {
            //List<MaintenanceTask> mlist = new MaintenanceTaskBL().GetActiveMaintenanceTasksList(de).OrderByDescending(x => x.Id).ToList();
            List<MaintenanceTask> mlist2 = new MaintenanceTaskBL().GetActiveMaintenanceTasksList(de).ToList();
            List<MaintenanceTask> mlist = new List<MaintenanceTask>();
            List<TagsTask> tags = new List<TagsTask>();
            List<int> taskid = new List<int>();

            if (tag != -1)
            {
                foreach (var item in mlist2)
                {
                    int count = new TagsTaskBL().GetActiveTagsTaskList(de).Where(x => x.TagsId == tag && x.TaskId == item.Id).Count();
                    if (count != 0)
                    {
                        mlist.Add(item);
                    }
                }
            }
            else
            {
                mlist = mlist2;
            }

            DateTime Today = DateTime.Today;
            DateTime week = DateTime.Today.AddDays(-7);
            DateTime month = DateTime.Today.AddMonths(-1);

            if (today != "")
            {
                mlist = mlist.Where(x => Convert.ToDateTime(x.CreatedAt) >= Convert.ToDateTime(Today)).ToList();
            }

            if (Week != "")
            {
                mlist = mlist.Where(x => Convert.ToDateTime(x.CreatedAt) >= Convert.ToDateTime(week)).ToList();
            }

            if (Month != "")
            {
                mlist = mlist.Where(x => Convert.ToDateTime(x.CreatedAt) >= Convert.ToDateTime(month)).ToList();
            }

            if (code != "")
            {
                mlist = mlist.Where(x => x.EquipmentCode.Code.ToLower().Contains(code.ToLower())).ToList();
            }
            if (pId != "")
            {
                mlist = mlist.Where(x => x.Pid != null && x.Pid.ToLower().Contains(pId.ToLower())).ToList();
            }
            if (type != "")
            {
                mlist = mlist.Where(x => x.MaintenanceType.ToLower().Contains(type.ToLower())).ToList();
            }
            
            if(searchdate != "")
            {
                DateTime dt = Convert.ToDateTime(searchdate);
                mlist = mlist.Where(x => Convert.ToDateTime(x.CreatedAt).ToShortDateString() == dt.ToShortDateString()).ToList();
            }
            if(searchmonth != "")
            {
                DateTime dts = Convert.ToDateTime(searchmonth);
                DateTime dtss = Convert.ToDateTime(dts).AddMonths(1).AddSeconds(-1);
                mlist = mlist.Where(x => Convert.ToDateTime(x.CreatedAt) >= dts && Convert.ToDateTime(x.CreatedAt) <= dtss).ToList();
            }
            if(searchweek != "")
            {
                var thisWeekStart = Today.AddDays(-(int)Today.DayOfWeek);
                var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
                var lastWeekStart = thisWeekStart.AddDays(-7);
                var lastWeekEnd = thisWeekStart.AddSeconds(-1);
                var NextWeekStart = thisWeekEnd.AddSeconds(1);
                var NextWeekEnd = NextWeekStart.AddDays(7).AddSeconds(-1);

                if (searchweek == "This Week")
                {
                    mlist = mlist.Where(x => Convert.ToDateTime(x.CreatedAt).Date >= Convert.ToDateTime(thisWeekStart).Date &&
                    Convert.ToDateTime(x.CreatedAt).Date <= Convert.ToDateTime(thisWeekEnd).Date).ToList();
                }
                if (searchweek == "Previous Week")
                {
                    mlist = mlist.Where(x => Convert.ToDateTime(x.CreatedAt).Date >= Convert.ToDateTime(lastWeekEnd).Date &&
                    Convert.ToDateTime(x.CreatedAt).Date <= Convert.ToDateTime(lastWeekEnd).Date).ToList();
                }
                if (searchweek == "Next Week")
                {
                    mlist = mlist.Where(x => Convert.ToDateTime(x.CreatedAt).Date >= Convert.ToDateTime(NextWeekStart).Date &&
                    Convert.ToDateTime(x.CreatedAt).Date <= Convert.ToDateTime(NextWeekEnd).Date).ToList();
                }
            }
            if (searchperiod_startdate != "")
            {
                DateTime dt = Convert.ToDateTime(searchperiod_startdate);
                mlist = mlist.Where(x => Convert.ToDateTime(x.CreatedAt) >= dt).ToList();
            }

            if (searchperiod_enddate != "")
            {
                DateTime dt = Convert.ToDateTime(searchperiod_enddate);
                mlist = mlist.Where(x => Convert.ToDateTime(x.CreatedAt) <= dt).ToList();
            }


            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null)
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        if (sortColumnName == "EquipmentCode")
                        {
                            mlist = mlist.OrderByDescending(x => x.EquipmentCode.Code).ToList();
                        }
                        else
                        {
                            mlist = mlist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                        }
                    }
                    else
                    {
                        if (sortColumnName == "EquipmentCode")
                        {
                            mlist = mlist.OrderBy(x => x.EquipmentCode.Code).ToList();
                        }
                        else
                        {
                            mlist = mlist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                        }
                    }
                }
            }
            int totalrows = -1;
            totalrows = mlist.Count();


            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                mlist = mlist.Where(x => x.EquipmentCode.Code.ToLower().Contains(searchValue.ToLower()) ||
                                    (x.Pid != null && x.Pid.ToLower().Contains(searchValue.ToLower())) ||
                                    (x.MaintenanceType != null && x.MaintenanceType.ToLower().Contains(searchValue.ToLower())) ||
                                    (x.TaskDescription != null && x.TaskDescription.ToLower().Contains(searchValue.ToLower()))
                                    ).ToList();
            }
            int totalrowsafterfilterinig = -1;
            totalrowsafterfilterinig = mlist.Count();



            // pagination
            mlist = mlist.Skip(start).Take(length).ToList();

            List<TaskDTO> mdto = new List<TaskDTO>();
            string interval = "";
            foreach (MaintenanceTask m in mlist)
            {
                if (m != null)
                {
                    if (m.Interval != null)
                    {
                        interval = m.Interval.ToString();
                    }
                    else
                    {
                        interval = "";
                    }
                    TaskDTO obj = new TaskDTO()
                    {
                        Id = m.Id,
                        EncId = GeneralPurpose.EncryptId(m.Id),
                        EquipmentCode = m.EquipmentCode.Code,
                        AssetDescription = m.AssetDescription,
                        Pid = m.Pid,
                        SystemDescription = m.SystemDescription,
                        MaintenanceType = m.MaintenanceType,
                        TaskDescription = m.TaskDescription,
                        Comment = m.Comment,
                        Interval = interval,
                        Unit = m.Unit,
                        PlantShutDownJob = m.PlantShutDownJob,
                        AtexZone = m.AtexZone,
                        AuxiliaryMaterial = m.AuxiliaryMaterial,
                        GreaseOilManufacturer = m.GreaseOilManufacturer,
                        GreaseOilType = m.GreaseOilType,
                        TopupAmount = m.TopupAmount,
                        RoutineType = m.RoutineType.ToString(),
                    };

                    mdto.Add(obj);
                }
            }
            return Json(new { data = mdto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMaintenanceTaskList(string taskDes = "")
        {
            List<MaintenanceTask> mList = new MaintenanceTaskBL().GetActiveMaintenanceTasksList(de);

            if(taskDes != "")
            {
                mList = mList.Where(x => x.TaskDescription.ToLower().Contains(taskDes.ToLower())).ToList();
            }

            List<TaskDTO> tdto = new List<TaskDTO>();

            foreach (MaintenanceTask m in mList)
            {
                TaskDTO obj = new TaskDTO()
                {
                    Id = m.Id,
                    TaskDescription = m.TaskDescription,
                };

                tdto.Add(obj);
            }

            return new JsonResult()
            {
                Data = tdto,
                ContentType = "application/json",
                ContentEncoding = Encoding.UTF8,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult GetMaintenanceTaskById(int id)
        {
            MaintenanceTask m = new MaintenanceTaskBL().GetActiveMaintenanceTaskById(id, de);
            
            if(m == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            List<TagsTask> tagsTask = new TagsTaskBL().GetActiveTagsTaskList(de).Where(x => x.TaskId == m.Id).ToList();
            TaskDTO obj = new TaskDTO()
            {
                Id = m.Id,
                EncId = GeneralPurpose.EncryptId(m.Id),
                EquipmentCodeId = (int)m.EquipmentCodeId,
                EquipmentCode = m.EquipmentCode.Code,
                AssetDescription = m.AssetDescription,
                Pid = m.Pid,
                SystemDescription = m.SystemDescription,
                MaintenanceType = m.MaintenanceType,
                TaskDescription = m.TaskDescription,
                Comment = m.Comment,
                Interval = m.Interval.ToString(),
                Unit = m.Unit,
                PlantShutDownJob = m.PlantShutDownJob,
                AtexZone = m.AtexZone,
                AuxiliaryMaterial = m.AuxiliaryMaterial,
                GreaseOilManufacturer = m.GreaseOilManufacturer,
                GreaseOilType = m.GreaseOilType,
                TopupAmount = m.TopupAmount,
                RoutineType = m.RoutineType.ToString(),
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetTaskByTags(int WorkOrderId = -1)
        {
            string interval = "";
            WorkOrder w = new WorkOrderBL().GetActiveWorkOrderById(WorkOrderId, de);
            if(w != null)
            {
                List<TagsTask> tagsTasks = new TagsTaskBL().GetActiveTagsTaskList(de).Where(x => x.TagsId == w.TagsId).ToList();
                int start = Convert.ToInt32(Request["start"]);
                int length = Convert.ToInt32(Request["length"]);
                string searchValue = Request["search[value]"];
                string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
                string sortDirection = Request["order[0][dir]"];

                if (sortColumnName != "" && sortColumnName != null)
                {
                    if (sortColumnName != "0")
                    {
                        if (sortDirection == "asc")
                        {
                            if (sortColumnName == "EquipmentCode")
                            {
                                tagsTasks = tagsTasks.OrderByDescending(x => x.MaintenanceTask.EquipmentCode.Code).ToList();
                            }
                            else
                            {
                                tagsTasks = tagsTasks.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                            }
                        }
                        else
                        {
                            if (sortColumnName == "EquipmentCode")
                            {
                                tagsTasks = tagsTasks.OrderBy(x => x.MaintenanceTask.EquipmentCode.Code).ToList();
                            }
                            else
                            {
                                tagsTasks = tagsTasks.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                            }
                        }
                    }
                }

                int totalrows = tagsTasks.Count();

                //filter
                if (!string.IsNullOrEmpty(searchValue))
                {
                    tagsTasks = tagsTasks.Where(x => x.MaintenanceTask.EquipmentCode.Code.ToLower().Contains(searchValue.ToLower()) ||
                                        (x.MaintenanceTask.Pid != null && x.MaintenanceTask.Pid.ToLower().Contains(searchValue.ToLower())) ||
                                        (x.MaintenanceTask.MaintenanceType != null && x.MaintenanceTask.MaintenanceType.ToLower().Contains(searchValue.ToLower())) ||
                                        (x.MaintenanceTask.TaskDescription != null && x.MaintenanceTask.TaskDescription.ToLower().Contains(searchValue.ToLower()))
                                        ).ToList();
                }

                int totalrowsafterfilterinig = tagsTasks.Count();


                // pagination
                tagsTasks = tagsTasks.Skip(start).Take(length).ToList();
                List<TaskDTO> Dto = new List<TaskDTO>();
                if(tagsTasks != null)
                {
                    foreach (var x in tagsTasks)
                    {
                        MaintenanceTask maintenanceTask = new MaintenanceTaskBL().GetActiveMaintenanceTasksList(de).Where(q => q.Id == x.TaskId).FirstOrDefault();
                        if (maintenanceTask != null)
                        {
                            if (maintenanceTask.Interval != null)
                            {
                                interval = maintenanceTask.Interval.ToString();
                            }
                            else
                            {
                                interval = "";
                            }
                            TaskDTO newdto = new TaskDTO()
                            {
                                Id = maintenanceTask.Id,
                                EncId = GeneralPurpose.EncryptId(maintenanceTask.Id),
                                EquipmentCode = maintenanceTask.EquipmentCode.Code,
                                AssetDescription = maintenanceTask.AssetDescription,
                                Pid = maintenanceTask.Pid,
                                SystemDescription = maintenanceTask.SystemDescription,
                                MaintenanceType = maintenanceTask.MaintenanceType,
                                TaskDescription = maintenanceTask.TaskDescription,
                                Comment = maintenanceTask.Comment,
                                Interval = interval,
                                Unit = maintenanceTask.Unit,
                                PlantShutDownJob = maintenanceTask.PlantShutDownJob,
                                AtexZone = maintenanceTask.AtexZone,
                                AuxiliaryMaterial = maintenanceTask.AuxiliaryMaterial,
                                GreaseOilManufacturer = maintenanceTask.GreaseOilManufacturer,
                                GreaseOilType = maintenanceTask.GreaseOilType,
                                TopupAmount = maintenanceTask.TopupAmount,
                                RoutineType = maintenanceTask.RoutineType.ToString(),
                            };
                            Dto.Add(newdto);
                        }
                    }
                    return Json(new { data = Dto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { draw = Request["draw"] }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region WorkOrder
        
        [HttpPost]
        public ActionResult GetWorkOrderDataTableList(int isOpen = -1, int isClose = -1, string date = "",
            string type = "", string author = "", string department = "", string OpenClose = "", int tag = -1)
        {
            List<WorkOrder> wlist = new List<WorkOrder>();
            if (gp.ValidateLoggedinUser().Role == 1 || gp.ValidateLoggedinUser().Role == 2)
            {
                wlist = new WorkOrderBL().GetActiveWorkOrdersList(de).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                wlist = new WorkOrderBL().GetActiveWorkOrdersList(de).Where(x => x.UserId == gp.ValidateLoggedinUser().Id).OrderByDescending(x => x.Id).ToList();
            }

            if(isClose == 1)
            {
                wlist = wlist.Where(x => x.IsClosed == 1).ToList();
            }

            if(isOpen == 2)
            {
                wlist = wlist.Where(x => x.IsClosed != 1).ToList();
            }

            if(date != "")
            {
                wlist = wlist.Where(x => Convert.ToDateTime(x.OrderDateTime).Date == Convert.ToDateTime(date).Date).ToList();
            }
            if (type != "")
            {
                wlist = wlist.Where(x => x.OrderType == type).ToList();
            }
            if (author != "")
            {
                wlist = wlist.Where(x => x.UserId!= null && ( x.User.FName.ToLower().Contains(author.ToLower()) || x.User.LName.ToLower().Contains(author.ToLower()))).ToList();
            }
            if (department != "")
            {
                wlist = wlist.Where(x => x.DepartmentId != null && x.Department.Name.ToLower().Contains(department.ToLower())).ToList();
            }
            
            if(OpenClose != "")
            {
                if(OpenClose == "Opened")
                {
                    wlist = wlist.Where(x => x.IsClosed != 1).ToList();
                }
                if (OpenClose == "Closed")
                {
                    wlist = wlist.Where(x => x.IsClosed == 1).ToList();
                }
            }
            if(tag != -1)
            {
                wlist = wlist.Where(x => x.TagsId == tag).ToList();
            }

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];
            int Count = 0;
            int TotalCount = 0;
            if (sortColumnName != "" && sortColumnName != null)
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        if (sortColumnName == "User")
                        {
                            wlist = wlist.OrderByDescending(x => x.User.FName).ToList();
                        }
                        else if(sortColumnName == "EquipmentCodeId")
                        {
                            wlist = wlist.OrderByDescending(x => x.MaintenanceTask.EquipmentCode.Code).ToList();
                        }
                        else if(sortColumnName == "MaintenanceTaskId")
                        {
                            wlist = wlist.OrderByDescending(x => x.MaintenanceTask.TaskDescription).ToList();
                        }
                        else
                        {
                            wlist = wlist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                        }
                    }
                    else
                    {
                        if (sortColumnName == "User")
                        {
                            wlist = wlist.OrderBy(x => x.User.FName).ToList();
                        }
                        else if (sortColumnName == "EquipmentCodeId")
                        {
                            wlist = wlist.OrderBy(x => x.MaintenanceTask.EquipmentCode.Code).ToList();
                        }
                        else if (sortColumnName == "MaintenanceTaskId")
                        {
                            wlist = wlist.OrderBy(x => x.MaintenanceTask.TaskDescription).ToList();
                        }
                        else
                        {
                            wlist = wlist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                        }
                        TotalCount = wlist.Count();
                    }
                }
            }

            int totalrows = wlist.Count();

            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                wlist = wlist.Where(x => x.OrderDateTime.ToString().Contains(searchValue.ToLower()) ||
                                    x.Id.ToString().Contains(searchValue.ToLower()) ||
                                    x.OrderType != null && x.OrderType.ToLower().Contains(searchValue.ToLower()) ||
                                    x.User.FName.ToLower().Contains(searchValue.ToLower()) ||
                                    x.User.LName.ToLower().Contains(searchValue.ToLower()) ||
                                    x.MaintenanceTaskId != null && x.MaintenanceTask.EquipmentCodeId != null && x.MaintenanceTask.EquipmentCode.Code.ToLower().Contains(searchValue.ToLower())
                                    ).ToList();
            }

            int totalrowsafterfilterinig = wlist.Count();


            // pagination
            wlist = wlist.Skip(start).Take(length).ToList();

            List<WorkOrderDTO> wdto = new List<WorkOrderDTO>();
            string OrderType = "", equipcode = "";
            foreach (WorkOrder w in wlist)
            {
                if(sortDirection == "asc")
                {
                    Count = Count+1;
                }
                else
                {
                    Count = TotalCount--;
                }
                if (w.TypeId != null)
                {
                    OrderType = w.Type.TypeName;
                }
                else
                {
                    OrderType = "";
                }
                if (w.MaintenanceTaskId != null)
                {
                    if (w.MaintenanceTask.EquipmentCodeId != null)
                    {
                        equipcode = w.MaintenanceTask.EquipmentCode.Code;
                    }
                    else
                    {
                        equipcode = "";
                    }
                }
                else
                {
                    equipcode = "";
                }
                WorkOrderDTO obj = new WorkOrderDTO()
                {
                    Id = w.Id,
                    Count = Count,
                    EncId = GeneralPurpose.EncryptId(w.Id),
                    FollowupParentId = w.FollowupParentId,
                    FollowupParentName = w.FollowupParentId != null? w.WorkOrder2.MaintenanceTask.MaintenanceType : null,
                    EquipmentCodeId = w.EquipmentCodeId,
                    //Code = w.EquipmentCodeId != null ? w.EquipmentCode.Code: null,
                    Code = equipcode,
                    MaintenanceTaskId = w.MaintenanceTaskId,
                    TaskDescription = w.MaintenanceTaskId != null ? w.MaintenanceTask.TaskDescription : null,
                    Location = w.Location,
                    Category = w.Category,
                    OrderType = OrderType,
                    OrderFor = w.OrderFor,
                    DepartmentId = w.DepartmentId,
                    DepartmentName = w.DepartmentId != null ? w.Department.Name : null,
                    UserId = w.UserId,
                    UserName = w.UserId != null ? w.User.FName + " " + w.User.LName: null,
                    OrderDateTime = w.OrderDateTime.ToString(),
                    OrderDescription = w.OrderDescription,
                    OrderStatus = w.OrderStatus,
                    WorkTime = w.WorkTime.ToString(),
                    File1Path = w.File1Path,
                    File2Path = w.File2Path,
                    File3Path = w.File3Path,
                    File4Path = w.File4Path,
                    File5Path = w.File5Path,
                    IsClosed = (int)w.IsClosed,
                    Role = gp.ValidateLoggedinUser().Role,
                };

                wdto.Add(obj);
            }

            return Json(new { data = wdto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetWorkOrderById(int id)
        {
            WorkOrder w = new WorkOrderBL().GetActiveWorkOrderById(id, de);

            if (w == null)
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

            WorkOrderDTO obj = new WorkOrderDTO()
            {
                Id = w.Id,
                EncId = GeneralPurpose.EncryptId(w.Id),
                FollowupParentId = w.FollowupParentId,
                FollowupParentName = w.FollowupParentId != null ? w.WorkOrder2.MaintenanceTask.MaintenanceType : null,
                EquipmentCodeId = w.EquipmentCodeId,
                Code = w.EquipmentCodeId != null ? w.EquipmentCode.Code : null,
                MaintenanceTaskId = w.MaintenanceTaskId,
                TaskDescription = w.MaintenanceTaskId != null ? w.MaintenanceTask.MaintenanceType : null,
                Location = w.Location,
                Category = w.Category,
                OrderType = w.OrderType,
                OrderFor = w.OrderFor,
                DepartmentId = w.DepartmentId,
                DepartmentName = w.DepartmentId != null ? w.Department.Name : null,
                UserId = w.UserId,
                UserName = w.UserId != null ? w.User.FName + " " + w.User.LName : null,
                OrderDateTime = w.OrderDateTime.ToString(),
                OrderDescription = w.OrderDescription,
                OrderStatus = w.OrderStatus,
                WorkTime = w.WorkTime.ToString(),
                IsClosed = (int)w.IsClosed,
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWorkOrderHistoryDataTableList()
        {
            List<WorkOrderHistory> wlist = new WorkOrderHistoryBL().GetActiveWorkOrderHistorysList(de).OrderByDescending(x => x.Id).ToList();

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null)
            {
                if (sortColumnName != "0")
                {
                    if (sortDirection == "asc")
                    {
                        wlist = wlist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                    else
                    {
                        wlist = wlist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                    }
                }
            }

            int totalrows = wlist.Count();

            //filter
            if (!string.IsNullOrEmpty(searchValue))
            {
                wlist = wlist.Where(x => x.OrderStatus.ToLower().Contains(searchValue.ToLower())
                                    ).ToList();
            }

            int totalrowsafterfilterinig = wlist.Count();


            // pagination
            wlist = wlist.Skip(start).Take(length).ToList();

            List<WorkOrderDTO> wdto = new List<WorkOrderDTO>();

            foreach (WorkOrderHistory w in wlist)
            {
                WorkOrderDTO obj = new WorkOrderDTO()
                {
                    Id = w.Id,
                    EncId = GeneralPurpose.EncryptId(w.Id),
                    CreatedAt = Convert.ToDateTime(w.CreatedAt).ToString("MM/dd/yyyy"),
                    OrderStatus = w.OrderStatus,
                    EquipmentCodeId = w.EquipmentCodeId,
                    Location = w.WorkOrder.Location,
                    TaskDescription = w.WorkOrder.MaintenanceTaskId != null ? w.WorkOrder.MaintenanceTask.MaintenanceType : null,
                    File1Path = w.WorkOrder.File1Path,
                    File2Path = w.WorkOrder.File2Path,
                    File3Path = w.WorkOrder.File3Path,
                    File4Path = w.WorkOrder.File4Path,
                    File5Path = w.WorkOrder.File5Path,
                };

                wdto.Add(obj);
            }

            return Json(new { data = wdto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region WorkOrder_Graph
        [ValidationFilter(CheckLogin = false)]
        [HttpPost]
        public ActionResult GetWorkOrder(string StartDate = "", string EndDate = "", int Order = -1)
        {
            List<WorkOrder> MainList = new List<WorkOrder>();
            MainList = new WorkOrderBL().GetActiveWorkOrdersList(de).ToList();

            if (StartDate != "")
            {
                MainList = MainList.Where(x => Convert.ToDateTime(x.CreatedAt) >= Convert.ToDateTime(StartDate)).ToList();
            }

            if (EndDate != "")
            {
                MainList = MainList.Where(x => Convert.ToDateTime(x.CreatedAt) >= Convert.ToDateTime(EndDate)).ToList();
            }

            int Closed = MainList.Where(x => x.IsClosed == 1).ToList().Count();
            int Opened = MainList.Where(x => x.IsClosed != 1).ToList().Count();

            WorkOrderDTO obj = new WorkOrderDTO()
            {
                IsOpen = Opened,
                IsClosed = Closed,
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [ValidationFilter(CheckLogin = false)]
        [HttpPost]
        public ActionResult GetWorkOrderByUser()
        {
            List<WorkOrder> MainList = new List<WorkOrder>();
            MainList = new WorkOrderBL().GetActiveWorkOrdersList(de).ToList();

            List<WorkOrderDTO> Dto = new List<WorkOrderDTO>();
            string author = "";
            int Closed = 0, Opened = 0;
            List<User> u = new List<User>();
            u = new UserBL().GetActiveUsersList(de).ToList();
            foreach (User x in u)
            {
                Opened = MainList.Where(v => v.UserId == x.Id && v.IsClosed != 1).Count();
                Closed = MainList.Where(v => v.UserId == x.Id && v.IsClosed == 1).Count();
                author = x.FName + " " + x.LName;

                WorkOrderDTO obj = new WorkOrderDTO()
                {
                    OrderFor = author,
                    IsClosed = Closed,
                    IsOpen = Opened
                };
                Dto.Add(obj);
            }

            return Json(Dto, JsonRequestBehavior.AllowGet);
        }

        [ValidationFilter(CheckLogin = false)]
        [HttpPost]
        public ActionResult GetRepairAnalytics(string StartDate = "", string EndDate = "", int Order = -1)
        {
            List<WorkOrder> MainList = new List<WorkOrder>();
            List<WorkOrder> ClosedList = new List<WorkOrder>();
            List<WorkOrder> OpenedList = new List<WorkOrder>();
            MainList = new WorkOrderBL().GetActiveWorkOrdersList(de).Where(x => x.Category == "Repair").ToList();

            if (StartDate != "")
            {
                MainList = MainList.Where(x => Convert.ToDateTime(x.CreatedAt) >= Convert.ToDateTime(StartDate)).ToList();
            }

            if (EndDate != "")
            {
                MainList = MainList.Where(x => Convert.ToDateTime(x.CreatedAt) >= Convert.ToDateTime(EndDate)).ToList();
            }

            ClosedList = MainList.Where(x => x.IsClosed == 1).ToList();
            OpenedList = MainList.Where(x => x.IsClosed != 1).ToList();
            MainList = MainList.ToList();

            int Closed = MainList.Where(x => x.IsClosed == 1).Count();
            int Opened = MainList.Where(x => x.IsClosed != 1).Count();

            List<WorkOrderDTO> close = new List<WorkOrderDTO>();
            List<WorkOrderDTO> open = new List<WorkOrderDTO>();

            WorkOrderDTO obj = new WorkOrderDTO()
            {
                IsOpen = Opened,
                IsClosed = Closed,
            };

            //foreach (WorkOrder order in MainList)
            //{
            //    if (order.IsClosed == 1)
            //    {
            //        WorkOrderDTO Closeds = new WorkOrderDTO()
            //        {
            //            EquipmentCodeId = order.EquipmentCodeId,
            //            MaintenanceTaskId = order.MaintenanceTaskId,
            //            IsClosed = (int)order.IsClosed,
            //        };
            //        close.Add(Closeds);
            //    }
            //    else
            //    {
            //        WorkOrderDTO Openeds = new WorkOrderDTO()
            //        {
            //            EquipmentCodeId = order.EquipmentCodeId,
            //            MaintenanceTaskId = order.MaintenanceTaskId,
            //            IsOpen = (int)order.IsClosed,
            //        };
            //        open.Add(Openeds);
            //    }
            //    var Result = new { Result1 = close, Result2 = open };
            //    return Json(Result, JsonRequestBehavior.AllowGet);
            //}

            return Json(obj, JsonRequestBehavior.AllowGet);
            //return Json(JsonRequestBehavior.AllowGet);


        }
        #endregion
    }
}