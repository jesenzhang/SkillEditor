
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Wait Combo Action", TrackItemGenre.SkillActionItem)]
public class  WaitComboActionItem : SkillActionItem
{
    public string comboSkillID;

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            return Example.SkillAction.ActionType.WAIT_COMBO;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[1];
            args[0].StrValue = comboSkillID;
            return args;
        }
    }

    private void OnEnable()
    {
    }
    void OnDrawGizmos()
    {
    }

    public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
    {
       
    }
    public override void SetTime(GameObject Actor, float time, float deltaTime)
    {
        base.SetTime(Actor, time, deltaTime);
    }

    public override void Trigger(GameObject actor)
    {
      
    }
    public override void Pause(GameObject Actor)
    {

    }

    public override void Resume(GameObject Actor)
    {

    }

    public override void Stop(GameObject actor)
    {
       
    }

    public override void End(GameObject actor)
    {
       
    }
}
