using System;
using UnityEditor;
using UnityEngine;

public class CinemaAnimationCurveWrapper
{
    public UnityEngine.Color Color;
    private AnimationCurve curve;
    public int Id;
    public bool IsVisible = true;
    private CinemaKeyframeWrapper[] KeyframeControls = new CinemaKeyframeWrapper[0];
    public string Label;

    public void AddKey(float time, float value)
    {
        Keyframe keyframe = new Keyframe();
        Keyframe keyframe2 = new Keyframe();
        for (int i = 0; i < (this.curve.length - 1); i++)
        {
            Keyframe keyframe4 = this.curve.keys[i];
            Keyframe keyframe5 = this.curve.keys[i + 1];
            if ((keyframe4.time < time) && (time < keyframe5.time))
            {
                keyframe = keyframe4;
                keyframe2 = keyframe5;
            }
        }
        Keyframe keyframe3 = new Keyframe(time, value);
        int num = this.curve.AddKey(keyframe3);
        if (num > 0)
        {
            this.curve.MoveKey(num - 1, keyframe);
            if (this.IsAuto(num - 1))
            {
                this.SmoothTangents(num - 1, 0f);
            }
            if (this.IsBroken(num - 1) && this.IsRightLinear(num - 1))
            {
                this.SetKeyRightLinear(num - 1);
            }
        }
        if (num < (this.curve.length - 1))
        {
            this.curve.MoveKey(num + 1, keyframe2);
            if (this.IsAuto(num + 1))
            {
                this.SmoothTangents(num + 1, 0f);
            }
            if (this.IsBroken(num + 1) && this.IsLeftLinear(num + 1))
            {
                this.SetKeyLeftLinear(num + 1);
            }
        }
        ArrayUtility.Insert<CinemaKeyframeWrapper>(ref this.KeyframeControls, num, new CinemaKeyframeWrapper());
    }

    internal void CollapseEnd(float oldEndTime, float newEndTime)
    {
        for (int i = this.curve.length - 2; i > 0; i--)
        {
            if ((this.curve.keys[i].time >= newEndTime) && (this.curve.keys[i].time <= oldEndTime))
            {
                this.RemoveKey(i);
            }
        }
        if (newEndTime > this.GetKeyframe(0).time)
        {
            Keyframe keyframe = this.GetKeyframe(this.KeyframeCount - 1);
            float introduced4 = keyframe.value;
            float introduced5 = keyframe.inTangent;
            Keyframe kf = new Keyframe(newEndTime, introduced4, introduced5, keyframe.outTangent);
            kf.tangentMode=(keyframe.tangentMode);
            this.MoveKey(this.KeyframeCount - 1, kf);
        }
    }

    internal void CollapseStart(float oldStartTime, float newStartTime)
    {
        for (int i = this.curve.length - 2; i > 0; i--)
        {
            if ((this.curve.keys[i].time >= oldStartTime) && (this.curve.keys[i].time <= newStartTime))
            {
                this.RemoveKey(i);
            }
        }
        if (newStartTime < this.GetKeyframe(this.curve.length - 1).time)
        {
            Keyframe keyframe = this.GetKeyframe(0);
            float introduced4 = keyframe.value;
            float introduced5 = keyframe.inTangent;
            Keyframe kf = new Keyframe(newStartTime, introduced4, introduced5, keyframe.outTangent);
            kf.tangentMode=(keyframe.tangentMode);
            this.MoveKey(0, kf);
        }
    }

    public float Evaluate(float time)
    {
        return this.curve.Evaluate(time);
    }

    internal void FlattenKey(int index)
    {
        Keyframe keyframe = new Keyframe(this.curve.keys[index].time, this.curve.keys[index].value, 0f, 0f);
        keyframe.tangentMode=(0);
        this.curve.MoveKey(index, keyframe);
    }

    internal void Flip()
    {
        if (this.curve.length >= 2)
        {
            AnimationCurve curve = new AnimationCurve();
            float num = this.curve.keys[0].time;
            float num2 = this.curve.keys[this.curve.length - 1].time;
            for (int i = 0; i < this.curve.length; i++)
            {
                Keyframe keyframe = this.GetKeyframe(i);
                float num4 = (num2 - keyframe.time) + num;
                float introduced8 = keyframe.value;
                float introduced9 = keyframe.inTangent;
                Keyframe keyframe3 = new Keyframe(num4, introduced8, introduced9, keyframe.outTangent);
                keyframe3.tangentMode=(keyframe.tangentMode);
                curve.AddKey(keyframe3);
            }
            this.Curve = curve;
        }
    }

