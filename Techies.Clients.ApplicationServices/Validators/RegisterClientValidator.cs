using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Techies.Clients.ApplicationServices.Resources;
using Techies.Clients.DTOs.Request;

namespace Techies.Clients.ApplicationServices.Validators
{
    public class RegisterClientValidator: AbstractValidator<RegisterClient>
    {
        public RegisterClientValidator()
        {
            RuleFor(c=> c.Email)
                .EmailAddress()
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(c=> c.FirstName)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(200);

            RuleFor(c=> c.LastName)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(200);

            RuleFor(c=> c.Birthdate)
                .LessThanOrEqualTo(DateTime.UtcNow.Date);
        }
    }
}
