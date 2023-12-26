using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ProjectManagement.Infrastructures.Helper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateCompare:ValidationAttribute, IClientValidatable
    {
        /// <summary>
        /// Gets the name of the other property to compare to
        /// </summary>
        public string OtherPropertyName { get; private set; }

        public bool AllowEquality { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="DateCompare"/> class.
        /// </summary>
        /// <param name="otherPropertyName">Name of the compare to date property.</param>
        /// <param name="allowEquality">if set to <c>true</c> equal dates are allowed.</param>
        public DateCompare(string otherPropertyName, bool allowEquality = true)
        {
            AllowEquality = allowEquality;
            OtherPropertyName = otherPropertyName;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = ValidationResult.Success;
            var otherValue = validationContext.ObjectType.GetProperty(OtherPropertyName).GetValue(validationContext.ObjectInstance, null);
            if (value != null)
            {
                DateTime currentValue;
                if(value is DateTime)
                {
                    if (otherValue != null)
                    {
                        DateTime otherDate;
                        if (otherValue is DateTime)
                        {
                            if ((DateTime)value < (DateTime)otherValue)
                            {
                                result = new ValidationResult(ErrorMessage);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "comparedate"
            };
            rule.ValidationParameters["otherpropertyname"] = OtherPropertyName;
            rule.ValidationParameters["allowequality"] = AllowEquality ? "true" : "";
            yield return rule; 
        }
    }
}