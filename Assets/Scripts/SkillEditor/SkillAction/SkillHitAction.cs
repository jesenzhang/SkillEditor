using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillHitAction : SkillAction {  

	//public SkillParticle particle;

	public string[] buffIDs;  

	public GameObject hitEffect;

	[HideInInspector]
	public string hitEffectName;

	public int hitEffectTime = 100;

    public AudioClip hitAudioClip;
    public string hitAudio;

    public int maxHitCount = 1;
    public bool canThrough = false;
	public bool isSequence = false;
    public override string displayName{
		get { 
			return "Hit-" +hitEffectName+"-"+string.Join(",",buffIDs);
		}
	}

	public override Example.SkillAction.ActionType actionType {
		get{
			return Example.SkillAction.ActionType.HIT_POINT;
		}
	}

	public override ContentValue[] arguments{
		get { 
			ContentValue[] args = new ContentValue[buffIDs.Length + 6]; 
			args [0].StrValue = hitEffectName;
			args [1].IntValue = hitEffectTime;
			args [2].IntValue = isSequence ? 1 : 0;
			args [3].StrValue = hitAudio;
            args[4].IntValue = maxHitCount;
            args[5].IntValue = canThrough ? 1 : 0; 
            for (int i = 0; i < buffIDs.Length; ++i) {
				args [6 + i].StrValue = buffIDs [i];
			}
			return args;
		}
	}

	protected override void OnEditorUpdate ()
	{
		if (hitEffect != null) {
			hitEffectName = hitEffect.name;
		}
        if (hitAudioClip != null)
        {
            hitAudio = hitAudioClip.name;
        }
	}
}
