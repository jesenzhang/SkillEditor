using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TriangleSkillShape : SkillShape { 
	public float width = 1;

	public float height = 1;

	public override bool Intersect (SkillShape other)
	{
		return base.Intersect (other);
	}

	public override bool Intersect (Vector3 point)
	{
		return base.Intersect (point);
	}

	void OnDrawGizmos(){
        // 设置矩阵
        Matrix4x4 defaultMatrix = Gizmos.matrix;
        if (Target != null)
        {
            Gizmos.matrix = Target.localToWorldMatrix;
            float w = width * scale;
            float h = height * scale;
            // 设置颜色
            Color defaultColor = Gizmos.color;
            Gizmos.color = Color.magenta;
            Vector3 pointA = new Vector3(-w / 2f, 0, 0) + offset;
            Vector3 pointB = new Vector3(w / 2f, 0, 0) + offset;
            Vector3 pointC = new Vector3(0, 0, h) + offset;
            // 绘制方形

            // 绘制最后一条线段
            Gizmos.DrawLine(pointA, pointB);
            Gizmos.DrawLine(pointB, pointC);
            Gizmos.DrawLine(pointC, pointA);

            // 恢复默认颜色
            Gizmos.color = defaultColor;

            // 恢复默认矩阵
            Gizmos.matrix = defaultMatrix;
        }
    }

    public override void Build(AutoSkillParticleEmitterItem targetObj)
    {
        ///TRIANGLE
        if (targetObj.emitterShapeType == Example.SkillShapeNew.ShapeType.TRIANGLE)
        {
            TriangleSkillShape Triangle = (TriangleSkillShape)targetObj.emitterShape;

            Vector3 dir = Target.forward;
            Vector3 beginPos = Vector3.zero;
            Vector3 launchPos = Vector3.zero;
            Vector3 pdir = Vector3.zero;

            Vector3 ndir = RotateRound(dir, new Vector3(0, 1, 0), 90);
            beginPos = -ndir * Triangle.width / 2f;
            if (targetObj.emitterCount > 1)
            {
                ndir = ndir * (Triangle.width / (targetObj.emitterCount - 1));
            }
            for (int j = 0; j < targetObj.waves; j++)
            {
                for (int i = 0; i < targetObj.emitterCount; i++)
                {
                    GameObject sub = new GameObject();
                    sub.name = "SubParticle_" + j + "_" + i;
                    sub.transform.SetParent(targetObj.transform);
                    launchPos = beginPos + i * ndir;
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
                        line.startPos.position = Target.position + targetObj.emitterOffset + launchPos;
                        line.startPos.localScale = Vector3.one * 0.3f;

                        line.endPos = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                        line.endPos.parent = sg.transform;
                        line.endPos.gameObject.name = "LineEnd";
                        line.endPos.position = Target.position + targetObj.emitterOffset + dir * Triangle.height;
                        line.endPos.localScale = Vector3.one * 0.3f;
                    }
                }
            }
        }

    }

    public override float Width{
		get { 
			return width;
		}
	}

	public override float Height{
		get { 
			return height;
		}
	}
}
