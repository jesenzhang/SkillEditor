using DirectorEditor;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class TrackItemControl : DirectorBehaviourControl
{
    protected int controlID;
    protected Rect controlPosition;
    private int drawPriority;
    protected bool hasSelectionChanged;
    protected bool isRenaming;
    protected bool mouseDragActivity;
    private int renameControlID;
    protected bool renameRequested;
    private TimelineTrackWrapper track;
    private TimelineTrackControl trackControl;
    private TimelineItemWrapper wrapper;

    [field: CompilerGenerated]
    public event TrackItemEventHandler AlterTrackItem;

    [field: CompilerGenerated]
    internal event TranslateTrackItemEventHandler RequestTrackItemTranslate;

    [field: CompilerGenerated]
    internal event TranslateTrackItemEventHandler TrackItemTranslate;

    [field: CompilerGenerated]
    internal event TrackItemEventHandler TrackItemUpdate;

    internal void BoxSelect(Rect selectionBox)
    {
        Rect rect = new Rect(this.controlPosition);
        rect.x=(rect.x + this.trackControl.Rect.x);
        rect.y=(rect.y + this.trackControl.Rect.y);
        if (rect.Overlaps(selectionBox, true))
        {
            GameObject[] objArray = Selection.gameObjects;
            ArrayUtility.Add<GameObject>(ref objArray, this.wrapper.Behaviour.gameObject);
            Selection.objects=(objArray);
        }
        else if (Selection.Contains(this.wrapper.Behaviour.gameObject))
        {
            GameObject[] objArray2 = Selection.gameObjects;
            ArrayUtility.Remove<GameObject>(ref objArray2, this.wrapper.Behaviour.gameObject);
            Selection.objects=(objArray2);
        }
    }

    internal virtual void ConfirmTranslate()
    {
        if (this.AlterTrackItem != null)
        {
            this.AlterTrackItem(this, new TrackItemEventArgs(this.wrapper.Behaviour, this.wrapper.Firetime));
        }
    }

    protected void copyItem(object userData)
    {
        Behaviour behaviour = userData as Behaviour;
        if (behaviour != null)
        {
            DirectorCopyPaste.Copy(behaviour);
        }
    }

    internal override void Delete()
    {
        Undo.DestroyObjectImmediate(this.Wrapper.Behaviour.gameObject);
    }

    protected void deleteItem(object userData)
    {
        Behaviour behaviour = userData as Behaviour;
        if (behaviour != null)
        {
            base.Behaviour = behaviour;
            base.RequestDelete();
        }
    }

    public virtual void Draw(DirectorControlState state)
    {
        Behaviour behaviour = this.wrapper.Behaviour;
        if (behaviour != null)
        {
            Color ocolor = Color.white;
            if (base.IsSelected)
            {
                GUI.color = (new Color(0.5f, 0.6f, 0.905f, 1f));
            }
            else
            {
                ocolor = GUI.color;
            }
            Rect controlPosition = this.controlPosition;
            controlPosition.height=(17f);
            GUI.Box(controlPosition, GUIContent.none, TimelineTrackControl.styles.EventItemStyle);
            if (this.trackControl.isExpanded)
            {
                float introduced4 = this.controlPosition.width;
                GUI.Box(new Rect(this.controlPosition.x, controlPosition.yMax, introduced4, this.controlPosition.height - controlPosition.height), GUIContent.none, TimelineTrackControl.styles.EventItemBottomStyle);
            }
            GUI.color= ocolor;
            Rect labelPosition = new Rect(this.controlPosition.x + 16f, this.controlPosition.y, 128f, this.controlPosition.height);
            string name = behaviour.name;
            this.DrawRenameLabel(name, labelPosition, null);
        }
    }

    protected virtual void DrawRenameLabel(string name, Rect labelPosition, GUIStyle labelStyle = null)
    {
        if (this.isRenaming)
        {
            GUI.SetNextControlName("TrackItemControlRename");
            name = EditorGUI.TextField(labelPosition, GUIContent.none, name);
            if (this.renameRequested)
            {
                EditorGUI.FocusTextInControl("TrackItemControlRename");
                this.renameRequested = false;
                this.renameControlID = GUIUtility.keyboardControl;
            }
            if ((!EditorGUIUtility.editingTextField || (this.renameControlID != GUIUtility.keyboardControl)) || ((Event.current.keyCode == (KeyCode)13) || ((Event.current.type == EventType.MouseUp) && !labelPosition.Contains(Event.current.mousePosition))))
            {
                this.isRenaming = false;
                GUIUtility.hotControl=(0);
                GUIUtility.keyboardControl=(0);
                EditorGUIUtility.editingTextField=(false);
                int drawPriority = this.DrawPriority;
                this.DrawPriority = drawPriority - 1;
            }
        }
        if (base.Behaviour.name != name)
        {
            Undo.RecordObject(base.Behaviour.gameObject, string.Format("Renamed {0}", base.Behaviour.name));
            base.Behaviour.name=(name);
        }
        if (!this.isRenaming)
        {
            if (base.IsSelected)
            {
                GUI.Label(labelPosition, base.Behaviour.name, EditorStyles.whiteLabel);
            }
            else
            {
                GUI.Label(labelPosition, base.Behaviour.name);
            }
        }
    }

    public virtual void HandleInput(DirectorControlState state, Rect trackPosition)
    {
        Behaviour behaviour = this.wrapper.Behaviour;
        if (behaviour == null)
        {
            return;
        }
        float num = (this.wrapper.Firetime * state.Scale.x) + state.Translation.x;
        this.controlPosition = new Rect(num - 8f, 0f, 16f, trackPosition.height);
        this.controlID = GUIUtility.GetControlID(this.wrapper.Behaviour.GetInstanceID(), (FocusType)2, this.controlPosition);
        switch (Event.current.GetTypeForControl(this.controlID))
        {
            case 0:
            {
                if (!this.controlPosition.Contains(Event.current.mousePosition) || (Event.current.button != 0))
                {
                    goto Label_0183;
                }
                GUIUtility.hotControl=(this.controlID);
                if (!Event.current.control)
                {
                    if (!base.IsSelected)
                    {
                        Selection.activeInstanceID=(behaviour.GetInstanceID());
                    }
                    break;
                }
                if (!base.IsSelected)
                {
                    GameObject[] objArray2 = Selection.gameObjects;
                    ArrayUtility.Add<GameObject>(ref objArray2, this.Wrapper.Behaviour.gameObject);
                    Selection.objects=(objArray2);
                    this.hasSelectionChanged = true;
                    break;
                }
                GameObject[] objArray = Selection.gameObjects;
                ArrayUtility.Remove<GameObject>(ref objArray, this.Wrapper.Behaviour.gameObject);
                Selection.objects=(objArray);
                this.hasSelectionChanged = true;
                break;
            }
            case (EventType)1:
                if (GUIUtility.hotControl == this.controlID)
                {
                    GUIUtility.hotControl=(0);
                    if (this.mouseDragActivity)
                    {
                        if (this.TrackItemUpdate != null)
                        {
                            this.TrackItemUpdate(this, new TrackItemEventArgs(this.wrapper.Behaviour, this.wrapper.Firetime));
                        }
                    }
                    else if (!Event.current.control)
                    {
                        Selection.activeInstanceID=(behaviour.GetInstanceID());
                    }
                    else if (!this.hasSelectionChanged)
                    {
                        if (!base.IsSelected)
                        {
                            GameObject[] objArray5 = Selection.gameObjects;
                            ArrayUtility.Add<GameObject>(ref objArray5, this.Wrapper.Behaviour.gameObject);
                            Selection.objects=(objArray5);
                        }
                        else
                        {
                            GameObject[] objArray4 = Selection.gameObjects;
                            ArrayUtility.Remove<GameObject>(ref objArray4, this.Wrapper.Behaviour.gameObject);
                            Selection.objects=(objArray4);
                        }
                    }
                    this.hasSelectionChanged = false;
                }
                goto Label_03AD;

            case (EventType)3:
                if ((GUIUtility.hotControl == this.controlID) && !this.hasSelectionChanged)
                {
                    Undo.RecordObject(behaviour, string.Format("Changed {0}", behaviour.name));
                    float time = (Event.current.mousePosition.x - state.Translation.x) / state.Scale.x;
                    time = state.SnappedTime(time);
                    if (!this.mouseDragActivity)
                    {
                        this.mouseDragActivity = this.Wrapper.Firetime != time;
                    }
                    if (this.RequestTrackItemTranslate != null)
                    {
                        float firetime = time - this.wrapper.Firetime;
                        float num4 = this.RequestTrackItemTranslate(this, new TrackItemEventArgs(this.wrapper.Behaviour, firetime));
                        if (this.TrackItemTranslate != null)
                        {
                            this.TrackItemTranslate(this, new TrackItemEventArgs(this.wrapper.Behaviour, num4));
                        }
                    }
                }
                goto Label_03AD;

            default:
                goto Label_03AD;
        }
        this.mouseDragActivity = false;
        if (!this.TrackControl.TargetTrack.IsLocked)
        {
            Event.current.Use();
        }
    Label_0183:
        if (this.controlPosition.Contains(Event.current.mousePosition) && (Event.current.button == 1))
        {
            if (!base.IsSelected)
            {
                GameObject[] objArray3 = Selection.gameObjects;
                ArrayUtility.Add<GameObject>(ref objArray3, this.Wrapper.Behaviour.gameObject);
                Selection.objects=(objArray3);
                this.hasSelectionChanged = true;
            }
            this.showContextMenu(behaviour);
            Event.current.Use();
        }
    Label_03AD:
        if (Selection.activeGameObject == behaviour.gameObject)
        {
            if ((Event.current.type == (EventType)13) && (Event.current.commandName == "Copy"))
            {
                Event.current.Use();
            }
            if ((Event.current.type == (EventType)14) && (Event.current.commandName == "Copy"))
            {
                DirectorCopyPaste.Copy(behaviour);
                Event.current.Use();
            }
        }
        if (((Event.current.type == (EventType)4) && (Event.current.keyCode ==(KeyCode)0x7f)) && (Selection.activeGameObject == behaviour.gameObject))
        {
            this.deleteItem(behaviour);
            Event.current.Use();
        }
    }

    public virtual void Initialize(TimelineItemWrapper wrapper, TimelineTrackWrapper track)
    {
        this.wrapper = wrapper;
        this.track = track;
    }

    public virtual void PostUpdate(DirectorControlState state)
    {
    }

    public virtual void PreUpdate(DirectorControlState state, Rect trackPosition)
    {
    }

    protected void renameItem(object userData)
    {
        if (userData is Behaviour)
        {
            this.renameRequested = true;
            this.isRenaming = true;
            int drawPriority = this.DrawPriority;
            this.DrawPriority = drawPriority + 1;
        }
    }

    internal virtual float RequestTranslate(float amount)
    {
        float num = this.Wrapper.Firetime + amount;
        float num2 = Mathf.Max(0f, num);
        return (amount + (num2 - num));
    }

    protected virtual void showContextMenu(Behaviour behaviour)
    {
        GenericMenu menu1 = new GenericMenu();
        menu1.AddItem(new GUIContent("Rename"), false, new GenericMenu.MenuFunction2(this.renameItem), behaviour);
        menu1.AddItem(new GUIContent("Copy"), false, new GenericMenu.MenuFunction2( this.copyItem), behaviour);
        menu1.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction2( this.deleteItem), behaviour);
        menu1.ShowAsContext();
    }

    internal virtual void Translate(float amount)
    {
        TimelineItemWrapper wrapper = this.Wrapper;
        wrapper.Firetime += amount;
    }

    protected void TriggerRequestTrackItemTranslate(float firetime)
    {
        if (this.RequestTrackItemTranslate != null)
        {
            float num = firetime - this.wrapper.Firetime;
            float num2 = this.RequestTrackItemTranslate(this, new TrackItemEventArgs(this.wrapper.Behaviour, num));
            if (this.TrackItemTranslate != null)
            {
                this.TrackItemTranslate(this, new TrackItemEventArgs(this.wrapper.Behaviour, num2));
            }
        }
    }

    protected void TriggerTrackItemUpdateEvent()
    {
        if (this.TrackItemUpdate != null)
        {
            this.TrackItemUpdate(this, new TrackItemEventArgs(this.wrapper.Behaviour, this.wrapper.Firetime));
        }
    }

    public int DrawPriority
    {
        get
        {
            return this.drawPriority;
        }
        set
        {
            this.drawPriority = value;
        }
    }

    public TimelineTrackWrapper Track
    {
        get
        {
            return this.track;
        }
        set
        {
            this.track = value;
        }
    }

    public TimelineTrackControl TrackControl
    {
        get
        {
            return this.trackControl;
        }
        set
        {
            this.trackControl = value;
        }
    }

    public TimelineItemWrapper Wrapper
    {
        get
        {
            return this.wrapper;
        }
        set
        {
            this.wrapper = value;
            base.Behaviour = value.Behaviour;
        }
    }
}

