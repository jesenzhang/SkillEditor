using System;
using UnityEngine;

internal class MinMaxSliderControl
{
    private static int firstScrollWait = 250;
    private static int kFirstScrollWait = 250;
    private static int kScrollWait = 30;
    private static float nextScrollStepTime = 0f;
    private static int repeatButtonHash = "repeatButton".GetHashCode();
    internal static int s_MinMaxSliderHash = "MinMaxSlider".GetHashCode();
    private static MinMaxSliderState s_MinMaxSliderState;
    private static DateTime s_NextScrollStepTime = DateTime.Now;
    private static int scrollControlID;
    private static int scrollWait = 30;

    internal static void DoMinMaxSlider(Rect position, int id, ref float value, ref float size, float visualStart, float visualEnd, float startLimit, float endLimit, GUIStyle slider, GUIStyle thumb, bool horiz)
    {
        Event event2 = Event.current;
        bool flag = size == 0f;
        float dragStartLimit = Mathf.Min(visualStart, visualEnd);
        float dragEndLimit = Mathf.Max(visualStart, visualEnd);
        float num7 = Mathf.Min(startLimit, endLimit);
        float num8 = Mathf.Max(startLimit, endLimit);
        MinMaxSliderState state = s_MinMaxSliderState;
        if ((GUIUtility.hotControl == id) && (state != null))
        {
            dragStartLimit = state.dragStartLimit;
            num7 = state.dragStartLimit;
            dragEndLimit = state.dragEndLimit;
            num8 = state.dragEndLimit;
        }
        float num9 = 0f;
        float num10 = Mathf.Clamp(value, dragStartLimit, dragEndLimit);
        float num11 = Mathf.Clamp(value + size, dragStartLimit, dragEndLimit) - num10;
        float num12 = (visualStart <= visualEnd) ? 1f : -1f;
        if ((slider != null) && (thumb != null))
        {
            float num;
            float num2;
            Rect rect;
            Rect rect2;
            Rect rect3;
            float num3;
            float num4;
            if (horiz)
            {
                float num13 = (thumb.fixedWidth == 0f) ? ((float) thumb.padding.horizontal) : thumb.fixedWidth;
                num = ((position.width - slider.padding.horizontal) - num13) / (dragEndLimit - dragStartLimit);
                rect = new Rect((((num10 - dragStartLimit) * num) + position.x) + slider.padding.left, position.y + slider.padding.top, (num11 * num) + num13, position.height - slider.padding.vertical);
                float introduced24 = rect.x;
                rect2 = new Rect(introduced24, rect.y, (float) thumb.padding.left, rect.height);
                rect3 = new Rect(rect.xMax - thumb.padding.right, rect.y, (float) thumb.padding.right, rect.height);
                num2 = event2.mousePosition.x - position.x;
            }
            else
            {
                float num14 = (thumb.fixedHeight == 0f) ? ((float) thumb.padding.vertical) : thumb.fixedHeight;
                num = ((position.height - slider.padding.vertical) - num14) / (dragEndLimit - dragStartLimit);
                rect = new Rect(position.x + slider.padding.left, (((num10 - dragStartLimit) * num) + position.y) + slider.padding.top, position.width - slider.padding.horizontal, (num11 * num) + num14);
                float introduced25 = rect.x;
                float introduced26 = rect.y;
                rect2 = new Rect(introduced25, introduced26, rect.width, (float) thumb.padding.top);
                float introduced27 = rect.x;
                rect3 = new Rect(introduced27, rect.yMax - thumb.padding.bottom, rect.width, (float) thumb.padding.bottom);
                num2 = event2.mousePosition.y - position.y;
            }
            switch (event2.GetTypeForControl(id))
            {
                case 0:
                    if (position.Contains(event2.mousePosition) && ((dragStartLimit - dragEndLimit) != 0f))
                    {
                        if (state == null)
                        {
                            state = s_MinMaxSliderState = new MinMaxSliderState();
                        }
                        if (!rect.Contains(event2.mousePosition))
                        {
                            if (slider != GUIStyle.none)
                            {
                                if ((size != 0f) & flag)
                                {
                                    if (horiz)
                                    {
                                        if (num2 > (rect.xMax - position.x))
                                        {
                                            value += (size * num12) * 0.9f;
                                        }
                                        else
                                        {
                                            value -= (size * num12) * 0.9f;
                                        }
                                    }
                                    else if (num2 > (rect.yMax - position.y))
                                    {
                                        value += (size * num12) * 0.9f;
                                    }
                                    else
                                    {
                                        value -= (size * num12) * 0.9f;
                                    }
                                    state.whereWeDrag = 0;
                                    GUI.changed=(true);
                                    s_NextScrollStepTime = DateTime.Now.AddMilliseconds((double) kFirstScrollWait);
                                    num3 = !horiz ? event2.mousePosition.y : event2.mousePosition.x;
                                    num4 = !horiz ? rect.y : rect.x;
                                    state.whereWeDrag = (num3 <= num4) ? 3 : 4;
                                }
                                else
                                {
                                    if (horiz)
                                    {
                                        value = (((num2 - (rect.width * 0.5f)) / num) + dragStartLimit) - (size * 0.5f);
                                    }
                                    else
                                    {
                                        value = (((num2 - (rect.height * 0.5f)) / num) + dragStartLimit) - (size * 0.5f);
                                    }
                                    state.dragStartPos = num2;
                                    state.dragStartValue = value;
                                    state.dragStartSize = size;
                                    state.whereWeDrag = 0;
                                    GUI.changed=(true);
                                }
                                GUIUtility.hotControl=(id);
                                value = Mathf.Clamp(value, num7, num8 - size);
                                event2.Use();
                            }
                            return;
                        }
                        state.dragStartPos = num2;
                        state.dragStartValue = value;
                        state.dragStartSize = size;
                        state.dragStartValuesPerPixel = num;
                        state.dragStartLimit = startLimit;
                        state.dragEndLimit = endLimit;
                        if (rect2.Contains(event2.mousePosition))
                        {
                            state.whereWeDrag = 1;
                        }
                        else if (rect3.Contains(event2.mousePosition))
                        {
                            state.whereWeDrag = 2;
                        }
                        else
                        {
                            state.whereWeDrag = 0;
                        }
                        GUIUtility.hotControl=(id);
                        event2.Use();
                    }
                    return;

                case (EventType)1:
                    if (GUIUtility.hotControl == id)
                    {
                        event2.Use();
                        GUIUtility.hotControl=(0);
                    }
                    return;

                case (EventType)2:
                case (EventType)4:
                case (EventType)5:
                case (EventType)6:
                    return;

                case (EventType)3:
                    if (GUIUtility.hotControl == id)
                    {
                        float num15 = (num2 - state.dragStartPos) / state.dragStartValuesPerPixel;
                        switch (state.whereWeDrag)
                        {
                            case 0:
                                value = Mathf.Clamp(state.dragStartValue + num15, num7, num8 - size);
                                break;

                            case 1:
                                value = state.dragStartValue + num15;
                                size = state.dragStartSize - num15;
                                if (value < num7)
                                {
                                    size -= num7 - value;
                                    value = num7;
                                }
                                if (size < num9)
                                {
                                    value -= num9 - size;
                                    size = num9;
                                }
                                break;

                            case 2:
                                size = state.dragStartSize + num15;
                                if ((value + size) > num8)
                                {
                                    size = num8 - value;
                                }
                                if (size < num9)
                                {
                                    size = num9;
                                }
                                break;
                        }
                        GUI.changed=(true);
                        event2.Use();
                    }
                    return;

                case (EventType)7:
                    slider.Draw(position, GUIContent.none, id);
                    thumb.Draw(rect, GUIContent.none, id);
                    if (((GUIUtility.hotControl == id) && position.Contains(event2.mousePosition)) && ((dragStartLimit - dragEndLimit) != 0f))
                    {
                        if (!rect.Contains(event2.mousePosition))
                        {
                            if (DateTime.Now >= s_NextScrollStepTime)
                            {
                                num3 = !horiz ? event2.mousePosition.y : event2.mousePosition.x;
                                num4 = !horiz ? rect.y : rect.x;
                                if (((num3 <= num4) ? 3 : 4) != state.whereWeDrag)
                                {
                                    return;
                                }
                                if ((size != 0f) & flag)
                                {
                                    if (horiz)
                                    {
                                        if (num2 > (rect.xMax - position.x))
                                        {
                                            value += (size * num12) * 0.9f;
                                        }
                                        else
                                        {
                                            value -= (size * num12) * 0.9f;
                                        }
                                    }
                                    else if (num2 > (rect.yMax - position.y))
                                    {
                                        value += (size * num12) * 0.9f;
                                    }
                                    else
                                    {
                                        value -= (size * num12) * 0.9f;
                                    }
                                    state.whereWeDrag = -1;
                                    GUI.changed=(true);
                                }
                                value = Mathf.Clamp(value, num7, num8 - size);
                                s_NextScrollStepTime = DateTime.Now.AddMilliseconds((double) kScrollWait);
                            }
                            return;
                        }
                        if ((state != null) && ((state.whereWeDrag == 3) || (state.whereWeDrag == 4)))
                        {
                            GUIUtility.hotControl=(0);
                        }
                    }
                    return;
            }
        }
    }

