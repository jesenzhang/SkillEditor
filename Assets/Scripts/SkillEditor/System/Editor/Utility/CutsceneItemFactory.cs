using TimeLine;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// A factory for creating all the game objects associated with a cutscene.
/// </summary>
public class CutsceneItemFactory
{
    private const string DIRECTOR_GROUP_NAME = "Director Group";
    private const string ACTOR_GROUP_NAME = "Actor Group";
    private const string MULTI_ACTOR_GROUP_NAME = "Multi Actor Group";
    private const string SHOT_NAME_DEFAULT = "Shot";
    private const string AUDIO_CLIP_NAME_DEFAULT = "Audio Clip";
    private const string CURVE_CLIP_NAME_DEFAULT = "Curve Clip";
    private const string SHOT_TRACK_LABEL = "Shot Track";
    private const string AUDIO_TRACK_LABEL = "Audio Track";
    private const string GLOBAL_TRACK_LABEL = "Global Track";
    private const string CURVE_TRACK_LABEL = "Curve Track";
    private const string EVENT_TRACK_LABEL = "Actor Track";

    private const float DEFAULT_SHOT_LENGTH = 5f;
    private const float DEFAULT_GLOBAL_ACTION_LENGTH = 5f;
    private const float DEFAULT_ACTOR_ACTION_LENGTH = 5f;
    private const float DEFAULT_CURVE_LENGTH = 5f;

    /// <summary>
    /// Create a new Track Group.
    /// </summary>
    /// <param name="cutscene">The cutscene that this Track Group will be attached to.</param>
    /// <param name="type">The type of the new track group.</param>
    /// <param name="label">The name of the new track group.</param>
    /// <returns>The new track group. Reminder: Register an Undo.</returns>
    public static TrackGroup CreateTrackGroup(TimelineManager cutscene, Type type, string label)
    {
        // Create the director group.
        GameObject trackGroupGO = new GameObject(label, type);
        trackGroupGO.transform.parent = cutscene.transform;
        return trackGroupGO.GetComponent<TrackGroup>();
    }
    public static TrackGroup CreateSubTrackGroup(TrackGroup Parent, Type type, string label)
    {
        // Create the director group.
        GameObject trackGroupGO = new GameObject(label, type);
        trackGroupGO.transform.parent = Parent.transform;
        return trackGroupGO.GetComponent<TrackGroup>();
    }
    /// <summary>
    /// Create a new Track.
    /// </summary>
    /// <param name="trackGroup">The track group that this track will be attached to.</param>
    /// <param name="type">The type of the new track.</param>
    /// <param name="label">The name of the new track.</param>
    /// <returns>The newly created track. Reminder: Register an Undo.</returns>
    internal static TimelineTrack CreateTimelineTrack(TrackGroup trackGroup, Type type, string label)
    {
        GameObject timelineTrackGO = new GameObject(label, type);
        timelineTrackGO.transform.parent = trackGroup.transform;
        return timelineTrackGO.GetComponent<TimelineTrack>();
    }

    /// <summary>
    /// Create a new Timeline Item (Cutscene Item)
    /// </summary>
    /// <param name="timelineTrack">The track that this item will be attached to.</param>
    /// <param name="type">the type of the new item.</param>
    /// <param name="label">The name of the new item.</param>
    /// <returns>The newly created Cutscene Item. Reminder: Register an Undo.</returns>
    internal static TimelineItem CreateCutsceneItem(TimelineTrack timelineTrack, Type type, string label, float firetime)
    {
        GameObject itemGO = new GameObject(label, type);
        TimelineItem ti = itemGO.GetComponent<TimelineItem>();
        ti.SetDefaults();

        // Find an appropriate firetime/duration for it.
        if (type.IsSubclassOf(typeof(TimelineActionFixed)))
        {
            // The new timeline item is an action of fixed length.
            TimelineActionFixed newAction = ti as TimelineActionFixed;

            SortedDictionary<float, TimelineActionFixed> sortedClips = new SortedDictionary<float, TimelineActionFixed>();
            foreach (TimelineItem current in timelineTrack.TimelineItems)
            {
                TimelineActionFixed action = current as TimelineActionFixed;
                if (action == null) continue;
                sortedClips.Add(action.Firetime, action);
            }

            float latestTime = firetime;
            float length = newAction.ItemLength;
            foreach (TimelineActionFixed a in sortedClips.Values)
            {
                if (!(latestTime < a.Firetime && latestTime + length <= a.Firetime))
                {
                    latestTime = a.Firetime + a.Duration;
                }
            }

            newAction.Firetime = latestTime;
        }
        else if (type.IsSubclassOf(typeof(TimelineAction)))
        {
            // The new timeline item is an action with arbitrary length.
            TimelineAction newAction = ti as TimelineAction;

            SortedDictionary<float, TimelineAction> sortedActions = new SortedDictionary<float, TimelineAction>();
            foreach (TimelineItem current in timelineTrack.TimelineItems)
            {
                TimelineAction action = current as TimelineAction;
                if (action == null) continue;
                sortedActions.Add(action.Firetime, action);
            }

            float latestTime = firetime;
            float length = newAction.Duration;
            foreach (TimelineAction a in sortedActions.Values)
            {
                if (latestTime >= a.Firetime)
                {
                    latestTime = Mathf.Max(latestTime, a.Firetime + a.Duration);
                }
                else
                {
                    length = a.Firetime - latestTime;
                    break;
                }
            }

            newAction.Firetime = latestTime;
            newAction.Duration = length;
        }
        else
        {
            ti.Firetime = firetime;
        }

        itemGO.transform.parent = timelineTrack.transform;
        timelineTrack.Manager.recache();
        return ti;
    }

