namespace Schedule_App.API.Helpers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }

        // Represents Payload in a successful Response or Errors in a failed Response
        public T? Data { get; set; }
    }
}
