using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TimeLine;

[CustomEditor(typeof(ManualSkillParticleEmitterItem))]

public class ManualSkillParticleEmitterItemInspector : Editor
{
   // private static List<GameObject> children = new List<GameObject>();
    private SerializedObject Obj;
    private SerializedProperty firetime;
    private SerializedProperty duration;

    public override void OnInspectorGUI()
    {
        Obj = new SerializedObject(this.target);
        this.firetime = Obj.FindProperty("firetime");
        this.duration = Obj.FindProperty("duration");

        EditorGUILayout.PropertyField(firetime);
        EditorGUILayout.PropertyField(duration);


        ManualSkillParticleEmitterItem targetObj = this.target as ManualSkillParticleEmitterItem; 
        //
        Obj.ApplyModifiedProperties(); 

        if (GUILayout.Button("添加技能粒子"))
        {
            GameObject sub = new GameObject();
            sub.name = "SubParticle";
            sub.transform.SetParent(targetObj.transform);
            SkillParticleActionItem sg = sub.AddComponent<SkillParticleActionItem>();
            sg.Duration = 1;
        }

    }
}
