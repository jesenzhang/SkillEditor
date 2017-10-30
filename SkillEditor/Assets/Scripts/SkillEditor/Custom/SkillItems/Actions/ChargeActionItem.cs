
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
 

[TimelineItemAttribute("SkillAction", "Charge Action Item", TrackItemGenre.SkillActionItem)]
public class  ChargeActionItem : SkillActionItem
{
    public string endAction;

    public int endTime;

    public float distance;

    Vector3 orignalPos;
    float GetCurrnetDistance(float time)
    {
        float a = (2 * distance) / (duration * duration);
        float h = a * time*time/2;
        return h;
    }
    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            return Example.SkillAction.ActionType.CHARGE;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[4];
            args[0].StrValue = endAction;
            args[1].IntValue = endTime;
            args[2].FloatValue = distance;
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
        actor.transform.position = orignalPos + GetCurrnetDistance(runningTime) * actor.transform.forward;
    }
    public override void SetTime(GameObject Actor, float time, float deltaTime)
    {
        base.SetTime(Actor, time, deltaTime);
    }

    public override void Trigger(GameObject actor)
    {
        orignalPos = actor.transform.position;
    }
    public override void Pause(GameObject Actor)
    {

    }

    public override void Resume(GameObject Actor)
    {

    }

    public override void Stop(GameObject actor)
    {
        actor.transform.position = orignalPos;
    }

    public override void End(GameObject actor)
    {
       
    }
}
