namespace NotificationService.Validators
{
    using FluentValidation;
    using NotificationService.Common.DTOs;

    public class SubscriptionMessageValidator : AbstractValidator<SubscriptionMessage>
    {
        public SubscriptionMessageValidator()
        {
            this.RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId can NOT be empty");
            this.RuleFor(x => x.EmailAddress).NotEmpty().WithMessage("EmailAddress can NOT be empty");
        }
    }
}
