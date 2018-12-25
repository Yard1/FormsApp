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
    public class IndexModel : PageModel
    {
        private readonly FormsApp.Models.FormsAppContext _context;

        public IndexModel(FormsApp.Models.FormsAppContext context)
        {
            _context = context;
        }

        public IList<Course> Course { get;set; }

        public async Task OnGetAsync()
        {
            Course = await _context.Course.ToListAsync();
        }
    }
}
