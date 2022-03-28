using Database.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repository
{
    public interface IRoleRepository : IRepository<Roles>
    { 
    }
    public class RoleRepository : Repository<Roles>, IRoleRepository
    {
        public RoleRepository(DbContextModel dbContext) : base(dbContext)
        {
        }
    }
}
