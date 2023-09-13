using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BranchCrudAPI_JWT.Models;

namespace BranchCrudAPI_JWT.Data
{
    public class BranchCrudAPI_JWTContext : DbContext
    {
        public BranchCrudAPI_JWTContext (DbContextOptions<BranchCrudAPI_JWTContext> options)
            : base(options)
        {
        }

        public DbSet<BranchCrudAPI_JWT.Models.User> User { get; set; } = default!;

        public DbSet<BranchCrudAPI_JWT.Models.Branch>? Branch { get; set; }
    }
}
