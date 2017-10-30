using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FollowTargetType
{
    TARGET = 0,
    SELF = 1
}

[ExecuteInEditMode]
public class FollowSkillPath : SkillPath {

    public FollowSkillPath()
    {
        pathType = Example.SkillPath.PathType.FOLLOW;
    }
    public SkillLife followTarget;
    public Transform targetObj;
    public FollowTargetType targetType;
    public float speed = 1;

	protected override void OnUpdatePath (Transform target,float factor)
	{
        if (followTarget != null)
        {
            if (followTarget.IsSelf)
            {
                target.position = followTarget.transform.position;
            }
            else
            {
                var dir = followTarget.transform.position - target.position;
                dir.Normalize();
                target.position = target.position + Time.deltaTime * speed * dir;
                if (updateRotation)
                {
                    target.rotation = Quaternion.LookRotation(dir);
                }
            }
        }
        else
        {
            if (targetObj != null)
            {
                var dir = targetObj.position - target.position;
                dir.Normalize();
                target.position = target.position + Time.deltaTime * speed * dir;
                if (updateRotation && dir!=Vector3.zero)
                {
                    target.rotation = Quaternion.LookRotation(dir);
                }
            }
        }

	}

	public override ContentValue[] arguments {
		get {
			ContentValue[] args = new ContentValue[2];
			args [0].FloatValue = speed;
			args [1].IntValue = (int)targetType;
			return args;
		}
	}
}
