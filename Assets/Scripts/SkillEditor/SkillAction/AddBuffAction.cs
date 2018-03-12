using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AddBuffAction : SkillAction { 
	
	public string buffID;  

	protected override void OnStart (SkillCaller caller, Skill skill, int time)
	{
		
	}

	public override string displayName{
		get { 
			return "AddBuff-" + buffID;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.ADD_BUFF;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[1];
			args [0].StrValue = buffID;
			return args;
		}
	}

}
