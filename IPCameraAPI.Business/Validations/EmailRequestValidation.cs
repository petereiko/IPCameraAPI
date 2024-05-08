using FluentValidation;
using IPCameraAPI.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Validations
{
    public class EmailRequestValidation: AbstractValidator<EmailRequest>
    {
        public EmailRequestValidation()
        {
            RuleFor(u => u.Recipient).NotEmpty().WithMessage("Email Address is required").Must(x => x.Contains("@")).WithMessage("Please enter a valid email address").MaximumLength(50).WithMessage("Maximum length is 50");
            RuleFor(u => u.Subject).NotEmpty().WithMessage("Subject is required");

        }
    }
}
