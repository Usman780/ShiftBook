using Shiftbook.BL;
using Shiftbook.Helping_Classes;
using Shiftbook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shiftbook.Controllers
{
    [ValidationFilter(Role = 12)]
    public class TypeController : Controller
    {
        DatabaseEntities db = new DatabaseEntities();
        GeneralPurpose gp = new GeneralPurpose();
        MailSender ms = new MailSender();
        private bool isLogedIn()
        {
            if (gp.ValidateLoggedinUser() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [Route("ViewTypes")]
        // GET: Type
        public ActionResult ViewTypes(string message = "", string color = "")
        {
            if (color == "red")
            {
                ViewBag.errormessage = message;
                ViewBag.color = color;
            }
            if (color == "green")
            {
                ViewBag.successfullmessage = message;
                ViewBag.color = color;
            }
            return View();
        }

        [HttpPost]
        public ActionResult GetTypesList()
        {
            List<Models.Type> list = new List<Models.Type>();
            if (gp.ValidateLoggedinUser().Role == 1 && gp.ValidateLoggedinUser().IsPrimary == 1)
            {
                list = new TypeBL().GetActiveTypesList(db).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                list = new TypeBL().GetActiveTypesList(db).Where(x => x.CreatedBy == gp.ValidateLoggedinUser().Id).OrderByDescending(x => x.Id).ToList();
            }

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null)
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

            if (!string.IsNullOrEmpty(searchValue))
            {
                list = list.Where(x => x.TypeName.ToLower().Contains(searchValue.ToLower())
                        ).ToList();
            }
            int totalrows = list.Count();
            int totalrowsafterfilterinig = list.Count();
            list = list.Skip(start).Take(length).ToList();

            List<DTO> udto = new List<DTO>();
            foreach (Models.Type u in list)
            {
                DTO obj = new DTO()
                {
                    TypeId = u.Id,
                    TypeEncId = GeneralPurpose.EncryptId(u.Id),
                    TypeName = u.TypeName,
                    Role = (int)gp.ValidateLoggedinUser().Role
                    
                };

                udto.Add(obj);
            }
            return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostAddNewType(Models.Type _Type, int PageId = -1)
        {
            if ((!isLogedIn()))
            {
                return RedirectToAction("Login", "Auth");
            }
            if (_Type.TypeName == null)
            {
                if (PageId == 2)
                {
                    return RedirectToAction("NewInventory", "Inventory", new { message = "Fill all The <strong>(*)</strong> Fields", color = "red" });
                }
                else
                {
                    return RedirectToAction("ViewTypes", new { message = "Fill all The <strong>(*)</strong> Fields", color = "red" });
                }
            }
            int count = new TypeBL().GetActiveTypesList(db).Where(x => x.IsActive == 1 && x.TypeName == _Type.TypeName).Count();
            if (count > 0)
            {
                if (PageId == 2)
                {
                    return RedirectToAction("NewInventory", "Inventory", new { message = "This Type Name is Already Registered", color = "red" });
                }
                else
                {
                    return RedirectToAction("ViewTypes", new { message = "This Type Name is Already Registered", color = "red" });
                }
            }

            Models.Type type = new Models.Type()
            {
                TypeName = _Type.TypeName,
                IsActive = 1,
                CreatedAt = GeneralPurpose.DateTimeNow(),
                CreatedBy = gp.ValidateLoggedinUser().Id
            };
            if (new TypeBL().AddType(type, db))
            {
                if (PageId == 2)
                {
                    return RedirectToAction("NewInventory", "Inventory", new { message = "Record is Added Successfully", color = "green" });
                }
                else
                {
                    return RedirectToAction("ViewTypes", new { message = "Record is Added Successfully", color = "green" });
                }
            }
            if (PageId == 2)
            {
                return RedirectToAction("NewInventory", "Inventory", new { message = "Record connot be added to database", color = "red" });
            }
            else
            {
                return RedirectToAction("ViewTypes", new { message = "Record connot be added to database", color = "red" });
            }
        }

        [HttpPost]
        public ActionResult TypeById(int Id = -1)
        {
            Models.Type _Type = new TypeBL().GetActiveTypeById(Id, db);
            if (_Type != null)
            {
                Models.Type sST = new Models.Type()
                {
                    Id = _Type.Id,
                    TypeName = _Type.TypeName,
                };
                return Json(sST, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostUpdate(Models.Type _Type)
        {
            int count = new TypeBL().GetActiveTypesList(db).Where(x => x.TypeName == _Type.TypeName && x.Id != _Type.Id).Count();
            if (count > 0)
            {
                return RedirectToAction("ViewTypes", new { message = "This Option is Already Registered", color = "red" });
            }
            Models.Type obj = new TypeBL().GetActiveTypeById(_Type.Id, db);

            obj.TypeName = _Type.TypeName;

            if (new TypeBL().UpdateType(obj, db))
            {
                return RedirectToAction("ViewTypes", new { message = "Record is Updated Successfully", color = "green" });
            }

            return RedirectToAction("ViewTypes", new { message = "Something is Wrong", color = "red" });
        }

        public ActionResult Delete(int id, string way = "")
        {
            Models.Type u = new TypeBL().GetActiveTypeById(id, db);
            if (u == null)
            {
                return RedirectToAction("ViewTypes", new { msg = "Record not found", color = "red", way = way });
            }

            if (new TypeBL().DeleteType(id, db))
            {
                return RedirectToAction("ViewTypes", new { msg = "Record deleted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewTypes", new { msg = "Something is wrong", color = "red", way = way });
            }
        }
    }
}