using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FormsApp.Models;

namespace FormsApp.Pages.Students
{
    public class DeleteModel : PageModel
    {
        private readonly FormsApp.Models.FormsAppContext _context;

        public DeleteModel(FormsApp.Models.FormsAppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Student Student { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            Student = await _context.Student.Include(sc => sc.StudentCourses).ThenInclude(c => c.Course).AsNoTracking().FirstOrDefaultAsync(m => m.StudentId == id);

            if (Student == null)
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

            var Student = await _context.Student
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.StudentId == id);

            if (Student == null)
            {
                return NotFound();
            }

            try
            {
                _context.Student.Remove(Student);
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
