using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class TimelineTrackWrapper : UnityBehaviourWrapper
{
    public bool IsLocked;
    private Dictionary<Behaviour, TimelineItemWrapper> itemMap;
    private int ordinal;

    public TimelineTrackWrapper(Behaviour behaviour) : base(behaviour)
    {
        this.itemMap = new Dictionary<Behaviour, TimelineItemWrapper>();
    }

    public void AddItem(Behaviour behaviour, TimelineItemWrapper wrapper)
    {
        this.itemMap.Add(behaviour, wrapper);
    }

    public bool ContainsItem(Behaviour behaviour, out TimelineItemWrapper itemWrapper)
    {
        return this.itemMap.TryGetValue(behaviour, out itemWrapper);
    }

    public TimelineItemWrapper GetItemWrapper(Behaviour behaviour)
    {
        return this.itemMap[behaviour];
    }

    public void RemoveItem(Behaviour behaviour)
    {
        this.itemMap.Remove(behaviour);
    }

    public IEnumerable<Behaviour> Behaviours
    {
        get
        {
            return this.itemMap.Keys;
        }
    }

    public IEnumerable<TimelineItemWrapper> Items
    {
        get
        {
            return this.itemMap.Values;
        }
    }

    public int Ordinal
    {
        get
        {
            return this.ordinal;
        }
        set
        {
            this.ordinal = value;
        }
    }
}

