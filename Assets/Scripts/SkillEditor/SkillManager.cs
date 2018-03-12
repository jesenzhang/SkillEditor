using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager{
	
	public static SkillCaller GetSkillCaller(Animator animator){
		SkillCaller caller = null;
		var go = GameObject.Find (animator.name+"-Skills");
		if (go == null) {
			go = new GameObject (animator.name + "-Skills");
		}
		caller = go.GetComponent<SkillCaller> ();
		if (caller == null) {
			caller = go.AddComponent<SkillCaller> ();
		}
		caller.animator = animator;
		return caller;
	}

	public static Skill CreateSkill(SkillCaller caller,string skillID,Example.Skill.SkillType skillType,params Example.Skill.TargetType[] skillTargets){ 
		var go = CreateGameObject (caller.transform,"Skill-" + skillID);  
		var skill = go.AddComponent<Skill> ();

		GetSkillActionRoot (skill);
		GetSkillParticleRoot(skill);

		skill.skillID = skillID;
		skill.skillType = skillType;
		skill.targetTypes = skillTargets;

		return skill;
	}

	public static PlayEffectAction CreateSkillEffect(Skill skill,GameObject effectPrefab,int startFrame,int duration){
		if (effectPrefab == null)
			return null;

		var root = GetSkillEffectRoot (skill); 
		var go = CreateGameObject (root,"PlayEffect-"+effectPrefab.name);

		var effect = InstantiateGameObject (go.transform,effectPrefab,"Effect"); 

		var skillEffect = go.AddComponent<PlayEffectAction> ();


		skillEffect.startFrame = startFrame;
		skillEffect.duration = duration;
		skillEffect.effectName = effectPrefab.name; 
		skillEffect.effect = effect;

		return skillEffect;
	}

	public static PlayEffectAction CreateSkillEffect(Skill skill,string effectName,int startFrame,int duration){ 

		var root = GetSkillEffectRoot (skill); 
		var go = CreateGameObject (root,"PlayEffect-"+effectName); 

		var skillEffect = go.AddComponent<PlayEffectAction> ();

		skillEffect.startFrame = startFrame;
		skillEffect.duration = duration;
		skillEffect.effectName = effectName;  

		return skillEffect;
	}

	public static SkillParticle CreateSkillParticle(Skill skill,GameObject effectPrefab,int startFrame,int duration,Example.SkillPath.PathType pathType,Example.SkillShapeNew.ShapeType shapeType){ 
		var root = GetSkillParticleRoot (skill); 

		var go = CreateGameObject (root,"Particle"); 

		var path = CreateSkillPath (go,pathType);
		var shape = CreateSkillShape(go,shapeType);

		var skillParticle = go.AddComponent<SkillParticle> ();

		//var effect = effectPrefab!=null?InstantiateGameObject (go.transform,effectPrefab,"Effect"):null; 

		skillParticle.path = path;
		skillParticle.hitShape = shape;
		skillParticle.startFrame = startFrame;
		skillParticle.duration = duration; 
		skillParticle.effect = effectPrefab;
		skillParticle.effectName = effectPrefab!=null?effectPrefab.name:"";

		return skillParticle;
	}

	public static SkillParticle CreateSkillParticle(SkillParticleEmitter skillEmitter,GameObject effectPrefab,int startFrame,int duration,Example.SkillPath.PathType pathType,Example.SkillShapeNew.ShapeType shapeType){ 
		 
		var go = CreateGameObject (skillEmitter.transform,"Particle"); 

		var path = CreateSkillPath (go,pathType);
		var shape = CreateSkillShape(go,shapeType);

		var skillParticle = go.AddComponent<SkillParticle> ();

		//var effect = InstantiateGameObject (go.transform,effectPrefab,"Effect"); 

		skillParticle.path = path;
		skillParticle.hitShape = shape;
		skillParticle.startFrame = startFrame;
		skillParticle.duration = duration; 
		skillParticle.effect = effectPrefab;
		skillParticle.effectName = effectPrefab!=null?effectPrefab.name:"";

		return skillParticle;
	}

	public static AutoSkillParticleEmitter CreateSkillParticleEmitter(Skill skill,Example.SkillParticleEmitter.EmitterType emitterType,Example.SkillShapeNew.ShapeType shapeType,int count){ 
 
		var go = CreateGameObject (skill.transform, "ParticleEmitter");
		 
		var shape = CreateSkillShape(go,shapeType); 

		var particleEmitter = go.AddComponent<AutoSkillParticleEmitter> ();  
		particleEmitter.emitterType = emitterType;
		particleEmitter.emitterShape = shape;
		particleEmitter.emitterCount = count;

		return particleEmitter;
	}

	public static T CreateSkillEvent<T>(Skill skill,Example.SkillEvent.EventType eventType) where T:SkillEvent{
		var root = GetSkillEventRoot (skill);
		var go = CreateGameObject (root,"Event");
		var skillEvent =  go.AddComponent<T> (); 
		skillEvent.eventType = eventType;
		return skillEvent;
	}
	 
	public static T CreateSkillAction<T>(MonoBehaviour parent,int startFrame,int duration) where T:SkillAction{
		var root = GetSkillActionRoot (parent);
		var go = CreateGameObject (root,"Action");
		var skillEvent =  go.AddComponent<T> (); 
		skillEvent.startFrame = startFrame;
		skillEvent.duration = duration;
		return skillEvent;
	}

	public static PlayModelAction CreatePlayAction(Skill skill,string actionName,int startFrame){
		var root = GetSkillActionRoot (skill);
		var go = CreateGameObject (root,"Action-" + actionName); 

		var skillEvent = go.AddComponent<PlayModelAction> ();

		skillEvent.actionName = actionName;
		skillEvent.startFrame = startFrame; 

		return skillEvent;
	}
	 

	public static SkillHitAction CreateSkillHit(SkillParticle skillParticle,params string[] buffIDs){
		var root = GetSkillActionRoot (skillParticle);
		var go = CreateGameObject (root,"Hit"); 

		var skillEvent = go.AddComponent<SkillHitAction> ();

		//skillEvent.particle = skillParticle;
		skillEvent.buffIDs = buffIDs; 

		return skillEvent;
	}

	public static AddBuffAction CreateBuffEvent(Skill skill,string buffID,int startFrame){
		var root = GetSkillActionRoot (skill);
		var go = CreateGameObject (root,"Buff-" + buffID); 

		var skillEvent = go.AddComponent<AddBuffAction> (); 

		skillEvent.buffID = buffID;
		skillEvent.startFrame = startFrame; 

		return skillEvent;
	}

	public static Transform GetSkillActionRoot(MonoBehaviour parent){
		var root = parent.transform.Find ("Actions");
		if (root == null) {
			root = CreateGameObject(parent.transform,"Actions").transform; 
		}
		return root;
	}

	public static Transform GetSkillEventRoot(MonoBehaviour parent){
		var root = parent.transform.Find ("Events");
		if (root == null) {
			root = CreateGameObject(parent.transform,"Events").transform; 
		}
		return root;
	}
	 

	public static Transform GetSkillParticleRoot(MonoBehaviour parent){
		var root = parent.transform.Find ("Particles");
		if (root == null) {
			root = CreateGameObject(parent.transform,"Particles").transform; 
			root.gameObject.AddComponent<ManualSkillParticleEmitter> ();
		}
		return root;
	}


	static Transform GetSkillEffectRoot(Skill skill){
		var root = skill.transform.Find ("Actions");
		if (root == null) {
			root = CreateGameObject(skill.transform,"Actions").transform; 
		}
		return root;
	}

	public static SkillPath CreateSkillPath(GameObject go,Example.SkillPath.PathType type) {
		SkillPath path = null;
		switch (type) {
		case Example.SkillPath.PathType.NONE:
			path = go.AddComponent<NOPSkillPath> ();
			break;
		case Example.SkillPath.PathType.LINE:
			var linePath = go.AddComponent<LineSkillPath> (); 
			linePath.startPos = CreateGameObject (go.transform, "Start").transform;
			linePath.endPos = CreateGameObject (go.transform, "End").transform;
			path = linePath;
			break;
		case Example.SkillPath.PathType.FOLLOW:
			var followPath = go.AddComponent<FollowSkillPath> (); 
			path = followPath;
			break;
		case Example.SkillPath.PathType.HELIX:
			var helixPath = go.AddComponent<HelixSkillPath> (); 
			path = helixPath;
			break;
		case Example.SkillPath.PathType.FIXED_POSITION:
			var fixedPath = go.AddComponent<FixedPositionSkillPath> (); 
			fixedPath.fixedPosition = CreateGameObject (go.transform, "FixedPosition").transform;
			path = fixedPath;
			break;
		}
		path.pathType = type;
		return path;
	}

	static SkillShape CreateSkillShape(GameObject go,Example.SkillShapeNew.ShapeType type){
		SkillShape shape = null;
		switch (type) {
		case Example.SkillShapeNew.ShapeType.BOX:
			shape = go.AddComponent<BoxSkillShape> ();
			break;
		case Example.SkillShapeNew.ShapeType.CIRCLE:
			var circle = go.AddComponent<CircleSkillShape> (); 
			shape = circle;
			break; 
		}
		shape.shapeType = type;
		return shape;
	}

	public static GameObject CreateGameObject(Transform parent,string name){
		var go = new GameObject (name); 
		go.transform.parent = parent;
		go.transform.localScale = Vector3.one;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localPosition = Vector3.zero;
		return go;
	}

	public static GameObject InstantiateGameObject(Transform parent,GameObject prefab,string name){
		var go = GameObject.Instantiate (prefab);
		go.name = name;
		go.transform.parent = parent;
		go.transform.localScale = Vector3.one;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localPosition = Vector3.zero;
		return go;
	}
}
