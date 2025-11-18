namespace ERP_Models.Entities.Common.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;

        private ApiResponse(bool success, T? data, string message)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        public static ApiResponse<T> Ok(T data, string message = "Success")
            => new ApiResponse<T>(true, data, message);

        public static ApiResponse<T> Fail(string message)
            => new ApiResponse<T>(false, default, message);
    }

    // Non-generic version for DeleteAsync(bool) etc.
    public static class ApiResponse
    {
        public static ApiResponse<bool> Ok(bool data, string message = "Success")
            => ApiResponse<bool>.Ok(data, message);

        public static ApiResponse<bool> Fail(string message)
            => ApiResponse<bool>.Fail(message);
    }
}