namespace Micro.NotificationService.Models
{
    using FluentResults;
    using System.Collections.Generic;
    using System.Net;

    public class StatusCodeReason : IReason
    {
        public string Message => "";

        public HttpStatusCode StatusCode { get; set; }

        public Dictionary<string, object> Metadata => [];
    }
}
