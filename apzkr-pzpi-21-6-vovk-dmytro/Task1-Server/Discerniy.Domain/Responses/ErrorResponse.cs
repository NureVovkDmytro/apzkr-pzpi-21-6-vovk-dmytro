namespace Discerniy.Domain.Responses
{
    public class ErrorResponse
    {
        public string Message { get; set; } = "An error occurred.";
        public string? Details { get; set; }

        public ErrorResponse(string message, string? details = null)
        {
            Message = message;
            Details = details;
        }

        public ErrorResponse(Exception ex)
        {
            Message = ex.Message;
            Details = ex.StackTrace;
        }

        public ErrorResponse(Exception ex, string message)
        {
            Message = message;
            Details = ex.StackTrace;
        }

        public ErrorResponse(string message)
        {
            Message = message;
        }

        public ErrorResponse()
        {
        }
    }
}
