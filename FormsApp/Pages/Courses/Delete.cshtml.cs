using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FormsApp.Models;

namespace FormsApp.Pages.Courses
{
    public class DeleteModel : PageModel
    {
        private readonly FormsApp.Models.FormsAppContext _context;

        public DeleteModel(FormsApp.Models.FormsAppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student).Include(dp => dp.DatesAndPlaces).AsNoTracking().FirstOrDefaultAsync(m => m.CourseId == id);

            if (Course == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ErrorMessage = "Delete failed. Try again";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Course
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            try
            {
                _context.Course.Remove(course);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                return RedirectToAction("./Delete",
                    new { id, saveChangesError = true });
            }
        }
    }
}
