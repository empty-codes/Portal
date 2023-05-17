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
        [Authorize]
        public ActionResult Index(StudentTable student)
        {
            ViewBag.Student = student;
            return View();
        }

        [Authorize]
        public ActionResult GetData(StudentTable student)
        {
            ViewBag.Student = student;
            List<CourseListTable> cList = db.CourseListTables.ToList<CourseListTable>();
            return Json(new { data = cList }, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpPost]
        public ActionResult SendSelectedCourses(List<SelectedCoursesTable> selectedRows)
        {
            int userStudyLevel = GetUserStudyLevel();

            foreach (var row in selectedRows)
            {
                if (row.LevelTaken == userStudyLevel)
                {
                    // Check if the data already exists in the database for the given matricNo
                    bool dataExists = db.SelectedCoursesTables.Any(x => x.CourseId == row.CourseId &&
                       x.CourseTitle == row.CourseTitle &&
                       x.CreditHours == row.CreditHours &&
                       x.LevelTaken == row.LevelTaken &&
                       x.Lecturer == row.Lecturer &&
                       x.Programme == row.Programme &&
                       x.MatricNo == User.Identity.Name);

                    if (!dataExists)
                    {
                        // Data does not exist, add it to the database
                        db.SelectedCoursesTables.Add(new SelectedCoursesTable
                        {
                            CourseId = row.CourseId,
                            CourseTitle = row.CourseTitle,
                            CreditHours = row.CreditHours,
                            LevelTaken = row.LevelTaken,
                            Lecturer = row.Lecturer,
                            Programme = row.Programme,
                            MatricNo = User.Identity.Name
                        });
                    }
                }


            }

            db.SaveChanges();


            return Json(new { success = true });
        }


        public int GetUserStudyLevel()
        {
            string MatricNo = User.Identity.Name;
            int userStudyLevel;
            var student = db.StudentTables.FirstOrDefault(s => s.MatricNo == MatricNo);
            if (student != null)
            {
                userStudyLevel = (int)student.StudyLevel;
                return userStudyLevel;
            }
            else
            {
                return 0;
            }


        }

        [Authorize]
        public ActionResult SelectedCourses(StudentTable student)
        {
            ViewBag.Student = student;
            return View();
        }

        [Authorize]
        public ActionResult GetSelectedCourses(StudentTable student)
        {
            ViewBag.Student = student;
            string currentUserName = User.Identity.Name;

            // Retrieve selected courses for the current user
            List<SelectedCoursesTable> cList = db.SelectedCoursesTables
                .Where(c => c.MatricNo == currentUserName)
                .ToList();

            return Json(new { data = cList }, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult DropCourse(List<SelectedCoursesTable> selectedRows)
        {
            string MatricNo = User.Identity.Name;
                foreach (var row in selectedRows)
                {
                    var course = db.SelectedCoursesTables.FirstOrDefault(s => s.MatricNo == MatricNo && s.CourseId == row.CourseId);

                    if (course != null)
                    {
                        db.SelectedCoursesTables.Remove(course);
                    }
                    else
                    {
                        // Return appropriate response if the course is not found
                        return HttpNotFound();
                    }
                }

                // Save the changes to the database
                db.SaveChanges();

                // Return success message or redirect to a desired page
                return Content("Course dropped successfully.");
            }


        }
    }

