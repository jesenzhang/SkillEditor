
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Camera Shake Action", TrackItemGenre.SkillActionItem)]
public class CameraShakeActionItem : SkillActionItem
{
    public float intensity;
    public float freq;

    public Camera effectCamera;
    private Vector3 originPos;
    private float beginShakeTime = 0;
    

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            return Example.SkillAction.ActionType.CAMERA_SHAKE;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[2];
            args[0].FloatValue = freq;
            args[1].FloatValue = intensity;
            return args;
        }
    }

    private void OnEnable()
    {
        if (effectCamera == null)
            effectCamera = Camera.main;
    }
    void OnDrawGizmos()
    {
    }

    public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
    {
        beginShakeTime += deltaTime;
        {
            if ((beginShakeTime) > (1f / freq))
            {
                effectCamera.transform.position = originPos + UnityEngine.Random.insideUnitSphere * intensity;

                beginShakeTime = 0;
            }
        }
    }
    public override void SetTime(GameObject Actor, float time, float deltaTime)
    {
        base.SetTime(Actor, time, deltaTime);
    }

    public override void Trigger(GameObject actor)
    {
        originPos = effectCamera.transform.position;
        beginShakeTime = 0;
     
    }
    public override void Pause(GameObject Actor)
    {

    }

    public override void Resume(GameObject Actor)
    {

    }

    public override void Stop(GameObject actor)
    {
        effectCamera.transform.position = originPos;
        beginShakeTime = 0;
       
    }

    public override void End(GameObject actor)
    {
        effectCamera.transform.position = originPos;
        beginShakeTime = 0;
       
    }
}
