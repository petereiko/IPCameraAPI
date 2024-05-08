using FluentValidation;
using IPCameraAPI.Business.DTOs;
using IPCameraAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace IPCameraAPI.Business.Validations
{
    public class AdminUserDtoValidation:AbstractValidator<AdminUserDto>
    {
        private readonly ApplicationDbContext _context;
        public AdminUserDtoValidation(ApplicationDbContext context)
        {
            _context = context;
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email Address is required").Must(x => x.Contains("@")).WithMessage("Please enter a valid email address").MaximumLength(50).WithMessage("Maximum length is 50");
            RuleFor(x => x.Email).Must(ValidateEmail).WithMessage(x => $"The email {x.Email} is in use");

            RuleFor(u => u.Phone).NotEmpty().WithMessage("Phone number is required");

            RuleFor(x => x.Phone).Must(ValidatePhone).WithMessage(x => $"The phone number {x.Phone} is in use");
        }

        private bool ValidateEmail(string email)
        {
            return !_context.AdminUsers.Any(x => x.Email.Trim().ToLower() == email.Trim().ToLower());
        }

        private bool ValidatePhone(string phone)
        {
            return !_context.AdminUsers.Any(x => x.Phone.Trim().ToLower() == phone.Trim().ToLower());
        }
    }
}
