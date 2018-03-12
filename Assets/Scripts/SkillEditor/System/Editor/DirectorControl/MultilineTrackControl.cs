
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class MultilineTrackControl : TimelineTrackControl
{
    private int controlSelection;
    private CinemaCurveClipItemControl focusedControl;
    protected int MINIMUM_ROWS_SHOWING = 3;
    protected int rowsShowing = 5;

    protected MultilineTrackControl()
    {
    }

    public override void calculateHeight()
    {
        if (base.isExpanded)
        {
            this.trackArea.height = (17f * this.rowsShowing);
        }
        else
        {
            this.trackArea.height = (17f);
        }
    }

    private void curveClipControl_RequestEdit(object sender, CurveClipWrapperEventArgs e)
    {
        if (sender is CinemaCurveClipItemControl)
        {
            this.togglEditItemControl(sender as CinemaCurveClipItemControl);
        }
    }

    private void editItemControl(int controlSelection)
    {
        int num = 0;
        foreach (CinemaCurveClipItemControl control in base.Controls)
        {
            control.IsEditing = num++ == controlSelection;
            if (control.IsEditing)
            {
                this.focusedControl = control;
                this.MINIMUM_ROWS_SHOWING = 10;
                this.rowsShowing = 10;
            }
        }
    }

    protected override void initializeTrackItemControl(TrackItemControl control)
    {
        base.initializeTrackItemControl(control);
        if (control is CinemaCurveClipItemControl)
        {
            (control as CinemaCurveClipItemControl).RequestEdit += new CurveClipWrapperEventHandler(this.curveClipControl_RequestEdit);
        }
    }

    protected override void prepareTrackItemControlForRemoval(TrackItemControl control)
    {
        base.prepareTrackItemControlForRemoval(control);
        if (control is CinemaCurveClipItemControl)
        {
            (control as CinemaCurveClipItemControl).RequestEdit -= new CurveClipWrapperEventHandler(this.curveClipControl_RequestEdit);
        }
    }

    private void togglEditItemControl(CinemaCurveClipItemControl control)
    {
        IEnumerator<TrackItemControl> enumerator;
        if (!control.IsEditing)
        {
            using (enumerator = base.Controls.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ((CinemaCurveClipItemControl)enumerator.Current).IsEditing = false;
                }
            }
            control.IsEditing = true;
            this.focusedControl = control;
            this.MINIMUM_ROWS_SHOWING = 10;
            this.rowsShowing = 10;
        }
        else
        {
            using (enumerator = base.Controls.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ((CinemaCurveClipItemControl)enumerator.Current).IsEditing = false;
                }
            }
            this.focusedControl = null;
            this.editItemControl(-1);
            this.MINIMUM_ROWS_SHOWING = 3;
            this.rowsShowing = 5;
        }
    }

    private void updateEditPanel(Rect row1)
    {
        Rect rect = new Rect(row1.x, row1.y, row1.width / 2f, row1.height);
        Rect rect2 = new Rect(row1.x + (row1.width / 2f), row1.y, row1.width / 2f, row1.height);
        List<GUIContent> list = new List<GUIContent>();
        foreach (TrackItemControl control in base.Controls)
        {
            if ((control.Wrapper != null) && (control.Wrapper.Behaviour != null))
            {
                list.Add(new GUIContent(control.Wrapper.Behaviour.name));
            }
        }
        int num = 1;
        for (int i = 0; i < (list.Count - 1); i++)
        {
            for (int j = i + 1; (j < list.Count) && (string.Compare(list[i].text, list[j].text) == 0); j++)
            {
                list[j].text = (string.Format("{0} (duplicate {1})", list[j].text, num++));
            }
            num = 1;
        }
        if (list.Count > 0)
        {
            this.controlSelection = Mathf.Min(this.controlSelection, list.Count - 1);
            this.controlSelection = EditorGUI.Popup(rect, this.controlSelection, list.ToArray());
            if (GUI.Button(rect2, new GUIContent("Edit")))
            {
                this.editItemControl(this.controlSelection);
            }
        }
    }

    public override void UpdateHeaderContents(DirectorControlState state, Rect position, Rect headerBackground)
    {
        Rect rect = new Rect(position.x + 14f, position.y, 14f, position.height);
        float introduced10 = rect.x;
        Rect rect2 = new Rect(introduced10 + rect.width, position.y, ((position.width - 14f) - 96f) - 14f, 17f);
        string str = base.TargetTrack.Behaviour.name;
        bool flag = EditorGUI.Foldout(rect, base.isExpanded, GUIContent.none, false);
        if (flag != base.isExpanded)
        {
            base.isExpanded = flag;
            EditorPrefs.SetBool(base.IsExpandedKey, base.isExpanded);
        }
        this.updateHeaderControl1(new Rect(position.width - 64f, position.y, 16f, 16f));
        this.updateHeaderControl2(new Rect(position.width - 48f, position.y, 16f, 16f));
        this.updateHeaderControl3(new Rect(position.width - 32f, position.y, 16f, 16f));
        this.updateHeaderControl4(new Rect(position.width - 16f, position.y, 16f, 16f));
        this.updateHeaderControl6(new UnityEngine.Rect(position.width - 96, position.y, 16f, 16f));
        if (base.isExpanded)
        {
            Rect rect3 = new Rect(rect2.x + 28f, position.y + 17f, (headerBackground.width - rect2.x) - 28f, 17f);
            if (((this.focusedControl != null) && (this.focusedControl.Wrapper != null)) && (this.focusedControl.Wrapper.Behaviour != null))
            {
                float introduced11 = rect3.x;
                float introduced12 = rect3.y;
                new Rect(introduced11, introduced12, rect3.width / 2f, rect3.height);
                float introduced13 = rect3.x;
                float introduced14 = rect3.y;
                float introduced15 = rect3.x;
                float introduced16 = rect3.y;
                Rect controlHeaderArea = new Rect(introduced15, introduced16 + rect3.height, rect3.width, position.height - 34f);
                this.focusedControl.UpdateHeaderArea(state, controlHeaderArea);
                if (GUI.Button(new Rect(introduced13 + (rect3.width / 2f), introduced14, rect3.width / 2f, rect3.height), new GUIContent("Done")))
                {
                    this.focusedControl = null;
                    this.editItemControl(-1);
                    this.MINIMUM_ROWS_SHOWING = 3;
                    this.rowsShowing = 5;
                }
            }
            else
            {
                this.updateEditPanel(rect3);
            }
        }
        if (base.isRenaming)
        {
            GUI.SetNextControlName("CurveTrackRename");
            str = EditorGUI.TextField(rect2, GUIContent.none, str);
            if (base.renameRequested)
            {
                EditorGUI.FocusTextInControl("CurveTrackRename");
                base.renameRequested = false;
            }
            if ((Event.current.keyCode == (KeyCode)13) || ((Event.current.type == EventType.MouseUp) && !rect2.Contains(Event.current.mousePosition)))
            {
                base.isRenaming = false;
                GUIUtility.hotControl = (0);
                GUIUtility.keyboardControl = (0);
            }
        }
        if (base.TargetTrack.Behaviour.name != str)
        {
            Undo.RecordObject(base.TargetTrack.Behaviour.gameObject, string.Format("Renamed {0}", base.TargetTrack.Behaviour.name));
            base.TargetTrack.Behaviour.name = (str);
        }
        if (!base.isRenaming)
        {
            string str2 = str;
            for (Vector2 vector = GUI.skin.label.CalcSize(new GUIContent(str2)); (vector.x > rect2.width) && (str2.Length > 5); vector = GUI.skin.label.CalcSize(new GUIContent(str2)))
            {
                str2 = str2.Substring(0, str2.Length - 4) + "...";
            }
            if (Selection.Contains(base.TargetTrack.Behaviour.gameObject))
            {
                GUI.Label(rect2, str2, EditorStyles.whiteLabel);
            }
            else
            {
                GUI.Label(rect2, str2);
            }
            int num = GUIUtility.GetControlID(base.TargetTrack.Behaviour.GetInstanceID(), (FocusType)2, position);
            if (Event.current.GetTypeForControl(num) == EventType.MouseDown)
            {
                if (position.Contains(Event.current.mousePosition) && (Event.current.button == 1))
                {
                    if (!base.IsSelected)
                    {
                        base.RequestSelect();
                    }
                    base.showHeaderContextMenu();
                    Event.current.Use();
                }
                else if (position.Contains(Event.current.mousePosition) && (Event.current.button == 0))
                {
                    base.RequestSelect();
                    Event.current.Use();
                }
            }
        }
    }

    protected override void updateHeaderControl1(Rect position)
    {
        if (GUI.Button(position, string.Empty, TimelineTrackControl.styles.compressStyle) && (this.rowsShowing > this.MINIMUM_ROWS_SHOWING))
        {
            this.rowsShowing--;
        }
    }

    protected override void updateHeaderControl2(Rect position)
    {
        if (GUI.Button(position, string.Empty, TimelineTrackControl.styles.expandStyle))
        {
            this.rowsShowing++;
        }
    }
}

