using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraShakeAction  : SkillAction { 

	public float freq;
	public float intensity;

	public override string displayName{
		get { 
			return "CameraShakeAction-" + duration;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.CAMERA_SHAKE;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[2]; 
			args [0].FloatValue = freq; 
			args [1].FloatValue = intensity; 
			return args;
		}
	}
}
