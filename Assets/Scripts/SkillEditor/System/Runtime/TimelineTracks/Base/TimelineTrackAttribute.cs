// Cinema Suite 2014
using System;
using System.Collections.Generic;

namespace TimeLine
{
    /// <summary>
    /// The Attribute for tracks.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TimelineTrackAttribute : Attribute
    {
        // The user friendly name for this Track.
        private string label; 

        // The genres of this track.
        private List<TimelineTrackGenre> trackGenres = new List<TimelineTrackGenre>();

        // The genres of items that this track can contain.
        private List<TrackItemGenre> itemGenres = new List<TrackItemGenre>();

        /// <summary>
        /// Attribute for Track Groups
        /// </summary>
        /// <param name="label">The name of this track group.</param>
        /// <param name="TrackGenres">The Genres of this track.</param>
        /// <param name="AllowedItemGenres">The Genres allowed to be contained in this track.</param>
        public TimelineTrackAttribute(string label, TimelineTrackGenre[] TrackGenres, params TrackItemGenre[] AllowedItemGenres)
        {
            this.label = label;
            this.trackGenres.AddRange(TrackGenres);
            this.itemGenres.AddRange(AllowedItemGenres);
        }

        /// <summary>
        /// Attribute for Track Groups.
        /// </summary>
        /// <param name="label">The name of this track group.</param>
        /// <param name="TrackGenres">The Genre of this track.</param>
        /// <param name="AllowedItemGenres">The Genres allowed to be contained in this track.</param>
        public TimelineTrackAttribute(string label, TimelineTrackGenre TrackGenre, params TrackItemGenre[] AllowedItemGenres)
        {
            this.label = label;
            this.trackGenres.Add(TrackGenre);
            this.itemGenres.AddRange(AllowedItemGenres);
        }

        /// <summary>
        /// The label of this track.
        /// </summary>
        public string Label
        {
            get
            {
                return label;
            }
        }

        /// <summary>
        /// The genres of this Track.
        /// </summary>
        public TimelineTrackGenre[] TrackGenres
        {
            get
            {
                return trackGenres.ToArray();
            }
        }

        /// <summary>
        /// The allowed item genres for this track.
        /// </summary>
        public TrackItemGenre[] AllowedItemGenres
        {
            get
            {
                return itemGenres.ToArray();
            }
        }
    }
}