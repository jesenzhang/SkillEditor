using System;
using UnityEngine;

public class ZoomableArea
{
    private int horizontalScrollbarID;
    private Rect m_DrawArea = new Rect(0f, 0f, 100f, 100f);
    private bool m_HRangeLocked;
    private float m_HRangeMax = float.PositiveInfinity;
    private float m_HRangeMin = float.NegativeInfinity;
    private float m_HScaleMax = 100000f;
    private float m_HScaleMin = 0.001f;
    private float m_hScrollMax;
    private bool m_HSlider = true;
    private bool m_IgnoreScrollWheelUntilClicked;
    private Rect m_LastShownAreaInsideMargins = new Rect(0f, 0f, 100f, 100f);
    private float m_MarginBottom;
    public float m_MarginLeft;
    private float m_MarginRight;
    private float m_MarginTop;
    private bool m_MinimalGUI = false;
    private static Vector2 m_MouseDownPosition = new Vector2(-1000000f, -1000000f);
    private Vector2 m_Scale = new Vector2(1f, -1f);
    private bool m_ScaleWithWindow;
    private Vector2 m_Translation = new Vector2(0f, 0f);
    private Styles styles = new Styles();
    private static int zoomableAreaHash = "ZoomableArea".GetHashCode();

    public void BeginViewGUI(bool handleUserInteraction)
    {
        if (this.styles.horizontalScrollbar == null)
        {
            this.styles.InitGUIStyles();
        }
        Rect drawArea = this.m_DrawArea;
        drawArea.x=(0f);
        drawArea.y=(0f);
        GUILayout.BeginArea(this.drawRect);
        if (handleUserInteraction)
        {
            int num = GUIUtility.GetControlID(zoomableAreaHash, 0, drawArea);
            switch (Event.current.GetTypeForControl(num))
            {
                case 0:
                    if (drawArea.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.keyboardControl=(num);
                        if (this.IsZoomEvent() || this.IsPanEvent())
                        {
                            GUIUtility.hotControl=(num);
                            m_MouseDownPosition = this.mousePositionInDrawing;
                            Event.current.Use();
                        }
                    }
                    break;

                case (EventType)1:
                    if (GUIUtility.hotControl == num)
                    {
                        GUIUtility.hotControl=(0);
                        m_MouseDownPosition = new Vector2(-1000000f, -1000000f);
                    }
                    break;

                case (EventType)3:
                    if (GUIUtility.hotControl == num)
                    {
                        if (!this.IsZoomEvent())
                        {
                            if (this.IsPanEvent())
                            {
                                this.Pan();
                                Event.current.Use();
                            }
                            break;
                        }
                        this.Zoom(m_MouseDownPosition, false);
                        Event.current.Use();
                    }
                    break;

                case (EventType)6:
                    if ((drawArea.Contains(Event.current.mousePosition) && (GUIUtility.keyboardControl == num)) && Event.current.control)
                    {
                        this.Zoom(this.mousePositionInDrawing, true);
                        Event.current.Use();
                    }
                    break;
            }
        }
        GUILayout.EndArea();
        this.horizontalScrollbarID = GUIUtility.GetControlID(MinMaxSliderControl.s_MinMaxSliderHash, (FocusType)2);
        if (!this.m_MinimalGUI || (Event.current.type != (EventType)7))
        {
            this.SliderGUI();
        }
    }

    internal Vector2 DrawingToViewTransformPoint(Vector2 lhs)
    {
        return new Vector2((lhs.x * this.m_Scale.x) + this.m_Translation.x, (lhs.y * this.m_Scale.y) + this.m_Translation.y);
    }

    internal Vector2 DrawingToViewTransformVector(Vector2 lhs)
    {
        return new Vector2(lhs.x * this.m_Scale.x, lhs.y * this.m_Scale.y);
    }

    public void EndViewGUI()
    {
        if (this.m_MinimalGUI && (Event.current.type == (EventType)7))
        {
            this.SliderGUI();
        }
    }

