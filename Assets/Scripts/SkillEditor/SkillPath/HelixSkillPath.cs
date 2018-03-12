using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HelixSkillPath : SkillPath {
    public HelixSkillPath()
    {
        pathType = Example.SkillPath.PathType.HELIX;
    }
    public Transform StartPoint;
    public float radius = 10;
    Vector3 orignalDir;
    protected override void OnStartPath(Transform target)
    {
        orignalDir = target.forward;
    }

    protected override void OnUpdatePath (Transform target,float factor)
	{
        float angleSpeed = 360;
        float moveTime1 =  30 / 360;
        float moveTime2 =  1 - moveTime1;
        float passedTime1 = factor;
        float passedTime2 = factor - moveTime1;
        float time = 1;
        float factor2 = 1;
        if (passedTime1 <= moveTime1)
        {
            factor2 = passedTime1 / moveTime1;
            time = passedTime1 - moveTime1;
        } 
        else
        {
            factor2 = (moveTime2 - passedTime2) / moveTime2;
            time = passedTime2;
        }
        Vector3 point =  Quaternion.AngleAxis(angleSpeed * factor, new Vector3(0,1,0)) * orignalDir;
        Vector3 dir =new  Vector3(point.x, 0, point.z);
        dir = Vector3.Normalize(dir);
        Vector3 pos = StartPoint.position+ dir * factor2*radius;
        target.position =  pos;
        if (updateRotation && dir.sqrMagnitude > 0)
        {
            target.rotation = Quaternion.LookRotation(dir);
        }
    }

	public override ContentValue[] arguments {
		get {
			ContentValue[] args = new ContentValue[1];
			args [0].FloatValue = radius;
			return args;
		}
	}
}
