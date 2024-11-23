using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Tests.Comparers
{
    internal class LessonStatusComparer : IEqualityComparer<LessonStatusReadDTO>
    {
        public bool Equals(LessonStatusReadDTO? x, LessonStatusReadDTO? y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && x.Description == y.Description;
        }

        public int GetHashCode([DisallowNull] LessonStatusReadDTO obj)
        {
            return HashCode.Combine(obj.Id, obj.Description);
        }
    }
}
