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


            //// Prepare the data to be returned to the client-side
            //var courseData = selectedCourses.Select(course => new
            //{
            //    MatricNumber = course.MatricNumber,
            //    CourseId = course.CourseId,
            //    CourseTitle = course.CourseTitle
            //});

            //// Return the data as JSON
            //return Json(new { data = courseData }, JsonRequestBehavior.AllowGet);
            //foreach (var row in selectedRows)
            //{

            //    List<SelectedCoursesTable> scList = db.SelectedCoursesTables
            //    .Where(sc => sc.CourseId == row.CourseId && sc.Lecturer == row.Lecturer)
            //    .ToList();

            //    List<string> matricNumbers =scList.Select(sc => sc.MatricNo).ToList();
            //    List<string> courseIds = scList.Select(sc => sc.CourseId).ToList();
            //    List<string> courseTitles = scList.Select(sc => sc.CourseTitle).ToList();

            //    ViewBag.MatricNumbers = matricNumbers;
            //    ViewBag.CourseIds = courseIds;
            //    ViewBag.CourseTitles = courseTitles;

            //}

            //return Json(new { success = true });
            //return View();
        }

        public ActionResult MyStudents()
        {
            return View();
        }
    }
}