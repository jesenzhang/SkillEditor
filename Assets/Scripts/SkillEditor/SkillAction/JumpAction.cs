using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class JumpAction  : SkillAction { 

	public string endAction;
	public int endTime;

	public float distance;
	public float height;

	public override string displayName{
		get { 
			return "JumpAction-" + distance;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.JUMP;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[6];  
			args [0].StrValue = endAction;
			args [1].IntValue = endTime;
			args [2].FloatValue = distance;
			args [3].FloatValue = height; 
			return args;
		}
	}
}
