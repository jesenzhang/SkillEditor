using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraBlackAction  : SkillAction {  
	public float intensity;

	public override string displayName{
		get { 
			return "CameraBlackAction-" + duration;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.CAMERA_BLACK;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[2];  
			args [0].FloatValue = intensity; 
			return args;
		}
	}
}