    public Vector2 GetInTangentScreenPosition(int index)
    {
        return this.GetKeyframeWrapper(index).InTangentControlPointPosition;
    }

    public Keyframe GetKeyframe(int index)
    {
        return this.curve.keys[index];
    }

    public Vector2 GetKeyframeScreenPosition(int index)
    {
        return this.GetKeyframeWrapper(index).ScreenPosition;
    }

    public CinemaKeyframeWrapper GetKeyframeWrapper(int i)
    {
        return this.KeyframeControls[i];
    }

    public Vector2 GetOutTangentScreenPosition(int index)
    {
        return this.GetKeyframeWrapper(index).OutTangentControlPointPosition;
    }

    private void initializeKeyframeWrappers()
    {
        this.KeyframeControls = new CinemaKeyframeWrapper[this.curve.length];
        for (int i = 0; i < this.curve.length; i++)
        {
            this.KeyframeControls[i] = new CinemaKeyframeWrapper();
        }
    }

    internal bool IsAuto(int index)
    {
        return (this.curve.keys[index].tangentMode == 10);
    }

    internal bool IsBroken(int index)
    {
        return ((this.curve.keys[index].tangentMode % 2) == 1);
    }

    internal bool IsFreeSmooth(int index)
    {
        return (this.curve.keys[index].tangentMode == 0);
    }

    internal bool IsLeftConstant(int index)
    {
        return (this.IsBroken(index) && ((this.curve.keys[index].tangentMode % 8) == 7));
    }

    internal bool IsLeftFree(int index)
    {
        return (this.IsBroken(index) && ((this.curve.keys[index].tangentMode % 8) == 1));
    }

    internal bool IsLeftLinear(int index)
    {
        return (this.IsBroken(index) && ((this.curve.keys[index].tangentMode % 8) == 5));
    }

    internal bool IsRightConstant(int index)
    {
        return ((this.curve.keys[index].tangentMode / 8) == 3);
    }

    internal bool IsRightFree(int index)
    {
        return (this.IsBroken(index) && ((this.curve.keys[index].tangentMode / 8) == 0));
    }

    internal bool IsRightLinear(int index)
    {
        return ((this.curve.keys[index].tangentMode / 8) == 2);
    }

    public int MoveKey(int index, Keyframe kf)
    {
        int num = this.curve.MoveKey(index, kf);
        CinemaKeyframeWrapper wrapper = this.KeyframeControls[index];
        this.KeyframeControls[index] = this.KeyframeControls[num];
        this.KeyframeControls[num] = wrapper;
        if (this.IsAuto(num))
        {
            this.SmoothTangents(num, 0f);
        }
        if (this.IsBroken(num))
        {
            if (this.IsLeftLinear(num))
            {
                this.SetKeyLeftLinear(num);
            }
            if (this.IsRightLinear(num))
            {
                this.SetKeyRightLinear(num);
            }
        }
        if (index > 0)
        {
            if (this.IsAuto(index - 1))
            {
                this.SmoothTangents(index - 1, 0f);
            }
            if (this.IsBroken(index - 1) && this.IsRightLinear(index - 1))
            {
                this.SetKeyRightLinear(index - 1);
            }
        }
        if (index < (this.curve.length - 1))
        {
            if (this.IsAuto(index + 1))
            {
                this.SmoothTangents(index + 1, 0f);
            }
            if (this.IsBroken(index + 1) && this.IsLeftLinear(index + 1))
            {
                this.SetKeyLeftLinear(index + 1);
            }
        }
        return num;
    }

    internal void RemoveAtTime(float time)
    {
        int index = -1;
        for (int i = 0; i < this.curve.length; i++)
        {
            if (this.curve.keys[i].time == time)
            {
                index = i;
            }
        }
        if (index >= 0)
        {
            ArrayUtility.RemoveAt<CinemaKeyframeWrapper>(ref this.KeyframeControls, index);
            this.curve.RemoveKey(index);
            if (index > 0)
            {
                if (this.IsAuto(index - 1))
                {
                    this.SmoothTangents(index - 1, 0f);
                }
                if (this.IsBroken(index - 1) && this.IsRightLinear(index - 1))
                {
                    this.SetKeyRightLinear(index - 1);
                }
            }
            if (index < this.curve.length)
            {
                if (this.IsAuto(index))
                {
                    this.SmoothTangents(index, 0f);
                }
                if (this.IsBroken(index) && this.IsLeftLinear(index))
                {
                    this.SetKeyLeftLinear(index);
                }
            }
        }
    }

