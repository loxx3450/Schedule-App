using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.Group
{
    public class GroupReadSummaryDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;
    }
}