    private static bool DoRepeatButton(Rect position, GUIContent content, GUIStyle style, FocusType focusType)
    {
        int num = GUIUtility.GetControlID(repeatButtonHash, focusType, position);
        EventType typeForControl = Event.current.GetTypeForControl(num);
        if (typeForControl != (EventType)1)
        {
            if (typeForControl != (EventType)1)
            {
                if (typeForControl == (EventType)7)
                {
                    style.Draw(position, content, num);
                    if (num == GUIUtility.hotControl)
                    {
                        return position.Contains(Event.current.mousePosition);
                    }
                }
                return false;
            }
        }
        else
        {
            if (position.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl=(num);
                Event.current.Use();
            }
            return false;
        }
        if (GUIUtility.hotControl == num)
        {
            GUIUtility.hotControl=(0);
            Event.current.Use();
            return position.Contains(Event.current.mousePosition);
        }
        return false;
    }

    public static void MinMaxScroller(Rect position, int id, ref float value, ref float size, float visualStart, float visualEnd, float startLimit, float endLimit, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz)
    {
        float num;
        Rect rect;
        Rect rect2;
        Rect rect3;
        if (horiz)
        {
            num = (size * 10f) / position.width;
        }
        else
        {
            num = (size * 10f) / position.height;
        }
        if (horiz)
        {
            rect = new Rect(position.x + leftButton.fixedWidth, position.y, (position.width - leftButton.fixedWidth) - rightButton.fixedWidth, position.height);
            rect2 = new Rect(position.x, position.y, leftButton.fixedWidth, position.height);
            rect3 = new Rect(position.xMax - rightButton.fixedWidth, position.y, rightButton.fixedWidth, position.height);
        }
        else
        {
            rect = new Rect(position.x, position.y + leftButton.fixedHeight, position.width, (position.height - leftButton.fixedHeight) - rightButton.fixedHeight);
            rect2 = new Rect(position.x, position.y, position.width, leftButton.fixedHeight);
            rect3 = new Rect(position.x, position.yMax - rightButton.fixedHeight, position.width, rightButton.fixedHeight);
        }
        float num2 = Mathf.Min(visualStart, value);
        float num3 = Mathf.Max(visualEnd, value + size);
        MinMaxSlider(rect, ref value, ref size, num2, num3, num2, num3, slider, thumb, horiz);
        bool flag = false;
        if (Event.current.type == (EventType)1)
        {
            flag = true;
        }
        if (ScrollerRepeatButton(id, rect2, leftButton))
        {
            value -= num * ((visualStart >= visualEnd) ? -1f : 1f);
        }
        if (ScrollerRepeatButton(id, rect3, rightButton))
        {
            value += num * ((visualStart >= visualEnd) ? -1f : 1f);
        }
        if (flag && (Event.current.type == (EventType)12))
        {
            scrollControlID = 0;
        }
        if (startLimit < endLimit)
        {
            value = Mathf.Clamp(value, startLimit, endLimit - size);
        }
        else
        {
            value = Mathf.Clamp(value, endLimit, startLimit - size);
        }
    }

