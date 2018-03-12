using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillParticleCircleEmitter : AutoSkillParticleEmitter {	
	
	protected override void OnUpdateParticle(SkillParticle particle,int index){
		if (emitterType == Example.SkillParticleEmitter.EmitterType.FIXED) {
			//particle.transform.position = transform.position; 
		}else if(emitterType == Example.SkillParticleEmitter.EmitterType.RANDOM){
			
		}
	}
 
}
