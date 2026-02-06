using System;

namespace OneGuru.CFR.Domain.ResponseModels
{
    public class ErrorResponse
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Type { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Message { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string StackTrace { get; set; }
        public ErrorResponse(Exception ex)
        {
            Type = ex.GetType().Name;
            Message = ex.Message;
            StackTrace = ex.ToString();
        }
    }
}
