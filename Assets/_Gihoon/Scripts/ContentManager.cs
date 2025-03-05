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
        //[SerializeField][Tooltip("")] private List<List<Sprite>> sprites; // 2차원 배열은 Ispector 창에 나타나지 않음
        private Sprite[][] sprites = null;
        [SerializeField][Tooltip("")] private Sprite[] firstContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] secondContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] thirdContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] fourthContentSprites = new Sprite[6];

        private void CustomAwake()
        {
            //contentImg = gameObject.GetComponent<Image>();
            sprites = new Sprite[][]
            {
                firstContentSprites,
                secondContentSprites,
                thirdContentSprites,
                fourthContentSprites,
            };

        }

        public void MoveTo(EContentType contentType, EPanelType panelType)
        {
            int curPanelIdx = (int)(panelType - EPanelType.FIRST);
            int curContentIdx = (int)contentType;

            SetContentImg(contentType);

            /*Sprite targetSprite = sprites[curContentIdx][curPanelIdx];
            if (null != targetSprite)
            {
                contentImg.sprite = targetSprite;
                curContent = contentType;
            }
            else
            {
                Debug.LogWarning(contentType.ToString() + " Sprite not found");
            }*/

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

            EPanelType curPanel = ViewManager.Instance.CurPanel;
            GameObject curPanelObj = ViewManager.Instance.GetPanelObject(curPanel);
            if (null == curPanelObj)
            {
                Debug.LogWarning(curPanel.ToString() + " Object not found");
                return;
            }

            string scrollObjName = "scroll-content";
            GameObject scrollObj = curPanelObj.GetComponent<Transform>().Find(scrollObjName)?.gameObject;
            if (null == scrollObj)
            {
                Debug.LogWarning(scrollObjName + " Object not found");
                return;
            }

            ScrollRect scrollRect = scrollObj.GetComponent<ScrollRect>();
            GameObject content = scrollRect.content.gameObject;

            Image img = content.GetComponent<Image>();

            int curPanelIdx = (int)(curPanel - EPanelType.FIRST);
            int contentIdx = (int)contentType;

            Sprite targetSprite = sprites[contentIdx][curPanelIdx];
            img.sprite = targetSprite;
            img.SetNativeSize();
        }
    }
}
