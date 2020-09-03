using Nebula.Server.Users;
using Nebula.Shared.Packets;

namespace Nebula.Server.Extensions
{
    public static class UserExtensions
    {
        public static bool IsServer(this NebulaUser user)
        {
            return user is NebulaServerUser;
        }

        public static string ToInfoPacketString(this NebulaUser user)
        {
            return $"{user.Id}@{user.Name}@{user.ThumbnailUrl}";
        }
    }
}