
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
 

public abstract class SkillParticleEmitterItem : ActorAction
{
    public Example.SkillParticleEmitter.EmitterType emitterType;
    //返回层级 最多两层
    public int GetLevel()
    {
        int level = 1; 
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<SkillParticleActionItem>() != null)
            {
                level = 2;
                return level;
            }
        }
        return level;
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
    public abstract Example.SkillAction.ActionType actionType
    {
        get ;
    }

    public abstract ContentValue[] arguments
    {
        get;
    }

}
