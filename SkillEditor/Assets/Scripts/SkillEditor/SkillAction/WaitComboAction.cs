using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaitComboAction : SkillAction {

	public string comboSkillID;

	public override string displayName{
		get { 
			return "WaitCombo-" + duration;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.WAIT_COMBO;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[1]; 
			args [0].StrValue = comboSkillID;
			return args;
		}
	}
}
