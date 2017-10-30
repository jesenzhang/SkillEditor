using System;
using UnityEngine;

public class TrackControlEventArgs : EventArgs
{
    public Behaviour TrackBehaviour;
    public TimelineTrackControl TrackControl;

    public TrackControlEventArgs(Behaviour behaviour, TimelineTrackControl control)
    {
        this.TrackBehaviour = behaviour;
        this.TrackControl = control;
    }
}

