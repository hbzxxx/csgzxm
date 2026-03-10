using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LongPressAndClickBtnView : LongPressBtnView, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler
{
    public float longPressFunctionTime = 0.4f;//长按判断时间 非该时间就是点击
    float longPressFunctionTimer = 0f;//长按判断
    public Action endCallBack;//结束回调
    public Action clickCallBack;//点击回调

    void OnEnable()
    {
        base.OnEnable();
        pressed = false;
        timer = 0;
        cbTimer = 0;
        longPressFunctionTimer = 0;
 
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
  
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if (longPressFunctionTimer >= longPressFunctionTime)
        {
            endCallBack?.Invoke();
        }
        //判断为点击
        else
        {
            clickCallBack?.Invoke();
        }
        longPressFunctionTimer = 0;
    }
    public override void Update()
    {
        base.Update();
        if (pressed)
        {
            longPressFunctionTimer += Time.deltaTime;
        }
    }
}
