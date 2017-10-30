using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MakeSkillAction : SkillAction { 

	public string skillID;  

	protected override void OnStart (SkillCaller caller, Skill skill, int time)
	{

	}

	public override string displayName{
		get { 
			return "MakeSkill-" + skillID;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.MAKE_SKILL;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[1];
			args [0].StrValue = skillID;
			return args;
		}
	}

}
