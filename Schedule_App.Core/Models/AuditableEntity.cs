using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Models
{
    // Almost every model is auditable and should have CreatedAt/UpdatedAt/DeletedAt
    public abstract class AuditableEntity
    {
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
