using DirectorEditor;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TimeLine;
using UnityEditor;
using UnityEngine;

public class TrackGroupControl : SidebarControl
{
    protected const int DEFAULT_ROW_HEIGHT = 0x11;
    private DirectorControl directorControl;
    private bool isRenaming;
    protected Texture LabelPrefix;
    private Rect position;
    private int renameControlID;
    private bool renameRequested;
    private bool showContext;
    private float sortingOptionsWidth = 32f;
    protected DirectorControlState state;
    public static TrackGroupStyles styles;
    private Dictionary<TimelineTrackWrapper, TimelineTrackControl> timelineTrackMap = new Dictionary<TimelineTrackWrapper, TimelineTrackControl>();
    protected const int TRACK_GROUP_ICON_WIDTH = 0x10;
    protected TrackGroupWrapper trackGroup;

    private TrackGroupControl parentControl;


    public bool ParentExpended()
    {
        if (ParentControl != null)
        {
            return ParentControl.isExpanded && ParentControl.ParentExpended();
        }
        else
        {
            return true;
        }
    }
    protected virtual void addTrackContext()
    {
    }

    internal void BindTrackControls(TrackGroupWrapper trackGroup, List<SidebarControl> newSidebarControls, List<SidebarControl> removedSidebarControls, List<TrackItemControl> newTimelineControls, List<TrackItemControl> removedTimelineControls)
    {
        if (trackGroup.HasChanged)
        {
            bool flag = false;
            foreach (TimelineTrackWrapper wrapper in trackGroup.Tracks)
            {
                TimelineTrackControl control = null;
                if (!this.timelineTrackMap.TryGetValue(wrapper, out control))
                {
                    flag = true;
                    Type type = typeof(TimelineTrackControl);
                    int num = 0x7fffffff;
                    foreach (Type type2 in DirectorControlHelper.GetAllSubTypes(typeof(TimelineTrackControl)))
                    {
                        Type c = null;
                        foreach (CutsceneTrackAttribute attribute in type2.GetCustomAttributes(typeof(CutsceneTrackAttribute), true))
                        {
                            if (attribute != null)
                            {
                                c = attribute.TrackType;
                            }
                        }
                        if (c == wrapper.Behaviour.GetType())
                        {
                            type = type2;
                            num = 0;
                            break;
                        }
                        if ((c != null) && wrapper.Behaviour.GetType().IsSubclassOf(c))
                        {
                            Type baseType = wrapper.Behaviour.GetType();
                            int num4 = 0;
                            while ((baseType != null) && (baseType != c))
                            {
                                baseType = baseType.BaseType;
                                num4++;
                            }
                            if (num4 <= num)
                            {
                                num = num4;
                                type = type2;
                            }
                        }
                    }
                    control = (TimelineTrackControl) Activator.CreateInstance(type);
                    control.Initialize();
                    control.TrackGroupControl = this;
                    control.TargetTrack = wrapper;
                    control.SetExpandedFromEditorPrefs();
                    newSidebarControls.Add(control);
                    this.timelineTrackMap.Add(wrapper, control);
                }
            }
            List<TimelineTrackWrapper> list = new List<TimelineTrackWrapper>();
            foreach (TimelineTrackWrapper wrapper2 in this.timelineTrackMap.Keys)
            {
                bool flag2 = false;
                foreach (TimelineTrackWrapper wrapper3 in trackGroup.Tracks)
                {
                    if (wrapper2.Equals(wrapper3))
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    removedSidebarControls.Add(this.timelineTrackMap[wrapper2]);
                    list.Add(wrapper2);
                }
            }
            foreach (TimelineTrackWrapper wrapper4 in list)
            {
                flag = true;
                this.timelineTrackMap.Remove(wrapper4);
            }
            if (flag)
            {
                SortedDictionary<int, TimelineTrackWrapper> dictionary = new SortedDictionary<int, TimelineTrackWrapper>();
                List<TimelineTrackWrapper> list2 = new List<TimelineTrackWrapper>();
                foreach (TimelineTrackWrapper wrapper5 in this.timelineTrackMap.Keys)
                {
                    if ((wrapper5.Ordinal >= 0) && !dictionary.ContainsKey(wrapper5.Ordinal))
                    {
                        dictionary.Add(wrapper5.Ordinal, wrapper5);
                    }
                    else
                    {
                        list2.Add(wrapper5);
                    }
                }
                int num5 = 0;
                using (SortedDictionary<int, TimelineTrackWrapper>.ValueCollection.Enumerator enumerator4 = dictionary.Values.GetEnumerator())
                {
                    while (enumerator4.MoveNext())
                    {
                        enumerator4.Current.Ordinal = num5;
                        num5++;
                    }
                }
                using (List<TimelineTrackWrapper>.Enumerator enumerator3 = list2.GetEnumerator())
                {
                    while (enumerator3.MoveNext())
                    {
                        enumerator3.Current.Ordinal = num5;
                        num5++;
                    }
                }
            }
        }
        foreach (TimelineTrackWrapper wrapper6 in this.timelineTrackMap.Keys)
        {
            this.timelineTrackMap[wrapper6].BindTimelineItemControls(wrapper6, newTimelineControls, removedTimelineControls);
        }
        trackGroup.HasChanged = false;
    }

