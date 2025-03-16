using HRMS.Domain.Base;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HRMS.Persistence.Test.FakeContext
{
    public class FakeHRMSContext : HRMSContext
    {
        // bdcontext falso, utilziando una bd en memoria
        
        public FakeHRMSContext() : base(new DbContextOptionsBuilder<HRMSContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).
            Options)
        { }
        public static Mock<ILoggingServices> GetLoggingServiceMock()
        {
            var mockLogger = new Mock<ILoggingServices>();
            // configuracion de sus comportamientos, define como que deberia de hacer el mock cuando se llame un metodo
            // el is any atring 1, es el primer parametro del mensaje de error, el segundo un objeto, el tercer el string del nombre de metodo
            mockLogger.Setup(m => m.LogError(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>())).ReturnsAsync(new OperationResult { IsSuccess = true, Message = "Error logeado", Data = null });

            mockLogger.Setup(m => m.LogWarning(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>())).ReturnsAsync(new OperationResult { IsSuccess = true, Message = "Advertencia logeada", Data = null });

            return mockLogger;
        }
    }
}
