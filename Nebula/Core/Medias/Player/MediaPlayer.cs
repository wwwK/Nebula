using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CSCore;
using CSCore.Codecs;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using CSCore.Streams.Effects;
using Nebula.Core.Medias.Player.Events;

namespace Nebula.Core.Medias.Player
{
    public class MediaPlayer : Component
    {
        private bool _repeat = false;
        private bool _muted  = false;

        public MediaPlayer()
        {
        }

        private ISoundOut   SoundOut   { get; set; }
        private IWaveSource WaveSource { get; set; }

        private int VolumeBeforeMute { get; set; }

        private CancellationTokenSource CancellationToken { get; set; } = null;

        public PlaybackState PlaybackState => SoundOut?.PlaybackState ?? PlaybackState.Stopped;

        public TimeSpan Length => WaveSource?.GetLength() ?? TimeSpan.Zero;

        public IMediaInfo CurrentMedia { get; private set; }

        public bool IsPaused { get; private set; } = false;

        public bool Repeat
        {
            get => _repeat;
            set
            {
                _repeat = value;
                RepeatChanged?.Invoke(this, new PlaybackRepeatChangedEventArgs(value));
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
            get => SoundOut != null ? Math.Min(100, Math.Max((int) (SoundOut.Volume * 100), 0)) : 100;
            set
            {
                if (SoundOut == null)
                    return;
                int oldVolume = Volume;
                SoundOut.Volume = Math.Min(1.0f, Math.Max(value / 100f, 0f));
                PlaybackVolumeChanged?.Invoke(this, new PlaybackVolumeChangedEventArgs(oldVolume, Volume));
            }
        }

        public event EventHandler<PlaybackVolumeChangedEventArgs> PlaybackVolumeChanged;
        public event EventHandler<PlaybackMuteChangedEventArgs>   PlaybackMuteChanged;
        public event EventHandler<TimeSpan>                       PlaybackPositionChanged;
        public event EventHandler<PlaybackRepeatChangedEventArgs> RepeatChanged;
        public event EventHandler<PlaybackStoppedEventArgs>       PlaybackStopped;
        public event EventHandler<MediaChangingEventArgs>         MediaChanging;
        public event EventHandler<MediaChangedEventArgs>          MediaChanged;

        private async void EventBusLoop()
        {
            CancellationToken = new CancellationTokenSource();
            TimeSpan position = TimeSpan.Zero;
            while (true)
            {
                if (CancellationToken == null || CancellationToken.IsCancellationRequested)
                    break;
                await Task.Delay(1000);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (WaveSource != null && position != WaveSource.GetPosition())
                    {
                        position = WaveSource.GetPosition();
                        PlaybackPositionChanged?.Invoke(this, position);
                    }
                });
            }

            CancellationToken = null;
        }

        public async void Open(IMediaInfo mediaInfo, bool play = true)
        {
            MediaChangingEventArgs mediaChangingEvent = new MediaChangingEventArgs(CurrentMedia, mediaInfo);
            MediaChanging?.Invoke(this, mediaChangingEvent);
            if (mediaChangingEvent.Cancel)
                return;
            IMediaInfo oldMedia = CurrentMedia;
            Repeat = false;
            Cleanup();
            WaveSource = CodecFactory.Instance.GetCodec(await mediaInfo.GetStreamUri());
            SoundOut = new WaveOut();
            SoundOut.Initialize(WaveSource);
            SoundOut.Stopped += OnPlaybackStopped;
            CurrentMedia = mediaInfo;
            MediaChanged?.Invoke(this, new MediaChangedEventArgs(oldMedia, mediaInfo));
            Task.Run(EventBusLoop);
            if (play)
                Play();
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

        private void OnPlaybackStopped(object sender, PlaybackStoppedEventArgs e)
        {
            if (Repeat && SoundOut != null && WaveSource != null)
            {
                Position = TimeSpan.Zero;
                Play();
                return;
            }

            CancellationToken?.Cancel();
            PlaybackStopped?.Invoke(this, e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Cleanup();
        }
    }
}