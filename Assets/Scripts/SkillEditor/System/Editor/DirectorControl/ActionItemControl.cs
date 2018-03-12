using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ActionItemControl : TrackItemControl
{
    protected Texture actionIcon;
    protected const float ITEM_RESIZE_HANDLE_SIZE = 5f;
    private static float mouseDownOffset = -1f;

    [field: CompilerGenerated]
    public event ActionItemEventHandler AlterAction;

    internal override void ConfirmTranslate()
    {
        CinemaActionWrapper wrapper = base.Wrapper as CinemaActionWrapper;
        if ((wrapper != null) && (this.AlterAction != null))
        {
            this.AlterAction(this, new ActionItemEventArgs(wrapper.Behaviour, wrapper.Firetime, wrapper.Duration));
        }
    }

    private bool doActionsConflict(float firetime, float endtime, float newFiretime, float newEndtime)
    {
        if (((newFiretime < firetime) || (newFiretime >= endtime)) && ((firetime < newFiretime) || (firetime >= newEndtime)))
        {
            return (newFiretime < 0f);
        }
        return true;
    }

    public override void Draw(DirectorControlState state)
    {
        if (base.Wrapper.Behaviour != null)
        {
            string str = base.Behaviour.name;
            if (base.isRenaming)
            {
                GUI.Box(base.controlPosition, GUIContent.none, TimelineTrackControl.styles.TrackItemSelectedStyle);
                GUI.SetNextControlName("TrackItemControlRename");
                str = EditorGUI.TextField(base.controlPosition, GUIContent.none, str);
                if (base.renameRequested)
                {
                    EditorGUI.FocusTextInControl("TrackItemControlRename");
                    base.renameRequested = false;
                }
                if ((!base.IsSelected || (Event.current.keyCode == (KeyCode)13)) || (((Event.current.type == (EventType)11)) && !this.controlPosition.Contains(Event.current.mousePosition)))
                {
                    base.isRenaming = false;
                    GUIUtility.hotControl =(0);
                    GUIUtility.keyboardControl=(0);
                    int drawPriority = base.DrawPriority;
                    base.DrawPriority = drawPriority - 1;
                }
            }
            if (base.Behaviour.name != str)
            {
                Undo.RecordObject(base.Behaviour.gameObject, string.Format("Renamed {0}", base.Behaviour.name));
                base.Behaviour.name=(str);
            }
            if (!base.isRenaming)
            {
                if (base.IsSelected)
                {
                    GUI.Box(base.controlPosition, new GUIContent(str), TimelineTrackControl.styles.TrackItemSelectedStyle);
                }
                else
                {
                    GUI.Box(base.controlPosition, new GUIContent(str), TimelineTrackControl.styles.TrackItemStyle);
                }
            }
        }
    }
    public virtual void ResizeControlPosition(DirectorControlState state, Rect trackPosition)
    {
        CinemaActionWrapper wrapper = base.Wrapper as CinemaActionWrapper;
        if (wrapper == null)
        {
            return;
        }
        if (base.isRenaming)
        {
            return;
        }
        float num = (wrapper.Firetime * state.Scale.x) + state.Translation.x;
        float num2 = ((wrapper.Firetime + wrapper.Duration) * state.Scale.x) + state.Translation.x;
        this.controlPosition = new Rect(num, 0f, num2 - num, trackPosition.height);
    }
    public override void HandleInput(DirectorControlState state, Rect trackPosition)
    {
        CinemaActionWrapper wrapper = base.Wrapper as CinemaActionWrapper;
        if (wrapper == null)
        {
            return;
        }
        if (base.isRenaming)
        {
            return;
        }
        float num = (wrapper.Firetime * state.Scale.x) + state.Translation.x;
        float num2 = ((wrapper.Firetime + wrapper.Duration) * state.Scale.x) + state.Translation.x; 
        ResizeControlPosition(state, trackPosition);
        Rect rect = new Rect(num, this.controlPosition.y, 5f, this.controlPosition.height);
        Rect rect2 = new Rect(num + 5f, this.controlPosition.y, (num2 - num) - 10f, this.controlPosition.height);
        Rect rect3 = new Rect(num2 - 5f, this.controlPosition.y, 5f, this.controlPosition.height);
        EditorGUIUtility.AddCursorRect(rect, (MouseCursor)3);
        EditorGUIUtility.AddCursorRect(rect2, (MouseCursor)5);
        EditorGUIUtility.AddCursorRect(rect3, (MouseCursor)3);
        base.controlID = GUIUtility.GetControlID(base.Wrapper.Behaviour.GetInstanceID(), (FocusType)2, base.controlPosition);
        int num3 = GUIUtility.GetControlID(base.Wrapper.Behaviour.GetInstanceID(), (FocusType)2, rect);
        int num4 = GUIUtility.GetControlID(base.Wrapper.Behaviour.GetInstanceID(), (FocusType)2, rect2);
        int num5 = GUIUtility.GetControlID(base.Wrapper.Behaviour.GetInstanceID(), (FocusType)2, rect3);
        if (((Event.current.GetTypeForControl(base.controlID) == EventType.MouseDown) && rect2.Contains(Event.current.mousePosition)) && (Event.current.button == 1))
        {
            if (!base.IsSelected)
            {
                GameObject[] objArray = Selection.gameObjects;
                ArrayUtility.Add<GameObject>(ref objArray, base.Wrapper.Behaviour.gameObject);
                Selection.objects =objArray;
                base.hasSelectionChanged = true;
            }
            this.showContextMenu(base.Wrapper.Behaviour);
            Event.current.Use();
        }
        switch (Event.current.GetTypeForControl(num4))
        {
            case 0:
            {
                if (!rect2.Contains(Event.current.mousePosition) || (Event.current.button != 0))
                {
                    goto Label_0471;
                }
                GUIUtility.hotControl =(num4);
                if (!Event.current.control)
                {
                    if (!base.IsSelected)
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
                    GUIUtility.hotControl =(0);
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
                goto Label_0471;

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
                goto Label_0471;

            default:
                goto Label_0471;
        }
        base.mouseDragActivity = false;
        mouseDownOffset = ((Event.current.mousePosition.x - state.Translation.x) / state.Scale.x) - wrapper.Firetime;
        if (!base.TrackControl.TargetTrack.IsLocked)
        {
            Event.current.Use();
        }
    Label_0471:
        switch (Event.current.GetTypeForControl(num3))
        {
            case 0:
                if (rect.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl =(num3);
                    mouseDownOffset = ((Event.current.mousePosition.x - state.Translation.x) / state.Scale.x) - wrapper.Firetime;
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
                    GUIUtility.hotControl =(0);
                    if (this.AlterAction != null)
                    {
                        this.AlterAction(this, new ActionItemEventArgs(wrapper.Behaviour, wrapper.Firetime, wrapper.Duration));
                    }
                }
                break;

            case (EventType)3:
                if (GUIUtility.hotControl == num3)
                {
                    float time = (Event.current.mousePosition.x - state.Translation.x) / state.Scale.x;
                    time = state.SnappedTime(time);
                    float num8 = 0f;
                    float num9 = wrapper.Firetime + wrapper.Duration;
                    foreach (CinemaActionWrapper wrapper2 in base.Track.Items)
                    {
                        if ((wrapper2 != null) && (wrapper2.Behaviour != base.Wrapper.Behaviour))
                        {
                            float num10 = wrapper2.Firetime + wrapper2.Duration;
                            if (num10 <= base.Wrapper.Firetime)
                            {
                                num8 = Mathf.Max(num8, num10);
                            }
                        }
                    }
                    time = Mathf.Max(num8, time);
                    time = Mathf.Min(num9, time);
                    wrapper.Duration += base.Wrapper.Firetime - time;
                    wrapper.Firetime = time;
                }
                break;
        }
        switch (Event.current.GetTypeForControl(num5))
        {
            case 0:
                if (rect3.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl =(num5);
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
                    GUIUtility.hotControl =(0);
                    if (this.AlterAction != null)
                    {
                        this.AlterAction(this, new ActionItemEventArgs(wrapper.Behaviour, wrapper.Firetime, wrapper.Duration));
                    }
                }
                break;

            case (EventType)3:
                if (GUIUtility.hotControl == num5)
                {
                    float num11 = (Event.current.mousePosition.x - state.Translation.x) / state.Scale.x;
                    num11 = state.SnappedTime(num11);
                    float positiveInfinity = float.PositiveInfinity;
                    foreach (CinemaActionWrapper wrapper3 in base.Track.Items)
                    {
                        if ((wrapper3 != null) && (wrapper3.Behaviour != base.Wrapper.Behaviour))
                        {
                            float num13 = wrapper.Firetime + wrapper.Duration;
                            if (wrapper3.Firetime >= num13)
                            {
                                positiveInfinity = Mathf.Min(positiveInfinity, wrapper3.Firetime);
                            }
                        }
                    }
                    num11 = Mathf.Clamp(num11, base.Wrapper.Firetime, positiveInfinity);
                    wrapper.Duration = num11 - base.Wrapper.Firetime;
                }
                break;
        }
        if (base.Wrapper.Behaviour.gameObject!=null && Selection.activeGameObject!=null && Selection.activeGameObject == base.Wrapper.Behaviour.gameObject)
        {
            if ((Event.current.type == (EventType)13) && (Event.current.commandName == "Copy"))
            {
                Event.current.Use();
            }
            if ((Event.current.type == (EventType)14) && (Event.current.commandName == "Copy"))
            {
                DirectorCopyPaste.Copy(base.Wrapper.Behaviour);
                Event.current.Use();
            }
        }
        if (((Event.current.type == (EventType)4) && (Event.current.keyCode == (KeyCode)0x7f)) && (Selection.activeGameObject == base.Wrapper.Behaviour.gameObject))
        {
            base.deleteItem(base.Wrapper.Behaviour);
            Event.current.Use();
        }
    }

    internal override float RequestTranslate(float amount)
    {
        CinemaActionWrapper wrapper = base.Wrapper as CinemaActionWrapper;
        if (wrapper == null)
        {
            return 0f;
        }
        float num = base.Wrapper.Firetime + amount;
        float newFiretime = base.Wrapper.Firetime + amount;
        float num3 = newFiretime;
        bool flag = true;
        float num4 = 0f;
        float positiveInfinity = float.PositiveInfinity;
        float newEndtime = newFiretime + wrapper.Duration;
        foreach (CinemaActionWrapper wrapper2 in base.Track.Items)
        {
            if (((wrapper2 != null) && (wrapper2.Behaviour != wrapper.Behaviour)) && !Selection.Contains(wrapper2.Behaviour.gameObject))
            {
                float endtime = wrapper2.Firetime + wrapper2.Duration;
                float num8 = wrapper.Firetime + wrapper.Duration;
                if (this.doActionsConflict(wrapper2.Firetime, endtime, newFiretime, newEndtime))
                {
                    flag = false;
                }
                if (endtime <= wrapper.Firetime)
                {
                    num4 = Mathf.Max(num4, endtime);
                }
                if (wrapper2.Firetime >= num8)
                {
                    positiveInfinity = Mathf.Min(positiveInfinity, wrapper2.Firetime);
                }
            }
        }
        if (flag)
        {
            num3 = Mathf.Max(0f, newFiretime);
        }
        else
        {
            newFiretime = Mathf.Max(num4, newFiretime);
            num3 = Mathf.Min(positiveInfinity - wrapper.Duration, newFiretime);
        }
        return (amount + (num3 - num));
    }
}

