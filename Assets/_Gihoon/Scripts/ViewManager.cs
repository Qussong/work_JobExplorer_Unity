using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH
{
    public class ViewManager : MonoBehaviour
    {

        /// <summary>
        /// ���������������� Singleton ����������������
        /// </summary>

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
                Debug.LogWarning("�ߺ��� �ν��Ͻ��� �����Ǿ� �����˴ϴ�.");
                Destroy(gameObject);
                return;
            }

        }
    }
}
