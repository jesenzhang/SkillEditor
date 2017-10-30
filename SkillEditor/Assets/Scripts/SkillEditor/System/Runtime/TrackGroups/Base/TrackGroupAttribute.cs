using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace TimeLine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TrackGroupAttribute : Attribute
    {
        //轨道组名称
        private string label;
        public string Label
        {
            get
            {
                return label;
            }
        }
        // 允许的轨道类型列表
        private List<TimelineTrackGenre> trackGenres = new List<TimelineTrackGenre>();

         // 允许的组类型列表
        private List<TimelineGroupGenre> groupGenres = new List<TimelineGroupGenre>();

        /// <summary>
        /// Attribute for Track Groups
        /// </summary>
        /// <param name="label">The name of this track group.</param>
        /// <param name="TrackGenres">The Track Genres that this Track Group is allowed to contain.</param>
        public TrackGroupAttribute(string label, params TimelineTrackGenre[] TrackGenres)
        {
            this.label = label;
            this.trackGenres.AddRange(TrackGenres);
        }
        //返回支持的轨道类型
        public TimelineTrackGenre[] AllowedTrackGenres
        {
            get
            {
                return trackGenres.ToArray();
            }
        }
        //返回支持的组类型
        public TimelineGroupGenre[] AllowedGroupGenres
        {
            get
            {
                return groupGenres.ToArray();
            }
        }
    }
}