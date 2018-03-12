using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ActionFixedItemControl : ActionItemControl
{
    private static float mouseDownOffset = -1f;

    [field: CompilerGenerated]
    public event ActionFixedItemEventHandler AlterFixedAction;

    internal override void ConfirmTranslate()
    {
        CinemaActionFixedWrapper wrapper = base.Wrapper as CinemaActionFixedWrapper;
        if ((wrapper != null) && (this.AlterFixedAction != null))
        {
            this.AlterFixedAction(this, new ActionFixedItemEventArgs(wrapper.Behaviour, wrapper.Firetime, wrapper.Duration, wrapper.InTime, wrapper.OutTime));
        }
    }

    public override void HandleInput(DirectorControlState state, Rect trackPosition)
    {
        CinemaActionFixedWrapper wrapper = base.Wrapper as CinemaActionFixedWrapper;
        if (wrapper == null)
        {
            return;
        }
        if (base.isRenaming)
        {
            return;
        }
        float num = state.TimeToPosition(wrapper.Firetime);
        float num2 = state.TimeToPosition(wrapper.Firetime + wrapper.Duration);
        base.controlPosition = new Rect(num, 0f, num2 - num, trackPosition.height);
        bool flag1 = this.controlPosition.width < 15f;
        float num3 = flag1 ? 0f : 5f;
        Rect rect = new Rect(num, 0f, num3, trackPosition.height);
        Rect rect2 = new Rect(num + num3, 0f, (num2 - num) - (2f * num3), trackPosition.height);
        Rect rect3 = new Rect(num2 - num3, 0f, num3, trackPosition.height);
        EditorGUIUtility.AddCursorRect(rect2,  MouseCursor.SlideArrow);
        if (!flag1)
        {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeHorizontal);
            EditorGUIUtility.AddCursorRect(rect3, MouseCursor.ResizeHorizontal);
        }
        base.controlID = GUIUtility.GetControlID(wrapper.Behaviour.GetInstanceID(),UnityEngine.FocusType.Passive, base.controlPosition);
        int num4 = GUIUtility.GetControlID(wrapper.Behaviour.GetInstanceID(), FocusType.Passive, rect);
        int num5 = GUIUtility.GetControlID(wrapper.Behaviour.GetInstanceID(), FocusType.Passive, rect2);
        int num6 = GUIUtility.GetControlID(wrapper.Behaviour.GetInstanceID(), FocusType.Passive, rect3);
        if (((Event.current.GetTypeForControl(base.controlID) == EventType.MouseDown) && rect2.Contains(Event.current.mousePosition)) && (Event.current.button == 1))
        {
            if (!base.IsSelected)
            {
                GameObject[] objArray = Selection.gameObjects;
                ArrayUtility.Add<GameObject>(ref objArray, base.Wrapper.Behaviour.gameObject);
                Selection.objects =objArray;
                base.hasSelectionChanged = true;
            }
            this.showContextMenu(wrapper.Behaviour);
            if (!base.TrackControl.TargetTrack.IsLocked)
            {
                Event.current.Use();
            }
        }
        switch (Event.current.GetTypeForControl(num5))
        {
            case 0:
            {
                if (!rect2.Contains(Event.current.mousePosition) || (Event.current.button != 0))
                {
                    goto Label_0447;
                }
                GUIUtility.hotControl=num5;
                if (!Event.current.control)
                {
                    if (!base.IsSelected)
                    {
                        Selection.activeInstanceID=base.Behaviour.GetInstanceID();
                    }
                    break;
                }
                if (!base.IsSelected)
                {
                    GameObject[] objArray3 = Selection.gameObjects;
                    ArrayUtility.Add<GameObject>(ref objArray3, base.Wrapper.Behaviour.gameObject);
                    Selection.objects = objArray3;
                    base.hasSelectionChanged = true;
                    break;
                }
                GameObject[] objArray2 = Selection.gameObjects;
                ArrayUtility.Remove<GameObject>(ref objArray2, base.Wrapper.Behaviour.gameObject);
                Selection.objects = (objArray2);
                base.hasSelectionChanged = true;
                break;
            }
            case EventType.MouseUp:
                if (GUIUtility.hotControl == num5)
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
                goto Label_0447;

            case (EventType)3:
                if (GUIUtility.hotControl == num5)
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
                goto Label_0447;

            default:
                goto Label_0447;
        }
        base.mouseDragActivity = false;
        mouseDownOffset = ((Event.current.mousePosition.x - state.Translation.x) / state.Scale.x) - wrapper.Firetime;
        if (!base.TrackControl.TargetTrack.IsLocked)
        {
            Event.current.Use();
        }
    Label_0447:
        switch (Event.current.GetTypeForControl(num4))
        {
            case 0:
                if (rect.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl=(num4);
                    mouseDownOffset = ((Event.current.mousePosition.x - state.Translation.x) / state.Scale.x) - wrapper.Firetime;
                    if (!base.TrackControl.TargetTrack.IsLocked)
                    {
                        Event.current.Use();
                    }
                }
                break;

            case (EventType)1:
                if (GUIUtility.hotControl == num4)
                {
                    mouseDownOffset = -1f;
                    GUIUtility.hotControl=(0);
                }
                break;

            case (EventType)3:
                if (GUIUtility.hotControl == num4)
                {
                    float time = (Event.current.mousePosition.x - state.Translation.x) / state.Scale.x;
                    time = state.SnappedTime(time);
                    if (time >= 0f)
                    {
                        float num9 = wrapper.InTime - (wrapper.Firetime - time);
                        num9 = Mathf.Clamp(num9, 0f, wrapper.ItemLength);
                        float num10 = num9 - wrapper.InTime;
                        wrapper.InTime = num9;
                        wrapper.Firetime += num10;
                        if (this.AlterFixedAction != null)
                        {
                            this.AlterFixedAction(this, new ActionFixedItemEventArgs(wrapper.Behaviour, wrapper.Firetime, wrapper.Duration, wrapper.InTime, wrapper.OutTime));
                        }
                    }
                }
                break;
        }
        switch (Event.current.GetTypeForControl(num6))
        {
            case 0:
                if (rect3.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl=(num6);
                    mouseDownOffset = ((Event.current.mousePosition.x - state.Translation.x) / state.Scale.x) - wrapper.Firetime;
                    if (!base.TrackControl.TargetTrack.IsLocked)
                    {
                        Event.current.Use();
                    }
                }
                break;

            case (EventType)1:
                if (GUIUtility.hotControl == num6)
                {
                    mouseDownOffset = -1f;
                    GUIUtility.hotControl=(0);
                }
                break;

            case (EventType)3:
                if (GUIUtility.hotControl == num6)
                {
                    float num11 = (Event.current.mousePosition.x - state.Translation.x) / state.Scale.x;
                    float num12 = state.SnappedTime(num11) - (wrapper.Firetime - wrapper.InTime);
                    Undo.RecordObject(wrapper.Behaviour, string.Format("Changed {0}", wrapper.Behaviour.name));
                    wrapper.OutTime = Mathf.Clamp(num12, 0f, wrapper.ItemLength);
                    if (this.AlterFixedAction != null)
                    {
                        this.AlterFixedAction(this, new ActionFixedItemEventArgs(wrapper.Behaviour, wrapper.Firetime, wrapper.Duration, wrapper.InTime, wrapper.OutTime));
                    }
                }
                break;
        }
        if (Selection.activeGameObject == base.Wrapper.Behaviour.gameObject)
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
}

