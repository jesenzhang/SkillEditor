using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ActionState{
	IDLE,
	STARTED,
	STOPED,
	CANCELED,
}

[ExecuteInEditMode]
public abstract class SkillAction : SkillTimeTween {	 

	protected override void OnEditorUpdate(){ 
		base.OnEditorUpdate ();
		this.transform.localPosition = Vector3.zero;
		this.transform.localRotation = Quaternion.identity;
		this.transform.localScale = Vector3.one;
	}	 

	public abstract Example.SkillAction.ActionType actionType {
		get;
	}

	public abstract ContentValue[] arguments {
		get;
	}
}
