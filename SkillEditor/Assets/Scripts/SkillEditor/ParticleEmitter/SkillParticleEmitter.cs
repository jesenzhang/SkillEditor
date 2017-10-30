using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillParticleEmitter : SkillTimeTween { 


	[HideInInspector]
	public Example.SkillParticleEmitter.EmitterType emitterType = Example.SkillParticleEmitter.EmitterType.FIXED; 

	
	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR
		if(!Application.isPlaying){
			OnEditorUpdate();
		}
		#endif
	}

	public virtual SkillShape EmitterShape{
		get { 
			return null;
		}
	}

	public virtual Vector3 EmitterPosition{
		get { 
			return Vector3.zero;
		}
	}
}
