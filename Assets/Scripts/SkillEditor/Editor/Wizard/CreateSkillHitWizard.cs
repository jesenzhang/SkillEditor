using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateSkillHitWizard : ScriptableWizard { 

	public string[] buffIDs;  

	public GameObject hitEffect; 

	public int hitEffectTime;

	void OnWizardUpdate(){
		errorString = "";
		isValid = true; 
	}

	void OnWizardCreate(){ 
		var skillEvent = SkillManager.CreateSkillHit (Selection.activeGameObject.GetComponentInParent<SkillParticle> (),buffIDs);
		skillEvent.hitEffect = hitEffect;
		skillEvent.hitEffectTime = hitEffectTime;
		Selection.activeGameObject = skillEvent.gameObject;
	}
}
