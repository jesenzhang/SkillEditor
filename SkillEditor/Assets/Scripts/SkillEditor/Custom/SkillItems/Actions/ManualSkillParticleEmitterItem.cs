using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Manual Skill Particle Emitter Action", TrackItemGenre.SkillActionItem)]
public class ManualSkillParticleEmitterItem : SkillParticleEmitterItem
{

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            return Example.SkillAction.ActionType.EMIT_PARTICLE;
        }
    }


    public override ContentValue[] arguments
    {
        get
        {
            return null;
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

    public override void Stop(GameObject actor)
    {
      
    }

    public override void End(GameObject actor)
    {
       
    }
}
