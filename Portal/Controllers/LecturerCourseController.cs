using Portal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal.Controllers
{
    public class LecturerCourseController : Controller
    {
        PortalDBEntities db = new PortalDBEntities();
        // GET: LecturerCourse
        public ActionResult MyCourseList()
        {
            LecturerTable lecturer = (LecturerTable)TempData["LoginData"];
            TempData.Keep("LoginData");
            ViewBag.Lecturer = lecturer;
            return View();
        }


        public ActionResult GetLecturerCourses()
        {
            LecturerTable lecturer = (LecturerTable)TempData["LoginData"];
            TempData.Keep("LoginData");
            //LecturerTable lecturer = (LecturerTable)TempData["LoginData"];
            ViewBag.Lecturer = lecturer;
            
            // Retrieve selected courses for the current user
            List<CourseListTable> mList = db.CourseListTables
                .Where(m => m.Lecturer == lecturer.FullName)
                .ToList();

            return Json(new { data = mList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLecturerStudents()
        {
            LecturerTable lecturer = (LecturerTable)TempData["LoginData"];
            TempData.Keep("LoginData");
            ViewBag.Lecturer = lecturer;


            List<SelectedCoursesTable> slList = db.SelectedCoursesTables
                .Where(sl => sl.Lecturer == lecturer.FullName)
                .ToList();

            return Json(new { data = slList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyStudents()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateScore(string courseId, string matricNumber, string newValue)
        {
            try
            {
                    // Retrieve the record based on the course ID and matric number
                    var score = db.SelectedCoursesTables.FirstOrDefault(c => c.CourseId == courseId && c.MatricNo == matricNumber);
                        score.Score = int.Parse(newValue);
                        db.SaveChanges();

                        return Json(new { success = true });
                
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the update process
                return Json(new { success = false, message = "An error occurred while updating the score." });
            }
        }

    }
}