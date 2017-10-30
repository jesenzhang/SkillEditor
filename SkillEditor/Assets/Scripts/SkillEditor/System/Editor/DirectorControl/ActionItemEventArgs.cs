using System;
using UnityEngine;

public class ActionItemEventArgs : EventArgs
{
    public Behaviour actionItem;
    public float duration;
    public float firetime;

    public ActionItemEventArgs(Behaviour actionItem, float firetime, float duration)
    {
        this.actionItem = actionItem;
        this.firetime = firetime;
        this.duration = duration;
    }
}

