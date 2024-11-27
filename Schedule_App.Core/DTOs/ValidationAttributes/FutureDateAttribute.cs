using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Schedule_App.Core.DTOs.ValidationAttributes
{
    internal class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
                return null;

            var currentValue = (DateOnly)value!;

            // Solves the problem of client having other time zone
            // TODO: solve
            var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

            if (currentValue < yesterday)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
