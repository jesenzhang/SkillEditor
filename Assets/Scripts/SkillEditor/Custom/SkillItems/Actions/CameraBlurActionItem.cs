
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Camera Blur Action", TrackItemGenre.SkillActionItem)]
public class CameraBlurActionItem : SkillActionItem
{
    public float intensity; 
    
    public Camera effectCamera;
    
    CameraFilterPack_Blur_Focus focus;

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
          return Example.SkillAction.ActionType.CAMERA_BLUR;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[1];
            args[0].FloatValue = intensity;
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
       
    }
    public override void SetTime(GameObject Actor, float time, float deltaTime)
    {
        base.SetTime(Actor, time, deltaTime);
    }

    public override void Trigger(GameObject actor)
    {
        if (focus == null)
        {
            focus = effectCamera.gameObject.AddComponent<CameraFilterPack_Blur_Focus>();
            focus._Eyes = intensity;
        }
        else
        {
            focus._Eyes = intensity;
        }
    }
    public override void Pause(GameObject Actor)
    {

    }

    public override void Resume(GameObject Actor)
    {

    }

    public override void Stop(GameObject actor)
    {
        if (focus != null)
        {
            GameObject.DestroyImmediate(focus);
        }
    }

    public override void End(GameObject actor)
    {
        if (focus != null)
        {
            GameObject.DestroyImmediate(focus);
        }
    }
}
