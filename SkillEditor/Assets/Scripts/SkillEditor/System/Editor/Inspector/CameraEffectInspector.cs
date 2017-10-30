using TimeLine;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using System.IO;
using System.Text;


[CustomEditor(typeof(CameraEffect))]
public class CameraEffectInspector : Editor
{
    private SerializedObject effectCameraGameObject;
    private SerializedProperty effectCamera;
    private SerializedProperty effectType;
    private SerializedProperty firetime;
    private SerializedProperty duration;

    [MenuItem("CameraEffect/Load CameraEffects")]
    public static void LoadCameraEffects()
    {
        //Camera Filter Pack
        string[] folds = Directory.GetDirectories("Assets/", "Camera Filter Pack", SearchOption.AllDirectories);
        for (int i = 0; i < folds.Length; i++)
        {
            Debug.Log(folds[i]);
        }
        string Root = "";
        if (folds.Length == 1)
        {
            Root = folds[0];
        }
     
        string Path = Root+"/Scripts";
        DirectoryInfo direction = new DirectoryInfo(Path);
        FileInfo[] files = direction.GetFiles("*.cs", SearchOption.AllDirectories);
        // string[] filesList = Directory.GetFiles(Path, "*.cs", SearchOption.TopDirectoryOnly);
        // for (int i = 0; i < filesList.Length; i++)
        //  {
        //     string filePath = filesList[i].Replace(@"\", @"/");
        //  }
        CameraEffectTypes.Clean();
        for (int i = 0; i < files.Length; i++)
        {
            string name = files[i].Name.Split('.')[0];
           
			Assembly asmb = Assembly.GetAssembly (typeof(TimelineManager)); 
			Type type = asmb.GetType(name) ;
            CameraEffectTypes.AddType(type);
        }
    }

    public void OnEnable()
    {
        effectCameraGameObject = new SerializedObject(this.target);
        if (CameraEffectTypes.InitData == false)
        {
            LoadCameraEffects();
            CameraEffectTypes.InitData = true;
        }
        this.firetime = effectCameraGameObject.FindProperty("firetime");
        this.duration = effectCameraGameObject.FindProperty("duration");
        this.effectCamera = effectCameraGameObject.FindProperty("effectCamera");
        this.effectType = effectCameraGameObject.FindProperty("effectType");

        CameraEffect cameraeffect = (CameraEffect)target;
        if (cameraeffect.effect != null)
            cameraeffect.effect.enabled = true;

    }

    public void OnDisable()
    {
        CameraEffect cameraeffect = (CameraEffect)target;
        if (cameraeffect.effect != null)
            cameraeffect.effect.enabled = false;
    }
    

