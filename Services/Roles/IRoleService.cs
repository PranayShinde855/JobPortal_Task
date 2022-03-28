using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Roles
{
    public interface IRoleService
    {
        Task<Models.Roles> Add(Models.Roles role);
        Task<Models.Roles> Update(int Id, Models.Roles role);
        Task<bool> Delete(int Id);
        Task<Models.Roles> GetById(int Id);
        Task<IQueryable<Models.Roles>> GetAll();
    }
}
