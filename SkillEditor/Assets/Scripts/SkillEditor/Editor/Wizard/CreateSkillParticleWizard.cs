using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateSkillParticleWizard : ScriptableWizard {

	public GameObject effectPrefab; 

	public GameObject hitEffectPrefab; 

	public Example.SkillShapeNew.ShapeType shapeType;

	public Example.SkillPath.PathType pathType;

	public int startFrame = 300;

	public int duration = 500;

	void OnWizardUpdate(){
		errorString = "";
		isValid = true; 
	}

	void OnWizardCreate(){ 
		var skillParticle = SkillManager.CreateSkillParticle (Selection.activeGameObject.GetComponent<Skill> (), effectPrefab,startFrame, duration,pathType,shapeType);
		skillParticle.hitEffect = hitEffectPrefab;
		Selection.activeGameObject = skillParticle.gameObject;
	}
}
