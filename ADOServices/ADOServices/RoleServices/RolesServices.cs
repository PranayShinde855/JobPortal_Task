using Database.ADO;
using Microsoft.Data.SqlClient;
using Models;
using Services.ADOServices.RoleServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ADOServices.ADOServices.RoleServices
{
    public class RolesServices : IRoleServices
    {
        public async Task<bool> Add(string name, int userId)
        {
            string query = "INSERT INTO Roles VALUES(@Name,@CreatedDate,@ModifiedDate, @CreatedBy, @ModofoedBy,@IsActive)";
            var parameters = new IDataParameter[]
            {
                new SqlParameter("@Name", name.ToString()),
                new SqlParameter("@CreatedDate", DateTime.Now),
                new SqlParameter("@ModifiedDate",DateTime.Now),
                new SqlParameter("@CreatedBy", userId),
                new SqlParameter("@ModifiedBy",userId),
                new SqlParameter("@IsActive", 1)
           };
            var data = await DB<Roles>.ExecuteData(query, parameters);
            if (data > 0)
                return true;

            return false;
        }

        public async Task<bool> Delete(int id)
        {
            string query = $"DELETE Roles WHERE Id = @Id ";
            var parameters = new IDataParameter[]
           {
                new SqlParameter("@Id", Convert.ToInt32(id))
           };
            int info = await DB<Roles>.ExecuteData(query, parameters);
            if (info > 0)
                return true;
            return false;
        }

        public async Task<IEnumerable<Roles>> GetAll()
        {
            string query = "SELECT * FROM Roles";
            IEnumerable<Roles> obj = await DB<Roles>.GetList(query);
            return obj;
        }

        public async Task<Roles> GetById(int id)
        {
            object obj = new object();
            string query = $"SELECT * FROM Roles WHERE Id = @Id";
            var parameters = new IDataParameter[]
           {
                new SqlParameter("@Id", Convert.ToInt32(id))
           };
            Roles role = await DB<Roles>.GetSingleRecord(query, parameters);
            return role;
        }

        public async Task<bool> Update(int id, string role, int userId, bool isActive)
        {
            string query = $"UPDATE Roles SET Name = @Role, ModifiedDate = @ModifiedDate, ModifiedBy = @UserId, IsActive = @IsActive WHERE Id = @Id)";
            var parameters = new IDataParameter[]
           {
                new SqlParameter("@Role", Convert.ToString(role)),
                new SqlParameter("@ModifiedDate", Convert.ToDateTime(DateTime.Now)),
                new SqlParameter("@UserId", Convert.ToInt32(userId)),
                new SqlParameter("@IsActive", Convert.ToBoolean(isActive))
           };
            var data = await DB<Roles>.ExecuteData(query, parameters);
            if (data > 0)
                return true;
            return false;
        }
    }
}
