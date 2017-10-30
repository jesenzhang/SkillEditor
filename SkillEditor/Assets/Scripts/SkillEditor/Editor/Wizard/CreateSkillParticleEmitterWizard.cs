using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateSkillParticleEmitterWizard : ScriptableWizard {

	public Example.SkillParticleEmitter.EmitterType emitterType = Example.SkillParticleEmitter.EmitterType.RANDOM;

	public GameObject effectPrefab;  

	public Example.SkillShapeNew.ShapeType emitterShape = Example.SkillShapeNew.ShapeType.CIRCLE;

	public Example.SkillPath.PathType pathType = Example.SkillPath.PathType.NONE;

	public Example.SkillShapeNew.ShapeType particleHitShape = Example.SkillShapeNew.ShapeType.CIRCLE;

	public int startFrame = 300;

	public int duration = 500;

	public int count = 1; 

	void OnWizardUpdate(){
		errorString = "";
		isValid = true; 
	}

	void OnWizardCreate(){ 
		var particleEmitter = SkillManager.CreateSkillParticleEmitter (Selection.activeGameObject.GetComponent<Skill> (),emitterType, emitterShape,count); 
		particleEmitter.effectPrefab = effectPrefab;
		particleEmitter.particlePathType = pathType; 
		particleEmitter.particleHitShapeType = particleHitShape;
		particleEmitter.particleStartFrame = startFrame;
		particleEmitter.particleDuration = duration;

		Selection.activeGameObject = particleEmitter.gameObject;
	}
}

