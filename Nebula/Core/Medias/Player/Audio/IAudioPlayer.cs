using System;
using System.Threading.Tasks;
using Nebula.Core.Medias.Player.Events;

namespace Nebula.Core.Medias.Player.Audio
{
    public interface IAudioPlayer
    {
        public int  MinVolume { get; }
        public int  MaxVolume { get; }
        public int  Volume    { get; }
        public bool IsMuted   { get; }

        public event EventHandler<VolumeChangedEventArgs> VolumeChanged;
        public event EventHandler<MuteChangedEventArgs>   MuteChanged;

        public void SetVolume(int volume);
        public void SetMute(bool mute);
        public void Open(Uri uri);
        public Task OpenAsync(Uri uri);
    }
}