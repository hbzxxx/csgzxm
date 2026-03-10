using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PressBtnView : Button,IPointerDownHandler, IPointerUpHandler
{
    public bool pressed = false;
    public Action callBack;
    protected override void OnEnable()
    {
        base.OnEnable();
        pressed = false;
       
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
      
        pressed = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (pressed)
        {
            callBack?.Invoke();
        }

    }
}
