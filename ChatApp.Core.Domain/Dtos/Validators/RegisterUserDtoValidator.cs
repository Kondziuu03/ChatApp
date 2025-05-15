using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Core.Domain.Dtos.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(ChatDbContext dbContext)
        {
            RuleFor(x => x.Username)
                .NotEmpty();

            RuleFor(x => x.Username)
               .Custom((value, context) =>
               {
                   if (dbContext.Users.Any(user => EF.Functions.Collate(user.Username, "Latin1_General_CS_AS") == value))
                       context.AddFailure("Username", "That username is taken");
               });

            RuleFor(x => x.Password)
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
        }
    }
}
