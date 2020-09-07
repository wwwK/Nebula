using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using Nebula.Core.Events;
using Nebula.Core.Medias.Player.Events;
using Nebula.Core.Medias.Playlist;
using Nebula.Net.Packets.BOTH;

namespace Nebula.Core.Medias.Player
{
    public class MediaPlayer : Component
    {
        private bool     _repeat;
        private bool     _muted;
        private bool     _shuffle;
        private bool     _manualStop;
        private int      _volume;
        private TimeSpan _lastPosition = TimeSpan.Zero;

        public MediaPlayer()
        {
            NebulaClient.Tick += OnNebulaClientTick;
            _volume = NebulaClient.Settings.General.DefaultSoundLevel;
        }

        public  PlaybackState PlaybackState    => SoundOut?.PlaybackState ?? PlaybackState.Stopped;
        public  TimeSpan      Length           => WaveSource?.GetLength() ?? TimeSpan.Zero;
        public  MediaQueue    Queue            { get; } = new MediaQueue();
        public  IMediaInfo    CurrentMedia     { get; private set; }
        public  IPlaylist     CurrentPlaylist  { get; private set; }
        public  bool          IsPaused         { get; private set; }
        private ISoundOut     SoundOut         { get; set; }
        private IWaveSource   WaveSource       { get; set; }
        private int           VolumeBeforeMute { get; set; }

        public bool Repeat
        {
            get => _repeat;
            set
            {
                _repeat = value;
                RepeatChanged?.Invoke(this, new PlaybackRepeatChangedEventArgs(value));
            }
        }

        public bool Shuffle
        {
            get => _shuffle;
            set
            {
                _shuffle = value;
                ShuffleChanged?.Invoke(this, new PlaybackShuffleChangedEventArgs(value));
            }
        }

        public bool IsMuted
        {
            get => _muted;
            set
            {
                if (value)
                {
                    VolumeBeforeMute = Volume;
                    Volume = 0;
                }
                else
                    Volume = VolumeBeforeMute;

                _muted = value;
                PlaybackMuteChanged?.Invoke(this, new PlaybackMuteChangedEventArgs(value));
            }
        }

        public TimeSpan Position
        {
            get => WaveSource?.GetPosition() ?? TimeSpan.Zero;
            private set => WaveSource?.SetPosition(value);
        }

        public int Volume
        {
            get => _volume;
            set
            {
                int oldVolume = _volume;
                _volume = value < 0 ? 0 : value > 100 ? 100 : value;
                if (SoundOut != null)
                    SoundOut.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
                PlaybackVolumeChanged?.Invoke(this, new PlaybackVolumeChangedEventArgs(oldVolume, Volume));
            }
        }

        public event EventHandler<PlaybackVolumeChangedEventArgs>  PlaybackVolumeChanged;
        public event EventHandler<PlaybackMuteChangedEventArgs>    PlaybackMuteChanged;
        public event EventHandler<TimeSpan>                        PlaybackPositionChanged;
        public event EventHandler<PlaybackPausedEventArgs>         PlaybackPaused;
        public event EventHandler<PlaybackResumedEventArgs>        PlaybackResumed;
        public event EventHandler<PlaybackRepeatChangedEventArgs>  RepeatChanged;
        public event EventHandler<PlaybackShuffleChangedEventArgs> ShuffleChanged;
        public event EventHandler<PlaybackStoppedEventArgs>        PlaybackStopped;
        public event EventHandler<MediaChangingEventArgs>          MediaChanging;
        public event EventHandler<MediaChangedEventArgs>           MediaChanged;

        /// <summary>
        /// Open the specified <see cref="IPlaylist"/>
        /// </summary>
        /// <param name="playlist">Playlist to open</param>
        /// <param name="manualStop">Is this a user manual stop</param>
        /// <param name="play">Should start playing right after init</param>
        public void OpenPlaylist(IPlaylist playlist, bool manualStop = false, bool play = true)
        {
            if (playlist.MediasCount == 0)
                return;
            Queue.Enqueue(playlist);
            CurrentPlaylist = playlist;
            OpenMedia(Queue.Dequeue(Shuffle), manualStop, play);
        }

