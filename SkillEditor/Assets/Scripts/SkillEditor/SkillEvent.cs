using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillEvent : MonoBehaviour {

	public Example.SkillEvent.EventType eventType;

	// Update is called once per frame
	void Update () {
		this.name = displayName;
	}

	public virtual string displayName{
		get { 
			return "SkillEvent";
		}
	}

	public virtual ContentValue[] conditions {
		get{ 
			ContentValue[] args = new ContentValue[0];
			return args;
		}
	}
}
