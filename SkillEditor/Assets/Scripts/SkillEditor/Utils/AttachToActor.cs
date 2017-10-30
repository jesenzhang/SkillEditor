using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AttachToActor : MonoBehaviour {

    public GameObject Actor;
    [ExecuteInEditMode]
    private void LateUpdate()
    {
        if (Actor.transform.hasChanged)
        {
            transform.position = Actor.transform.position;
            transform.rotation = Actor.transform.rotation;
        }
    }
}
