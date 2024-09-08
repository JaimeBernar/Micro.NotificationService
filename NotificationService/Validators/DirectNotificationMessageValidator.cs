namespace NotificationService.Validators
{
    using FluentValidation;
    using NotificationService.Common.DTOs;

    public class DirectNotificationMessageValidator : AbstractValidator<DirectNotificationMessage>
    {
        public DirectNotificationMessageValidator()
        {
            this.RuleFor(x => x.Body).NotEmpty().WithMessage("The Body can NOT be empty");
            this.RuleFor(x => x.EmailAddress).NotEmpty().WithMessage("The Email can NOT be empty");
            this.RuleFor(x => x.UserId).NotEmpty().WithMessage("The UserId can NOT be empty");
        }
    }
}
