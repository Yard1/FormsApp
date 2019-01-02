using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FormsApp.Models
{
    public enum Faculty
    {
        WGGiOŚ, WMN, WZ
    }

    public class Student
    {
        public int StudentId { get; set; }

        [Required, MaxLength(256)]
        public string LastName { get; set; }

        [Required, MaxLength(256)]
        public string FirstMidName { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; }

        public Faculty Faculty { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }

        public Student()
        {
            StudentCourses = new HashSet<StudentCourse>();
        }
    }

    public class StudentVM
    {
        public int StudentId { get; set; }

        [Required, MaxLength(256)]
        public string LastName { get; set; }

        [Required, MaxLength(256)]
        public string FirstMidName { get; set; }

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; }

        public Faculty Faculty { get; set; }

        public string SearchValue { get; set; }
    }
}
