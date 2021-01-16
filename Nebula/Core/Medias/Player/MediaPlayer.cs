using System;
using System.Threading.Tasks;
using System.Windows;
using CSCore;
using CSCore.SoundOut;
using Nebula.Core.Events;
using Nebula.Core.Medias.Player.Audio;
using Nebula.Core.Medias.Player.Events;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.Medias.Provider;
using Nebula.Core.Networking;
using Nebula.Core.SharedSessions;
using Nebula.Net.Packets.BOTH;
using Nebula.Net.Packets.C2S;
using Nebula.Net.Packets.S2C;

namespace Nebula.Core.Medias.Player
{
    public class MediaPlayer
    {
        private IMediaInfo _currentMedia;
        private IPlaylist  _currentPlaylist;
        private TimeSpan   _lastPosition = TimeSpan.Zero;
        private bool       _shuffle;
        private bool       _repeat;
        private bool       _stoppedByUSer;
        private bool       _isMuted;
        private int        _volume;
        private int        _volumeBeforeMute;

        public MediaPlayer()
        {
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionPlayMediaPacket, NebulaNetClient>(OnReceivePlayMediaPacket);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionStartPlayingPacket, NebulaNetClient>(OnReceivePlayPacket);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionPausePacket, NebulaNetClient>(OnReceiveSessionPausePacket);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionResumePacket, NebulaNetClient>(OnReceiveSessionResumePacket);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionPositionChangedPacket, NebulaNetClient>(OnReceiveSessionPositionChangedPacket);
            NebulaClient.Tick += OnAppTick;
            Volume = NebulaClient.Settings.General.DefaultSoundLevel;
        }

        public IMediaInfo CurrentMedia
        {
            get => _currentMedia;
            private set
            {
                IMediaInfo oldMedia = _currentMedia;
                _currentMedia = value;
                MediaChanged?.Invoke(this, new MediaChangedEventArgs(null, oldMedia, value));
            }
        }

        public IPlaylist CurrentPlaylist
        {
            get => _currentPlaylist;
            private set
            {
                IPlaylist oldPlaylist = _currentPlaylist;
                _currentPlaylist = value;
                PlaylistChanged?.Invoke(this, new PlaylistChangedEventArgs(oldPlaylist, value));
            }
        }

        public bool Repeat
        {
            get => _repeat;
            set
            {
                if (_repeat == value)
                    return;
                _repeat = value;
                RepeatChanged?.Invoke(this, new RepeatChangedEventArgs(value));
            }
        }

        public bool Shuffle
        {
            get => _shuffle;
            set
            {
                if (_shuffle == value)
                    return;
                _shuffle = value;
                ShuffleChanged?.Invoke(this, new ShuffleChangedEventArgs(value));
            }
        }

        public bool IsMuted
        {
            get => _isMuted;
            set
            {
                if (value)
                    _volumeBeforeMute = Volume;
                Volume = value ? 0 : _volumeBeforeMute;
                _isMuted = value;
                MuteChanged?.Invoke(this, new MuteChangedEventArgs(value));
            }
        }

        public int Volume
        {
            get => _volume;
            set
            {
                int oldVolume = _volume;
                _volume = value < 0 ? 0 : value > 100 ? 100 : value;
                if (SoundOut.IsReady)
                    SoundOut.Out.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(oldVolume, value));
            }
        }

        public TimeSpan Position
        {
            get => SoundOut?.Source.GetPosition() ?? TimeSpan.Zero;
            private set => SoundOut?.Source.SetPosition(value);
        }

        public SoundOut         SoundOut          { get; }              = new SoundOut();
        public MediaQueue       MediaQueue        { get; }              = new MediaQueue();
        public MediaPlayerState State             { get; private set; } = MediaPlayerState.Idle;
        public bool             PlaylistAudioOnly { get; private set; }
        public bool             IsIdle            => State == MediaPlayerState.Idle;
        public bool             IsPreparing       => State == MediaPlayerState.Preparing;
        public bool             IsPaused          => State == MediaPlayerState.Paused;
        public bool             IsPlaying         => State == MediaPlayerState.Playing;
        public TimeSpan         Length            => SoundOut?.Source.GetLength() ?? TimeSpan.Zero;

        public event EventHandler<StateChangedEventArgs>    StateChanged;
        public event EventHandler<MediaChangedEventArgs>    MediaChanged;
        public event EventHandler<PlaylistChangedEventArgs> PlaylistChanged;
        public event EventHandler<MuteChangedEventArgs>     MuteChanged;
        public event EventHandler<VolumeChangedEventArgs>   VolumeChanged;
        public event EventHandler<ShuffleChangedEventArgs>  ShuffleChanged;
        public event EventHandler<RepeatChangedEventArgs>   RepeatChanged;
        public event EventHandler<PositionChangedEventArgs> PositionChanged;

        public void Play(bool fromRemote = false)
        {
            if (IsPlaying || !SoundOut.IsReady)
                return;
            if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
                return;
            _stoppedByUSer = false;
            SoundOut.Out.Play();
            SetState(MediaPlayerState.Playing);
        }

        public void Pause(bool fromRemote = false)
        {
            if (!IsPlaying || !SoundOut.IsReady)
                return;
            if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
            {
                NebulaClient.Network.SendPacket(new SharedSessionPausePacket());
                return;
            }

            SoundOut.Out.Pause();
            SetState(MediaPlayerState.Paused);
        }

