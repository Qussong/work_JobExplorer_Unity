using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GH
{
    /// <summary>
    /// 
    /// [ Panel Hirarchy ]
    /// =============================
    /// | Canvas                    |
    /// |     └ Intro               |
    /// |     └ Main                |
    /// |         └ Content         |
    /// |             └ Selection   |
    /// |             └ First       |
    /// |             └ Second      |
    /// |             └ Third       |
    /// |             └ Fourth      |
    /// |             └ Fifth       |
    /// |             └ Sixth       |
    /// =============================
    /// 
    /// </summary>

    public enum EPanelType
    {
        INTRO,
        MAIN,
        CONTENT,
        SELECTION,

        // Content
        FIRST,
        SECOND,
        THIRD,
        FOURTH,
        FIFTH,
        SIXTH,

        MAX_CNT,
        NONE
    }

    public class ViewManager : MonoBehaviour
    {

        /// <summary>
        /// ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓ Singleton ↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓
        /// </summary>
        #region Singleton

        private static ViewManager instance = null;
        private static readonly object lockKey = new object();

        public static ViewManager Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (lockKey)
                    {
                        instance = FindAnyObjectByType<ViewManager>();
                        if (null == instance)
                        {
                            GameObject obj = new GameObject("ViewManager");
                            instance = obj.AddComponent<ViewManager>();
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
        [SerializeField][ReadOnly] private List<string> panelNames = null;
        [SerializeField][ReadOnly] private List<GameObject> panels = new List<GameObject>();
        [SerializeField][ReadOnly] private EPanelType curPanel = EPanelType.NONE;

        public EPanelType CurPanel
        {
            get { return curPanel; }
            set { curPanel = value; }
        }

        public GameObject GetPanelObject(EPanelType panelType)
        {
            GameObject target = null;
            int panelIdx = (int)panelType;

            if (panels.Count > panelIdx)
            {
                target = panels[panelIdx];
            }

            return target;
        }

        private void CustomAwake()
        {
            string[] names = {
                "panel-intro",      // EPanelName:INTRO
                "panel-main",       // EPanelName::MAIN
                "panel-content",    // EPanelName::CONTENT
                "panel-selection",  // EPanelName::SELECTION

                "panel-first",      // EPanelName::FIRST
                "panel-second",     // EPanelName::SECOND
                "panel-third",      // EPanelName::THIRD
                "panel-fourth",     // EPanelName::FOURTH
                "panel-fifth",      // EPanelName::FIFTH
                "panel-sixth"       // EPanelName::SIXTH
            };

            panelNames = new List<string>(names);

            foreach (string panelName in panelNames)
            {
                GameObject targetPanel = GameObject.Find(panelName);
                if (null == targetPanel)
                {
                    Debug.LogError(panelName + " not found.");
                    return;
                }
                panels.Add(targetPanel);
            }
        }

        public void Start()
        {
            foreach (GameObject panel in panels)
            {
                panel.SetActive(false);
            }
            ActivePanel(EPanelType.INTRO);
            curPanel = EPanelType.INTRO;
        }

        public void Update()
        {
            if (Input.touchCount > 0
                || Input.GetMouseButtonDown(0))
            {
                SoundManager.Instance.PlayClickSound();
            }

        }

        public bool InActivePanel(EPanelType panelType)
        {
            GameObject panel = panels[(int)panelType];
            if (panel == null) return false;
            panel.SetActive(false);
            return true;
        }

        public bool ActivePanel(EPanelType panelType)
        {
            GameObject panel = panels[(int)panelType];
            if (panel == null) return false;
            panel.SetActive(true);
            return true;
        }

    }
}
