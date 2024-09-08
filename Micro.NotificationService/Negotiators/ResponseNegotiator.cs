namespace Micro.NotificationService.Negotiators
{
    using Carter;
    using FluentResults;
    using FluentValidation.Results;
    using Microsoft.Net.Http.Headers;
    using Micro.NotificationService.Extensions;
    using Micro.NotificationService.Models;
    using System.Net;
    using System.Text.Json;

    public class ResponseNegotiator : IResponseNegotiator
    {
        public bool CanHandle(MediaTypeHeaderValue accept)
        {
            return accept.MatchesMediaType("application/json");
        }

        public async Task Handle<T>(HttpRequest req, HttpResponse res, T model, CancellationToken cancellationToken)
        {
            res.ContentType = "application/json";

            var (code, entity) = ExtractStatusCodeAndModel(model);

            res.StatusCode = (int)code;

            await JsonSerializer.SerializeAsync(res.Body, entity);
        }

        private static (HttpStatusCode code, object model) ExtractStatusCodeAndModel<T>(T model)
        {
            switch (model)
            {
                case ValidationResult validationResult:
                    if (validationResult.IsValid)
                    {
                        return (HttpStatusCode.OK, "Validation Passed");
                    }

                    var errors = string.Join(",", validationResult.Errors.Select(x => x.ErrorMessage));

                    return (HttpStatusCode.UnprocessableEntity, $"The Entity did NOT pass the validation process. {errors}");

                case ResultBase result:

                    var statusCode = HttpStatusCode.OK;

                    var statusCodeReason = result.Reasons.OfType<StatusCodeReason>().FirstOrDefault();

                    if (statusCodeReason != null)
                    {
                        statusCode = statusCodeReason.StatusCode;
                    }
                    else
                    {
                        statusCode = result.IsSuccess ? HttpStatusCode.OK : HttpStatusCode.InternalServerError;
                    }

                    return (statusCode, result.Reasons.ToJoinedString());

                case Subscription subscription:
                    return (HttpStatusCode.OK, subscription);

                case IEnumerable<Subscription> subscriptions:
                    return (HttpStatusCode.OK, subscriptions);

                case Notification notification:
                    return (HttpStatusCode.OK, notification);

                case IEnumerable<Notification> notifications:
                    return (HttpStatusCode.OK, notifications);

                case DirectNotification directNotification:
                    return (HttpStatusCode.OK, directNotification);

                case IEnumerable<DirectNotification> directNotifications:
                    return (HttpStatusCode.OK, directNotifications);

                case Exception ex:
                    return (HttpStatusCode.InternalServerError, ex);

                default:
                    throw new NotImplementedException($"The Response Negotiator can NOT handle the type {model.GetType()}");
            }
        }
    }
}