    /// <summary>
    /// Create a new Timeline Item (Cutscene Item)
    /// </summary>
    /// <param name="timelineTrack">The track that this item will be attached to.</param>
    /// <param name="type">the type of the new item.</param>
    /// <param name="label">The name of the new item.</param>
    /// <param name="PairedObject">The paired object of this TimelineItem. (Ex: AudioClip/Animation)</param>
    /// <returns>The newly created Cutscene Item. Reminder: Register an Undo.</returns>
    internal static TimelineItem CreateCutsceneItem(TimelineTrack timelineTrack, Type type, string label, UnityEngine.Object PairedObject, float firetime)
    {
        GameObject itemGO = new GameObject(PairedObject.name, type);
        TimelineItem ti = itemGO.GetComponent<TimelineItem>();
        ti.SetDefaults(PairedObject);

        // Find an appropriate firetime/duration for it.
        if (type.IsSubclassOf(typeof(TimelineActionFixed)))
        {
            // The new timeline item is an action of fixed length.
            TimelineActionFixed newAction = ti as TimelineActionFixed;

            SortedDictionary<float, TimelineActionFixed> sortedClips = new SortedDictionary<float, TimelineActionFixed>();
            foreach (TimelineItem current in timelineTrack.TimelineItems)
            {
                TimelineActionFixed action = current as TimelineActionFixed;
                if (action == null) continue;
                sortedClips.Add(action.Firetime, action);
            }

            float latestTime = firetime;
            float length = newAction.ItemLength;
            foreach (TimelineActionFixed a in sortedClips.Values)
            {
                if (!(latestTime < a.Firetime && latestTime + length <= a.Firetime))
                {
                    latestTime = Mathf.Max(a.Firetime + a.Duration, latestTime);
                }
            }

            newAction.Firetime = latestTime;
        }
        else if (type.IsSubclassOf(typeof(TimelineAction)))
        {
            // The new timeline item is an action with arbitrary length.
            TimelineAction newAction = ti as TimelineAction;

            SortedDictionary<float, TimelineAction> sortedActions = new SortedDictionary<float, TimelineAction>();
            foreach (TimelineItem current in timelineTrack.TimelineItems)
            {
                TimelineAction action = current as TimelineAction;
                if (action == null) continue;
                sortedActions.Add(action.Firetime, action);
            }

            float latestTime = firetime;
            float length = newAction.Duration;
            foreach (TimelineAction a in sortedActions.Values)
            {
                if (latestTime >= a.Firetime)
                {
                    latestTime = Mathf.Max(latestTime, a.Firetime + a.Duration);
                }
                else
                {
                    length = a.Firetime - latestTime;
                    break;
                }
            }

            newAction.Firetime = latestTime;
            newAction.Duration = length;
        }
        else
        {
            ti.Firetime = firetime;
        }

        itemGO.transform.parent = timelineTrack.transform;
        itemGO.transform.localPosition = Vector3.zero;
        itemGO.transform.localRotation = Quaternion.identity;
        itemGO.transform.localScale = Vector3.one;
        timelineTrack.Manager.recache();
        return ti;
    }
    

    /// <summary>
    /// Create a blank actor track group.
    /// </summary>
    /// <returns></returns>
    public static TrackGroup CreateActorTrackGroup(TimelineManager cutscene)
    {
        return CreateActorTrackGroup(cutscene, null);
    }

    /// <summary>
    /// Create a track container for an actor in this cutscene.
    /// </summary>
    /// <param name="transform">The transform of the game object</param>
    /// <returns>the newly created container</returns>
    public static TrackGroup CreateActorTrackGroup(TimelineManager cutscene, Transform transform)
    {
        string trackGroupName = ACTOR_GROUP_NAME;
        if (transform != null)
        {
            trackGroupName = string.Format("{0} Group", transform.name);
        }

        GameObject actorGroupGO = new GameObject(trackGroupName, typeof(ActorTrackGroup));
        actorGroupGO.transform.parent = cutscene.transform;

        ActorTrackGroup actorTrackGroup = actorGroupGO.GetComponent<ActorTrackGroup>();
        actorTrackGroup.Actor = transform;

        return actorTrackGroup;
    }

    