    public void RemoveKey(int id)
    {
        ArrayUtility.RemoveAt<CinemaKeyframeWrapper>(ref this.KeyframeControls, id);
        this.curve.RemoveKey(id);
        if (id > 0)
        {
            if (this.IsAuto(id - 1))
            {
                this.SmoothTangents(id - 1, 0f);
            }
            if (this.IsBroken(id - 1) && this.IsRightLinear(id - 1))
            {
                this.SetKeyRightLinear(id - 1);
            }
        }
        if (id < this.curve.length)
        {
            if (this.IsAuto(id))
            {
                this.SmoothTangents(id, 0f);
            }
            if (this.IsBroken(id) && this.IsLeftLinear(id))
            {
                this.SetKeyLeftLinear(id);
            }
        }
    }

    internal void ScaleEnd(float oldDuration, float newDuration)
    {
        float num = newDuration / oldDuration;
        float num2 = this.curve.keys[0].time;
        for (int i = 1; i < this.curve.length; i++)
        {
            Keyframe keyframe = this.GetKeyframe(i);
            float num4 = ((keyframe.time - num2) * num) + num2;
            float introduced7 = keyframe.value;
            float introduced8 = keyframe.inTangent;
            Keyframe kf = new Keyframe(num4, introduced7, introduced8, keyframe.outTangent);
            kf.tangentMode=(keyframe.tangentMode);
            this.MoveKey(i, kf);
        }
    }

    internal void ScaleStart(float oldFiretime, float oldDuration, float newFiretime)
    {
        float num = ((oldFiretime + oldDuration) - newFiretime) / oldDuration;
        for (int i = this.curve.length - 1; i >= 0; i--)
        {
            Keyframe keyframe = this.GetKeyframe(i);
            float num3 = ((keyframe.time - oldFiretime) * num) + newFiretime;
            float introduced5 = keyframe.value;
            float introduced6 = keyframe.inTangent;
            Keyframe kf = new Keyframe(num3, introduced5, introduced6, keyframe.outTangent);
            kf.tangentMode=(keyframe.tangentMode);
            this.MoveKey(i, kf);
        }
    }

    public void SetInTangentScreenPosition(int index, Vector2 screenPosition)
    {
        this.GetKeyframeWrapper(index).InTangentControlPointPosition = screenPosition;
    }

    internal void SetKeyAuto(int index)
    {
        Keyframe keyframe = this.curve.keys[index];
        float introduced2 = keyframe.time;
        float introduced3 = keyframe.value;
        float introduced4 = keyframe.inTangent;
        Keyframe keyframe2 = new Keyframe(introduced2, introduced3, introduced4, keyframe.outTangent);
        keyframe2.tangentMode=(10);
        this.curve.MoveKey(index, keyframe2);
        this.curve.SmoothTangents(index, 0f);
    }

    internal void SetKeyBroken(int index)
    {
        Keyframe keyframe = this.curve.keys[index];
        float introduced2 = keyframe.time;
        float introduced3 = keyframe.value;
        float introduced4 = keyframe.inTangent;
        Keyframe keyframe2 = new Keyframe(introduced2, introduced3, introduced4, keyframe.outTangent);
        keyframe2.tangentMode=(1);
        this.curve.MoveKey(index, keyframe2);
    }

    public void SetKeyframeScreenPosition(int index, Vector2 screenPosition)
    {
        this.GetKeyframeWrapper(index).ScreenPosition = screenPosition;
    }

    internal void SetKeyFreeSmooth(int index)
    {
        Keyframe keyframe = this.curve.keys[index];
        float introduced2 = keyframe.time;
        float introduced3 = keyframe.value;
        float introduced4 = keyframe.inTangent;
        Keyframe keyframe2 = new Keyframe(introduced2, introduced3, introduced4, keyframe.outTangent);
        keyframe2.tangentMode=(0);
        this.curve.MoveKey(index, keyframe2);
    }

