using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
                var emailCheck = db.LecturerTables.Where(x => x.Email.ToLower() == login.Email.ToLower()).FirstOrDefault();
                if (emailCheck is null)
                {
                    ViewBag.Error = "Email not found.";
                    return View(login);
                }
                else if (emailCheck.Password != login.Password)
                {
                    ViewBag.Error = "Wrong Password.";
                    return View(login);
                }
                else
                {
                    TempData["LoginData"] = emailCheck;
                    //FormsAuthentication.SetAuthCookie(, false);
                    return RedirectToAction("Dashboard", "Lecturer");
                }
            }
            return View();
        }

        
        public ActionResult Dashboard()
        {
            LecturerTable login = (LecturerTable)TempData["LoginData"];
            TempData.Keep("LoginData");
            var lecturer = db.LecturerTables.FirstOrDefault(s => s.FirstName == login.FirstName);
                if (lecturer != null)
                {
                    ViewBag.Lecturer = lecturer;
            }
                return View();
           
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Lecturer");
        }

    }
        
            
    
}

    