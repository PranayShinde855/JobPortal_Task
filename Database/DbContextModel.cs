using Microsoft.EntityFrameworkCore;
using Models;

namespace Database
{
    public class DbContextModel : DbContext
    {
        public DbContextModel(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<AppliedJob> AppliedJobs { get; set; }
        public DbSet<OTP> OTP { get; set; }
    }
}
