using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class CinemaClipCurveWrapper : CinemaActionWrapper
{
    public CinemaMemberCurveWrapper[] MemberCurves;

    public CinemaClipCurveWrapper(Behaviour behaviour, float firetime, float duration) : base(behaviour, firetime, duration)
    {
        this.MemberCurves = new CinemaMemberCurveWrapper[0];
    }

    internal void CropDuration(float newDuration)
    {
        if (newDuration > 0f)
        {
            if (newDuration > base.Duration)
            {
                this.updateKeyframeTime(base.Firetime + base.Duration, base.Firetime + newDuration);
            }
            else
            {
                CinemaMemberCurveWrapper[] memberCurves = this.MemberCurves;
                for (int i = 0; i < memberCurves.Length; i++)
                {
                    CinemaAnimationCurveWrapper[] animationCurves = memberCurves[i].AnimationCurves;
                    for (int j = 0; j < animationCurves.Length; j++)
                    {
                        animationCurves[j].CollapseEnd(base.Firetime + base.Duration, base.Firetime + newDuration);
                    }
                }
            }
            base.Duration = newDuration;
        }
    }

    internal void CropFiretime(float newFiretime)
    {
        if (newFiretime < (base.Firetime + base.Duration))
        {
            if (newFiretime < base.Firetime)
            {
                this.updateKeyframeTime(base.Firetime, newFiretime);
            }
            else
            {
                CinemaMemberCurveWrapper[] memberCurves = this.MemberCurves;
                for (int i = 0; i < memberCurves.Length; i++)
                {
                    CinemaAnimationCurveWrapper[] animationCurves = memberCurves[i].AnimationCurves;
                    for (int j = 0; j < animationCurves.Length; j++)
                    {
                        animationCurves[j].CollapseStart(base.Firetime, newFiretime);
                    }
                }
            }
            base.Duration += base.Firetime - newFiretime;
            base.Firetime = newFiretime;
        }
    }

    internal void ExtendDuration(float newDuration)
    {
        if (newDuration > 0f)
        {
            base.Duration = newDuration;
        }
    }

    internal void Flip()
    {
        CinemaMemberCurveWrapper[] memberCurves = this.MemberCurves;
        for (int i = 0; i < memberCurves.Length; i++)
        {
            CinemaAnimationCurveWrapper[] animationCurves = memberCurves[i].AnimationCurves;
            for (int j = 0; j < animationCurves.Length; j++)
            {
                animationCurves[j].Flip();
            }
        }
    }

    internal void ScaleDuration(float newDuration)
    {
        if (newDuration > 0f)
        {
            CinemaMemberCurveWrapper[] memberCurves = this.MemberCurves;
            for (int i = 0; i < memberCurves.Length; i++)
            {
                CinemaAnimationCurveWrapper[] animationCurves = memberCurves[i].AnimationCurves;
                for (int j = 0; j < animationCurves.Length; j++)
                {
                    animationCurves[j].ScaleEnd(base.Duration, newDuration);
                }
            }
            base.Duration = newDuration;
        }
    }

    internal void ScaleFiretime(float newFiretime)
    {
        if (newFiretime < (base.Firetime + base.Duration))
        {
            CinemaMemberCurveWrapper[] memberCurves = this.MemberCurves;
            for (int i = 0; i < memberCurves.Length; i++)
            {
                CinemaAnimationCurveWrapper[] animationCurves = memberCurves[i].AnimationCurves;
                for (int j = 0; j < animationCurves.Length; j++)
                {
                    animationCurves[j].ScaleStart(base.Firetime, base.Duration, newFiretime);
                }
            }
            base.Duration += base.Firetime - newFiretime;
            base.Firetime = newFiretime;
        }
    }

    internal void TranslateCurves(float amount)
    {
        base.Firetime += amount;
        CinemaMemberCurveWrapper[] memberCurves = this.MemberCurves;
        for (int i = 0; i < memberCurves.Length; i++)
        {
            foreach (CinemaAnimationCurveWrapper wrapper in memberCurves[i].AnimationCurves)
            {
                if (amount > 0f)
                {
                    for (int j = wrapper.KeyframeCount - 1; j >= 0; j--)
                    {
                        Keyframe keyframe = wrapper.GetKeyframe(j);
                        float introduced11 = keyframe.value;
                        float introduced12 = keyframe.inTangent;
                        Keyframe kf = new Keyframe(keyframe.time + amount, introduced11, introduced12, keyframe.outTangent);
                        kf.tangentMode=(keyframe.tangentMode);
                        wrapper.MoveKey(j, kf);
                    }
                }
                else
                {
                    for (int k = 0; k < wrapper.KeyframeCount; k++)
                    {
                        Keyframe keyframe3 = wrapper.GetKeyframe(k);
                        float introduced13 = keyframe3.value;
                        float introduced14 = keyframe3.inTangent;
                        Keyframe keyframe4 = new Keyframe(keyframe3.time + amount, introduced13, introduced14, keyframe3.outTangent);
                        keyframe4.tangentMode=(keyframe3.tangentMode);
                        wrapper.MoveKey(k, keyframe4);
                    }
                }
            }
        }
    }

    public bool TryGetValue(string type, string propertyName, out CinemaMemberCurveWrapper memberWrapper)
    {
        memberWrapper = null;
        foreach (CinemaMemberCurveWrapper wrapper in this.MemberCurves)
        {
            if ((wrapper.Type == type) && (wrapper.PropertyName == propertyName))
            {
                memberWrapper = wrapper;
                return true;
            }
        }
        return false;
    }

    private void updateKeyframeTime(float oldTime, float newTime)
    {
        CinemaMemberCurveWrapper[] memberCurves = this.MemberCurves;
        for (int i = 0; i < memberCurves.Length; i++)
        {
            foreach (CinemaAnimationCurveWrapper wrapper in memberCurves[i].AnimationCurves)
            {
                for (int j = 0; j < wrapper.KeyframeCount; j++)
                {
                    Keyframe keyframe = wrapper.GetKeyframe(j);
                    if (Mathf.Abs(keyframe.time - oldTime) < 1E-05)
                    {
                        float introduced8 = keyframe.value;
                        float introduced9 = keyframe.inTangent;
                        Keyframe kf = new Keyframe(newTime, introduced8, introduced9, keyframe.outTangent);
                        kf.tangentMode=(keyframe.tangentMode);
                        wrapper.MoveKey(j, kf);
                    }
                }
            }
        }
    }

    internal bool IsEmpty
    {
        get
        {
            if ((this.MemberCurves != null) && (this.MemberCurves.Length != 0))
            {
                return false;
            }
            return true;
        }
    }

    internal int RowCount
    {
        get
        {
            int num = 0;
            foreach (CinemaMemberCurveWrapper wrapper in this.MemberCurves)
            {
                num++;
                if (wrapper.IsFoldedOut)
                {
                    CinemaAnimationCurveWrapper[] animationCurves = wrapper.AnimationCurves;
                    for (int i = 0; i < animationCurves.Length; i++)
                    {
                       // CinemaAnimationCurveWrapper wrapper1 = animationCurves[i];
                        num++;
                    }
                }
            }
            return num;
        }
    }
}

