using System;
using System.Collections.Generic;
using CSCore.SoundOut;
using Nebula.Core.Medias.Playlist;

namespace Nebula.Core.Medias.Player
{
    public class MediaPlayerSession
    {
        public MediaPlayerSession(MediaPlayer mediaPlayer)
        {
            MediaPlayer = mediaPlayer ?? throw new ArgumentNullException(nameof(mediaPlayer));
            MediaPlayer.PlaybackStopped += OnPlaybackStopped;
        }

        public MediaPlayer MediaPlayer { get; }

        /// <summary>
        /// Current Played media
        /// </summary>
        public IMediaInfo CurrentMedia { get; }

        /// <summary>
        /// Current Played playlists
        /// </summary>
        public IPlaylist CurrentPlaylist { get; }

        private List<IMediaInfo> MediaQueue { get; } = new List<IMediaInfo>();

        /// <summary>
        /// True if a media is being played
        /// </summary>
        public bool HasMedia => CurrentMedia != null;

        /// <summary>
        /// True if a playlist is being played
        /// </summary>
        public bool HasPlaylist => CurrentPlaylist != null;

        public void AddMedia(IMediaInfo mediaInfo)
        {
            MediaQueue.Add(mediaInfo);
        }

        public void RemoveMedia(IMediaInfo mediaInfo)
        {
            if (MediaQueue.Contains(mediaInfo))
                MediaQueue.Remove(mediaInfo);
        }

        public IMediaInfo Dequeue(bool random)
        {
            if (MediaQueue.Count == 0)
                return null;
            IMediaInfo mediaInfo = MediaQueue[random ? new Random().Next(MediaQueue.Count) : 0];
            MediaQueue.Remove(mediaInfo);
            return mediaInfo;
        }

        private void OnPlaybackStopped(object sender, PlaybackStoppedEventArgs e)
        {
            IMediaInfo mediaInfo = Dequeue(MediaPlayer.Shuffle);
            if (mediaInfo == null)
                return;
            MediaPlayer.Open(mediaInfo);
        }
    }
}