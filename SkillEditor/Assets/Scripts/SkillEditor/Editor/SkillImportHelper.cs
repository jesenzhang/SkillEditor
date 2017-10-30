using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Example;
public class SkillImportHelper {

	private static Example.AllStringID strIDS = null;

	private static Example.AllEffectConfig effectConfigs = null;

	public static void ImportStringIDS(string path){
		var data = File.ReadAllBytes (path);
		if (data.Length == 0)
			return;
		strIDS = Example.AllStringID.Deserialize (data);		 
	}

	public static void ImportEffectConfigs(string path){
		var data = File.ReadAllBytes (path);
		if (data.Length == 0)
			return;
		effectConfigs = Example.AllEffectConfig.Deserialize (data);		 
	}
	
	public static void ImportLegacySkillUnits(string path){

		if (strIDS == null) { 

			ImportStringIDS(Path.GetDirectoryName (path)+"/StringID.bytes");

			if (strIDS == null) {
				string strPath = EditorUtility.OpenFilePanel ("select StringID.byte",Path.GetDirectoryName (path), "bytes");
				if (!string.IsNullOrEmpty (strPath)) {
					ImportStringIDS (strPath);
				} else {
					Debug.LogError ("StringIDS.bytes not selected");
					return;
				}
			}
		}

		if (effectConfigs == null) {
			ImportEffectConfigs(Path.GetDirectoryName (path)+"/EffectConfig.bytes");
		}

		var data = File.ReadAllBytes (path);
		var skillData = Example.AllSkillUnits.Deserialize (data);
		 
		foreach (var skillUnit in  skillData.Units) {
			var skill = ImportSkillUnit (skillData,skillUnit);
			if (skill != null) {
				SkillExportHelper.skillIDS[skill.skillID] = skillUnit.Id;
			}
		}

		var skillIDList = new List<Example.SkillID>(); 
		foreach (var strID in SkillExportHelper.skillIDS) {
			var skillID = new Example.SkillID ();
			skillID.Id = strID.Value;
			skillID.StrID = strID.Key;
			skillIDList.Add (skillID);
		}

		var skillIDS = new Example.SkillIDS ();
		skillIDS.Skills = skillIDList;
		data = Example.SkillIDS.SerializeToBytes (skillIDS);
		File.WriteAllBytes(Path.GetDirectoryName (path)+"/SkillIDS.bytes",data);
	}
	 

