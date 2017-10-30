using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class PlayEffectAction : SkillAction {   

	public string effectName;  

	public GameObject effect; 

	public Transform effectBone;

	 protected override void OnStart (SkillCaller caller, Skill skill, int time)
	{
		//Debug.LogFormat ("{0} SkillEffect.OnStart",SkillSimulator.playingTime);
		if (effectBone != null) {
			effect.transform.parent = effectBone;
		}
	}

	protected override void OnStop (SkillCaller caller, Skill skill, int time)
	{
		//Debug.LogFormat ("{0} SkillEffect.OnStop",SkillSimulator.playingTime);
		effect.transform.parent = transform;

	}

	public override string displayName{
		get { 
			return "PlayEffect-" + effectName;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[3];
			args [0].StrValue = effectName;  
			args [1].StrValue = effectBone != null ? effectBone.name : "";
			args [2].Vector3Value = effect!=null?effect.transform.localPosition:Vector3.zero;
			return args;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.PLAY_EFFECT;
		}
	}
	 
}
