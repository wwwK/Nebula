using System;
using System.Threading.Tasks;
using CSCore;
using CSCore.SoundOut;
using Nebula.Core.Medias.Player.Events;

namespace Nebula.Core.Medias.Player.Audio
{
    public class AudioPlayer : IAudioPlayer
    {
        public AudioPlayer()
        {
            SetVolume(NebulaClient.Settings.General.DefaultSoundLevel);
        }

        public  IMediaInfo       PlayedMedia      { get; private set; }
        public  MediaPlayerState State            { get; private set; } = MediaPlayerState.Idle;
        public  SoundOut         SoundOut         { get; }              = new SoundOut();
        public  int              MinVolume        { get; }              = 0;
        public  int              MaxVolume        { get; }              = 100;
        private int              VolumeBeforeMute { get; set; }
        public  int              Volume           { get; private set; }
        public  bool             IsMuted          { get; private set; }
        public  TimeSpan         Position         => SoundOut.Source.GetPosition();
        public  bool             IsPaused         => State == MediaPlayerState.Paused;
        public  bool             IsPlaying        => State == MediaPlayerState.Playing;
        public  bool             IsIdle           => State == MediaPlayerState.Idle;
        public  bool             IsPreparing      => State == MediaPlayerState.Preparing;

        public event EventHandler<VolumeChangedEventArgs> VolumeChanged;
        public event EventHandler<MuteChangedEventArgs>   MuteChanged;
        public event EventHandler<StateChangedEventArgs>  StateChanged;

        public void SetVolume(int volume)
        {
            if (Volume == volume)
                return;
            int oldVolume = Volume;
            Volume = volume < MinVolume ? MinVolume : volume > MaxVolume ? MaxVolume : volume;
            if (SoundOut.IsReady)
                SoundOut.Out.Volume = Math.Min(1.0f, Math.Max(volume / 100f, 0f));
            VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(oldVolume, volume));
        }

        public void SetMute(bool mute)
        {
            if (mute == IsMuted)
                return;
            if (mute)
            {
                VolumeBeforeMute = Volume;
                Volume = MinVolume;
            }
            else
                Volume = VolumeBeforeMute;

            IsMuted = mute;
            MuteChanged?.Invoke(this, new MuteChangedEventArgs(mute));
        }

        public void Play()
        {
            if (!SoundOut.IsReady)
                return;
            SoundOut.Out.Play();
        }

        public void Stop()
        {
            if (!SoundOut.IsReady)
                return;
            SoundOut.Out.Stop();
        }

        public void SetPosition(TimeSpan position)
        {
            if (!SoundOut.IsReady || !IsPlaying || !IsPaused)
                return;
            SoundOut.Source.SetPosition(position);
        }

        public void Pause()
        {
            if (!SoundOut.IsReady || IsPaused)
                return;
            SoundOut.Out.Pause();
            SetState(MediaPlayerState.Paused);
        }

        public void Resume()
        {
            if (!SoundOut.IsReady || !IsPaused)
                return;
            SoundOut.Out.Resume();
            SetState(MediaPlayerState.Playing);
        }

        public void Open(Uri uri)
        {
            SoundOut.Prepare(uri);
            if (!SoundOut.IsReady)
            {
                SetState(MediaPlayerState.Idle);
                return;
            }
        }

        public async Task OpenAsync(Uri uri)
        {
            await Task.Run(() => Open(uri));
        }

        private void SetState(MediaPlayerState state)
        {
            MediaPlayerState oldState = State;
            State = state;
            StateChanged?.Invoke(this, new StateChangedEventArgs(oldState, state));
        }

        private void OnPlaybackStopped(object sender, PlaybackStoppedEventArgs e)
        {
            PlayedMedia = null;
            SetState(MediaPlayerState.Idle);
        }
    }
}