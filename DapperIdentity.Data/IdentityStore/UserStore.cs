using DapperIdentity.Core.Identity;
using DapperIdentity.Data.Connections;
using DapperIdentity.Data.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;

namespace DapperIdentity.Data.IdentityStore
{
    public class UserStore<TUser> : IUserStore<TUser>, IUserPasswordStore<TUser>, IUserRoleStore<TUser>, IUserSecurityStampStore<TUser>, IUserEmailStore<TUser>
        where TUser : IdentityUser
    {
        private UserRepository<TUser> userRepository;
        private RoleRepository roleRepository;
        private UserRoleRepository userRolesRepository;

        public DbManager Database { get; private set; }

        /// <summary>
        /// Default constructor that initializes a new database
        /// instance using the Default Connection string
        /// </summary>
        public UserStore()
        {
            new UserStore<TUser>(new DbManager(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString));
        }

        /// <summary>
        /// Constructor that takes a dbmanager as argument 
        /// </summary>
        /// <param name="database"></param>
        public UserStore(DbManager database)
        {
            Database = database;
            userRepository = new UserRepository<TUser>();
            roleRepository = new RoleRepository();
            userRolesRepository = new UserRoleRepository();
        }

        public async Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            await userRepository.Insert(user);
        }

        public async Task DeleteAsync(TUser user)
        {
            if (user != null)
            {
                await userRepository.Delete(user);
            }
        }

        public void Dispose()
        {
            if (Database != null)
            {
                Database.Dispose();
                Database = null;
            }
        }

        public async Task<TUser> FindByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }
            return await userRepository.GetUserById(userId);
        }

        public async Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }
            return await userRepository.GetUserByName(userName);
        }

        public async Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            await userRepository.Update(user);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            throw new NotImplementedException();
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

            string roleId = await roleRepository.GetRoleId(roleName);
            if (roleId != null)
            {
                await userRolesRepository.Insert(user, roleId);
            }
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}
