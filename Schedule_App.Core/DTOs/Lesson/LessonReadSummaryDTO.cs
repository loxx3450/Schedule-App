using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.Lesson
{
    public class LessonReadSummaryDTO
    {
        public int Id { get; set; }

        public ClassroomReadSummaryDTO Classroom { get; set; } = null!;

        public SubjectReadSummaryDTO Subject { get; set; } = null!;

        public GroupReadSummaryDTO Group { get; set; } = null!;

        public TeacherReadSummaryDTO Teacher { get; set; } = null!;

        public string? AdditionalInfo { get; set; }

        public DateOnly Date { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime EndsAt { get; set; }

        public LessonStatusReadDTO Status { get; set; } = null!;
    }
}
