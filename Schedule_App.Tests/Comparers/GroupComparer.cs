﻿using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.DTOs.Group;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Tests.Comparers
{
    internal class GroupComparer : IEqualityComparer<GroupReadSummaryDTO>
    {
        public bool Equals(GroupReadSummaryDTO? x, GroupReadSummaryDTO? y)
        {
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            return x.Id == y.Id
                && x.Title == y.Title;
        }

        public int GetHashCode([DisallowNull] GroupReadSummaryDTO obj)
        {
            return HashCode.Combine(obj.Id, obj.Title);
        }
    }
}
