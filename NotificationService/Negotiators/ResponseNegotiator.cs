namespace NotificationService.Negotiators
{
    using Carter;
    using Microsoft.Net.Http.Headers;
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

            await JsonSerializer.SerializeAsync(res.Body, model);
        }
    }
}
