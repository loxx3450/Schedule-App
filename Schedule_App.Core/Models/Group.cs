using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Models
{
    public class Group : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(48)]
        public string Title { get; set; } = null!;

        public virtual List<Lesson> Lessons { get; set; } = [];
    }
}
