
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Play Effect Action", TrackItemGenre.SkillActionItem)]

public class PlayEffectActionItem : SkillActionItem
{
    public string effectName;

    public GameObject effect;

    public Transform effectBone;

    public Vector3 offSet = Vector3.zero;

    GameObject runningEffect;

    private Transform ParticleRoot;
    ParticleSystem[] particleSys;
    public ParticleSystem[] ParticleSys
    {
        get
        {
            if ( ParticleRoot != null)
            {
                List<ParticleSystem> aparticleSys = new List<ParticleSystem>();
                ParticleSystem[] tparticleSys = ParticleRoot.GetComponentsInChildren<ParticleSystem>();
                for (int i = 0; i < tparticleSys.Length; i++)
                {
                    if (ParticleSystemUtility.IsRoot(tparticleSys[i]))
                    {
                        aparticleSys.Add(tparticleSys[i]);
                    }
                }
                particleSys = aparticleSys.ToArray();
            }
            return particleSys;
        }

        set
        {
            particleSys = value;
        }
    }

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            return Example.SkillAction.ActionType.PLAY_EFFECT;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            if(effect!=null)
                effectName = effect.name;
            ContentValue[] args = new ContentValue[3];
            args[0].StrValue = effectName;
            args[1].StrValue = effectBone != null ? effectBone.name : "";
            args[2].Vector3Value = effect != null ? offSet : Vector3.zero;
            return args;
        }
    }

    private void OnEnable()
    {

    }


    public override void Trigger(GameObject actor)
    {
        if (runningEffect != null)
            DestroyImmediate(runningEffect);
        if (effect != null)
        {
            runningEffect = GameObject.Instantiate<GameObject>(effect);
            effectName = effect.name;
           
        }
        if (runningEffect!=null && effectBone != null)
        {
            runningEffect.transform.parent = effectBone;
           
        }
        if (runningEffect != null)
        {
            ParticleRoot = runningEffect.transform;
            runningEffect.transform.localPosition = offSet;
     
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    ParticleSys[i].Play(true);
                    ParticleSys[i].Simulate(0,true);
                }
            }
        }
    }

    public override void Stop(GameObject actor)
    {
        if(runningEffect!=null)
            DestroyImmediate(runningEffect);
        runningEffect = null;
        ParticleRoot = null;
        ParticleSys = null;
    }
    public override void End(GameObject actor)
    {
        if (runningEffect != null)
            DestroyImmediate(runningEffect);

        runningEffect = null;
        ParticleRoot = null;
        ParticleSys = null;
    }
    public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
    {
        if (runningEffect != null)
        {
      
            if (ParticleRoot == null)
            {
                ParticleRoot = runningEffect.transform;
            }
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    UnityEditor.SceneView.RepaintAll();
                    System.Type t = System.Type.GetType("UnityEditor.GameView,UnityEditor.dll");
                    var mm = t.GetMethod("RepaintAll", System.Reflection.BindingFlags.Static| System.Reflection.BindingFlags.Public);
                    mm.Invoke(null,null);
                    ParticleSys[i].Simulate(deltaTime, true, false);

                }
            }
        }
    }
    public override void SetTime(GameObject Actor, float time, float deltaTime)
    {
        base.SetTime(Actor, time, deltaTime);

        if (runningEffect != null)
        {
            if (ParticleRoot == null)
            {
                ParticleRoot = runningEffect.transform;
            }
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    ParticleSys[i].Simulate(time, true, false);
                }
            }
        }
    }

  }
