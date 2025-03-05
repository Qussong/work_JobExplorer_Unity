using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace GH
{
    public enum EBtnPanelType
    {
        SECTION,
        CONTENT,

        MAX_CNT,
        NONE,
    }

    public enum ESectionBtnType
    {
        SECTION_FIRST,
        SECTION_SECOND,
        SECTION_THIRD,
        SECTION_FOURTH,
        SECTION_FIFTH,
        SECTION_SIXTH,

        MAX_CNT,
        NONE
    }

    public enum EContentBtnType
    {
        CONTENT_FIRST,
        CONTENT_SECOND,
        CONTENT_THIRD,
        CONTENT_FOURTH,

        MAX_CNT,
        NONE
    }

    public class ButtonManager : MonoBehaviour
    {
        /// <summary>
        /// ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ Singleton ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        /// </summary>
        #region Singleton

        private static ButtonManager instance = null;
        private static readonly object lockKey = new object();
        public static ButtonManager Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (lockKey)
                    {
                        instance = FindAnyObjectByType<ButtonManager>();
                        if (null == instance)
                        {
                            GameObject obj = new GameObject("ButtonManager");
                            instance = obj.AddComponent<ButtonManager>();
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
        [SerializeField][ReadOnly][Tooltip("")] private List<GameObject> buttonPanels = new List<GameObject>();
        [SerializeField][ReadOnly][Tooltip("")] private List<GameObject> sectionButtons = new List<GameObject>();
        [SerializeField][ReadOnly][Tooltip("")] private List<GameObject> contentButtons = new List<GameObject>();

        [SerializeField][Tooltip("")] Sprite[] contentButtonImges = new Sprite[5];
        [SerializeField][Tooltip("")] Sprite[] selectedContentButtonImges = new Sprite[5];
        [SerializeField][Tooltip("")] Sprite[] panelButtonImges = new Sprite[6];

        private void CustomAwake()
        {
            // Panel Setting
            string[] btnPanelNames = new string[] { "panel-buttons-section", "panel-buttons-content" };
            foreach (string name in btnPanelNames)
            {
                GameObject target = GameObject.Find(name);
                if (null == target)
                {
                    Debug.LogError(name + " not found.");
                    return;
                }
                buttonPanels.Add(target);
            }

            // Button Setting
            foreach (GameObject panel in buttonPanels)
            {
                int childCnt = panel.GetComponent<Transform>().childCount;
                for (int i = 0; i < childCnt; ++i)
                {
                    GameObject childButton = panel.GetComponent<Transform>().GetChild(i)?.gameObject;
                    if (childButton != null)
                    {
                        // Secion Button Setting
                        if (panel.name == btnPanelNames[0])
                        {
                            sectionButtons.Add(childButton);
                        }
                        // Content Button Setting
                        else if (panel.name == btnPanelNames[1])
                        {
                            contentButtons.Add(childButton);
                        }
                    }
                }
            }

        }

        // 패널 변경
        #region Panel Change

        public void MoveToIntroPanel()
        {
            // Btn Setting
            InActiveBtnPanel(EBtnPanelType.SECTION);
            InActiveBtnPanel(EBtnPanelType.CONTENT);

            EPanelType target = EPanelType.INTRO;
            MoveTo(target);
        }

        public void MoveToMainPanel()
        {
            ViewManager.Instance?.ActivePanel(EPanelType.MAIN);
            ViewManager.Instance?.ActivePanel(EPanelType.CONTENT);

            // Btn Setting
            ActiveBtnPanel(EBtnPanelType.SECTION);
            InActiveBtnPanel(EBtnPanelType.CONTENT);

            EPanelType target = EPanelType.SELECTION;
            MoveTo(target);
        }

        public void MoveToFirstPanel()
        {
            // Btn Setting
            ActiveBtnPanel(EBtnPanelType.CONTENT);
            InActiveBtnPanel(EBtnPanelType.SECTION);

            // Panel Setting
            MoveTo(EPanelType.FIRST);

            // Content Setting
            MoveToFirstContent();
        }

        public void MoveToSecondPanel()
        {
            // Btn Setting
            ActiveBtnPanel(EBtnPanelType.CONTENT);
            InActiveBtnPanel(EBtnPanelType.SECTION);

            // Panel Setting
            MoveTo(EPanelType.SECOND);

            // Content Setting
            MoveToFirstContent();
        }

        public void MoveToThirdPanel()
        {
            // Btn Setting
            ActiveBtnPanel(EBtnPanelType.CONTENT);
            InActiveBtnPanel(EBtnPanelType.SECTION);

            // Panel Setting
            MoveTo(EPanelType.THIRD);

            // Content Setting
            MoveToFirstContent();
        }

        public void MoveToFourthPanel()
        {
            // Btn Setting
            ActiveBtnPanel(EBtnPanelType.CONTENT);
            InActiveBtnPanel(EBtnPanelType.SECTION);

            // Panel Setting
            MoveTo(EPanelType.FOURTH);

            // Content Setting
            MoveToFirstContent();
        }

        public void MoveToFifthPanel()
        {
            // Btn Setting
            ActiveBtnPanel(EBtnPanelType.CONTENT);
            InActiveBtnPanel(EBtnPanelType.SECTION);

            // Panel Setting
            MoveTo(EPanelType.FIFTH);

            // Content Setting
            MoveToFirstContent();
        }

        public void MoveToSixthPanel()
        {
            // Btn Setting
            ActiveBtnPanel(EBtnPanelType.CONTENT);
            InActiveBtnPanel(EBtnPanelType.SECTION);

            // Panel Setting
            MoveTo(EPanelType.SIXTH);

            // Content Setting
            MoveToFirstContent();
        }

        private void MoveTo(EPanelType panel)
        {
            ViewManager.Instance?.InActivePanel(ViewManager.Instance.CurPanel);
            ViewManager.Instance?.ActivePanel(panel);
            ViewManager.Instance.CurPanel = panel;
        }

        #endregion

        // 버튼 패널 ON/OFF
        #region Button Panel ON/OFF

        public bool ActiveBtnPanel(EBtnPanelType panel)
        {
            GameObject target = buttonPanels[(int)panel];
            if (null == target)
            {
                Debug.Log("Button Panel (" + panel.ToString() + ") not found");
                return false;
            }
            target.SetActive(true);
            return true;
        }

        public bool InActiveBtnPanel(EBtnPanelType panel)
        {
            GameObject target = buttonPanels[(int)panel];
            if (null == target)
            {
                Debug.Log("Button Panel (" + panel.ToString() + ") not found");
                return false;
            }
            target.SetActive(false);
            return true;
        }

        #endregion

        // 컨텐츠 변경
        #region Content Change

        public void MoveToFirstContent()
        {
            ContentManager.Instance.MoveTo(EContentType.FIRST, ViewManager.Instance.CurPanel);
        }

        public void MoveToSecondContent()
        {
            ContentManager.Instance.MoveTo(EContentType.SECOND, ViewManager.Instance.CurPanel);
        }

        public void MoveToThirdContent()
        {
            ContentManager.Instance.MoveTo(EContentType.THIRD, ViewManager.Instance.CurPanel);
        }

        public void MoveToFourthContent()
        {
            ContentManager.Instance.MoveTo(EContentType.FOURTH, ViewManager.Instance.CurPanel);
        }

        #endregion

    }
}