using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Models
{
    public class Teacher : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        
        [Required]
        [MaxLength(20)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [Range(18, 80)]
        public byte Age { get; set; }

        public virtual List<Lesson> Lessons { get; set; } = [];
    }
}
