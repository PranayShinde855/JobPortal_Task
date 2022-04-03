using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Roles
{
    public interface IRoleService
    {
        Task<Models.Roles> Add(string role, int userId);
        Task<Models.Roles> Update(int Id, string role, int userId, bool isAcive);
        Task<bool> Delete(int Id);
        Task<Models.Roles> GetById(int Id);
        Task<IEnumerable<Models.Roles>> GetAll();
    }
}
