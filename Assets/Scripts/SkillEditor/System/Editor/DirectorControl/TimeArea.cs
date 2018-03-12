using System;
using UnityEditor;
using UnityEngine;

public class TimeArea : ZoomableArea
{
    private TickHandler horizontalTicks;
    private DirectorControlSettings m_Settings = new DirectorControlSettings();
    private static TimeAreaStyle styles;

    public TimeArea()
    {
        float[] tickModulos = new float[] { 0.0005f, 0.001f, 0.005f, 0.01f, 0.05f, 0.1f, 0.5f, 1f, 5f, 10f, 50f, 100f, 500f, 1000f, 5000f, 10000f };
        this.hTicks = new TickHandler();
        this.hTicks.SetTickModulos(tickModulos);
    }

    private void ApplySettings()
    {
        base.hRangeLocked = this.settings.hRangeLocked;
        base.hRangeMin = this.settings.HorizontalRangeMin;
        base.hRangeMax = this.settings.hRangeMax;
        base.scaleWithWindow = this.settings.scaleWithWindow;
        base.hSlider = this.settings.hSlider;
    }

    public void DrawMajorTicks(Rect position, float frameRate)
    {
        Color color = Handles.color;
        GUI.BeginGroup(position);
        if (Event.current.type != (EventType)7)
        {
            GUI.EndGroup();
        }
        else
        {
            InitStyles();
            this.SetTickMarkerRanges();
            this.hTicks.SetTickStrengths(1f, 80f, true);
            Color color2 = styles.TimelineTick.normal.textColor;
            color2.a = 0.3f;
            Handles.color=(color2);
            for (int i = 0; i < this.hTicks.tickLevels; i++)
            {
                float strengthOfLevel = this.hTicks.GetStrengthOfLevel(i);
                if (strengthOfLevel > 0.5f)
                {
                    float[] ticksAtLevel = this.hTicks.GetTicksAtLevel(i, true);
                    for (int j = 0; j < ticksAtLevel.Length; j++)
                    {
                        if (ticksAtLevel[j] >= 0f)
                        {
                            int num4 = Mathf.RoundToInt(ticksAtLevel[j] * frameRate);
                            float num5 = this.FrameToPixel((float) num4, frameRate, position);
                            Handles.DrawLine(new Vector3(num5, 0f, 0f), new Vector3(num5, position.height, 0f));
                            if (strengthOfLevel > 0.8f)
                            {
                                Handles.DrawLine(new Vector3(num5 + 1f, 0f, 0f), new Vector3(num5 + 1f, position.height, 0f));
                            }
                        }
                    }
                }
            }
            GUI.EndGroup();
            Handles.color=(color);
        }
    }

    public string FormatFrame(int frame, float frameRate)
    {
        int num = (int) frameRate;
        int length = num.ToString().Length;
        int num3 = frame / num;
        float num4 = ((float) frame) % frameRate;
        return string.Format("{0}:{1}", num3.ToString(), num4.ToString().PadLeft(length, '0'));
    }

    public float FrameToPixel(float i, float frameRate, Rect rect)
    {
        Rect shownArea = base.shownArea;
        return (((i - (shownArea.xMin * frameRate)) * rect.width) / (shownArea.width * frameRate));
    }

    public float GetMajorTickDistance(float frameRate)
    {
        for (int i = 0; i < this.hTicks.tickLevels; i++)
        {
            if (this.hTicks.GetStrengthOfLevel(i) > 0.5f)
            {
                return this.hTicks.GetPeriodOfLevel(i);
            }
        }
        return 0f;
    }

    private static void InitStyles()
    {
        if (styles == null)
        {
            styles = new TimeAreaStyle();
        }
    }

    public void SetTickMarkerRanges()
    {
        Rect shownArea = base.shownArea;
        float minValue = shownArea.xMin;
        this.hTicks.SetRanges(minValue, shownArea.xMax, base.drawRect.xMin, base.drawRect.xMax);
    }

    public void TimeRuler(Rect position, float frameRate)
    {
        Color color = Handles.color;
        GUI.BeginGroup(position);
        if (Event.current.type != (EventType)7)
        {
            GUI.EndGroup();
        }
        else
        {
            InitStyles();
            this.SetTickMarkerRanges();
            this.hTicks.SetTickStrengths(1f, 80f, true);
            Color color2 = styles.TimelineTick.normal.textColor;
            color2.a = 0.3f;
            Handles.color=(color2);
            for (int i = 0; i < this.hTicks.tickLevels; i++)
            {
                float strengthOfLevel = this.hTicks.GetStrengthOfLevel(i);
                if (strengthOfLevel > 0.2f)
                {
                    float[] numArray2 = this.hTicks.GetTicksAtLevel(i, true);
                    for (int k = 0; k < numArray2.Length; k++)
                    {
                        if ((numArray2[k] >= base.hRangeMin) && (numArray2[k] <= base.hRangeMax))
                        {
                            int num5 = Mathf.RoundToInt(numArray2[k] * frameRate);
                            float num6 = (position.height * Mathf.Min(1f, strengthOfLevel)) * 0.7f;
                            float num7 = this.FrameToPixel((float) num5, frameRate, position);
                            Handles.DrawLine(new Vector3(num7, (position.height - num6) + 0.5f, 0f), new Vector3(num7, position.height - 0.5f, 0f));
                            if (strengthOfLevel > 0.5f)
                            {
                                Handles.DrawLine(new Vector3(num7 + 1f, (position.height - num6) + 0.5f, 0f), new Vector3(num7 + 1f, position.height - 0.5f, 0f));
                            }
                        }
                    }
                }
            }
            GL.End();
            int levelWithMinSeparation = this.hTicks.GetLevelWithMinSeparation(40f);
            float[] ticksAtLevel = this.hTicks.GetTicksAtLevel(levelWithMinSeparation, false);
            for (int j = 0; j < ticksAtLevel.Length; j++)
            {
                if ((ticksAtLevel[j] >= base.hRangeMin) && (ticksAtLevel[j] <= base.hRangeMax))
                {
                    int frame = Mathf.RoundToInt(ticksAtLevel[j] * frameRate);
                    string str = this.FormatFrame(frame, frameRate);
                    GUI.Label(new Rect(Mathf.Floor(this.FrameToPixel((float) frame, frameRate, base.rect)) + 3f, -3f, 40f, 20f), str, styles.TimelineTick);
                }
            }
            GUI.EndGroup();
            Handles.color=(color);
        }
    }

    internal TickHandler hTicks
    {
        get
        {
            return this.horizontalTicks;
        }
        set
        {
            this.horizontalTicks = value;
        }
    }

    internal DirectorControlSettings settings
    {
        get
        {
            return this.m_Settings;
        }
        set
        {
            if (value != null)
            {
                this.m_Settings = value;
                this.ApplySettings();
            }
        }
    }

    private class TimeAreaStyle
    {
        public GUIStyle labelTickMarks = "CurveEditorLabelTickMarks";
        public GUIStyle TimelineTick = "AnimationTimelineTick";
    }
}