    internal void BoxSelect(Rect selectionBox)
    {
        if (base.isExpanded)
        {
            foreach (TimelineTrackWrapper wrapper in this.timelineTrackMap.Keys)
            {
                this.timelineTrackMap[wrapper].BoxSelect(selectionBox);
            }
        }
    }

    private void delete()
    {
        base.RequestDelete();
    }

    internal override void Delete()
    {
        Undo.DestroyObjectImmediate(this.trackGroup.Behaviour.gameObject);
    }

    internal void DeleteSelectedChildren()
    {
        foreach (TimelineTrackWrapper wrapper in this.timelineTrackMap.Keys)
        {
            TimelineTrackControl control = this.timelineTrackMap[wrapper];
            control.DeleteSelectedChildren();
            if (control.IsSelected)
            {
                control.Delete();
            }
        }
    }

    private void duplicate()
    {
        base.RequestDuplicate();
    }

    internal void Duplicate()
    {
        GameObject obj2 = GameObject.Instantiate<GameObject>(this.trackGroup.Behaviour.gameObject);
        string input = this.trackGroup.Behaviour.gameObject.name;
        string s = Regex.Match(input, @"(\d+)$").Value;
        int result = 1;
        if (int.TryParse(s, out result))
        {
            result++;
            obj2.name=(input.Substring(0, input.Length - s.Length) + result.ToString());
        }
        else
        {
            obj2.name=(input.Substring(0, input.Length - s.Length) + " " + 1.ToString());
        }
        obj2.transform.parent=(this.trackGroup.Behaviour.transform.parent);
        Undo.RegisterCreatedObjectUndo(obj2, "Duplicate " + obj2.name);
    }

    internal void DuplicateSelectedChildren()
    {
        foreach (TimelineTrackWrapper wrapper in this.timelineTrackMap.Keys)
        {
            TimelineTrackControl control = this.timelineTrackMap[wrapper];
            if (control.IsSelected)
            {
                control.Duplicate();
            }
        }
    }

    internal float GetHeight()
    {
        float num = 17f;
        if (base.isExpanded)
        {
            foreach (TimelineTrackControl control in this.timelineTrackMap.Values)
            {
                num += control.Rect.height;
            }
        }
        return (num + 1f);
    }

    internal List<SidebarControl> GetSidebarControlChildren(bool onlyVisible)
    {
        List<SidebarControl> list = new List<SidebarControl>();
        if (base.isExpanded)
        {
            foreach (TimelineTrackWrapper wrapper in this.timelineTrackMap.Keys)
            {
                TimelineTrackControl item = this.timelineTrackMap[wrapper];
                list.Add(item);
            }
        }
        return list;
    }

    public virtual void Initialize()
    {
        this.LabelPrefix = styles.DirectorGroupIcon.normal.background;
    }

