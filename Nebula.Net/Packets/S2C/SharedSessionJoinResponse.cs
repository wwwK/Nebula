using System;

namespace Nebula.Net.Packets.S2C
{
    public class SharedSessionJoinResponse
    {
        public SharedSessionInfo Session      { get; set; } = SharedSessionInfo.Empty;
        public UserInfo[]        Users        { get; set; } = Array.Empty<UserInfo>();
        public int               ResponseCode { get; set; }
        public bool              Success      => ResponseCode == 0;
    }
}