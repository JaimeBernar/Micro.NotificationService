namespace NotificationService.Validators
{
    using FluentValidation;
    using NotificationService.Common.DTOs;

    public class NotificationMessageValidator : AbstractValidator<NotificationMessage>
    {
        public NotificationMessageValidator()
        {
            this.RuleFor(x => x.NotificationType).NotEmpty().WithMessage("The NotificationType can NOT be empty");
            this.RuleFor(x => x.Body).NotEmpty().WithMessage("The Body can NOT be empty");
        }        
    
    }
}
