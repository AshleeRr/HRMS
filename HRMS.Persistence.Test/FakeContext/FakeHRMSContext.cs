using HRMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Test.FakeContext
{
    public class FakeHRMSContext : HRMSContext
    {
        // bdcontext falso, utilziando una bd en memoria

        public FakeHRMSContext() : base(new DbContextOptionsBuilder<HRMSContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).
            Options)
        { }
    }
}
