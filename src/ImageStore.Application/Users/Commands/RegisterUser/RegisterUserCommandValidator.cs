using FluentValidation;

namespace ImageStore.Application.Users.Commands.RegisterUser
{
    public class RegisterUserCommandValidator: AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible);

            RuleFor(x => x.Password)
                .MinimumLength(5);

            RuleFor(x => x.UserName)
                .MinimumLength(5);
        }
    }
}
