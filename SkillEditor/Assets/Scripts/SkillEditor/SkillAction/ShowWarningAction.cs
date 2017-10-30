using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

[ExecuteInEditMode]
public class ShowWarningAction : PlayEffectAction {

	public override string displayName{
		get { 
			return "ShowWarning-" + effectName;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[2];
			args [0].IntValue = 0; 
			args [1].StrValue = effectName; 
			return args;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.SHOW_WARNING;
		}
	}
}
