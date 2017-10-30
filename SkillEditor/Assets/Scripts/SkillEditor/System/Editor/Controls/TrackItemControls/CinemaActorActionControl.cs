using UnityEngine;
using System.Collections;
using UnityEditor;
using TimeLine;

[CutsceneItemControlAttribute(typeof(ActorAction))]
public class CinemaActorActionControl : CinemaActionControl
{
    public CinemaActorActionControl()
        : base()
    {
    }

    public override void Draw(DirectorControlState state)
    {
        ActorAction action = Wrapper.Behaviour as ActorAction;
        if (action == null) return;

        if (IsSelected)
        {
            GUI.Box(controlPosition, GUIContent.none, TimelineTrackControl.styles.ActorTrackItemSelectedStyle);
        }
        else
        {
            GUI.Box(controlPosition, GUIContent.none, TimelineTrackControl.styles.ActorTrackItemStyle);
        }
        
        DrawRenameLabel(action.name, controlPosition);
    }
}
