using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Models
{
    public class Lesson : AuditableEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public short ClassroomId { get; set; }
        public virtual Classroom Classroom { get; set; } = null!;

        [Required]
        public short SubjectId { get; set; }
        public virtual Subject Subject { get; set; } = null!;

        [Required]
        public int GroupId { get; set; }
        public virtual Group Group { get; set; } = null!;

        [Required]
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; } = null!;

        public string? AdditionalInfo { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public DateTime StartsAt { get; set; }

        [Required]
        public DateTime EndsAt { get; set; }

        [Required]
        public short StatusId { get; set; }
        public virtual LessonStatus Status { get; set; } = null!;
    }
}
