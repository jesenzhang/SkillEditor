using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TimeLine;

[CustomEditor(typeof(AutoSkillParticleEmitterItem))]

public class AutoSkillParticleEmitterItemInspector : Editor
{
   // private static List<GameObject> children = new List<GameObject>();
    private SerializedObject Obj;
    private SerializedProperty firetime;
    private SerializedProperty duration;
    private SerializedProperty emitterType;
    
    private SerializedProperty effectPrefab;
    private SerializedProperty emitterShape;
    private SerializedProperty particlePathType;
    private SerializedProperty particleHitShapeType;
    private SerializedProperty particleStartFrame;
    private SerializedProperty particleDuration;
    private SerializedProperty emitterCount;
    private SerializedProperty emitterShapeType;
    private SerializedProperty emitterOffset;
    private SerializedProperty waves;
    private SerializedProperty waveDelay;


    SkillShape ReplaceShape(Example.SkillShapeNew.ShapeType oldShape, Example.SkillShapeNew.ShapeType newShape, GameObject targetObj, SkillShape shape,Transform actor,Vector3 offset)
    {
        if (oldShape != newShape)
        {
            if (shape != null)
            {
                DestroyImmediate(shape);
            }
            if (newShape == Example.SkillShapeNew.ShapeType.CIRCLE)
            {
                CircleSkillShape shape0 = targetObj.AddComponent<CircleSkillShape>();
                shape0.Target = actor;
                shape0.offset = offset;
                return shape0;
            }
            if (newShape == Example.SkillShapeNew.ShapeType.BOX)
            {
                BoxSkillShape shape0 = targetObj.AddComponent<BoxSkillShape>();
                shape0.Target = actor;
                shape0.offset = offset;
                return shape0;
            }
            if (newShape == Example.SkillShapeNew.ShapeType.SECTOR)
            {
                SectorSkillShape shape0 = targetObj.AddComponent<SectorSkillShape>();
                shape0.Target = actor;
                shape0.offset = offset;
                return shape0;
            }
            if( newShape == Example.SkillShapeNew.ShapeType.TRIANGLE)
            {
                TriangleSkillShape shape0 = targetObj.AddComponent<TriangleSkillShape>();
                shape0.Target = actor;
                shape0.offset = offset;
                return shape0;
            }
            if (newShape == Example.SkillShapeNew.ShapeType.NONE)
            {
                return null;
            }
           
        }
        return shape;
    }
    public Vector3 RotateRound(Vector3 dir, Vector3 axis, float angle)
    {
        Vector3 point = Quaternion.AngleAxis(angle, axis) * dir;
        return point.normalized;
    }

    public void OnEmittRandomAreaBoxParticles(AutoSkillParticleEmitterItem targetObj,GameObject Actor, BoxSkillShape box)
    {
        for (int j = 0; j < targetObj.waves; j++)
        {
            for (int i = 0; i < targetObj.emitterCount; i++)
            {
                long tick = System.DateTime.Now.Ticks;
                System.Random ran = new System.Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
                int w = (int)(box.width * 1000);
                float ww = (float)ran.Next(0, w) / 1000f;
                int h = (int)(box.height * 1000);
                float hh = (float)ran.Next(0, h) / 1000f;
                float x = ww - box.width / 2;
                float z = hh - box.height / 2;
                Vector3 paticleDir = new Vector3(x, 0, z);
                GameObject sub = new GameObject();
                sub.name = "SubParticle_" + j + "_" + i;
                sub.transform.SetParent(targetObj.transform);
                SkillParticleActionItem sg = sub.AddComponent<SkillParticleActionItem>();
                sg.export = false;
                sg.effect = targetObj.effectPrefab;
                //sg.effectName = effectPrefab.name;
                sg.pathType = Example.SkillPath.PathType.FIXED_POSITION;
                sg.Firetime = targetObj.Firetime + targetObj.particleStartFrame / 1000f + targetObj.waveDelay * j / 1000f;
                sg.Duration = targetObj.particleDuration / 1000f;
                sg.path = sub.AddComponent<FixedPositionSkillPath>();
                FixedPositionSkillPath fpath = (FixedPositionSkillPath)sg.path;
                fpath.fixedPosition = new GameObject("fixedPosotion").transform;
                fpath.fixedPosition.SetParent(sub.transform);
                fpath.fixedPosition.position = Actor.transform.position + targetObj.emitterOffset + paticleDir;
            }
        }
    }

