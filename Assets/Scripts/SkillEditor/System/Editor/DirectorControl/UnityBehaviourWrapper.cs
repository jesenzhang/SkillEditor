using System;
using UnityEngine;

public class UnityBehaviourWrapper
{
    private UnityEngine.Behaviour behaviour;
    private bool hasChanged = true;

    public UnityBehaviourWrapper(UnityEngine.Behaviour behaviour)
    {
        this.behaviour = behaviour;
    }

    public UnityEngine.Behaviour Behaviour
    {
        get
        {
            return this.behaviour;
        }
        set
        {
            this.behaviour = value;
        }
    }

    public bool HasChanged
    {
        get
        {
            return this.hasChanged;
        }
        set
        {
            this.hasChanged = value;
        }
    }
}

