
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Show Warning Action", TrackItemGenre.SkillActionItem)]
public class  ShowWarningActionItem : PlayEffectActionItem
{
    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[2];
            args[0].IntValue = 0;
            args[1].StrValue = effectName;
            return args;
        }
    }

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            return Example.SkillAction.ActionType.SHOW_WARNING;
        }
    }

}
