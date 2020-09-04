using Nebula.Server.Users;

namespace Nebula.Server.Extensions
{
    public static class UserExtensions
    {
        public static bool IsServer(this NebulaUser user)
        {
            return user is NebulaServerUser;
        }
    }
}