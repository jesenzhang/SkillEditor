using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

[ExecuteInEditMode]
public abstract class SkillPath : MonoBehaviour {
	
    [HideInInspector]
	public Example.SkillPath.PathType pathType = Example.SkillPath.PathType.NONE;

	public bool updateRotation = true;

	protected Vector3 origPos;

	public virtual void StartPath(Transform target){
		origPos = target.position;
		OnStartPath (target);
	}
	 
	public virtual void UpdatePath(Transform target,float factor){
		OnUpdatePath (target,factor);
	}
    public virtual void OnDrawGizmos()
    {
    }
    protected virtual void OnStartPath(Transform target){
	}

	protected virtual void OnUpdatePath(Transform target,float factor){
	}

	public abstract ContentValue[] arguments {
		get;
	}
}
