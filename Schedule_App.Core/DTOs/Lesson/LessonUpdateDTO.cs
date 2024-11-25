﻿using Schedule_App.Core.DTOs.ValidationAttributes;
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
        [Range(1, short.MaxValue, ErrorMessage = "The Classroom's id must be a positive integer greater than zero.")]
        public short? ClassroomId { get; set; } = null;

        [DefaultValue(-1)]
        [Range(1, short.MaxValue, ErrorMessage = "The Subject's id must be a positive integer greater than zero.")]
        public short? SubjectId { get; set; } = null;

        [DefaultValue(-1)]
        [Range(1, int.MaxValue, ErrorMessage = "The Group's id must be a positive integer greater than zero.")]
        public int? GroupId { get; set; } = null;

        [DefaultValue(-1)]
        [Range(1, int.MaxValue, ErrorMessage = "The Teacher's id must be a positive integer greater than zero.")]
        public int? TeacherId { get; set; } = null;

        [DefaultValue(null)]
        public string? AdditionalInfo { get; set; } = null;

        [DefaultValue(null)]
        [FutureDate(ErrorMessage = "The date must be in the future.")]
        public DateOnly? Date { get; set; } = null;

        [DefaultValue(null)]
        public DateTime? StartsAt { get; set; } = null;

        [DefaultValue(null)]
        [LaterThan(nameof(StartsAt), ErrorMessage = "Lesson's end time should be bigger than start time.")]
        public DateTime? EndsAt { get; set; } = null;

        [DefaultValue(-1)]
        [Range(1, short.MaxValue, ErrorMessage = "The Status's id must be a positive integer greater than zero.")]
        public short? StatusId { get; set; } = null;
    }
}