    internal void SetKeyLeftConstant(int index)
    {
        Keyframe keyframe = this.curve.keys[index];
        float introduced3 = keyframe.time;
        Keyframe keyframe2 = new Keyframe(introduced3, keyframe.value, float.PositiveInfinity, keyframe.outTangent);
        int num = (keyframe.tangentMode> 0x10) ? ((keyframe.tangentMode / 8) * 8) : 0;
        keyframe2.tangentMode=((6 + num) + 1);
        this.curve.MoveKey(index, keyframe2);
    }

    internal void SetKeyLeftFree(int index)
    {
        Keyframe keyframe = this.curve.keys[index];
        float introduced3 = keyframe.time;
        float introduced4 = keyframe.value;
        float introduced5 = keyframe.inTangent;
        Keyframe keyframe2 = new Keyframe(introduced3, introduced4, introduced5, keyframe.outTangent);
        int num = (keyframe.tangentMode > 0x10) ? ((keyframe.tangentMode / 8) * 8) : 0;
        keyframe2.tangentMode=(num + 1);
        this.curve.MoveKey(index, keyframe2);
    }

    internal void SetKeyLeftLinear(int index)
    {
        Keyframe keyframe = this.curve.keys[index];
        float num = keyframe.inTangent;
        if (index > 0)
        {
            Keyframe keyframe3 = this.curve.keys[index - 1];
            num = (keyframe.value - keyframe3.value) / (keyframe.time - keyframe3.time);
        }
        float introduced5 = keyframe.time;
        Keyframe keyframe2 = new Keyframe(introduced5, keyframe.value, num, keyframe.outTangent);
        int num2 = (keyframe.tangentMode > 0x10) ? ((keyframe.tangentMode / 8) * 8) : 0;
        keyframe2.tangentMode=((num2 + 1) + 4);
        this.curve.MoveKey(index, keyframe2);
    }

    internal void SetKeyRightConstant(int index)
    {
        Keyframe keyframe = this.curve.keys[index];
        float introduced3 = keyframe.time;
        float introduced4 = keyframe.value;
        Keyframe keyframe2 = new Keyframe(introduced3, introduced4, keyframe.inTangent, float.PositiveInfinity);
        int num = ((keyframe.tangentMode == 10) || (keyframe.tangentMode == 0)) ? 0 : ((keyframe.tangentMode % 8) - 1);
        keyframe2.tangentMode=((0x18 + num) + 1);
        this.curve.MoveKey(index, keyframe2);
    }

    internal void SetKeyRightFree(int index)
    {
        Keyframe keyframe = this.curve.keys[index];
        float introduced3 = keyframe.time;
        float introduced4 = keyframe.value;
        float introduced5 = keyframe.inTangent;
        Keyframe keyframe2 = new Keyframe(introduced3, introduced4, introduced5, keyframe.outTangent);
        int num = ((keyframe.tangentMode == 10) || (keyframe.tangentMode == 0)) ? 0 : ((keyframe.tangentMode % 8) - 1);
        keyframe2.tangentMode=(num + 1);
        this.curve.MoveKey(index, keyframe2);
    }

    internal void SetKeyRightLinear(int index)
    {
        Keyframe keyframe = this.curve.keys[index];
        float num = keyframe.outTangent;
        if (index < (this.curve.length - 1))
        {
            Keyframe keyframe3 = this.curve.keys[index + 1];
            num = (keyframe3.value - keyframe.value) / (keyframe3.time - keyframe.time);
        }
        float introduced5 = keyframe.time;
        float introduced6 = keyframe.value;
        Keyframe keyframe2 = new Keyframe(introduced5, introduced6, keyframe.inTangent, num);
        int num2 = ((keyframe.tangentMode == 10) || (keyframe.tangentMode == 0)) ? 0 : ((keyframe.tangentMode % 8) - 1);
        keyframe2.tangentMode=((num2 + 0x10) + 1);
        this.curve.MoveKey(index, keyframe2);
    }

    public void SetOutTangentScreenPosition(int index, Vector2 screenPosition)
    {
        this.GetKeyframeWrapper(index).OutTangentControlPointPosition = screenPosition;
    }

    public void SmoothTangents(int index, float weight)
    {
        this.curve.SmoothTangents(index, weight);
    }

    public AnimationCurve Curve
    {
        get
        {
            return this.curve;
        }
        set
        {
            this.curve = value;
            this.initializeKeyframeWrappers();
        }
    }

    public int KeyframeCount
    {
        get
        {
            return this.curve.length;
        }
    }
}

