using System;
using UnityEngine;

public class CinemaDirectorDragArgs : EventArgs
{
    public Behaviour cutscene;
    public UnityEngine.Object[] references;

    public CinemaDirectorDragArgs(Behaviour cutscene, UnityEngine.Object[] references)
    {
        this.cutscene = cutscene;
        this.references = references;
    }
}

