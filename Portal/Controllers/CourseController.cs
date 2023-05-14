using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Portal.Models;

namespace Portal.Controllers
{
    public class CourseController : Controller
    {
        PortalDBEntities db = new PortalDBEntities();
        // GET: Course
        public ActionResult Index(StudentTable student)
        {
            ViewBag.Student = student;
            return View();
        }

        public ActionResult GetData(StudentTable student)
        {
            ViewBag.Student = student;
            List<CourseListTable> cList = db.CourseListTables.ToList<CourseListTable>();
            return Json(new { data = cList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSelectedCourses(StudentTable student, List<SelectedCoursesTable>  selectedRows)
        {
            ViewBag.Student = student;

            foreach (var row in selectedRows)
            {
                db.SelectedCoursesTables.Add(new SelectedCoursesTable
                {
                    CourseId = row.CourseId,
                    CourseTitle = row.CourseTitle,
                    CreditHours = row.CreditHours,
                    LevelTaken = row.LevelTaken,
                    Lecturer = row.Lecturer,
                    Programme = row.Programme
                });
            }

            // Save changes to the database
            db.SaveChanges();
            return Json(new { success = true });
        }

        
    }
}

 