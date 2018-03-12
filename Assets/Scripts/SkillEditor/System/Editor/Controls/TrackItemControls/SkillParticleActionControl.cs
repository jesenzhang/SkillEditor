

using UnityEngine;
using System.Collections;
using UnityEditor;
using TimeLine;

[CutsceneItemControlAttribute(typeof(SkillParticleActionItem))]
public class SkillParticleActionControl : CinemaActorActionControl
{
    SkillParticleEmitterItem parentEmitter;
    public SkillParticleActionControl()
        : base()
    {
    }

    public SkillParticleEmitterItem ParentEmitter
    {
        get
        {

            if (parentEmitter == null)
            {
                SkillParticleActionItem action = Wrapper.Behaviour as SkillParticleActionItem;
                parentEmitter = action.gameObject.transform.parent.GetComponent<SkillParticleEmitterItem>();
            }
            return parentEmitter;
        }

        set
        {
            parentEmitter = value;
        }
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
        float num = (wrapper.Firetime * state.Scale.x) + state.Translation.x;
        float num2 = ((wrapper.Firetime + wrapper.Duration) * state.Scale.x) + state.Translation.x;
        this.controlPosition = new Rect(num, 0f, num2 - num, trackPosition.height);
        SkillParticleActionItem action = Wrapper.Behaviour as SkillParticleActionItem;
        if (action == null) return;
        if (ParentEmitter != null)
        {
            int level =ParentEmitter.GetLevel();
            controlPosition = new Rect(controlPosition.x, trackPosition.height- trackPosition.height / level, controlPosition.width, trackPosition.height / level);
        }

    }

    public override void Draw(DirectorControlState state)
    {
        SkillParticleActionItem action = Wrapper.Behaviour as SkillParticleActionItem;
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
