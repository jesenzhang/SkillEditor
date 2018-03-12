using System;
using UnityEngine;

public class TimelineItemWrapper
{
    private UnityEngine.Behaviour behaviour;
    protected float firetime;

    public TimelineItemWrapper(UnityEngine.Behaviour behaviour, float firetime)
    {
        this.behaviour = behaviour;
        this.firetime = firetime;
    }

    public UnityEngine.Behaviour Behaviour
    {
        get
        {
            return this.behaviour;
        }
    }

    public float Firetime
    {
        get
        {
            return this.firetime;
        }
        set
        {
            this.firetime = value;
        }
    }
}

