using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Shiftbook.Models;
using Shiftbook.Helping_Classes;
using Shiftbook.BL;

namespace Shiftbook.Controllers
{
    [ValidationFilter(Role = 0)]
    public class HomeController : Controller
    {
        private GeneralPurpose gp = new GeneralPurpose();
        private DatabaseEntities de = new DatabaseEntities();

        public ActionResult Index(string msg = "", string color = "black", string way = "")
        {
            List<WorkOrder> wlist = new WorkOrderBL().GetActiveWorkOrdersList(de).Where(x => x.UserId == gp.ValidateLoggedinUser().Id).ToList();
            ViewBag.MyOrderCount = wlist.Count();
            ViewBag.ClosedOrderCount = wlist.Where(x=> x.IsClosed == 1).Count();

            ViewBag.Message = msg;
            ViewBag.Color = color;
            ViewBag.Way = way;

            return View();
        }
    }
}