	static Skill ImportSkillUnit(Example.AllSkillUnits skillData,Example.SkillUnit skillUnit){
		bool result = false;

		var skillArt = skillData.Arts [skillUnit.ArtId];
		var skillID = FindStringID (skillData,skillUnit); 

		var targetTypes = new Example.Skill.TargetType[]{skillUnit.targetType==Example.SkillUnit.TargetType.ENEMY?Example.Skill.TargetType.EMEMY:Example.Skill.TargetType.FRIEND};
		 
		var skill = SkillManager.CreateSkill (Selection.activeGameObject.GetComponent<SkillCaller>(), skillID, Example.Skill.SkillType.GENERAL, targetTypes); 

		skill.skillCD = skillUnit.Cd;
		skill.castingDistance = (int)skillUnit.Distance;  

		if (!string.IsNullOrEmpty (skillArt.GuideAction) && skillArt.GuideAction!="-1") {
			var action = SkillManager.CreatePlayAction (skill, skillArt.GuideAction, 0);
			action.duration = skillUnit.GuidePolicy.GuideTime;
		}

		if (!string.IsNullOrEmpty (skillArt.GuidingAction) && skillArt.GuidingAction!="-1") {
			var action = SkillManager.CreatePlayAction (skill, skillArt.GuidingAction,  skillUnit.GuidePolicy.GuideTime);
			action.duration = skillUnit.GuidePolicy.GuidingTime;
		}

		if (!string.IsNullOrEmpty (skillArt.EndAction) && skillArt.EndAction!="-1" && skillUnit.launchType!= Example.SkillUnit.LaunchType.JUMP) {
			var action = SkillManager.CreatePlayAction (skill, skillArt.EndAction,  skillUnit.GuidePolicy.GuideTime + skillUnit.GuidePolicy.GuidingTime);
			action.duration = skillUnit.GuidePolicy.EndTime;
		}

		foreach (var beginEffect in skillArt.BeginEffect) {
			if (beginEffect.Effect >= 0) {
				var effectName = strIDS.Ids [beginEffect.Effect].Id;
				var effectPrefab = FindEffectPrefab (effectName);
				PlayEffectAction action = null;
				if (effectPrefab != null) {
					action = SkillManager.CreateSkillEffect (skill, effectPrefab,0, beginEffect.PhaseTime);
					result = true;
				} else {
					action = SkillManager.CreateSkillEffect (skill, effectName,0, beginEffect.PhaseTime); 
				}
				string boneName = "";
				var bone = FindEffectBone (GameObject.FindObjectOfType<SkillCaller> (), effectName, out boneName);
				action.effectBone = bone;
			}
		}

		if (skillUnit.CombPolicy.CombSkill >= 0) {
			var combAction = SkillManager.CreateSkillAction<WaitComboAction> (skill,skillUnit.Cd,skillUnit.CombPolicy.MaxCombTime-skillUnit.Cd);
			combAction.comboSkillID = FindStringID (skillData, skillUnit.CombPolicy.CombSkill);
		}

		if (skillUnit.CombPolicy.AutoNextSkill >= 0) {

			var skillEvent = SkillManager.CreateSkillEvent<SkillOverEvent> (skill,Example.SkillEvent.EventType.SKILL_OVER);			
			var combAction = SkillManager.CreateSkillAction<MakeSkillAction> (skillEvent,0,0);
			combAction.skillID = FindStringID (skillData, skillUnit.CombPolicy.AutoNextSkill);
		}

		//if (!string.IsNullOrEmpty(skillArt.SkillAudio)) {
		//	var action = SkillManager.CreateSkillAction<PlaySoundAction> (skill, 0, 0);
		//	action.audioName = skillArt.SkillAudio;
		//}

		SkillParticle skillParticle = SkillManager.CreateSkillParticle (skill, null,skillUnit.GuidePolicy.GuideTime + skillUnit.GuidePolicy.GuidingTime,Mathf.Max(skillUnit.GuidePolicy.EndTime,skillArt.UnitEffect.PhaseTime),Example.SkillPath.PathType.NONE,Example.SkillShapeNew.ShapeType.CIRCLE);

		if (skillArt.UnitEffect.Effect >= 0) {
			var effectName = strIDS.Ids [skillArt.UnitEffect.Effect].Id;
			var effectPrefab = FindEffectPrefab (effectName);
			skillParticle.effectName = effectName;
			var effectConfig = FindEffectConfig (effectName);
			if (effectPrefab != null) {
				skillParticle.effect = SkillManager.InstantiateGameObject (skillParticle.transform, effectPrefab, "Effect");
				if (effectConfig != null) {
					skillParticle.effect.transform.localPosition = MathUtil.Vector3fToVector3(effectConfig.Position);
				}
			} else {
				result = false;
			}


		} 

		if (result) {
			var hitAction = SkillManager.CreateSkillAction<SkillHitAction> (skillParticle, skillUnit.GuidePolicy.GuideTime + skillUnit.GuidePolicy.GuidingTime, skillUnit.GuidePolicy.EndTime);

			switch (skillUnit.launchType) {
			case Example.SkillUnit.LaunchType.SINGLELINE:
				skill = ImportLineSkill (skill, skillUnit, skillData.SingeLines [skillUnit.ReferId], skillArt, skillParticle, hitAction);
				break;
			case Example.SkillUnit.LaunchType.MULLINE:
				skill = ImportMultiLineSkill (skill, skillUnit, skillData.MultLines [skillUnit.ReferId], skillArt, skillParticle, hitAction);
				break;
			case Example.SkillUnit.LaunchType.AREA:
				skill = ImportAreaSkill (skill, skillUnit, skillData.Areas [skillUnit.ReferId], skillArt, skillParticle, hitAction);
				break;
			case Example.SkillUnit.LaunchType.JUMP:
				skill = ImportJumpSkill (skill, skillUnit, skillData.Jumps [skillUnit.ReferId], skillArt, skillParticle, hitAction);
				break;
			case Example.SkillUnit.LaunchType.FOLLOW:
				skill = ImportFollowSkill (skill, skillUnit, skillData.Follows [skillUnit.ReferId], skillArt, skillParticle, hitAction);
				break;
			} 

			if (skillParticle.duration == 0) {
				Debug.LogErrorFormat ("{0} skillParticle duration is zero", skill.skillID);
			}
		} else {
			GameObject.DestroyImmediate (skill.gameObject);
		}
		 



		return result?skill:null;
	}

	static string FindStringID(Example.AllSkillUnits skillData,Example.SkillUnit skillUnit){
		return FindStringID (skillData,skillUnit.Id);
	}

