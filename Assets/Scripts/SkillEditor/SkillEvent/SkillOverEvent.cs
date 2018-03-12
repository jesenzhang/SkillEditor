using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkillOverEvent : SkillEvent {	
	
	public override string displayName {
		get {
			return "SkillOverEvent";
		}
	}

	public override ContentValue[] conditions {
		get{ 
			ContentValue[] args = new ContentValue[0];
			return args;
		}
	}
}
