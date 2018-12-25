using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormsApp.Models;

namespace FormsApp.Data
{
    public class DbInitializer
    {
        public static void Initialize(FormsAppContext context)
        {
            // context.Database.EnsureCreated();

            // Look for any students.
            if (context.Course.Any())
            {
                return;   // DB has been seeded
            }

            var students = new Student[]
            {
                new Student{FirstMidName="Carson",LastName="Alexander",Email="ca@gmail.com",Faculty=Faculty.WZ},
                new Student{FirstMidName="Meredith",LastName="Alonso",Email="ca@gmail.com",Faculty=Faculty.WZ},
                new Student{FirstMidName="Arturo",LastName="Anand",Email="ca@gmail.com",Faculty=Faculty.WGGiOŚ},
                new Student{FirstMidName="Gytis",LastName="Barzdukas",Email="ca@gmail.com",Faculty=Faculty.WGGiOŚ},
                new Student{FirstMidName="Yan",LastName="Li",Email="ca@gmail.com",Faculty=Faculty.WGGiOŚ},
                new Student{FirstMidName="Peggy",LastName="Justice",Email="ca@gmail.com",Faculty=Faculty.WGGiOŚ},
                new Student{FirstMidName="Laura",LastName="Norman",Email="ca@gmail.com",Faculty=Faculty.WMN},
                new Student{FirstMidName="Nino",LastName="Olivetto",Email="ca@gmail.com",Faculty=Faculty.WMN}
            };
            foreach (Student s in students)
            {
                context.Student.Add(s);
            }
            context.SaveChanges();

            var courses = new Course[]
            {
                new Course{Title="Chemistry",MaxStudents = 5, Stage = Stage.Soft, Trainer = "Jan"},
                new Course{Title="Microeconomics",MaxStudents = 5, Stage = Stage.Soft, Trainer = "Jan"},
                new Course{Title="Macroeconomics",MaxStudents = 5, Stage = Stage.Soft, Trainer = "Jan"},
                new Course{Title="Calculus",MaxStudents = 5, Stage = Stage.Soft, Trainer = "Jan"},
                new Course{Title="Trigonometry",MaxStudents = 5, Stage = Stage.Soft, Trainer = "Jan"},
                new Course{Title="Composition",MaxStudents = 5, Stage = Stage.Soft, Trainer = "Jan"},
                new Course{Title="Literature",MaxStudents = 5, Stage = Stage.Soft, Trainer = "Jan"}
            };
            courses[0].DatesAndPlaces.Add(new DateAndPlace { Date = DateTime.Parse("01/12/2019 17:00:00"), Place = "AGH" });
            courses[0].DatesAndPlaces.Add(new DateAndPlace { Date = DateTime.Parse("02/12/2019 17:00:00"), Place = "AGH" });
            foreach (Course c in courses)
            {
                context.Course.Add(c);
            }
            context.SaveChanges();

            var studentCourses = new StudentCourse[]
            {
                new StudentCourse{StudentId=1,CourseId=1},
                new StudentCourse{StudentId=2,CourseId=1},
                new StudentCourse{StudentId=3,CourseId=1},
                new StudentCourse{StudentId=1,CourseId=2},
                new StudentCourse{StudentId=4,CourseId=2}
            };
            foreach (StudentCourse sc in studentCourses)
            {
                context.StudentCourse.Add(sc);
            }
            context.SaveChanges();
        }
    }
}
