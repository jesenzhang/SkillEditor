using DirectorEditor;
using System;
using UnityEngine;

public class DirectorControlState
{
    public bool IsInPreviewMode;
    public bool IsSnapEnabled;
    public DirectorEditor.ResizeOption ResizeOption;
    public Vector2 Scale;
    public float ScrubberPosition;
    public float TickDistance;
    public Vector2 Translation;

    public float SnappedTime(float time)
    {
        if ((this.IsSnapEnabled && !Event.current.control) || (!this.IsSnapEnabled && Event.current.control))
        {
            time = ((int) ((time + (this.TickDistance / 2f)) / this.TickDistance)) * this.TickDistance;
        }
        return time;
    }

    public float TimeToPosition(float time)
    {
        return ((time * this.Scale.x) + this.Translation.x);
    }
}

