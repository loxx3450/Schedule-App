using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.GroupTeacher
{
    public class GroupTeacherReadDTO
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GroupReadSummaryDTO? Group { get; set; } = null;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TeacherReadSummaryDTO? Teacher { get; set; } = null;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
