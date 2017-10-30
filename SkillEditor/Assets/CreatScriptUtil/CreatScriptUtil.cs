using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using UnityEditor.ProjectWindowCallback;
using System.Text.RegularExpressions;

public class CreatScriptUtil
{ 
    public static string EditorPath()
    {
        string[] assets = UnityEditor.AssetDatabase.FindAssets("CreatScriptUtil"); // Something relatively unique
        string correctEditorPath = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[0]);
        // int subFolderIndex = correctEditorPath.LastIndexOf("CreatScriptUtil/");
        string editorPath = correctEditorPath;// correctEditorPath.Substring(0, subFolderIndex);
        return editorPath;
    }

    [MenuItem("Assets/Create/Action Script", false, 81)]
    public static void CreatNewAction()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<ActionCreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/New Action.cs",
        null,
        EditorPath() + "/Template/Action_Template.cs.txt");
    }
    [MenuItem("Assets/Create/Command Script", false, 82)]
    public static void CreatNewCommand()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<CommandCreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/New Command.cs",
        null,
        EditorPath()+"/Template/Command_Template.cs.txt");
    }

    [MenuItem("Assets/Create/Mediator Script", false, 81)]
    public static void CreatNewMediator()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<MediatorCreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/New Mediator.cs",
        null,
        EditorPath() + "/Template/Mediator_Template.cs.txt");
    }
    [MenuItem("Assets/Create/View Script", false, 83)]
    public static void CreatNewView()
    {
        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0,
        ScriptableObject.CreateInstance<ViewCreateScriptAsset>(),
        GetSelectedPathOrFallback() + "/New View.cs",
        null,
        EditorPath() + "/Template/View_Template.cs.txt");
    }
    class MediatorCreateScriptAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }

        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            text = Regex.Replace(text, "#SCRIPTNAME#", fileNameWithoutExtension);
 
            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }

    }
    class ActionCreateScriptAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }

        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            //string code = "";
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            StreamReader codestreamReader = new StreamReader(EditorPath() + "/Template/Action_Template.cs.txt");
           // code = codestreamReader.ReadToEnd();
            codestreamReader.Close();


            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            string friendname = ""+fileNameWithoutExtension[0];
            for (int i = 1; i < fileNameWithoutExtension.Length; i++)
            {
               int cc = fileNameWithoutExtension[i].GetHashCode();
                if (cc >= 65 && cc <= 90)
                {
                    friendname = friendname + " " + fileNameWithoutExtension[i];
                }
                else
                {
                    friendname = friendname  + fileNameWithoutExtension[i];
                }
            }
            text = Regex.Replace(text, "#SCRIPTNAME#", fileNameWithoutExtension);
            text = Regex.Replace(text, "#FRIENDNAME#", friendname);
            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }

    }

    class CommandCreateScriptAsset : EndNameEditAction
    { 
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }

        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string code = "";
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();

            StreamReader codestreamReader = new StreamReader(EditorPath() + "/Template/code.lua");
            code = codestreamReader.ReadToEnd();
            codestreamReader.Close();
            

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            text = Regex.Replace(text, "#SCRIPTNAME#", fileNameWithoutExtension);
            text = Regex.Replace(text, "#EXECUTECODE#", code);
            //string text2 = Regex.Replace(fileNameWithoutExtension, " ", string.Empty);
            //text = Regex.Replace(text, "#SCRIPTNAME#", text2);
            //if (char.IsUpper(text2, 0))
            //{
            //    text2 = char.ToLower(text2[0]) + text2.Substring(1);
            //    text = Regex.Replace(text, "#SCRIPTNAME_LOWER#", text2);
            //}
            //else
            //{
            //    text2 = "my" + char.ToUpper(text2[0]) + text2.Substring(1);
            //    text = Regex.Replace(text, "#SCRIPTNAME_LOWER#", text2);
            //}
            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }

    }
    class ViewCreateScriptAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            UnityEngine.Object o = CreateScriptAssetFromTemplate(pathName, resourceFile);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }

        internal static UnityEngine.Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string fullPath = Path.GetFullPath(pathName);
            StreamReader streamReader = new StreamReader(resourceFile);
            string text = streamReader.ReadToEnd();
            streamReader.Close();

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            text = Regex.Replace(text, "#SCRIPTNAME#", fileNameWithoutExtension);
            text = Regex.Replace(text, "#UPDATEEVENT#", "");
            text = Regex.Replace(text, "#INPITEVENTt#", "");
            bool encoderShouldEmitUTF8Identifier = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier, throwOnInvalidBytes);
            bool append = false;
            StreamWriter streamWriter = new StreamWriter(fullPath, append, encoding);
            streamWriter.Write(text);
            streamWriter.Close();
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
        }

    }
    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
}    

