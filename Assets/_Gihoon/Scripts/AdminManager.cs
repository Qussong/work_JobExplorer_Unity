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
        /// ���������������� Singleton ����������������
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
                Debug.LogWarning("�ߺ��� �ν��Ͻ��� �����Ǿ� �����˴ϴ�.");
                Destroy(gameObject);
                return;
            }

            CustomAwake();
        }

        #endregion

        [Header("Essential Property")]
        [SerializeField][ReadOnly] private float resetTime = 5.0f;   // �������� ���� ���� ������ ����Ǳ� �������� �ִ� ��� �ð�
        [SerializeField][ReadOnly] private float quitTime = 10.0f;    // Quit ��ư�� �Է��Ҷ� ���� ������ ����Ǳ� �������� �ִ� ��� �ð�
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

            // �Է� ����
            if (true == Input.GetMouseButton(0)
                || 0 < Input.touchCount)
            {
                bReset = false;
                resetTimer = 0.0f;
            }

            // �ʱ�ȭ ���� ���� Ȯ�� �� �ʱ�ȭ
            if (resetTimer > resetTime && false == bReset)
            {
                bReset = true;
                ButtonManager.Instance.MoveToIntroPanel();
            }

            // �������� ���� ���� Ȯ�� �� ����
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

                quitBtn.transform.SetParent(parent.GetComponent<Transform>(), false); // �θ� ����
                rectQuit.sizeDelta = new Vector2(200, 100); // ũ�� ����
                imgQuit.color = new Color(1, 1, 1, 0);      // ���� ����

                RectTransform rectParent = parent.GetComponent<RectTransform>();
                if (null != rectParent)
                {
                    // �ǹ��� Left Top �������� ����
                    rectQuit.anchorMin = new Vector2(0, 1);
                    rectQuit.anchorMax = new Vector2(0, 1);
                    rectQuit.pivot = new Vector2(0, 1);
                }
            }
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false; // �����Ϳ��� ���� ���̸� ����
#else
            Application.Quit(); // ����� ���ӿ����� ����
#endif
        }

    }
}