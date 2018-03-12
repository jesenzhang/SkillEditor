using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateSkillActionWizard : ScriptableWizard {

	public AnimationClip action;

	public string actionName; 

	public int startFrame = 0; 

	public int fadeTime = 0;

	void OnWizardUpdate(){
		errorString = "";
		isValid = true;

		if (action != null) {
			actionName = action.name;
		}

		if (string.IsNullOrEmpty(actionName)) {
			isValid = false;
			errorString = "action is null";
			return;
		}
	}

	void OnWizardCreate(){ 
		var skillEffect = SkillManager.CreatePlayAction (Selection.activeGameObject.GetComponentInParent<Skill> (), actionName,startFrame);
		skillEffect.crossFadeTime = fadeTime;
		Selection.activeGameObject = skillEffect.gameObject;
	}
}
