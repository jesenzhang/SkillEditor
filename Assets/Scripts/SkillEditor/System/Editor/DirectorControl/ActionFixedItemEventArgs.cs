using System;
using UnityEngine;

public class ActionFixedItemEventArgs : EventArgs
{
    public Behaviour actionItem;
    public float duration;
    public float firetime;
    public float inTime;
    public float outTime;

    public ActionFixedItemEventArgs(Behaviour actionItem, float firetime, float duration, float inTime, float outTime)
    {
        this.actionItem = actionItem;
        this.firetime = firetime;
        this.duration = duration;
        this.inTime = inTime;
        this.outTime = outTime;
    }
}

