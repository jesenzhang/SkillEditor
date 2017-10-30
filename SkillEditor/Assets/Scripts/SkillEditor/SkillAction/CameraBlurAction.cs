using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraBlurAction  : SkillAction {  
	public float intensity;

	public override string displayName{
		get { 
			return "CameraBlurAction-" + intensity;
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.CAMERA_BLUR;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[1];  
			args [0].FloatValue = intensity; 
			return args;
		}
	}
}
