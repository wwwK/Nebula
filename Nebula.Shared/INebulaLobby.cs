namespace Nebula.Shared
{
    public interface INebulaLobby
    {
        string Guid         { get; }
        string Name         { get; }
        string ThumbnailUrl { get; }
    }
}