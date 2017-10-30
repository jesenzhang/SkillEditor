using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "AddBuff Action", TrackItemGenre.SkillEventItem)]
public class AddBuffActionItem : SkillActionItem
{

    public string buffID;

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            return Example.SkillAction.ActionType.ADD_BUFF;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[1];
            args[0].StrValue = buffID;
            return args;
        }
    }
}
