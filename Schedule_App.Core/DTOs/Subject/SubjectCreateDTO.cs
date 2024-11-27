using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.Subject
{
    public class SubjectCreateDTO
    {
        [Required]
        [Length(5, 30, ErrorMessage = "The Subject's title should contain 5 to 30 characters.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "The Subject's title can only contain letters and spaces.")]
        public string Title { get; set; } = null!;
    }
}
