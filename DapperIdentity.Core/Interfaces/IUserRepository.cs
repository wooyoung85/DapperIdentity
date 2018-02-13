﻿using DapperIdentity.Core.Identity;
using Microsoft.AspNet.Identity;

namespace DapperIdentity.Core.Interfaces
{
    /// <summary>
    /// For our custom user repository, we're going to inherent the minimum required interfaces to implement identity.  There are others for more complex examples and for newer
    /// implementations we can even do all of this with claims.
    /// </summary>
    public interface IUserRepository<TUser> : IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserStore<TUser>
        where TUser : IdentityUser
    {
    }
}
