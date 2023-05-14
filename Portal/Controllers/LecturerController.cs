using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class LecturerController : Controller
    {

        PortalDBEntities db = new PortalDBEntities();
        // GET: Lecturer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LecturerTable login)
        {
            if (login.Email is null || login.Password is null)
            {
                ViewBag.Error = "Fill all fields";
                return View(login);
            }
            else
            {
                //check db for matric no
                var userCheck = db.LecturerTables.Where(x => x.Email.ToLower() == login.Email.ToLower()).FirstOrDefault();
                if (userCheck is null)
                {
                    ViewBag.Error = "Email not found.";
                    return View(login);
                }
                else if (userCheck.Password != login.Password)
                {
                    ViewBag.Error = "Wrong Password.";
                    return View(login);
                }
                else
                {
                    return RedirectToAction("Dashboard", userCheck);
                }
            }
            return View();
        }

        public ActionResult Dashboard(LecturerTable lecturer)
        {
            ViewBag.Lecturer = lecturer;
            return View();
        }

    }
}