using FluentValidation;

namespace Defra.Identity.Requests.Users.Commands.Create;

public class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress();
        RuleFor(x => x.OperatorId).NotEmpty();
    }
}
