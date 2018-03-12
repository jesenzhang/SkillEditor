using System;

using System.Collections.Generic;
using UnityEngine;

namespace TimeLine
{
    /// <summary>
    /// A track which maintains all timeline items marked for actor tracks and multi actor tracks.
    /// </summary>
    [TimelineTrackAttribute("Actor Track", new TimelineTrackGenre[] { TimelineTrackGenre.ActorTrack, TimelineTrackGenre.MultiActorTrack }, TrackItemGenre.ActorItem)]
    public class ActorItemTrack : TimelineTrack, IActorTrack
    {
        /// <summary>
        /// Initialize this Track and all the timeline items contained within.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0; i < this.ActorEvents.Length; i++)
            {
                
                    if (Actor != null)
                    {
                        this.ActorEvents[i].Initialize(Actor.gameObject);
                    }
                
            }
        }

        /// <summary>
        /// The cutscene has been set to an arbitrary time by the user.
        /// Processing must take place to catch up to the new time.
        /// </summary>
        /// <param name="time">The new cutscene running time</param>
        public override void SetTime(float time)
        {
            float previousTime = elapsedTime;
            base.SetTime(time);

            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                // Check if it is an actor event.
                ActorEvent cinemaEvent = items[i] as ActorEvent;
                if (cinemaEvent != null)
                {
                    if ((previousTime < cinemaEvent.Firetime && time >= cinemaEvent.Firetime) || (cinemaEvent.Firetime == 0f && previousTime <= cinemaEvent.Firetime && time > cinemaEvent.Firetime))
                    {
                       
                            if (Actor != null)
                            {
                                cinemaEvent.Trigger(Actor.gameObject);
                            }
                        
                    }
                    else if (previousTime > cinemaEvent.Firetime && time <= cinemaEvent.Firetime)
                    {
                       
                            if (Actor != null)
                                cinemaEvent.Reverse(Actor.gameObject);
                        
                    }
                }

                // Check if it is an actor action.
                ActorAction action = items[i] as ActorAction;
                if (action != null)
                {
                    
                        if (Actor != null)
                        {
                                action.SetTime(Actor.gameObject, (time - action.Firetime), time - previousTime);
                        }
                    
                }
            }
        }

        /// <summary>
        /// Update this track since the last frame.
        /// </summary>
        /// <param name="time">The new running time.</param>
        /// <param name="deltaTime">The deltaTime since last update.</param>
        public override void UpdateTrack(float time, float deltaTime)
        {
            if (mute)
                return;
            float previousTime = base.elapsedTime;
            base.UpdateTrack(time, deltaTime);

            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                // Check if it is an actor event.
                ActorEvent cinemaEvent = items[i] as ActorEvent;
                if (cinemaEvent != null)
                {
                    if ((previousTime < cinemaEvent.Firetime && time >= cinemaEvent.Firetime) || (cinemaEvent.Firetime == 0f && previousTime <= cinemaEvent.Firetime && time > cinemaEvent.Firetime))
                    {
                        
                            if (Actor != null)
                                cinemaEvent.Trigger(Actor.gameObject);
                        
                    }
                    else if (previousTime >= cinemaEvent.Firetime && base.elapsedTime <= cinemaEvent.Firetime)
                    {
                        if (Actor != null)
                           cinemaEvent.Reverse(Actor.gameObject);
                    }
                }

                ActorAction action = items[i] as ActorAction;
                if (action != null)
                {
                    if (((previousTime < action.Firetime || previousTime <= 0f) && base.elapsedTime >= action.Firetime) && base.elapsedTime < action.EndTime)
                    {
                       
                            if (Actor != null)
                            {
                                action.Trigger(Actor.gameObject);
                            }
                        
                    }
                    else if (previousTime <= action.EndTime && base.elapsedTime >= action.EndTime)
                    {
                       
                            if (Actor != null)
                            {
                                action.End(Actor.gameObject);
                            }
                        
                    }
                    else if (previousTime >= action.Firetime && previousTime < action.EndTime && base.elapsedTime <= action.Firetime)
                    {
                       
                            if (Actor != null)
                            {
                                action.ReverseTrigger(Actor.gameObject);
                            }
                        
                    }
                    else if (((previousTime > action.EndTime || previousTime >= action.Manager.Duration) && (base.elapsedTime > action.Firetime) && (base.elapsedTime <= action.EndTime)))
                    {
                       
                            if (Actor != null)
                            {
                                action.ReverseEnd(Actor.gameObject);
                            }
                        
                    }
                    else if ((base.elapsedTime > action.Firetime) && (base.elapsedTime <= action.EndTime))
                    {
                     
                            if (Actor != null)
                            {
                                float runningTime = time - action.Firetime;
                                action.UpdateTime(Actor.gameObject, runningTime, deltaTime);
                            }
                        
                    }
                }
            }
        }

        /// <summary>
        /// Pause playback while being played.
        /// </summary>
        public override void Pause()
        {
            base.Pause();
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                ActorAction action = items[i] as ActorAction;
                if (action != null)
                {
                    if (((elapsedTime > action.Firetime)) && (elapsedTime < (action.Firetime + action.Duration)))
                    {
                        
                            if (Actor != null)
                            {
                                action.Pause(Actor.gameObject);
                            }
                        
                    }
                }
            }
        }

        /// <summary>
        /// Resume playback after being paused.
        /// </summary>
        public override void Resume()
        {
            base.Resume();
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                ActorAction action = items[i] as ActorAction;
                if (action != null)
                {
                    if (((elapsedTime > action.Firetime)) && (elapsedTime < (action.Firetime + action.Duration)))
                    {
                       
                            if (Actor != null)
                            {
                                action.Resume(Actor.gameObject);
                            }
                        
                    }
                }
            }
        }

        /// <summary>
        /// Stop the playback of this track.
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            base.elapsedTime = 0f;
            TimelineItem[] items = GetTimelineItems();
            for (int i = 0; i < items.Length; i++)
            {
                ActorEvent cinemaEvent = items[i] as ActorEvent;
                if (cinemaEvent != null)
                {
                  
                        if (Actor != null)
                            cinemaEvent.Stop(Actor.gameObject);
                    
                }
            
                ActorAction action = items[i] as ActorAction;
                if (action != null)
                {
                   
                        if (Actor != null)
                            action.Stop(Actor.gameObject);
                    
                }
            }
        }

        /// <summary>
        /// Get the Actor associated with this track. Can return null.
        /// </summary>
        public Transform Actor
        {
            get
            {
                ActorTrackGroup atg = this.TrackGroup as ActorTrackGroup;
                if (atg == null)
                {
                    Debug.LogError("No ActorTrackGroup found on parent.", this);
                    return null;
                }
                return atg.Actor;
            }
        }
      

        public ActorEvent[] ActorEvents
        {
            get
            {
                return base.GetComponentsInChildren<ActorEvent>();
            }
        }

        public ActorAction[] ActorActions
        {
            get
            {
                return base.GetComponentsInChildren<ActorAction>();
            }
        }
    }
}