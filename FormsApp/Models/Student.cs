using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FormsApp.Models
{
    public enum Faculty
    {
        WZ, WGGiOŚ, WMN
    }

    public class Student
    {
        public int StudentId { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string FirstMidName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public Faculty Faculty { get; set; }

        public ICollection<StudentCourse> StudentCourses { get; set; }
    }
}
