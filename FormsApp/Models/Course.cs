using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FormsApp.Models
{
    public enum Stage
    {
        Soft, Workshops, Visit, Certification
    }

    public class Course 
    {
        public Course()
        {
            DatesAndPlaces = new HashSet<DateAndPlace>();
        }

        public int CourseId { get; set; }
        [Required]
        public string Title { get; set; }
        public Stage Stage { get; set; }
        public int MaxStudents { get; set; }
        public string Trainer { get; set; }

        public ICollection<DateAndPlace> DatesAndPlaces { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }

    public class CourseVM : IValidatableObject
    {
        public int CourseId { get; set; }
        [Required]
        public string Title { get; set; }
        public Stage Stage { get; set; }
        public int MaxStudents { get; set; }
        public string Trainer { get; set; }
        public int CurrentStudents { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MaxStudents < CurrentStudents)
            {
                yield return new ValidationResult
                    ("MaxStudents cannot be smaller than CurrentStudents", new[] { "MaxStudents" });
            }
        }
    }
}