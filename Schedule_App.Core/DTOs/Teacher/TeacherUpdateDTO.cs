using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.Teacher
{
    public class TeacherUpdateDTO
    {
        [MaxLength(20)]
        public string? Username { get; set; }

        [MaxLength(255)]
        public string? Password { get; set; }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [Range(18, 80)]
        public byte? Age { get; set; }
    }
}
