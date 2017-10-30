using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFilterPack_Blur_RadialCustom : MonoBehaviour
{
    public float startDist = 1f;
    public float endDist = 1f;

    public float startStrength = 2.2f;
    public float endStrendth = 1f;

    public float fadeInTime = 1f;
    public float fadeOutTime = 1f;

    public float delayTime = 1f;


    public float tmpDist = 1f;
    public float tmpStrength = 2.2f;

    public bool enableFadeMode = false;

    //Inspector面板上直接拖入  
    protected Material mMaterial = null;

    protected void CreateMaterial(Shader shader)
    {
        if (shader.isSupported == false)
        {
            mMaterial = null;
        }
        else
        {
            mMaterial = new Material(shader);
            mMaterial.hideFlags = HideFlags.DontSave;
        }
    }

    void Awake()
    {
        CreateMaterial(Shader.Find("Custom/RadialBlur"));
    }

    void OnEnable()
    {
        if (enableFadeMode && gameObject.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeValue());
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator ChangeValue()
    {
        tmpDist = startDist;
        tmpStrength = startStrength;
        float deltaDistChange = (endDist - startDist) / fadeInTime;
        float deltaStrengthChange = (endStrendth - startStrength) / fadeInTime;
        while (tmpDist < endDist || tmpStrength < endStrendth)
        {
            tmpDist += Time.deltaTime * deltaDistChange;
            tmpStrength += Time.deltaTime * deltaStrengthChange;
            yield return null;
        }

        yield return new WaitForSeconds(delayTime);

        deltaDistChange = (endDist - startDist) / fadeOutTime;
        deltaStrengthChange = (endStrendth - startStrength) / fadeOutTime;
        while (tmpDist > startDist || tmpStrength > startStrength)
        {
            tmpDist -= Time.deltaTime * deltaDistChange;
            tmpStrength -= Time.deltaTime * deltaStrengthChange;
            yield return null;
        }

        enabled = false;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        UnityEngine.Profiling.Profiler.BeginSample("GAUSSBLUR PROFILE");
        if (mMaterial != null)
        {
            mMaterial.SetFloat("_SampleDist", tmpDist);
            mMaterial.SetFloat("_SampleStrength", tmpStrength);
            Graphics.Blit(source, destination, mMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
        UnityEngine.Profiling.Profiler.EndSample();
    }
}
