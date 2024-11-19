using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Filters
{
    public class GroupFilter
    {
        public string? Title { get; set; } = null;
        public string? TitlePattern { get; set; } = null;
        public int? TeacherId { get; set; } = null;
    }
}
