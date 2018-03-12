
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
 

public abstract class  SkillEventItem : ActorEvent
{
    public abstract Example.SkillEvent.EventType eventType
    {
        get;
    }
    public abstract ContentValue[] conditions
    {
        get;
    }

}
