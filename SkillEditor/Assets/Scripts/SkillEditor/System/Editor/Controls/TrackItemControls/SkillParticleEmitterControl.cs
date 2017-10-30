
using UnityEngine;
using System.Collections;
using UnityEditor;
using TimeLine;

[CutsceneItemControlAttribute(typeof(SkillParticleEmitterItem))]
public class SkillParticleEmitterControl : CinemaActorActionControl
{
    public SkillParticleEmitterControl()
        : base()
    {

    }
 


    public override void ResizeControlPosition(DirectorControlState state, Rect trackPosition)
    {
        CinemaActionWrapper wrapper = base.Wrapper as CinemaActionWrapper;
        if (wrapper == null)
        {
            return;
        }
        if (base.isRenaming)
        {
            return;
        }
        SkillParticleEmitterItem action = Wrapper.Behaviour as SkillParticleEmitterItem;
        float num = (wrapper.Firetime * state.Scale.x) + state.Translation.x;
        float num2 = ((wrapper.Firetime + wrapper.Duration) * state.Scale.x) + state.Translation.x;
        this.controlPosition = new Rect(num, 0f, num2 - num, trackPosition.height/ action.GetLevel());
    }
    public override void Draw(DirectorControlState state)
    {
        SkillParticleEmitterItem action = Wrapper.Behaviour as SkillParticleEmitterItem;
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
