using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.ADOServices.RoleServices
{
    public interface IRoleServices
    {
        Task<bool> Add(string role, int userId);
        Task<bool> Update(int id, string role, int userId, bool isActive);
        Task<bool> Delete(int id);
        Task<Roles> GetById(int id);
        Task<IEnumerable<Roles>> GetAll();
    }
}
