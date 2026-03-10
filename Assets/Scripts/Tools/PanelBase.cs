using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelBase : MonoBehaviour,Entity,IComparable
{
    public ObjectPoolSingle objType { get; set; }
    public bool isTmpObj { get; set; }
    public GameObject obj { get; set; }

    public Button btn_emptyClose;//空白关闭
    public Dictionary<Button, UnityEngine.Events.UnityAction> btnListenerDic = new Dictionary<Button, UnityEngine.Events.UnityAction>();

    public Transform trans_content;//内容，会缩放的
    public bool blockTime;//时间暂停
    protected bool closed = false;
    public int layerWeight;//权重默认为0 越大越靠前

    public  Dictionary<TheEventType, List<Action>> eventDic = new Dictionary<TheEventType, List<Action>>();

    public  Dictionary<TheEventType, List<Action<object[]>>> eventDicWithParam = new Dictionary<TheEventType, List<Action<object[]>>>();

    public List<SingleViewBase> mySingleList = new List<SingleViewBase>();//这个panel下所有single
    public virtual void Init(params object[] args)
    {
        Clear();
    }

    /// <summary>
    /// 给组件赋值
    /// </summary>
    public virtual void OnOpenIng()
    {
        if (btn_emptyClose != null)
        {
            addBtnListener(btn_emptyClose, 
                ()=>PanelManager.Instance.ClosePanel(this));
        }
        closed = false;
        if (blockTime)
            GameTimeManager.Instance.AddTimeBlockCount();
        if(AuditionManager.Instance!=null)
        AuditionManager.Instance.PlayVoice(AudioClipType.FanShu);
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
    /// 增加按钮点击事件
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="listener"></param>
    public virtual void addBtnListener(Button btn, UnityEngine.Events.UnityAction listener, bool changeScale = true)
    {
        if (btn == null)
        {
            //Debug.LogError("按钮没赋值");

            return;
        }
        if (!btnListenerDic.ContainsKey(btn))
        {
            btn.onClick.AddListener(() =>
            {
                OnBtnListner(btn.gameObject,listener);
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
        AuditionManager.Instance.PlayVoice(AudioClipType.Click);
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

    /// <summary>
    /// 移除所有事件注册
    /// </summary>
    public void RemoveEventRegister()
    {
        foreach(KeyValuePair<TheEventType,List<Action>> kv in eventDic)
        {
            for(int i = 0; i < kv.Value.Count; i++)
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

    public virtual T AddSingle<T>(Transform parent, params object[] args) where T : SingleViewBase
    {
        T single= PanelManager.Instance.OpenSingle<T>(parent, args);
        this.mySingleList.Add((SingleViewBase)single);
        return single;
    }
    public virtual void ClearCertainParentAllSingle<T>(Transform trans) where T:SingleViewBase
    {
        for (int i = trans.childCount-1; i >= 0; i--)
        {
            SingleViewBase single = trans.GetChild(i).GetComponent<T>();
            PanelManager.Instance.CloseSingle(single);

            if (mySingleList.Contains(single))
            {
                mySingleList.Remove(single);
            }
            
        }
        //mySingleList.Clear();
    }
    public virtual void ClearAllSingle()
    {
        for(int i = mySingleList.Count - 1; i >= 0; i--)
        {
            SingleViewBase single = mySingleList[i];
            if (single.obj == null)
            {
                mySingleList.RemoveAt(i);
                single = null;
            }
            else
            {
                PanelManager.Instance.CloseSingle(single);

            }
        }
        mySingleList.Clear();
    }

    public virtual void Clear()
    {

    }

    public virtual void OnClose()
    {
        if (closed)
            return;
        closed = true;
        if (blockTime)
            GameTimeManager.Instance.DeTimeBlockCount();
    }

    public int CompareTo(object other)
    {
        PanelBase theOtherPanel = (PanelBase)other;
        if (layerWeight > theOtherPanel.layerWeight)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }
}
