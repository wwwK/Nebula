using System;
using LiteNetLib.Utils;

namespace Nebula.Net.Packets
{
    public struct SharedSessionInfo : INetSerializable
    {
        public Guid   Id                { get; set; }
        public string Name              { get; set; }
        public int    CurrentUsers      { get; set; }
        public int    MaximumUsers      { get; set; } //Server ignore : Uneditable by client
        public bool   PasswordProtected { get; set; } //Server ignore : Uneditable by client

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(Id.ToString());
            writer.Put(Name);
            writer.Put(CurrentUsers);
            writer.Put(MaximumUsers);
            writer.Put(PasswordProtected);
        }

        public void Deserialize(NetDataReader reader)
        {
            Id = Guid.Parse(reader.GetString());
            Name = reader.GetString();
            CurrentUsers = reader.GetInt();
            MaximumUsers = reader.GetInt();
            PasswordProtected = reader.GetBool();
        }

        public static SharedSessionInfo Empty => new SharedSessionInfo
            {Id = Guid.Empty, Name = String.Empty, CurrentUsers = 0, MaximumUsers = 0, PasswordProtected = false};

        public static bool operator ==(SharedSessionInfo session1, SharedSessionInfo session2)
        {
            return session1.Id == session2.Id;
        }

        public static bool operator !=(SharedSessionInfo session1, SharedSessionInfo session2)
        {
            return session1.Id != session2.Id;
        }
    }
}