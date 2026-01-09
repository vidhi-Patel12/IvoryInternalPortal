using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace IvoryInternalPortal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

       

    }

}
