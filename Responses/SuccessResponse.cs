namespace BillingSystemBackend.Models;

public class SuccessResponse
{
    public SuccessResponse(string message, object data = null, int code = 200)
    {
        Code = code;
        Success = true;
        Message = message;
        Data = data;
    }

    public int Code { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
}