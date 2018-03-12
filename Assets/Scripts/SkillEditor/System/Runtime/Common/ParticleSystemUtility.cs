using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ParticleSystemUtility
{
    public static bool IsRoot(ParticleSystem ps)
    {
        if (ps == null)
        {
            return false;
        }

        var parent = ps.transform.parent;

        if (parent == null)
            return true;
        if (parent.GetComponent<ParticleSystem>() != null)
            return false;
        else
            return true;

    }
    public static float GetParticleDuration(string stateName, GameObject gameObject)
    {
        List<ParticleSystem> list = new List<ParticleSystem>();
        ParticleSystem[] pchild = gameObject.GetComponentsInChildren<ParticleSystem>();

        if (pchild != null)
        {
            list.AddRange(pchild);
        }
        int len = list.Count;
        for (int i = 0; i < len; i++)
        {
            if (list[i].name == stateName)
            {
                return list[i].main.duration;
            }
        }
        return 0;
    }


}