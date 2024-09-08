
namespace Micro.NotificationService.Extensions
{
    using FluentResults;
    using Micro.NotificationService.Models;
    using System.Net;

    public static class ResultExtensions
    {
        public static T WithStatusCode<T>(this T result, HttpStatusCode code) where T : ResultBase
        {
            result.Reasons.Add(new StatusCodeReason() { StatusCode = code });
            return result;
        }

        public static string ToJoinedString(this IEnumerable<IReason> reasons)
        {
            return string.Join(",", reasons.Where(x => !string.IsNullOrEmpty(x.Message)).Select(x => x.Message));
        }
    }
}
