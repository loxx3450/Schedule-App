﻿using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.Subject
{
    public class SubjectReadSummaryDTO
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;
    }
}
