using System;

public class CutsceneTrackGroupAttribute : Attribute
{
    private Type trackGroupType;

    public CutsceneTrackGroupAttribute(Type type)
    {
        this.trackGroupType = type;
    }

    public Type TrackGroupType
    {
        get
        {
            return this.trackGroupType;
        }
    }
}

