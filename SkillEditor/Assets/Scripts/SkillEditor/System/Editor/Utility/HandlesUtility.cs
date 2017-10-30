using UnityEngine;
using UnityEditor;
using System.Collections;
 
public static class HandlesUtility
{
    static private Color xAxisColor = new Color(0.858823538f, 0.243137255f, 0.113725491f, 0.93f);
    static private Color yAxisColor = new Color(0.6039216f, 0.9529412f, 0.282352954f, 0.93f);
    static private Color zAxisColor = new Color(0.227450982f, 0.478431374f, 0.972549f, 0.93f);
    static private Color centerColor = new Color(0.8f, 0.8f, 0.8f, 0.93f);
    static private Color TangentColor = new Color(1f, 0f, 0.804f, 0.93f);
    static private Color ControlColor = new Color(1f, 0.4f, 0.10196f, 0.93f);

    static private Vector3 tPosition1 = Vector3.zero;
    static private Vector3 tPosition2 = Vector3.zero;
    static private Vector3 tinTangent1 = Vector3.zero;
    static private Vector3 toutTangent1 = Vector3.zero;
    static private Vector3 tinTangent2 = Vector3.zero;
    static private Vector3 toutTangent2 = Vector3.zero;

static public Vector3 ControlHandle(Vector3 tangent)
    {
        var snapX = EditorPrefs.GetFloat("MoveSnapX");
        var snapY = EditorPrefs.GetFloat("MoveSnapY");
        var snapZ = EditorPrefs.GetFloat("MoveSnapZ");
        var snapMove = new Vector3(snapX, snapY, snapZ);

        float handleSize = GetHandleSize(tangent);
        Color color = Handles.color;
        Handles.color = ControlColor;
        tangent = Handles.FreeMoveHandle(tangent, Quaternion.identity, handleSize * 0.15f, snapMove, new Handles.CapFunction(Handles.RectangleHandleCap));
            
        Handles.color = color;

        return tangent;
    }

    static public Vector3 TangentHandle(Vector3 tangent)
    {
        var snapX = EditorPrefs.GetFloat("MoveSnapX");
        var snapY = EditorPrefs.GetFloat("MoveSnapY");
        var snapZ = EditorPrefs.GetFloat("MoveSnapZ");
        var snapMove = new Vector3(snapX, snapY, snapZ);

        float handleSize = GetHandleSize(tangent);
        Color color = Handles.color;
        Handles.color = xAxisColor;
        tangent = Handles.Slider(tangent, Quaternion.identity * Vector3.right, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapX);
        Handles.color = yAxisColor;
        tangent = Handles.Slider(tangent, Quaternion.identity * Vector3.up, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapY);
        Handles.color = zAxisColor;
        tangent = Handles.Slider(tangent, Quaternion.identity * Vector3.forward, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapZ);
 

        Handles.color = TangentColor;
        tangent = Handles.FreeMoveHandle(tangent, Quaternion.identity, handleSize * 0.15f, snapMove, new Handles.CapFunction(Handles.RectangleHandleCap));
        Handles.color = color;

        return tangent;
    }

    static public Vector3 PositionHandle(Vector3 position, Quaternion rotation)
    {
        var snapX = EditorPrefs.GetFloat("MoveSnapX");
        var snapY = EditorPrefs.GetFloat("MoveSnapY");
        var snapZ = EditorPrefs.GetFloat("MoveSnapZ");
        var snapMove = new Vector3(snapX, snapY, snapZ);

        float handleSize = GetHandleSize(position);
        Color color = Handles.color;
        Handles.color = xAxisColor;
        position = Handles.Slider(position, rotation * Vector3.right, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapX);
        Handles.color = yAxisColor;
        position = Handles.Slider(position, rotation * Vector3.up, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapY);
        Handles.color = zAxisColor;
        position = Handles.Slider(position, rotation * Vector3.forward, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapZ);
        Handles.color = centerColor;
        position = Handles.FreeMoveHandle(position, rotation, handleSize*0.15f, snapMove, new Handles.CapFunction(Handles.RectangleHandleCap));
        Handles.color = color;

        return position;
    }