    public static GlobalAction CreateGlobalAction(GlobalItemTrack track, Type type, string name, float firetime)
    {
        GameObject item = new GameObject(name);
        GlobalAction action = item.AddComponent(type) as GlobalAction;

        SortedDictionary<float,GlobalAction> sortedActions = new SortedDictionary<float, GlobalAction>();
        foreach (GlobalAction a in track.Actions)
        {
            sortedActions.Add(a.Firetime, a);
        }

        float latestTime = firetime;
        float length = DEFAULT_GLOBAL_ACTION_LENGTH;
        foreach (GlobalAction a in sortedActions.Values)
        {
            if (latestTime >= a.Firetime)
            {
                latestTime = Mathf.Max(latestTime, a.Firetime + a.Duration);
            }
            else
            {
                length = a.Firetime - latestTime;
                break;
            }
        }

        action.Firetime = latestTime;
        action.Duration = length;
        item.transform.parent = track.transform;
        track.Manager.recache();
        return action;
    }

    internal static ActorAction CreateActorAction(ActorItemTrack track, Type type, string name, float firetime)
    {
        GameObject item = new GameObject(name);
        item.transform.parent = track.transform;
        ActorAction action = item.AddComponent(type) as ActorAction;

        SortedDictionary<float, ActorAction> sortedActions = new SortedDictionary<float, ActorAction>();
        foreach (ActorAction a in track.ActorActions)
        {
            sortedActions.Add(a.Firetime, a);
        }

        float latestTime = 0;
        float length = DEFAULT_ACTOR_ACTION_LENGTH;
        foreach (ActorAction a in sortedActions.Values)
        {
            if (latestTime >= a.Firetime)
            {
                latestTime = Mathf.Max(latestTime, a.Firetime + a.Duration);
            }
            else
            {
                length = a.Firetime - latestTime;
                break;
            }
        }

        action.Firetime = latestTime;
        action.Duration = length;
        track.Manager.recache();
        return action;

    }
    


    internal static GlobalItemTrack CreateGlobalItemTrack(GlobalGroup directorGroup)
    {
        string name = DirectorHelper.getCutsceneItemName(directorGroup.gameObject, GLOBAL_TRACK_LABEL, typeof(GlobalItemTrack));
        GameObject globalTrackGO = new GameObject(name, typeof(GlobalItemTrack));

        globalTrackGO.transform.parent = directorGroup.transform;
        return globalTrackGO.GetComponent<GlobalItemTrack>();
    }

    internal static ActorItemTrack CreateActorItemTrack(ActorTrackGroup trackGroup)
    {
        GameObject eventTrackGO = new GameObject(EVENT_TRACK_LABEL, typeof(ActorItemTrack));
        eventTrackGO.transform.parent = trackGroup.transform;
        return eventTrackGO.GetComponent<ActorItemTrack>();
    }

    internal static CurveTrack CreateCurveTrack(ActorTrackGroup trackGroup)
    {
        GameObject curveTrackGO = new GameObject(CURVE_TRACK_LABEL, typeof(CurveTrack));
        curveTrackGO.transform.parent = trackGroup.transform;
        return curveTrackGO.GetComponent<CurveTrack>();
    }


    internal static GlobalEvent CreateGlobalEvent(GlobalItemTrack track, Type type, string name, float firetime)
    {
        GameObject item = new GameObject(name);
        item.transform.parent = track.transform;
        GlobalEvent globalEvent = item.AddComponent(type) as GlobalEvent;

        globalEvent.Firetime = firetime;
        track.Manager.recache();
        return globalEvent;
    }

    internal static ActorEvent CreateActorEvent(ActorItemTrack track, Type type, string name, float firetime)
    {
        GameObject item = new GameObject(name);
        item.transform.parent = track.transform;
        ActorEvent actorEvent = item.AddComponent(type) as ActorEvent;

        actorEvent.Firetime = firetime;
        track.Manager.recache();
        return actorEvent;
    }

    internal static ActorClipCurve CreateActorClipCurve(CurveTrack track)
    {
        string name = DirectorHelper.getCutsceneItemName(track.gameObject, CURVE_CLIP_NAME_DEFAULT, typeof(ActorClipCurve));
        GameObject item = new GameObject(name);
        
        ActorClipCurve clip = item.AddComponent<ActorClipCurve>();

        SortedDictionary<float, ActorClipCurve> sortedItems = new SortedDictionary<float, ActorClipCurve>();
        foreach (ActorClipCurve c in track.TimelineItems)
        {
            sortedItems.Add(c.Firetime, c);
        }

        float latestTime = 0;
        float length = DEFAULT_CURVE_LENGTH;
        foreach (ActorClipCurve c in sortedItems.Values)
        {
            if (latestTime >= c.Firetime)
            {
                latestTime = Mathf.Max(latestTime, c.Firetime + c.Duration);
            }
            else
            {
                length = c.Firetime - latestTime;
                break;
            }
        }

        clip.Firetime = latestTime;
        clip.Duration = length;
        item.transform.parent = track.transform;

        return clip;
    }

    
}
