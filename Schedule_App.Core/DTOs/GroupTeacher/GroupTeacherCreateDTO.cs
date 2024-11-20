using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.GroupTeacher
{
    public class GroupTeacherCreateDTO
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int GroupId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int TeacherId { get; set; }
    }
}
