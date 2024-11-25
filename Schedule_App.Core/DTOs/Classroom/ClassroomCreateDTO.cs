﻿using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.Classroom
{
    public class ClassroomCreateDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "The Classroom's title should contain at least 3 characters.")]
        [MaxLength(10, ErrorMessage = "The Classroom's title should contain no more than 10 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_ ]+$", ErrorMessage = "The Classroom's title can only contain letters, digits, underscore and spaces.")]
        public string Title { get; set; } = null!;
    }
}