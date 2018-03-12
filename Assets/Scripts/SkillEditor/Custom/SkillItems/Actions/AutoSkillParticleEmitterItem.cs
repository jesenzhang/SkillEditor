using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Auto Skill Particle Emitter Action", TrackItemGenre.SkillActionItem)]
public class AutoSkillParticleEmitterItem : SkillParticleEmitterItem
{
    [Header("Emitter Setting")]
    public SkillShape emitterShape;
    public Example.SkillShapeNew.ShapeType emitterShapeType = Example.SkillShapeNew.ShapeType.NONE;
    public Vector3 emitterOffset;
    public int emitterCount = 1;
    public int waves = 1;
    public int waveDelay = 0;
    [Header("Particle Setting")]
    public GameObject effectPrefab;

    public Example.SkillPath.PathType particlePathType;
    public SkillShape hitShape;
    public Example.SkillShapeNew.ShapeType particleHitShapeType;

    public int particleStartFrame;

    public int particleDuration;

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
