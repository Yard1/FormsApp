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
    public class IndexModel : PageModel
    {
        private readonly FormsApp.Models.FormsAppContext _context;

        public IndexModel(FormsApp.Models.FormsAppContext context)
        {
            _context = context;
        }

        public enum SearchType { LastName, Email, Faculty }

        public string NameSort { get; set; }
        public string EmailSort { get; set; }
        public string FacultySort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public SearchType CurrentSearchType { get; set; }
        public readonly int DefaultEntriesPerPage = 25;
        public int EntriesPerPage { get; set; }

        public PaginatedList<Student> Student { get; set; }

        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, int? pageIndex, int? entriesPerPage, SearchType searchType)
        {
            CurrentSort = sortOrder;
            CurrentSearchType = searchType;
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_d" : "";
            EmailSort = sortOrder == "email" ? "email_d" : "email";
            FacultySort = sortOrder == "faculty" ? "faculty_d" : "faculty";
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            CurrentFilter = searchString;

            IQueryable<Student> studentIQ = _context.Student.Select(c => c);

            if (!string.IsNullOrEmpty(searchString))
            {
                switch (CurrentSearchType)
                {
                    case SearchType.Email:
                        studentIQ = studentIQ.Where(s => s.Email.Contains(CurrentFilter));
                        break;
                    case SearchType.Faculty:
                        studentIQ = studentIQ.Where(s => s.Faculty.ToString().Contains(CurrentFilter));
                        break;
                    default:
                        studentIQ = studentIQ.Where(s => s.LastName.Contains(searchString));
                        break;
                }
            }

            switch (sortOrder)
            {
                case "name_d":
                    studentIQ = studentIQ.OrderByDescending(s => s.LastName + s.FirstMidName);
                    break;
                case "email":
                    studentIQ = studentIQ.OrderBy(s => s.Email);
                    break;
                case "email_d":
                    studentIQ = studentIQ.OrderByDescending(s => s.Email);
                    break;
                case "faculty":
                    studentIQ = studentIQ.OrderBy(s => s.Email);
                    break;
                case "faculty_d":
                    studentIQ = studentIQ.OrderByDescending(s => s.Email);
                    break;
                default:
                    studentIQ = studentIQ.OrderBy(s => s.LastName + s.FirstMidName);
                    break;
            }

            EntriesPerPage = entriesPerPage ?? DefaultEntriesPerPage;
            Student = await PaginatedList<Student>.CreateAsync(
                studentIQ.AsNoTracking(), pageIndex ?? 1, EntriesPerPage);
        }
    }
}
