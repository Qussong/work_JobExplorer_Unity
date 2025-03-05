using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

namespace GH
{
    public enum EContentType
    {
        FIRST,  // 직업개요
        SECOND, // 직업탐색 및 준비
        THIRD,  // 직업현환 및 지표
        FOURTH, // 이 직업을 본 회원특성

        MAX_CNT,
        NONE
    }

    public class ContentManager : MonoBehaviour
    {
        /// <summary>
        /// ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ Singleton ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
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
                Debug.LogWarning("중복된 인스턴스가 감지되어 삭제됩니다.");
                Destroy(gameObject);
                return;
            }

            CustomAwake();
        }

        #endregion


        [Header("Essential Property")]
        [SerializeField][ReadOnly] private Image contentImg = null; // Conent 가 그려질 Img Component
        [SerializeField][ReadOnly] private EContentType curContent = EContentType.NONE;
        private Sprite[][] sprites = null;  // 2차원 배열은 Ispector 창에 나타나지 않음
        [SerializeField][Tooltip("")] private Sprite[] firstContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] secondContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] thirdContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] fourthContentSprites = new Sprite[6];

        private ScrollRect contentScrollRect = null;

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
            contentScrollRect = GameObject.Find(scrollObjName)?.GetComponent<ScrollRect>();
            if (null == contentScrollRect)
            {
                Debug.Log("Scroll Rect not found.");
                return;
            }
        }

        public void Start()
        {
            if (null != contentScrollRect)
            {
                contentScrollRect.onValueChanged.AddListener(OnScrollChanged);
            }
        }

        public void MoveTo(EContentType contentType)
        {
            // Scroll View Turn On
            TurnOnScrollView();
            // Scroll Bar Hand Init
            contentScrollRect.onValueChanged.Invoke(contentScrollRect.normalizedPosition);

            SetContentImg(contentType);
        }

        public void SetContentImg(EContentType contentType)
        {
            /// <summary>
            /// 
            /// [스크롤 뷰의 구조]
            /// Scroll View
            /// ├── Viewport
            /// │    ├── Content
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
            }

        }

        #region Scroll Setting

        private void OnScrollChanged(Vector2 scrollPos)
        {
            float verticalPosRatio = contentScrollRect.verticalNormalizedPosition;
            Debug.Log("Scrolled! Vertical : " + verticalPosRatio);

            string scrollHandName = "img-scrollHand";
            string scrollBarName = "img-scrollBar";
            GameObject scrollHand = contentScrollRect.gameObject.GetComponent<Transform>().Find(scrollHandName)?.gameObject;
            GameObject scrollBar = contentScrollRect.gameObject.GetComponent<Transform>().Find(scrollBarName)?.gameObject;

            RectTransform scrollBarRectTransform = scrollBar.GetComponent<RectTransform>();
            float height = scrollBarRectTransform.rect.height;
            Vector2 pivot = scrollBarRectTransform.localPosition;

            scrollHand.GetComponent<RectTransform>().localPosition = new Vector2(pivot.x, pivot.y - height / 2 + verticalPosRatio * height);
        }

        public void TurnOnScrollView()
        {
            if (null == contentScrollRect) return;

            GameObject scrollView = contentScrollRect.gameObject;
            if (false == scrollView.activeSelf)
            {
                scrollView.SetActive(true);
            }
        }

        public void TurnOffScrollView()
        {
            if (null == contentScrollRect) return;

            GameObject scrollView = contentScrollRect.gameObject;
            if (true == scrollView.activeSelf)
            {
                scrollView.SetActive(false);
            }
        }

        #endregion

    }
}
