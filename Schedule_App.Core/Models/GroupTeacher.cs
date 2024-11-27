using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Models
{
    public class GroupTeacher : AuditableEntity
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; } = null!;

        [Required]
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; } = null!;
    }
}
