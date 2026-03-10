using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// scroll自动定位
/// </summary>
public class ScrollAutoPoint : MonoBehaviour,IEndDragHandler
{
    public ScrollRect scroll;
    public List<RectTransform> sonList;
    public Transform trans_center;//中心点
    public float minSize;
    public float maxSize;
    public float sonWidth;

    public float minSpeed;
    
    public ScrollViewNevigation neviGation;

    bool initOk = false;
    public void OnEndDrag(PointerEventData eventData)
    {
 
        scroll.OnEndDrag(eventData);
        OnLocate();

    }

    public void OnLocate(RectTransform theRect=null)
    {

        if (theRect == null)
        {
            //找离中心最近的
            float minOffset = float.MaxValue;
             for (int i = 0; i < sonList.Count; i++)
            {
                RectTransform rect = sonList[i];
                float thePos = rect.localPosition.x + scroll.content.localPosition.x;
                float offset = Mathf.Abs(thePos - trans_center.localPosition.x);

                if (offset < minOffset)
                {
                    minOffset = offset;
                    theRect = rect;
                }


            }
        }
        if(gameObject.activeInHierarchy)
        StartCoroutine(test(theRect));
        //neviGation.Nevigate(theRect, false,0.1f);
        //EventCenter.Broadcast(TheEventType.NevigateScrollAutoPoint, theRect.gameObject);
    }
    IEnumerator test(RectTransform theRect)
    {
        yield return null;
        neviGation.Nevigate(theRect, false, 0.1f);
        EventCenter.Broadcast(TheEventType.NevigateScrollAutoPoint, theRect.gameObject);

    }

    public void Init()
    {

        int childCount = scroll.content.childCount;
        sonList = new List<RectTransform>();
        for (int i = 0; i < childCount; i++)
        {
            RectTransform rect = scroll.content.GetChild(i).GetComponent<RectTransform>();
            sonList.Add(rect);
            sonWidth = rect.sizeDelta.x;
        }
        initOk = true;
    }
    private void OnEnable()
    {

 
    }

    private void Update()
    {

        if (!initOk)
            return;
        for (int i = 0; i < sonList.Count; i++)
        {
            RectTransform rect = sonList[i];
            float thePos = rect.localPosition.x + scroll.content.localPosition.x;
            float offset = Mathf.Abs(thePos - trans_center.localPosition.x);

            float rate = (rect.sizeDelta.x - offset) / rect.sizeDelta.x;
            if (rate < 0)
                rate = 0;
            float sizeRate = 1 + (maxSize - 1) * rate;

            rect.localScale = new Vector3(sizeRate, sizeRate, sizeRate);
        }

 
    }

    public void OnClose()
    {
        initOk = false;
    }

}
