using HRMS.WebApi.Models;
using System.Runtime.CompilerServices;

namespace HRMS.WebApi.Logging
{
    public interface ILoggingServices
    {
        Task<OperationResult> LogError(string args, object instance, [CallerMemberName] string method = "");
        Task<OperationResult> LogWarning(string args, object instance, [CallerMemberName] string method = "");

    }
}
