using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateSkillLimitWizard : ScriptableWizard {

	public PlayModelAction syncWithAction;

	public bool moveLimit = true;

	public bool attackLimit = true;

	public int startFrame = 0; 

	public int duration = 100;

	void OnWizardUpdate(){
		errorString = "";
		isValid = true;
		if (syncWithAction != null && syncWithAction.clipInfo!=null) {
			startFrame = syncWithAction.startFrame;
			duration = Mathf.RoundToInt(syncWithAction.clipInfo.length * 1000);
		}
	}

	void OnWizardCreate(){ 
		if (moveLimit) {
			var skillEvent = SkillManager.CreateSkillAction<SkillLimitAction> (Selection.activeGameObject.GetComponentInParent<Skill> (), startFrame,duration);
			skillEvent.limitType = SkillLimitType.MOVE;
			Selection.activeObject = skillEvent.gameObject;
		}

		if (attackLimit) {
			var skillEvent = SkillManager.CreateSkillAction<SkillLimitAction> (Selection.activeGameObject.GetComponentInParent<Skill> (), startFrame,duration);
			Selection.activeObject = skillEvent.gameObject;
			skillEvent.limitType = SkillLimitType.CANCEL;
		}
		 

	}
}
