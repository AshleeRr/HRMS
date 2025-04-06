using HRMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Persistence.Test.TestContext
{
    public class FakeContext : HRMSContext
    {
        public FakeContext(DbContextOptions<HRMSContext> options) : base(options)
        {
        }
    }
}
