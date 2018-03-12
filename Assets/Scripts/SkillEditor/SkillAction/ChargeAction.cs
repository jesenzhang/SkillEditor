using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChargeAction   : SkillAction { 
	 
	public string endAction;

	public int endTime;

	public float distance; 

	protected override void OnStart (SkillCaller caller, Skill skill, int time)
	{ 
		caller.PlayAction(endAction,0);
	}

	protected override void OnClean (SkillCaller caller, Skill skill)
	{
		base.OnClean (caller, skill);
		caller.PlayAction ("Stand",300);
	}

	public override string displayName{
		get { 
			return "ChargeAction-" + distance;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.CHARGE;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[4];  
			args [0].StrValue = endAction;
			args [1].IntValue = endTime;
			args [2].FloatValue = distance;  
			return args;
		}
	}
}