    internal static void InitStyles(GUISkin skin)
    {
        if (styles == null)
        {
            styles = new TrackGroupStyles(skin);
        }
    }

    private void rename()
    {
        this.renameRequested = true;
        this.isRenaming = true;
    }

    protected virtual void showHeaderContextMenu()
    {
        GenericMenu menu1 = new GenericMenu();
        menu1.AddItem(new GUIContent("Rename"), false, new GenericMenu.MenuFunction(this.rename));
        menu1.AddItem(new GUIContent("Duplicate"), false, new GenericMenu.MenuFunction(this.duplicate));
        menu1.AddItem(new GUIContent("Delete"), false, new GenericMenu.MenuFunction( this.delete));
        menu1.ShowAsContext();
    }

    public virtual void Update(TrackGroupWrapper trackGroup, DirectorControlState state, Rect position, Rect fullHeader, Rect safeHeader, Rect content)
    {
        this.position = position;
        this.trackGroup = trackGroup;
        this.state = state;
        if (trackGroup.Behaviour != null)
        {
            bool parentExp = ParentExpended();
         
            if (parentExp)
            {
                this.updateHeaderBackground(fullHeader);
                this.updateContentBackground(content);
                if (base.isExpanded)
                {
                    Rect header = new Rect(safeHeader.x, safeHeader.y, fullHeader.width, safeHeader.height);
                    this.UpdateTracks(state, header, content);
                }
                this.updateHeaderContent(safeHeader);
            }
        }
    }

    protected virtual void updateContentBackground(Rect content)
    {
        if (Selection.Contains(this.trackGroup.Behaviour.gameObject) && !this.isRenaming)
        {
            GUI.Box(this.position, string.Empty, styles.BackgroundContentSelected);
        }
        else
        {
            GUI.Box(this.position, string.Empty, styles.TrackGroupArea);
        }
    }

    protected virtual void updateHeaderBackground(Rect position)
    {
        if (Selection.Contains(this.trackGroup.Behaviour.gameObject) && !this.isRenaming)
        {
            GUI.Box(position, string.Empty, styles.BackgroundSelected);
        }
    }

