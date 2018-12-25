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
        public StudentVM StudentVM { get; set; }
        public List<DateAndPlaceVM> DatesAndPlaceVms { get; set; }
        public string SaveMessage { get; set; }

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

            DatesAndPlaceVms = Course.DatesAndPlaces.Select(dp => new DateAndPlaceVM(){ Date = dp.Date, Place = dp.Place}).ToList();

            if (Course.DatesAndPlaces.Count > 0)
            {
                DateAndPlaceVM = new DateAndPlaceVM()
                {
                    Date = Course.DatesAndPlaces.Last().Date,
                    Place = Course.DatesAndPlaces.Last().Place
                };
            }
            else
            {
                DateAndPlaceVM = new DateAndPlaceVM()
                {
                    Date = DateTime.Today
                };
            }

            StudentVM = new StudentVM();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int formAction)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student)
                .Include(dp => dp.DatesAndPlaces).FirstOrDefaultAsync(m => m.CourseId == CourseVM.CourseId);

            if (formAction == 0) { 
                Course.Title = CourseVM.Title;
                Course.Stage = CourseVM.Stage;
                Course.MaxStudents = CourseVM.MaxStudents;
                Course.Trainer = CourseVM.Trainer;
             }

        if (formAction == 1)
            {

                var dateAndPlace = new DateAndPlace()
                {
                    Date = DateAndPlaceVM.Date,
                    Place = DateAndPlaceVM.Place,
                    Course = Course,
                    CourseId = Course.CourseId
                };
                Course.DatesAndPlaces.Add(dateAndPlace);
                _context.Add(dateAndPlace);
            }

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
            return await OnGetAsync(Course.CourseId, msg);
        }


        private bool CourseExists(int id)
        {
            return _context.Course.Any(e => e.CourseId == id);
        }
    }
}
