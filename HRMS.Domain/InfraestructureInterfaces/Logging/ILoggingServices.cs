using HRMS.Domain.Base;
using System.Runtime.CompilerServices;

namespace HRMS.Domain.InfraestructureInterfaces.Logging
{
    public interface ILoggingServices
    {
        Task<OperationResult> LogError(string args, object instance, [CallerMemberName] string method = "");
        Task<OperationResult> LogWarning(string args, object instance, [CallerMemberName] string method = "");

    }
}
