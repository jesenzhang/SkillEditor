using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateAddBuffWizard : ScriptableWizard { 

	public string buffID; 

	public int startFrame = 0; 

	void OnWizardUpdate(){
		errorString = "";
		isValid = true;
		if (string.IsNullOrEmpty(buffID)) {
			isValid = false;
			errorString = "buffID is null";
			return;
		}
	}

	void OnWizardCreate(){ 
		var skillEvent = SkillManager.CreateBuffEvent (Selection.activeGameObject.GetComponentInParent<Skill> (),buffID,startFrame);
		Selection.activeGameObject = skillEvent.gameObject;
	}
}
