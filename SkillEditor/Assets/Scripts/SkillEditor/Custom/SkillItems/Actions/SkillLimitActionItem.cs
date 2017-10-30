
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Skill Limit Action", TrackItemGenre.SkillActionItem)]

public class SkillLimitActionItem : SkillActionItem
{
    public SkillLimitType limitType = SkillLimitType.MOVE;

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            if (limitType == SkillLimitType.MOVE)
            {
                return Example.SkillAction.ActionType.MOVE_LIMIT;
            }
            if (limitType == SkillLimitType.CANCEL)
            {
                return Example.SkillAction.ActionType.CANCEL_LIMIT;
            }
            return Example.SkillAction.ActionType.UNKNOWN;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[0];
            return args;
        }
    }
    public override void End(GameObject actor)
    {
        
    }

    private void OnEnable()
    {

    }


    public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
    {
    }

    public override void Trigger(GameObject actor)
    {
    }

    public override void Stop(GameObject actor)
    {
    }
}