        /// <summary>
        /// Open the specified <see cref="IMediaInfo"/>
        /// </summary>
        /// <param name="mediaInfo">Media to open</param>
        /// <param name="manualStop">Is this a user manual stop</param>
        /// <param name="play">Should start playing right after init</param>
        /// <returns></returns>
        public async Task OpenMedia(IMediaInfo mediaInfo, bool manualStop = false, bool play = true, bool fromServer = false)
        {
            if (mediaInfo == null)
                return;
            MediaChangingEventArgs mediaChangingEvent = new MediaChangingEventArgs(CurrentMedia, mediaInfo);
            MediaChanging?.Invoke(this, mediaChangingEvent);
            if (mediaChangingEvent.Cancel)
                return;
            if (NebulaClient.SharedSession.IsSessionActive && !fromServer)
            {
                NebulaClient.Network.SendPacket(new SharedSessionPlayMediaPacket
                    {MediaId = mediaInfo.Id, MediaName = mediaInfo.Title, Provider = mediaInfo.GetMediaProvider().Name});
                _manualStop = false;
                return;
            }

            IMediaInfo oldMedia = CurrentMedia;
            _manualStop = manualStop;
            Stop();
            Cleanup();
            Setup(await mediaInfo.GetAudioStreamUri());
            CurrentMedia = mediaInfo;
            MediaChanged?.Invoke(this, new MediaChangedEventArgs(CurrentPlaylist, oldMedia, mediaInfo));
            if (play && !fromServer)
                Play();
            _manualStop = false;
        }

        /// <summary>
        /// Play playback.
        /// </summary>
        public void Play()
        {
            SoundOut?.Play();
        }

        /// <summary>
        /// Pause playback.
        /// </summary>
        public void Pause(bool fromServer = false)
        {
            if (IsPaused)
                return;
            if (NebulaClient.SharedSession.IsSessionActive && !fromServer)
                NebulaClient.Network.SendPacket(new SharedSessionPausePacket());
            SoundOut?.Pause();
            IsPaused = true;
            PlaybackPaused?.Invoke(this, new PlaybackPausedEventArgs());
        }

        /// <summary>
        /// Resume playback.
        /// </summary>
        public void Resume(bool fromServer = false)
        {
            if (!IsPaused)
                return;
            if (NebulaClient.SharedSession.IsSessionActive && !fromServer)
                NebulaClient.Network.SendPacket(new SharedSessionResumePacket());
            SoundOut?.Resume();
            IsPaused = false;
            PlaybackResumed?.Invoke(this, new PlaybackResumedEventArgs());
        }

        /// <summary>
        /// Play the next queued media.
        /// </summary>
        /// <param name="manualStop">Is this a user manual stop</param>
        public void Forward(bool manualStop = false, bool fromServer = false)
        {
            OpenMedia(Queue.Dequeue(Shuffle), manualStop, true, fromServer);
        }

        /// <summary>
        /// Play the previous media.
        /// </summary>
        /// <param name="manualStop">Is this a user manual stop</param>
        public void Backward(bool manualStop = false)
        {
            OpenMedia(Queue.RewindDequeue(), manualStop);
        }

        public void SetPosition(double seconds, bool fromServer = false)
        {
            if (NebulaClient.SharedSession.IsSessionActive && !fromServer)
                NebulaClient.Network.SendPacket(new SharedSessionPositionChangedPacket {NewPosition = seconds});
            Position = TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Stop playback.
        /// </summary>
        public void Stop(bool clearQueue = false)
        {
            Repeat = false;
            if (clearQueue)
                Queue.Clear();
            SoundOut?.Stop();
        }

        /// <summary>
        /// Setup playback.
        /// </summary>
        /// <param name="uri">Uri to media</param>
        private void Setup(Uri uri)
        {
            if (uri.ToString().StartsWith("file") && !File.Exists(uri.LocalPath))
                return;
            WaveSource = CodecFactory.Instance.GetCodec(uri);
            SoundOut = new WasapiOut();
            SoundOut.Initialize(WaveSource);
            SoundOut.Stopped += OnPlaybackStopped;
            SoundOut.Volume = Math.Min(1.0f, Math.Max(Volume / 100f, 0f));
        }

        /// <summary>
        /// Perform playback cleanup.
        /// </summary>
        private void Cleanup()
        {
            if (SoundOut != null)
            {
                //SoundOut.Stopped -= OnPlaybackStopped;
                SoundOut.Dispose();
                SoundOut = null;
            }

            if (WaveSource != null)
            {
                WaveSource.Dispose();
                WaveSource = null;
            }
        }

        private void OnNebulaClientTick(object sender, NebulaAppLoopEventArgs e)
        {
            if (WaveSource != null && _lastPosition != WaveSource.GetPosition())
            {
                _lastPosition = WaveSource.GetPosition();
                PlaybackPositionChanged?.Invoke(this, _lastPosition);
            }
        }

        private void OnPlaybackStopped(object sender, PlaybackStoppedEventArgs e)
        {
            if (Repeat && SoundOut != null && WaveSource != null)
            {
                Position = TimeSpan.Zero;
                Play();
            }
            else if (!_manualStop && !Queue.IsEmpty)
            {
                Forward();
            }
            else
            {
                CurrentPlaylist = null;
            }

            NebulaClient.BeginInvoke(() => PlaybackStopped?.Invoke(this, new PlaybackStoppedEventArgs()));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Cleanup();
        }
    }
}