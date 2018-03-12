using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
 

[ExecuteInEditMode]
public abstract class SkillShape : MonoBehaviour {

	[HideInInspector]
	public Example.SkillShapeNew.ShapeType shapeType;
	 
	[HideInInspector]
	public float scale = 1;

    public Vector3 offset = Vector3.zero;
    private Transform target;

    public Vector3 RotateRound(Vector3 dir, Vector3 axis, float angle)
    {
        Vector3 point = Quaternion.AngleAxis(angle, axis) * dir;
        return point.normalized;
    }

    public virtual bool Intersect(SkillShape other)
    {
        throw new Exception(GetType().Name + ".Intersect(SkillShape) not impl");
    }


    public virtual void Build(AutoSkillParticleEmitterItem targetObj)
    {
		throw new Exception (GetType().Name + ".Build(AutoSkillParticleEmitterItem) not impl");
	}

	public virtual bool Intersect(Vector3 point){
		throw new Exception (GetType().Name +".Intersect(Vector3) not impl");
	}

	public virtual float Width{
		get { 
			return 0;
		}
	}

	public virtual float Height{
		get { 
			return 0;
		}
	}

	public virtual float Radius{
		get { 
			return 0;
		}

		set { 
		}
	}

	public virtual float Angle{
		get { 
			return 0;
		}
	}

    public Transform Target
    {
        get
        {
            if (target == null)
                target = this.transform;
            return target;
        }

        set
        {
            target = value;
        }
    }

    public void AddShape(SkillParticleActionItem sg,SkillShape shape)
    {
        GameObject targetObj = sg.gameObject;
        if (shape.shapeType == Example.SkillShapeNew.ShapeType.CIRCLE)
        {
            CircleSkillShape cskill = (CircleSkillShape)shape;
            CircleSkillShape shape0 = targetObj.AddComponent<CircleSkillShape>();
            shape0.Target = targetObj.transform;
            shape0.offset = cskill.offset;
            shape0.radius = cskill.radius;
            sg.hitShape = shape0;
        }

        if (shape.shapeType == Example.SkillShapeNew.ShapeType.BOX)
        {
            BoxSkillShape cskill = (BoxSkillShape)shape;
            BoxSkillShape shape0 = targetObj.AddComponent<BoxSkillShape>();
            shape0.Target = targetObj.transform;
            shape0.offset = cskill.offset;
            shape0.width = cskill.width;
            shape0.height = cskill.height;
            sg.hitShape = shape0;
        }

        if (shape.shapeType == Example.SkillShapeNew.ShapeType.SECTOR)
        {
            SectorSkillShape cskill = (SectorSkillShape)shape;
            SectorSkillShape shape0 = targetObj.AddComponent<SectorSkillShape>();
            shape0.Target = targetObj.transform;
            shape0.offset = cskill.offset;
            shape0.angle = cskill.angle;
            shape0.radius = cskill.radius;
            sg.hitShape = shape0;
        }
        if (shape.shapeType == Example.SkillShapeNew.ShapeType.TRIANGLE)
        {
            TriangleSkillShape cskill = (TriangleSkillShape)shape;
            TriangleSkillShape shape0 = targetObj.AddComponent<TriangleSkillShape>();
            shape0.Target = targetObj.transform;
            shape0.offset = cskill.offset;
            shape0.width = cskill.width;
            shape0.height = cskill.height;
            sg.hitShape = shape0;
        }
    
    }
}
