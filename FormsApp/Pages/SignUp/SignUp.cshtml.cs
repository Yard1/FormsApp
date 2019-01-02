using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FormsApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FormsApp.Pages.SignUp
{
    public class SignUpModel : PageModel
    {
        private readonly FormsApp.Models.FormsAppContext _context;

        public SignUpModel(FormsApp.Models.FormsAppContext context)
        {
            _context = context;
        }

        [BindProperty]
        public StudentVM StudentVM { get; set; }

        public int? Id { get; set; }

        [BindProperty]
        public string CoursePassword { get; set; }

        [BindProperty, EmailAddress]
        public string Email { get; set; }

        public bool State { get; set; }

        public string Message { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string message = "")
        {
            Message = message;

            if (id == null)
            {
                return NotFound();
            }

            var Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student).Include(dp => dp.DatesAndPlaces).FirstOrDefaultAsync(m => m.CourseId == id);

            if (Course == null)
            {
                return NotFound();
            }

            Id = id;

            return Page();
        }

        public async Task<IActionResult> OnPostCheckEmailAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Id = id;

            var student = await _context.Student.Include(s => s.StudentCourses).AsNoTracking().FirstOrDefaultAsync(s => s.Email == Email);

            var Course = await _context.Course.FindAsync(id);
            if (Course == null)
            {
                return NotFound();
            }

            if (!Course.CheckCoursePassword(CoursePassword))
            {
                return await OnGetAsync(id, "Wrong course password.");
            }

            if (student == null)
            {
                State = true;
                return Page();
            }

            if (Course.StudentCourses.Any(sc => sc.StudentId == student.StudentId))
            {
                return Page();
            }

            return await OnGetAsync(id, "Added to course sucessfully!");
        }

        public async Task<IActionResult> OnPostCreateNewAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Id = id;

            if (!ModelState.IsValid)
            {
                return Page();
            }


            var Course = await _context.Course.Include(sc => sc.StudentCourses).ThenInclude(s => s.Student).Include(dp => dp.DatesAndPlaces).FirstOrDefaultAsync(m => m.CourseId == id);

            if (Course == null)
            {
                return NotFound();
            }
            if (!Course.CheckCoursePassword(CoursePassword))
            {
                return await OnGetAsync(id, "Wrong course password.");
            }

            var student = new Student();
            var entry = _context.Add(student);
            try
            {
                entry.CurrentValues.SetValues(StudentVM);
                await _context.SaveChangesAsync();
                
                var sc = new StudentCourse
                {
                    Course = Course,
                    Student = student
                };

                _context.StudentCourse.Add(sc);
                await _context.SaveChangesAsync();

                return await OnGetAsync(id, "Added to course sucessfully!");
            }
            catch (DbUpdateException ex)
            {
                return await OnGetAsync(id, "Error when adding to course. Please try again.");
            }
        }
    }
}