using Microsoft.AspNet.Identity;
using System;

namespace DapperIdentity.Core.Identity
{
    public class IdentityUser : IUser
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Nickname { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public bool IsConfirmed { get; set; }
        public string ConfirmationToken { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Company { get; set; }
    }
}
