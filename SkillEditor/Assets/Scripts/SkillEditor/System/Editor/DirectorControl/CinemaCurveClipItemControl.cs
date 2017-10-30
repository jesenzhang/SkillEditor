using DirectorEditor;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor;
using UnityEngine;

public abstract class CinemaCurveClipItemControl : ActionItemControl
{
    private Rect curveCanvasPosition;
    private Rect curveTrackSafeArea;
    private const float DEFAULT_TRACK_LOWER_RANGE = 0f;
    private const float DEFAULT_TRACK_UPPER_RANGE = 1f;
    private bool haveCurvesChanged;
    private const int INDENT_AMOUNT = 12;
    protected bool isAutoResizeEnabled = true;
    protected bool isCurveClipEmpty = true;
    private bool isEditing;
    protected bool isFolded;
    private SortedList<float, int> keyframeTimes = new SortedList<float, int>();
    private Rect masterKeysPosition;
    private static float mouseDownOffset = -1f;
    private const float ONE_THIRD = 0.3333333f;
    private const float SAFE_ZONE_BUFFER = 8f;
    private const float SAFE_ZONE_BUFFER_WIDTH = 4f;
    private float scrollBarPosition;
    protected CinemaCurveSelection selection = new CinemaCurveSelection();
    private const float TANGENT_HANDLE_LENGTH = 30f;
    private const float THRESHOLD = 0.01f;
    private Rect timelineItemPosition;
    private Rect viewingSpace = new Rect(0f, 0f, 1f, 1f);

    [field: CompilerGenerated]
    public event CurveClipItemEventHandler AlterDuration;

    [field: CompilerGenerated]
    public event CurveClipItemEventHandler AlterFiretime;

    [field: CompilerGenerated]
    public event CurveClipWrapperEventHandler CurvesChanged;

    [field: CompilerGenerated]
    internal event CurveClipWrapperEventHandler RequestEdit;

    [field: CompilerGenerated]
    public event CurveClipSrubberEventHandler SnapScrubber;

    [field: CompilerGenerated]
    public event CurveClipItemEventHandler TranslateCurveClipItem;

    protected CinemaCurveClipItemControl()
    {
    }

    private void addKeyframes(object userData)
    {
        CurvesContext context = userData as CurvesContext;
        if (context != null)
        {
            CinemaMemberCurveWrapper[] memberCurves = context.wrapper.MemberCurves;
            for (int i = 0; i < memberCurves.Length; i++)
            {
                foreach (CinemaAnimationCurveWrapper wrapper in memberCurves[i].AnimationCurves)
                {
                    wrapper.AddKey(context.time, wrapper.Evaluate(context.time));
                }
            }
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper2 = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper2 != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper2));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void addKeyToCurve(object userData)
    {
        CurveContext context = userData as CurveContext;
        if (context != null)
        {
            Undo.RecordObject(base.Wrapper.Behaviour, "Added Key");
            context.curveWrapper.AddKey(context.time, context.curveWrapper.Evaluate(context.time));
        }
    }

    internal override void ConfirmTranslate()
    {
        CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
        if (wrapper != null)
        {
            if (this.TranslateCurveClipItem != null)
            {
                this.TranslateCurveClipItem(this, new CurveClipItemEventArgs(wrapper.Behaviour, wrapper.Firetime, wrapper.Duration));
            }
            this.haveCurvesChanged = true;
        }
    }

