using HRMS.WebApi.Models;
using System.Runtime.CompilerServices;


namespace HRMS.WebApi.Logging
{
    public class LoggingServices : ILoggingServices
    {
        private readonly ILogger<LoggingServices> _logger;
        private readonly IConfiguration _configuration;

        public LoggingServices(ILogger<LoggingServices> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<OperationResult> LogError(string args, object instance, [CallerMemberName] string method = "")
        {
            OperationResult result = new();
            string message = resolveMessage(instance, method, "Error");
            _logger.LogError(message, args);
            result.Message = message;
            result.IsSuccess = false;
            return result;
        }


        public async Task<OperationResult> LogWarning(string args, object instance, [CallerMemberName] string method = "")
        {
            OperationResult result = new();
            string message = resolveMessage(instance, method, "Warning");
            _logger.LogError(message, args);
            result.Message = message;
            result.IsSuccess = false;
            return result;
        }

        private string resolveMessage(object instance, string methodName, string type)
        {
            string className = instance.GetType().Name;
            string completeNameForConfig = $"{type}{className}:{methodName}";
            string? message = _configuration[completeNameForConfig];
            return message ?? $"Undefined message for {completeNameForConfig}";
        }

    }
}
