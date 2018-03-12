using DirectorEditor;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TimeLine;
using UnityEditor;
using UnityEngine;

public class DirectorControl : TimeArea
{
    private Rect bodyArea;
    private GUISkin customSkin;
    private CutsceneWrapper cutscene;
    private DirectorControlState directorState = new DirectorControlState();
    private const int FRAME_RATE = 60;
    private Texture frameBackwardButton;
    private Texture frameForwardButton;
    private int frameRate;
    private bool hasLayoutChanged = true;
    private const float HEADER_HEIGHT = 17f;
    private Rect headerArea;
    private bool isBoxSelecting;
    private const float MARGIN = 20f;
    private Vector2 mouseDownPosition = Vector2.zero;
    private Texture pauseButton;
    private Texture playButton;
    private Rect previousControlArea;
    private const float SCROLLBAR_WIDTH = 15f;
    private Texture scrubDurationHead;
    private Texture scrubHead;
    private Rect selectionBox;
    private const float SIDEBAR_WIDTH = 4f;
    private Rect sidebarControlArea;
    private List<SidebarControl> sidebarControls = new List<SidebarControl>();
    private Texture stopButton;
    private List<TrackItemControl> timelineControls = new List<TrackItemControl>();
    private Rect timeRuleArea;
    private float track_header_area_width = 256f;
    private const float TRACK_HEADER_ICON_HEIGHT = 16f;
    private const float TRACK_HEADER_ICON_WIDTH = 16f;
    private const float TRACK_HEADER_WIDTH_MAX = 512f;
    private const float TRACK_HEADER_WIDTH_MIN = 256f;
    private Rect trackBodyBackground;
    private Rect trackBodyBackgroundNoScrollbars;
    private Rect trackBodyBackgroundNoVerticalScrollbar;
    private Dictionary<TrackGroupWrapper, TrackGroupControl> trackGroupBinding = new Dictionary<TrackGroupWrapper, TrackGroupControl>();
    private Rect verticalScrollbarArea;
    private float verticalScrollValue;

    [field: CompilerGenerated]
    public event DirectorDragHandler DragPerformed;

    [field: CompilerGenerated]
    public event CutsceneEventHandler EnterPreviewMode;

    [field: CompilerGenerated]
    public event CutsceneEventHandler ExitPreviewMode;

    [field: CompilerGenerated]
    public event CutsceneEventHandler PauseCutscene;

    [field: CompilerGenerated]
    public event CutsceneEventHandler PlayCutscene;

    [field: CompilerGenerated]
    public event CutsceneEventHandler RepaintRequest;

    [field: CompilerGenerated]
    public event CutsceneEventHandler ScrubCutscene;

    [field: CompilerGenerated]
    public event CutsceneEventHandler SetCutsceneTime;

    [field: CompilerGenerated]
    public event CutsceneEventHandler StopCutscene;

    public DirectorControl()
    {
        base.rect = new Rect();
        this.frameRate = 60;
        base.margin = 20f;
        DirectorControlSettings settings = new DirectorControlSettings {
            HorizontalRangeMin = 0f
        };
        base.settings = settings;
    }
    private void BindParent(CutsceneWrapper cutscene)
    {
        foreach (TrackGroupWrapper wrapper in trackGroupBinding.Keys)
        {
            foreach (TrackGroupWrapper wrapper1 in trackGroupBinding.Keys)
            {
                if (wrapper.Behaviour.transform.parent == wrapper1.Behaviour.transform)
                {
                    trackGroupBinding[wrapper].ParentControl = trackGroupBinding[wrapper1];
                    break;
                }
            }
        }
    }
    private void bindControls(CutsceneWrapper cutscene)
    {
        List<SidebarControl> newSidebarControls = new List<SidebarControl>();
        List<SidebarControl> removedSidebarControls = new List<SidebarControl>();
        List<TrackItemControl> newTimelineControls = new List<TrackItemControl>();
        List<TrackItemControl> removedTimelineControls = new List<TrackItemControl>();
        this.bindTrackGroupControls(cutscene, newSidebarControls, removedSidebarControls, newTimelineControls, removedTimelineControls);
        foreach (SidebarControl local1 in newSidebarControls)
        {
            local1.DeleteRequest += new DirectorBehaviourControlHandler(this.control_DeleteRequest);
            local1.DuplicateRequest += new SidebarControlHandler(this.sidebarControl_Duplicate);
            local1.SelectRequest += new SidebarControlHandler(this.sidebarControl_SelectRequest);
        }
        foreach (SidebarControl local2 in removedSidebarControls)
        {
            local2.DeleteRequest -= new DirectorBehaviourControlHandler(this.control_DeleteRequest);
            local2.DuplicateRequest -= new SidebarControlHandler(this.sidebarControl_Duplicate);
            local2.SelectRequest -= new SidebarControlHandler(this.sidebarControl_SelectRequest);
        }
        foreach (TrackItemControl control in newTimelineControls)
        {
            this.timelineControls.Add(control);
            control.DeleteRequest += new DirectorBehaviourControlHandler(this.control_DeleteRequest);
            control.RequestTrackItemTranslate += new TranslateTrackItemEventHandler(this.itemControl_RequestTrackItemTranslate);
            control.TrackItemTranslate += new TranslateTrackItemEventHandler(this.itemControl_TrackItemTranslate);
            control.TrackItemUpdate += new TrackItemEventHandler(this.itemControl_TrackItemUpdate);
        }
        foreach (TrackItemControl control2 in removedTimelineControls)
        {
            this.timelineControls.Remove(control2);
            control2.DeleteRequest -= new DirectorBehaviourControlHandler(this.control_DeleteRequest);
            control2.RequestTrackItemTranslate -= new TranslateTrackItemEventHandler(this.itemControl_RequestTrackItemTranslate);
            control2.TrackItemTranslate -= new TranslateTrackItemEventHandler(this.itemControl_TrackItemTranslate);
            control2.TrackItemUpdate -= new TrackItemEventHandler(this.itemControl_TrackItemUpdate);
        }
    }

