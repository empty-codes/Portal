using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class StudentController : Controller
    {
        PortalDBEntities db = new PortalDBEntities();

        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(StudentTable signup)
        {
            //Validation
            if (ModelState.IsValid)
            {
                //check if password == confirm password 
                if (signup.Password != signup.ConfirmPassword)
                {
                    ViewBag.Error = "Password does not match";
                    return View(signup);
                }
                else
                {
                    //check if matric number already exists
                    var userCheck = db.StudentTables.Where(x => x.MatricNo == signup.MatricNo).FirstOrDefault();

                    if (userCheck != null)
                    {
                        ViewBag.Error = "Student already registered";
                        return View(signup);
                    }
                    else
                    {
                        //Process form
                        //submit form
                        db.StudentTables.Add(signup);
                        try
                        {
                            db.SaveChanges();
                            return RedirectToAction("Dashboard", signup);
                        }
                        catch
                        {
                            ViewBag.Error = "Registration Unsuccessful.";
                            return View(signup);
                        }
                    }

                    return View();
                }
            }
            else
            {
                return View(signup);
            }
            
        }



        public ActionResult Login()
        {
            return View();
        }
    }
}