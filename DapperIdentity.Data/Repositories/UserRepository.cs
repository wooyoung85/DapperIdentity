using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperIdentity.Core.Interfaces;
using Microsoft.AspNet.Identity;
using DapperIdentity.Core.Identity;

namespace DapperIdentity.Data.Repositories
{
    public class UserRepository<TUser> : BaseRepository, IUserRepository<TUser> where TUser : IdentityUser
    {
        private RoleRepository roleRepository;
        private UserRoleRepository userRoleRepository;

        public UserRepository()
        {
            roleRepository = new RoleRepository();
            userRoleRepository = new UserRoleRepository();
        }

        /// <summary>
        /// INSERT operation for a new user.
        /// </summary>
        /// <param name="user">The User object must be passed in.  We create this during the Register Action.</param>
        /// <returns>Returns a 0 or 1 depending on whether operation is successful or not.</returns>
        public async Task CreateAsync(TUser user)
        {
            await WithConnection(async connection =>
            {
                string query = "INSERT INTO Users(Id,UserName,Nickname,PasswordHash,SecurityStamp,IsConfirmed,ConfirmationToken,CreatedDate,Company) VALUES(@Id,@UserName,@Nickname,@PasswordHash,@SecurityStamp,@IsConfirmed,@ConfirmationToken,@CreatedDate,'SKCC')";
                user.Id = Guid.NewGuid().ToString();
                return await connection.ExecuteAsync(query, user);
            });
        }

        /// <summary>
        /// DELETE operation for a user.  This is not currently used, but required by .NET Identity.
        /// </summary>
        /// <param name="user">The User object</param>
        /// <returns>Returns a 0 or 1 depending on whether operation is successful or not.</returns>
        public async Task DeleteAsync(TUser user)
        {
            await WithConnection(async connection =>
            {
                string query = "DELETE FROM Users WHERE Id=@Id";
                return await connection.ExecuteAsync(query, new { @Id = user.Id });
            });
        }

        /// <summary>
        /// SELECT operation for finding a user by the Id value.  Our Id is currently a GUID but this can be another data type as well.
        /// </summary>
        /// <param name="userId">The Id of the user object.</param>
        /// <returns>Returns the User object for the supplied Id or null.</returns>
        public async Task<TUser> FindByIdAsync(string userId)
        {
            return await WithConnection(async connection =>
            {
                string query = "SELECT * FROM Users WHERE Id=@Id";
                var user = await connection.QueryAsync<TUser>(query, new { @Id = userId });
                return user.SingleOrDefault();
            });
        }

        /// <summary>
        /// SELECT operation for finding a user by the username.
        /// </summary>
        /// <param name="userName">The username of the user object.</param>
        /// <returns>Returns the User object for the supplied username or null.</returns>
        public async Task<TUser> FindByNameAsync(string userName)
        {
            return await WithConnection(async connection =>
            {
                string query = "SELECT * FROM Users WHERE LOWER(UserName)=LOWER(@UserName)";
                var user = await connection.QueryAsync<TUser>(query, new { @UserName = userName });
                return user.SingleOrDefault();
            });
        }

        /// <summary>
        /// UPDATE operation for updating a user.
        /// </summary>
        /// <param name="user">The user that will be updated.  The updated values must be passed in to this method.</param>
        /// <returns>Returns a 0 or 1 depending on whether operation is successful or not.</returns>
        public async Task UpdateAsync(TUser user)
        {
            await WithConnection(async connection =>
            {
                string query =
                    "UPDATE Users SET UserName=@UserName,Nickname=@Nickname,PasswordHash=@PasswordHash,SecurityStamp=@SecurityStamp,IsConfirmed=@IsConfirmed,CreatedDate=@CreatedDate,ConfirmationToken=@ConfirmationToken WHERE Id=@Id";
                return await connection.ExecuteAsync(query, user);
            });
        }
        
        /// <summary>
        /// Method for setting the password hash for the user account.  This hash is used to encode the users password.
        /// </summary>
        /// <param name="user">The user to has the password for.</param>
        /// <param name="passwordHash">The password has to use.</param>
        /// <returns></returns>
        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Method for getting teh password hash for the user account.
        /// </summary>
        /// <param name="user">The user to get the password hash for.</param>
        /// <returns>The password hash.</returns>
        public Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        /// Method for checking if an account has a password hash.
        /// </summary>
        /// <param name="user">The user to check for an existing password hash.</param>
        /// <returns>True of false depending on whether the password hash exists or not.</returns>
        public Task<bool> HasPasswordAsync(TUser user)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        /// <summary>
        /// Method for setting the security stamp for the user account.
        /// </summary>
        /// <param name="user">The user to set the security stamp for.</param>
        /// <param name="stamp">The stamp to set.</param>
        /// <returns></returns>
        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Method for getting the security stamp for the user account.
        /// </summary>
        /// <param name="user">The user to get the security stamp for.</param>
        /// <returns>The security stamp.</returns>
        public Task<string> GetSecurityStampAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.SecurityStamp);
        }

        public void Dispose()
        {
            //
        }

        public async Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }

            string roleId = roleRepository.GetRoleId(roleName).Result;
            if (roleId != "")
            {
                await userRoleRepository.Insert(user, roleId);
            }
        }

        /// <summary>
        /// Gets the IdentityRole given the role Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IdentityRole GetRoleById(string roleId)
        {
            var roleName = roleRepository.GetRoleName(roleId).Result;
            IdentityRole role = null;

            if (roleName != null)
            {
                role = new IdentityRole(roleId, roleName);
            }

            return role;
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            List<string> roles = userRoleRepository.FindByUserId(user.Id).Result.ToList();
            {
                if (roles != null)
                {
                    return Task.FromResult<IList<string>>(roles);
                }
            }

            return Task.FromResult<IList<string>>(null);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        async Task<TUser> IUserStore<TUser, string>.FindByNameAsync(string userName)
        {
            return await WithConnection(async connection =>
            {
                string query = "SELECT * FROM Users WHERE LOWER(UserName)=LOWER(@UserName)";
                var user = await connection.QueryAsync<TUser>(query, new { @UserName = userName });
                return user.SingleOrDefault();
            });
        }
    }
}
