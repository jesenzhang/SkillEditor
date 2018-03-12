using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFilterPack_Blur_Motion : MonoBehaviour
{
    public Transform Target = null;
    [Range(0, 0.9f)]
    public float blurAlpha = 0.85f;

    public BLUR_TYPE blurType = BLUR_TYPE.CENTER;

    [Range(0, 180)]
    public float Angle = 120;
    [Range(0, 3)]
    public float Strength = 2f;

    private BLUR_TYPE oldType = BLUR_TYPE.CENTER;
    private bool reCalculTexture = false;
    private RenderTexture accumTexture;

    private Vector2 mCenter = new Vector4(1, 1);
    private Vector2 mForward = new Vector2(1, 1);

    public enum BLUR_TYPE
    {
        CENTER,
        FOWARD,
    }

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
        CreateMaterial(Shader.Find("Custom/MotionBlur"));
    }

    void OnEnable()
    {
        reCalculTexture = true;
    }

    void OnDestroy()
    {
        if (this && accumTexture != null)
        {
            Destroy(accumTexture);
            accumTexture = null;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
        if (mMaterial)
        {
            //计算初始帧
            if (accumTexture == null)
            {
                accumTexture = new RenderTexture(source.width, source.height, 0);
                accumTexture.hideFlags = HideFlags.HideAndDontSave;
                accumTexture.MarkRestoreExpected();
            }
            if (oldType != blurType)
            {
                reCalculTexture = true;
            }
            oldType = blurType;
            if (reCalculTexture)
            {
                reCalculTexture = false;
                Graphics.Blit(source, accumTexture);
            }

            //累加计算模糊图像
            mMaterial.SetFloat("_AccumOrig", 1 - blurAlpha);
            Graphics.Blit(source, accumTexture, mMaterial, 0);
            Graphics.Blit(source, accumTexture, mMaterial, 1);

            if (Target != null)
            {
                //计算角色运动方向
                Vector3 curPos = Camera.main.WorldToScreenPoint(Target.position);
                mCenter.x = curPos.x / Screen.width;
                mCenter.y = curPos.y / Screen.height;
                mMaterial.SetVector("_Center", mCenter);

                if (blurType == BLUR_TYPE.FOWARD)
                {
                    Vector3 dirPos = Camera.main.WorldToScreenPoint(Target.position + Target.forward);
                    mForward.x = dirPos.x / Screen.width;
                    mForward.y = dirPos.y / Screen.height;

                    mMaterial.SetVector("_Forward", mForward);
                    mMaterial.SetFloat("_Angle", Mathf.Cos(Angle / 2 / 360 * 2 * Mathf.PI));

                    //根据角色运动方向和角度计算模糊图和清晰图
                    Graphics.Blit(accumTexture, destination, mMaterial, 2);
                }
                else
                {
                    mMaterial.SetFloat("_Radius", Strength);
                    //根据圆心计算模糊图和清晰图
                    Graphics.Blit(accumTexture, destination, mMaterial, 3);
                }
            }
            else if (blurType == BLUR_TYPE.CENTER)
            {
                mCenter.x = 0.5f;
                mCenter.y = 0.5f;
                mMaterial.SetFloat("_Radius", Strength);
                mMaterial.SetVector("_Center", mCenter);
                //根据圆心计算模糊图和清晰图
                Graphics.Blit(accumTexture, destination, mMaterial, 3);
            }
        }
    }
}
