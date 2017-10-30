using System;
using UnityEngine;

public class CurveClipScrubberEventArgs : EventArgs
{
    public Behaviour curveClipItem;
    public float time;

    public CurveClipScrubberEventArgs(Behaviour curveClipItem, float time)
    {
        this.curveClipItem = curveClipItem;
        this.time = time;
    }
}

