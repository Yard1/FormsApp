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

        public enum SearchType { Title, MaxStudents, Stage, Trainer }

        public string TitleSort { get; set; }
        public string MaxStudentsSort { get; set; }
        public string StageSort { get; set; }
        public string TrainerSort { get; set; }
        public string CurrentStudentsSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public SearchType CurrentSearchType { get; set; }
        public readonly int DefaultEntriesPerPage = 25;
        public int EntriesPerPage { get; set; }

        public PaginatedList<CourseVM> CourseVM { get;set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex, int? entriesPerPage, SearchType searchType)
        {
            CurrentSort = sortOrder;
            CurrentSearchType = searchType;
            TitleSort = string.IsNullOrEmpty(sortOrder) ? "title_d" : "";
            MaxStudentsSort = sortOrder == "max_students" ? "max_students_d" : "max_students";
            StageSort = sortOrder == "stage" ? "stage_d" : "stage";
            TrainerSort = sortOrder == "trainer" ? "trainer_d" : "trainer";
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
                switch (CurrentSearchType)
                {
                    case SearchType.MaxStudents:
                        courseIQ = courseIQ.Where(c => c.MaxStudents.ToString().Contains(CurrentFilter));
                        break;
                    case SearchType.Stage:
                        courseIQ = courseIQ.Where(c => c.Stage.ToString().Contains(CurrentFilter));
                        break;
                    case SearchType.Trainer:
                        courseIQ = courseIQ.Where(c => c.Trainer.Contains(CurrentFilter));
                        break;
                    default:
                        courseIQ = courseIQ.Where(c => c.Title.Contains(CurrentFilter));
                        break;
                }
            }

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
                case "stage":
                    courseIQ = courseIQ.OrderBy(c => c.Stage);
                    break;
                case "stage_d":
                    courseIQ = courseIQ.OrderByDescending(c => c.Stage);
                    break;
                case "trainer":
                    courseIQ = courseIQ.OrderBy(c => c.Trainer);
                    break;
                case "trainer_d":
                    courseIQ = courseIQ.OrderByDescending(c => c.Trainer);
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
