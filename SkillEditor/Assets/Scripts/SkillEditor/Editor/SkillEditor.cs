using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SkillEditor  {
	 
	[MenuItem("SkillEditor/导入技能数据",false,1)]
	static void ImportSkills(){
		string path = EditorUtility.OpenFilePanel ("Select SkillUnit data", "E://dev//MmoCard//doc//VersionResource//2017.9.15/Data", "bytes");
		if (!string.IsNullOrEmpty (path)) {
			SkillImportHelper.ImportLegacySkillUnits (path);
		}

	} 

	[MenuItem("SkillEditor/Hidden/模型/创建技能",false,1001)]
	[MenuItem("SkillEditor/创建技能",false,100)]
	static void CreateSkill(){
		ScriptableWizard.DisplayWizard<CreateSkillWizard> ("创建技能","Create");
	} 

	[MenuItem("SkillEditor/Hidden/技能/导出技能",false,1002)]
	[MenuItem("SkillEditor/导出技能",false,110)]
	static void ExportSkill(){
		foreach (var go in Selection.gameObjects) {
			var skill = go.GetComponent<Skill> ();
			if (skill != null) {
				SkillExportHelper.ExportSkill (skill);
			} else {
				//Debug.Log ("未选中技能");
			}
		}
	}

    [MenuItem("SkillEditor/Hidden/技能/导出时间线技能", false, 1002)]
    [MenuItem("SkillEditor/导出时间线技能", false, 110)]
    static void ExportTimelineSkill()
    {
        foreach (var go in Selection.gameObjects)
        {
            var skill = go.GetComponent<TimeLine.TimelineManager>();
            if (skill != null)
            {
                SkillExportHelper.ExportTimelineSkill(skill);
            }
            else
            {
                //Debug.Log ("未选中技能");
            }
        }
    }
    [MenuItem("SkillEditor/Hidden/技能/添加技能特效",false,1101)]
	[MenuItem("SkillEditor/添加技能特效",false,201)]
	static void CreateSkillEffect(){
		var skill = Selection.activeGameObject.GetComponent<Skill> ();
		if (skill != null) {
			ScriptableWizard.DisplayWizard<CreateSkillEffectWizard> ("添加技能特效", "Create"); 
		} else {
			Debug.Log ("未选中技能");
		}
	}	 

	[MenuItem("SkillEditor/Hidden/技能/创建技能粒子",false,2001)]
	[MenuItem("SkillEditor/创建技能粒子",false,202)]
	static void CreateSkillParticle(){
		var skill = Selection.activeGameObject.GetComponent<Skill> ();
		if (skill != null) {
			ScriptableWizard.DisplayWizard<CreateSkillParticleWizard> ("创建技能粒子", "Create"); 
		} else {
			Debug.Log ("未选中技能");
		}
	}

	[MenuItem("SkillEditor/Hidden/技能/创建技能粒子发射器",false,2001)]
	[MenuItem("SkillEditor/创建技能粒子发射器",false,202)]
	static void CreateSkillParticleEmitter(){
		var skill = Selection.activeGameObject.GetComponent<Skill> ();
		if (skill != null) {
			ScriptableWizard.DisplayWizard<CreateSkillParticleEmitterWizard> ("创建技能粒子发射器", "Create"); 
		} else {
			Debug.Log ("未选中技能");
		}
	}

	[MenuItem("SkillEditor/Hidden/技能/播放动作",false,3000)]
	[MenuItem("SkillEditor/事件/播放动作",false,300)]
	static void PlaySkillAction(){
		var skill = Selection.activeGameObject.GetComponentInParent<Skill> ();
		if (skill != null) {
			ScriptableWizard.DisplayWizard<CreateSkillActionWizard> ("播放动作", "Create"); 
		} else {
			Debug.Log ("未选中技能");
		}
	}

	[MenuItem("SkillEditor/Hidden/技能粒子/添加打击点",false,3001)] 
	[MenuItem("SkillEditor/技能粒子/添加打击点",false,301)]
	static void AddSkillHit(){
		var skillParticle = Selection.activeGameObject.GetComponentInParent<SkillParticle> ();
		if (skillParticle != null) {
			ScriptableWizard.DisplayWizard<CreateSkillHitWizard> ("添加打击点", "Create"); 
		} else {
			Debug.Log ("未选中技能粒子");
		}
	}


	[MenuItem("SkillEditor/Hidden/技能/添加BUFF",false,3002)]
	[MenuItem("SkillEditor/事件/添加BUFF",false,301)]
	static void AddSkillBuff(){
		var skill = Selection.activeGameObject.GetComponentInParent<Skill> ();
		if (skill != null) {
			ScriptableWizard.DisplayWizard<CreateAddBuffWizard> ("添加BUF", "Create"); 
		} else {
			Debug.Log ("未选中技能");
		}
	}

	[MenuItem("SkillEditor/Hidden/技能/添加限制",false,3003)]
	[MenuItem("SkillEditor/事件/添加限制",false,303)]
	static void AddSkillLimit(){
		var skill = Selection.activeGameObject.GetComponentInParent<Skill> ();
		if (skill != null) {
			ScriptableWizard.DisplayWizard<CreateSkillLimitWizard> ("添加限制", "Create"); 
		} else {
			Debug.Log ("未选中技能");
		}
	}

	[InitializeOnLoadMethod]
	static void OnInitEditor(){
		EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
	}

	static void OnHierarchyGUI(int instanceID,Rect selectionRect){
		if (Event.current != null && selectionRect.Contains (Event.current.mousePosition) && Event.current.button == 1 && Event.current.type <= EventType.MouseUp) {
			GameObject selectedGameObject = EditorUtility.InstanceIDToObject (instanceID) as GameObject;
			if (selectedGameObject != null) {
				var mousePosition = Event.current.mousePosition;
				if (selectedGameObject.GetComponent<SkillCaller> () != null) {
					EditorUtility.DisplayPopupMenu (new Rect (mousePosition.x, mousePosition.y, 0, 0), "SkillEditor/Hidden/模型", null);
					Event.current.Use ();
				}else if (selectedGameObject.GetComponent<Skill> () != null) {
					EditorUtility.DisplayPopupMenu (new Rect (mousePosition.x, mousePosition.y, 0, 0), "SkillEditor/Hidden/技能", null);
					Event.current.Use ();
				}else if (selectedGameObject.GetComponent<SkillParticle> () != null) {
					EditorUtility.DisplayPopupMenu (new Rect (mousePosition.x, mousePosition.y, 0, 0), "SkillEditor/Hidden/技能粒子", null);
					Event.current.Use ();
				}else if (selectedGameObject.name == "Events") {
					EditorUtility.DisplayPopupMenu (new Rect (mousePosition.x, mousePosition.y, 0, 0), "SkillEditor/事件", null);
					Event.current.Use ();
				}
			}
		}
	}

}
