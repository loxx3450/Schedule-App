using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.Group
{
    public class GroupUpdateDTO
    {
        [Required]
        [MinLength(3, ErrorMessage = "The Group's title should contain at least 3 characters.")]
        [MaxLength(20, ErrorMessage = "The Group's title should contain no more than 20 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9_ ]+$", ErrorMessage = "The Group's title can only contain letters, digits, underscore and spaces.")]
        public string Title { get; set; } = null!;
    }
}
