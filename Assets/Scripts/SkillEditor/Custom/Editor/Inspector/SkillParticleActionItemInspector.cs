using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TimeLine;

[CustomEditor(typeof(SkillParticleActionItem))]
public class SkillParticleActionItemInspector : Editor
{
    private SerializedObject Obj;
    private SerializedProperty firetime;
    private SerializedProperty duration;
    private SerializedProperty id;
    private SerializedProperty effect;
    private SerializedProperty pathType;
    private SerializedProperty hitshapeType;
    private SerializedProperty export;
    private SerializedProperty isBullet;
    public override void OnInspectorGUI()
    {
        Obj = new SerializedObject(this.target);
        this.firetime = Obj.FindProperty("firetime");
        this.duration = Obj.FindProperty("duration");
        this.id = Obj.FindProperty("id");
        this.effect = Obj.FindProperty("effect");
        this.pathType = Obj.FindProperty("pathType");
        this.hitshapeType = Obj.FindProperty("hitshapeType");
        this.isBullet = Obj.FindProperty("isBullet");
        export = Obj.FindProperty("export");

        EditorGUILayout.PropertyField(firetime);
        EditorGUILayout.PropertyField(duration);
        EditorGUILayout.PropertyField(id);
        EditorGUILayout.PropertyField(effect);
        EditorGUILayout.PropertyField(pathType);
        EditorGUILayout.PropertyField(hitshapeType);
        EditorGUILayout.PropertyField(isBullet);
        EditorGUILayout.PropertyField(export);
        
        SkillParticleActionItem targetObj = this.target as SkillParticleActionItem;
        Example.SkillPath.PathType oldPath = targetObj.pathType;
        Example.SkillShapeNew.ShapeType oldShape = targetObj.hitshapeType;
        //targetObj.pathType = (Example.SkillPath.PathType)EditorGUILayout.EnumPopup("pathType", targetObj.pathType);
        Obj.ApplyModifiedProperties();
        if (oldPath != targetObj.pathType)
        {
            if (targetObj.path != null)
            {
                if (targetObj.path is LineSkillPath)
                {
                    LineSkillPath line = (LineSkillPath)targetObj.path;
                    if(line.startPos.gameObject!=null)
                        DestroyImmediate(line.startPos.gameObject);
                    if (line.endPos.gameObject != null)
                        DestroyImmediate(line.endPos.gameObject);
                }
                DestroyImmediate(targetObj.path);
            }
            if (targetObj.pathType == Example.SkillPath.PathType.LINE)
            {
                targetObj.path = targetObj.gameObject.AddComponent<LineSkillPath>();
                LineSkillPath line =(LineSkillPath)targetObj.path;
                line.startPos = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                line.startPos.gameObject.name = "LineStart";
                line.startPos.localScale = Vector3.one * 0.3f;
                line.startPos.parent = targetObj.gameObject.transform;
                line.endPos = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                line.endPos.parent = targetObj.gameObject.transform;
                line.endPos.gameObject.name = "LineEnd";
                line.endPos.localScale = Vector3.one * 0.3f;
            }
            if (targetObj.pathType == Example.SkillPath.PathType.FOLLOW)
            {
                targetObj.path = targetObj.gameObject.AddComponent<FollowSkillPath>();
                //FollowSkillPath line = (FollowSkillPath)targetObj.path;
               
            }
            if (targetObj.pathType == Example.SkillPath.PathType.HELIX)
            {
                targetObj.path = targetObj.gameObject.AddComponent<HelixSkillPath>();
                HelixSkillPath line = (HelixSkillPath)targetObj.path;
                line.StartPoint = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                line.StartPoint.gameObject.name = "StartPoint";
                line.StartPoint.localScale = Vector3.one * 0.3f;
                line.StartPoint.parent = targetObj.gameObject.transform;
            }
            if (targetObj.pathType == Example.SkillPath.PathType.SCALE)
            {
                targetObj.path = targetObj.gameObject.AddComponent<ScaleSkillPath>();
                //ScaleSkillPath line = (ScaleSkillPath)targetObj.path;
            }
            if (targetObj.pathType == Example.SkillPath.PathType.FIXED_POSITION)
            {
                targetObj.path = targetObj.gameObject.AddComponent<FixedPositionSkillPath>();
                FixedPositionSkillPath line = (FixedPositionSkillPath)targetObj.path;
                line.fixedPosition = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
                line.fixedPosition.gameObject.name = "fixedPosition";
                line.fixedPosition.localScale = Vector3.one * 0.3f;
                line.fixedPosition.parent = targetObj.gameObject.transform;
            }
            if (targetObj.pathType == Example.SkillPath.PathType.NONE)
            {
                targetObj.path = targetObj.gameObject.AddComponent<NOPSkillPath>(); 
            }
        }

        if (oldShape != targetObj.hitshapeType)
        {
            if (targetObj.hitShape != null)
            {
              //  if (targetObj.hitShape is CircleSkillShape)
                {
                   // CircleSkillShape line = (CircleSkillShape)targetObj.hitShape;
                }
                DestroyImmediate(targetObj.hitShape);
            }
            if (targetObj.hitshapeType == Example.SkillShapeNew.ShapeType.CIRCLE)
            {
                targetObj.hitShape = targetObj.gameObject.AddComponent<CircleSkillShape>();
            }
            if (targetObj.hitshapeType == Example.SkillShapeNew.ShapeType.BOX)
            {
                targetObj.hitShape = targetObj.gameObject.AddComponent<BoxSkillShape>();
            }
            if (targetObj.hitshapeType == Example.SkillShapeNew.ShapeType.SECTOR)
            {
                targetObj.hitShape = targetObj.gameObject.AddComponent<SectorSkillShape>();
            }
            if (targetObj.hitshapeType == Example.SkillShapeNew.ShapeType.TRIANGLE)
            {
                targetObj.hitShape = targetObj.gameObject.AddComponent<TriangleSkillShape>();
            }
          
        }

        if(GUILayout.Button("添加碰撞动作"))
        {
            GameObject hit = new GameObject();
            hit.name = "HitAction";
            SkillHitAction skillhit = hit.AddComponent<SkillHitAction>();
            hit.transform.SetParent(targetObj.transform);
            targetObj.HitActions.Add(skillhit);
        }
        if (GUILayout.Button("整理碰撞动作"))
        {
            ClearBoom(targetObj);
        }
        if (GUILayout.Button("生成碰撞动作模拟"))
        {
            ClearBoom(targetObj);
            targetObj.Boom();
        }
    }


    private void ClearBoom(SkillParticleActionItem targetObj)
    {
        List<SkillHitAction> clearlist = new List<SkillHitAction>();
        foreach (SkillHitAction shit in targetObj.HitActions)
        {
            if (shit == null)
            {
                clearlist.Add(shit);
            }
        }
        foreach (SkillHitAction shit in clearlist)
        {
            targetObj.HitActions.Remove(shit);
        }
    }
}