    static public float GetHandleSize(Vector3 position)
    {
        Camera current = Camera.current;
        position = Handles.matrix.MultiplyPoint(position);
        if (current)
        {
            Transform transform = current.transform;
            Vector3 position2 = transform.position;
            float z = Vector3.Dot(position - position2, transform.TransformDirection(new Vector3(0f, 0f, 1f)));
            Vector3 a = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(0f, 0f, z)));
            Vector3 b = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(1f, 0f, z)));
            float magnitude = (a - b).magnitude;
            return 40f / Mathf.Max(magnitude, 0.0001f);
        }
        return 20f;
    }

    static public void DrawKeyFrame(Vector3 tPosition1,Vector3 tinTangent1,Vector3 toutTangent1,int j,AnimationCurve Curve1, AnimationCurve Curve2, AnimationCurve Curve3)
    {
        float handleSize = HandlesUtility.GetHandleSize(tPosition1);
        Handles.Label(tPosition1 + new Vector3(0.25f * handleSize, 0.0f * handleSize, 0.0f * handleSize), "key" + j.ToString());
        Vector3 nPosition = HandlesUtility.PositionHandle(tPosition1, Quaternion.identity);
        Vector3 ninTangent1 = HandlesUtility.TangentHandle(tinTangent1 + tPosition1);
        Vector3 noutTangent1 = HandlesUtility.TangentHandle(toutTangent1 + tPosition1);

        //Handles.DrawLine(tPosition1, tinTangent1 + tPosition1);
        //Handles.Label(tinTangent1 + tPosition1 + new Vector3(0.25f * handleSize, 0.0f * handleSize, 0.0f * handleSize), "iT" + j.ToString());
        Handles.DrawLine(tPosition1, toutTangent1 + tPosition1);
        Handles.Label(toutTangent1 + tPosition1 + new Vector3(0.25f * handleSize, 0.0f * handleSize, 0.0f * handleSize), "oT" + j.ToString());

        Vector3 newTin1 = (ninTangent1 - tPosition1);
        Vector3 newTout1 = (noutTangent1 - tPosition1);

        if (tPosition1 != nPosition || ninTangent1 != newTin1 || noutTangent1 != newTout1)
        {
            Keyframe nkey1 = new Keyframe(Curve1.keys[j].time, nPosition.x, newTout1.x, newTout1.x);
            Curve1.MoveKey(j, nkey1);
            Keyframe nkey2 = new Keyframe(Curve2.keys[j].time, nPosition.y, newTout1.y, newTout1.y);
            Curve2.MoveKey(j, nkey2);
            Keyframe nkey3 = new Keyframe(Curve3.keys[j].time, nPosition.z, newTout1.z, newTout1.z);
            Curve3.MoveKey(j, nkey3);
        }
    }

    static public void DrawCurvePath(AnimationCurve Curve1, AnimationCurve Curve2 ,AnimationCurve Curve3)
    { 
        for (int j = 0; j < Curve1.keys.Length - 1; j++)
        {
            Keyframe key1x = Curve1.keys[j];
            Keyframe key1y = Curve2.keys[j];
            Keyframe key1z = Curve3.keys[j];
            Keyframe key2x = Curve1.keys[j + 1];
            Keyframe key2y = Curve2.keys[j + 1];
            Keyframe key2z = Curve3.keys[j + 1];

            //绘制关键帧之间的曲线
            float time1 = key1x.time;
            float time2 = key2x.time;
            int N = 20;
            float gap = (time2 - time1) / N;
            for (float t = time1; t < time2; t += gap)
            {
                tPosition1.x = Curve1.Evaluate(t);
                tPosition1.y = Curve2.Evaluate(t);
                tPosition1.z = Curve3.Evaluate(t);
                tPosition2.x = Curve1.Evaluate(t + gap);
                tPosition2.y = Curve2.Evaluate(t + gap);
                tPosition2.z = Curve3.Evaluate(t + gap);
            
                Handles.color = Color.red;
                Handles.DrawLine(tPosition1, tPosition2);
                Handles.color = Color.white;
            }

            tPosition1.x = key1x.value;
            tPosition1.y = key1y.value;
            tPosition1.z = key1z.value;
            tinTangent1.x = key1x.inTangent;
            tinTangent1.y = key1y.inTangent;
            tinTangent1.z = key1z.inTangent;
            toutTangent1.x = key1x.outTangent;
            toutTangent1.y = key1y.outTangent;
            toutTangent1.z = key1z.outTangent;
            tPosition2.x = key2x.value;
            tPosition2.y = key2y.value;
            tPosition2.z = key2z.value;
            tinTangent2.x = key2x.inTangent;
            tinTangent2.y = key2y.inTangent;
            tinTangent2.z = key2z.inTangent;
            toutTangent2.x = key2x.outTangent;
            toutTangent2.y = key2y.outTangent;
            toutTangent2.z = key2z.outTangent;

            DrawKeyFrame(tPosition1, tinTangent1, toutTangent1, j, Curve1, Curve2, Curve3);
           
            if (j == Curve1.length - 2)
            {
                DrawKeyFrame(tPosition2, tinTangent2, toutTangent2, j+1, Curve1, Curve2, Curve3);
                
            }
              
        }
    }
}
