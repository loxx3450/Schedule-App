using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.Teacher
{
    public class TeacherLoginDTO
    {
        [Required]
        [MinLength(8, ErrorMessage = "The Teacher's username should contain at least 8 characters.")]
        [MaxLength(20, ErrorMessage = "The Teacher's username should contain no more than 20 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "The Teacher's username can only contain letters, digits and spaces.")]
        public string Username { get; set; } = null!;

        [Required]
        [MinLength(8, ErrorMessage = "The Teacher's password should contain at least 8 characters.")]
        [MaxLength(30, ErrorMessage = "The Teacher's password should contain no more than 30 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "The Teacher's password can only contain letters, digits and spaces.")]
        public string Password { get; set; } = null!;
    }
}
