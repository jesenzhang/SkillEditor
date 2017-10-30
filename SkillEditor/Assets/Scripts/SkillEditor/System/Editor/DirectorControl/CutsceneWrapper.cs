using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CutsceneWrapper : UnityBehaviourWrapper
{
    private float duration;
    private bool isPlaying;
    private float runningTime;
    private Dictionary<Behaviour, TrackGroupWrapper> TrackGroupMap;

    public CutsceneWrapper(Behaviour behaviour) : base(behaviour)
    {
        this.TrackGroupMap = new Dictionary<Behaviour, TrackGroupWrapper>();
    }

    public void AddTrackGroup(Behaviour behaviour, TrackGroupWrapper wrapper)
    {
        this.TrackGroupMap.Add(behaviour, wrapper);
    }

    public bool ContainsTrackGroup(Behaviour behaviour, out TrackGroupWrapper trackGroupWrapper)
    {
        return this.TrackGroupMap.TryGetValue(behaviour, out trackGroupWrapper);
    }

    public TrackGroupWrapper GetTrackGroupWrapper(Behaviour behaviour)
    {
        return this.TrackGroupMap[behaviour];
    }

    public void RemoveTrackGroup(Behaviour behaviour)
    {
        this.TrackGroupMap.Remove(behaviour);
    }

    public IEnumerable<Behaviour> Behaviours
    {
        get
        {
            return this.TrackGroupMap.Keys;
        }
    }

    public float Duration
    {
        get
        {
            return this.duration;
        }
        set
        {
            this.duration = value;
        }
    }

    public bool IsPlaying
    {
        get
        {
            return this.isPlaying;
        }
        set
        {
            this.isPlaying = value;
        }
    }

    public float RunningTime
    {
        get
        {
            return this.runningTime;
        }
        set
        {
            this.runningTime = value;
        }
    }

    public IEnumerable<TrackGroupWrapper> TrackGroups
    {
        get
        {
            return this.TrackGroupMap.Values;
        }
    }
}