    private void deleteKey(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if (context != null)
        {
            context.curveWrapper.RemoveKey(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
            this.selection.CurveId = -1;
            this.selection.KeyId = -1;
        }
    }

    private void deleteKeyframes(object userData)
    {
        CurvesContext context = userData as CurvesContext;
        if (context != null)
        {
            CinemaMemberCurveWrapper[] memberCurves = context.wrapper.MemberCurves;
            for (int i = 0; i < memberCurves.Length; i++)
            {
                CinemaAnimationCurveWrapper[] animationCurves = memberCurves[i].AnimationCurves;
                for (int j = 0; j < animationCurves.Length; j++)
                {
                    animationCurves[j].RemoveAtTime(context.time);
                }
            }
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    public override void Draw(DirectorControlState state)
    {
        CinemaClipCurveWrapper clipCurveWrapper = base.Wrapper as CinemaClipCurveWrapper;
        if (clipCurveWrapper != null)
        {
            this.drawCurveItem();
            if (!this.isCurveClipEmpty && !this.isFolded)
            {
                this.drawCurveCanvas(state, clipCurveWrapper);
                if (this.IsEditing)
                {
                    this.drawMasterKeys(state);
                    this.handleCurveCanvasInput(clipCurveWrapper, state);
                }
            }
        }
    }

    protected void drawCurve(Rect position, CinemaAnimationCurveWrapper wrapper, DirectorControlState state, Rect view)
    {
  
        for (int i = 0; i < (wrapper.KeyframeCount - 1); i++)
        {
            CinemaKeyframeWrapper keyframeWrapper = wrapper.GetKeyframeWrapper(i);
            CinemaKeyframeWrapper wrapper3 = wrapper.GetKeyframeWrapper(i + 1);
            if (float.IsPositiveInfinity(wrapper.GetKeyframe(i).outTangent) || float.IsPositiveInfinity(wrapper.GetKeyframe(i + 1).inTangent))
            {
                Handles.DrawBezier(new Vector3(keyframeWrapper.ScreenPosition.x, keyframeWrapper.ScreenPosition.y), new Vector3(wrapper3.ScreenPosition.x, keyframeWrapper.ScreenPosition.y), new Vector3(wrapper3.ScreenPosition.x, keyframeWrapper.ScreenPosition.y), new Vector3(keyframeWrapper.ScreenPosition.x, keyframeWrapper.ScreenPosition.y), wrapper.Color, null, 4f);
                Handles.DrawBezier(new Vector3(wrapper3.ScreenPosition.x, keyframeWrapper.ScreenPosition.y), new Vector3(wrapper3.ScreenPosition.x, wrapper3.ScreenPosition.y), new Vector3(wrapper3.ScreenPosition.x, wrapper3.ScreenPosition.y), new Vector3(wrapper3.ScreenPosition.x, keyframeWrapper.ScreenPosition.y), wrapper.Color, null, 4f);
            }
            else
            {
                Handles.DrawBezier(new Vector3(keyframeWrapper.ScreenPosition.x, keyframeWrapper.ScreenPosition.y), new Vector3(wrapper3.ScreenPosition.x, wrapper3.ScreenPosition.y), new Vector3(keyframeWrapper.OutTangentControlPointPosition.x, keyframeWrapper.OutTangentControlPointPosition.y), new Vector3(wrapper3.InTangentControlPointPosition.x, wrapper3.InTangentControlPointPosition.y), wrapper.Color, null, 4f);
            }
        }
    }

    protected void drawCurveCanvas(DirectorControlState state, CinemaClipCurveWrapper clipCurveWrapper)
    {
        GUI.Box(this.curveCanvasPosition, GUIContent.none, TimelineTrackControl.styles.curveCanvasStyle);
        foreach (CinemaMemberCurveWrapper wrapper in clipCurveWrapper.MemberCurves)
        {
            foreach (CinemaAnimationCurveWrapper wrapper2 in wrapper.AnimationCurves)
            {
                if (wrapper.IsVisible && wrapper2.IsVisible)
                {
                    this.drawCurve(this.curveTrackSafeArea, wrapper2, state, this.viewingSpace);
                    if (this.IsEditing)
                    {
                        this.drawKeyframes(this.curveTrackSafeArea, wrapper, wrapper2, state, this.viewingSpace, clipCurveWrapper);
                    }
                }
            }
        }
    }

    protected void drawCurveItem()
    {
        if (base.Wrapper.Behaviour != null)
        {
            if (base.TrackControl.isExpanded)
            {
                if (base.IsSelected)
                {
                    GUI.Box(this.timelineItemPosition, GUIContent.none, TimelineTrackControl.styles.TrackItemSelectedStyle);
                }
                else
                {
                    GUI.Box(this.timelineItemPosition, GUIContent.none, TimelineTrackControl.styles.TrackItemStyle);
                }
            }
            else if (base.IsSelected)
            {
                GUI.Box(this.timelineItemPosition, GUIContent.none, TimelineTrackControl.styles.CurveTrackItemSelectedStyle);
            }
            else
            {
                GUI.Box(this.timelineItemPosition, GUIContent.none, TimelineTrackControl.styles.CurveTrackItemStyle);
            }
            Color ocolor = GUI.color;
            GUI.color=(new Color(0.71f, 0.14f, 0.8f));
            Rect timelineItemPosition = this.timelineItemPosition;
            timelineItemPosition.x=(timelineItemPosition.x + 4f);
            timelineItemPosition.width=(16f);
            timelineItemPosition.height=(16f);
            GUI.Box(timelineItemPosition, base.actionIcon, GUIStyle.none);
            GUI.color= ocolor;
            Rect controlPosition = base.controlPosition;
            controlPosition.x=(timelineItemPosition.xMax);
            controlPosition.width=(controlPosition.width - (timelineItemPosition.width + 4f));
            controlPosition.height=(17f);
            base.DrawRenameLabel(base.Wrapper.Behaviour.name, controlPosition, null);
        }
    }

    protected void drawKeyframes(Rect position, CinemaMemberCurveWrapper memberWrapper, CinemaAnimationCurveWrapper wrapper, DirectorControlState state, Rect view, CinemaClipCurveWrapper clipCurveWrapper)
    {
       // Vector2 translation = state.Translation;
       // Vector2 scale = state.Scale;
   
        for (int i = 0; i < wrapper.KeyframeCount; i++)
        {
           // Keyframe keyframe = wrapper.GetKeyframe(i);
            CinemaKeyframeWrapper keyframeWrapper = wrapper.GetKeyframeWrapper(i);
           // bool flag1 = (keyframe.time == clipCurveWrapper.Firetime) || (keyframe.time == (clipCurveWrapper.Firetime + clipCurveWrapper.Duration));
            Color color = GUI.color;
            if ((((this.selection.Type == memberWrapper.Type) && (this.selection.Property == memberWrapper.PropertyName)) && (this.selection.KeyId == i)) && (this.selection.CurveId == wrapper.Id))
            {
                if (((i > 0) && !wrapper.IsAuto(i)) && (!wrapper.IsLeftLinear(i) && !wrapper.IsLeftConstant(i)))
                {
                    Vector2 vector = new Vector2(keyframeWrapper.InTangentControlPointPosition.x - keyframeWrapper.ScreenPosition.x, keyframeWrapper.InTangentControlPointPosition.y - keyframeWrapper.ScreenPosition.y);
                    vector.Normalize();
                    vector = (Vector2) (vector * 30f);
                    Handles.color=(Color.gray);
                    Handles.DrawLine(new Vector3(keyframeWrapper.ScreenPosition.x, keyframeWrapper.ScreenPosition.y, 0f), new Vector3(keyframeWrapper.ScreenPosition.x + vector.x, keyframeWrapper.ScreenPosition.y + vector.y, 0f));
                    GUI.Label(new Rect((keyframeWrapper.ScreenPosition.x + vector.x) - 4f, (keyframeWrapper.ScreenPosition.y + vector.y) - 4f, 8f, 8f), string.Empty, TimelineTrackControl.styles.tangentStyle);
                }
                if (((i < (wrapper.KeyframeCount - 1)) && !wrapper.IsAuto(i)) && (!wrapper.IsRightLinear(i) && !wrapper.IsRightConstant(i)))
                {
                    Vector2 vector2 = new Vector2(keyframeWrapper.OutTangentControlPointPosition.x - keyframeWrapper.ScreenPosition.x, keyframeWrapper.OutTangentControlPointPosition.y - keyframeWrapper.ScreenPosition.y);
                    vector2.Normalize();
                    vector2 = (Vector2) (vector2 * 30f);
                    Handles.color=(Color.gray);
                    Handles.DrawLine(new Vector3(keyframeWrapper.ScreenPosition.x, keyframeWrapper.ScreenPosition.y, 0f), new Vector3(keyframeWrapper.ScreenPosition.x + vector2.x, keyframeWrapper.ScreenPosition.y + vector2.y, 0f));
                    GUI.Label(new Rect((keyframeWrapper.ScreenPosition.x + vector2.x) - 4f, (keyframeWrapper.ScreenPosition.y + vector2.y) - 4f, 8f, 8f), string.Empty, TimelineTrackControl.styles.tangentStyle);
                }
                GUI.color=(wrapper.Color);
            }
            GUI.Label(new Rect(keyframeWrapper.ScreenPosition.x - 4f, keyframeWrapper.ScreenPosition.y - 4f, 8f, 8f), string.Empty, TimelineTrackControl.styles.keyframeStyle);
            GUI.color=(color);
        }
    }

    protected void drawMasterKeys(DirectorControlState state)
    {
        for (int i = 0; i < this.keyframeTimes.Count; i++)
        {
            float num2 = (this.keyframeTimes.Keys[i] * state.Scale.x) + state.Translation.x;
            int num3 = this.keyframeTimes.Values[i];
            Color color = GUI.color;
            GUI.color=((GUIUtility.hotControl == num3) ? new Color(0.5f, 0.6f, 0.905f, 1f) : color);
            GUI.Label(new Rect(num2 - 4f, this.masterKeysPosition.y + 4f, 8f, 8f), string.Empty, TimelineTrackControl.styles.keyframeStyle);
            GUI.color=(color);
        }
    }

    private Rect getViewingArea(bool isResizeEnabled, CinemaClipCurveWrapper clipCurveWrapper)
    {
        Rect rect = new Rect(float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
        foreach (CinemaMemberCurveWrapper wrapper in clipCurveWrapper.MemberCurves)
        {
            if (wrapper.IsVisible)
            {
                foreach (CinemaAnimationCurveWrapper wrapper2 in wrapper.AnimationCurves)
                {
                    if (wrapper2.IsVisible)
                    {
                        for (int i = 0; i < wrapper2.KeyframeCount; i++)
                        {
                            Keyframe keyframe = wrapper2.GetKeyframe(i);
                            rect.x=(Mathf.Min(rect.x, keyframe.time));
                            rect.width=(Mathf.Max(rect.width, keyframe.time));
                            rect.y=(Mathf.Min(rect.y, keyframe.value));
                            rect.height=(Mathf.Max(rect.height, keyframe.value));
                            if (i > 0)
                            {
                                Keyframe keyframe2 = wrapper2.GetKeyframe(i - 1);
                                float num4 = Mathf.Abs(keyframe.time - keyframe2.time) * 0.333333f;
                                float num5 = wrapper2.Evaluate(keyframe2.time + num4);
                                float num6 = wrapper2.Evaluate(keyframe.time - num4);
                                float num7 = wrapper2.Evaluate(keyframe2.time + (Mathf.Abs(keyframe.time - keyframe2.time) * 0.5f));
                                float[] singleArray1 = new float[] { rect.y, num5, num6, num7 };
                                rect.y=(Mathf.Min(singleArray1));
                                float[] singleArray2 = new float[] { rect.height, num5, num6, num7 };
                                rect.height=(Mathf.Max(singleArray2));
                            }
                        }
                    }
                }
            }
        }
        float introduced16 = rect.height;
        if ((introduced16 - rect.y) == 0f)
        {
            rect.y=(rect.y + 0f);
            rect.height=(rect.y + 1f);
        }
        if (!isResizeEnabled)
        {
            rect.y=(this.viewingSpace.y);
            rect.height=(this.viewingSpace.height);
        }
        return rect;
    }

    private void handleCurveCanvasInput(CinemaClipCurveWrapper clipCurveWrapper, DirectorControlState state)
    {
        int controlID = GUIUtility.GetControlID("CurveCanvas".GetHashCode(), (FocusType)2);
        if (((Event.current.GetTypeForControl(controlID) == EventType.MouseDown) && this.curveCanvasPosition.Contains(Event.current.mousePosition)) && (Event.current.button == 1))
        {
            float time = (Event.current.mousePosition.x - state.Translation.x) / state.Scale.x;
            CurvesContext context = new CurvesContext(clipCurveWrapper, time, state);
            this.showCurveCanvasContextMenu(context);
            if (!base.TrackControl.TargetTrack.IsLocked)
            {
                Event.current.Use();
            }
        }
    }

    public override void HandleInput(DirectorControlState state, Rect trackPosition)
    {
        CinemaClipCurveWrapper clipCurveWrapper = base.Wrapper as CinemaClipCurveWrapper;
        if (clipCurveWrapper != null)
        {
            this.handleItemInput(clipCurveWrapper, state, trackPosition);
            if ((!this.isCurveClipEmpty && this.IsEditing) && !this.isFolded)
            {
                this.handleKeyframeInput(clipCurveWrapper, state);
                this.updateMasterKeys(clipCurveWrapper);
                this.handleMasterKeysInput(clipCurveWrapper, state);
            }
        }
    }

    private void handleItemInput(CinemaClipCurveWrapper clipCurveWrapper, DirectorControlState state, Rect trackPosition)
    {
        if (base.isRenaming)
        {
            return;
        }
        float num = (clipCurveWrapper.Firetime * state.Scale.x) + state.Translation.x;
        float num2 = ((clipCurveWrapper.Firetime + clipCurveWrapper.Duration) * state.Scale.x) + state.Translation.x;
        Rect rect = new Rect(num, 0f, 5f, this.timelineItemPosition.height);
        Rect rect2 = new Rect(num + 5f, 0f, (num2 - num) - 10f, this.timelineItemPosition.height);
        Rect rect3 = new Rect(num2 - 5f, 0f, 5f, this.timelineItemPosition.height);
        EditorGUIUtility.AddCursorRect(rect, (MouseCursor)3);
        EditorGUIUtility.AddCursorRect(rect2, (MouseCursor)5);
        EditorGUIUtility.AddCursorRect(rect3, (MouseCursor)3);
        base.controlID = GUIUtility.GetControlID(base.Wrapper.Behaviour.GetInstanceID(), (FocusType)2, this.timelineItemPosition);
        int num3 = GUIUtility.GetControlID(base.Wrapper.Behaviour.GetInstanceID(), (FocusType)2, rect);
        int num4 = GUIUtility.GetControlID(base.Wrapper.Behaviour.GetInstanceID(), (FocusType)2, rect2);
        int num5 = GUIUtility.GetControlID(base.Wrapper.Behaviour.GetInstanceID(), (FocusType)2, rect3);
        if (((Event.current.GetTypeForControl(base.controlID) == EventType.MouseDown) && rect2.Contains(Event.current.mousePosition)) && (Event.current.button == 1))
        {
            if (!base.IsSelected)
            {
                GameObject[] objArray = Selection.gameObjects;
                ArrayUtility.Add<GameObject>(ref objArray, base.Wrapper.Behaviour.gameObject);
                Selection.objects=(objArray);
                base.hasSelectionChanged = true;
            }
            this.showContextMenu(base.Wrapper.Behaviour);
            if (!base.TrackControl.TargetTrack.IsLocked)
            {
                Event.current.Use();
            }
        }
        switch (Event.current.GetTypeForControl(num4))
        {
            case 0:
            {
                if (!rect2.Contains(Event.current.mousePosition) || (Event.current.button != 0))
                {
                    goto Label_0497;
                }
                GUIUtility.hotControl=(num4);
                if (!Event.current.control)
                {
                    if (Event.current.clickCount >= 2)
                    {
                        CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                        if ((wrapper != null) && (this.RequestEdit != null))
                        {
                            this.RequestEdit(this, new CurveClipWrapperEventArgs(wrapper));
                        }
                    }
                    else if (!base.IsSelected)
                    {
                        Selection.activeInstanceID=(base.Behaviour.GetInstanceID());
                    }
                    break;
                }
                if (!base.IsSelected)
                {
                    GameObject[] objArray3 = Selection.gameObjects;
                    ArrayUtility.Add<GameObject>(ref objArray3, base.Wrapper.Behaviour.gameObject);
                    Selection.objects=(objArray3);
                    base.hasSelectionChanged = true;
                    break;
                }
                GameObject[] objArray2 = Selection.gameObjects;
                ArrayUtility.Remove<GameObject>(ref objArray2, base.Wrapper.Behaviour.gameObject);
                Selection.objects=(objArray2);
                base.hasSelectionChanged = true;
                break;
            }
            case (EventType)1:
                if (GUIUtility.hotControl == num4)
                {
                    mouseDownOffset = -1f;
                    GUIUtility.hotControl=(0);
                    if (base.mouseDragActivity)
                    {
                        base.TriggerTrackItemUpdateEvent();
                    }
                    else if (!Event.current.control)
                    {
                        Selection.activeInstanceID=(base.Behaviour.GetInstanceID());
                    }
                    else if (!base.hasSelectionChanged)
                    {
                        if (!base.IsSelected)
                        {
                            GameObject[] objArray5 = Selection.gameObjects;
                            ArrayUtility.Add<GameObject>(ref objArray5, base.Wrapper.Behaviour.gameObject);
                            Selection.objects=(objArray5);
                        }
                        else
                        {
                            GameObject[] objArray4 = Selection.gameObjects;
                            ArrayUtility.Remove<GameObject>(ref objArray4, base.Wrapper.Behaviour.gameObject);
                            Selection.objects=(objArray4);
                        }
                    }
                    base.hasSelectionChanged = false;
                }
                goto Label_0497;

            case (EventType)3:
                if ((GUIUtility.hotControl == num4) && !base.hasSelectionChanged)
                {
                    Undo.RecordObject(base.Behaviour, string.Format("Changed {0}", base.Behaviour.name));
                    float firetime = (Event.current.mousePosition.x - state.Translation.x) / state.Scale.x;
                    firetime = state.SnappedTime(firetime - mouseDownOffset);
                    if (!base.mouseDragActivity)
                    {
                        base.mouseDragActivity = base.Wrapper.Firetime != firetime;
                    }
                    base.TriggerRequestTrackItemTranslate(firetime);
                }
                goto Label_0497;

            default:
                goto Label_0497;
        }
        base.mouseDragActivity = false;
        mouseDownOffset = ((Event.current.mousePosition.x - state.Translation.x) / state.Scale.x) - clipCurveWrapper.Firetime;
        if (!base.TrackControl.TargetTrack.IsLocked)
        {
            Event.current.Use();
        }
    Label_0497:
        switch (Event.current.GetTypeForControl(num3))
        {
            case 0:
                if (rect.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl=(num3);
                    mouseDownOffset = ((Event.current.mousePosition.x - state.Translation.x) / state.Scale.x) - clipCurveWrapper.Firetime;
                    if (!base.TrackControl.TargetTrack.IsLocked)
                    {
                        Event.current.Use();
                    }
                }
                break;

            case (EventType)1:
                if (GUIUtility.hotControl == num3)
                {
                    mouseDownOffset = -1f;
                    GUIUtility.hotControl=(0);
                    CinemaClipCurveWrapper wrapper2 = base.Wrapper as CinemaClipCurveWrapper;
                    if (wrapper2 != null)
                    {
                        this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper2));
                    }
                    this.haveCurvesChanged = true;
                }
                break;

            case (EventType)3:
                if (GUIUtility.hotControl == num3)
                {
                    float time = (Event.current.mousePosition.x - state.Translation.x) / state.Scale.x;
                    time = state.SnappedTime(time);
                    float num8 = 0f;
                    float num9 = clipCurveWrapper.Firetime + clipCurveWrapper.Duration;
                    foreach (CinemaActionWrapper wrapper3 in base.Track.Items)
                    {
                        if ((wrapper3 != null) && (wrapper3.Behaviour != base.Wrapper.Behaviour))
                        {
                            float num10 = wrapper3.Firetime + wrapper3.Duration;
                            if (num10 <= base.Wrapper.Firetime)
                            {
                                num8 = Mathf.Max(num8, num10);
                            }
                        }
                    }
                    time = Mathf.Max(num8, time);
                    time = Mathf.Min(num9, time);
                    if (state.ResizeOption == ResizeOption.Crop)
                    {
                        clipCurveWrapper.CropFiretime(time);
                    }
                    else if (state.ResizeOption == ResizeOption.Scale)
                    {
                        clipCurveWrapper.ScaleFiretime(time);
                    }
                }
                break;
        }
        switch (Event.current.GetTypeForControl(num5))
        {
            case 0:
                if (rect3.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl=(num5);
                    mouseDownOffset = ((Event.current.mousePosition.x - state.Translation.x) / state.Scale.x) - base.Wrapper.Firetime;
                    if (!base.TrackControl.TargetTrack.IsLocked)
                    {
                        Event.current.Use();
                    }
                }
                break;

            case (EventType)1:
                if (GUIUtility.hotControl == num5)
                {
                    mouseDownOffset = -1f;
                    GUIUtility.hotControl=(0);
                    CinemaClipCurveWrapper wrapper4 = base.Wrapper as CinemaClipCurveWrapper;
                    if (wrapper4 != null)
                    {
                        this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper4));
                    }
                    this.haveCurvesChanged = true;
                }
                break;

            case (EventType)3:
                if (GUIUtility.hotControl == num5)
                {
                    float num11 = (Event.current.mousePosition.x - state.Translation.x) / state.Scale.x;
                    num11 = state.SnappedTime(num11);
                    float positiveInfinity = float.PositiveInfinity;
                    foreach (CinemaActionWrapper wrapper5 in base.Track.Items)
                    {
                        if ((wrapper5 != null) && (wrapper5.Behaviour != base.Wrapper.Behaviour))
                        {
                            float num13 = clipCurveWrapper.Firetime + clipCurveWrapper.Duration;
                            if (wrapper5.Firetime >= num13)
                            {
                                positiveInfinity = Mathf.Min(positiveInfinity, wrapper5.Firetime);
                            }
                        }
                    }
                    num11 = Mathf.Clamp(num11, base.Wrapper.Firetime, positiveInfinity);
                    if (state.ResizeOption == ResizeOption.Crop)
                    {
                        clipCurveWrapper.CropDuration(num11 - base.Wrapper.Firetime);
                    }
                    else if (state.ResizeOption == ResizeOption.Scale)
                    {
                        clipCurveWrapper.ScaleDuration(num11 - base.Wrapper.Firetime);
                    }
                }
                break;
        }
        if (((Event.current.type == (EventType)4) && (Event.current.keyCode == (KeyCode)0x7f)) && (Selection.activeGameObject == base.Wrapper.Behaviour.gameObject))
        {
            base.deleteItem(base.Wrapper.Behaviour);
            if (!base.TrackControl.TargetTrack.IsLocked)
            {
                Event.current.Use();
            }
        }
    }

