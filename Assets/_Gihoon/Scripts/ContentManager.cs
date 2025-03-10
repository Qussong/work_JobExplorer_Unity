using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace GH
{
    public enum EContentType
    {
        FIRST,  // ��������
        SECOND, // ����Ž�� �� �غ�
        THIRD,  // ������ȯ �� ��ǥ
        FOURTH, // �� ������ �� ȸ��Ư��

        MAX_CNT,
        NONE
    }

    public class ContentManager : MonoBehaviour
    {
        /// <summary>
        /// ���������������� Singleton ����������������
        /// </summary>
        #region Singleton

        private static ContentManager instance = null;
        private static readonly object lockKey = new object();

        public static ContentManager Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (lockKey)
                    {
                        instance = FindAnyObjectByType<ContentManager>();
                        if (null == instance)
                        {
                            GameObject obj = new GameObject("ContentManager");
                            instance = obj.AddComponent<ContentManager>();
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
        //[SerializeField][ReadOnly] private Image contentImg = null; // Content �� �׷��� Img Component
        [SerializeField][ReadOnly] public EContentType curContent = EContentType.NONE;
        private Sprite[][] sprites = null;  // 2���� �迭�� Ispector â�� ��Ÿ���� ����
        [SerializeField][Tooltip("")] private Sprite[] firstContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] secondContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] thirdContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] fourthContentSprites = new Sprite[6];

        private GameObject scrollContainer = null;
        private ScrollRect contentScrollRect = null;
        private GameObject scrollBar = null;
        private GameObject scrollHand = null;

        public ScrollRect ContentScrollRect
        {
            get { return contentScrollRect; }
        }

        private void CustomAwake()
        {
            sprites = new Sprite[][]
            {
                firstContentSprites,
                secondContentSprites,
                thirdContentSprites,
                fourthContentSprites,
            };

            string scrollObjName = "scroll-content";
            scrollContainer = GameObject.Find(scrollObjName);
            if(null == scrollContainer)
            {
                Debug.LogError("Scroll Content not found.");
                return;
            }

            contentScrollRect = scrollContainer?.GetComponent<ScrollRect>();
            if (null == contentScrollRect)
            {
                Debug.LogError("Scroll Rect not found.");
                return;
            }

            string scrollHandName = "img-scrollHand";
            string scrollBarName = "img-scrollBar";
            scrollBar = scrollContainer.GetComponent<Transform>().Find(scrollBarName)?.gameObject;
            scrollHand = scrollContainer.GetComponent<Transform>().Find(scrollHandName)?.gameObject;
            if(null == scrollHand || null == scrollBar)
            {
                Debug.LogError("Scroll Component not found.");
                return;
            }

            // add Scroll Hand Touch Component
            scrollHand.AddComponent<ScrollHandler>();

        }

        public void Start()
        {
            if (null != contentScrollRect)
            {
                contentScrollRect.onValueChanged.AddListener(OnScrollChanged);
            }

            scrollHand.GetComponent<RectTransform>().anchoredPosition = new Vector2(400.0f, 100.0f);
            scrollBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(400.0f, 100.0f);
        }

        public void MoveTo(EContentType contentType)
        {
            // Scroll View Turn On
            TurnOnScrollViewer();
            // Scroll Bar Hand Init
            contentScrollRect.verticalNormalizedPosition = 1.0f; // Scroll Hand �� ���� ���� �̵�
            contentScrollRect.onValueChanged.Invoke(contentScrollRect.normalizedPosition);

            SetContentImg(contentType);
            curContent = contentType;
        }

        public void SetContentImg(EContentType contentType)
        {
            /// <summary>
            /// 
            /// [��ũ�� ���� ����]
            /// Scroll View
            /// ������ Viewport
            /// ��    ������ Content
            /// 
            /// </summary>

            if (null != contentScrollRect)
            {
                GameObject content = contentScrollRect.content.gameObject;
                Image img = content.GetComponent<Image>();

                EPanelType curPanel = ViewManager.Instance.CurPanel;
                int curPanelIdx = (int)(curPanel - EPanelType.FIRST);
                int contentIdx = (int)contentType;

                Sprite targetSprite = sprites[contentIdx][curPanelIdx];
                img.sprite = targetSprite;
                img.SetNativeSize();

                float spriteImgHeight = targetSprite.rect.height;
                AutoHideScroller(spriteImgHeight);
            }

        }

        #region Scroll Setting

        private void OnScrollChanged(Vector2 scrollPos)
        {
            float verticalPosRatio = contentScrollRect.verticalNormalizedPosition;
            //Debug.Log("Scrolled! Vertical : " + verticalPosRatio);

            RectTransform scrollBarRectTransform = scrollBar.GetComponent<RectTransform>();
            float height = scrollBarRectTransform.rect.height * scrollBarRectTransform.localScale.y;
            Vector2 pivot = scrollBarRectTransform.localPosition;

            scrollHand.GetComponent<RectTransform>().localPosition = new Vector2(pivot.x, pivot.y - height / 2 + verticalPosRatio * height);
        }

        public void TurnOnScrollViewer()
        {
            if (null == contentScrollRect) return;

            GameObject scrollView = contentScrollRect.gameObject;
            if (false == scrollView.activeSelf)
            {
                scrollView.SetActive(true);
            }
        }

        public void TurnOffScrollViewer()
        {
            if (null == contentScrollRect) return;

            GameObject scrollView = contentScrollRect.gameObject;
            if (true == scrollView.activeSelf)
            {
                scrollView.SetActive(false);
            }
        }
        public void AutoHideScroller(float targetHeight)
        {
            float contentViewerHeight = scrollContainer.GetComponent<RectTransform>().rect.height;
            if(targetHeight < contentViewerHeight)
            {
                TurnOffScroller();
            }
            else
            {
                TurnOnScroller();
            }
        }

        void TurnOnScroller()
        {
            scrollBar.SetActive(true);
            scrollHand.SetActive(true);
        }

        void TurnOffScroller()
        {
            scrollBar.SetActive(false);
            scrollHand.SetActive(false);
        }

        #endregion

    }
}
