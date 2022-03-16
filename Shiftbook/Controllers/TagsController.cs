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
    public class TagsController : Controller
    {
        DatabaseEntities db = new DatabaseEntities();
        GeneralPurpose gp = new GeneralPurpose();
        MailSender ms = new MailSender();
        // GET: Task

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

        [Route("ViewTags")]
        // GET: Type
        public ActionResult ViewTags(string message = "", string color = "")
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
        public ActionResult GetTagList()
        {
            List<Tag> list = new List<Tag>();
            
            list = new TagsBL().GetActiveTagsList(db).OrderByDescending(x => x.Id).ToList();
            

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
                list = list.Where(x => x.TagName.ToLower().Contains(searchValue.ToLower())
                        ).ToList();
            }
            int totalrows = list.Count();
            int totalrowsafterfilterinig = list.Count();
            list = list.Skip(start).Take(length).ToList();

            List<DTO> udto = new List<DTO>();
            foreach (Tag u in list)
            {
                DTO obj = new DTO()
                {
                    TagId = u.Id,
                    TagEncId = GeneralPurpose.EncryptId(u.Id),
                    TagName = u.TagName,
                    Role = (int)gp.ValidateLoggedinUser().Role

                };

                udto.Add(obj);
            }
            return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostAddNewTag(Tag _tag, int PageId = -1)
        {
            if ((!isLogedIn()))
            {
                return RedirectToAction("Login", "Auth");
            }
            if (_tag.TagName == null)
            {
                if (PageId == 2)
                {
                    return RedirectToAction("NewInventory", "Inventory", new { message = "Fill all The <strong>(*)</strong> Fields", color = "red" });
                }
                else
                {
                    return RedirectToAction("ViewTags", new { message = "Fill all The <strong>(*)</strong> Fields", color = "red" });
                }
            }
            int count = new TagsBL().GetActiveTagsList(db).Where(x => x.TagName == _tag.TagName).Count();
            if (count > 0)
            {
                if (PageId == 2)
                {
                    return RedirectToAction("NewInventory", "Inventory", new { message = "This Name is Already Registered", color = "red" });
                }
                else
                {
                    return RedirectToAction("ViewTags", new { message = "This Name is Already Registered", color = "red" });
                }
            }

            Tag tag = new Tag()
            {
                TagName = _tag.TagName,
                IsActive = 1,
                CreatedAt = GeneralPurpose.DateTimeNow(),
            };
            if (new TagsBL().AddTag(tag, db))
            {
                if (PageId == 2)
                {
                    return RedirectToAction("NewInventory", "Inventory", new { message = "Record is Added Successfully", color = "green" });
                }
                else
                {
                    return RedirectToAction("ViewTags", new { message = "Record is Added Successfully", color = "green" });
                }
            }
            if (PageId == 2)
            {
                return RedirectToAction("NewInventory", "Inventory", new { message = "Record connot be added to database", color = "red" });
            }
            else
            {
                return RedirectToAction("ViewTags", new { message = "Record connot be added to database", color = "red" });
            }
        }

        [HttpPost]
        public ActionResult TagById(int Id = -1)
        {
            Tag _tag = new TagsBL().GetActiveTagById(Id, db);
            if (_tag != null)
            {
                Tag sST = new Tag()
                {
                    Id = _tag.Id,
                    TagName = _tag.TagName,
                };
                return Json(sST, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostUpdate(Tag _tag)
        {
            int count = new TagsBL().GetActiveTagsList(db).Where(x => x.TagName == _tag.TagName && x.Id != _tag.Id).Count();
            if (count > 0)
            {
                return RedirectToAction("ViewTags", new { message = "This Name is Already Registered", color = "red" });
            }
            Tag obj = new TagsBL().GetActiveTagById(_tag.Id, db);

            obj.TagName = _tag.TagName;

            if (new TagsBL().UpdateTag(obj, db))
            {
                return RedirectToAction("ViewTags", new { message = "Record is Updated Successfully", color = "green" });
            }

            return RedirectToAction("ViewTags", new { message = "Something is Wrong", color = "red" });
        }

        public ActionResult Delete(int id, string way = "")
        {
            Tag u = new TagsBL().GetActiveTagById(id, db);
            if (u == null)
            {
                return RedirectToAction("ViewTags", new { msg = "Record not found", color = "red", way = way });
            }

            List<TagsTask> tagsTask = new TagsTaskBL().GetActiveTagsTaskList(db).Where(x => x.TagsId == id).ToList();
            if(tagsTask.Count() >= 1)
            {
                foreach(var del in tagsTask)
                {
                    new TagsTaskBL().DeleteTagTask(del.Id, db);
                }
            }
            if (new TagsBL().DeleteTag(id, db))
            {
                return RedirectToAction("ViewTags", new { msg = "Record deleted successfully", color = "green", way = way });
            }
            else
            {
                return RedirectToAction("ViewTags", new { msg = "Something is wrong", color = "red", way = way });
            }
        }

    }
}