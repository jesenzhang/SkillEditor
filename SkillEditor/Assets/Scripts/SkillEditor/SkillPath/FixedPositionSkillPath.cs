using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class FixedPositionSkillPath : SkillPath {

	public FollowTargetType targetType = FollowTargetType.SELF;

    public Transform fixedPosition; 

    public FixedPositionSkillPath()
    {
        pathType = Example.SkillPath.PathType.FIXED_POSITION;
    }
    protected override void OnStartPath(Transform target)
    {
        target.position = fixedPosition.position;
    }
    protected override void OnUpdatePath (Transform target,float factor)
	{
		
	}

	public override ContentValue[] arguments {
		get {
			ContentValue[] args = new ContentValue[2]; 
			args [0].Vector3Value = fixedPosition.localPosition;
			args [1].IntValue = (int)targetType;
			return args;
		}
	}
}
