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
        [Range(1, int.MaxValue, ErrorMessage = "The Group's id must be a positive integer greater than zero.")]
        public int GroupId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "The Teacher's id must be a positive integer greater than zero.")]
        public int TeacherId { get; set; }
    }
}
