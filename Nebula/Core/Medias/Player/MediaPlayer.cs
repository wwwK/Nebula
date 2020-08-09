using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using CSCore.Streams.Effects;
using Nebula.Core.Events;
using Nebula.Core.Medias.Player.Events;
using Nebula.Core.Medias.Playlist;

namespace Nebula.Core.Medias.Player
{
    public class MediaPlayer : Component
    {
        private bool     _repeat       = false;
        private bool     _muted        = false;
        private bool     _shuffle      = false;
        private bool     _manualStop   = false;
        private int      _volume       = 50;
        private TimeSpan _lastPosition = TimeSpan.Zero;

        public MediaPlayer()
        {
            NebulaClient.Tick += NebulaClientOnTick;
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
            set => WaveSource?.SetPosition(value);
        }

        public int Volume
        {
            get => _volume;
            set
            {
                int oldVolume = _volume;
                _volume = value;
                if (SoundOut != null)
                    SoundOut.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
                PlaybackVolumeChanged?.Invoke(this, new PlaybackVolumeChangedEventArgs(oldVolume, Volume));
            }
        }

        public event EventHandler<PlaybackVolumeChangedEventArgs>  PlaybackVolumeChanged;
        public event EventHandler<PlaybackMuteChangedEventArgs>    PlaybackMuteChanged;
        public event EventHandler<TimeSpan>                        PlaybackPositionChanged;
        public event EventHandler<PlaybackRepeatChangedEventArgs>  RepeatChanged;
        public event EventHandler<PlaybackShuffleChangedEventArgs> ShuffleChanged;
        public event EventHandler<PlaybackStoppedEventArgs>        PlaybackStopped;
        public event EventHandler<MediaChangingEventArgs>          MediaChanging;
        public event EventHandler<MediaChangedEventArgs>           MediaChanged;

        public void OpenPlaylist(IPlaylist playlist, bool manualStop = false, bool play = true)
        {
            if (playlist.MediasCount == 0)
                return;
            Queue.Clear();
            CurrentPlaylist = playlist;
            foreach (IMediaInfo mediaInfo in playlist)
                Queue.Enqueue(mediaInfo);
            Open(Queue.Dequeue(Shuffle), manualStop, play);
        }

        public async Task Open(IMediaInfo mediaInfo, bool manualStop = false, bool play = true)
        {
            if (mediaInfo == null)
                return;
            _manualStop = manualStop;
            MediaChangingEventArgs mediaChangingEvent = new MediaChangingEventArgs(CurrentMedia, mediaInfo);
            MediaChanging?.Invoke(this, mediaChangingEvent);
            if (mediaChangingEvent.Cancel)
                return;
            IMediaInfo oldMedia = CurrentMedia;
            Stop();
            Cleanup();
            WaveSource = CodecFactory.Instance.GetCodec(await mediaInfo.GetStreamUri());
            SoundOut = new WasapiOut();
            SoundOut.Initialize(WaveSource);
            SoundOut.Stopped += OnPlaybackStopped;
            SoundOut.Volume = Math.Min(1.0f, Math.Max(Volume / 100f, 0f));
            CurrentMedia = mediaInfo;
            MediaChanged?.Invoke(this, new MediaChangedEventArgs(oldMedia, mediaInfo));
            if (play)
                Play();
            manualStop = false;
        }

        public void Play()
        {
            SoundOut?.Play();
        }

        public void Pause()
        {
            if (IsPaused)
                return;
            SoundOut?.Pause();
            IsPaused = true;
        }

        public void Resume()
        {
            if (!IsPaused)
                return;
            SoundOut?.Resume();
            IsPaused = false;
        }

        public void Forward()
        {
            if (Queue.IsEmpty)
                return;
            Open(Queue.Dequeue(Shuffle), true);
        }

        public void Stop()
        {
            SoundOut?.Stop();
            Repeat = false;
        }

        private void Cleanup()
        {
            if (SoundOut != null)
            {
                SoundOut.Dispose();
                SoundOut = null;
            }

            if (WaveSource != null)
            {
                WaveSource.Dispose();
                WaveSource = null;
            }
        }

        private void NebulaClientOnTick(object sender, NebulaAppLoopEventArgs e)
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