using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Skill Hit Action", TrackItemGenre.SkillActionItem)]

public class SkillHitActionItem : SkillActionItem
{
    public string[] buffIDs;

    public GameObject hitEffect;

    [HideInInspector]
    public string hitEffectName;

    public int hitEffectTime = 100;
     
    public string hitAudio;

    public int maxHitCount = 1;
    public bool canThrough = false;

    float NormalizedTime = 0;
    public AudioClip audioClip;
    private AudioSource AudioSourceObj;
    private Transform ParticleRoot;
    ParticleSystem[] particleSys;
    GameObject EffectObj;
    public ParticleSystem[] ParticleSys
    {
        get
        {
            if (ParticleRoot != null)
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
            return Example.SkillAction.ActionType.HIT_POINT;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[buffIDs.Length + 3];
            args[0].StrValue = hitEffectName;
            args[1].IntValue = hitEffectTime;
            args[2].StrValue = hitAudio;
            for (int i = 0; i < buffIDs.Length; ++i)
            {
                args[3 + i].StrValue = buffIDs[i];
            }
            return args;
        }
    }
    private void OnEnable()
    {

    }
    void OnDrawGizmos()
    {
    }

    private void PlayClip(AudioClip clipToPlay, float RunningTime)
    {
        if (AudioSourceObj == null)
            return;
        if (AudioSourceObj.clip != clipToPlay)
        {
            AudioSourceObj.clip = clipToPlay;
        }

        float normalizedTime = (RunningTime - Firetime) / Duration;
        //AudioSourceObj.pitch = clipToPlay.length / Duration;
        normalizedTime = Mathf.Clamp(normalizedTime * clipToPlay.length, 0, clipToPlay.length);
        NormalizedTime = normalizedTime;
        if ((clipToPlay.length - normalizedTime) > 0.0001f)
        {
            if (!AudioSourceObj.isPlaying)
            {
                AudioSourceObj.time = normalizedTime;
                AudioSourceObj.Play();
            }

        }
    }
    public override void UpdateTime(GameObject actor, float runningTime, float deltaTime)
    {
        if (EffectObj != null)
        { 

            if (ParticleRoot == null)
            {
                ParticleRoot = EffectObj.transform;
            }
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    ParticleSys[i].Simulate(deltaTime, true, false);
                }
            }
        }

        if (AudioSourceObj != null && audioClip != null)
        {
            PlayClip(audioClip, runningTime);
        }

        
    }
    public override void SetTime(GameObject Actor, float time, float deltaTime)
    {
        base.SetTime(Actor, time, deltaTime);

        if (AudioSourceObj != null && audioClip != null)
        {
            AudioSourceObj.Stop();
            float normalizedTime = (time - Firetime) / Duration;
            //AudioSourceObj.pitch = clipToPlay.length / Duration;
            normalizedTime = Mathf.Clamp(normalizedTime * audioClip.length, 0, audioClip.length);

            NormalizedTime = normalizedTime;
            if ((audioClip.length - normalizedTime) > 0.0001f)
            {
                AudioSourceObj.time = normalizedTime;
            }
        }
        if (EffectObj != null)
        {
         
          
            if (ParticleRoot == null)
            {
                ParticleRoot = EffectObj.transform;
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
    public override void Trigger(GameObject actor)
    {
        if (AudioSourceObj)
        {
            GameObject.DestroyImmediate(AudioSourceObj.gameObject);
        }
        if (AudioSourceObj == null)
        {
            AudioSourceObj = new GameObject("AudioSourceObj").AddComponent<AudioSource>();
            AudioSourceObj.gameObject.transform.SetParent(transform);
            AudioSourceObj.gameObject.transform.position = Vector3.zero;
        }
        AudioSourceObj.clip = audioClip;
        hitAudio = audioClip.name;
        NormalizedTime = 0;
       
        if (EffectObj == null && hitEffect != null)
        {
            EffectObj = GameObject.Instantiate<GameObject>(hitEffect);
            EffectObj.transform.SetParent(transform);

        }
        
        if (EffectObj != null)
        { 
            if (ParticleRoot == null)
            {
                ParticleRoot = EffectObj.transform;
            }
            if (ParticleSys != null)
            {
                for (int i = 0; i < ParticleSys.Length; i++)
                {
                    ParticleSys[i].Play(true);
                }
            }
        }

        AudioSourceObj.Play();
    }
    public override void Pause(GameObject Actor)
    {
        if (AudioSourceObj != null)
        {
            AudioSourceObj.Stop();
        }
    }

    public override void Resume(GameObject Actor)
    {
        if (AudioSourceObj != null && audioClip != null)
        {
            AudioSourceObj.time = NormalizedTime;
            AudioSourceObj.Play();
        }
    }

    public override void Stop(GameObject actor)
    {
        if (AudioSourceObj != null)
        {
            NormalizedTime = 0;
            AudioSourceObj.Stop();
            GameObject.DestroyImmediate(AudioSourceObj.gameObject);
        }
        if (EffectObj != null)
            DestroyImmediate(EffectObj);
        EffectObj = null;
        ParticleRoot = null;
        ParticleSys = null;
       // GameObject.DestroyImmediate(this.gameObject);
    }

    public override void End(GameObject actor)
    {
        if (AudioSourceObj != null)
        {
            NormalizedTime = 0;
            AudioSourceObj.Stop();
            GameObject.DestroyImmediate(AudioSourceObj.gameObject);
        }
        if (EffectObj != null)
            DestroyImmediate(EffectObj);
        EffectObj = null;
        ParticleRoot = null;
        ParticleSys = null;
      //  GameObject.DestroyImmediate(this.gameObject);
    }
}
