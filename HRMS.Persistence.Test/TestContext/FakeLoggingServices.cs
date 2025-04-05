using HRMS.Domain.Base;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Runtime.CompilerServices;

namespace HRMS.Persistence.Test.TestContext
{
    public class FakeLoggingServices : ILoggingServices
    {
        public async  Task<OperationResult> LogError(string args, object instance, [CallerMemberName] string method = "")
        {
            System.Diagnostics.Debug.WriteLine(args);
            var res = new OperationResult();
            res.IsSuccess = false;
            res.Message = args;
            return res;
        }

        public async Task<OperationResult> LogWarning(string args, object instance, [CallerMemberName] string method = "")
        {
            Console.WriteLine(args);
            return new OperationResult();
        }
    }
}
