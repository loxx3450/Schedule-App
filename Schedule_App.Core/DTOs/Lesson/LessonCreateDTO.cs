using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.DTOs.ValidationAttributes;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.Lesson
{
    public class LessonCreateDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The Classroom's id must be a positive integer greater than zero.")]
        public int ClassroomId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The Subject's id must be a positive integer greater than zero.")]
        public int SubjectId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The Group's id must be a positive integer greater than zero.")]
        public int GroupId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The Teacher's id must be a positive integer greater than zero.")]
        public int TeacherId { get; set; }

        [DefaultValue(null)]
        public string? AdditionalInfo { get; set; }

        [Required]
        [FutureDate(ErrorMessage = "The Lesson's start time should be from the future")]
        public DateTime StartsAt { get; set; }

        [Required]
        [LaterThan(nameof(StartsAt), ErrorMessage = "Lesson's end time should be bigger than start time.")]
        public DateTime EndsAt { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The Status's id must be a positive integer greater than zero.")]
        public int StatusId { get; set; }
    }
}
