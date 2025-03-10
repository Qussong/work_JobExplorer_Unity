using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GH
{
    public class ScrollHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Essential Property")]
        [SerializeField][Tooltip("")] private Image uiImg = null;
        private RectTransform scrollHandRectTransform = null;
        private RectTransform scrollBarRectTransform = null;

        [SerializeField][ReadOnly] float scrollRange = 0.0f;
        [SerializeField][ReadOnly] float topLimit = 0.0f;
        [SerializeField][ReadOnly] float bottomLimit = 0.0f;

        public void Awake()
        {
            uiImg = gameObject.GetComponent<Image>();
            if(null == uiImg)
            {
                Debug.LogError("Image not found.");
                return;
            }

            scrollHandRectTransform = GetComponent<RectTransform>();
            if (null == scrollHandRectTransform)
            {
                Debug.LogError("Scroll-Hand RectTransform not found.");
                return;
            }
            //scrollerRectTransform.localPosition = Vector3.zero;    // 부모기준 상대적인 위치 
            //scrollerRectTransform.anchoredPosition = Vector3.zero; // 부모의 pivot 과 anchor 기준으로 상대적인 위치
            // anchor : 부모의 RectTransform 의 기준
            // pivot : UI요소 자기 자신의 중심점(기준)

            string scrollBarName = "img-scrollBar";
            Transform parentTransform = gameObject.transform.parent;
            GameObject scrollBar = parentTransform.Find(scrollBarName)?.gameObject;
            scrollBarRectTransform = scrollBar.GetComponent<RectTransform>();
            if(null == scrollBarRectTransform)
            {
                Debug.LogError("Scroll-Bar RectTransform not found.");
                return;
            }

        }

        public void Start()
        {
            scrollRange = scrollBarRectTransform.rect.height * scrollBarRectTransform.localScale.y;
            topLimit = scrollBarRectTransform.anchoredPosition.y + scrollRange / 2.0f;
            bottomLimit = scrollBarRectTransform.anchoredPosition.y - scrollRange / 2.0f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log("Pointer Down");
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Debug.Log("Drag");

            Vector2 curLocalPos = scrollHandRectTransform.anchoredPosition;
            Canvas canvas = GameObject.Find("Canvas")?.gameObject.GetComponent<Canvas>();
            if(null == canvas)
            {
                Debug.Log("Canvas not found.");
                return;
            }

            float deltaY = eventData.delta.y / canvas.scaleFactor;
            float newY = curLocalPos.y + deltaY;
            newY = Mathf.Clamp(newY, bottomLimit, topLimit);
            scrollHandRectTransform.anchoredPosition = new Vector2(curLocalPos.x, newY);

            float curPosRatio = (newY - bottomLimit) / scrollRange;
            ScrollControl(curPosRatio);

            //if (curLocalPos.y <= topLimit && curLocalPos.y >= bottomLimit)
            //{
            //    scrollHandRectTransform.anchoredPosition += new Vector2(0.0f, eventData.delta.y / canvas.scaleFactor);
            //}
            //else
            //{
            //    scrollHandRectTransform.anchoredPosition = new Vector2(curLocalPos.x, curLocalPos.y > topLimit ? topLimit : bottomLimit);
            //}
            //
            //float curPosRatio = (curLocalPos.y - bottomLimit) / scrollRange;
            //ScrollControl(curPosRatio);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log("Pointer Up");
        }

        private void ScrollControl(float curPosRatio)
        {
            ScrollRect sr = ContentManager.Instance.ContentScrollRect;
            if (null == sr)
            {
                Debug.LogWarning("ScrollRect not found");
                return;
            }

            sr.verticalNormalizedPosition = curPosRatio;
        }

    }
}
