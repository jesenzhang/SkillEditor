using System;

public class CurveClipWrapperEventArgs : EventArgs
{
    public CinemaClipCurveWrapper wrapper;

    public CurveClipWrapperEventArgs(CinemaClipCurveWrapper wrapper)
    {
        this.wrapper = wrapper;
    }
}

