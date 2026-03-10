using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 凡是点空白关闭的panel都继承该类
/// </summary>
public class EmptyClosePanel : PanelBase
{
    //Canvas通过它接受点击事件  官网介绍   https://docs.unity3d.com/Manual/script-GraphicRaycaster.html
    public GraphicRaycaster RaycastInCanvas;
    //如果点击了这些，则得到该事件的点击方法
    //public List<Button> blockers=new List<Button>();
    public Transform blockParent;//点该父物体下的所有东西都不视为空白

    EventSystem eventSystem;

    //为了避面判断下级时 多执行一次回调方法
    bool state;

    private void Start()
    {
        //Init(null);
        //blockParent = transform.Find("bg").transform;
        //OnOpenIng();
        eventSystem = EventSystem.current;
        if (RaycastInCanvas == null)
            RaycastInCanvas = GetComponentInParent<GraphicRaycaster>();
    }



    public override void OnOpenIng()
    {
        base.OnOpenIng();
        eventSystem = EventSystem.current;
        if (RaycastInCanvas == null)
            RaycastInCanvas = GetComponentInParent<GraphicRaycaster>();
    }

    public virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            state = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (state)
                CheckGuiRaycastObjects();
        }
    }
    /// <summary>
    /// 关闭时需要注册的点击事件
    /// </summary>
    Action closeAction;

    /// <summary>
    /// 点到需要反应的点击事件
    /// </summary>
    Action pointOtherAction;

    /// <summary>
    /// 设置这些button无视tips的点击
    /// </summary>
    /// <param name="blockBtnList"></param>
    public void SetTipsBtn(List<Button> blockBtnList)
    {
        //blockers = new List<Button>();
        //for(int i = 0; i < blockBtnList.Count; i++)
        //{
        //    Button btn = blockBtnList[i];
        //    blockers.Add(btn);
        //}
    }

    /// <summary>
    /// 注册点击空白区域时的事件（比如  隐藏  干掉   需自己传入执行方法）
    /// </summary>
    /// <param name="callback"></param>
    public void RegisterEvent(Action pointOther, Action closeCallback)
    {
        this.closeAction = closeCallback;
        this.pointOtherAction = pointOther;
    }

    /// <summary>
    /// 设置点空白不会关闭的父物体
    /// </summary>
    //public void SetBlockParent(Transform trans)
    //{
    //    this.blockParent = trans;
    //}

    /// <summary>
    /// 是否点击在空白区域
    /// </summary>
    /// <returns></returns>
    public bool CheckGuiRaycastObjects()
    {
        //当执行了一次之后  本次鼠标点击就不再执行此方法
        state = false;

        //获取到所有鼠标射线检测到的UI
        PointerEventData eventData = new PointerEventData(eventSystem);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;
        List<RaycastResult> list = new List<RaycastResult>();
        RaycastInCanvas.Raycast(eventData, list);
        if (SearchIfBelowTheBlockRange(list))
        {
            Debug.Log("点到的范围内，不关闭");
            return false;
        }
        if (SearchOutRangeBtn(list))
        {
            Debug.Log("点到了外面的按钮，要关闭");
        }

        //if (list.Count > 0)
        //{
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        GameObject go = list[i].gameObject;
                
        //        if (blockParent != null 
        //            && go.transform.parent == blockParent)
        //        {
        //            Debug.Log("点到的对的地方");
        //            return false;
        //        }
        //        Button theBtn = go.GetComponent<Button>();
        //        if (theBtn == null)
        //            continue;
        //        //theBtn.onClick.Invoke();
        //        //PanelMgr.Instance.ClosePanel(this);
        //        return false;
        //    }
        //}

        Debug.Log("应该关闭！");
        PanelManager.Instance.ClosePanel(this);
        //closeAction?.Invoke();
        return true;
    }

    /// <summary>
    /// 查看是否在范围内
    /// </summary>
    /// <returns></returns>
    bool SearchIfBelowTheBlockRange(List<RaycastResult> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            RaycastResult res = list[i];
            if (res.gameObject == null)
                continue;
            if (!res.gameObject.activeSelf)
                continue;
            if (res.gameObject.transform == blockParent)
                return true;
            else
            {
            
            }
        }

        return false;
    }
    /// <summary>
    /// 检查是否点到了外面的按钮
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    bool SearchOutRangeBtn(List<RaycastResult> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            RaycastResult res = list[i];
            //如果是点到了外面，则调用button事件并关闭该窗口
            Button theBtn = res.gameObject.GetComponent<Button>();
            if (theBtn == null)
                continue;
            theBtn.onClick.Invoke();
            Debug.Log("调用外面的点击按钮事件");
            //PanelMgr.Instance.ClosePanel(this);
            return true;
        }
        return false;
    }
}
