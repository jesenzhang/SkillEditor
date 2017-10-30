using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateSkillWizard : ScriptableWizard {
	public Animator model;

	public string skillID;

	public Example.Skill.SkillType skillType = Example.Skill.SkillType.GENERAL;

	public Example.Skill.TargetType targetType = Example.Skill.TargetType.EMEMY;

	public int castingDistance = 5;

	void OnWizardUpdate(){
		errorString = "";
		isValid = true;
		if (model==null) {
			isValid = false;
			errorString = "model is null";
			return;
		}
		if (string.IsNullOrEmpty (skillID)) {
			isValid = false;
			errorString = "skillID is empty";
			return;
		} 
	}

	void OnWizardCreate(){
		var skillCaller = SkillManager.GetSkillCaller (model);
		var skill = SkillManager.CreateSkill (skillCaller,skillID,skillType,targetType);
		skill.castingDistance = castingDistance;
		Selection.activeGameObject = skill.gameObject;
	}
}
