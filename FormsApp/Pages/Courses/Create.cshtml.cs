using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FormsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FormsApp.Pages.Courses
{
    public class CreateModel : PageModel
    {
        private readonly FormsApp.Models.FormsAppContext _context;

        public CreateModel(FormsApp.Models.FormsAppContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public CourseVM CourseVM { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var entry = _context.Add(new Course());
            try
            {
                entry.CurrentValues.SetValues(CourseVM);
                await _context.SaveChangesAsync();
                //log.Info($"Added Course {CourseVM.CourseId} {CourseVM.Title} to database");
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                //log.Error($"Exception when adding {CourseVM.CourseId} {CourseVM.Title} in Courses.Create.OnPostAsync", ex);
                return RedirectToAction("./Create");
            }
        }
    }
}