    protected virtual void updateHeaderContent(Rect header)
    {
        Rect rect = new Rect(header.x + 14f, header.y, 16f, 17f);
        float introduced8 = rect.x;
        float introduced9 = rect.x;
        Rect rect2 = new Rect(introduced8 + rect.width, header.y, (header.width - (introduced9 + rect.width)) - 32f, 17f);
        string str = this.trackGroup.Behaviour.name;
        bool flag = EditorGUI.Foldout(new Rect(header.x, header.y, 14f, 17f), base.isExpanded, GUIContent.none, false);
        if (flag != base.isExpanded)
        {
            base.isExpanded = flag;
            EditorPrefs.SetBool(base.IsExpandedKey, base.isExpanded);
        }
        GUI.Box(rect, this.LabelPrefix, GUIStyle.none);
        this.updateHeaderControl1(new Rect(header.x+header.width - 96f, header.y, 16f, 16f));
        this.updateHeaderControl2(new Rect(header.x + header.width - 80f, header.y, 16f, 16f));
        this.updateHeaderControl3(new Rect(header.x + header.width - 64f, header.y, 16f, 16f));
        this.updateHeaderControl4(new Rect(header.x + header.width - 48f, header.y, 16f, 16f));
        this.updateHeaderControl5(new Rect(header.x + header.width - 32f, header.y, 16f, 16f));
        this.updateHeaderControl6(new Rect(header.x + header.width - 16f, header.y, 16f, 16f));
        if (this.isRenaming)
        {
            GUI.SetNextControlName("TrackGroupRename");
            str = EditorGUI.TextField(rect2, GUIContent.none, str);
            if (this.renameRequested)
            {
                EditorGUI.FocusTextInControl("TrackGroupRename");
                this.renameRequested = false;
                this.renameControlID = GUIUtility.keyboardControl;
            }
            if ((!EditorGUIUtility.editingTextField || (this.renameControlID != GUIUtility.keyboardControl)) || ((Event.current.keyCode == (KeyCode)13) || ((Event.current.type == EventType.MouseUp) && !rect2.Contains(Event.current.mousePosition))))
            {
                this.isRenaming = false;
                GUIUtility.hotControl=(0);
                GUIUtility.keyboardControl=(0);
                EditorGUIUtility.editingTextField=(false);
            }
        }
        if (this.trackGroup.Behaviour.name != str)
        {
            Undo.RecordObject(this.trackGroup.Behaviour.gameObject, string.Format("Renamed {0}", this.trackGroup.Behaviour.name));
            this.trackGroup.Behaviour.name=(str);
        }
        if (!this.isRenaming)
        {
            string str2 = str;
            for (Vector2 vector = GUI.skin.label.CalcSize(new GUIContent(str2)); (vector.x > rect2.width) && (str2.Length > 5); vector = GUI.skin.label.CalcSize(new GUIContent(str2)))
            {
                str2 = str2.Substring(0, str2.Length - 4) + "...";
            }
            int num = GUIUtility.GetControlID(this.trackGroup.Behaviour.GetInstanceID(), (FocusType)2, header);
            if (Event.current.GetTypeForControl(num) == EventType.MouseDown)
            {
                if (header.Contains(Event.current.mousePosition) && (Event.current.button == 1))
                {
                    if (!base.IsSelected)
                    {
                        base.RequestSelect();
                    }
                    this.showHeaderContextMenu();
                    Event.current.Use();
                }
                else if (header.Contains(Event.current.mousePosition) && (Event.current.button == 0))
                {
                    base.RequestSelect();
                    Event.current.Use();
                }
            }
            if (base.IsSelected)
            {
                GUI.Label(rect2, str2, EditorStyles.whiteLabel);
            }
            else
            {
                GUI.Label(rect2, str2);
            }
        }
    }

    protected virtual void updateHeaderControl1(Rect position)
    {
    }

    protected virtual void updateHeaderControl2(Rect position)
    {
    }

    protected virtual void updateHeaderControl3(Rect position)
    {
    }

    protected virtual void updateHeaderControl4(Rect position)
    {
    }

