using System;
using UnityEngine;

public class TrackItemEventArgs : EventArgs
{
    public float firetime;
    public Behaviour item;

    public TrackItemEventArgs(Behaviour item, float firetime)
    {
        this.item = item;
        this.firetime = firetime;
    }
}

