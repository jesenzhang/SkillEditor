
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Camera Black Action", TrackItemGenre.SkillActionItem)]
public class CameraBlackActionItem : SkillActionItem
{
    public float startIntensity;
    public float endIntensity;

    public Camera effectCamera;

    CameraFilterPack_Colors_Brightness bright;

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
           return Example.SkillAction.ActionType.CAMERA_BLACK;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[2];
            args[0].FloatValue = startIntensity;
            args[1].FloatValue = endIntensity;
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
        if (bright != null)
        {
            bright._Brightness = Mathf.Lerp(startIntensity, endIntensity, (runningTime ) / duration);
        }
    }
    public override void SetTime(GameObject Actor, float time, float deltaTime)
    {
        base.SetTime(Actor, time, deltaTime);
        UpdateTime(Actor, time, deltaTime);
    }

    public override void Trigger(GameObject actor)
    {
        if (bright == null)
        {
            bright = effectCamera.gameObject.AddComponent<CameraFilterPack_Colors_Brightness>();
            bright._Brightness = startIntensity;
        }
        else
        {
            bright._Brightness = startIntensity;
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
        if (bright != null)
        {
            GameObject.DestroyImmediate(bright);
        }
    }

    public override void End(GameObject actor)
    {
        if (bright != null)
        {
            GameObject.DestroyImmediate(bright);
        }
    }
}
