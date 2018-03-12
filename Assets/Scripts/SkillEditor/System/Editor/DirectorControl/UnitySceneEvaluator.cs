using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitySceneEvaluator
{
    private static int Comparison(KeyValuePair<float, Transform> x, KeyValuePair<float, Transform> y)
    {
        int num = 0;
        if (x.Key < y.Key)
        {
            return 1;
        }
        if (x.Key > y.Key)
        {
            num = -1;
        }
        return num;
    }

    public static List<Transform> GetHighestRankedGameObjects(int amount)
    {
        List<Transform> list = new List<Transform>();
        List<KeyValuePair<float, Transform>> list2 = new List<KeyValuePair<float, Transform>>();
        foreach (Transform transform in GameObject.FindObjectsOfType<Transform>())
        {
            float score = GetScore(transform.gameObject);
            KeyValuePair<float, Transform> item = new KeyValuePair<float, Transform>(score, transform);
            list2.Add(item);
        }
        list2.Sort(new Comparison<KeyValuePair<float, Transform>>(UnitySceneEvaluator.Comparison));
        foreach (KeyValuePair<float, Transform> pair2 in list2)
        {
            if (list.Count < amount)
            {
                list.Add(pair2.Value);
            }
        }
        return list;
    }

    public static float GetScore(GameObject gameObject)
    {
        bool flag = false;
        if (gameObject.GetComponent<Animation>() != null)
        {
            flag = true;
        }
        if (gameObject.GetComponent<AudioSource>() != null)
        {
            flag = true;
        }
        if (gameObject.GetComponent<Camera>() != null)
        {
            flag = true;
        }
        if (gameObject.GetComponent<Light>() != null)
        {
            flag = true;
        }
        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            flag = true;
        }
        return (((0f + (gameObject.isStatic ? 0f : 20f)) + ((gameObject.tag == "Untagged") ? 0f : 20f)) + (flag ? 20f : 0f));
    }
}

