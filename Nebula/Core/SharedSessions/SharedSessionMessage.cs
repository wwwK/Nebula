using Nebula.Net.Packets;

namespace Nebula.Core.SharedSessions
{
    public class SharedSessionMessage
    {
        public SharedSessionMessage(UserInfo user, string message, string foreground = "#ffffff")
        {
            User = user;
            Message = message;
            Foreground = foreground;
        }

        public UserInfo User       { get; }
        public string   Message    { get; }
        public string   Foreground { get; }
    }
}