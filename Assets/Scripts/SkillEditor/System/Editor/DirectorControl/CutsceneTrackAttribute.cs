using System;

public class CutsceneTrackAttribute : Attribute
{
    private Type trackType;

    public CutsceneTrackAttribute(Type type)
    {
        this.trackType = type;
    }

    public Type TrackType
    {
        get
        {
            return this.trackType;
        }
    }
}

