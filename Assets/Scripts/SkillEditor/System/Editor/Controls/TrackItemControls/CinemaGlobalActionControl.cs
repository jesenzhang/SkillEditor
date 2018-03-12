using UnityEngine;
using System.Collections;
using UnityEditor;
using TimeLine;

[CutsceneItemControlAttribute(typeof(GlobalAction))]
public class CinemaGlobalActionControl : CinemaActionControl
{
    public CinemaGlobalActionControl() : base()
    {
    }

    public override void Draw(DirectorControlState state)
    {
        GlobalAction action = Wrapper.Behaviour as GlobalAction;
        if (action == null) return;

        if (IsSelected)
        {
            GUI.Box(controlPosition, GUIContent.none, TimelineTrackControl.styles.GlobalTrackItemSelectedStyle);
        }
        else
        {
            GUI.Box(controlPosition, GUIContent.none, TimelineTrackControl.styles.GlobalTrackItemStyle);
        }
        
        DrawRenameLabel(action.name, controlPosition);
    }
}
