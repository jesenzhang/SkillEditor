
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TimeLine;
using System;
using Example;

[TimelineItemAttribute("SkillAction", "Play Sound Action", TrackItemGenre.SkillActionItem)]


public class PlaySoundActionItem : SkillActionItem
{
    public AudioClip audioClip;
    private AudioSource AudioSourceObj;

    private string audioName;

    public int fade;

    float NormalizedTime = 0;

    public string AudioName
    {
        get
        {
            if (audioClip != null)
            {
                audioName = audioClip.name;
            }
            return audioName;
        }

        set
        {
            audioName = value;
        }
    }

    public override Example.SkillAction.ActionType actionType
    {
        get
        {
            return Example.SkillAction.ActionType.PLAY_SOUND;
        }
    }

    public override ContentValue[] arguments
    {
        get
        {
            ContentValue[] args = new ContentValue[2];
            args[0].StrValue = audioName;
            args[1].IntValue = fade;
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

        float normalizedTime = (RunningTime-Firetime) / Duration;
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
        Duration = audioClip.length;
        AudioName = audioClip.name;
        NormalizedTime = 0;
    }
    public override void Pause(GameObject Actor) {
        if (AudioSourceObj != null)
        {
            AudioSourceObj.Stop();
        }
    }
     
    public override void Resume(GameObject Actor) {
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
    }

    public override void End(GameObject actor)
    {
        if (AudioSourceObj != null)
        {
            NormalizedTime = 0;
            AudioSourceObj.Stop();
            GameObject.DestroyImmediate(AudioSourceObj.gameObject);
        }
    }
}
