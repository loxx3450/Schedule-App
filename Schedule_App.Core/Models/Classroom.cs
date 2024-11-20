using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Models
{
    public class Classroom : AuditableEntity
    {
        [Key]
        public short Id { get; set; }

        [Required]
        [MaxLength(38)]
        public string Title { get; set; } = null!;

        public virtual List<Lesson> Lessons { get; set; } = [];
    }
}