    public void EmittRandomAreaCircleParticles(AutoSkillParticleEmitterItem targetObj, GameObject Actor, CircleSkillShape circle)
    {
        for (int j = 0; j < targetObj.waves; j++)
        {
            for (int i = 0; i < targetObj.emitterCount; i++)
            {
                long tick = System.DateTime.Now.Ticks;
                System.Random ran = new System.Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
                int rc = (int)(circle.radius * 1000);
                float radius = (float)ran.Next(0, rc) / 1000f;
                float angle = ran.Next(0, 360);
                float x = radius * Mathf.Cos(Mathf.Deg2Rad * angle);
                float z = radius * Mathf.Sin(Mathf.Deg2Rad * angle);
                Vector3 paticleDir = new Vector3(x, 0, z);
                GameObject sub = new GameObject();
                sub.name = "SubParticle_" + j + "_" + i;
                sub.transform.SetParent(targetObj.transform);
                SkillParticleActionItem sg = sub.AddComponent<SkillParticleActionItem>();
                sg.export = false;
                sg.effect = targetObj.effectPrefab;
                //sg.effectName = effectPrefab.name;
                sg.pathType = Example.SkillPath.PathType.FIXED_POSITION;
                sg.Firetime = targetObj.Firetime + targetObj.particleStartFrame / 1000f + targetObj.waveDelay * j / 1000f;
                sg.Duration = targetObj.particleDuration / 1000f;
                sg.path = sub.AddComponent<FixedPositionSkillPath>();
                FixedPositionSkillPath fpath = (FixedPositionSkillPath)sg.path;
                fpath.fixedPosition = new GameObject("fixedPosotion").transform;
                fpath.fixedPosition.SetParent(sub.transform);
                fpath.fixedPosition.position = Actor.transform.position + targetObj.emitterOffset + paticleDir;
            }
        }
    }


