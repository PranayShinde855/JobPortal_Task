using Database.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Models.Roles> Add(string role, int userId)
        {
            try
            {
                Models.Roles obj = new Models.Roles();
                obj.Name = role;
                obj.CreatedDate = DateTime.Now;
                obj.ModifiedDate = DateTime.Now;
                obj.CreatedBy = userId;
                obj.ModifiedBy = userId;
                obj.IsActive = true;
                var info = await _roleRepository.Add(obj);
                return info;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Delete(int Id)
        {
            var obj = await _roleRepository.GetById(Id);
            if (obj != null)
            {
                await _roleRepository.Delete(obj);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Models.Roles>> GetAll()
        {
            return await _roleRepository.GetAll();
        }

        public async Task<Models.Roles> GetById(int Id)
        {
            var data =  await _roleRepository.GetById(Id);
            return data;
        }

        public async Task<Models.Roles> Update(int Id, string role, int userId, bool isActive)
        {
            var info = new Models.Roles();
            var obj = await _roleRepository.GetById(Id);
            if(obj !=null)
            {
                obj.Name = role;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = userId;
                obj.IsActive = isActive;
                info = await _roleRepository.Update(obj);
                return obj;
            }
            return info;
        }
    }
}
