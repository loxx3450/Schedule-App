using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.Teacher
{
    public class TeacherCreateDTO
    {
        [Required]
        [Length(8, 20, ErrorMessage = "The Teacher's username should contain 8 to 20 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "The Teacher's username can only contain letters, digits and spaces.")]
        public string Username { get; set; } = null!;

        [Required]
        [Length(8, 30, ErrorMessage = "The Teacher's password should contain 8 to 30 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "The Teacher's password can only contain letters, digits and spaces.")]
        public string Password { get; set; } = null!;

        [Required]
        [Length(1, 50, ErrorMessage = "The Teacher's first name should contain 1 to 50 characters.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "The Teacher's first name can only contain letters and spaces.")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Length(1, 50, ErrorMessage = "The Teacher's last name should contain 1 to 50 characters.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "The Teacher's last name can only contain letters and spaces.")]
        public string LastName { get; set; } = null!;

        [Required]
        [Range(18, 80, ErrorMessage = "The Teacher's age should be between 18 and 80")]
        public byte Age { get; set; }
    }
}