    private void EnforceScaleAndRange()
    {
        float hScaleMin = this.m_HScaleMin;
        float hScaleMax = this.m_HScaleMax;
        if ((this.hRangeMax != float.PositiveInfinity) && (this.hRangeMin != float.NegativeInfinity))
        {
            hScaleMax = Mathf.Min(this.m_HScaleMax, this.hRangeMax - this.hRangeMin);
        }
        Rect lastShownAreaInsideMargins = this.m_LastShownAreaInsideMargins;
        Rect shownAreaInsideMargins = this.shownAreaInsideMargins;
        if (shownAreaInsideMargins != lastShownAreaInsideMargins)
        {
            float num3 = 1E-05f;
            if (shownAreaInsideMargins.width < (lastShownAreaInsideMargins.width - num3))
            {
                float num4 = Mathf.InverseLerp(lastShownAreaInsideMargins.width, shownAreaInsideMargins.width, hScaleMin);
                float num5 = Mathf.Lerp(lastShownAreaInsideMargins.x, shownAreaInsideMargins.x, num4);
                float num6 = Mathf.Lerp(lastShownAreaInsideMargins.width, shownAreaInsideMargins.width, num4);
                shownAreaInsideMargins = new Rect(num5, shownAreaInsideMargins.y, num6, shownAreaInsideMargins.height);
            }
            if (shownAreaInsideMargins.height < (lastShownAreaInsideMargins.height - num3))
            {
                float num7 = Mathf.InverseLerp(lastShownAreaInsideMargins.height, shownAreaInsideMargins.height, 1f);
                float num8 = Mathf.Lerp(lastShownAreaInsideMargins.y, shownAreaInsideMargins.y, num7);
                shownAreaInsideMargins = new Rect(shownAreaInsideMargins.x, num8, shownAreaInsideMargins.width, Mathf.Lerp(lastShownAreaInsideMargins.height, shownAreaInsideMargins.height, num7));
            }
            if (shownAreaInsideMargins.width > (lastShownAreaInsideMargins.width + num3))
            {
                float num9 = Mathf.InverseLerp(lastShownAreaInsideMargins.width, shownAreaInsideMargins.width, hScaleMax);
                float num10 = Mathf.Lerp(lastShownAreaInsideMargins.x, shownAreaInsideMargins.x, num9);
                float num11 = Mathf.Lerp(lastShownAreaInsideMargins.width, shownAreaInsideMargins.width, num9);
                shownAreaInsideMargins = new Rect(num10, shownAreaInsideMargins.y, num11, shownAreaInsideMargins.height);
            }
            if (shownAreaInsideMargins.height > (lastShownAreaInsideMargins.height + num3))
            {
                float num12 = Mathf.InverseLerp(lastShownAreaInsideMargins.height, shownAreaInsideMargins.height, 1f);
                float num13 = Mathf.Lerp(lastShownAreaInsideMargins.y, shownAreaInsideMargins.y, num12);
                shownAreaInsideMargins = new Rect(shownAreaInsideMargins.x, num13, shownAreaInsideMargins.width, Mathf.Lerp(lastShownAreaInsideMargins.height, shownAreaInsideMargins.height, num12));
            }
            if (shownAreaInsideMargins.xMin < this.hRangeMin)
            {
                shownAreaInsideMargins.x=(this.hRangeMin);
            }
            if (shownAreaInsideMargins.xMax > this.hRangeMax)
            {
                shownAreaInsideMargins.x=(this.hRangeMax - shownAreaInsideMargins.width);
            }
            this.shownAreaInsideMarginsInternal = shownAreaInsideMargins;
            this.m_LastShownAreaInsideMargins = shownAreaInsideMargins;
        }
    }

    private bool IsPanEvent()
    {
        return (((Event.current.button == 0) && Event.current.alt) || ((Event.current.button == 2) && !Event.current.command));
    }

    private bool IsZoomEvent()
    {
        return ((Event.current.button == 1) && Event.current.alt);
    }

    private void Pan()
    {
        if (!this.m_HRangeLocked)
        {
            this.m_Translation.x += Event.current.delta.x;
        }
        this.EnforceScaleAndRange();
    }

    internal void SetShownHRange(float min, float max)
    {
        this.m_Scale.x = this.drawRect.width / (max - min);
        this.m_Translation.x = -min * this.m_Scale.x;
        this.EnforceScaleAndRange();
    }

    internal void SetShownHRangeInsideMargins(float min, float max)
    {
        this.m_Scale.x = ((this.drawRect.width - this.leftmargin) - this.rightmargin) / (max - min);
        this.m_Translation.x = (-min * this.m_Scale.x) + this.leftmargin;
        this.EnforceScaleAndRange();
    }

