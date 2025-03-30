namespace HRMS.WebApi.Models
{
    public class OperationResult
    {
        public OperationResult() 
        {
            this.IsSuccess = true;
        }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public dynamic? Data { get; set; } //podemos usar generic
        
        public static OperationResult Success(object data = null, string message = null) =>
            new() { IsSuccess = true, Data = data, Message = message };

        public static OperationResult Failure(string message) =>
            new() { IsSuccess = false, Message = message };
        public static async Task<OperationResult> ExecuteOperationAsync(Func<Task<OperationResult>> operation)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                return OperationResult.Failure($"Ocurrió un error: {ex.Message}");
            }
        }
    }
}
