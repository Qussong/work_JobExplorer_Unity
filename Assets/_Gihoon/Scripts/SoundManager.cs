using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GH
{
    public class SoundManager : MonoBehaviour
    {

        /// <summary>
        /// ���������������� Singleton ����������������
        /// </summary>
        #region Singleton

        private static SoundManager instance = null;
        private static readonly object key = new object();

        public static SoundManager Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (key)
                    {
                        instance = FindAnyObjectByType<SoundManager>();
                        if (null == instance)
                        {
                            GameObject obj = new GameObject("SoundManager");
                            obj.AddComponent<SoundManager>();
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
        [NonSerialized] private AudioSource audioSource = null;
        [SerializeField] private AudioClip touchSound = null;

        private void CustomAwake()
        {
            // Audio Setting
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void PlayClickSound()
        {
            if (null == touchSound)
            {
                Debug.LogWarning("Touch Sound not set.");
                return;
            }

            audioSource.PlayOneShot(touchSound);
        }
    }
}
