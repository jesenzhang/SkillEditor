using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CircleSkillShape : SkillShape {

    CircleSkillShape()
    {
        shapeType = Example.SkillShapeNew.ShapeType.CIRCLE;
    }
    public float radius;
    float m_Theta = 0.1f; // 值越低圆环越平滑

    public override bool Intersect (SkillShape other)
	{
		return base.Intersect (other);
	}

	public override bool Intersect (Vector3 point)
	{
		point.y = transform.position.y;
		return (transform.position - point).sqrMagnitude <= radius * radius * scale * scale;
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red; 
		float r = radius * scale;
        //Gizmos.DrawWireSphere (transform.position,r);
        //Gizmos.DrawSphere(transform.position,0.1f);

        if (m_Theta < 0.0001f) m_Theta = 0.0001f;
        if (Target != null)
        {
            // 设置矩阵
            Matrix4x4 defaultMatrix = Gizmos.matrix;
            Gizmos.matrix = Target.localToWorldMatrix;

            // 设置颜色
            Color defaultColor = Gizmos.color;
            Gizmos.color = Color.red;

            // 绘制圆环
            Vector3 beginPoint = Vector3.zero + offset;
            Vector3 firstPoint = Vector3.zero + offset;
            for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
            {
                float x = r * Mathf.Cos(theta);
                float z = r * Mathf.Sin(theta);
                Vector3 endPoint = new Vector3(x, 0, z) + offset ;
                if (theta == 0)
                {
                    firstPoint = endPoint;
                }
                else
                {
                    Gizmos.DrawLine(beginPoint, endPoint);
                }
                beginPoint = endPoint;
            }

            // 绘制最后一条线段
            Gizmos.DrawLine(firstPoint, beginPoint);

            // 恢复默认颜色
            Gizmos.color = defaultColor;

            // 恢复默认矩阵
            Gizmos.matrix = defaultMatrix;
            Gizmos.color = Color.white;
        }
     
    }

    public override void Build(AutoSkillParticleEmitterItem targetObj)
    {
        if (targetObj.emitterShapeType == Example.SkillShapeNew.ShapeType.CIRCLE)
        {
            offset = targetObj.emitterOffset;
            CircleSkillShape circle = (CircleSkillShape)targetObj.emitterShape;
            float angle = 360f / targetObj.emitterCount;

            Vector3 dir = Target.transform.forward;
            for (int j = 0; j < targetObj.waves; j++)
            {
                for (int i = 0; i < targetObj.emitterCount; i++)
                {
                    Vector3 ndir = RotateRound(dir, new Vector3(0, 1, 0), i * angle);

                    GameObject sub = new GameObject();
                    sub.name = "SubParticle_" + j + "_" + i;
                    sub.transform.SetParent(targetObj.transform);
                    SkillParticleActionItem sg = sub.AddComponent<SkillParticleActionItem>();
                    sg.export = true;
                    sg.effect = targetObj.effectPrefab;
                    //sg.effectName = targetObj.effectPrefab.name;
                    sg.pathType = targetObj.particlePathType;
                    sg.Firetime = targetObj.Firetime + targetObj.particleStartFrame / 1000f + targetObj.waveDelay * j / 1000f;
                    sg.Duration = targetObj.particleDuration / 1000f;
                    sg.hitshapeType = targetObj.particleHitShapeType;
                    //sg.hitShape = targetObj.hitShape;
                    AddShape(sg, targetObj.hitShape);
                    if (targetObj.particlePathType == Example.SkillPath.PathType.LINE)
                    {
                        sg.path = sub.AddComponent<LineSkillPath>();
                        sg.path.pathType = Example.SkillPath.PathType.LINE;
                        LineSkillPath line = (LineSkillPath)sg.path;
                        line.startPos = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                        line.startPos.gameObject.name = "LineStart";
                        line.startPos.localScale = Vector3.one * 0.3f;
                        line.startPos.parent = sg.transform;
                        line.startPos.position = Target.transform.position + targetObj.emitterOffset;

                        line.endPos = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                        line.endPos.parent = sg.transform;
                        line.endPos.gameObject.name = "LineEnd";
                        line.endPos.localScale = Vector3.one * 0.3f;
                        line.endPos.position = Target.transform.position + targetObj.emitterOffset + ndir * circle.radius;
                    }

                }
            }
        }
    }
	public override float Radius{
		get { 
			return radius;
		}
		set { 
			radius = value;
		}
	}
}
