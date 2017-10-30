using UnityEngine;

namespace TimeLine
{

    [ExecuteInEditMode]
    public abstract class TimelineItem : MonoBehaviour
    {

        [SerializeField]
        protected float firetime = 0f;
        float FIXTIME = 1f / 15f;
#if UNITY_EDITOR

        [HideInInspector, SerializeField]
        protected int frameCount = 0;
        public int FrameCount
        {
            get { return this.frameCount; }
            set
            {
                frameCount = value;
                firetime = FIXTIME * frameCount;
                if (frameCount < 0f)
                {
                    firetime = 0f;
                    frameCount = 0;
                }
            }
        }
#endif

        //触发时间
        public float Firetime
        {
            get { return this.firetime; }
            set
            {
                firetime = value;
#if UNITY_EDITOR
                frameCount = (int)(firetime / FIXTIME);
#endif
                if (firetime < 0f)
                {
                    firetime = 0f;
#if UNITY_EDITOR
                    frameCount = 0;
#endif
                }
            }
        }

       //时间线初始化
        public virtual void Initialize() { }

        //时间线停止
        public virtual void Stop() { }

        //获取捆绑的对象 用于动态换装
        public virtual GameObject[] GetTiedGameObject() { return null; }
        //设置捆绑的对象
        public virtual void SetTieGameObject(GameObject obj,int index) { }

        //新创建item时调用 重写设置默认值
        public virtual void SetDefaults() { }

        //新创建item时调用
        public virtual void SetDefaults(UnityEngine.Object PairedItem) { }

   
        //获取时间轨道
        public TimelineTrack TimelineTrack
        {
            get
            {
                TimelineTrack track = null;
                if (transform.parent != null)
                {
                    track = base.transform.parent.GetComponentInParent<TimelineTrack>();
                    if (track == null)
                    {
                        Debug.LogError("No TimelineTrack found on parent!", this);
                    }
                }
                else
                {
                    Debug.LogError("Timeline Item has no parent!", this);
                }
                return track;
            }
        }
    }
}