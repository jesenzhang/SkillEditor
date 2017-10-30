using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateSkillEffectWizard : ScriptableWizard {

	public GameObject effectPrefab; 

	public int startFrame = 0;

	public int duration = -1;

	void OnWizardUpdate(){
		errorString = "";
		isValid = true;
		if (effectPrefab==null) {
			isValid = false;
			errorString = "effect is null";
			return;
		}
	}

	void OnWizardCreate(){ 
		var skillEffect = SkillManager.CreateSkillEffect (Selection.activeGameObject.GetComponent<Skill> (), effectPrefab,startFrame, duration);
		Selection.activeGameObject = skillEffect.gameObject;
	}
}
