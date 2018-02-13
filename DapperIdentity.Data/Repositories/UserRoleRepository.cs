using Dapper;
using DapperIdentity.Core.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperIdentity.Data.Repositories
{
    public class UserRoleRepository : BaseRepository
    {
        /// <summary>
        /// Inserts a new Role in the Roles table
        /// </summary>
        /// <param name="roleName">The role's name</param>
        /// <returns></returns>
        public async Task Insert(IdentityUser user, string roleId)
        {
            await WithConnection(async connection =>
            {
                string query = "INSERT INTO UserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)";
                return await connection.ExecuteAsync(query, new { @UserId = user.Id, @RoleId = roleId });
            });
        }

        /// <summary>
        /// Returns a list of user's roles
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public async Task<IList<string>> FindByUserId(string userId)
        {
            return await WithConnection(async connection =>
            {
                string query = "SELECT Roles.Name FROM UserRoles, Roles WHERE UserRoles.UserId = @UserId AND UserRoles.RoleId = Roles.Id";
                var userRoles = await connection.QueryAsync<string>(query, new { @UserId = userId });
                return userRoles.ToList();
            });
        }

        /// <summary>
        /// Deletes all roles from a user in the UserRoles table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public async Task Delete(string userId)
        {
            await WithConnection(async connection =>
            {
                string query = "DELETE FROM UserRoles WHERE UserId = @UserId";
                return await connection.ExecuteAsync(query, new { @UserId = userId });
            });
        }
    }
}
