using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LongPressBtnView : Button, IPointerDownHandler, IPointerUpHandler,IPointerClickHandler,IDragHandler
{

    public float initSpeed = 1;
    public List<float> range = new List<float> { 0, 1, 2, 3, 4 ,5};
    //public float maxSpeed = 0.1f;
    public bool pressed = false;
    public float timer = 0;
    public float cbTimer = 0;//回调计时器
    public Action callBack;
    public Action<PointerEventData> dragAction;//拖动
    void OnEnable()
    {
        base.OnEnable();
        pressed = false;
        timer = 0;
        cbTimer = 0;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
       
            callBack?.Invoke();
        
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        timer = 0;
        cbTimer = 0;
        pressed = false;
     }


    // Update is called once per frame
    public virtual void Update()
    {
        if (pressed)
        {
            timer += Time.deltaTime;
            if (timer < 1)
            {
                initSpeed = .4f;
            }else if (timer < 2)
            {
                initSpeed = 0.2f;
            }
            else if (timer <3)
            {
                initSpeed = 0.12f;
            }
            else if (timer < 4)
            {
                initSpeed = 0.08f;
            }
            else if (timer < 5)
            {
                initSpeed = 0.04f;
            }
            else
            {
                initSpeed = 0.02f;
            }
            cbTimer += Time.deltaTime;
            if (cbTimer >= initSpeed)
            {
                Debug.Log("长安生效");
                callBack();
                cbTimer = 0;
            }

        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        dragAction?.Invoke(eventData);
    }
}
