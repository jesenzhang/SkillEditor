using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineSkillPath : SkillPath {

    public LineSkillPath()
    {
        pathType = Example.SkillPath.PathType.LINE;
    }

    public Transform startPos;
	public Transform endPos; 

	private Vector3 dir;
	 
	protected override void OnStartPath (Transform target)
	{
		origPos = startPos.position;
		dir = endPos.localPosition - startPos.localPosition;
	}

	protected override void OnUpdatePath (Transform target,float factor)
	{  
		target.position = origPos + dir * factor;
		if (updateRotation && dir.sqrMagnitude > 0) {
			target.rotation = Quaternion.LookRotation (dir);
		}
	}

	public override void  OnDrawGizmos(){
		Gizmos.color = Color.blue;
		if (Application.isPlaying) {
			Gizmos.DrawLine (origPos, origPos + dir);
		} else {
			Gizmos.DrawLine (startPos.position, endPos.position);
		}
	}

	public override ContentValue[] arguments {
		get {
			ContentValue[] args = new ContentValue[2];
			args [0].Vector3Value = (startPos.localPosition);
			args [1].Vector3Value = (endPos.localPosition); 
			return args;
		}
	}
}
