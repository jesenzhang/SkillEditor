using System.Collections;
using System.Collections.Generic;
using UnityEngine; 


[ExecuteInEditMode]
public class SkillParticle : SkillTimeTween { 

	public int id;

	public GameObject effect;

	[HideInInspector]
	public string effectName;

	public GameObject hitEffect;

	[HideInInspector]
	public string hitEffectName;

	public int hitEffectTime = 100;


	public SkillShape hitShape; 

	public SkillPath path;	 


	public override string displayName{
		get { 
			return "Particle-" + effectName;
		}
	}
	 

	protected override void OnEditorUpdate ()
	{
		base.OnEditorUpdate ();
		if (hitEffect != null) {
			hitEffectName = hitEffect.name;
		}
	}

	protected override void OnStart(SkillCaller caller,Skill skill,int time){
		path.StartPath (transform);
	}

	protected override void OnUpdate(SkillCaller caller,Skill skill,int time){
		float factor = 0;
		if (duration > 0) {
			factor = ((float)time) / duration;
		}
		path.UpdatePath (transform, factor); 
	}

	public virtual void CheckHit(SkillCaller caller,Skill skill,int time){ 
		
	}

}
