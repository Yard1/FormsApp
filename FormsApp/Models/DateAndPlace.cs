using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormsApp.Models
{
    public class DateAndPlace
    {
        public int DateAndPlaceId { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
    }

    public class DateAndPlaceVM
    {
        public DateTime Date { get; set; }
        public string Place { get; set; }
    }
}
