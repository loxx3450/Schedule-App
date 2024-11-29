using Schedule_App.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Schedule_App.API.DTOs.LessonStatus
{
    public class LessonStatusReadDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
    }
}
