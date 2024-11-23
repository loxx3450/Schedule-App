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
        [MinLength(5, ErrorMessage = "The Subject's title should contain at least 5 characters.")]
        [MaxLength(30, ErrorMessage = "The Subject's title should contain no more than 30 characters.")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "The Subject's title can only contain letters and spaces.")]
        public string Title { get; set; } = null!;
    }
}
