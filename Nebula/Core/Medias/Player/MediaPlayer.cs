﻿using System;
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

namespace Nebula.Core.Medias.Player
{
    public class MediaPlayer : Component
    {
        private bool     _repeat       = false;
        private bool     _muted        = false;
        private bool     _shuffle      = false;
        private TimeSpan _lastPosition = TimeSpan.Zero;

        public MediaPlayer()
        {
            Session = new MediaPlayerSession(this);
            NebulaClient.Tick += NebulaClientOnTick;
        }

        private ISoundOut   SoundOut   { get; set; }
        private IWaveSource WaveSource { get; set; }

        private int VolumeBeforeMute { get; set; }

        public PlaybackState PlaybackState => SoundOut?.PlaybackState ?? PlaybackState.Stopped;

        public TimeSpan Length => WaveSource?.GetLength() ?? TimeSpan.Zero;

        public MediaPlayerSession Session { get; }

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

        public event EventHandler<PlaybackVolumeChangedEventArgs>  PlaybackVolumeChanged;
        public event EventHandler<PlaybackMuteChangedEventArgs>    PlaybackMuteChanged;
        public event EventHandler<TimeSpan>                        PlaybackPositionChanged;
        public event EventHandler<PlaybackRepeatChangedEventArgs>  RepeatChanged;
        public event EventHandler<PlaybackShuffleChangedEventArgs> ShuffleChanged;
        public event EventHandler<PlaybackStoppedEventArgs>        PlaybackStopped;
        public event EventHandler<MediaChangingEventArgs>          MediaChanging;
        public event EventHandler<MediaChangedEventArgs>           MediaChanged;

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
            SoundOut = new WasapiOut();
            SoundOut.Initialize(WaveSource);
            SoundOut.Stopped += OnPlaybackStopped;
            CurrentMedia = mediaInfo;
            MediaChanged?.Invoke(this, new MediaChangedEventArgs(oldMedia, mediaInfo));
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
                return;
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