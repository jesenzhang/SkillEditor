using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlaySoundAction : SkillAction {

	public string audioName;

	public int fade;

	public override string displayName{
		get { 
			return "PlaySound-" + audioName;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.PLAY_SOUND;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[2]; 
			args [0].StrValue = audioName;
			args [1].IntValue = fade;
			return args;
		}
	}
}
