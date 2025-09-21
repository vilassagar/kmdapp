using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;

namespace KMDRecociliationApp.API.Common
{
    public class CamelCasePropertyNamesInterceptor : IValidatorInterceptor
    {
        
      public  ValidationResult AfterAspNetValidation(ActionContext actionContext, IValidationContext validationContext, ValidationResult result)
        {
            var camelCaseErrors = result.Errors.Select(error =>
                 new ValidationFailure(
                     JsonNamingPolicy.CamelCase.ConvertName(error.PropertyName),
                     error.ErrorMessage,
                     error.AttemptedValue
                 )
             ).ToList();

            return new ValidationResult(camelCaseErrors);
        }

      public  IValidationContext BeforeAspNetValidation(ActionContext actionContext, IValidationContext commonContext)
        {
            return commonContext;
        }
    }

}
