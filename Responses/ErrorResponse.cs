namespace BillingSystemBackend.Models
{
    public class ErrorResponse
    {
        public int Code { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorDetails { get; set; }

        public ErrorResponse(string message, string errorDetails = null,int code = 400)
        {
            Code = code;
            Success = false;
            Message = message;
            ErrorDetails = errorDetails;
        }
    }
}