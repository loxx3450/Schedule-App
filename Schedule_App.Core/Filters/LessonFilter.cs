using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Filters
{
    public class LessonFilter
    {
        public int? ClassroomId { get; set; } = null;
        public int? SubjectId { get; set; } = null;
        public int? GroupId { get; set; } = null;
        public int? TeacherId { get; set; } = null;
        public DateOnly? Date { get; set; } = null;
        public int? StatusId { get; set; } = null;
    }
}
