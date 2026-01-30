namespace Defra.Identity.Requests.Users.Commands.Update;

using FluentValidation;

public class UpdateUserValidator : AbstractValidator<UpdateUser>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress();
        RuleFor(x => x.OperatorId).NotEmpty();
    }
}
