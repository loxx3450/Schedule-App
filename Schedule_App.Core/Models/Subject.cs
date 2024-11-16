using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Models
{
    public class Subject : AuditableEntity
    {
        [Key]
        public short Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Title { get; set; } = null!;

        public virtual List<Teacher> Teachers { get; set; } = [];

        public virtual List<Lesson> Lessons { get; set; } = [];
    }
}