    private void bindTrackGroupControls(CutsceneWrapper cutscene, List<SidebarControl> newSidebarControls, List<SidebarControl> removedSidebarControls, List<TrackItemControl> newTimelineControls, List<TrackItemControl> removedTimelineControls)
    {
        if (cutscene.HasChanged)
        {
            foreach (TrackGroupWrapper wrapper in cutscene.TrackGroups)
            {
                TrackGroupControl control = null;
                if (!this.trackGroupBinding.TryGetValue(wrapper, out control))
                {
                    Type type = typeof(TrackGroupControl);
                    int num2 = 0x7fffffff;
                    foreach (Type type2 in DirectorControlHelper.GetAllSubTypes(typeof(TrackGroupControl)))
                    {
                        Type c = null;
                        foreach (CutsceneTrackGroupAttribute attribute in type2.GetCustomAttributes(typeof(CutsceneTrackGroupAttribute), true))
                        {
                            if (attribute != null)
                            {
                                c = attribute.TrackGroupType;
                            }
                        }
                        if (c == wrapper.Behaviour.GetType())
                        {
                            type = type2;
                            num2 = 0;
                            break;
                        }
                        if (wrapper.Behaviour.GetType().IsSubclassOf(c))
                        {
                            Type baseType = wrapper.Behaviour.GetType();
                            int num5 = 0;
                            while ((baseType != null) && (baseType != c))
                            {
                                baseType = baseType.BaseType;
                                num5++;
                            }
                            if (num5 <= num2)
                            {
                                num2 = num5;
                                type = type2;
                            }
                        }
                    }
                    control = (TrackGroupControl) Activator.CreateInstance(type);
                    control.TrackGroup = wrapper;
                    control.DirectorControl = this;
                    control.Initialize();
                    control.SetExpandedFromEditorPrefs();
                    newSidebarControls.Add(control);
                    this.trackGroupBinding.Add(wrapper, control);
                }
            }
            List<TrackGroupWrapper> list = new List<TrackGroupWrapper>();
            foreach (TrackGroupWrapper wrapper2 in this.trackGroupBinding.Keys)
            {
                bool flag = false;
                foreach (TrackGroupWrapper wrapper3 in cutscene.TrackGroups)
                {
                    if (wrapper2.Equals(wrapper3))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    removedSidebarControls.Add(this.trackGroupBinding[wrapper2]);
                    list.Add(wrapper2);
                }
            }
            foreach (TrackGroupWrapper wrapper4 in list)
            {
                this.trackGroupBinding.Remove(wrapper4);
            }
            SortedDictionary<int, TrackGroupWrapper> dictionary = new SortedDictionary<int, TrackGroupWrapper>();
            List<TrackGroupWrapper> list2 = new List<TrackGroupWrapper>();
            foreach (TrackGroupWrapper wrapper5 in this.trackGroupBinding.Keys)
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
            int num = 0;
            using (SortedDictionary<int, TrackGroupWrapper>.ValueCollection.Enumerator enumerator4 = dictionary.Values.GetEnumerator())
            {
                while (enumerator4.MoveNext())
                {
                    enumerator4.Current.Ordinal = num;
                    num++;
                }
            }
            using (List<TrackGroupWrapper>.Enumerator enumerator3 = list2.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    enumerator3.Current.Ordinal = num;
                    num++;
                }
            }
            cutscene.HasChanged = false;
        }
        foreach (TrackGroupWrapper wrapper6 in this.trackGroupBinding.Keys)
        {
            this.trackGroupBinding[wrapper6].BindTrackControls(wrapper6, newSidebarControls, removedSidebarControls, newTimelineControls, removedTimelineControls);
        }
    }

    private void control_DeleteRequest(object sender, DirectorBehaviourControlEventArgs e)
    {
        foreach (TrackGroupWrapper wrapper in this.trackGroupBinding.Keys)
        {
            TrackGroupControl control = this.trackGroupBinding[wrapper];
            control.DeleteSelectedChildren();
            if (control.IsSelected)
            {
                control.Delete();
            }
        }
    }

    private void drawBackground()
    {
        GUI.Box(this.trackBodyBackground, GUIContent.none,GUI.skin.box);//"AnimationCurveEditorBackground"
        base.rect = this.trackBodyBackgroundNoVerticalScrollbar;
        base.BeginViewGUI(false);
        base.SetTickMarkerRanges();
        base.DrawMajorTicks(this.trackBodyBackground, (float) this.frameRate);
        base.EndViewGUI();
    }

    private float getTrackGroupsHeight(CutsceneWrapper cutscene)
    {
        float num = 0f;
        foreach (TrackGroupWrapper wrapper in cutscene.TrackGroups)
        {
            if (this.trackGroupBinding.ContainsKey(wrapper))
            {
                TrackGroupControl control = this.trackGroupBinding[wrapper];
                num += control.GetHeight();
            }
        }
        return num;
    }

    private float itemControl_RequestTrackItemTranslate(object sender, TrackItemEventArgs e)
    {
        float firetime = e.firetime;
        float num2 = e.firetime;
        bool flag = false;
        while (!flag && (num2 != 0f))
        {
            foreach (TrackItemControl control in this.timelineControls)
            {
                if (control.IsSelected)
                {
                    if (e.firetime > 0f)
                    {
                        num2 = Mathf.Min(control.RequestTranslate(firetime), num2);
                    }
                    else
                    {
                        num2 = Mathf.Max(control.RequestTranslate(firetime), num2);
                    }
                }
            }
            if (num2 != firetime)
            {
                firetime = num2;
            }
            else
            {
                flag = true;
            }
        }
        return firetime;
    }

    private float itemControl_TrackItemTranslate(object sender, TrackItemEventArgs e)
    {
        foreach (TrackItemControl control in this.timelineControls)
        {
            if (control.IsSelected)
            {
                control.Translate(e.firetime);
            }
        }
        return 0f;
    }

    private void itemControl_TrackItemUpdate(object sender, TrackItemEventArgs e)
    {
        foreach (TrackItemControl control in this.timelineControls)
        {
            if (control.IsSelected)
            {
                control.ConfirmTranslate();
            }
        }
    }

    public void OnDisable()
    {
        EditorPrefs.SetFloat("DirectorControl.areaX", base.shownAreaInsideMargins.x);
        EditorPrefs.SetFloat("DirectorControl.areaWidth", base.shownAreaInsideMargins.width);
        EditorPrefs.SetBool("DirectorControl.isSnappingEnabled", this.directorState.IsSnapEnabled);
        EditorPrefs.SetFloat("DirectorControl.SidebarWidth", this.track_header_area_width);
    }

    public void OnGUI(Rect controlArea, CutsceneWrapper cs)
    {
        this.cutscene = cs;
        this.updateControlLayout(controlArea);
        this.drawBackground();
        this.updateTimelineHeader(this.headerArea, this.timeRuleArea);
        if (this.cutscene != null)
        {
            this.bindControls(this.cutscene);
            this.BindParent(this.cutscene);
            this.updateControlState();
            float num = this.getTrackGroupsHeight(this.cutscene);
            if (Event.current.type == (EventType)6)
            {
                this.verticalScrollValue += (17f * Event.current.delta.y) / 3f;
            }
            this.verticalScrollValue = GUI.VerticalScrollbar(this.verticalScrollbarArea, this.verticalScrollValue, Mathf.Min(this.bodyArea.height, num), 0f, num);
            Vector2 vector = new Vector2(base.Translation.x, this.verticalScrollValue);
            base.Translation = vector;
            Rect area = new Rect(this.bodyArea.x, -base.Translation.y, this.bodyArea.width, num);
            this.directorState.Translation = base.Translation;
            this.directorState.Scale = base.Scale;
          
            GUILayout.BeginArea(this.bodyArea, string.Empty);
            this.updateTrackGroups(area);
            this.updateDurationBar();
            GUILayout.EndArea();
          
            this.updateScrubber();
            base.BeginViewGUI(true);
            this.updateUserInput();
            this.updateDragAndDrop();
        }
    }

    public void OnLoad(GUISkin skin)
    {
        this.customSkin = skin;
        string str = "Cinema Suite/Cinema Director/";
        string str2 = ".png";
        float min = 0f;
        float @float = 60f;
        if (EditorPrefs.HasKey("DirectorControl.areaX"))
        {
            min = EditorPrefs.GetFloat("DirectorControl.areaX");
        }
        if (EditorPrefs.HasKey("DirectorControl.areaWidth"))
        {
            @float = EditorPrefs.GetFloat("DirectorControl.areaWidth");
        }
        if (EditorPrefs.HasKey("DirectorControl.isSnappingEnabled"))
        {
            this.directorState.IsSnapEnabled = EditorPrefs.GetBool("DirectorControl.isSnappingEnabled");
        }
        base.SetShownHRangeInsideMargins(min, min + @float);
        if (EditorPrefs.HasKey("DirectorControl.SidebarWidth"))
        {
            this.track_header_area_width = EditorPrefs.GetFloat("DirectorControl.SidebarWidth");
        }
        if (this.playButton == null)
        {
            this.playButton = EditorGUIUtility.Load(str + "Director_PlayIcon" + str2) as Texture;
        }
        if (this.playButton == null)
        {
            Debug.Log("Play button icon missing from Resources folder.");
        }
        if (this.pauseButton == null)
        {
            this.pauseButton = EditorGUIUtility.Load(str + "Director_PauseIcon" + str2) as Texture;
        }
        if (this.pauseButton == null)
        {
            Debug.Log("Pause button missing from Resources folder.");
        }
        if (this.stopButton == null)
        {
            this.stopButton = EditorGUIUtility.Load(str + "Director_StopIcon" + str2) as Texture;
        }
        if (this.stopButton == null)
        {
            Debug.Log("Stop button icon missing from Resources folder.");
        }
        if (this.frameForwardButton == null)
        {
            this.frameForwardButton = EditorGUIUtility.Load(str + "Director_FrameForwardIcon" + str2) as Texture;
        }
        if (this.frameForwardButton == null)
        {
            Debug.Log("Director_FrameForwardIcon.png missing from Resources folder.");
        }
        if (this.frameBackwardButton == null)
        {
            this.frameBackwardButton = EditorGUIUtility.Load(str + "Director_FrameBackwardIcon" + str2) as Texture;
        }
        if (this.frameBackwardButton == null)
        {
            Debug.Log("Director_FrameBackwardIcon.png missing from Resources folder.");
        }
        if (this.scrubHead == null)
        {
            this.scrubHead = EditorGUIUtility.Load(str + "Director_Playhead" + str2) as Texture;
        }
        if (this.scrubHead == null)
        {
            Debug.Log("Director_Playhead missing from Resources folder.");
        }
        if (this.scrubDurationHead == null)
        {
            this.scrubDurationHead = EditorGUIUtility.Load(str + "Director_Duration_Playhead" + str2) as Texture;
        }
        if (this.scrubDurationHead == null)
        {
            Debug.Log("Director_Duration_Playhead missing from Resources folder.");
        }
        if (this.customSkin != null)
        {
            DirectorControlStyles.BoxSelect = this.customSkin.FindStyle("BoxSelect");
            DirectorControlStyles.UpArrowIcon = this.customSkin.FindStyle("UpArrowIcon");
            DirectorControlStyles.DownArrowIcon = this.customSkin.FindStyle("DownArrowIcon");
        }
        else
        {
            DirectorControlStyles.BoxSelect = "box";
            DirectorControlStyles.UpArrowIcon = "box";
            DirectorControlStyles.DownArrowIcon = "box";
        }
        TrackGroupControl.InitStyles(this.customSkin);
        TimelineTrackControl.InitStyles(this.customSkin);
    }

    public void Repaint()
    {
        if (this.RepaintRequest != null)
        {
            this.RepaintRequest(this, new CinemaDirectorArgs(this.cutscene.Behaviour));
        }
    }

    public void Rescale()
    {
        if (this.cutscene != null)
        {
            base.SetShownHRangeInsideMargins(0f, this.cutscene.Duration);
        }
        else
        {
            base.SetShownHRangeInsideMargins(0f, 60f);
        }
    }

    private void sidebarControl_Duplicate(object sender, SidebarControlEventArgs e)
    {
        foreach (TrackGroupWrapper wrapper in this.trackGroupBinding.Keys)
        {
            TrackGroupControl control = this.trackGroupBinding[wrapper];
            control.DuplicateSelectedChildren();
            if (control.IsSelected)
            {
                control.Duplicate();
            }
        }
    }

    private void sidebarControl_SelectRequest(object sender, SidebarControlEventArgs e)
    {
        Behaviour behaviour = e.Behaviour;
        if (behaviour != null)
        {
            if (Event.current.control)
            {
                if (Selection.Contains(behaviour.gameObject))
                {
                    GameObject[] objArray = Selection.gameObjects;
                    ArrayUtility.Remove<GameObject>(ref objArray, behaviour.gameObject);
                    Selection.objects=(objArray);
                }
                else
                {
                    GameObject[] objArray2 = Selection.gameObjects;
                    ArrayUtility.Add<GameObject>(ref objArray2, behaviour.gameObject);
                    Selection.objects=(objArray2);
                }
            }
            else if (Event.current.shift)
            {
                List<SidebarControl> list = new List<SidebarControl>();
                foreach (TrackGroupWrapper wrapper in this.trackGroupBinding.Keys)
                {
                    TrackGroupControl item = this.trackGroupBinding[wrapper];
                    list.Add(item);
                    list.AddRange(item.GetSidebarControlChildren(true));
                }
                SidebarControl sidebarControl = e.SidebarControl;
                SidebarControl control2 = e.SidebarControl;
                foreach (SidebarControl control4 in list)
                {
                    if (control4.IsSelected)
                    {
                        if (sidebarControl.CompareTo(control4) > 0)
                        {
                            sidebarControl = control4;
                        }
                        if (control2.CompareTo(control4) < 0)
                        {
                            control2 = control4;
                        }
                    }
                }
                foreach (SidebarControl control5 in list)
                {
                    if ((!control5.IsSelected && (sidebarControl.CompareTo(control5) <= 0)) && (control2.CompareTo(control5) >= 0))
                    {
                        control5.Select();
                    }
                }
            }
            else
            {
                Selection.activeObject=(behaviour);
            }
            Event.current.Use();
        }
    }

    private void updateControlLayout(Rect controlArea)
    {
        this.hasLayoutChanged = controlArea != this.previousControlArea;
        this.headerArea = new Rect(controlArea.x, controlArea.y, controlArea.width, 17f);
        this.sidebarControlArea = new Rect(this.track_header_area_width, this.headerArea.y + 17f, 4f, (controlArea.height - 17f) - 15f);
        EditorGUIUtility.AddCursorRect(this.sidebarControlArea, (MouseCursor)3);
        int num = GUIUtility.GetControlID("SidebarResize".GetHashCode(), (FocusType)2, this.sidebarControlArea);
        switch (Event.current.GetTypeForControl(num))
        {
            case 0:
                if (this.sidebarControlArea.Contains(Event.current.mousePosition) && (Event.current.button == 0))
                {
                    GUIUtility.hotControl=(num);
                    Event.current.Use();
                }
                break;

            case (EventType)1:
                if (GUIUtility.hotControl == num)
                {
                    GUIUtility.hotControl=(0);
                }
                break;

            case (EventType)3:
                if (GUIUtility.hotControl == num)
                {
                    this.track_header_area_width = Mathf.Clamp(Event.current.mousePosition.x, 256f, 512f);
                    this.hasLayoutChanged = true;
                }
                break;
        }
        if (this.hasLayoutChanged)
        {
            this.timeRuleArea = new Rect(this.track_header_area_width + this.sidebarControlArea.width, controlArea.y, ((controlArea.width - this.track_header_area_width) - 15f) - this.sidebarControlArea.width, 17f);
            this.bodyArea = new Rect(controlArea.x, this.headerArea.y + 17f, controlArea.width - 15f, (controlArea.height - 17f) - 15f);
            this.trackBodyBackground = new Rect((controlArea.x + this.track_header_area_width) + this.sidebarControlArea.width, this.bodyArea.y, ((controlArea.width - 15f) - this.track_header_area_width) - this.sidebarControlArea.width, (controlArea.height - 17f) - 15f);
            this.trackBodyBackgroundNoVerticalScrollbar = new Rect((controlArea.x + this.track_header_area_width) + this.sidebarControlArea.width, this.bodyArea.y, ((controlArea.width - 15f) - this.track_header_area_width) - this.sidebarControlArea.width, controlArea.height - 17f);
            this.trackBodyBackgroundNoScrollbars = new Rect((controlArea.x + this.track_header_area_width) + this.sidebarControlArea.width, this.bodyArea.y, ((controlArea.width - 15f) - this.track_header_area_width) - this.sidebarControlArea.width, (controlArea.height - 17f) - 15f);
            float introduced2 = this.bodyArea.x;
            this.verticalScrollbarArea = new Rect(introduced2 + this.bodyArea.width, this.bodyArea.y, 15f, (controlArea.height - 17f) - 15f);
        }
        this.previousControlArea = controlArea;
    }

    private void updateControlState()
    {
        base.HScrollMax = this.cutscene.Duration;
        this.directorState.TickDistance = base.GetMajorTickDistance((float) this.frameRate);
        this.directorState.ScrubberPosition = this.cutscene.RunningTime;
    }

    private void updateDragAndDrop()
    {
        Event event2 = Event.current;
        if (event2.type == (EventType)15)
        {
            DragAndDrop.PrepareStartDrag();
        }
        if (this.bodyArea.Contains(event2.mousePosition))
        {
            EventType type = event2.type;
            if (type != (EventType)9)
            {
                if (type == (EventType)10)
                {
                    DragAndDrop.AcceptDrag();
                    if (this.DragPerformed != null)
                    {
                        this.DragPerformed(this, new CinemaDirectorDragArgs(this.cutscene.Behaviour, DragAndDrop.objectReferences));
                    }
                    event2.Use();
                }
            }
            else
            {
                DragAndDrop.visualMode=((DragAndDropVisualMode)2);
            }
        }
    }

    private void updateDurationBar()
    {
        float num = this.directorState.TimeToPosition(this.cutscene.Duration) + this.trackBodyBackground.x;
        Color color = GUI.color;
       GUI.color=(new Color(0.25f, 0.5f, 0.5f));
        Rect rect = new Rect(num - 8f, this.bodyArea.height - 13f, 16f, 16f);
        int num2 = GUIUtility.GetControlID("DurationBar".GetHashCode(), (FocusType)2, rect);
        switch (Event.current.GetTypeForControl(num2))
        {
            case 0:
                if (rect.Contains(Event.current.mousePosition))
                {
                    GUIUtility.hotControl=(num2);
                    Event.current.Use();
                }
                break;

            case (EventType)1:
                if (GUIUtility.hotControl == num2)
                {
                    GUIUtility.hotControl=(0);
                }
                break;

            case (EventType)3:
                if (GUIUtility.hotControl == num2)
                {
                    Vector2 lhs = Event.current.mousePosition;
                    lhs.x -= this.trackBodyBackground.x;
                    Undo.RecordObject(this.cutscene.Behaviour, "Changed Cutscene Duration");
                    float x = base.ViewToDrawingTransformPoint(lhs).x;
                    this.cutscene.Duration = this.directorState.SnappedTime(x);
                    Event.current.Use();
                }
                break;
        }
        if ((num > this.trackBodyBackground.x) && (num < this.bodyArea.width))
        {
            GUI.DrawTexture(rect, this.scrubDurationHead);
        }
        GUI.color=(color);
        Handles.color=(new Color(0.25f, 0.5f, 0.5f));
        if (num > this.trackBodyBackground.x)
        {
            Handles.DrawLine(new Vector3(num, 0f, 0f), new Vector2(num, (this.timeRuleArea.y + this.trackBodyBackgroundNoVerticalScrollbar.height) - 13f));
            Handles.DrawLine(new Vector3(num + 1f, 0f, 0f), new Vector2(num + 1f, (this.timeRuleArea.y + this.trackBodyBackgroundNoVerticalScrollbar.height) - 13f));
        }
    }

    private void updateScrubber()
    {
        if ((Event.current.type == (EventType)7) && (this.InPreviewMode || this.cutscene.IsPlaying))
        {
            float num = this.directorState.TimeToPosition(this.cutscene.RunningTime) + this.trackBodyBackground.x;
            GUI.color=(new Color(1f, 0f, 0f, 1f));
            Handles.color=(new Color(1f, 0f, 0f, 1f));
            if ((num > this.trackBodyBackground.x) && (num < this.bodyArea.width))
            {
                GUI.DrawTexture(new Rect(num - 8f, 20f, 16f, 16f), this.scrubHead);
                Handles.DrawLine(new Vector2(num, 34f), new Vector2(num, (this.timeRuleArea.y + this.trackBodyBackgroundNoVerticalScrollbar.height) + 3f));
                Handles.DrawLine(new Vector2(num + 1f, 34f), new Vector2(num + 1f, (this.timeRuleArea.y + this.trackBodyBackgroundNoVerticalScrollbar.height) + 3f));
            }
            GUI.color=(GUI.color);
        }
    }

    private void updateTimelineHeader(Rect headerArea, Rect timeRulerArea)
    {
        GUILayout.BeginArea(headerArea, string.Empty, EditorStyles.toolbarButton);
        this.updateToolbar();
        GUILayout.BeginArea(timeRulerArea, string.Empty, EditorStyles.toolbarButton);
        GUILayout.EndArea();
        GUILayout.EndArea();
        base.TimeRuler(timeRulerArea, (float) this.frameRate);
        if (this.cutscene != null)
        {
            int num = GUIUtility.GetControlID("TimeRuler".GetHashCode(), (FocusType)2, timeRulerArea);
            switch (Event.current.GetTypeForControl(num))
            {
                case 0:
                    if (timeRulerArea.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.hotControl=(num);
                        Vector2 lhs = Event.current.mousePosition;
                        lhs.x -= timeRulerArea.x;
                        this.InPreviewMode = true;
                        float time = Mathf.Max(base.ViewToDrawingTransformPoint(lhs).x, 0f);
                        if (this.cutscene == null)
                        {
                            return;
                        }
                        this.directorState.ScrubberPosition = time;
                        this.SetCutsceneTime(this, new CinemaDirectorArgs(this.cutscene.Behaviour, time));
                    }
                    return;

                case (EventType)1:
                    if (GUIUtility.hotControl == num)
                    {
                        GUIUtility.hotControl=(0);
                        if (this.cutscene == null)
                        {
                            return;
                        }
                        this.PauseCutscene(this, new CinemaDirectorArgs(this.cutscene.Behaviour));
                    }
                    return;

                case (EventType)3:
                    if (GUIUtility.hotControl == num)
                    {
                        Vector2 vector2 = Event.current.mousePosition;
                        vector2.x -= timeRulerArea.x;
                        float num3 = Mathf.Max(base.ViewToDrawingTransformPoint(vector2).x, 0f);
                        if (this.cutscene != null)
                        {
                            this.ScrubCutscene(this, new CinemaDirectorArgs(this.cutscene.Behaviour, num3));
                            this.directorState.ScrubberPosition = num3;
                        }
                        Event.current.Use();
                    }
                    return;
            }
            if (GUIUtility.hotControl == num)
            {
                Vector2 vector3 = Event.current.mousePosition;
                vector3.x -= timeRulerArea.x;
                float num4 = Mathf.Max(base.ViewToDrawingTransformPoint(vector3).x, 0f);
                if (this.cutscene != null)
                {
                    this.ScrubCutscene(this, new CinemaDirectorArgs(this.cutscene.Behaviour, num4));
                    this.directorState.ScrubberPosition = num4;
                }
            }
        }
    }

    private void updateToolbar()
    {
        GUILayoutOption[] optionArray = new GUILayoutOption[] { GUILayout.MaxWidth(150f) };
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, optionArray);
        GUILayout.FlexibleSpace();
        if ((this.cutscene != null) && this.cutscene.IsPlaying)
        {
            if (GUILayout.Button(this.pauseButton, EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.PauseCutscene(this, new CinemaDirectorArgs(this.cutscene.Behaviour));
            }
        }
        else if (GUILayout.Button(this.playButton, EditorStyles.toolbarButton, new GUILayoutOption[0]) && (this.cutscene != null))
        {
            this.InPreviewMode = true;
            this.PlayCutscene(this, new CinemaDirectorArgs(this.cutscene.Behaviour));
        }
        if (GUILayout.Button(this.stopButton, EditorStyles.toolbarButton, new GUILayoutOption[0]) && (this.cutscene != null))
        {
            this.InPreviewMode = false;
            this.StopCutscene(this, new CinemaDirectorArgs(this.cutscene.Behaviour));
        }
        GUILayout.FlexibleSpace();
        if (((Event.current.type == (EventType)4) && !EditorGUIUtility.editingTextField) && (Event.current.keyCode == (KeyCode)0x20))
        {
            if (!this.cutscene.IsPlaying)
            {
                this.InPreviewMode = true;
                this.PlayCutscene(this, new CinemaDirectorArgs(this.cutscene.Behaviour));
            }
            else
            {
                this.PauseCutscene(this, new CinemaDirectorArgs(this.cutscene.Behaviour));
            }
            Event.current.Use();
        }
        float time = 0f;
        if (this.cutscene != null)
        {
            time = this.cutscene.RunningTime;
        }
        GUILayout.Space(10f);
        GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(50f) };
        time = EditorGUILayout.FloatField(time, optionArray2);
        if ((this.cutscene != null) && (time != this.cutscene.RunningTime))
        {
            this.InPreviewMode = true;
            time = Mathf.Max(time, 0f);
            this.directorState.ScrubberPosition = time;
            this.SetCutsceneTime(this, new CinemaDirectorArgs(this.cutscene.Behaviour, time));
        }
        EditorGUILayout.EndHorizontal();
    }

    private void updateTrackGroups(Rect area)
    {
        float num = area.y;
        SortedDictionary<int, TrackGroupWrapper> dictionary = new SortedDictionary<int, TrackGroupWrapper>();
        foreach (TrackGroupWrapper wrapper in this.trackGroupBinding.Keys)
        {
            this.trackGroupBinding[wrapper].TrackGroup = wrapper;
            dictionary.Add(wrapper.Ordinal, wrapper);
        }
        foreach (int num2 in dictionary.Keys)
        {
            TrackGroupWrapper trackGroup = dictionary[num2];
            TrackGroupControl control = this.trackGroupBinding[trackGroup];
            bool parentExp = control.ParentExpended();
            float offset = 0;

            control.Ordinal = new int[] { num2 };
            float height = control.GetHeight();
            Rect position = new Rect(area.x + offset, num, area.width - offset, height);
            Rect fullHeader = new Rect(area.x + offset, num, this.track_header_area_width + this.sidebarControlArea.width - offset, height);
            Rect safeHeader = new Rect(area.x + offset, num, this.track_header_area_width - 32f - offset, height);
            float introduced16 = safeHeader.x;
            Rect rect4 = new Rect(introduced16 + safeHeader.width, num, 16f, 16f);
            float introduced17 = fullHeader.x;
            Rect content = new Rect(introduced17 + fullHeader.width, num, area.width - fullHeader.width, height);

            if (control.ParentControl != null)
            {
                offset =control.ParentControl.Position.x+14;
                position = new Rect(offset, num, control.ParentControl.Position.width-14, height);
                float of = area.width - position.width;
                fullHeader = new Rect( offset, num, this.track_header_area_width + this.sidebarControlArea.width - of, height);
                safeHeader = new Rect( offset, num, this.track_header_area_width + this.sidebarControlArea.width - of-32, height);
                introduced16 = safeHeader.x;
                rect4 = new Rect(introduced16 + safeHeader.width, num, 16f, 16f);
                introduced17 = fullHeader.x;
                content = new Rect(introduced17 + fullHeader.width, num, area.width - fullHeader.width, height);
            }
            
            control.Update(trackGroup, this.directorState, position, fullHeader, safeHeader, content);
           
            if (parentExp)
            {
                GUI.enabled = (num2 > 0);
                if (GUI.Button(rect4, string.Empty, DirectorControlStyles.UpArrowIcon))
                {
                    trackGroup.Ordinal--;
                    TrackGroupWrapper wrapper1 = this.trackGroupBinding[dictionary[num2 - 1]].TrackGroup;
                    wrapper1.Ordinal++;
                }
                GUI.enabled = (num2 < (dictionary.Count - 1));
                if (GUI.Button(new Rect(rect4.x + 16f, num, 16f, 16f), string.Empty, DirectorControlStyles.DownArrowIcon))
                {
                    trackGroup.Ordinal++;
                    TrackGroupWrapper wrapper3 = this.trackGroupBinding[dictionary[num2 + 1]].TrackGroup;
                    wrapper3.Ordinal--;
                }
                GUI.enabled = (true);
            }
            num += height;
        }
    }

    private void updateUserInput()
    {
        int num = GUIUtility.GetControlID("DirectorBody".GetHashCode(), (FocusType)2, this.trackBodyBackgroundNoVerticalScrollbar);
        switch (Event.current.GetTypeForControl(num))
        {
            case 0:
                if (this.trackBodyBackgroundNoVerticalScrollbar.Contains(Event.current.mousePosition) && (Event.current.button == 0))
                {
                    this.isBoxSelecting = true;
                    this.mouseDownPosition = Event.current.mousePosition;
                    Selection.activeObject=(null);
                    GUIUtility.hotControl=(num);
                    Event.current.Use();
                }
                break;

            case (EventType)1:
                if (GUIUtility.hotControl == num)
                {
                    this.isBoxSelecting = false;
                    this.selectionBox = new Rect();
                    GUIUtility.hotControl=(0);
                }
                break;

            case (EventType)3:
                if (GUIUtility.hotControl == num)
                {
                    float introduced11 = this.trackBodyBackgroundNoScrollbars.x;
                    float num2 = Mathf.Clamp(Event.current.mousePosition.x, introduced11, this.trackBodyBackgroundNoScrollbars.xMax);
                    float introduced12 = this.trackBodyBackgroundNoScrollbars.y;
                    float num3 = Mathf.Clamp(Event.current.mousePosition.y, introduced12, this.trackBodyBackgroundNoScrollbars.yMax);
                    float num4 = Mathf.Min(this.mouseDownPosition.x, num2);
                    float num5 = Mathf.Abs(num2 - this.mouseDownPosition.x);
                    float num6 = Mathf.Min(this.mouseDownPosition.y, num3);
                    float num7 = Mathf.Abs(this.mouseDownPosition.y - num3);
                    this.selectionBox = new Rect(num4, num6, num5, num7);
                    Rect selectionBox = new Rect(this.selectionBox);
                    selectionBox.y=(selectionBox.y- 34f);
                    foreach (TrackGroupWrapper wrapper in this.trackGroupBinding.Keys)
                    {
                        this.trackGroupBinding[wrapper].BoxSelect(selectionBox);
                    }
                }
                break;
        }
        if (this.isBoxSelecting)
        {
            GUI.Box(this.selectionBox, GUIContent.none, DirectorControlStyles.BoxSelect);
        }
    }

    public void ZoomIn()
    {
        base.Scale = (Vector2) (base.Scale * 1.5f);
    }

    public void ZoomOut()
    {
        base.Scale = (Vector2) (base.Scale * 0.75f);
    }

    public bool InPreviewMode
    {
        get
        {
            return this.directorState.IsInPreviewMode;
        }
        set
        {
            if (this.cutscene != null)
            {
                if (!this.directorState.IsInPreviewMode & value)
                {
                    this.EnterPreviewMode(this, new CinemaDirectorArgs(this.cutscene.Behaviour));
                }
                else if (this.directorState.IsInPreviewMode && !value)
                {
                    this.ExitPreviewMode(this, new CinemaDirectorArgs(this.cutscene.Behaviour));
                }
            }
            this.directorState.IsInPreviewMode = value;
        }
    }

    public bool IsSnappingEnabled
    {
        get
        {
            return this.directorState.IsSnapEnabled;
        }
        set
        {
            this.directorState.IsSnapEnabled = value;
        }
    }

    public DirectorEditor.ResizeOption ResizeOption
    {
        get
        {
            return this.directorState.ResizeOption;
        }
        set
        {
            this.directorState.ResizeOption = value;
        }
    }

    public static class DirectorControlStyles
    {
        public static GUIStyle BoxSelect;
        public static GUIStyle DownArrowIcon;
        public static GUIStyle UpArrowIcon;
    }
}

