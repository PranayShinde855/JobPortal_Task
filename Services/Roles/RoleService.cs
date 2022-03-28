using Database.Repository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Models.Roles> Add(Models.Roles role)
        {
            try
            {
                Models.Roles obj = new Models.Roles();
                obj.Name = role.Name;
                obj.CreatedDate = DateTime.Now;
                obj.ModifiedDate = DateTime.Now;
                obj.CreatedBy = role.CreatedBy;
                obj.ModifiedBy = role.ModifiedBy;
                obj.IsActive = true;
                _roleRepository.Add(obj);
                return role;
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
                _roleRepository.Delete(obj);
                return true;
            }
            return false;
        }

        public async Task<IQueryable<Models.Roles>> GetAll()
        {
            return await _roleRepository.GetAll();
        }

        public async Task<Models.Roles> GetById(int Id)
        {
            var data =  await _roleRepository.GetById(Id);
            return data;
        }

        public async Task<Models.Roles> Update(int Id, Models.Roles role)
        {
            var obj = await _roleRepository.GetById(Id);
            if(Id !=null)
            {
                obj.Name = role.Name;
                obj.ModifiedDate = DateTime.Now;
                obj.ModifiedBy = role.ModifiedBy;
                obj.IsActive = role.IsActive;
                await _roleRepository.Update(obj);
                return obj;
            }
            return role;
        }
    }
}
