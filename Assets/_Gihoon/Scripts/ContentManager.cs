using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField][ReadOnly] private Image contentImg = null; // Conent �� �׷��� Img Component
        [SerializeField][ReadOnly] private EContentType curContent = EContentType.NONE;
        //[SerializeField][Tooltip("")] private List<List<Sprite>> sprites; // 2���� �迭�� Ispector â�� ��Ÿ���� ����
        private Sprite[][] sprites = null;
        [SerializeField][Tooltip("")] private Sprite[] firstContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] secondContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] thirdContentSprites = new Sprite[6];
        [SerializeField][Tooltip("")] private Sprite[] fourthContentSprites = new Sprite[6];

        private void CustomAwake()
        {
            contentImg = gameObject.GetComponent<Image>();
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
            Sprite targetSprite = sprites[curPanelIdx][curContentIdx];
            if(null != targetSprite)
            {
                contentImg.sprite = targetSprite;
                curContent = contentType;
            }
            else
            {
                Debug.LogWarning(contentType.ToString() + " Sprite not found");
            }

            // test
            curContent = contentType;
        }

    }
}