    private void SliderGUI()
    {
        if (this.m_HSlider)
        {
            Bounds drawingBounds = this.drawingBounds;
            Rect shownAreaInsideMargins = this.shownAreaInsideMargins;
            float num3 = this.styles.sliderWidth - this.styles.visualSliderWidth;
            float num4 = !this.hSlider ? 0f : num3;
            if (this.m_HSlider)
            {
                float size = shownAreaInsideMargins.width;
                float num6 = shownAreaInsideMargins.xMin;
                MinMaxSliderControl.MinMaxScroller(new Rect(this.drawRect.x, this.drawRect.yMax - num3, this.drawRect.width - num4, this.styles.sliderWidth), this.horizontalScrollbarID, ref num6, ref size, drawingBounds.min.x, drawingBounds.max.x, float.NegativeInfinity, float.PositiveInfinity, this.styles.horizontalScrollbar, this.styles.horizontalMinMaxScrollbarThumb, this.styles.horizontalScrollbarLeftButton, this.styles.horizontalScrollbarRightButton, true);
                float min = num6;
                float max = num6 + size;
                if (min > shownAreaInsideMargins.xMin)
                {
                    min = Mathf.Min(min, max - this.m_HScaleMin);
                }
                if (max < shownAreaInsideMargins.xMax)
                {
                    max = Mathf.Max(max, min + this.m_HScaleMin);
                }
                this.SetShownHRangeInsideMargins(min, max);
            }
        }
    }

    internal float TimeToPixel(float time)
    {
        Rect shownArea = this.shownArea;
        return ((((time - shownArea.x) / shownArea.width) * this.m_DrawArea.width) + this.m_DrawArea.x);
    }

    internal float TimeToPixel(float time, Rect rect)
    {
        Rect shownArea = this.shownArea;
        return ((((time - shownArea.x) / shownArea.width) * rect.width) + rect.x);
    }

    internal Vector2 ViewToDrawingTransformPoint(Vector2 lhs)
    {
        return new Vector2((lhs.x - this.m_Translation.x) / this.m_Scale.x, (lhs.y - this.m_Translation.y) / this.m_Scale.y);
    }

    internal Vector2 ViewToDrawingTransformVector(Vector2 lhs)
    {
        return new Vector2(lhs.x / this.m_Scale.x, lhs.y / this.m_Scale.y);
    }

    private void Zoom(Vector2 zoomAround, bool scrollwhell)
    {
        float num = Event.current.delta.x + Event.current.delta.y;
        if (scrollwhell)
        {
            num = -num;
        }
        float num2 = Mathf.Max(0.01f, 1f + (num * 0.01f));
        if (!this.m_HRangeLocked)
        {
            this.m_Translation.x -= (zoomAround.x * (num2 - 1f)) * this.m_Scale.x;
            this.m_Scale.x *= num2;
        }
        this.EnforceScaleAndRange();
    }

    internal float bottommargin
    {
        get
        {
            return this.m_MarginBottom;
        }
        set
        {
            this.m_MarginBottom = value;
        }
    }

    internal virtual Bounds drawingBounds
    {
        get
        {
            bool flag = (this.hRangeMin > float.NegativeInfinity) && (this.hRangeMax < float.PositiveInfinity);
            return new Bounds(new Vector3(!flag ? (this.HScrollMax * 0.5f) : ((this.hRangeMin + this.hRangeMax) * 0.5f), 0f, 0f), new Vector3(!flag ? this.HScrollMax : (this.hRangeMax - this.hRangeMin), 2f, 1f));
        }
    }

    internal Rect drawRect
    {
        get
        {
            return this.m_DrawArea;
        }
    }

    internal bool hRangeLocked
    {
        get
        {
            return this.m_HRangeLocked;
        }
        set
        {
            this.m_HRangeLocked = value;
        }
    }

    internal float hRangeMax
    {
        get
        {
            return this.m_HRangeMax;
        }
        set
        {
            this.m_HRangeMax = value;
        }
    }

    internal float hRangeMin
    {
        get
        {
            return this.m_HRangeMin;
        }
        set
        {
            this.m_HRangeMin = value;
        }
    }

    internal float HScrollMax
    {
        get
        {
            return this.m_hScrollMax;
        }
        set
        {
            this.m_hScrollMax = value;
        }
    }

    internal bool hSlider
    {
        get
        {
            return this.m_HSlider;
        }
        set
        {
            Rect rect = this.rect;
            this.m_HSlider = value;
            this.rect = rect;
        }
    }

    internal bool ignoreScrollWheelUntilClicked
    {
        get
        {
            return this.m_IgnoreScrollWheelUntilClicked;
        }
        set
        {
            this.m_IgnoreScrollWheelUntilClicked = value;
        }
    }

    internal float leftmargin
    {
        get
        {
            return this.m_MarginLeft;
        }
        set
        {
            this.m_MarginLeft = value;
        }
    }

    internal float margin
    {
        set
        {
            this.m_MarginLeft = this.m_MarginRight = this.m_MarginTop = this.m_MarginBottom = value;
        }
    }

