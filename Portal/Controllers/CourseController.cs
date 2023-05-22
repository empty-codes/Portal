using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
            int semester = 2;
            int userStudyLevel = GetUserStudyLevel();

            foreach (var row in selectedRows)
            {
                if (row.LevelTaken == userStudyLevel)
                {
                    if (row.SemesterNo == semester || row.SemesterNo == 0)
                    {
                        // Check if the data already exists in the database for the given matricNo
                        bool dataExists = db.SelectedCoursesTables.Any(x => x.CourseId == row.CourseId &&
                           x.CourseTitle == row.CourseTitle &&
                           x.CreditHours == row.CreditHours &&
                           x.LevelTaken == row.LevelTaken &&
                           x.Lecturer == row.Lecturer &&
                           x.Programme == row.Programme &&
                           x.MatricNo == User.Identity.Name &&
                           x.SemesterNo == row.SemesterNo);

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
                                MatricNo = User.Identity.Name,
                                SemesterNo = row.SemesterNo
                            });
                        }
                    }
                    else if (row.SemesterNo != semester && row.SemesterNo != 0)
                    {
                        continue;
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


        [Authorize]
        public ActionResult Results()
        {
            string MatricNo = User.Identity.Name;
            var student = db.StudentTables.FirstOrDefault(s => s.MatricNo == MatricNo);
            ViewBag.Student = student;

            List<SelectedCoursesTable> scc = db.SelectedCoursesTables
                    .Where(d => d.MatricNo == MatricNo)
                    .ToList();
            foreach (var c in scc) {
                if (c.Score == null)
                {
                    string errorMessage = "Your results have not been released";
                    ViewBag.ErrorMessage = errorMessage;
                    return RedirectToAction("SelectedCourses");
                }
                else 
                {
                    CalculateGPA();
                }
            }
            return View();
            
        }

        [Authorize]
        public ActionResult SemesterRegistration()
        {
            return View();
        }

        [Authorize]
        public ActionResult ResultSlip()
        {
            string MatricNo = User.Identity.Name;
            var student = db.StudentTables.FirstOrDefault(s => s.MatricNo == MatricNo);
            ViewBag.Student = student;
            CalculateGPA();
            return View();
        }

        [Authorize]
        public ActionResult CourseForm()
        {
            string MatricNo = User.Identity.Name;
            var student = db.StudentTables.FirstOrDefault(s => s.MatricNo == MatricNo);
            if (student != null)
            {
                ViewBag.Student = student;
            }
            return View();
        }


        public ActionResult CalculateGPA()
        {
            string matricNumber = User.Identity.Name;
            // Get all the selected courses for the student
            try
            {
                List<SelectedCoursesTable> selectedCourses = db.SelectedCoursesTables
                    .Where(s => s.MatricNo == matricNumber)
                    .ToList();

                    // Calculate the GPA
                    decimal gpSum = 0;
                    int? creditHourSum = 0;

                    foreach (var course in selectedCourses)
                    {

                        // Assign grade based on the score
                        string grade = GetGradeFromScore((int)course.Score);
                        course.Grade = grade;

                        // Calculate GP based on grade and credit hours
                        decimal point = GetPointFromGrade(grade);
                        decimal gp = (decimal)(point * course.CreditHours);
                        course.GP = (int?)gp;

                        // Update the database with the calculated values
                        db.Entry(course).State = EntityState.Modified;

                        // Accumulate the GP and credit hours
                        gpSum += gp;
                        creditHourSum += course.CreditHours;


                   
                    }

                    // Save the changes to the database
                    db.SaveChanges();

                    // Calculate the GPA
                    decimal gpa = (decimal)(gpSum / creditHourSum);

                    // Update the student's GPA in the database
                    StudentTable student = db.StudentTables.FirstOrDefault(s => s.MatricNo == matricNumber);
                    if (student != null)
                    {
                        student.SemesterGPA = Math.Round(gpa, 5);
                        db.Entry(student).State = EntityState.Modified;
                        db.SaveChanges();
                    }


                    return RedirectToAction("Results", "Course");
                

            }
            catch (DbEntityValidationException ex)
            {
                // Handle validation errors
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                string errorMessage = string.Join("\n", errorMessages);
                ViewBag.ErrorMessage = errorMessage;
                return View("Error");
            }
        }
    

    private string GetGradeFromScore(int score)
    {
        if (score >= 80 && score <= 100)
            return "A";
        else if (score >= 60 && score < 80)
            return "B";
        else if (score >= 50 && score < 60)
            return "C";
        else if (score >= 45 && score < 50)
            return "D";
        else if (score >= 40 && score < 45)
            return "E";
        else
            return "F";
    }

    private decimal GetPointFromGrade(string grade)
    {
        switch (grade)
        {
            case "A":
                return 5;
            case "B":
                return 4;
            case "C":
                return 3;
            case "D":
                return 2;
            case "E":
                return 1;
            default:
                return 0;
        }
    }
}
    }


