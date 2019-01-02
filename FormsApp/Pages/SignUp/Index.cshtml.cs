using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FormsApp.Models;

namespace FormsApp.Pages.SignUp
{
    public class IndexModel : PageModel
    {
        private readonly FormsApp.Models.FormsAppContext _context;

        public IndexModel(FormsApp.Models.FormsAppContext context)
        {
            _context = context;
        }

        public string TitleSort { get; set; }
        public string MaxStudentsSort { get; set; }
        public string CurrentStudentsSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public Stage SelectedStage { get; set; }
        public readonly int DefaultEntriesPerPage = 25;
        public int EntriesPerPage { get; set; }

        public PaginatedList<CourseVM> CourseVM { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex, int? entriesPerPage, Stage selectedStage)
        {
            SelectedStage = selectedStage;
            CurrentSort = sortOrder;
            TitleSort = string.IsNullOrEmpty(sortOrder) ? "title_d" : "";
            MaxStudentsSort = sortOrder == "max_students" ? "max_students_d" : "max_students";
            CurrentStudentsSort = sortOrder == "current_students" ? "current_students_d" : "current_students";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<CourseVM> courseIQ = _context.Course.Select(c => new CourseVM {
                CourseId = c.CourseId,
                Title = c.Title,
                Stage = c.Stage,
                MaxStudents = c.MaxStudents,
                Trainer = c.Trainer,
                CurrentStudents = c.StudentCourses.Count
            });

            if (!string.IsNullOrEmpty(CurrentFilter))
            {
                courseIQ = courseIQ.Where(c => c.Title.Contains(CurrentFilter));
            }

            courseIQ = courseIQ.Where(c => c.Stage == SelectedStage);

            switch (sortOrder)
            {
                case "title_d":
                    courseIQ = courseIQ.OrderByDescending(c => c.Title);
                    break;
                case "max_students":
                    courseIQ = courseIQ.OrderBy(c => c.MaxStudents);
                    break;
                case "max_students_d":
                    courseIQ = courseIQ.OrderByDescending(c => c.MaxStudents);
                    break;
                case "current_students":
                    courseIQ = courseIQ.OrderBy(c => c.CurrentStudents);
                    break;
                case "current_students_d":
                    courseIQ = courseIQ.OrderByDescending(c => c.CurrentStudents);
                    break;
                default:
                    courseIQ = courseIQ.OrderBy(c => c.Title);
                    break;
            }

            EntriesPerPage = entriesPerPage ?? DefaultEntriesPerPage;
            CourseVM = await PaginatedList<CourseVM>.CreateAsync(
                courseIQ.AsNoTracking(), pageIndex ?? 1, EntriesPerPage);
        }
    }
}
