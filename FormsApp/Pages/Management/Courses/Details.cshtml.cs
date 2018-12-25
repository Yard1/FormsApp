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
    public class DetailsModel : PageModel
    {
        private readonly FormsApp.Models.FormsAppContext _context;

        public DetailsModel(FormsApp.Models.FormsAppContext context)
        {
            _context = context;
        }

        public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
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
            return Page();
        }
    }
}
