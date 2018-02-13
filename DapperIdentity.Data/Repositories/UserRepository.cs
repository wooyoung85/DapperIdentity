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
    public class UserRepository<TUser> : BaseRepository where TUser : IdentityUser
    {
        public UserRepository()
        {
        }

        /// <summary>
        /// INSERT operation for a new user.
        /// </summary>
        /// <param name="user">The User object must be passed in.  We create this during the Register Action.</param>
        /// <returns>Returns a 0 or 1 depending on whether operation is successful or not.</returns>
        public async Task Insert(TUser user)
        {
            await WithConnection(async connection =>
            {
                string query = "INSERT INTO Users(Id,Email,UserName,PasswordHash,SecurityStamp,IsConfirmed,ConfirmationToken,CreatedDate,Company) VALUES(@Id,@Email,@UserName,@PasswordHash,@SecurityStamp,@IsConfirmed,@ConfirmationToken,@CreatedDate,'SKCC')";
                user.Id = Guid.NewGuid().ToString();
                return await connection.ExecuteAsync(query, user);
            });
        }

        /// <summary>
        /// DELETE operation for a user.  This is not currently used, but required by .NET Identity.
        /// </summary>
        /// <param name="user">The User object</param>
        /// <returns>Returns a 0 or 1 depending on whether operation is successful or not.</returns>
        public async Task Delete(TUser user)
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
        public async Task<TUser> GetUserById(string userId)
        {
            return await WithConnection(async connection =>
            {
                string query = "SELECT * FROM Users WHERE Id=@Id";
                var user = await connection.QueryAsync<TUser>(query, new { @Id = userId });
                return user.SingleOrDefault();
            });
        }

        /// <summary>
        /// SELECT operation for finding a user by the email.
        /// </summary>
        /// <param name="email">The email of the user object.</param>
        /// <returns>Returns the User object for the supplied username or null.</returns>
        public async Task<TUser> GetUserByEmail(string email)
        {
            return await WithConnection(async connection =>
            {
                string query = "SELECT * FROM Users WHERE Email = @Email";
                var user = await connection.QueryAsync<TUser>(query, new { @Email = email });
                return user.SingleOrDefault();
            });
        }

        /// <summary>
        /// SELECT operation for finding a user by the username.
        /// </summary>
        /// <param name="userName">The username of the user object.</param>
        /// <returns>Returns the User object for the supplied username or null.</returns>
        public async Task<TUser> GetUserByName(string userName)
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
        public async Task Update(TUser user)
        {
            await WithConnection(async connection =>
            {
                string query =
                    @"UPDATE Users 
                        SET Email=@Email,UserName=@UserName,PasswordHash=@PasswordHash,SecurityStamp=@SecurityStamp,
                            IsConfirmed=@IsConfirmed,CreatedDate=@CreatedDate,ConfirmationToken=@ConfirmationToken,LockoutEnabled=@LockoutEnabled,LockoutEndDateUtc=@LockoutEndDateUtc
                        WHERE Id=@Id";
                return await connection.ExecuteAsync(query, user);
            });
        }
    }
}
