using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleViewBase : MonoBehaviour, Entity
{
    public ObjectPoolSingle objType { get; set; }
    public bool isTmpObj { get; set; }
    public GameObject obj { get; set; }

    public Dictionary<Button, UnityEngine.Events.UnityAction> btnListenerDic = new Dictionary<Button, UnityEngine.Events.UnityAction>();



    public  Dictionary<TheEventType, List<Action>> eventDic = new Dictionary<TheEventType, List<Action>>();

    public  Dictionary<TheEventType, List<Action<object[]>>> eventDicWithParam = new Dictionary<TheEventType, List<Action<object[]>>>();


    public virtual void Init(params object[] args)
    {
    
    }

    /// <summary>
    /// 给组件赋值
    /// </summary>
    public virtual void OnOpenIng()
    {

    }

    //public virtual void Closed()
    //{
    //    //EntityManager.Instance.CloseEntity(this);

    //    //OnClose();
    //    //HideObj();
    //}

    /// <summary>
    /// 增加按钮点击事件
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="listener"></param>
    public virtual void addBtnListener(Button btn, UnityEngine.Events.UnityAction listener, bool changeScale = true)
    {
        if (!btnListenerDic.ContainsKey(btn))
        {
            btn.onClick.AddListener(() =>
            {
                OnBtnListner(btn.gameObject, listener);
                //listener();
            });
            btnListenerDic.Add(btn, listener);

            //if (changeScale)
            //    btn.onClick.AddListener(() => UIUtil.BtnScale(btn.transform));
            //if (AuditionManage.S != null)
            //    btn.onClick.AddListener(() => AuditionManage.S.Play(0));
        }
    }

    void OnBtnListner(GameObject obj, UnityEngine.Events.UnityAction listener)
    {
        EventCenter.Broadcast(TheEventType.OnBtnClick, obj);
        listener();
    }
    /// <summary>
    /// 移除按钮点击事件
    /// </summary>
    public void RemoveBtnClick()
    {
        if (btnListenerDic != null && btnListenerDic.Count > 0)
        {
            foreach (KeyValuePair<Button, UnityEngine.Events.UnityAction> kv in btnListenerDic)
            {
                Button btn = kv.Key;
                UnityEngine.Events.UnityAction action = kv.Value;
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                }
                //    btn.onClick.RemoveListener(action);

                //btn.onClick.AddListener(() => BtnScale(btn.transform));

            }
            btnListenerDic.Clear();

        }
    }
    public virtual void RegisterEvent(TheEventType theType, Action callBack)
    {
        EventCenter.Register(theType, callBack);
        if (!eventDic.ContainsKey(theType))
            eventDic.Add(theType, new List<Action>());

        if (!eventDic[theType].Contains(callBack))
        {
            eventDic[theType].Add(callBack);
        }
    }
    public virtual void RegisterEvent(TheEventType theType, Action<object[]> callBack)
    {
        EventCenter.Register(theType, callBack);
        if (!eventDicWithParam.ContainsKey(theType))
            eventDicWithParam.Add(theType, new List<Action<object[]>>());
        if (!eventDicWithParam[theType].Contains(callBack))
        {
            eventDicWithParam[theType].Add(callBack);
        }
    }

    /// <summary>
    /// 移除所有事件注册
    /// </summary>
    public void RemoveEventRegister()
    {
        foreach (KeyValuePair<TheEventType, List<Action>> kv in eventDic)
        {
            for (int i = 0; i < kv.Value.Count; i++)
            {
                Action theAction = kv.Value[i];
                EventCenter.Remove(kv.Key, theAction);

            }
        }
        eventDic.Clear();
        foreach (KeyValuePair<TheEventType, List<Action<object[]>>> kv in eventDicWithParam)
        {
            for (int i = 0; i < kv.Value.Count; i++)
            {
                Action<object[]> theAction = kv.Value[i];
                EventCenter.Remove(kv.Key, theAction);

            }
        }
        eventDicWithParam.Clear();
    }
    public virtual void Clear()
    {

    }

    public virtual void OnClose()
    {


    }
}
