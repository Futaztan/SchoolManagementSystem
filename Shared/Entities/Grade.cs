using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Shared.Entities
{
    public class Grade
    {
        public int Id { get; set; }
        [Range(1, 5)]
        public int Value { get; set; }
        public Course Course { get; set; }
        public int CourseId { get; set; }
        [JsonIgnore]
        public Student Student { get; set; }
        public int StudentId { get; set; }
        public DateOnly Date { get; set; }
    }
}
