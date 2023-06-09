﻿using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(StudentTable login)
        {
            if(login.MatricNo is null || login.Password is null)
            {
                ViewBag.Error = "Fill all fields";
                return View(login);
            }
            else
            {
                //check db for matric no
                var userCheck = db.StudentTables.Where(x => x.MatricNo == login.MatricNo).FirstOrDefault();
                if(userCheck is null)
                {
                    ViewBag.Error = "Matric Number not found.";
                    return View(login);
                }
                else if (userCheck.Password != login.Password)
                {
                    ViewBag.Error = "Wrong matric number or password.";
                    return View(login);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(login.MatricNo, false);
                    return RedirectToAction("Dashboard", "Student");
                }
            }
            return View();
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            string MatricNo = User.Identity.Name;
            var student = db.StudentTables.FirstOrDefault(s => s.MatricNo == MatricNo);
            if (student != null)
            {
                ViewBag.Student = student;
            }
            return View();
        }

        [Authorize]
        public ActionResult Course(StudentTable student)
        {
            ViewBag.Student = student;
            return RedirectToAction("Index", "Course");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Student");
        }

        [Authorize]
        public ActionResult StudentDetails()
        { 
            string MatricNo = User.Identity.Name;
            var student = db.StudentTables.FirstOrDefault(s => s.MatricNo == MatricNo);
            if (student != null)
            {
                ViewBag.Student = student;
            }
            return View();
        }

    }
}