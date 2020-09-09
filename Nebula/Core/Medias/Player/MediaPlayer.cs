using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Nebula.Core.Medias.Player.Events;
using Nebula.Core.Medias.Playlist;
using Nebula.Core.Medias.Provider;
using Nebula.Core.Networking;
using Nebula.Core.SharedSessions;
using Nebula.Net.Packets.BOTH;
using Nebula.Net.Packets.C2S;
using Nebula.Net.Packets.S2C;
using Unosquare.FFME;
using Unosquare.FFME.Common;

namespace Nebula.Core.Medias.Player
{
    public class MediaPlayer
    {
        private IMediaInfo _currentMedia;
        private IPlaylist  _currentPlaylist;
        private bool       _shuffle;
        private bool       _repeat;
        private bool       _stoppedByUSer;
        private double     _volume;

        public MediaPlayer()
        {
            Library.FFmpegDirectory = Path.Combine(NebulaClient.AssemblyDirectory, "ffmpeg", "x64");
            MediaElement = new MediaElement
            {
                Background = new SolidColorBrush(Colors.Black),
                LoadedBehavior = MediaPlaybackState.Manual,
                UnloadedBehavior = MediaPlaybackState.Manual,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Visibility = Visibility.Visible,
                VerticalSyncEnabled = true
            };
            MediaElement.MediaReady += OnMediaReady;
            MediaElement.MediaEnded += OnMediaEnded;
            MediaElement.MediaFailed += OnMediaFailed;
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionPlayMediaPacket, NebulaNetClient>(OnReceivePlayMediaPacket);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionStartPlayingPacket, NebulaNetClient>(OnReceivePlayPacket);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionPausePacket, NebulaNetClient>(OnReceiveSessionPausePacket);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionResumePacket, NebulaNetClient>(OnReceiveSessionResumePacket);
            NebulaClient.Network.PacketProcessor.SubscribeReusable<SharedSessionPositionChangedPacket, NebulaNetClient>(OnReceiveSessionPositionChangedPacket);
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
            get => MediaElement.IsMuted;
            set
            {
                if (MediaElement.IsMuted == value)
                    return;
                MediaElement.IsMuted = value;
                MuteChanged?.Invoke(this, new MuteChangedEventArgs(value));
            }
        }

        public double Volume
        {
            get => _volume;
            set
            {
                double oldVolume = _volume;
                _volume = value < 0 ? 0 : value > 100 ? 100 : value;
                MediaElement.Volume = _volume / 100;
                VolumeChanged?.Invoke(this, new VolumeChangedEventArgs(oldVolume, value));
            }
        }

        public TimeSpan Position
        {
            get => MediaElement.Position;
            private set => MediaElement.Position = value;
        }

        public MediaElement     MediaElement      { get; }
        public MediaQueue       MediaQueue        { get; }              = new MediaQueue();
        public MediaPlayerState State             { get; private set; } = MediaPlayerState.Idle;
        public bool             PlaylistAudioOnly { get; private set; }
        public bool             IsIdle            => State == MediaPlayerState.Idle;
        public bool             IsPreparing       => State == MediaPlayerState.Preparing;
        public bool             IsPaused          => MediaElement.IsPaused;
        public bool             IsPlaying         => MediaElement.IsPlaying;

        public event EventHandler<StateChangedEventArgs>    StateChanged;
        public event EventHandler<MediaChangedEventArgs>    MediaChanged;
        public event EventHandler<PlaylistChangedEventArgs> PlaylistChanged;
        public event EventHandler<MuteChangedEventArgs>     MuteChanged;
        public event EventHandler<VolumeChangedEventArgs>   VolumeChanged;
        public event EventHandler<ShuffleChangedEventArgs>  ShuffleChanged;
        public event EventHandler<RepeatChangedEventArgs>   RepeatChanged;

        public void Play(bool fromRemote = false)
        {
            if (IsPlaying)
                return;
            if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
                return;
            MediaElement.Play();
            SetState(MediaPlayerState.Playing);
        }

        public void Pause(bool fromRemote = false)
        {
            if (!IsPlaying)
                return;
            if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
            {
                NebulaClient.Network.SendPacket(new SharedSessionPausePacket());
                return;
            }

            MediaElement.Pause();
            SetState(MediaPlayerState.Paused);
        }

        public void Resume(bool fromRemote = false)
        {
            if (IsPlaying)
                return;
            if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
            {
                NebulaClient.Network.SendPacket(new SharedSessionResumePacket());
                return;
            }

            MediaElement.Play();
            SetState(MediaPlayerState.Playing);
        }

