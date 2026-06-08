using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class NewCourseModel
    {
       
        public int SchoolClassId { get; set; }
        public int SubjectId { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public int LessonNumber { get; set; }
        public int RoomId { get; set; }
    }
}
