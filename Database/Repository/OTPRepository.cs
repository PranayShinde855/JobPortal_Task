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
    public interface IOTPRepository : IRepository<OTP>
    {

    }
    public class OTPRepository : Repository<OTP>, IOTPRepository
    {
        public OTPRepository(DbContextModel dbContext) : base(dbContext)
        {
        }
    }
}