    internal Vector2 mousePositionInDrawing
    {
        get
        {
            return this.ViewToDrawingTransformPoint(Event.current.mousePosition);
        }
    }

    internal Rect rect
    {
        get
        {
            return new Rect(this.drawRect.x, this.drawRect.y, this.drawRect.width, this.drawRect.height + (!this.m_HSlider ? 0f : this.styles.visualSliderWidth));
        }
        set
        {
            Rect rect = new Rect(value.x, value.y, value.width, value.height - (!this.m_HSlider ? 0f : this.styles.visualSliderWidth));
            if (rect != this.m_DrawArea)
            {
                if (this.m_ScaleWithWindow)
                {
                    this.m_DrawArea = rect;
                    this.shownAreaInsideMargins = this.m_LastShownAreaInsideMargins;
                }
                else
                {
                    this.m_Translation += new Vector2((rect.width - this.m_DrawArea.width) / 2f, 0f);
                    this.m_DrawArea = rect;
                }
            }
            this.EnforceScaleAndRange();
        }
    }

    internal float rightmargin
    {
        get
        {
            return this.m_MarginRight;
        }
        set
        {
            this.m_MarginRight = value;
        }
    }

    public Vector2 scale
    {
        get
        {
            return this.m_Scale;
        }
    }

    internal Vector2 Scale
    {
        get
        {
            return this.m_Scale;
        }
        set
        {
            this.m_Scale = value;
        }
    }

    internal bool scaleWithWindow
    {
        get
        {
            return this.m_ScaleWithWindow;
        }
        set
        {
            this.m_ScaleWithWindow = value;
        }
    }

    internal Rect shownArea
    {
        get
        {
            return new Rect(-this.m_Translation.x / this.m_Scale.x, -(this.m_Translation.y - this.drawRect.height) / this.m_Scale.y, this.drawRect.width / this.m_Scale.x, this.drawRect.height / -this.m_Scale.y);
        }
        set
        {
            this.m_Scale.x = this.drawRect.width / value.width;
            this.m_Translation.x = -value.x * this.m_Scale.x;
            this.m_Translation.y = this.drawRect.height - (value.y * this.m_Scale.y);
            this.EnforceScaleAndRange();
        }
    }

    internal Rect shownAreaInsideMargins
    {
        get
        {
            return this.shownAreaInsideMarginsInternal;
        }
        set
        {
            this.shownAreaInsideMarginsInternal = value;
            this.EnforceScaleAndRange();
        }
    }

    internal Rect shownAreaInsideMarginsInternal
    {
        get
        {
            float num = this.leftmargin / this.m_Scale.x;
            float num2 = this.rightmargin / this.m_Scale.x;
            float num3 = this.topmargin / this.m_Scale.y;
            float num4 = this.bottommargin / this.m_Scale.y;
            Rect shownArea = this.shownArea;
            shownArea.x=(shownArea.x + num);
            shownArea.y=(shownArea.y - num3);
            shownArea.width=(shownArea.width - (num + num2));
            shownArea.height=(shownArea.height + (num3 + num4));
            return shownArea;
        }
        set
        {
            this.m_Scale.x = ((this.drawRect.width - this.leftmargin) - this.rightmargin) / value.width;
            this.m_Translation.x = (-value.x * this.m_Scale.x) + this.leftmargin;
            this.m_Translation.y = (this.drawRect.height - (value.y * this.m_Scale.y)) - this.topmargin;
        }
    }

    internal float topmargin
    {
        get
        {
            return this.m_MarginTop;
        }
        set
        {
            this.m_MarginTop = value;
        }
    }

    internal Vector2 Translation
    {
        get
        {
            return this.m_Translation;
        }
        set
        {
            this.m_Translation = value;
        }
    }

    [Serializable]
    internal class Styles
    {
        public GUIStyle horizontalMinMaxScrollbarThumb;
        public GUIStyle horizontalScrollbar;
        public GUIStyle horizontalScrollbarLeftButton;
        public GUIStyle horizontalScrollbarRightButton;
        public float sliderWidth = 15f;
        public float visualSliderWidth = 15f;

        public void InitGUIStyles()
        {
            this.horizontalMinMaxScrollbarThumb = "horizontalMinMaxScrollbarThumb";
            this.horizontalScrollbarLeftButton = "horizontalScrollbarLeftbutton";
            this.horizontalScrollbarRightButton = "horizontalScrollbarRightbutton";
            this.horizontalScrollbar = GUI.skin.horizontalScrollbar;
        }
    }
}

