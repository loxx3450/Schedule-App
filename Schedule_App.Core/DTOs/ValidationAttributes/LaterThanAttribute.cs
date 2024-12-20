﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.DTOs.ValidationAttributes
{
    internal class LaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public LaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is null)
                return null;

            var actualValue = (DateTime)value!;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty) 
                ?? throw new ArgumentException($"Property with name {_comparisonProperty} is not found");

            // Value that is saved in the 'property'
            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance)!;

            if (actualValue > comparisonValue)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }
    }
}
