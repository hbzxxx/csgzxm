using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskGuidePanel : PanelBase
{
    public Vector3 pos;
    public GameObject obj_finger;
    public GameObject targetObj;
    public bool initOk = false;
    GuideType guideType;
    public Text txt;
    public Action callBackAction;
    public override void Init(params object[] args)
    {
        base.Init(args);
        guideType = (GuideType)args[0];
        pos = (Vector3)args[1];
        targetObj = args[2] as GameObject;
        switch (guideType)
        {
            case GuideType.Common:
                txt.gameObject.SetActive(false);
                break;
            case GuideType.WithTxt:
                string content = (string)args[3];
                txt.gameObject.SetActive(true);
                txt.SetText(content);
                break;

        }
        if (args.Length >= 5)
            callBackAction = args[4] as Action;
        else
            callBackAction = null;
        obj_finger.transform.position = pos;
        initOk = true;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    private void Update()
    {
        if(initOk&&targetObj!=null&&targetObj.activeInHierarchy)
        obj_finger.transform.position = targetObj.transform.position;

    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.curTaskGuidePanel = null;
        initOk = false;
        callBackAction?.Invoke();

    }

    /// <summary>
    /// 更新手指位置
    /// </summary>
    /// <param name="pos"></param>
    public void RefreshShow(Vector3 pos)
    {

    }
}

/// <summary>
/// 指引类型 是否一系列
/// </summary>
public enum GuideType
{
    None=0,
    Common,
    Array,
    WithTxt,//有文字提示
}