	static string FindStringID(Example.AllSkillUnits skillData,int skillID){
		for (int i = 0; i < skillData.StringID.Ids.Count; ++i) {
			if (skillID == i) {
				return skillData.StringID.Ids [i].Id;
			}
		}
		return null;
	}

	static Example.EffectConfig FindEffectConfig(string effectName){ 
		foreach (var effectConfig in effectConfigs.Effects) {
			if (effectConfig.EffectName == effectName) {
				return effectConfig;
			}
		} 
		return null;
	}

	static Transform FindEffectBone(SkillCaller caller, string effectName,out string boneName){ 
		var effectConfig = FindEffectConfig (effectName);
		if (effectConfig != null && effectConfig.posType == Example.EffectConfig.PosType.BONE) {
			boneName = effectConfig.BoneName;
		} else {
			boneName = "";
		}
		if (!string.IsNullOrEmpty (boneName)) {
			return FindChildTransform(caller.animator.transform,boneName);
		}
		return null;
	}

	static Transform FindChildTransform(Transform trans,string childName){
		Transform result = null;
		for (int i = 0; i < trans.childCount; ++i) {
			var child = trans.GetChild (i);
			if (child.name == childName) {
				result = child;
				break;
			} else {
				result = FindChildTransform (child,childName);
				if (result != null)
					break;
			}
		}
		return result;
	}

	static GameObject FindEffectPrefab(string effectName){
		var uids = AssetDatabase.FindAssets (effectName);
		if (uids.Length > 0) {
			var path = AssetDatabase.GUIDToAssetPath (uids[0]);
			return AssetDatabase.LoadAssetAtPath<GameObject> (path);
		}
		return null;
	}


	static SkillShape ImportSkillShape(MonoBehaviour who, Example.SkillShape shape){
		SkillShape skillShape = null;
		switch (shape.area) {
		case Example.SkillShape.Area.CIRCLE:		
			var circle = who.gameObject.AddComponent<CircleSkillShape> ();	
			circle.shapeType = SkillShapeNew.ShapeType.CIRCLE;
			circle.radius = shape.Param1;
			skillShape = circle;
			break;
		case Example.SkillShape.Area.QUADRATE:
			var box = who.gameObject.AddComponent<BoxSkillShape> ();
			box.shapeType = SkillShapeNew.ShapeType.BOX;
			box.width = shape.Param1;
			box.height = shape.Param2;
			skillShape = box;
			break;
		case Example.SkillShape.Area.SECTOR:
			var sector = who.gameObject.AddComponent<SectorSkillShape> ();
			sector.shapeType = SkillShapeNew.ShapeType.SECTOR;
			sector.radius = shape.Param1;
			sector.angle = shape.Param3 * 180/ Mathf.PI;
			skillShape = sector;
			break;
		case Example.SkillShape.Area.TRIANGLE:
			var triangle = who.gameObject.AddComponent<TriangleSkillShape> ();
			triangle.shapeType = SkillShapeNew.ShapeType.TRIANGLE;
			triangle.width = shape.Param1;
			triangle.height = shape.Param2;
			skillShape = triangle;
			break;
		}
		return skillShape;
	}

	static Skill ImportAreaSkill(Skill skill,Example.SkillUnit skillUnit,Example.SkillArea skillInfo,Example.SkillArt skillArt,SkillParticle skillParticle,SkillHitAction hitAction){ 
		 
		Object.DestroyImmediate (skillParticle.path); 
		Object.DestroyImmediate (skillParticle.hitShape); 

		var fixedPath = SkillManager.CreateSkillPath(skillParticle.gameObject,Example.SkillPath.PathType.FIXED_POSITION) as FixedPositionSkillPath;

		if (skillInfo.BasePoint == SkillUnit.BasePoint.EDGE) {
			if (skillInfo.HitArea.area == Example.SkillShape.Area.CIRCLE) {
				fixedPath.fixedPosition.localPosition = new Vector3 (0, 0, skillInfo.HitArea.Param1);
			}
		} else {
			if (skillInfo.HitArea.area != Example.SkillShape.Area.CIRCLE) {
				fixedPath.fixedPosition.localPosition = new Vector3 (0, 0, -skillInfo.HitArea.Param1/2);
			}
		}

		if (skillInfo.ReferPoint != SkillUnit.ReferPoint.SELF) {
			fixedPath.targetType = FollowTargetType.TARGET;
		}

		skillParticle.path = fixedPath;
		skillParticle.hitShape = ImportSkillShape (skillParticle,skillInfo.HitArea); 
		skillParticle.duration = Mathf.Max(skillInfo.MoveDelay + skillInfo.Waves * skillInfo.WaveDelay,skillArt.UnitEffect.PhaseTime);

		hitAction.startFrame = skillUnit.GuidePolicy.GuideTime + skillUnit.GuidePolicy.GuidingTime+skillInfo.MoveDelay;
		hitAction.duration = skillInfo.Waves * skillInfo.WaveDelay;

		return skill;
	}

