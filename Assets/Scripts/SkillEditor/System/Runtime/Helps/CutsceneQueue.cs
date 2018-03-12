using System.Collections.Generic;
using UnityEngine;

namespace TimeLine
{
    /// <summary>
    /// Plays through a list of given cutscenes one by one.
    /// </summary>
    public class TimelineQueue : MonoBehaviour
    {
        public List<TimelineManager> TimelineManagers;
        private int index = 0;

        /// <summary>
        /// Play the first cutscene and waits for it to finish
        /// </summary>
        void Start()
        {
            if (TimelineManagers != null && TimelineManagers.Count > 0)
            {
                TimelineManagers[index].TimelineFinished += TimelineQueue_TimelineFinished;
                TimelineManagers[index].Play();
            }
        }

        /// <summary>
        /// On cutscene finish, play the next cutscene.
        /// </summary>
        void TimelineQueue_TimelineFinished(object sender, CutsceneEventArgs e)
        {
            TimelineManagers[index].TimelineFinished -= TimelineQueue_TimelineFinished;
            if (TimelineManagers != null && index + 1 < TimelineManagers.Count)
            {
                index++;
                TimelineManagers[index].Play();
                TimelineManagers[index].TimelineFinished += TimelineQueue_TimelineFinished;
            }
        }
    }
}