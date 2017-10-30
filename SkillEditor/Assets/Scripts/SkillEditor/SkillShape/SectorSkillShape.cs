using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SectorSkillShape : SkillShape {

	public float radius;

	public float angle;


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

        float m_Theta = 0.0001f;

        // 设置矩阵
        Matrix4x4 defaultMatrix = Gizmos.matrix;
        if (Target != null)
        {
            Gizmos.matrix = Target.localToWorldMatrix;

            // 设置颜色
            Color defaultColor = Gizmos.color;
            Gizmos.color = Color.cyan;

            // 绘制圆环
            Vector3 beginPoint = Vector3.zero + offset;
            Vector3 firstPoint = Vector3.zero + offset;
            float rad = Mathf.Deg2Rad * angle;
            for (float theta = Mathf.PI / 2f - rad / 2f; theta <= Mathf.PI / 2f + rad / 2; theta += m_Theta)
            {
                float x = r * Mathf.Cos(theta);
                float z = r * Mathf.Sin(theta);
                Vector3 endPoint = new Vector3(x, 0, z) + offset;
                if (theta == -rad / 2f)
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
            Gizmos.DrawLine(firstPoint, offset);
            Gizmos.DrawLine(beginPoint, offset);

            // 恢复默认颜色
            Gizmos.color = defaultColor;

            // 恢复默认矩阵
            Gizmos.matrix = defaultMatrix;
            Gizmos.color = Color.white;
        }
    }

    public override void Build(AutoSkillParticleEmitterItem targetObj)
    {
        ///SECTOR
        if (targetObj.emitterShapeType == Example.SkillShapeNew.ShapeType.SECTOR)
        {
            SectorSkillShape Sector = (SectorSkillShape)targetObj.emitterShape;
            Vector3 dir = Target.forward;
            float angle = Sector.angle / (targetObj.emitterCount - 1);
            float beginAngle = -angle * (targetObj.emitterCount - 1) / 2;

            for (int j = 0; j < targetObj.waves; j++)
            {
                for (int i = 0; i < targetObj.emitterCount; i++)
                {
                    Vector3 ndir = RotateRound(dir, new Vector3(0, 1, 0), beginAngle + i * angle);
                    GameObject sub = new GameObject();
                    sub.name = "SubParticle_" + j + "_" + i;
                    sub.transform.SetParent(targetObj.transform);
                    SkillParticleActionItem sg = sub.AddComponent<SkillParticleActionItem>();
                    sg.effect = targetObj.effectPrefab;
                    sg.effectName = targetObj.effectPrefab.name;
                    sg.pathType = targetObj.particlePathType;
                    sg.Firetime = targetObj.particleStartFrame / 1000f + targetObj.waveDelay * j / 1000f;
                    sg.Duration = targetObj.particleDuration / 1000f;
                    if (targetObj.particlePathType == Example.SkillPath.PathType.LINE)
                    {
                        sg.path = sub.AddComponent<LineSkillPath>();
                        sg.path.pathType = Example.SkillPath.PathType.LINE;
                        LineSkillPath line = (LineSkillPath)sg.path;
                        line.startPos = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                        line.startPos.gameObject.name = "LineStart";
                        line.startPos.parent = sg.transform;
                        line.startPos.position = Target.position + targetObj.emitterOffset;
                        line.startPos.localScale = Vector3.one * 0.3f;

                        line.endPos = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                        line.endPos.parent = sg.transform;
                        line.endPos.gameObject.name = "LineEnd";
                        line.endPos.position = Target.position + targetObj.emitterOffset + ndir * Sector.radius;
                        line.endPos.localScale = Vector3.one * 0.3f;
                    }
                }
            }
        }

    }

    public override float Radius{
		get { 
			return radius;
		}
	}

	public override float Angle{
		get { 
			return angle;
		}
	}
}
