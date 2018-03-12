using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScaleSkillPath : SkillPath {
    public ScaleSkillPath()
    {
        pathType = Example.SkillPath.PathType.SCALE;
    }
    public float fromScale = 1;
	public float toScale = 2;

	private Vector3 scale = Vector3.one;

	protected override void OnUpdatePath (Transform target,float factor)
	{
		scale.x = Mathf.Lerp (fromScale, toScale, factor);
		scale.y = scale.x;
		scale.z = scale.x;
		target.localScale = scale;
	} 

	public override ContentValue[] arguments {
		get {
			ContentValue[] args = new ContentValue[2]; 
			args[0].FloatValue = fromScale;
			args[1].FloatValue = toScale;
			return args;
		}
	}
}
