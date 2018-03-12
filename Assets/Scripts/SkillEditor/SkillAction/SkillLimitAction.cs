using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillLimitType{
	MOVE,
	CANCEL,
}

[ExecuteInEditMode]
public class SkillLimitAction : SkillAction {
	public PlayModelAction syncAction;
	public SkillLimitType limitType = SkillLimitType.MOVE; 
	 

	protected override void OnStart (SkillCaller caller, Skill skill, int time)
	{
		base.OnStart (caller, skill, time);
		skill.AddLimit (this);
	}

	protected override void OnClean (SkillCaller caller, Skill skill)
	{
		base.OnClean (caller, skill);
		skill.RemoveLimit (this);
	}

	protected override void OnEditorUpdate ()
	{
		if (syncAction != null && syncAction.clipInfo!=null) {
			startFrame = syncAction.startFrame;
			duration = Mathf.RoundToInt(syncAction.clipInfo.length * 1000);
		}
	}
	public override string displayName {
		get { 
			return "Limit-" + limitType + (syncAction!=null?"-"+syncAction.actionName:"");
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			if (limitType == SkillLimitType.MOVE) {
				return Example.SkillAction.ActionType.MOVE_LIMIT;
			}
			if (limitType == SkillLimitType.CANCEL) {
				return Example.SkillAction.ActionType.CANCEL_LIMIT;
			}
			return Example.SkillAction.ActionType.UNKNOWN;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[0]; 
			return args;
		}
	}
}
