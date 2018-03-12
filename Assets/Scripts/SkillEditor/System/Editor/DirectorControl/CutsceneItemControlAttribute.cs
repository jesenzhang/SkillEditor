using System;

public class CutsceneItemControlAttribute : Attribute
{
    private int drawPriority;
    private Type itemType;

    public CutsceneItemControlAttribute(Type type)
    {
        this.itemType = type;
        this.drawPriority = 0;
    }

    public CutsceneItemControlAttribute(Type type, int drawPriority)
    {
        this.itemType = type;
        this.drawPriority = drawPriority;
    }

    public int DrawPriority
    {
        get
        {
            return this.drawPriority;
        }
    }

    public Type ItemType
    {
        get
        {
            return this.itemType;
        }
    }
}

