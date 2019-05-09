using FluentValidation.Results;
using System.Linq;
using Techies.Clients.DTOs.Responses;

namespace Techies.Clients.ApplicationServices.Validators
{
    public static class ValidatorExtensions
    {
        public static OperationResult ToOperationResult(this ValidationResult result)
        {
            return new OperationResult(result.IsValid,result.Errors.FirstOrDefault()?.ErrorMessage);
        }

        public static OperationResult<T> ToOperationResult<T>(this ValidationResult result)
        {
            return new OperationResult<T>(result.IsValid,default(T), result.Errors.FirstOrDefault()?.ErrorMessage);
        }
    }
}
