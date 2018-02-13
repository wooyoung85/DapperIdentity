using Dapper;
using DapperIdentity.Core.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperIdentity.Data.Repositories
{
    public class RoleRepository : BaseRepository
    {
        /// <summary>
        /// Inserts a new Role in the Roles table
        /// </summary>
        /// <param name="roleName">The role's name</param>
        /// <returns></returns>
        public async Task Insert(IdentityRole role)
        {
            await WithConnection(async connection =>
            {
                string query = "INSERT INTO Role (Id, Name) VALUES (@Id, @Name)";
                role.Id = Guid.NewGuid().ToString();
                return await connection.ExecuteAsync(query, role);
            });
        }

        /// <summary>
        /// Returns the role Id given a role name
        /// </summary>
        /// <param name="roleName">Role's name</param>
        /// <returns>Role's Id</returns>
        public async Task<string> GetRoleId(string roleName)
        {
            return await WithConnection(async connection =>
            {
                string query = "SELECT Id FROM Role WHERE Name = @Name";
                var result = await connection.ExecuteScalarAsync<string>(query, new { @Name = roleName });
                return result;
            });
        }

        /// <summary>
        /// Returns a role name given the roleId
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns>Role name</returns>
        public async Task<string> GetRoleName(string roleId)
        {
            return await WithConnection(async connection =>
            {
                string query = "SELECT Name FROM Role WHERE Id = @Id";
                var result = await connection.ExecuteScalarAsync<string>(query, new { @Id = roleId });
                return result;
            });
        }
    }
}