    void RandomEmitterFunc(AutoSkillParticleEmitterItem targetObj)
    {
        if (targetObj.emitterType == Example.SkillParticleEmitter.EmitterType.RANDOM)
        {
            SkillParticleActionItem[] children = targetObj.transform.GetComponentsInChildren<SkillParticleActionItem>();
            for (int i = 0; i < children.Length; i++)
            {
                GameObject.DestroyImmediate(children[i].gameObject);
            }
            children = null;
            GameObject Actor = null;
            if (targetObj.TimelineTrack.TrackGroup is ActorTrackGroup)
            {
                ActorTrackGroup ag = (ActorTrackGroup)targetObj.TimelineTrack.TrackGroup;
                Actor = ag.Actor.gameObject;
            }
            if (targetObj.waves <= 0)
                targetObj.waves = 1;

            if (targetObj.emitterShapeType == Example.SkillShapeNew.ShapeType.CIRCLE)
            {
                CircleSkillShape circle = (CircleSkillShape)targetObj.emitterShape;
                if (targetObj.particlePathType == Example.SkillPath.PathType.FIXED_POSITION)
                {
                    EmittRandomAreaCircleParticles(targetObj,Actor, circle);
                }

            }
            if (targetObj.emitterShapeType == Example.SkillShapeNew.ShapeType.BOX)
            {
                BoxSkillShape box = (BoxSkillShape)targetObj.emitterShape;
                if (targetObj.particlePathType == Example.SkillPath.PathType.FIXED_POSITION)
                {
                    OnEmittRandomAreaBoxParticles(targetObj,Actor, box);
                }
            }
        }
    }
    public override void OnInspectorGUI()
    {
        Obj = new SerializedObject(this.target);
        this.firetime = Obj.FindProperty("firetime");
        this.duration = Obj.FindProperty("duration");
       
        this.emitterShapeType = Obj.FindProperty("emitterShapeType");
        this.emitterType = Obj.FindProperty("emitterType");
        
        this.emitterOffset = Obj.FindProperty("emitterOffset");
        
        this.emitterCount = Obj.FindProperty("emitterCount");
        this.emitterShape = Obj.FindProperty("emitterShape");
        this.effectPrefab = Obj.FindProperty("effectPrefab");

        this.particlePathType = Obj.FindProperty("particlePathType");
        this.particleHitShapeType = Obj.FindProperty("particleHitShapeType");
        this.particleStartFrame = Obj.FindProperty("particleStartFrame");
        this.particleDuration = Obj.FindProperty("particleDuration");
        this.waves = Obj.FindProperty("waves");
        this.waveDelay = Obj.FindProperty("waveDelay");


        EditorGUILayout.PropertyField(firetime);
        EditorGUILayout.PropertyField(duration);
        
        EditorGUILayout.PropertyField(emitterShape);
        EditorGUILayout.PropertyField(emitterType);
        EditorGUILayout.PropertyField(emitterShapeType);
        EditorGUILayout.PropertyField(emitterOffset);
        EditorGUILayout.PropertyField(emitterCount);
        EditorGUILayout.PropertyField(waves);
        EditorGUILayout.PropertyField(waveDelay);

        EditorGUILayout.PropertyField(effectPrefab);

        EditorGUILayout.PropertyField(particlePathType);
        EditorGUILayout.PropertyField(particleHitShapeType);
        EditorGUILayout.PropertyField(particleStartFrame);
        EditorGUILayout.PropertyField(particleDuration);

        AutoSkillParticleEmitterItem targetObj = this.target as AutoSkillParticleEmitterItem;
       // Example.SkillPath.PathType oldPath = targetObj.particlePathType;
        Example.SkillShapeNew.ShapeType oldShape =  targetObj.emitterShapeType;
        Example.SkillShapeNew.ShapeType oldhitShape = targetObj.particleHitShapeType;
       
        Obj.ApplyModifiedProperties();

        targetObj.emitterShape = ReplaceShape(oldShape, targetObj.emitterShapeType, targetObj.gameObject, targetObj.emitterShape, targetObj.Actor(), targetObj.emitterOffset);
        if(targetObj.emitterShape!=null)
            targetObj.emitterShape.shapeType = targetObj.emitterShapeType;

        targetObj.hitShape = ReplaceShape(oldhitShape, targetObj.particleHitShapeType, targetObj.gameObject, targetObj.hitShape, targetObj.Actor(),Vector3.zero);
        if (targetObj.hitShape != null)
            targetObj.hitShape.shapeType = targetObj.particleHitShapeType;

        if (targetObj.emitterType == Example.SkillParticleEmitter.EmitterType.RANDOM)
        {
            if (targetObj.emitterShapeType != Example.SkillShapeNew.ShapeType.CIRCLE && targetObj.emitterShapeType != Example.SkillShapeNew.ShapeType.BOX)
            {
                EditorGUILayout.HelpBox("随机发射形状仅支持Circle和Box", MessageType.Error);
            }
            if (targetObj.particlePathType != Example.SkillPath.PathType.FIXED_POSITION)
            {
                EditorGUILayout.HelpBox("随机发射仅支持FIXED_POSITION粒子线路类型", MessageType.Error);
            }
          
        }
        else
        {
            
            if (targetObj.particlePathType != Example.SkillPath.PathType.LINE)
            {
                EditorGUILayout.HelpBox("不支持除直线以外的粒子线路类型", MessageType.Error);
            }
        }
        if (targetObj.emitterType == Example.SkillParticleEmitter.EmitterType.RANDOM)
        {
            if (GUILayout.Button("生成"))
            {
                RandomEmitterFunc(targetObj);
             }
        }
        if (targetObj.emitterType == Example.SkillParticleEmitter.EmitterType.FIXED)
        {
            if (GUILayout.Button("生成"))
            {
                SkillParticleActionItem[] children = targetObj.transform.GetComponentsInChildren<SkillParticleActionItem>();
                for (int i = 0; i < children.Length; i++)
                {
                    GameObject.DestroyImmediate(children[i].gameObject);
                }
                children = null;

                GameObject Actor = null;
                if (targetObj.TimelineTrack.TrackGroup is ActorTrackGroup)
                {
                    ActorTrackGroup ag = (ActorTrackGroup)targetObj.TimelineTrack.TrackGroup;
                    Actor = ag.Actor.gameObject;
                }
                if (targetObj.waves <= 0)
                    targetObj.waves = 1;

                targetObj.emitterShape.Build(targetObj);
                
            }
        }
    }
}
