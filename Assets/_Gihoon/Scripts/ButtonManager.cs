using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
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

        [SerializeField][Tooltip("")] Sprite[] contentButtonImges = new Sprite[4];
        [SerializeField][Tooltip("")] Sprite[] selectedContentButtonImges = new Sprite[4];
        //[SerializeField][Tooltip("")] Sprite[] panelButtonImges = new Sprite[6];

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
                        // Section Button Setting
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
            ViewManager.Instance?.InActivePanel(EPanelType.MAIN);
            ViewManager.Instance?.InActivePanel(EPanelType.CONTENT);

            // Btn Setting
            InActiveBtnPanel(EBtnPanelType.SECTION);
            InActiveBtnPanel(EBtnPanelType.CONTENT);

            // Panel Setting
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

            // Panel Setting
            EPanelType target = EPanelType.SELECTION;
            MoveTo(target);

            // Scroll View Setting
            ContentManager.Instance.TurnOffScrollView();
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

        // 버튼 활성화
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

        // 버튼 비 활성화
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
            ContentManager.Instance.MoveTo(EContentType.FIRST);
            SetSelectedContentBtnImg(EContentBtnType.CONTENT_FIRST);
        }

        public void MoveToSecondContent()
        {
            ContentManager.Instance.MoveTo(EContentType.SECOND);
            SetSelectedContentBtnImg(EContentBtnType.CONTENT_SECOND);
        }

        public void MoveToThirdContent()
        {
            ContentManager.Instance.MoveTo(EContentType.THIRD);
            SetSelectedContentBtnImg(EContentBtnType.CONTENT_THIRD);
        }

        public void MoveToFourthContent()
        {
            ContentManager.Instance.MoveTo(EContentType.FOURTH);
            SetSelectedContentBtnImg(EContentBtnType.CONTENT_FOURTH);
        }

        #endregion

        private void SetSelectedContentBtnImg(EContentBtnType btnType)
        {
            if(contentButtons.Count <= 0)
            {
                Debug.LogWarning("Content Btn Container is empty.");
                return;
            }

            ResetContentBtnImg();

            // Target Setting
            Image targetImg = contentButtons[(int)btnType].GetComponent<Image>();
            Sprite selectedBtnImg = selectedContentButtonImges[(int)btnType];
            targetImg.sprite = selectedBtnImg;
        }

        private void ResetContentBtnImg()
        {
            for(int i = 0; i < contentButtons.Count; ++i)
            {
                Image targetImg = contentButtons[i].GetComponent<Image>();
                Sprite selectedBtnImg = contentButtonImges[i];
                targetImg.sprite = selectedBtnImg;
            }
        }

    }
}