using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace TimeLine
{
    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }

    [TimelineItemAttribute("Animator", "Play Mecanim Animation Timeline", TrackItemGenre.ActorItem, TrackItemGenre.MecanimItem)]
    public class PlayAnimatorTimeline : ActorAction
    {
        public string StateName;
        public int Layer = -1;
        float Normalizedtime = float.NegativeInfinity;

        public override void End(GameObject actor)
        {
            ac = null;
        }

        AnimationClip ac;
        Animator animator;
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
            if (ac == null)
            {
                animator.Play(StateName, Layer, Normalizedtime);
                
               AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
               for (int i = 0; i < clips.Length; i++)
               {
                   if (clips[i].name == StateName)
                   {
                       ac = clips[i];
                       Duration = clips[i].length;
                       ac.SampleAnimation(actor, runningTime);
                       break;
                   }
               }
                /*
                    AnimatorOverrideController overrideController = new AnimatorOverrideController();
                overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
                AnimationClipOverrides newoverrides = new AnimationClipOverrides(animator.runtimeAnimatorController.animationClips.Length);
                overrideController.GetOverrides(newoverrides);
                 
                for (int i = 0; i < newoverrides.Count; i++)
                {
                    if (newoverrides[StateName] != null)
                    {
                        ac = newoverrides[StateName];
                        Duration = newoverrides[StateName].length;
                        ac.SampleAnimation(actor, runningTime);
                        break;
                    }
                }*/
            }
            else
            {
                ac.SampleAnimation(actor, runningTime);
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

            animator.Play(StateName, Layer, Normalizedtime);
 
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name == StateName)
                {
                    ac = clips[i];
                    Duration = clips[i].length;
                }
            }
            /*
             *  AnimatorOverrideController overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
            AnimationClipPair[] pairclips = overrideController.clips;
            for (int i = 0; i < pairclips.Length; i++)
            {
                if (pairclips[i].originalClip.name.IndexOf(StateName) != -1)
                {
                    ac = pairclips[i].originalClip;
                    Duration = pairclips[i].originalClip.length;
                }
            }*/
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

            if (ac == null)
            {
                Debug.Log(actor.name + " AnimationClip = null");
                animator.StopPlayback();
            }
            else
            {
                animator.StopPlayback();
            }
        }
    }
}