	static Skill ImportLineSkill(Skill skill,Example.SkillUnit skillUnit,Example.SkillLine skillInfo,Example.SkillArt skillArt,SkillParticle skillParticle,SkillHitAction hitAction){
		if (skillParticle != null) {
			var path = skillParticle.path;
			Object.DestroyImmediate (path);

			var linePath = SkillManager.CreateSkillPath(skillParticle.gameObject,Example.SkillPath.PathType.LINE) as LineSkillPath;
			linePath.endPos.transform.localPosition = new Vector3 (0,0,skillInfo.MoveTime * skillInfo.Speed/1000);

			skillParticle.path = linePath; 
			skillParticle.hitShape.Radius = skillInfo.HitArea.Param1;

			skillParticle.duration = skillInfo.MoveTime; 

			hitAction.duration = skillInfo.MoveTime;
		}
		return skill;
	}

	static Skill ImportMultiLineSkill(Skill skill,Example.SkillUnit skillUnit,Example.SkillMultiLine skillInfo,Example.SkillArt skillArt,SkillParticle skillParticle,SkillHitAction hitAction){
		if (skillParticle != null) {
			skillParticle.duration = skillInfo.MoveTime;
		}
		return skill;
	}

	static Skill ImportFollowSkill(Skill skill,Example.SkillUnit skillUnit,Example.SkillFollow skillInfo,Example.SkillArt skillArt,SkillParticle skillParticle,SkillHitAction hitAction){
		if (skillParticle != null) {

			var path = skillParticle.path;
			Object.DestroyImmediate (path);

			var linePath = SkillManager.CreateSkillPath(skillParticle.gameObject,Example.SkillPath.PathType.FOLLOW) as FollowSkillPath;
			linePath.speed = skillInfo.Speed;

			skillParticle.path = linePath;
			skillParticle.hitShape.Radius = skillInfo.HitArea.Param1;

			hitAction.duration = skillInfo.MaxFollowTime;
			skillParticle.duration = skillInfo.MaxFollowTime;
		} 
		return skill;
	}

	static Skill ImportJumpSkill(Skill skill,Example.SkillUnit skillUnit,Example.SkillJump skillInfo,Example.SkillArt skillArt,SkillParticle skillParticle,SkillHitAction hitAction){
		if (skillParticle != null) { 

			var path = skillParticle.path;
			Object.DestroyImmediate (path);

			var linePath = SkillManager.CreateSkillPath(skillParticle.gameObject,Example.SkillPath.PathType.FOLLOW) as FollowSkillPath;
			linePath.followTarget = skill.GetComponentInParent<SkillCaller> ();
			linePath.speed = -1;

			skillParticle.path = linePath;

			skillParticle.duration = skillInfo.MoveTime ;
			skillParticle.hitShape.Radius = skillInfo.HitArea.Param1;

			hitAction.duration = skillInfo.MoveTime;

			if (skillInfo.Height > 1) {
				var jumpAction = SkillManager.CreateSkillAction<JumpAction> (skill, 0, skillUnit.GuidePolicy.GuideTime + skillUnit.GuidePolicy.GuidingTime + skillUnit.GuidePolicy.EndTime);
				jumpAction.distance = skillInfo.Speed * skillInfo.MoveTime / 1000;
				jumpAction.height = skillInfo.Height;
				jumpAction.endAction = skillArt.EndAction;
				jumpAction.endTime = skillUnit.GuidePolicy.EndTime;
			} else {
				var jumpAction = SkillManager.CreateSkillAction<ChargeAction> (skill, 0, skillUnit.GuidePolicy.GuideTime + skillUnit.GuidePolicy.GuidingTime + skillUnit.GuidePolicy.EndTime);
				jumpAction.distance = skillInfo.Speed * skillInfo.MoveTime / 1000; 
				jumpAction.endAction = skillArt.EndAction;
				jumpAction.endTime = skillUnit.GuidePolicy.EndTime;
			}

		}
		return skill;
	}
}
