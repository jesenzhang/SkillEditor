using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SummonedAction : SkillAction {

	public string npcID;

	public int summonedCount = 1;

	public override string displayName{
		get { 
			return "Summoned-" + duration;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.SUMMONED;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[2]; 
			args [0].StrValue = npcID;
			args [1].IntValue = summonedCount;
			return args;
		}
	}
}
