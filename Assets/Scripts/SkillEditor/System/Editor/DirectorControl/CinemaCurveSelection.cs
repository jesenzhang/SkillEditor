using System;

public class CinemaCurveSelection
{
    public int CurveId = -1;
    public int KeyId = -1;
    public string Property = string.Empty;
    public string Type = string.Empty;

    internal void Reset()
    {
        this.Type = string.Empty;
        this.Property = string.Empty;
        this.CurveId = -1;
        this.KeyId = -1;
    }
}

