using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace GH
{
    public enum EBtnPanelName
    {
        TOP,
        CENTER,
        MAX_CNT,
        NONE,
    }

    public enum EBtnName
    {
        TOP_FIRST,      
        TOP_SECOND,
        TOP_THIRD,
        TOP_FOURTH,
        TOP_FIFTH,
        CENTER_FIRST,   
        CENTER_SECOND,
        CENTER_THIRD,
        CENTER_FOURTH,
        CENTER_FIFTH,
        CENTER_SIXTH,
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
        [SerializeField][ReadOnly] private string[] btnPanelNames = null;
        [SerializeField][ReadOnly] private List<GameObject> buttonPanels = new List<GameObject>();
        [SerializeField][ReadOnly] private List<string> buttonNames = null;
        [SerializeField][ReadOnly] private List<GameObject> buttons = new List<GameObject>();

        private void CustomAwake()
        {
            btnPanelNames = new string[] { "panel-buttons-top", "panel-buttons-center" };

            string[] names =
            {
                "btn-first",
                "btn-second",
                "btn-third",
                "btn-fourth",
                "btn-fifth",
                "btn-sixth",
            };
            buttonNames = new List<string>(names);

            // Panel Setting
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

            // btn Setting
            foreach (GameObject panel in buttonPanels)
            {
                foreach (string btnName in buttonNames)
                {
                    GameObject target = panel.GetComponent<Transform>().Find(btnName)?.gameObject;
                    if (null == target)
                    {
                        Debug.Log(btnName + " not found. (" + panel.name + ")");
                    }
                    else
                    {
                        buttons.Add(target);
                    }
                }
            }
        }

        public void Start()
        {
            //
        }

        public void MoveToIntro()
        {
            // Btn Setting
            InActiveBtnPanel(EBtnPanelName.TOP);
            InActiveBtnPanel(EBtnPanelName.CENTER);

            EPanelName target = EPanelName.INTRO;
            MoveTo(target);

        }

        public void MoveToMain()
        {
            ViewManager.Instance?.ActivePanel(EPanelName.MAIN);
            ViewManager.Instance?.ActivePanel(EPanelName.CONTENT);
            // Btn Setting
            InActiveBtnPanel(EBtnPanelName.TOP);
            ActiveBtnPanel(EBtnPanelName.CENTER);

            EPanelName target = EPanelName.SELECTION;
            MoveTo(target);
        }

        public void MoveToFirst()
        {
            // Btn Setting
            InActiveBtnPanel(EBtnPanelName.CENTER);
            ActiveBtnPanel(EBtnPanelName.TOP);

            EPanelName target = EPanelName.FIRST;
            Debug.Log(target.ToString());
            MoveTo(target);
        }

        public void MoveToSecond()
        {
            // Btn Setting
            InActiveBtnPanel(EBtnPanelName.CENTER);
            ActiveBtnPanel(EBtnPanelName.TOP);

            EPanelName target = EPanelName.SECOND;
            Debug.Log(target.ToString());
            MoveTo(target);
        }

        public void MoveToThird()
        {
            // Btn Setting
            InActiveBtnPanel(EBtnPanelName.CENTER);
            ActiveBtnPanel(EBtnPanelName.TOP);

            EPanelName target = EPanelName.THIRD;
            Debug.Log(target.ToString());
            MoveTo(target);
        }

        public void MoveToFourth()
        {
            // Btn Setting
            InActiveBtnPanel(EBtnPanelName.CENTER);
            ActiveBtnPanel(EBtnPanelName.TOP);

            EPanelName target = EPanelName.FOURTH;
            Debug.Log(target.ToString());
            MoveTo(target);
        }

        public void MoveToFifth()
        {
            // Btn Setting
            InActiveBtnPanel(EBtnPanelName.CENTER);
            ActiveBtnPanel(EBtnPanelName.TOP);

            EPanelName target = EPanelName.FIFTH;
            Debug.Log(target.ToString());
            MoveTo(target);
        }

        public void MoveToSixth()
        {
            // Btn Setting
            InActiveBtnPanel(EBtnPanelName.CENTER);
            ActiveBtnPanel(EBtnPanelName.TOP);

            EPanelName target = EPanelName.SIXTH;
            Debug.Log(target.ToString());
            MoveTo(target);
        }

        private void MoveTo(EPanelName panel)
        {
            ViewManager.Instance?.InActivePanel(ViewManager.Instance.CurPanel);
            ViewManager.Instance?.ActivePanel(panel);
            ViewManager.Instance.CurPanel = panel;
        }

        public bool ActiveBtnPanel(EBtnPanelName panel)
        {
            GameObject target = buttonPanels[(int)panel];
            if(null == target)
            {
                Debug.Log("Button Panel (" + panel.ToString() + ") not found");
                return false;
            }
            target.SetActive(true);
            return true;
        }

        public bool InActiveBtnPanel(EBtnPanelName panel)
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
    }
}
