using UnityEngine;

namespace TimeLine
{
    [TimelineTrackAttribute("Global Track", TimelineTrackGenre.GlobalTrack, TrackItemGenre.GlobalItem)]
    public class GlobalItemTrack : TimelineTrack
    {
        public GlobalEvent[] Events
        {
            get
            {
                return base.GetComponentsInChildren<GlobalEvent>();
            }
        }

        public GlobalAction[] Actions
        {
            get
            {
                return base.GetComponentsInChildren<GlobalAction>();
            }
        }

        public override TimelineItem[] TimelineItems
        {
            get
            {
                GlobalEvent[] events = Events;
                GlobalAction[] actions = Actions;

                TimelineItem[] items = new TimelineItem[events.Length + actions.Length];
                for (int i = 0; i < events.Length; i++)
                {
                    items[i] = events[i];
                }

                for (int i = 0; i < actions.Length; i++)
                {
                    items[i + events.Length] = actions[i];
                }

                return items;
            }
        }
    }
}