        public void Resume(bool fromRemote = false)
        {
            if (IsPlaying || !SoundOut.IsReady)
                return;
            if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
            {
                NebulaClient.Network.SendPacket(new SharedSessionResumePacket());
                return;
            }

            SoundOut.Out.Resume();
            SetState(MediaPlayerState.Playing);
        }

        public void Stop(bool byUser = false)
        {
            if (IsIdle || !SoundOut.IsReady)
                return;
            _stoppedByUSer = byUser;
            SoundOut.Out.Stop();
            SetState(MediaPlayerState.Idle);
        }

        public async Task Forward(bool byUser = false)
        {
            await Open(MediaQueue.Dequeue(Shuffle), byUser);
        }

        public async Task Backward()
        {
        }

        public void SetPosition(double position, bool fromRemote = false)
        {
            if (!SoundOut.IsReady)
                return;
            if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
            {
                NebulaClient.Network.SendPacket(new SharedSessionPositionChangedPacket {NewPosition = position});
                return;
            }

            Position = TimeSpan.FromSeconds(position);
        }

        public async Task OpenPlaylist(IPlaylist playlist, bool audioOnly = true)
        {
            if (playlist == null)
                return;
            CurrentPlaylist = playlist;
            PlaylistAudioOnly = audioOnly;
            MediaQueue.Enqueue(playlist);
            await Open(MediaQueue.Dequeue(Shuffle));
        }

        public async Task Open(IMediaInfo mediaInfo, bool byUser = true, bool fromRemote = false)
        {
            if (mediaInfo == null)
                return;
            try
            {
                if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
                {
                    NebulaClient.Network.SendPacket(new SharedSessionPlayMediaPacket {MediaInfo = mediaInfo.AsMediaInfo(), PlayVideo = false});
                    return;
                }

                SetState(MediaPlayerState.Preparing);
                Stop(byUser);

                SoundOut.Prepare(await mediaInfo.GetAudioStreamUri());
                if (SoundOut.IsReady)
                    SoundOut.Out.Volume = Math.Min(1.0f, Math.Max(Volume / 100f, 0f));
                SoundOut.Out.Stopped += OnMediaStopped;
            }
            catch
            {
                NebulaClient.Notifications.NotifyError("ErrorCantOpenMedia");
                SetState(MediaPlayerState.Idle);
                return;
            }

            CurrentMedia = mediaInfo;
            SetState(MediaPlayerState.Ready);
            Play();
        }

        private void SetState(MediaPlayerState state)
        {
            MediaPlayerState oldState = State;
            State = state;
            StateChanged?.Invoke(this, new StateChangedEventArgs(oldState, state));
        }

        private async void OnMediaStopped(object sender, PlaybackStoppedEventArgs e)
        {
            if (!_stoppedByUSer && !MediaQueue.IsEmpty)
                Forward(true);
        }

        private void OnAppTick(object sender, NebulaAppLoopEventArgs e)
        {
            if (!SoundOut.IsReady)
                return;
            if (Math.Abs(_lastPosition.TotalSeconds - Position.TotalSeconds) >= 1)
            {
                PositionChanged?.Invoke(this, new PositionChangedEventArgs(Position));
                _lastPosition = Position;
            }
        }

        #region Packet Handlers

        private void OnReceivePlayMediaPacket(SharedSessionPlayMediaPacket packet, NebulaNetClient net)
        {
            if (!NebulaClient.SharedSession.IsSessionActive)
                return;
            NebulaClient.BeginInvoke(async () =>
            {
                NebulaClient.SharedSession.AddMessage(new SharedSessionMessage(packet.UserInfo,
                    NebulaClient.GetLocString("SharedSessionMessagePlayMedia", packet.MediaInfo.Title), "#ffee00"));
                IMediaProvider provider = NebulaClient.GetMediaProviderByName(packet.MediaInfo.Provider);
                if (provider == null)
                {
                    NebulaClient.Network.Disconnect();
                    //Todo: Error notification
                    return;
                }

                IMediaInfo mediaInfo = await provider.GetMediaInfo(packet.MediaInfo.Id);
                if (mediaInfo == null)
                    return;
                await Open(mediaInfo, true, true);
                net.SendPacket(new SharedSessionPlayReadyPacket()); //Todo: maybe move in media player class
            });
        }

        private void OnReceivePlayPacket(SharedSessionStartPlayingPacket packet, NebulaNetClient net)
        {
            if (!NebulaClient.SharedSession.IsSessionActive)
                return;
            NebulaClient.BeginInvoke(() => Play(true));
        }

        private void OnReceiveSessionPausePacket(SharedSessionPausePacket packet, NebulaNetClient net)
        {
            if (!NebulaClient.SharedSession.IsSessionActive)
                return;
            NebulaClient.BeginInvoke(() => Pause(true));
        }

        private void OnReceiveSessionResumePacket(SharedSessionResumePacket packet, NebulaNetClient net)
        {
            if (!NebulaClient.SharedSession.IsSessionActive)
                return;
            NebulaClient.BeginInvoke(() => Resume(true));
        }

        private void OnReceiveSessionPositionChangedPacket(SharedSessionPositionChangedPacket packet, NebulaNetClient net)
        {
            if (!NebulaClient.SharedSession.IsSessionActive)
                return;
            NebulaClient.BeginInvoke(() => SetPosition(packet.NewPosition, true));
        }

        #endregion
    }
}