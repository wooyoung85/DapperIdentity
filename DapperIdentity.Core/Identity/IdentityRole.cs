using Microsoft.AspNet.Identity;

namespace DapperIdentity.Core.Identity
{
    public class IdentityRole : IRole
    {
        public IdentityRole()
        {

        }

        public IdentityRole(string name) : this()
        {
            Name = name;
        }

        public IdentityRole(string id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Role ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Role name
        /// </summary>
        public string Name { get; set; }
    }
}
