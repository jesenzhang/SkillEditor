
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Jump Action", TrackItemGenre.SkillActionItem)]
public class JumpActionItem : SkillActionItem
{
	public string endAction;
    public int endTime;

    public float distance;
    public float height;
    Vector3 orignalPos;
    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            return Example.SkillAction.ActionType.JUMP;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[6];
            args[0].StrValue = endAction;
            args[1].IntValue = endTime;
            args[2].FloatValue = distance;
            args[3].FloatValue = height;
            return args;
        }
    }

    float GetCurrnetHeight(float time)
    {
        float a =(8 * height)/(duration* duration);
        float t = (duration / 2 - time);
        float h = height - a * t * t / 2;
        return h;
    }
    float GetCurrnetDistance(float time)
    {
        float s = distance / duration;
        float h = s   * time;
        return h;
    }
    public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
    {
        actor.transform.position = orignalPos + GetCurrnetDistance(runningTime) * actor.transform.forward + GetCurrnetHeight(runningTime) * actor.transform.up;
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
