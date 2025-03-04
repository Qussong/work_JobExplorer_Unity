using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace GH
{
    public class AdminManager : MonoBehaviour
    {
        [SerializeField][ReadOnly] private int touchCnt = 0;

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
            if(null == instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if(this != instance)
            {
                Debug.LogWarning("중복된 인스턴스가 감지되어 삭제됩니다.");
                Destroy(gameObject);
                return;
            }

            CustomAwake();
        }

        #endregion
        private void CustomAwake()
        {

        }

        void TouchWait()
        {

        }

        void QuitExit()
        {
            
        }
    }
}
