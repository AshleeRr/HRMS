namespace HRMS.Domain.Base
{
    public class OperationResult
    {
        public OperationResult() 
        {
            this.IsSuccessSuccess = true;

        }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public dynamic? Data { get; set; } //podemos usar generic
    }
}
