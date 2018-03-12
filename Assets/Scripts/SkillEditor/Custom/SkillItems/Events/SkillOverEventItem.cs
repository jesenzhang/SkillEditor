
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillEvent", "SkillOver Event", TrackItemGenre.SkillEventItem)]
public class SkillOverEventItem : SkillEventItem
{
    public List<SkillActionItem> Actions;

    public override ContentValue[] conditions
    {
        get
        {
            ContentValue[] args = new ContentValue[0];
            return args;
        }
    }

    public override Example.SkillEvent.EventType eventType
    {
        get
        {
            return Example.SkillEvent.EventType.SKILL_OVER;
        }
    }

    public override void Trigger(GameObject Actor)
    {

    }
}