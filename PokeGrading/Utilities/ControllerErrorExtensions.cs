using Microsoft.AspNetCore.Mvc;

namespace PokeGrading.Utilities
{
    public static class ControllerErrorExtensions
    {
        private const string TraceHeaderName = "X-Trace-Id";

        public static string EnsureTraceId(this ControllerBase controller)
        {
            string traceId = controller.HttpContext.TraceIdentifier;

            if (string.IsNullOrWhiteSpace(traceId))
            {
                traceId = Guid.NewGuid().ToString();
                controller.HttpContext.TraceIdentifier = traceId;
            }

            controller.Response.Headers[TraceHeaderName] = traceId;
            return traceId;
        }

        public static IActionResult BadRequestWithTrace(this ControllerBase controller, object error)
        {
            controller.EnsureTraceId();
            return controller.BadRequest(error);
        }

        public static IActionResult UnauthorizedWithTrace(this ControllerBase controller, object error)
        {
            controller.EnsureTraceId();
            return controller.Unauthorized(error);
        }

        public static IActionResult NotFoundWithTrace(this ControllerBase controller, object error)
        {
            controller.EnsureTraceId();
            return controller.NotFound(error);
        }

        public static IActionResult ConflictWithTrace(this ControllerBase controller, object error)
        {
            controller.EnsureTraceId();
            return controller.Conflict(error);
        }

        public static IActionResult StatusCodeWithTrace(this ControllerBase controller, int statusCode, object error)
        {
            controller.EnsureTraceId();
            return controller.StatusCode(statusCode, error);
        }
    }
}
