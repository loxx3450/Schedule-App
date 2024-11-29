using Schedule_App.Core.DTOs.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.Lesson
{
    public class LessonUpdateDTO
    {
        [DefaultValue(-1)]
        [Range(1, int.MaxValue, ErrorMessage = "The Classroom's id must be a positive integer greater than zero.")]
        public int? ClassroomId { get; set; } = null;

        [DefaultValue(-1)]
        [Range(1, int.MaxValue, ErrorMessage = "The Subject's id must be a positive integer greater than zero.")]
        public int? SubjectId { get; set; } = null;

        [DefaultValue(-1)]
        [Range(1, int.MaxValue, ErrorMessage = "The Group's id must be a positive integer greater than zero.")]
        public int? GroupId { get; set; } = null;

        [DefaultValue(-1)]
        [Range(1, int.MaxValue, ErrorMessage = "The Teacher's id must be a positive integer greater than zero.")]
        public int? TeacherId { get; set; } = null;

        [DefaultValue(null)]
        public string? AdditionalInfo { get; set; } = null;

        [DefaultValue(null)]
        [FutureDate(ErrorMessage = "The Lesson's start time should be from the future")]
        public DateTime? StartsAt { get; set; } = null;

        [DefaultValue(null)]
        [LaterThan(nameof(StartsAt), ErrorMessage = "Lesson's end time should be bigger than start time.")]
        public DateTime? EndsAt { get; set; } = null;

        [DefaultValue(-1)]
        [Range(1, int.MaxValue, ErrorMessage = "The Status's id must be a positive integer greater than zero.")]
        public int? StatusId { get; set; } = null;
    }
}
