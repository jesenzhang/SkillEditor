using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "SkillParticle Action", TrackItemGenre.SkillActionItem)]

public class SkillParticleActionItem : SkillActionItem
{
    public int id;

    public GameObject effect;
    GameObject EffectObj;

    public bool isBullet = false;

    [HideInInspector]
    private string effectName;

    public SkillShape hitShape;

    public SkillPath path;

    public Example.SkillPath.PathType pathType = Example.SkillPath.PathType.NONE;

    public Example.SkillShapeNew.ShapeType hitshapeType = Example.SkillShapeNew.ShapeType.NONE;

    [SerializeField]
    private List<SkillHitAction> hitActions;

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

    public List<SkillHitAction> HitActions
    {
        get
        {
            SkillHitAction[] actions = transform.GetComponentsInChildren<SkillHitAction>();
            if (hitActions == null)
            {
                hitActions = new List<SkillHitAction>(actions);
               
            }
            else
            {
                hitActions.Clear();
                for (int i = 0; i < actions.Length; i++)
                {
                    hitActions.Add(actions[i]);
                }
            }
            return hitActions;
        }

        set
        {
            hitActions = value;
        }
    }

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
            ContentValue[] args = new ContentValue[0];
            return args;
        }
    }

    public string EffectName
    {
        get
        {
            if (effect)
            {
                return effect.name;
            }
            return "";
        }
        
    }

    private void OnEnable()
    {

    }
    void OnDrawGizmos()
    {
        if(path!=null)
            path.OnDrawGizmos();
    }

    public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
    {
        if (EffectObj != null)
        {
            float factor = 0;
            if (duration > 0)
            {
                factor = ((float)runningTime) / duration;
            }
            if (path != null)
            {
                if (path is FollowSkillPath)
                {
                    FollowSkillPath fpath = (FollowSkillPath)path;
                    if (fpath.targetType == FollowTargetType.SELF)
                    {
                        fpath.targetObj = actor.transform;
                    }
                }
                path.UpdatePath(EffectObj.transform, factor);
            }
            if (ParticleRoot == null)
            {
                ParticleRoot = EffectObj.transform;
            }
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    UnityEditor.SceneView.RepaintAll();
                    System.Type t = System.Type.GetType("UnityEditor.GameView,UnityEditor.dll");
                    var mm = t.GetMethod("RepaintAll", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    mm.Invoke(null, null);
                    ParticleSys[i].Simulate(deltaTime, true, false);
                }
            }
        }
    }
    public override void SetTime(GameObject Actor, float time, float deltaTime)
    {
        base.SetTime(Actor, time, deltaTime);
       
        if (EffectObj != null)
        {
            float factor = 0;
            if (duration > 0)
            {
                factor = ((float)time) / duration;
            }
            if (path != null)
            {
                path.UpdatePath(EffectObj.transform, factor);
            }

            if (ParticleRoot == null)
            {
                ParticleRoot = EffectObj.transform;
            }
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    UnityEditor.SceneView.RepaintAll();
                    System.Type t = System.Type.GetType("UnityEditor.GameView,UnityEditor.dll");
                    var mm = t.GetMethod("RepaintAll", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                    mm.Invoke(null, null);
                    ParticleSys[i].Simulate(time, true, false);
                }
            }
        }
    }

    public override void Trigger(GameObject actor)
    {
        if (EffectObj == null && effect != null)
        {
            EffectObj = GameObject.Instantiate<GameObject>(effect);
            EffectObj.transform.SetParent(transform);

        }
        if (EffectObj != null)
        {
            if(path!=null)
                path.StartPath(EffectObj.transform);
            if (ParticleRoot == null)
            {
                ParticleRoot = EffectObj.transform;
            }
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    ParticleSys[i].Play();
                    ParticleSys[i].Simulate(0);
                }
            }
        }
    }

    public override void Stop(GameObject actor)
    {
        if (EffectObj != null)
            DestroyImmediate(EffectObj);
        EffectObj = null;
        ParticleRoot = null;
        ParticleSys = null;
    }

    public void Boom()
    {
        if (hitActions.Count > 0)
        {
            foreach (SkillHitAction ac in hitActions)
            {
                GameObject itemGO = new GameObject("Boom");
                itemGO.SetActive(false);
                SkillHitActionItem ti = itemGO.AddComponent<SkillHitActionItem>();
                ti.SetDefaults();
                ti.hitEffect = ac.hitEffect;
                ti.hitEffectName = ac.hitEffectName;
                ti.hitAudio = ac.hitAudio;
                ti.hitEffectTime = ac.hitEffectTime;
                ti.canThrough = ac.canThrough;
                ti.maxHitCount = ac.maxHitCount;
                ti.Firetime = this.firetime + ac.startFrame / 1000f+ this.duration;
                ti.Duration = ac.hitEffectTime / 1000f;
                ti.audioClip = ac.hitAudioClip;

                itemGO.transform.parent = this.TimelineTrack.transform;
                itemGO.transform.localPosition = Vector3.zero;
                itemGO.transform.localRotation = Quaternion.identity;
                itemGO.transform.localScale = Vector3.one;
                this.TimelineTrack.Manager.recache();
                itemGO.SetActive(true);
            }
        }
    }

    public override void End(GameObject actor)
    {
        if (EffectObj != null)
            DestroyImmediate(EffectObj);
        EffectObj = null;
        ParticleRoot = null;
        ParticleSys = null;
    }



}
