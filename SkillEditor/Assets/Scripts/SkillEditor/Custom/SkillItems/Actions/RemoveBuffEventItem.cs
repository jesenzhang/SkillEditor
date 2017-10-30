
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "RemoveBuff Action", TrackItemGenre.SkillEventItem)]
public class RemoveBuffEventItem : SkillActionItem
{
    public string buffID;

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override void Trigger(GameObject Actor)
    {

    }

}
