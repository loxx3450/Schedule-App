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
        [Length(8, 20, ErrorMessage = "The Teacher's username should contain 8 to 20 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "The Teacher's username can only contain letters, digits and spaces.")]
        public string Username { get; set; } = null!;

        [Required]
        [Length(8, 30, ErrorMessage = "The Teacher's password should contain 8 to 30 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "The Teacher's password can only contain letters, digits and spaces.")]
        public string Password { get; set; } = null!;
    }
}
