using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shiftbook.Models;
using Shiftbook.BL;
using Shiftbook.Helping_Classes;
using System.IO;

namespace Shiftbook.Controllers
{
    [ValidationFilter(Role = 1)]
    public class AdminController : Controller
    {
        private readonly GeneralPurpose gp = new GeneralPurpose();
        private readonly DatabaseEntities de = new DatabaseEntities();
        [ValidationFilter(Role =0)]
        public ActionResult Index(string msg = "", string color = "black", string way = "")
        {
            ViewBag.DepartmentCount = new DepartmentBL().GetActiveDepartmentsList(de).Count();
            ViewBag.UserCount = new UserBL().GetActiveUsersList(de).Where(x => x.IsPrimary != 1).Count();
            ViewBag.LoggedInUser = gp.ValidateLoggedinUser().Role;
            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }
        
        #region Manage User

        public ActionResult AddUser(string msg = "", string color = "black", string way = "")
        {
            ViewBag.Departments = new DepartmentBL().GetActiveDepartmentsList(de);

            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        [HttpPost]
        public ActionResult PostAddUser(User _user, string way = "")
        {
            if(gp.ValidateLoggedinUser().IsPrimary != 1 && _user.Role == 1)
            {
                return RedirectToAction("AddUser", "Admin", new { msg = "Not allowed", color = "red", way = way });
            }

            bool checkEmail = gp.ValidateEmail(_user.Email);

            if (checkEmail == false)
            {
                return RedirectToAction("AddUser", "Admin", new { msg = "Email already exist, Please try another", color = "red", way = way });
            }

            User obj = new User()
            {
                DepartmentId = _user.DepartmentId,
                FName = _user.FName.Trim(),
                LName = _user.LName.Trim(),
                UserName = _user.UserName,
                Language = _user.Language,
                Contact = _user.Contact,
                Email = _user.Email.Trim(),
                Password = StringCipher.Encrypt(_user.Password.Trim()),
                Author = _user.Author,
                Role = _user.Role,
                IsPrimary = 0,
                IsActive = 1,
                CreatedAt = GeneralPurpose.DateTimeNow()
            };

            bool chkUser = new UserBL().AddUser(obj, de);

            if (chkUser)
            {
                return RedirectToAction("AddUser", "Admin", new { msg = "User inserted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("AddUser", "Admin", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        public ActionResult ViewUser(string msg = "", string color = "black", string way = "")
        {
            ViewBag.Departments = new DepartmentBL().GetActiveDepartmentsList(de);

            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        [HttpPost]
        public ActionResult PostUpdateUser(User _user, string way = "")
        {
            bool checkEmail = gp.ValidateEmail(_user.Email, _user.Id);
            if (checkEmail == false)
            {
                return RedirectToAction("ViewUser", "Admin", new { msg = "Email already exist, Please try another", color = "red", way = way });
            }

            User u = new UserBL().GetActiveUserById(_user.Id, de);
            u.DepartmentId = _user.DepartmentId;
            u.FName = _user.FName.Trim();
            u.LName = _user.LName.Trim();
            u.UserName = _user.UserName;
            u.Language = _user.Language;
            u.Contact = _user.Contact;
            u.Email = _user.Email;
            u.Password = StringCipher.Encrypt(_user.Password.Trim());
            u.Author = _user.Author;

            bool chkUser = new UserBL().UpdateUser(u, de);

            if (chkUser)
            {
                return RedirectToAction("ViewUser", "Admin", new { msg = "User updated successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewUser", "Admin", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        public ActionResult DeleteUser(int id, string way = "")
        {
            User u = new UserBL().GetActiveUserById(id, de);
            if(u == null)
            {
                return RedirectToAction("ViewUser", "Admin", new { msg = "Record not found", color = "red", way = way });
            }
            u.IsActive = 0;

            bool chkUser = new UserBL().UpdateUser(u, de);

            if (chkUser)
            {
                return RedirectToAction("ViewUser", "Admin", new { msg = "Record deleted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewUser", "Admin", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        #endregion

        #region Manage Department

        [ValidationFilter(Role = 12)]
        public ActionResult AddDepartment(string msg = "", string color = "black", string way = "")
        {
            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        [ValidationFilter(Role = 12)]
        [HttpPost]
        public ActionResult PostAddDepartment(Department _department, string way = "")
        {
            bool chkDep = gp.ValidateDepartment(_department.Name);
            if (chkDep == false)
            {
                return RedirectToAction("AddDepartment", new { msg = "Name already exist, Please try another", color = "red", way = way });
            }

            Department obj = new Department()
            {
                Name = _department.Name.Trim(),
                Location = _department.Location,
                IsActive = 1,
                CreatedAt = GeneralPurpose.DateTimeNow()
            };

            bool chkUser = new DepartmentBL().AddDepartment(obj, de);

            if (chkUser)
            {
                return RedirectToAction("AddDepartment", new { msg = "Department inserted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("AddDepartment", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        [ValidationFilter(Role = 12)]
        public ActionResult ViewDepartments(string msg = "", string color = "black", string way = "")
        {
            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        [ValidationFilter(Role = 12)]
        [HttpPost]
        public ActionResult PostUpdateDepartment(Department _department, string way = "")
        {
            bool chkDep = gp.ValidateDepartment(_department.Name, _department.Id);
            if (chkDep == false)
            {
                return RedirectToAction("ViewDepartments", new { msg = "Name already exist, Please try another", color = "red", way = way });
            }

            Department d = new DepartmentBL().GetActiveDepartmentById(_department.Id, de);
            d.Name = _department.Name.Trim();
            d.Location = _department.Location;

            bool chkUser = new DepartmentBL().UpdateDepartment(d, de);

            if (chkUser)
            {
                return RedirectToAction("ViewDepartments", new { msg = "Department updated successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewDepartments", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        [ValidationFilter(Role = 12)]
        public ActionResult DeleteDepartment(int id, string way = "")
        {
            Department d = new DepartmentBL().GetActiveDepartmentById(id, de);
            if(d == null)
            {
                return RedirectToAction("ViewDepartments", new { msg = "Record not found", color = "red", way = way });
            }
            d.IsActive = 0;

            bool chkdep = new DepartmentBL().UpdateDepartment(d, de);
            if (chkdep)
            {
                return RedirectToAction("ViewDepartments", new { msg = "Record deleted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewDepartments", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }
        #endregion

        #region Manage EquipmentCode
        
        [ValidationFilter(Role = 12)]
        public ActionResult AddEquipmentCode(string msg = "", string color = "black", string way = "")
        {
            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        [ValidationFilter(Role = 12)]
        [HttpPost]
        public ActionResult PostAddEquipmentCode(EquipmentCode _equipment, string way = "")
        {
            bool chkDep = gp.ValidateEquipmentCode(_equipment.Code);
            if (chkDep == false)
            {
                return RedirectToAction("AddEquipmentCode", new { msg = "Code already exist, Please try another", color = "red", way = way });
            }

            EquipmentCode obj = new EquipmentCode()
            {
                Code = _equipment.Code.Trim(),
                EquipmentName = _equipment.EquipmentName,
                Description = _equipment.Description,
                IsActive = 1,
                CreatedAt = GeneralPurpose.DateTimeNow()
            };

            bool chkCode = new EquipmentCodeBL().AddEquipmentCode(obj, de);

            if (chkCode)
            {
                return RedirectToAction("AddEquipmentCode", new { msg = "Code inserted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("AddEquipmentCode", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        [ValidationFilter(Role = 12)]
        public ActionResult ViewEquipmentCode(string msg = "", string color = "black", string way = "")
        {
            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        [ValidationFilter(Role = 12)]
        [HttpPost]
        public ActionResult PostUpdateEquipmentCode(EquipmentCode _equipment, string way = "")
        {
            bool chkCode = gp.ValidateEquipmentCode(_equipment.Code, _equipment.Id);
            if (chkCode == false)
            {
                return RedirectToAction("ViewEquipmentCode", new { msg = "Code already exist, Please try another", color = "red", way = way });
            }

            EquipmentCode eq = new EquipmentCodeBL().GetActiveEquipmentCodeById(_equipment.Id, de);
            eq.Code = _equipment.Code.Trim();
            eq.EquipmentName = _equipment.EquipmentName;
            eq.Description = _equipment.Description;

            bool chkCode2 = new EquipmentCodeBL().UpdateEquipmentCode(eq, de);

            if (chkCode2)
            {
                return RedirectToAction("ViewEquipmentCode", new { msg = "Code updated successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewEquipmentCode", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        [ValidationFilter(Role = 12)]
        public ActionResult DeleteEquipmentCode(int id, string way = "")
        {
            EquipmentCode eq = new EquipmentCodeBL().GetActiveEquipmentCodeById(id, de);
            if(eq == null)
            {
                return RedirectToAction("ViewEquipmentCode", new { msg = "Record not found", color = "red", way = way });
            }
            eq.IsActive = 0;

            bool chkCode = new EquipmentCodeBL().UpdateEquipmentCode(eq, de);
            if (chkCode)
            {
                return RedirectToAction("ViewEquipmentCode", new { msg = "Record deleted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewEquipmentCode", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        [ValidationFilter(Role = 12)]
        [HttpPost]
        public ActionResult DeleteMultipleEquipmentCode(int[] ids)
        {
            try
            {
                foreach (int id in ids)
                {
                    EquipmentCode eq = new EquipmentCodeBL().GetActiveEquipmentCodeById(id, de);
                    eq.IsActive = 0;

                    bool chkCode = new EquipmentCodeBL().UpdateEquipmentCode(eq, de);
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidationFilter(Role = 12)]
        public ActionResult CodeHistory(string Id = "")
        {
            var Ids = GeneralPurpose.DecryptId(Id);
            EquipmentCode Code = new EquipmentCodeBL().GetActiveEquipmentCodeById(Ids, de);
            ViewBag.History = Code;
            return View();
        }
        #endregion

        #region Manage MaintenanceTask
        
        [ValidationFilter(Role = 12)]
        public ActionResult AddMaintenanceTask(string msg = "", string color = "black", string way = "")
        {
            ViewBag.CodeList = new EquipmentCodeBL().GetActiveEquipmentCodesList(de);
            ViewBag.TagsList = new TagsBL().GetActiveTagsList(de);
            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        [ValidationFilter(Role = 12)]
        [HttpPost]
        public ActionResult PostAddMaintenanceTask(MaintenanceTask _task, int[] TagsId, string way = "")
        {
            int count = new MaintenanceTaskBL().GetActiveMaintenanceTasksList(de).Where(x => x.TaskDescription.ToLower() == _task.TaskDescription.ToLower()).Count();
            if(count >= 1)
            {
                return RedirectToAction("AddMaintenanceTask", new { msg = "Maintainance Task of this Name is Already Registered", color = "red", way = way });
            }

            MaintenanceTask obj = new MaintenanceTask()
            {
                EquipmentCodeId = _task.EquipmentCodeId,
                AssetDescription = _task.AssetDescription,
                Pid = _task.Pid,
                SystemDescription = _task.SystemDescription,
                MaintenanceType = _task.MaintenanceType,
                TaskDescription = _task.TaskDescription,
                Comment = _task.Comment,
                Interval = _task.Interval,
                Unit = _task.Unit,
                PlantShutDownJob = _task.PlantShutDownJob,
                AtexZone = _task.AtexZone,
                AuxiliaryMaterial = _task.AuxiliaryMaterial,
                GreaseOilManufacturer = _task.GreaseOilManufacturer,
                GreaseOilType = _task.GreaseOilType,
                TopupAmount = _task.TopupAmount,
                RoutineType = _task.RoutineType,
                IsActive = 1,
                CreatedAt = GeneralPurpose.DateTimeNow()
            };

            bool chkTask = new MaintenanceTaskBL().AddMaintenanceTask(obj, de);

            if (chkTask)
            {
                if(TagsId.Count() >= 1)
                {
                    foreach(var x in TagsId)
                    {
                        TagsTask tt = new TagsTask()
                        {
                            TaskId = obj.Id,
                            TagsId = x,
                            CreatedAt = GeneralPurpose.DateTimeNow(),
                            IsActive = 1,
                        };
                        if (!new TagsTaskBL().AddTagsTask(tt, de))
                        {
                            return RedirectToAction("AddMaintenanceTask", new { msg = "Something is wrong", color = "red", way = way });
                        }
                    }
                }
                return RedirectToAction("AddMaintenanceTask", new { msg = "Task inserted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("AddMaintenanceTask", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        [ValidationFilter(Role = 12)]
        public ActionResult ViewMaintenanceTask(string msg = "", string color = "black", string way = "",
            string today = "", string Week = "", string Month = "", string Period = "")
        {
            ViewBag.CodeList = new EquipmentCodeBL().GetActiveEquipmentCodesList(de);
            ViewBag.TagsList = new TagsBL().GetActiveTagsList(de);

            List<Tag> tags = new TagsBL().GetActiveTagsList(de).ToList();
            ViewBag.tags = tags;

            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            ViewBag.today = today;
            ViewBag.Week = Week;
            ViewBag.Month = Month;
            ViewBag.Period = Period;

            return View();
        }

        [ValidationFilter(Role = 12)]
        public ActionResult UpdateMaintenanceTask(string Id = "", string msg = "", string color = "black", string way = "")
        {
            var Ids = GeneralPurpose.DecryptId(Id);
            MaintenanceTask task = new MaintenanceTaskBL().GetMaintenanceTaskById(Convert.ToInt32(Ids), de);
            ViewBag.CodeList = new EquipmentCodeBL().GetActiveEquipmentCodesList(de);
            ViewBag.TagsList = new TagsBL().GetActiveTagsList(de);
            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View(task);
        }

        [ValidationFilter(Role = 12)]
        [HttpPost]
        public ActionResult PostUpdateMaintenanceTask(MaintenanceTask _task, int[] TagsId, string way = "")
        {
            List<TagsTask> tagsTasks = new TagsTaskBL().GetActiveTagsTaskList(de).Where(x => x.TaskId == _task.Id).ToList();
            List<int> list = new List<int>();

            MaintenanceTask t = new MaintenanceTaskBL().GetActiveMaintenanceTaskById(_task.Id, de);
            int Count = new MaintenanceTaskBL().GetActiveMaintenanceTasksList(de).Where(x => x.Id != t.Id && x.TaskDescription == _task.TaskDescription).Count();
            if(Count > 1)
            {
                return RedirectToAction("ViewMaintenanceTask", new { msg = "Maintainance Task of this Name is Already Registered", color = "red", way = way });
            }
            t.EquipmentCodeId = _task.EquipmentCodeId;
            t.AssetDescription = _task.AssetDescription;
            t.Pid = _task.Pid;
            t.SystemDescription = _task.SystemDescription;
            t.MaintenanceType = _task.MaintenanceType;
            t.TaskDescription = _task.TaskDescription;
            t.Comment = _task.Comment;
            t.Interval = _task.Interval;
            t.Unit = _task.Unit;
            t.PlantShutDownJob = _task.PlantShutDownJob;
            t.AtexZone = _task.AtexZone;
            t.AuxiliaryMaterial = _task.AuxiliaryMaterial;
            t.GreaseOilManufacturer = _task.GreaseOilManufacturer;
            t.GreaseOilType = _task.GreaseOilType;
            t.TopupAmount = _task.TopupAmount;
            t.RoutineType = _task.RoutineType;

            bool chkTask = new MaintenanceTaskBL().UpdateMaintenanceTask(t, de);

            if (chkTask)
            {
                if(TagsId.Count() >= 1)
                {
                    foreach (var del in tagsTasks)
                    {
                        if (!new TagsTaskBL().DeleteTagTask(del.Id, de))
                        {
                            return RedirectToAction("ViewMaintenanceTask", new { msg = "Something is Worng", color = "red", way = way });
                        }
                    }

                    foreach (var item in TagsId)
                    {
                        TagsTask tagsTask = new TagsTask()
                        {
                            TagsId = item,
                            TaskId = _task.Id,
                            IsActive = 1,
                            CreatedAt = GeneralPurpose.DateTimeNow()
                        };
                        if (!new TagsTaskBL().AddTagsTask(tagsTask, de))
                        {
                            return RedirectToAction("ViewMaintenanceTask", new { msg = "Something is Worng", color = "red", way = way });
                        }
                    }
                }
                return RedirectToAction("ViewMaintenanceTask", new { msg = "Task updated successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewMaintenanceTask", new { msg = "Something is wrong", color = "red", way = way });
            }
        }

        [ValidationFilter(Role = 12)]
        public ActionResult DeleteMaintenanceTask(int id, string way = "")
        {
            MaintenanceTask t = new MaintenanceTaskBL().GetActiveMaintenanceTaskById(id, de);
            if(t == null)
            {
                return RedirectToAction("ViewMaintenanceTask", new { msg = "Record not found", color = "red", way = way });
            }
            t.IsActive = 0;

            bool chkTask = new MaintenanceTaskBL().UpdateMaintenanceTask(t, de);
            if (chkTask)
            {
                return RedirectToAction("ViewMaintenanceTask", new { msg = "Record deleted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewMaintenanceTask", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        [ValidationFilter(Role = 12)]
        [HttpPost]
        public ActionResult DeleteMultipleMaintenanceTask(int[] ids)
        {
            try
            {
                foreach (int id in ids)
                {
                    MaintenanceTask t = new MaintenanceTaskBL().GetActiveMaintenanceTaskById(id, de);
                    t.IsActive = 0;

                    bool chkTask = new MaintenanceTaskBL().UpdateMaintenanceTask(t, de);
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [ValidationFilter(Role = 12)]
        public ActionResult ImportMaintenanceTask(string msg = "", string color = "black", string way = "")
        {
            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        #endregion

        #region Manage WorkOrder

        [ValidationFilter(Role = 12)]
        public ActionResult AddWorkOrder(string msg = "", string color = "black", string way = "", int TagId = -1, string Ids = "", string TagName = "")
        {
            Dictionary<int, string> Tasks = new MaintenanceTaskBL().GetActiveMaintenanceTasksList(de).Select(x => new { x.Id, x.TaskDescription }).ToDictionary(x => x.Id, x => x.TaskDescription);

            ViewBag.Tasks = Tasks;
            ViewBag.CodeList = new EquipmentCodeBL().GetActiveEquipmentCodesList(de).Select(x=> new { x.Id, x.Code}).ToDictionary(x=> x.Id, x=> x.Code);
            List<Models.Type> types = new TypeBL().GetActiveTypesList(de).OrderByDescending(x => x.TypeName).ToList();
            List<Tag> tags = new TagsBL().GetActiveTagsList(de).OrderBy(x => x.TagName).ToList();
            ViewBag.tags = tags;
            ViewBag.TypeList = types;
            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;
            ViewBag.TagId = TagId;
            ViewBag.Id = Ids;
            ViewBag.TagName = TagName;

            return View();
        }

        [ValidationFilter(Role = 12)]
        [HttpPost]
        public ActionResult PostAddWorkOrder(WorkOrder _order, int author, string way = "", string ids = "", string TagName = "")
        {
            int Id = -1;
            if (ids != "" && TagName != null)
            {
                string[] SplidId = ids.Split(',');
                Tag tag = new Tag()
                {
                    TagName = TagName,
                    IsActive = 1,
                    CreatedAt = GeneralPurpose.DateTimeNow(),
                };
                if (!new TagsBL().AddTag(tag, de))
                {
                    return RedirectToAction("AddWorkOrder", new { msg = "Something is Wrong", color = "red", Ids = ids, TagName = TagName });
                }

                foreach (string id in SplidId)
                {
                    TagsTask task = new TagsTask()
                    {
                        TagsId = tag.Id,
                        TaskId = Convert.ToInt32(id),
                        IsActive = 1,
                        CreatedAt = GeneralPurpose.DateTimeNow()
                    };
                    if (!new TagsTaskBL().AddTagsTask(task, de))
                    {
                        return RedirectToAction("AddWorkOrder", new { msg = "Something is Wrong", color = "red", Ids = ids, TagName = TagName });
                    }
                }
                Id = tag.Id;
            }
            if(Id != -1)
            {
                _order.TagsId = Id;
            }
            else
            {
                _order.TagsId = _order.TagsId;
            }

            if (_order.OrderFor == "Department")
            {
                List<int> userIds = new UserBL().GetActiveUsersListByDep(author, de).Select(x => x.Id).ToList();

                string msg = "";
                foreach(int userId in userIds)
                {
                    WorkOrder obj = new WorkOrder()
                    {
                        FollowupParentId = _order.FollowupParentId,
                        EquipmentCodeId = _order.EquipmentCodeId,
                        MaintenanceTaskId = _order.MaintenanceTaskId,
                        Location = _order.Location,
                        Category = _order.Category,
                        OrderType = _order.OrderType,
                        OrderFor = _order.OrderFor,
                        UserId = userId,
                        DepartmentId = author,
                        OrderDateTime = _order.OrderDateTime,
                        OrderDescription = _order.OrderDescription,
                        IsClosed = 0,
                        IsActive = 1,
                        TypeId = _order.TypeId,
                        TagsId = _order.TagsId,
                        CreatedAt = GeneralPurpose.DateTimeNow()
                    };


                    HttpFileCollectionBase files = Request.Files;
                    int TotalFiles = Request.Files.Count;
                    for (int k = 0; k < TotalFiles; k++)
                    {
                        if (k == 5)
                        {
                            break;
                        }

                        HttpPostedFileBase PostedFile = files[k];

                        string fileName = Path.GetFileNameWithoutExtension(PostedFile.FileName);
                        string fileExt = Path.GetExtension(PostedFile.FileName);

                        string FileName = fileName + DateTime.Now.Ticks.ToString() + fileExt;
                        if (PostedFile.ContentLength != 0)
                        {
                            switch (k)
                            {
                                case 0:
                                    obj.File1Path = "../Content/OrderFiles/" + FileName;
                                    break;
                                case 1:
                                    obj.File2Path = "../Content/OrderFiles/" + FileName;
                                    break;
                                case 2:
                                    obj.File3Path = "../Content/OrderFiles/" + FileName;
                                    break;
                                case 3:
                                    obj.File4Path = "../Content/OrderFiles/" + FileName;
                                    break;
                                case 4:
                                    obj.File5Path = "../Content/OrderFiles/" + FileName;
                                    break;
                            }

                            FileName = Path.Combine(Server.MapPath("../Content/OrderFiles/"), FileName);

                            PostedFile.SaveAs(FileName);
                        }
                    }

                    bool chkOrder = new WorkOrderBL().AddWorkOrder(obj, de);
                    if(!chkOrder)
                    {
                        msg = "Work-order assigned with errors";
                    }
                }

                if(msg == "")
                {
                    return RedirectToAction("AddWorkOrder", new { msg = "Work order assigned successfully", color = "green", way = way });
                }

                return RedirectToAction("AddWorkOrder", new { msg = msg, color = "red", way = way, Ids = ids, TagName = TagName });
            }
            else if (_order.OrderFor == "User")
            {
                User u = new UserBL().GetActiveUserById(author, de);
                WorkOrder obj = new WorkOrder()
                {
                    FollowupParentId = _order.FollowupParentId,
                    EquipmentCodeId = _order.EquipmentCodeId,
                    MaintenanceTaskId = _order.MaintenanceTaskId,
                    Location = _order.Location,
                    Category = _order.Category,
                    OrderType = _order.OrderType,
                    OrderFor = _order.OrderFor,
                    UserId = u.Id,
                    DepartmentId = u.DepartmentId,
                    OrderDateTime = _order.OrderDateTime,
                    OrderDescription = _order.OrderDescription,
                    TypeId = _order.TypeId,
                    TagsId = _order.TagsId,
                    IsClosed = 0,
                    IsActive = 1,
                    CreatedAt = GeneralPurpose.DateTimeNow()
                };


                HttpFileCollectionBase files = Request.Files;
                int TotalFiles = Request.Files.Count;
                for (int k = 0; k < TotalFiles; k++)
                {
                    if (k == 5)
                    {
                        break;
                    }

                    HttpPostedFileBase PostedFile = files[k];
                    if(files[k].ContentLength == 0 || files[k].FileName == "")
                    {
                        continue;
                    }

                    string fileName = Path.GetFileNameWithoutExtension(PostedFile.FileName);
                    string fileExt = Path.GetExtension(PostedFile.FileName);

                    string FileName = fileName + DateTime.Now.Ticks.ToString() + fileExt;

                    if (PostedFile.ContentLength != 0)
                    {
                        switch (k)
                        {
                            case 0:
                                obj.File1Path = "../Content/OrderFiles/" + FileName;
                                break;
                            case 1:
                                obj.File2Path = "../Content/OrderFiles/" + FileName;
                                break;
                            case 2:
                                obj.File3Path = "../Content/OrderFiles/" + FileName;
                                break;
                            case 3:
                                obj.File4Path = "../Content/OrderFiles/" + FileName;
                                break;
                            case 4:
                                obj.File5Path = "../Content/OrderFiles/" + FileName;
                                break;
                        }

                        FileName = Path.Combine(Server.MapPath("../Content/OrderFiles/"), FileName);

                        PostedFile.SaveAs(FileName);
                    }
                }

                bool chkOrder = new WorkOrderBL().AddWorkOrder(obj, de);
                if(chkOrder)
                {
                    return RedirectToAction("AddWorkOrder", new { msg = "Work order assigned successfully", color = "green", way = way });
                }
            }
           
            return RedirectToAction("AddWorkOrder", new { msg = "Somethings' wrong", color = "red", way = way, Ids = ids, TagName = TagName });
        }

        [ValidationFilter(Role = 0)]
        public ActionResult ViewWorkOrders(int isClose=-1, string msg = "", string color = "black", string way = "")
        {
            List<Tag> tags = new TagsBL().GetActiveTagsList(de).ToList();
            ViewBag.tags = tags;

            ViewBag.IsClose = isClose;

            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        [ValidationFilter(Role = 12)]
        public ActionResult UpdateWorkOrder(string eId, string msg = "", string color = "black", string way = "")
        {
            WorkOrder w = new WorkOrderBL().GetActiveWorkOrderById(GeneralPurpose.DecryptId(eId), de);
            if (w == null)
            {
                return RedirectToAction("ViewWorkOrders", new { msg = "Record not found!", color = "red", way = way });
            }
            ViewBag.WorkOrder= w;
            List<Tag> tags = new TagsBL().GetActiveTagsList(de).OrderBy(x => x.TagName).ToList();
            ViewBag.tags = tags;

            ViewBag.CodeList = new EquipmentCodeBL().GetActiveEquipmentCodesList(de).Select(x=> new { x.Id, x.Code}).ToDictionary(x=> x.Id, x=> x.Code);
            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }

        [ValidationFilter(Role = 12)]
        [HttpPost]
        public ActionResult PostUpdateWorkOrder(WorkOrder _order, int fileCount, string way = "")
        {
            WorkOrder w = new WorkOrderBL().GetActiveWorkOrderById(_order.Id, de);
            
            if (w == null)
            {
                return RedirectToAction("ViewWorkOrders", new { msg = "Record not found!", color = "red", way = way });
            }
            
            User u = new UserBL().GetActiveUserById((int)_order.UserId, de);
            if(_order.OrderStatus == "Completed Work Order")
            {
                w.IsClosed = 1; // closed
            }
            w.OrderStatus = _order.OrderStatus;
            w.EquipmentCodeId = _order.EquipmentCodeId;
            w.OrderDescription = _order.OrderDescription;
            w.UserId = u.Id;
            w.DepartmentId = u.DepartmentId;
            w.WorkTime = _order.WorkTime;
            w.OrderDateTime = _order.OrderDateTime;
            w.TagsId = _order.TagsId;

            HttpFileCollectionBase files = Request.Files;
            int TotalFiles = Request.Files.Count;
            int loopEnd = 5 - fileCount;
            for (int k = 0; k < loopEnd; k++)
            {
                try
                {
                    if (k == 5)
                    {
                        break;
                    }

                    HttpPostedFileBase PostedFile = files[k];
                    if (files[k].ContentLength == 0 || files[k].FileName == "")
                    {
                        continue;
                    }
                

                    string fileName = Path.GetFileNameWithoutExtension(PostedFile.FileName);
                    string fileExt = Path.GetExtension(PostedFile.FileName);

                    string FileName = fileName + DateTime.Now.Ticks.ToString() + fileExt;

                    if(String.IsNullOrEmpty(w.File1Path))
                    {
                        w.File1Path = "../Content/OrderFiles/" + FileName;
                    }
                    else if (String.IsNullOrEmpty(w.File2Path))
                    {
                        w.File2Path = "../Content/OrderFiles/" + FileName;
                    }
                    else if (String.IsNullOrEmpty(w.File3Path))
                    {
                        w.File3Path = "../Content/OrderFiles/" + FileName;
                    }
                    else if (String.IsNullOrEmpty(w.File4Path))
                    {
                        w.File4Path = "../Content/OrderFiles/" + FileName;
                    }
                    else if (String.IsNullOrEmpty(w.File5Path))
                    {
                        w.File5Path = "../Content/OrderFiles/" + FileName;
                    }

                    FileName = Path.Combine(Server.MapPath("../Content/OrderFiles/"), FileName);

                    PostedFile.SaveAs(FileName);
                }
                catch
                {
                    continue;
                }
            }

            bool chkOrder = new WorkOrderBL().UpdateWorkOrder(w, de);

            if (chkOrder)
            {
                return RedirectToAction("ViewWorkOrders", new { msg = "Work order updated successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewWorkOrders", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }


        [ValidationFilter(Role = 0)]
        public ActionResult DeleteWorkOrder(int id, string way = "")
        {
            WorkOrder w = new WorkOrderBL().GetActiveWorkOrderById(id, de);
            
            if(w== null)
            {
                if(way == "3")
                {
                    return RedirectToAction("Index", new { msg = "Record not found", color = "red", way = way });
                }
                return RedirectToAction("ViewWorkOrders", new { msg = "Record not found", color = "red", way = way });
            }
            
            w.IsActive = 0;

            bool chkOrder = new WorkOrderBL().UpdateWorkOrder(w, de);
            
            if (chkOrder)
            {
                if(way == "3")
                {
                    return RedirectToAction("Index", new { msg = "Record deleted successfully", color = "green", way = way });
                }
                return RedirectToAction("ViewWorkOrders", new { msg = "Record deleted successfully", color = "green", way = way });
            }
            else
            {
                if(way == "3")
                {
                    return RedirectToAction("Index", new { msg = "Something is wrong", color = "red", way = way });
                }
                return RedirectToAction("ViewWorkOrders", new { msg = "Somethings' wrong", color = "red", way = way });
            }
        }

        [ValidationFilter(Role = 0)]
        public ActionResult RemoveWorkOrderFile(int id, int fileNumber, string way = "")
        {
            WorkOrder w = new WorkOrderBL().GetActiveWorkOrderById(id, de);

            if(w == null)
            {
                return RedirectToAction("ViewWorkOrders", new { msg = "Record not found!", color = "red", way = way });
            }

            switch (fileNumber)
            {
                case 0:
                    RemoveServerFile(w.File1Path);
                    w.File1Path = null;
                    break;
                case 1:
                    RemoveServerFile(w.File2Path);
                    w.File2Path = null;
                    break;
                case 2:
                    RemoveServerFile(w.File3Path);
                    w.File3Path = null;
                    break;
                case 3:
                    RemoveServerFile(w.File4Path);
                    w.File4Path = null;
                    break;
                case 4:
                    RemoveServerFile(w.File5Path);
                    w.File5Path = null;
                    break;
            }

            bool chkOrder = new WorkOrderBL().UpdateWorkOrder(w, de);

            if (chkOrder)
            {
                return RedirectToAction("UpdateWorkOrder", new { eId = GeneralPurpose.EncryptId(w.Id), msg = "File deleted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("UpdateWorkOrder", new { eId = GeneralPurpose.EncryptId(w.Id), msg = "Somethings' wrong", color = "red", way = way });
            }
        }
        
        [ValidationFilter(Role = 0)]
        [Route("Details")]
        public ActionResult Details(string Id = "")
        {
            WorkOrder w = new WorkOrderBL().GetActiveWorkOrderById(GeneralPurpose.DecryptId(Id), de);
            ViewBag.WorkOrder = w;
            ViewBag.CodeList = new EquipmentCodeBL().GetActiveEquipmentCodesList(de).Select(x => new { x.Id, x.Code }).ToDictionary(x => x.Id, x => x.Code);
            List<TagsTask> tagsTasks = new TagsTaskBL().GetActiveTagsTaskList(de).Where(y => y.TagsId == w.TagsId).ToList();
            ViewBag.tagList = tagsTasks;
            return View();
        }

        [ValidationFilter(Role = 12)]
        [Route("Analytics")]
        public ActionResult Analytics()
        {
            return View();
        }
        #endregion

        [ValidationFilter(Role = 0)]
        public bool RemoveServerFile(string path)
        {
            path = Server.MapPath(path);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                try
                {
                    file.Delete();

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        #region Tagging/Location
        [ValidationFilter(Role = 12)]
        public ActionResult TaggingMultipleMaintenanceTask(string ids = "", string TagName = "")
        {
            return RedirectToAction("AddWorkOrder", new { Ids = ids, TagName = TagName });
        }
        #endregion
    }
}