    private void handleKeyframeInput(CinemaClipCurveWrapper clipCurveWrapper, DirectorControlState state)
    {
        float introduced20 = this.viewingSpace.height;
        float verticalRange = introduced20 - this.viewingSpace.y;
        int controlID = GUIUtility.GetControlID("KeyframeControl".GetHashCode(), (FocusType)2);
        foreach (CinemaMemberCurveWrapper wrapper in clipCurveWrapper.MemberCurves)
        {
            foreach (CinemaAnimationCurveWrapper wrapper2 in wrapper.AnimationCurves)
            {
                if (wrapper2.IsVisible)
                {
                    for (int i = 0; i < wrapper2.KeyframeCount; i++)
                    {
                        Keyframe keyframe = wrapper2.GetKeyframe(i);
                        bool flag = (((this.selection.Type == wrapper.Type) && (this.selection.Property == wrapper.PropertyName)) && (this.selection.KeyId == i)) && (this.selection.CurveId == wrapper2.Id);
                        bool isBookEnd = (keyframe.time == clipCurveWrapper.Firetime) || (keyframe.time == (clipCurveWrapper.Firetime + clipCurveWrapper.Duration));
                        Vector2 keyframeScreenPosition = wrapper2.GetKeyframeScreenPosition(i);
                        Rect rect = new Rect(keyframeScreenPosition.x - 4f, keyframeScreenPosition.y - 4f, 8f, 8f);
                        switch (Event.current.GetTypeForControl(controlID))
                        {
                            case (EventType)0:
                            {
                                if (rect.Contains(Event.current.mousePosition))
                                {
                                    GUIUtility.hotControl=(controlID);
                                    Selection.activeInstanceID=(base.Wrapper.Behaviour.GetInstanceID());
                                    this.selection.Type = wrapper.Type;
                                    this.selection.Property = wrapper.PropertyName;
                                    this.selection.CurveId = wrapper2.Id;
                                    this.selection.KeyId = i;
                                    if (!base.TrackControl.TargetTrack.IsLocked)
                                    {
                                        Event.current.Use();
                                    }
                                }
                                continue;
                            }
                            case (EventType)1:
                            {
                                if ((GUIUtility.hotControl != controlID) || (Event.current.button != 1))
                                {
                                    break;
                                }
                                if (flag)
                                {
                                    this.showKeyframeContextMenu(wrapper2, i, isBookEnd);
                                    GUIUtility.hotControl=(0);
                                    if (!base.TrackControl.TargetTrack.IsLocked)
                                    {
                                        Event.current.Use();
                                    }
                                }
                                continue;
                            }
                            case (EventType)2:
                            {
                                continue;
                            }
                            case (EventType)3:
                            {
                                 
                                if (((GUIUtility.hotControl == controlID) && (Event.current.button == 0)) && flag)
                                {
                                    Keyframe keyframe2 = wrapper2.GetKeyframe(i);
                                    float time = (Event.current.mousePosition.x - state.Translation.x) / state.Scale.x;
                                    float introduced21 = this.curveTrackSafeArea.y;
                                    float num7 = ((((introduced21 + this.curveTrackSafeArea.height) - Event.current.mousePosition.y) / this.curveTrackSafeArea.height) * verticalRange) + this.viewingSpace.y;
                                    time = state.SnappedTime(time);
                                    if (isBookEnd)
                                    {
                                        time = keyframe2.time;
                                    }
                                    float introduced22 = keyframe2.inTangent;
                                    Keyframe kf = new Keyframe(time, num7, introduced22, keyframe2.outTangent);
                                    kf.tangentMode=(keyframe2.tangentMode);
                                    if (((time > clipCurveWrapper.Firetime) && (time < (clipCurveWrapper.Firetime + clipCurveWrapper.Duration))) | isBookEnd)
                                    {
                                        this.selection.KeyId = wrapper2.MoveKey(i, kf);
                                        this.haveCurvesChanged = true;
                                    }
                                }
                                continue;
                            }
                            default:
                            {
                                continue;
                            }
                        }
                        if ((GUIUtility.hotControl == controlID) && flag)
                        {
                            GUIUtility.hotControl=(0);
                            if (!base.TrackControl.TargetTrack.IsLocked)
                            {
                                Event.current.Use();
                            }
                            if (this.CurvesChanged != null)
                            {
                                CinemaClipCurveWrapper wrapper3 = base.Wrapper as CinemaClipCurveWrapper;
                                if (wrapper3 != null)
                                {
                                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper3));
                                }
                                this.haveCurvesChanged = true;
                            }
                        }
                    }
                }
            }
        }
        this.handleKeyframeTangentInput(clipCurveWrapper, state, verticalRange);
    }

    private void handleKeyframeTangentInput(CinemaClipCurveWrapper clipCurveWrapper, DirectorControlState state, float verticalRange)
    {
        foreach (CinemaMemberCurveWrapper wrapper in clipCurveWrapper.MemberCurves)
        {
            foreach (CinemaAnimationCurveWrapper wrapper2 in wrapper.AnimationCurves)
            {
                for (int i = 0; i < wrapper2.KeyframeCount; i++)
                {
                    int controlID;
                    int num7;
                    Keyframe keyframe = wrapper2.GetKeyframe(i);
                    CinemaKeyframeWrapper keyframeWrapper = wrapper2.GetKeyframeWrapper(i);
                   // bool flag1 = (keyframe.time == clipCurveWrapper.Firetime) || (keyframe.time == (clipCurveWrapper.Firetime + clipCurveWrapper.Duration));
                    if (((((this.selection.Type != wrapper.Type) || (this.selection.Property != wrapper.PropertyName)) || (this.selection.KeyId != i)) || (this.selection.CurveId != wrapper2.Id)) || wrapper2.IsAuto(i))
                    {
                        continue;
                    }
                    if (((i > 0) && !wrapper2.IsLeftLinear(i)) && !wrapper2.IsLeftConstant(i))
                    {
                        Vector2 vector = new Vector2(keyframeWrapper.InTangentControlPointPosition.x - keyframeWrapper.ScreenPosition.x, keyframeWrapper.InTangentControlPointPosition.y - keyframeWrapper.ScreenPosition.y);
                        vector.Normalize();
                        vector = (Vector2) (vector * 30f);
                        Rect rect = new Rect((keyframeWrapper.ScreenPosition.x + vector.x) - 4f, (keyframeWrapper.ScreenPosition.y + vector.y) - 4f, 8f, 8f);
                        controlID = GUIUtility.GetControlID("TangentIn".GetHashCode(), (FocusType)2);
                        switch (Event.current.GetTypeForControl(controlID))
                        {
                            case 0:
                                if (rect.Contains(Event.current.mousePosition))
                                {
                                    GUIUtility.hotControl=(controlID);
                                    if (!base.TrackControl.TargetTrack.IsLocked)
                                    {
                                        Event.current.Use();
                                    }
                                }
                                break;

                            case (EventType)1:
                                goto Label_020B;

                            case (EventType)3:
                                goto Label_0258;
                        }
                    }
                    goto Label_034E;
                Label_020B:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl=(0);
                        if (this.CurvesChanged != null)
                        {
                            CinemaClipCurveWrapper wrapper4 = base.Wrapper as CinemaClipCurveWrapper;
                            if (wrapper4 != null)
                            {
                                this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper4));
                            }
                            this.haveCurvesChanged = true;
                        }
                    }
                    goto Label_034E;
                Label_0258:
                    if (GUIUtility.hotControl == controlID)
                    {
                        float introduced28 = this.curveTrackSafeArea.y;
                        float introduced29 = keyframe.time;
                        Vector2 vector2 = new Vector2((Event.current.mousePosition.x - state.Translation.x) / state.Scale.x, ((((introduced28 + this.curveTrackSafeArea.height) - Event.current.mousePosition.y) / this.curveTrackSafeArea.height) * verticalRange) + this.viewingSpace.y) - new Vector2(introduced29, keyframe.value);
                        float num5 = vector2.y / vector2.x;
                        float num6 = keyframe.outTangent;
                        if (wrapper2.IsFreeSmooth(i))
                        {
                            num6 = num5;
                        }
                        float introduced30 = keyframe.time;
                        Keyframe kf = new Keyframe(introduced30, keyframe.value, num5, num6);
                        kf.tangentMode=(keyframe.tangentMode);
                        wrapper2.MoveKey(i, kf);
                        this.haveCurvesChanged = true;
                    }
                Label_034E:
                    if (((i < (wrapper2.KeyframeCount - 1)) && !wrapper2.IsRightLinear(i)) && !wrapper2.IsRightConstant(i))
                    {
                        Vector2 vector3 = new Vector2(keyframeWrapper.OutTangentControlPointPosition.x - keyframeWrapper.ScreenPosition.x, keyframeWrapper.OutTangentControlPointPosition.y - keyframeWrapper.ScreenPosition.y);
                        vector3.Normalize();
                        vector3 = (Vector2) (vector3 * 30f);
                        Rect rect2 = new Rect((keyframeWrapper.ScreenPosition.x + vector3.x) - 4f, (keyframeWrapper.ScreenPosition.y + vector3.y) - 4f, 8f, 8f);
                        num7 = GUIUtility.GetControlID("TangentOut".GetHashCode(), (FocusType)2);
                        switch (Event.current.GetTypeForControl(num7))
                        {
                            case 0:
                                if (rect2.Contains(Event.current.mousePosition))
                                {
                                    GUIUtility.hotControl=(num7);
                                    if (!base.TrackControl.TargetTrack.IsLocked)
                                    {
                                        Event.current.Use();
                                    }
                                }
                                break;

                            case (EventType)1:
                                goto Label_048A;

                            case (EventType)3:
                                goto Label_04D7;
                        }
                    }
                    continue;
                Label_048A:
                    if (GUIUtility.hotControl == num7)
                    {
                        GUIUtility.hotControl=(0);
                        if (this.CurvesChanged != null)
                        {
                            CinemaClipCurveWrapper wrapper5 = base.Wrapper as CinemaClipCurveWrapper;
                            if (wrapper5 != null)
                            {
                                this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper5));
                            }
                            this.haveCurvesChanged = true;
                        }
                    }
                    continue;
                Label_04D7:
                    if (GUIUtility.hotControl == num7)
                    {
                        float introduced31 = this.curveTrackSafeArea.y;
                        Vector2 vector4 = new Vector2((Event.current.mousePosition.x - state.Translation.x) / state.Scale.x, ((((introduced31 + this.curveTrackSafeArea.height) - Event.current.mousePosition.y) / this.curveTrackSafeArea.height) * verticalRange) + this.viewingSpace.y);
                        float introduced32 = keyframe.time;
                        Vector2 vector5 = new Vector2(introduced32, keyframe.value) - vector4;
                        float num8 = vector5.y / vector5.x;
                        float num9 = keyframe.inTangent;
                        if (wrapper2.IsFreeSmooth(i))
                        {
                            num9 = num8;
                        }
                        float introduced33 = keyframe.time;
                        Keyframe keyframe3 = new Keyframe(introduced33, keyframe.value, num9, num8);
                        keyframe3.tangentMode=(keyframe.tangentMode);
                        wrapper2.MoveKey(i, keyframe3);
                        this.haveCurvesChanged = true;
                    }
                }
            }
        }
    }

    private void handleMasterKeysInput(CinemaClipCurveWrapper clipCurveWrapper, DirectorControlState state)
    {
        bool flag = false;
        for (int i = 0; i < this.keyframeTimes.Count; i++)
        {
            float num6;
            bool flag4;
            float num2 = this.keyframeTimes.Keys[i];
            float num3 = (num2 * state.Scale.x) + state.Translation.x;
            Rect rect = new Rect(num3 - 4f, this.masterKeysPosition.y + 4f, 8f, 8f);
            int num4 = this.keyframeTimes.Values[i];
            bool flag2 = (num2 == clipCurveWrapper.Firetime) || (num2 == (clipCurveWrapper.Firetime + clipCurveWrapper.Duration));
            bool isDeletable = !flag2 && (this.keyframeTimes.Count > 2);
            if (flag)
            {
                break;
            }
            switch (Event.current.GetTypeForControl(num4))
            {
                case 0:
                {
                    if (!rect.Contains(Event.current.mousePosition) || (Event.current.button != 0))
                    {
                        break;
                    }
                    GUIUtility.hotControl=(num4);
                    this.selection.Reset();
                    if (!base.TrackControl.TargetTrack.IsLocked)
                    {
                        Event.current.Use();
                    }
                    continue;
                }
                case (EventType)1:
                {
                    if (GUIUtility.hotControl == num4)
                    {
                        GUIUtility.hotControl=(0);
                        if (this.CurvesChanged != null)
                        {
                            CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                            if (wrapper != null)
                            {
                                this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                            }
                            this.haveCurvesChanged = true;
                        }
                    }
                    continue;
                }
                case (EventType)2:
                {
                    continue;
                }
                case (EventType)3:
                    if (((GUIUtility.hotControl != num4) || (Event.current.button != 0)) || flag2)
                    {
                        continue;
                    }
                    Undo.RecordObject(base.Wrapper.Behaviour, "Moved Keyframes");
                    num6 = (Event.current.mousePosition.x - state.Translation.x) / state.Scale.x;
                    num6 = Mathf.Clamp(num6, clipCurveWrapper.Firetime, clipCurveWrapper.Firetime + clipCurveWrapper.Duration);
                    flag4 = false;
                    num6 = state.SnappedTime(num6);
                    if (Event.current.delta.x != 0f)
                    {
                        goto Label_0281;
                    }
                    flag4 = true;
                    goto Label_0377;

                default:
                {
                    continue;
                }
            }
            if (rect.Contains(Event.current.mousePosition) && (Event.current.button == 1))
            {
                float time = num2;
                CurvesContext context = new CurvesContext(clipCurveWrapper, time, state);
                this.showMasterKeyContextMenu(context, isDeletable);
                if (!base.TrackControl.TargetTrack.IsLocked)
                {
                    Event.current.Use();
                }
            }
            continue;
        Label_0281:
            if ((i > 0) && (num6 <= this.keyframeTimes.Keys[i - 1]))
            {
                if (Math.Abs((float) (num6 - this.keyframeTimes.Keys[i - 1])) < 0.01f)
                {
                    flag4 = true;
                }
                else if (num6 < this.keyframeTimes.Keys[i - 1])
                {
                    GUIUtility.hotControl=(this.keyframeTimes.Values[i - 1]);
                }
            }
            else if ((i < (this.keyframeTimes.Count - 1)) && (num6 >= this.keyframeTimes.Keys[i + 1]))
            {
                if (Math.Abs((float) (num6 - this.keyframeTimes.Keys[i + 1])) < 0.01f)
                {
                    flag4 = true;
                }
                else if (num6 > this.keyframeTimes.Keys[i + 1])
                {
                    GUIUtility.hotControl=(this.keyframeTimes.Values[i + 1]);
                }
            }
        Label_0377:
            if (!flag4 && (num6 != num2))
            {
                foreach (CinemaMemberCurveWrapper wrapper2 in clipCurveWrapper.MemberCurves)
                {
                    if (wrapper2.IsVisible)
                    {
                        foreach (CinemaAnimationCurveWrapper wrapper3 in wrapper2.AnimationCurves)
                        {
                            if (wrapper3.IsVisible)
                            {
                                for (int j = 0; j < wrapper3.KeyframeCount; j++)
                                {
                                    Keyframe keyframe = wrapper3.GetKeyframe(j);
                                    if (keyframe.time == num2)
                                    {
                                        wrapper3.GetKeyframeWrapper(j);
                                        float introduced23 = keyframe.value;
                                        float introduced24 = keyframe.inTangent;
                                        Keyframe kf = new Keyframe(num6, introduced23, introduced24, keyframe.outTangent);
                                        kf.tangentMode=(keyframe.tangentMode);
                                        wrapper3.MoveKey(j, kf);
                                        flag = true;
                                        this.haveCurvesChanged = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public override void PreUpdate(DirectorControlState state, Rect trackPosition)
    {
        CinemaClipCurveWrapper clipWrapper = base.Wrapper as CinemaClipCurveWrapper;
        if (clipWrapper != null)
        {
            this.isCurveClipEmpty = clipWrapper.IsEmpty;
            this.isFolded = trackPosition.height == 17f;
            if (GUIUtility.hotControl == 0)
            {
                this.UpdateCurveWrappers(clipWrapper);
                this.viewingSpace = this.getViewingArea(this.isAutoResizeEnabled, clipWrapper);
            }
            float num = (clipWrapper.Firetime * state.Scale.x) + state.Translation.x;
            float num2 = ((clipWrapper.Firetime + clipWrapper.Duration) * state.Scale.x) + state.Translation.x;
            base.controlPosition = new Rect(num, 0f, num2 - num, trackPosition.height);
            if (this.isCurveClipEmpty || this.isFolded)
            {
                this.timelineItemPosition = base.controlPosition;
            }
            else
            {
                float introduced3 = this.controlPosition.x;
                float introduced4 = this.controlPosition.y;
                this.timelineItemPosition = new Rect(introduced3, introduced4, this.controlPosition.width, 17f);
            }
            if (this.isEditing)
            {
                float introduced5 = this.timelineItemPosition.y;
                this.masterKeysPosition = new Rect(this.controlPosition.x, introduced5 + this.timelineItemPosition.height, this.controlPosition.width, 17f);
                float introduced6 = this.masterKeysPosition.y;
                this.curveCanvasPosition = new Rect(this.controlPosition.x, introduced6 + this.masterKeysPosition.height, this.controlPosition.width, (trackPosition.height - this.timelineItemPosition.height) - this.masterKeysPosition.height);
            }
            else
            {
                float introduced7 = this.timelineItemPosition.y;
                this.curveCanvasPosition = new Rect(this.controlPosition.x, introduced7 + this.timelineItemPosition.height, this.controlPosition.width, trackPosition.height - this.timelineItemPosition.height);
            }
            float introduced8 = this.curveCanvasPosition.x;
            float introduced9 = this.curveCanvasPosition.width;
            this.curveTrackSafeArea = new Rect(introduced8, this.curveCanvasPosition.y + 8f, introduced9, this.curveCanvasPosition.height - 16f);
            if (!this.isCurveClipEmpty)
            {
                this.updateKeyframes(clipWrapper, state);
            }
        }
    }

    private void setKeyAuto(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && !context.curveWrapper.IsAuto(context.key))
        {
            context.curveWrapper.SetKeyAuto(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void setKeyBothConstant(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && (!context.curveWrapper.IsRightConstant(context.key) || !context.curveWrapper.IsLeftConstant(context.key)))
        {
            context.curveWrapper.SetKeyLeftConstant(context.key);
            context.curveWrapper.SetKeyRightConstant(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void setKeyBothFree(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && (!context.curveWrapper.IsRightFree(context.key) || !context.curveWrapper.IsLeftFree(context.key)))
        {
            context.curveWrapper.SetKeyLeftFree(context.key);
            context.curveWrapper.SetKeyRightFree(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void setKeyBothLinear(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && (!context.curveWrapper.IsRightLinear(context.key) || !context.curveWrapper.IsLeftLinear(context.key)))
        {
            context.curveWrapper.SetKeyLeftLinear(context.key);
            context.curveWrapper.SetKeyRightLinear(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void setKeyBroken(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && !context.curveWrapper.IsRightFree(context.key))
        {
            context.curveWrapper.SetKeyBroken(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void setKeyFlat(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if (context != null)
        {
            Keyframe keyframe = context.curveWrapper.GetKeyframe(context.key);
            if (((keyframe.tangentMode != 0) || (keyframe.inTangent != 0f)) || (keyframe.outTangent != 0f))
            {
                context.curveWrapper.FlattenKey(context.key);
                if (this.CurvesChanged != null)
                {
                    CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                    if (wrapper != null)
                    {
                        this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                    }
                }
                this.haveCurvesChanged = true;
            }
        }
    }

    private void setKeyFreeSmooth(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && !context.curveWrapper.IsFreeSmooth(context.key))
        {
            context.curveWrapper.SetKeyFreeSmooth(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void setKeyLeftConstant(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && !context.curveWrapper.IsLeftConstant(context.key))
        {
            context.curveWrapper.SetKeyLeftConstant(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void setKeyLeftFree(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && !context.curveWrapper.IsLeftFree(context.key))
        {
            context.curveWrapper.SetKeyLeftFree(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void setKeyLeftLinear(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && !context.curveWrapper.IsLeftLinear(context.key))
        {
            context.curveWrapper.SetKeyLeftLinear(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void setKeyRightConstant(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && !context.curveWrapper.IsRightConstant(context.key))
        {
            context.curveWrapper.SetKeyRightConstant(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void setKeyRightFree(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && !context.curveWrapper.IsRightFree(context.key))
        {
            context.curveWrapper.SetKeyRightFree(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void setKeyRightLinear(object userData)
    {
        KeyframeContext context = userData as KeyframeContext;
        if ((context != null) && !context.curveWrapper.IsLeftLinear(context.key))
        {
            context.curveWrapper.SetKeyRightLinear(context.key);
            if (this.CurvesChanged != null)
            {
                CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
                if (wrapper != null)
                {
                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper));
                }
            }
            this.haveCurvesChanged = true;
        }
    }

    private void showCurveCanvasContextMenu(CurvesContext context)
    {
        GenericMenu menu1 = new GenericMenu();
        menu1.AddItem(new GUIContent("Add Keyframe"), false, new GenericMenu.MenuFunction2(addKeyframes), context);
        menu1.ShowAsContext();
    }

    private void showKeyframeContextMenu(CinemaAnimationCurveWrapper animationCurve, int i, bool isBookEnd)
    {
        GenericMenu menu = new GenericMenu();
        CinemaKeyframeWrapper keyframeWrapper = animationCurve.GetKeyframeWrapper(i);
        KeyframeContext context = new KeyframeContext(animationCurve, i, keyframeWrapper);
        Keyframe keyframe = animationCurve.GetKeyframe(i);
        if (!isBookEnd)
        {
            menu.AddItem(new GUIContent("Delete Key"), false, new GenericMenu.MenuFunction2(deleteKey), context);
            menu.AddSeparator(string.Empty);
        }
        menu.AddItem(new GUIContent("Auto"), animationCurve.IsAuto(i), new GenericMenu.MenuFunction2(setKeyAuto), context);
        menu.AddItem(new GUIContent("Free Smooth"), animationCurve.IsFreeSmooth(i), new GenericMenu.MenuFunction2(setKeyFreeSmooth), context);
        menu.AddItem(new GUIContent("Flat"), (animationCurve.IsFreeSmooth(i) && (keyframe.inTangent == 0f)) && (keyframe.outTangent == 0f), new GenericMenu.MenuFunction2(setKeyFlat), context);
        menu.AddItem(new GUIContent("Broken"), animationCurve.IsBroken(i), new GenericMenu.MenuFunction2(setKeyBroken), context);
        menu.AddSeparator(string.Empty);
        menu.AddItem(new GUIContent("Left Tangent/Free"), animationCurve.IsLeftFree(i), new GenericMenu.MenuFunction2(setKeyLeftFree), context);
        menu.AddItem(new GUIContent("Left Tangent/Linear"), animationCurve.IsLeftLinear(i), new GenericMenu.MenuFunction2(setKeyLeftLinear), context);
        menu.AddItem(new GUIContent("Left Tangent/Constant"), animationCurve.IsLeftConstant(i), new GenericMenu.MenuFunction2(setKeyLeftConstant), context);
        menu.AddItem(new GUIContent("Right Tangent/Free"), animationCurve.IsRightFree(i), new GenericMenu.MenuFunction2(setKeyRightFree), context);
        menu.AddItem(new GUIContent("Right Tangent/Linear"), animationCurve.IsRightLinear(i), new GenericMenu.MenuFunction2(setKeyRightLinear), context);
        menu.AddItem(new GUIContent("Right Tangent/Constant"), animationCurve.IsRightConstant(i), new GenericMenu.MenuFunction2(setKeyRightConstant), context);
        menu.AddItem(new GUIContent("Both Tangents/Free"), animationCurve.IsLeftFree(i) && animationCurve.IsRightFree(i), new GenericMenu.MenuFunction2(setKeyBothFree), context);
        menu.AddItem(new GUIContent("Both Tangents/Linear"), animationCurve.IsLeftLinear(i) && animationCurve.IsRightLinear(i), new GenericMenu.MenuFunction2(setKeyBothLinear), context);
        menu.AddItem(new GUIContent("Both Tangents/Constant"), animationCurve.IsLeftConstant(i) && animationCurve.IsRightConstant(i), new GenericMenu.MenuFunction2(setKeyBothConstant), context);
        menu.ShowAsContext();
    }

    private void showMasterKeyContextMenu(CurvesContext context, bool isDeletable)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Snap scrubber"), false, new GenericMenu.MenuFunction2(snapScrubber), context);
        if (isDeletable)
        {
            menu.AddSeparator(string.Empty);
            menu.AddItem(new GUIContent("Delete Keyframes"), false, new GenericMenu.MenuFunction2(deleteKeyframes), context);
        }
        menu.ShowAsContext();
    }

    private void snapScrubber(object userData)
    {
        CurvesContext context = userData as CurvesContext;
        if ((context != null) && (this.SnapScrubber != null))
        {
            this.SnapScrubber(this, new CurveClipScrubberEventArgs(base.Wrapper.Behaviour, context.time));
            context.state.IsInPreviewMode = true;
            context.state.ScrubberPosition = context.time;
        }
    }

    private void toggleCurveVisibility(object userData)
    {
        CinemaAnimationCurveWrapper wrapper = userData as CinemaAnimationCurveWrapper;
        if (wrapper != null)
        {
            wrapper.IsVisible = !wrapper.IsVisible;
        }
    }

    private void toggleMemberVisibility(object userData)
    {
        CinemaMemberCurveWrapper wrapper = userData as CinemaMemberCurveWrapper;
        if (wrapper != null)
        {
            wrapper.IsVisible = !wrapper.IsVisible;
        }
    }

    internal override void Translate(float amount)
    {
        CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
        if (wrapper != null)
        {
            wrapper.TranslateCurves(amount);
            this.haveCurvesChanged = true;
        }
    }

    public abstract void UpdateCurveWrappers(CinemaClipCurveWrapper clipWrapper);
    private void updateFooter(Rect position)
    {
        float num = position.width / 3f;
        Rect rect = new Rect(position.x, position.y, num, position.height);
        Rect rect2 = new Rect(position.x + num, position.y, num, position.height);
        this.isAutoResizeEnabled = GUI.Toggle(rect, this.isAutoResizeEnabled, "Auto Resize", EditorStyles.miniButton);
        float num2 = EditorGUI.FloatField(rect2, this.viewingSpace.y);
        float num3 = EditorGUI.FloatField(new Rect(position.x + (num * 2f), position.y, num, position.height), this.viewingSpace.height);
        if ((num2 != this.viewingSpace.y) || (num3 != this.viewingSpace.height))
        {
            this.isAutoResizeEnabled = false;
            this.viewingSpace.y=(num2);
            this.viewingSpace.height=(num3);
        }
    }

    internal void UpdateHeaderArea(DirectorControlState state, Rect controlHeaderArea)
    {
        CinemaClipCurveWrapper wrapper = base.Wrapper as CinemaClipCurveWrapper;
        if (wrapper != null)
        {
            Rect rect = new Rect(controlHeaderArea.x, controlHeaderArea.y, controlHeaderArea.width, controlHeaderArea.height - 17f);
            Rect position = new Rect(controlHeaderArea.x, (controlHeaderArea.y + controlHeaderArea.height) - 17f, controlHeaderArea.width, 17f);
            GUILayout.BeginArea(rect);
            Rect rect3 = new Rect(controlHeaderArea.width - 15f, 0f, 15f, rect.height);
            this.scrollBarPosition = GUI.VerticalScrollbar(rect3, this.scrollBarPosition, Mathf.Min(rect.height, wrapper.RowCount * 17f), 0f, wrapper.RowCount * 17f);
            float num = (controlHeaderArea.width - 12f) - rect3.width;
            int num2 = 0;
            foreach (CinemaMemberCurveWrapper wrapper2 in wrapper.MemberCurves)
            {
                Rect rect4 = new Rect(0f, (17f * num2) - this.scrollBarPosition, num * 0.66f, 17f);
                Rect rect5 = new Rect((num * 0.8f) + 4f, (17f * num2) - this.scrollBarPosition, 32f, 16f);
                string userFriendlyName = DirectorControlHelper.GetUserFriendlyName(wrapper2.Type, wrapper2.PropertyName);
                string str2 = (userFriendlyName == string.Empty) ? wrapper2.Type : string.Format("{0}.{1}", wrapper2.Type, userFriendlyName);
                wrapper2.IsFoldedOut = EditorGUI.Foldout(rect4, wrapper2.IsFoldedOut, new GUIContent(str2, wrapper2.Texture));
                GUI.Box(rect5, string.Empty, TimelineTrackControl.styles.keyframeContextStyle);
                int num4 = GUIUtility.GetControlID(base.Wrapper.Behaviour.GetInstanceID(), (FocusType)2, rect5);
                if (((Event.current.GetTypeForControl(num4) == EventType.MouseUp) && rect5.Contains(Event.current.mousePosition)) && (Event.current.button == 0))
                {
                    GenericMenu menu1 = new GenericMenu();
                    menu1.AddItem(new GUIContent("Visible"), wrapper2.IsVisible, new GenericMenu.MenuFunction2(toggleMemberVisibility), wrapper2);
                    float introduced31 = rect5.x;
                    float introduced32 = rect5.y;
                    menu1.DropDown(new Rect(introduced31, introduced32 + rect5.height, 0f, 0f));
                }
                num2++;
                if (wrapper2.IsFoldedOut)
                {
                    foreach (CinemaAnimationCurveWrapper wrapper3 in wrapper2.AnimationCurves)
                    {
                        Rect rect6 = new Rect(12f, (17f * num2) - this.scrollBarPosition, num * 0.5f, 17f);
                        float introduced33 = rect6.x;
                        Rect rect7 = new Rect(introduced33 + rect6.width, (17f * num2) - this.scrollBarPosition, num * 0.3f, 17f);
                        float introduced34 = rect7.x;
                        Rect rect8 = new Rect((introduced34 + rect7.width) + 4f, (17f * num2) - this.scrollBarPosition, 32f, 16f);
                        string str3 = (userFriendlyName == string.Empty) ? wrapper3.Label : string.Format("{0}.{1}", userFriendlyName, wrapper3.Label);
                        EditorGUI.LabelField(rect6, str3);
                        float num6 = wrapper3.Evaluate(state.ScrubberPosition);
                        GUIStyle style = EditorStyles.toolbarTextField;
                        float newValue = EditorGUI.FloatField(rect7, num6, style);
                        if (((newValue != num6) && (state.ScrubberPosition >= wrapper.Firetime)) && (state.ScrubberPosition <= (wrapper.Firetime + wrapper.Duration)))
                        {
                            this.updateOrAddKeyframe(wrapper3, state.ScrubberPosition, newValue);
                            if (this.CurvesChanged != null)
                            {
                                CinemaClipCurveWrapper wrapper4 = base.Wrapper as CinemaClipCurveWrapper;
                                if (wrapper4 != null)
                                {
                                    this.CurvesChanged(this, new CurveClipWrapperEventArgs(wrapper4));
                                }
                            }
                            this.haveCurvesChanged = true;
                        }
                        Color color = GUI.color;
                        GUI.color=(wrapper3.Color);
                        GUI.Box(rect8, string.Empty, TimelineTrackControl.styles.keyframeContextStyle);
                        int num8 = GUIUtility.GetControlID(base.Wrapper.Behaviour.GetInstanceID(), (FocusType)2, rect8);
                        if (((Event.current.GetTypeForControl(num8) == EventType.MouseUp) && rect8.Contains(Event.current.mousePosition)) && (Event.current.button == 0))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Visible"), wrapper3.IsVisible, new GenericMenu.MenuFunction2(toggleCurveVisibility), wrapper3);
                            if ((state.ScrubberPosition >= wrapper.Firetime) && (state.ScrubberPosition <= (wrapper.Firetime + wrapper.Duration)))
                            {
                                menu.AddSeparator(string.Empty);
                                CurveContext context = new CurveContext(wrapper3, state, state.ScrubberPosition);
                                menu.AddItem(new GUIContent("Add Key"), false, new GenericMenu.MenuFunction2(addKeyToCurve), context);
                            }
                            float introduced35 = rect8.x;
                            float introduced36 = rect8.y;
                            menu.DropDown(new Rect(introduced35, introduced36 + rect8.height, 0f, 0f));
                        }
                        GUI.color=(color);
                        num2++;
                    }
                }
            }
            GUILayout.EndArea();
            this.updateFooter(position);
        }
    }

    private void updateKeyframes(CinemaClipCurveWrapper clipWrapper, DirectorControlState state)
    {
        float introduced20 = this.viewingSpace.height;
        float num = introduced20 - this.viewingSpace.y;
        float introduced21 = this.curveTrackSafeArea.y;
        float num2 = introduced21 + this.curveTrackSafeArea.height;
        for (int i = 0; i < clipWrapper.MemberCurves.Length; i++)
        {
            CinemaMemberCurveWrapper wrapper = clipWrapper.MemberCurves[i];
            for (int j = 0; j < wrapper.AnimationCurves.Length; j++)
            {
                CinemaAnimationCurveWrapper wrapper2 = wrapper.AnimationCurves[j];
                for (int k = 0; k < wrapper2.KeyframeCount; k++)
                {
                    Keyframe keyframe = wrapper2.GetKeyframe(k);
                    Vector2 vector = new Vector2(state.TimeToPosition(keyframe.time), num2 - (((keyframe.value - this.viewingSpace.y) / num) * this.curveTrackSafeArea.height));
                    CinemaKeyframeWrapper keyframeWrapper = wrapper2.GetKeyframeWrapper(k);
                    keyframeWrapper.ScreenPosition = vector;
                    if (k < (wrapper2.KeyframeCount - 1))
                    {
                        float num6 = Mathf.Abs(wrapper2.GetKeyframe(k + 1).time - keyframe.time) * 0.3333333f;
                        float num7 = keyframe.outTangent;
                        if (float.IsPositiveInfinity(keyframe.outTangent))
                        {
                            num7 = 0f;
                        }
                        Vector2 vector2 = new Vector2(keyframe.time + num6, keyframe.value + (num6 * num7));
                        Vector2 vector3 = new Vector2((vector2.x * state.Scale.x) + state.Translation.x, num2 - (((vector2.y - this.viewingSpace.y) / num) * this.curveTrackSafeArea.height));
                        keyframeWrapper.OutTangentControlPointPosition = vector3;
                    }
                    if (k > 0)
                    {
                        float num8 = Mathf.Abs(wrapper2.GetKeyframe(k - 1).time - keyframe.time) * 0.3333333f;
                        float num9 = keyframe.inTangent;
                        if (float.IsPositiveInfinity(keyframe.inTangent))
                        {
                            num9 = 0f;
                        }
                        Vector2 vector4 = new Vector2(keyframe.time - num8, keyframe.value - (num8 * num9));
                        Vector2 vector5 = new Vector2((vector4.x * state.Scale.x) + state.Translation.x, num2 - (((vector4.y - this.viewingSpace.y) / num) * this.curveTrackSafeArea.height));
                        keyframeWrapper.InTangentControlPointPosition = vector5;
                    }
                }
            }
        }
    }

    private void updateMasterKeys(CinemaClipCurveWrapper clipWrapper)
    {
        this.keyframeTimes = new SortedList<float, int>();
        foreach (CinemaMemberCurveWrapper wrapper in clipWrapper.MemberCurves)
        {
            if (wrapper.IsVisible)
            {
                foreach (CinemaAnimationCurveWrapper wrapper2 in wrapper.AnimationCurves)
                {
                    if (wrapper2.IsVisible)
                    {
                        for (int j = 0; j < wrapper2.KeyframeCount; j++)
                        {
                            Keyframe keyframe = wrapper2.GetKeyframe(j);
                            if (!this.keyframeTimes.ContainsKey(keyframe.time))
                            {
                                this.keyframeTimes.Add(keyframe.time, 0);
                            }
                        }
                    }
                }
            }
        }
        for (int i = 0; i < this.keyframeTimes.Count; i++)
        {
            this.keyframeTimes[this.keyframeTimes.Keys[i]] = GUIUtility.GetControlID("MasterKeyframe".GetHashCode(), (FocusType)2);
        }
    }

    private void updateOrAddKeyframe(CinemaAnimationCurveWrapper curveWrapper, float time, float newValue)
    {
        bool flag = false;
        for (int i = 0; i < curveWrapper.KeyframeCount; i++)
        {
            Keyframe keyframe = curveWrapper.GetKeyframe(i);
            if (keyframe.time == time)
            {
                float introduced4 = keyframe.inTangent;
                Keyframe kf = new Keyframe(keyframe.time, newValue, introduced4, keyframe.outTangent);
                kf.tangentMode=(keyframe.tangentMode);
                curveWrapper.MoveKey(i, kf);
                flag = true;
            }
        }
        if (!flag)
        {
            curveWrapper.AddKey(time, newValue);
        }
    }

    public bool HaveCurvesChanged
    {
        get
        {
            return this.haveCurvesChanged;
        }
        set
        {
            this.haveCurvesChanged = value;
        }
    }

    public bool IsEditing
    {
        get
        {
            return this.isEditing;
        }
        set
        {
            this.isEditing = value;
        }
    }

    private class CurveContext
    {
        public CinemaAnimationCurveWrapper curveWrapper;
        public DirectorControlState state;
        public float time;

        public CurveContext(CinemaAnimationCurveWrapper curveWrapper, DirectorControlState state, float time)
        {
            this.curveWrapper = curveWrapper;
            this.state = state;
            this.time = time;
        }
    }

    private class CurvesContext
    {
        public DirectorControlState state;
        public float time;
        public CinemaClipCurveWrapper wrapper;

        public CurvesContext(CinemaClipCurveWrapper wrapper, float time, DirectorControlState state)
        {
            this.wrapper = wrapper;
            this.time = time;
            this.state = state;
        }
    }

    private class KeyframeContext
    {
        public CinemaKeyframeWrapper ckw;
        public CinemaAnimationCurveWrapper curveWrapper;
        public int key;

        public KeyframeContext(CinemaAnimationCurveWrapper curveWrapper, int key, CinemaKeyframeWrapper ckw)
        {
            this.curveWrapper = curveWrapper;
            this.key = key;
            this.ckw = ckw;
        }
    }
}

