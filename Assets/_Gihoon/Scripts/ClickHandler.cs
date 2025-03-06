using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GH
{
    public class ClickHandler : MonoBehaviour, IPointerClickHandler
    {
        [Header("Essential Property")]
        [SerializeField][ReadOnly] private int touchCnt = 0;
        [SerializeField][ReadOnly] private bool bQuit = false;
        [SerializeField][ReadOnly] private float resetTimer = 0.0f;
        [SerializeField][ReadOnly] private float resetTime = 10.0f;

        public int TouchCnt
        {
            get { return touchCnt; }
            set { touchCnt = value; }
        }

        public bool QuitFlag
        {
            get { return bQuit; }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ++touchCnt;
            if(touchCnt >= 10)
            {
                bQuit = true;
            }
        }

        public void Update()
        {
            QuitTimer();
        }

        private void QuitTimer()
        {
            if (0 < touchCnt)
            {
                resetTimer += Time.deltaTime;
                if (resetTime < resetTimer)
                {
                    resetTimer = 0.0f;
                    touchCnt = 0;
                }
            }
        }

    }
}
