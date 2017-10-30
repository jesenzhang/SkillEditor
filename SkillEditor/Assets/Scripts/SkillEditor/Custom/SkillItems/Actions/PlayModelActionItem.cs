using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Play Model Action", TrackItemGenre.SkillActionItem)]

public class PlayModelActionItem : SkillActionItem
{
    public string StateName;
    public int Layer = -1;
    public int crossFadeTime = 0;
    float Normalizedtime = float.NegativeInfinity;
    public AnimationClip clipInfo;
    public Animator animator;

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            return Example.SkillAction.ActionType.PLAY_ACTION;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[2];
            args[0].StrValue = StateName;
            args[1].IntValue = crossFadeTime;
            return args;
        }
    }

    public override void End(GameObject actor)
    {
        if (clipInfo == null)
        {
            animator.StopPlayback();
            animator.Update(0f);
        }
        else
        {
            animator.StopPlayback();
            animator.Play(0, 0, 0);
            animator.Update(0f);
            clipInfo = null;
        }
    }

    private void OnEnable()
    {

    }
 

    public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
    {

        if (animator == null)
        {
            animator = actor.GetComponentInChildren<Animator>();
        }
        if (animator == null)
        {
            Debug.Log(actor.name + " animator  == null ");
            return;
        }
        if (clipInfo == null)
        {
            animator.Play(StateName, Layer, Normalizedtime);

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == StateName)
                {
                    clipInfo = clips[i];
                    Duration = clips[i].length;
                    clipInfo.SampleAnimation(actor, runningTime);
                    break;
                }
            }
        }
        else
        {
            clipInfo.SampleAnimation(actor, runningTime);
        }
    }

    public override void Trigger(GameObject actor)
    {
        if (animator == null)
        {
            animator = actor.GetComponentInChildren<Animator>();
        }
        if (animator == null)
        {
            return;
        }

        if (crossFadeTime>0)
        {
            animator.CrossFade(StateName, crossFadeTime/1000f);
        }
        else
        {
            animator.Play(StateName, Layer, Normalizedtime);
        }

        if (clipInfo == null)
        {
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == StateName)
                {
                    clipInfo = clips[i];
                  //  Duration = clips[i].length;
                    break;
                }
            }
        }
    }

    public override void Stop(GameObject actor)
    {
        if (animator == null)
        {
            animator = actor.GetComponentInChildren<Animator>();
        }
        if (animator == null)
        {
            return;
        }

        if (clipInfo == null)
        {
            animator.StopPlayback();
            animator.Update(0f);
        }
        else
        {
            
            animator.StopPlayback();
            animator.Play(0,0,0);
            animator.Update(0f);
            clipInfo = null;
        }
    }
}
