using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FormsApp.Models;
using Microsoft.Extensions.Logging;

namespace FormsApp.Pages.Courses
{
    public class EditModel : PageModel
    {
        private readonly FormsApp.Models.FormsAppContext _context;

        public EditModel(FormsApp.Models.FormsAppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CourseVM CourseVM { get; set; }
        [BindProperty]
        public DateAndPlaceVM DateAndPlaceVM { get; set; }
        [BindProperty]
        public List<DateAndPlaceVM> DatesAndPlaceVms { get; set; }
        [BindProperty]
        public int? StudentId { get; set; }
        [BindProperty]
        public List<StudentVM> StudentVms { get; set; }
        public SelectList AvailableStudents { get; set; }
        public string SaveMessage { get; set; }

        public void PopulateDepartmentsDropDownList(FormsAppContext context, int courseId,
            object selectedStudentVM = null)
        {
            var studentsQuery = context.Student.Include(s => s.StudentCourses).Where(s => s.StudentCourses.All(sc => sc.CourseId != courseId))
                .OrderBy(s => s.LastName).ThenBy(s => s.StudentId).Select(s => new StudentVM()
                {
                    StudentId = s.StudentId,
                    Email = s.Email,
                    Faculty = s.Faculty,
                    FirstMidName = s.FirstMidName,
                    LastName = s.LastName,
                    SearchValue = $"{s.FirstMidName} {s.LastName} | {s.Email}"
                });

            AvailableStudents = new SelectList(studentsQuery.AsNoTracking(),
                "StudentId", "SearchValue", selectedStudentVM);
        }

        public async Task<IActionResult> OnGetAsync(int? id, string message = "")
        {
            SaveMessage = message;
            if (id == null)
            {
                return NotFound();
            }

            var Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student).Include(dp => dp.DatesAndPlaces).FirstOrDefaultAsync(m => m.CourseId == id);

            if (Course == null)
            {
                return NotFound();
            }

            CourseVM = new CourseVM
            {
                CourseId = Course.CourseId,
                Title = Course.Title,
                Stage = Course.Stage,
                MaxStudents = Course.MaxStudents,
                Trainer = Course.Trainer,
                CurrentStudents = Course.StudentCourses.Count
            };

            DatesAndPlaceVms = Course.DatesAndPlaces
                .OrderBy(d => d.Date).ThenBy(d => d.DateAndPlaceId)
                .Select(dp => new DateAndPlaceVM()
                {
                    Date = dp.Date,
                    Place = dp.Place,
                    CourseId = dp.CourseId,
                    DateAndPlaceId = dp.DateAndPlaceId
                }).ToList();

            StudentVms = Course.StudentCourses.Select(sc => sc.Student)
                .OrderBy(s => s.StudentId)
                .Select(s => new StudentVM()
                {
                    StudentId = s.StudentId,
                    Email = s.Email,
                    Faculty = s.Faculty,
                    FirstMidName = s.FirstMidName,
                    LastName = s.LastName
                }).ToList();

            PopulateDepartmentsDropDownList(_context, Course.CourseId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? deleteId)
        {
            return Page();
        }

        public async Task<IActionResult> OnPostMainAsync(int? deleteId)
        {
            if (!ModelState.IsValid)
            {
                DateAndPlaceVM = null;
                return await OnGetAsync(CourseVM.CourseId);
            }

            var Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student)
                .Include(dp => dp.DatesAndPlaces).FirstOrDefaultAsync(m => m.CourseId == CourseVM.CourseId);

            Course.Title = CourseVM.Title;
            Course.Stage = CourseVM.Stage;
            Course.MaxStudents = CourseVM.MaxStudents;
            Course.Trainer = CourseVM.Trainer;

            _context.Attach(Course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(Course.CourseId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var msg = "Saved sucessfully.";
            DateAndPlaceVM = null;
            return await OnGetAsync(CourseVM.CourseId, msg);
        }

        public async Task<IActionResult> OnPostAddDaPAsync()
        {
            var Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student)
                .Include(dp => dp.DatesAndPlaces).FirstOrDefaultAsync(m => m.CourseId == CourseVM.CourseId);

            DateAndPlace dpToAdd;

            if (Course.DatesAndPlaces.Count > 0)
            {
                dpToAdd = new DateAndPlace()
                {
                    Date = Course.DatesAndPlaces.Last().Date,
                    Place = Course.DatesAndPlaces.Last().Place,
                    CourseId = Course.CourseId
                };
            }
            else
            {
                dpToAdd = new DateAndPlace()
                {
                    Date = DateTime.Today,
                    CourseId = Course.CourseId
                };
            }

            Course.DatesAndPlaces.Add(dpToAdd);
            _context.Add(dpToAdd);

            _context.Attach(Course).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(Course.CourseId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            DateAndPlaceVM = null;
            var msg = "Added date and place entry sucessfully.";
            return await OnGetAsync(CourseVM.CourseId, msg);
        }

        public async Task<IActionResult> OnPostDeleteDaPAsync(int deleteId)
        {
            //var Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student)
            //    .Include(dp => dp.DatesAndPlaces).FirstOrDefaultAsync(m => m.CourseId == CourseVM.CourseId);

            var dpToRemove = await _context.DateAndPlace.AsNoTracking()
                .FirstOrDefaultAsync(m => m.DateAndPlaceId == deleteId);

            if (dpToRemove == null)
            {
                return NotFound();
            }

            _context.DateAndPlace.Remove(dpToRemove);

            //_context.Attach(Course).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            DateAndPlaceVM = null;
            var msg = "Deleted date and place entry sucessfully.";
            return await OnGetAsync(CourseVM.CourseId, msg);
        }

        public async Task<IActionResult> OnPostEditDaPAsync(int editId)
        {
            //var Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student)
            //    .Include(dp => dp.DatesAndPlaces).FirstOrDefaultAsync(m => m.CourseId == CourseVM.CourseId);

            if (!ModelState.IsValid)
            {
                DateAndPlaceVM = null;
                return await OnGetAsync(CourseVM.CourseId);
            }

            var dpToEdit = await _context.DateAndPlace.FindAsync(editId);

            if (dpToEdit == null)
            {
                return NotFound();
            }

            dpToEdit.Date = DateAndPlaceVM.Date;
            dpToEdit.Place = DateAndPlaceVM.Place;

            _context.Attach(dpToEdit).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DateAndPlaceExists(dpToEdit.DateAndPlaceId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            DateAndPlaceVM = null;
            var msg = "Edited date and place entry sucessfully.";
            return await OnGetAsync(CourseVM.CourseId, msg);
        }
        public async Task<IActionResult> OnPostEditStartDaPAsync(int editId)
        {
            //var Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student)
            //    .Include(dp => dp.DatesAndPlaces).FirstOrDefaultAsync(m => m.CourseId == CourseVM.CourseId);

            var dp = await _context.DateAndPlace.FindAsync(editId);


            if (dp == null)
            {
                return NotFound();
            }

            DateAndPlaceVM = new DateAndPlaceVM()
            {
                Date = dp.Date,
                Place = dp.Place,
                CourseId = dp.CourseId,
                DateAndPlaceId = dp.DateAndPlaceId
            };

            return await OnGetAsync(CourseVM.CourseId, DateAndPlaceVM.DateAndPlaceId.ToString());
        }

        public async Task<IActionResult> OnPostDeleteStudentAsync(int deleteId)
        {
            var Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student)
                .Include(dp => dp.DatesAndPlaces).FirstOrDefaultAsync(m => m.CourseId == CourseVM.CourseId);

            if (Course == null)
            {
                return NotFound();
            }

            var studentCourseToRemove = await _context.StudentCourse
                .FirstOrDefaultAsync(m => m.StudentId == deleteId && m.CourseId == Course.CourseId);

            if (studentCourseToRemove == null)
            {
                return NotFound();
            }

            _context.StudentCourse.Remove(studentCourseToRemove);

            await _context.SaveChangesAsync();
            DateAndPlaceVM = null;
            var msg = "Deleted student entry sucessfully.";
            return await OnGetAsync(CourseVM.CourseId, msg);
        }

        public async Task<IActionResult> OnPostAddStudentAsync(int studentId)
        {
            var Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student)
                .Include(dp => dp.DatesAndPlaces).FirstOrDefaultAsync(m => m.CourseId == CourseVM.CourseId);

            if (Course == null)
            {
                return NotFound();
            }

            var studentToAdd = await _context.Student.FindAsync(studentId);

            if (studentToAdd == null)
            {
                return NotFound();
            }

            StudentCourse studentCourseToAdd = new StudentCourse()
            {
                Student = studentToAdd,
                StudentId = studentToAdd.StudentId,
                Course = Course,
                CourseId = Course.CourseId
            };

            _context.Add(studentCourseToAdd);
            await _context.SaveChangesAsync();
            DateAndPlaceVM = null;
            var msg = "Added student entry sucessfully.";
            return await OnGetAsync(CourseVM.CourseId, msg);
        }

        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }

        private bool DateAndPlaceExists(int id)
        {
            return _context.DateAndPlace.Any(e => e.DateAndPlaceId == id);
        }
    }
}
