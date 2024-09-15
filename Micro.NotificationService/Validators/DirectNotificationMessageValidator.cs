namespace Micro.NotificationService.Validators
{
    using FluentValidation;
    using Micro.NotificationService.Common.DTOs;

    public class DirectNotificationMessageValidator : AbstractValidator<DirectNotificationMessage>
    {
        public DirectNotificationMessageValidator()
        {
            this.RuleFor(x => x.Body).NotEmpty().WithMessage("The Body can NOT be empty");
            this.RuleFor(x => x).Must(VerifyEmailOrUserId).WithMessage("Email and UserId can NOT be empty. One of them must be filled.");
        }

        private static bool VerifyEmailOrUserId(DirectNotificationMessage directNotification)
        {
            return !string.IsNullOrEmpty(directNotification.EmailAddress) || directNotification.UserId != default;
        }
    }

    public class DirectNotificationMessagesValidator : AbstractValidator<IEnumerable<DirectNotificationMessage>>
    {
        public DirectNotificationMessagesValidator(DirectNotificationMessageValidator validator)
        {
            this.RuleForEach(x => x).SetValidator(validator);
        }
    }
}
