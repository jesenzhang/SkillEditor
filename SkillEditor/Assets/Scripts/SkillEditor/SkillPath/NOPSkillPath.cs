using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NOPSkillPath : SkillPath {
	
	protected override void OnUpdatePath (Transform target,float factor)
	{
		
	}

	public override ContentValue[] arguments {
		get {
			ContentValue[] args = new ContentValue[0]; 
			return args;
		}
	}
}
