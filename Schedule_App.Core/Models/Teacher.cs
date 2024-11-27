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
        [Length(36, 48)]
        [RegularExpression(@"^[a-zA-Z0-9_]+$")]
        public string Username { get; set; } = null!;

        [Required]
        [Length(48, 48)]
        [RegularExpression(@"^[a-zA-Z0-9_]+$")]
        public string Password { get; set; } = null!;

        [Required]
        [Length(1, 50)]
        [RegularExpression(@"^[a-zA-Z ]+$")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Length(1, 50)]
        [RegularExpression(@"^[a-zA-Z ]+$")]
        public string LastName { get; set; } = null!;

        [Required]
        [Range(18, 80)]
        public byte Age { get; set; }


        public virtual List<Subject> Subjects { get; set; } = [];

        public virtual List<Lesson> Lessons { get; set; } = [];
    }
}