    public override void OnInspectorGUI()
    {
        CameraEffect cameraeffect = (CameraEffect)target;
        effectCameraGameObject.Update();
        EditorGUI.BeginChangeCheck();
        this.firetime = effectCameraGameObject.FindProperty("firetime");
        this.duration = effectCameraGameObject.FindProperty("duration");
        EditorGUILayout.PropertyField(firetime);
        EditorGUILayout.PropertyField(duration);
        EditorGUILayout.PropertyField(effectCamera);
        effectCameraGameObject.ApplyModifiedProperties();
        if (cameraeffect.effectCamera == null)
        {
            EditorGUILayout.HelpBox("请先设置相机 ", MessageType.Error);
            return;
        }

        int oindex = cameraeffect.index;// CameraEffectTypes.getIndex(cameraeffect.effectName);
        int newindex = EditorGUILayout.Popup("Type:", oindex, CameraEffectTypes.getEnums());
        if (newindex != oindex)
        {
            cameraeffect.index = newindex;
            if (cameraeffect.effect != null)
                    DestroyImmediate(cameraeffect.effect);
            cameraeffect.effectName = CameraEffectTypes.getName(cameraeffect.index);
            cameraeffect.effect = (MonoBehaviour)cameraeffect.effectCamera.gameObject.AddComponent(CameraEffectTypes.getType(cameraeffect.index));
            cameraeffect.effect.enabled = false;
        }
        Type effectype = CameraEffectTypes.getType(cameraeffect.index);
        if (effectype != null)
        { 
            FieldInfo[] fields = effectype.GetFields();
            if (cameraeffect.effect == null)
            {
                cameraeffect.effect = (MonoBehaviour)cameraeffect.effectCamera.gameObject.AddComponent(effectype);
                
            }
            
            foreach (FieldInfo f in fields)
            {
                if (f.IsPublic)
                    if (f.FieldType == typeof(bool))
                    {
                        bool oldvalue = (bool)f.GetValue(cameraeffect.effect);
                        bool newvalue = EditorGUILayout.Toggle(f.Name, oldvalue);
                        if (newvalue != oldvalue)
                        {
                            f.SetValue(cameraeffect.effect, newvalue);
                        }
                    }
                {
                    if (f.FieldType == typeof(string))
                    {
                        string oldvalue = (string)f.GetValue(cameraeffect.effect);
                        string newvalue = EditorGUILayout.TextField(f.Name, oldvalue);
                        if (newvalue != oldvalue)
                        {
                            f.SetValue(cameraeffect.effect, newvalue);
                        }
                    }
                    if (f.FieldType == typeof(float))
                    {
                        float oldvalue = (float)f.GetValue(cameraeffect.effect);
                        float newvalue = EditorGUILayout.FloatField(f.Name, oldvalue);
                        if (newvalue != oldvalue)
                        {
                            f.SetValue(cameraeffect.effect, newvalue);
                        }
                    }
                    if (f.FieldType == typeof(int))
                    {
                        int oldvalue = (int)f.GetValue(cameraeffect.effect);
                        int newvalue = EditorGUILayout.IntField(f.Name, oldvalue);
                        if (newvalue != oldvalue)
                        {
                            f.SetValue(cameraeffect.effect, newvalue);
                        }
                    }
                    if (f.FieldType == typeof(Vector2))
                    {
                        Vector2 oldvalue = (Vector2)f.GetValue(cameraeffect.effect);
                        Vector2 newvalue = EditorGUILayout.Vector2Field(f.Name, oldvalue);
                        if (newvalue != oldvalue)
                        {
                            f.SetValue(cameraeffect.effect, newvalue);
                        }
                    }
                    if (f.FieldType == typeof(Vector3))
                    {
                        Vector3 oldvalue = (Vector3)f.GetValue(cameraeffect.effect);
                        Vector3 newvalue = EditorGUILayout.Vector3Field(f.Name, oldvalue);
                        if (newvalue != oldvalue)
                        {
                            f.SetValue(cameraeffect.effect, newvalue);
                        }
                    }
                    if (f.FieldType == typeof(Vector4))
                    {
                        Vector4 oldvalue = (Vector4)f.GetValue(cameraeffect.effect);
                        Vector4 newvalue = EditorGUILayout.Vector4Field(f.Name, oldvalue);
                        if (newvalue != oldvalue)
                        {
                            f.SetValue(cameraeffect.effect, newvalue);
                        }
                    }
                    if (f.FieldType == typeof(Color))
                    {
                        Color oldvalue = (Color)f.GetValue(cameraeffect.effect);
                        Color newvalue = EditorGUILayout.ColorField(f.Name, oldvalue);
                        if (newvalue != oldvalue)
                        {
                            f.SetValue(cameraeffect.effect, newvalue);
                        }
                    }
                    if (f.FieldType == typeof(Texture))
                    {
                        Texture oldvalue = (Texture)f.GetValue(cameraeffect.effect);
                        Texture newvalue = (Texture)EditorGUILayout.ObjectField(f.Name,oldvalue,typeof(Texture),false);
                        if (newvalue != oldvalue)
                        {
                            f.SetValue(cameraeffect.effect, newvalue);
                        }
                    }
                }
            }
        }

        effectCameraGameObject.ApplyModifiedProperties();
    }
}