        public void Stop(bool byUser = false)
        {
            if (!IsPlaying)
                return;
            _stoppedByUSer = byUser;
            MediaElement.Stop();
            SetState(MediaPlayerState.Idle);
        }

        public async Task Forward(bool byUser = false)
        {
            Stop(byUser);
            if (PlaylistAudioOnly)
                await OpenAudioOnly(MediaQueue.Dequeue(Shuffle));
            else
                await Open(MediaQueue.Dequeue(_shuffle), true);
        }

        public async Task Backward()
        {
        }

        public void SetPosition(double position, bool fromRemote = false)
        {
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
            await (audioOnly ? OpenAudioOnly(MediaQueue.Dequeue(Shuffle), true) : Open(MediaQueue.Dequeue(_shuffle), true));
        }

        public async Task Open(IMediaInfo mediaInfo, bool byUser = false, bool fromRemote = false)
        {
            SetState(MediaPlayerState.Preparing);
            try
            {
                Stop(byUser);
                if (mediaInfo.SupportMuxed)
                {
                    if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
                        NebulaClient.Network.SendPacket(new SharedSessionPlayMediaPacket {MediaInfo = mediaInfo.AsMediaInfo(), PlayVideo = false});
                    await MediaElement.Open(await mediaInfo.GetAudioVideoStreamUri());
                }
                else
                {
                    if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
                        NebulaClient.Network.SendPacket(new SharedSessionPlayMediaPacket {MediaInfo = mediaInfo.AsMediaInfo(), PlayVideo = false});
                    await MediaElement.Open(await mediaInfo.GetAudioVideoStreamUri());
                    //Todo: find a way to play audio and video together.
                    //Possibilities:
                    // - Playing audio at same time with another player 
                    // - Writing audio data while playing video
                    // - Merge two streams together via ffmpeg ( don't know if current wrapper lib support that)
                }
            }
            catch
            {
                NebulaClient.Notifications.NotifyError("ErrorCantOpenMedia");
                _stoppedByUSer = false;
                SetState(MediaPlayerState.Idle);
                return;
            }

            CurrentMedia = mediaInfo;
            Play();
            SetState(MediaPlayerState.Ready);
        }

        public async Task OpenAudioOnly(IMediaInfo mediaInfo, bool byUser = true, bool fromRemote = false)
        {
            SetState(MediaPlayerState.Preparing);
            try
            {
                if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
                    NebulaClient.Network.SendPacket(new SharedSessionPlayMediaPacket {MediaInfo = mediaInfo.AsMediaInfo(), PlayVideo = false});
                Stop(byUser);
                await MediaElement.Open(await mediaInfo.GetAudioStreamUri());
            }
            catch
            {
                NebulaClient.Notifications.NotifyError("ErrorCantOpenMedia");
                _stoppedByUSer = false;
                SetState(MediaPlayerState.Idle);
                return;
            }

            CurrentMedia = mediaInfo;
            Play();
            SetState(MediaPlayerState.Ready);
        }

        public async Task OpenVideoOnly(IMediaInfo mediaInfo, bool byUser = true, bool fromRemote = false)
        {
            SetState(MediaPlayerState.Preparing);
            try
            {
                if (NebulaClient.SharedSession.IsSessionActive && !fromRemote)
                    NebulaClient.Network.SendPacket(new SharedSessionPlayMediaPacket {MediaInfo = mediaInfo.AsMediaInfo(), PlayVideo = true});
                Stop(byUser);
                await MediaElement.Open(await mediaInfo.GetVideoStreamUri());
            }
            catch
            {
                NebulaClient.Notifications.NotifyError("ErrorCantOpenMedia");
                _stoppedByUSer = false;
                SetState(MediaPlayerState.Idle);
                return;
            }

            CurrentMedia = mediaInfo;
            Play();
            SetState(MediaPlayerState.Ready);
        }

        private void SetState(MediaPlayerState state)
        {
            MediaPlayerState oldState = State;
            State = state;
            StateChanged?.Invoke(this, new StateChangedEventArgs(oldState, state));
        }

        private void OnMediaFailed(object sender, MediaFailedEventArgs e)
        {
            NebulaClient.Notifications.NotifyError(e.ErrorException.Message);
        }

        private void OnMediaReady(object sender, EventArgs e)
        {
            MediaElement.Volume = Volume / 100;
            _stoppedByUSer = false;
            if (NebulaClient.SharedSession.IsSessionActive)
                NebulaClient.Network.SendPacket(new SharedSessionPlayReadyPacket());
        }

        private async void OnMediaEnded(object sender, EventArgs e)
        {
            if (!_stoppedByUSer && !MediaQueue.IsEmpty)
                await Forward();
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
                await OpenAudioOnly(mediaInfo, true, true);
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