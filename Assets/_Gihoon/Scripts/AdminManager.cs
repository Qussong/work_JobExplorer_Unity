using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GH
{
    public class AdminManager : MonoBehaviour
    {
        /// <summary>
        /// ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ Singleton ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        /// </summary>
        #region Singleton

        private static AdminManager instance = null;
        private static readonly object lockKey = new object();

        public static AdminManager Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (lockKey)
                    {
                        instance = FindAnyObjectByType<AdminManager>();
                        if (null == instance)
                        {
                            GameObject obj = new GameObject("AdminManager");
                            instance = obj.AddComponent<AdminManager>();
                            DontDestroyOnLoad(obj);
                        }
                    }
                }

                return instance;
            }
        }

        public void Awake()
        {
            if (null == instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (this != instance)
            {
                Debug.LogWarning("중복된 인스턴스가 감지되어 삭제됩니다.");
                Destroy(gameObject);
                return;
            }

            CustomAwake();
        }

        #endregion

        [Header("Essential Property")]
        [SerializeField][ReadOnly] private float resetTime = 5.0f;   // 컨텐츠에 대한 다음 동작이 실행되기 전까지의 최대 대기 시간
        [SerializeField][ReadOnly] private float quitTime = 10.0f;    // Quit 버튼을 입력할때 다음 동작이 실행되기 전까지의 최대 대기 시간
        [SerializeField][ReadOnly] private float resetTimer = 0.0f;
        private bool bReset = false;
        ClickHandler clickHandle = null;

        public float ResetTime
        {
            get { return resetTime; }
        }

        public float QuitTime
        {
            get { return quitTime; }
        }

        private void CustomAwake()
        {
            SetQuitButton();
        }

        public void Update()
        {
            resetTimer += Time.deltaTime;

            // 입력 감지
            if (true == Input.GetMouseButton(0)
                || 0 < Input.touchCount)
            {
                bReset = false;
                resetTimer = 0.0f;
            }

            // 초기화 조건 여부 확인 및 초기화
            if (resetTimer > resetTime && false == bReset)
            {
                bReset = true;
                ButtonManager.Instance.MoveToIntroPanel();
            }

            // 종료조건 충족 여부 확인 및 종료
            if (null != clickHandle)
            {
                if (true == clickHandle.QuitFlag)
                {
                    QuitGame();
                }
            }

        }

        private void SetQuitButton()
        {
            GameObject parent = GameObject.Find("panel-canvas");
            if (null != parent)
            {
                GameObject quitBtn = new GameObject("QuitButton");
                RectTransform rectQuit = quitBtn.AddComponent<RectTransform>();
                Image imgQuit = quitBtn.AddComponent<Image>();
                clickHandle = quitBtn.AddComponent<ClickHandler>();

                quitBtn.transform.SetParent(parent.GetComponent<Transform>(), false); // 부모 설정
                rectQuit.sizeDelta = new Vector2(200, 100); // 크기 설정
                imgQuit.color = new Color(1, 1, 1, 0);      // 색상 설정

                RectTransform rectParent = parent.GetComponent<RectTransform>();
                if (null != rectParent)
                {
                    // 피벗도 Left Top 기준으로 변경
                    rectQuit.anchorMin = new Vector2(0, 1);
                    rectQuit.anchorMax = new Vector2(0, 1);
                    rectQuit.pivot = new Vector2(0, 1);
                }
            }
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false; // 에디터에서 실행 중이면 중지
#else
            Application.Quit(); // 빌드된 게임에서는 종료
#endif
        }

    }
}