
namespace WebApi.Models {
    public class OperationResult {

        public OperationResult() {
            IsSuccess = true;
        }

        public OperationResult(bool isSuccess, string message = null, object data = null) {
            IsSuccess = isSuccess;
            Message = message;
            Data = data;
        }

        public bool IsSuccess { get; set; }

        public string? Message { get; set; }

        public object? Data { get; set; }
        
        public static OperationResult Success(string message = "Operación completada con éxito", object data = null) {
            return new OperationResult(true, message, data);
        }
        
        public static OperationResult Failure(string message = "La operación ha fallado", object data = null) {
            return new OperationResult(false, message, data);
        }
    }
}