using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

[ExecuteInEditMode]
public class AutoSkillParticleEmitter : SkillParticleEmitter {

	[Header("Emitter Setting")] 
	public SkillShape emitterShape;

	public Vector3 emitterOffset;

	public int emitterCount = 1;

	[Header("Particle Setting")]
	public GameObject effectPrefab;   

	public Example.SkillPath.PathType particlePathType;

	public Example.SkillShapeNew.ShapeType particleHitShapeType;

	public int particleStartFrame;

	public int particleDuration ;

	protected override void OnEditorUpdate(){
		base.OnEditorUpdate ();
		if(emitterShape ==null){
			emitterShape = GetComponent<SkillShape>();
		}
		name = displayName;
		UpdateParticles();
	}

	protected virtual void UpdateParticles(){
		var particles = GetComponentsInChildren<SkillParticle> ();
		int index = 0;
		foreach (var particle in particles) {
			if (index >= emitterCount) {
				GameObject.DestroyImmediate (particle.gameObject);
			} else {
				OnUpdateParticle (particle,index);
			}
			++index;
		}
		for (int i = index; i < emitterCount; ++i) {
			var particle = SkillManager.CreateSkillParticle (this, effectPrefab, particleStartFrame, particleDuration, particlePathType, particleHitShapeType);
			OnUpdateParticle (particle,index);
		}		 
	}

	protected virtual void OnUpdateParticle(SkillParticle particle,int index){ 
		 
	}


	public override string displayName{
		get {  
			return "Particles-"+emitterShape.shapeType;
		}
	}

	public override SkillShape EmitterShape{
		get { 
			return emitterShape;
		}
	}

	public override Vector3 EmitterPosition{
		get { 
			return emitterOffset;
		}
	}

}
