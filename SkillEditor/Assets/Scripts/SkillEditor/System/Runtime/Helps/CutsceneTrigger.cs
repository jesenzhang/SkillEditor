using System.Collections;
using UnityEngine;

namespace TimeLine
{
    /// <summary>
    /// A sample behaviour for triggering Cutscenes.
    /// </summary>
    public class CutsceneTrigger : MonoBehaviour
    {
        public StartMethod StartMethod;
        public TimelineManager Manager;
        public GameObject TriggerObject;
        public string SkipButtonName = "Jump";
        public string TriggerButtonName = "Fire1";
        public float Delay = 0f;
        private bool hasTriggered = false;

        /// <summary>
        /// When the trigger is loaded, optimize the Cutscene.
        /// </summary>
        void Awake()
        {
            if (Manager != null)
            {
                Manager.Optimize();
            }
        }

        // When the scene starts trigger the Cutscene if necessary.
        void Start()
        {
            if (StartMethod == StartMethod.OnStart && Manager != null)
            {
                hasTriggered = true;
                StartCoroutine(PlayCutscene());
            }
        }

        private IEnumerator PlayCutscene()
        {
            yield return new WaitForSeconds(Delay);
            this.Manager.Play();
        }

        void Update()
        {
            if (SkipButtonName != null || SkipButtonName != string.Empty)
            {
                // Check if the user wants to skip.
                if (Input.GetButtonDown(SkipButtonName))
                {
                    if (Manager != null && Manager.State == TimelineManager.TimeLineState.Playing)
                    {
                        Manager.Skip();
                    }
                }
            }
        }


        /// <summary>
        /// If Cutscene is setup to play on trigger, watch for the trigger event.
        /// </summary>
        /// <param name="other">The other collider.</param>
        void OnTriggerEnter(Collider other)
        {
            if (StartMethod == StartMethod.OnTrigger && !hasTriggered && other.gameObject == TriggerObject)
            {
                hasTriggered = true;
                Manager.Play();
            }
        }

        /// <summary>
        /// If Cutscene is setup to play on trigger, watch for the trigger event.
        /// </summary>
        /// <param name="other">The other collider.</param>
        void OnTriggerEnter2D(Collider2D other)
        {
            if (StartMethod == StartMethod.OnTrigger && !hasTriggered && other.gameObject == TriggerObject)
            {
                hasTriggered = true;
                Manager.Play();
            }
        }


        /// <summary>
        /// If Cutscene is setup to play on button down and on trigger, watch for the trigger event.
        /// </summary>
        /// <param name="other">The other collider.</param>
        void OnTriggerStay(Collider other)
        {
            if (StartMethod == StartMethod.OnTriggerStayAndButtonDown && !hasTriggered && other.gameObject == TriggerObject && Input.GetButtonDown(TriggerButtonName))
            {
                hasTriggered = true;
                Manager.Play();
            }
        }

        void OnTriggerStay2D(Collider2D other)
        {
            if (StartMethod == StartMethod.OnTriggerStayAndButtonDown && !hasTriggered && other.gameObject == TriggerObject && Input.GetButtonDown(TriggerButtonName))
            {
                hasTriggered = true;
                Manager.Play();
            }
        }
    }

    public enum StartMethod
    {
        OnStart,
        OnTrigger,
        OnTriggerStayAndButtonDown,
        None
    }
}