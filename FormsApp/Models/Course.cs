using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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
            StudentCourses = new HashSet<StudentCourse>();
        }

        public int CourseId { get; set; }
        [Required, MaxLength(256)]
        public string Title { get; set; }
        public Stage Stage { get; set; }
        public int MaxStudents { get; set; }
        [MaxLength(256)]
        public string Trainer { get; set; }

        public string Password { get; set; }
        public string PasswordSalt { get; set; }

        public ICollection<DateAndPlace> DatesAndPlaces { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }

        public void GenerateCoursePassword(string password)
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            Password = hashed;
            PasswordSalt = Convert.ToBase64String(salt);
        }

        public bool CheckCoursePassword(string password)
        {
            var salt = Convert.FromBase64String(PasswordSalt);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed.Equals(Password);
        }
    }

    public class CourseVM : IValidatableObject
    {
        public int CourseId { get; set; }
        [Required, MaxLength(256)]
        public string Title { get; set; }
        public Stage Stage { get; set; }
        public int MaxStudents { get; set; }
        [MaxLength(256)]
        public string Trainer { get; set; }
        public int CurrentStudents { get; set; }
        public string Password { get; set; }
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