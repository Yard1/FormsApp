using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FormsApp.Models;
using Microsoft.AspNetCore.Http;

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

        public async Task<IActionResult> OnPostDownloadFileAsync(int? id)
        {
            Debug.WriteLine("My debug string here");
            Response.Clear();

            if (Course == null)
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
            }

            var _delimiter = ",";
            var fileName = $"{Course.CourseId}_{Course.Title}_Students_{DateTime.Now}.csv";
            var data = Course.StudentCourses.Select(s => s.Student);
            var str = new StringBuilder();

            //head
            str.Append("StudentId" + _delimiter + "\t");
            str.Append("LastName" + _delimiter + "\t");
            str.Append("FirstName" + _delimiter + "\t");
            str.Append("Email" + _delimiter + "\t");
            str.Append("Faculty" + _delimiter + "\t");
            str.Append("\r\n");

            //data
            foreach (var s in data)
            {
                str.Append(s.StudentId + _delimiter + "\t");
                str.Append(s.LastName + _delimiter + "\t");
                str.Append(s.FirstMidName + _delimiter + "\t");
                str.Append(s.Email + _delimiter + "\t");
                str.Append(s.Faculty + _delimiter + "\t");
                str.Append("\r\n");
            }
            Encoding encode = new UTF8Encoding();
            byte[] bytes = encode.GetBytes(str.ToString());
            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileName,
                Inline = false,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(bytes, "text/csv");
        }
    }
}