    protected virtual void updateHeaderControl5(Rect position)
    {
        Color color = GUI.color;
        int num = 0;
        using (IEnumerator<TimelineTrackWrapper> enumerator = this.trackGroup.Tracks.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                TimelineTrackWrapper current = enumerator.Current;
                num++;
            }
        }
        GUI.color=((num > 0) ? new Color(0f, 53f, 0f) : new Color(53f, 0f, 0f));
        if (GUI.Button(position, string.Empty, styles.AddIcon))
        {
            this.addTrackContext();
        }
        GUI.color=(color);
    }

    protected virtual void updateHeaderControl6(Rect position)
    {
    }

    protected virtual void UpdateTracks(DirectorControlState state, Rect header, Rect content)
    {
        SortedDictionary<int, TimelineTrackWrapper> dictionary = new SortedDictionary<int, TimelineTrackWrapper>();
        foreach (TimelineTrackWrapper wrapper in this.timelineTrackMap.Keys)
        {
            this.timelineTrackMap[wrapper].TargetTrack = wrapper;
            dictionary.Add(wrapper.Ordinal, wrapper);
        }
        float num = header.y + 17f;
        foreach (int num2 in dictionary.Keys)
        {
            TimelineTrackWrapper wrapper2 = dictionary[num2];
            TimelineTrackControl control = this.timelineTrackMap[wrapper2];
            control.Ordinal = new int[] { this.trackGroup.Ordinal, num2 };
            float num3 = control.Rect.height;
            Rect position = new Rect(content.x, num, content.width, num3);
            Rect rect2 = new Rect(header.x, num, header.width, num3);
            Rect rect3 = new Rect(header.x, num, (header.width - this.sortingOptionsWidth) - 4f, num3);
            float introduced17 = rect3.x;
            Rect rect4 = new Rect(introduced17 + rect3.width, num, this.sortingOptionsWidth / 2f, 16f);
            control.UpdateTrackBodyBackground(position);
            control.UpdateHeaderBackground(rect2, num2);
            GUILayout.BeginArea(position);
            Rect rect5 = position;
            control.UpdateTrackContents(state, rect5);
            GUILayout.EndArea();
            control.UpdateHeaderContents(state, rect3, rect2);
            GUI.enabled=(num2 > 0);
            if (GUI.Button(rect4, string.Empty, DirectorControl.DirectorControlStyles.UpArrowIcon))
            {
                wrapper2.Ordinal--;
                TimelineTrackWrapper targetTrack = this.timelineTrackMap[dictionary[num2 - 1]].TargetTrack;
                targetTrack.Ordinal++;
            }
            GUI.enabled=(num2 < (dictionary.Count - 1));
            if (GUI.Button(new Rect(rect4.x + (this.sortingOptionsWidth / 2f), num, this.sortingOptionsWidth / 2f, 16f), string.Empty, DirectorControl.DirectorControlStyles.DownArrowIcon))
            {
                wrapper2.Ordinal++;
                TimelineTrackWrapper wrapper3 = this.timelineTrackMap[dictionary[num2 + 1]].TargetTrack;
                wrapper3.Ordinal--;
            }
            GUI.enabled=(true);
            num += num3;
        }
    }

    public DirectorControl DirectorControl
    {
        get
        {
            return this.directorControl;
        }
        set
        {
            this.directorControl = value;
        }
    }

    public TrackGroupWrapper TrackGroup
    {
        get
        {
            return this.trackGroup;
        }
        set
        {
            this.trackGroup = value;
            base.Behaviour = this.trackGroup.Behaviour;
        }
    }

    public TrackGroupControl ParentControl
    {
        get
        {
            return parentControl;
        }

        set
        {
            parentControl = value;
        }
    }

    public Rect Position
    {
        get
        {
            return position;
        }

        set
        {
            position = value;
        }
    }

    public class TrackGroupStyles
    {
        private GUIStyle actorGroupIcon;
        private GUIStyle addIcon;
        private GUIStyle backgroundContentSelected;
        private GUIStyle backgroundSelected;
        private GUIStyle characterGroupIcon;
        private GUIStyle directorGroupIcon;
        private GUIStyle inspectorIcon;
        private GUIStyle lockIconLRG;
        private GUIStyle lockIconSM;
        private GUIStyle multiActorGroupIcon;
        private GUIStyle pickerStyle;
        private GUIStyle trackGroupArea;
        private GUIStyle unlockIconLRG;
        private GUIStyle unlockIconSM;
        private GUIStyle muteSM;
        private GUIStyle unmuteSM;

        public TrackGroupStyles(GUISkin skin)
        {
            if (skin != null)
            {
                this.AddIcon = skin.FindStyle("Add");
                this.lockIconLRG = skin.FindStyle("LockItemLRG");
                this.unlockIconLRG = skin.FindStyle("UnlockItemLRG");
                this.lockIconSM = skin.FindStyle("LockItemSM");
                this.unlockIconSM = skin.FindStyle("UnlockItemSM");
                this.muteSM = skin.FindStyle("MuteOn");
                this.unmuteSM = skin.FindStyle("MuteOff");
                this.inspectorIcon = skin.FindStyle("InspectorIcon");
                this.trackGroupArea = skin.FindStyle("Track Group Area");
                this.directorGroupIcon = skin.FindStyle("DirectorGroupIcon");
                this.actorGroupIcon = skin.FindStyle("ActorGroupIcon");
                this.multiActorGroupIcon = skin.FindStyle("MultiActorGroupIcon");
                this.characterGroupIcon = skin.FindStyle("CharacterGroupIcon");
                this.pickerStyle = skin.FindStyle("Picker");
                this.backgroundSelected = skin.FindStyle("TrackGroupFocused");
                this.backgroundContentSelected = skin.FindStyle("TrackGroupContentFocused");
            }
        }

        public GUIStyle ActorGroupIcon
        {
            get
            {
                if (this.actorGroupIcon == null)
                {
                    this.actorGroupIcon = "box";
                }
                return this.actorGroupIcon;
            }
            set
            {
                this.actorGroupIcon = value;
            }
        }

        public GUIStyle AddIcon
        {
            get
            {
                if (this.addIcon == null)
                {
                    this.addIcon = "box";
                }
                return this.addIcon;
            }
            set
            {
                this.addIcon = value;
            }
        }

        public GUIStyle BackgroundContentSelected
        {
            get
            {
                if (this.backgroundContentSelected == null)
                {
                    this.backgroundContentSelected = "box";
                }
                return this.backgroundContentSelected;
            }
            set
            {
                this.backgroundContentSelected = value;
            }
        }

        public GUIStyle BackgroundSelected
        {
            get
            {
                if (this.backgroundSelected == null)
                {
                    this.backgroundSelected = "box";
                }
                return this.backgroundSelected;
            }
            set
            {
                this.backgroundSelected = value;
            }
        }

        public GUIStyle CharacterGroupIcon
        {
            get
            {
                if (this.characterGroupIcon == null)
                {
                    this.characterGroupIcon = "box";
                }
                return this.characterGroupIcon;
            }
            set
            {
                this.characterGroupIcon = value;
            }
        }

        public GUIStyle DirectorGroupIcon
        {
            get
            {
                if (this.directorGroupIcon == null)
                {
                    this.directorGroupIcon = "box";
                }
                return this.directorGroupIcon;
            }
            set
            {
                this.directorGroupIcon = value;
            }
        }

        public GUIStyle LockIconLRG
        {
            get
            {
                if (this.lockIconLRG == null)
                {
                    this.lockIconLRG = "box";
                }
                return this.lockIconLRG;
            }
            set
            {
                this.lockIconLRG = value;
            }
        }

        public GUIStyle LockIconSM
        {
            get
            {
                if (this.lockIconSM == null)
                {
                    this.lockIconSM = "box";
                }
                return this.lockIconSM;
            }
            set
            {
                this.lockIconSM = value;
            }
        }
        public GUIStyle MuteSM
        {
            get
            {
                if (this.muteSM == null)
                {
                    this.muteSM = "box";
                }
                return this.muteSM;
            }
            set
            {
                this.muteSM = value;
            }
        }
        public GUIStyle UnMuteSM
        {
            get
            {
                if (this.unmuteSM == null)
                {
                    this.unmuteSM = "box";
                }
                return this.unmuteSM;
            }
            set
            {
                this.unmuteSM = value;
            }
        }

        public GUIStyle MultiActorGroupIcon
        {
            get
            {
                if (this.multiActorGroupIcon == null)
                {
                    this.multiActorGroupIcon = "box";
                }
                return this.multiActorGroupIcon;
            }
            set
            {
                this.multiActorGroupIcon = value;
            }
        }

        public GUIStyle PickerStyle
        {
            get
            {
                if (this.pickerStyle == null)
                {
                    this.pickerStyle = "box";
                }
                return this.pickerStyle;
            }
            set
            {
                this.pickerStyle = value;
            }
        }

        public GUIStyle TrackGroupArea
        {
            get
            {
                if (this.trackGroupArea == null)
                {
                    this.trackGroupArea = "box";
                }
                return this.trackGroupArea;
            }
            set
            {
                this.trackGroupArea = value;
            }
        }

        public GUIStyle UnlockIconLRG
        {
            get
            {
                if (this.unlockIconLRG == null)
                {
                    this.unlockIconLRG = "box";
                }
                return this.unlockIconLRG;
            }
            set
            {
                this.unlockIconLRG = value;
            }
        }

        public GUIStyle UnlockIconSM
        {
            get
            {
                if (this.unlockIconSM == null)
                {
                    this.unlockIconSM = "box";
                }
                return this.unlockIconSM;
            }
            set
            {
                this.unlockIconSM = value;
            }
        }
    }
}