    public static void MinMaxSlider(Rect position, ref float value, ref float size, float visualStart, float visualEnd, float startLimit, float endLimit, GUIStyle slider, GUIStyle thumb, bool horiz)
    {
        DoMinMaxSlider(position, GUIUtility.GetControlID(s_MinMaxSliderHash, (FocusType)2), ref value, ref size, visualStart, visualEnd, startLimit, endLimit, slider, thumb, horiz);
    }

    private static bool ScrollerRepeatButton(int scrollerID, Rect rect, GUIStyle style)
    {
        bool flag = false;
        if (DoRepeatButton(rect, GUIContent.none, style, (FocusType)2))
        {
            scrollControlID = scrollerID;
            if (scrollControlID != scrollerID)
            {
                flag = true;
                nextScrollStepTime = Time.realtimeSinceStartup + (0.001f * firstScrollWait);
                return flag;
            }
            if (Time.realtimeSinceStartup >= nextScrollStepTime)
            {
                flag = true;
                nextScrollStepTime = Time.realtimeSinceStartup + (0.001f * scrollWait);
            }
        }
        return flag;
    }

    private class MinMaxSliderState
    {
        public float dragEndLimit;
        public float dragStartLimit;
        public float dragStartPos;
        public float dragStartSize;
        public float dragStartValue;
        public float dragStartValuesPerPixel;
        public int whereWeDrag = -1;
    }
}

