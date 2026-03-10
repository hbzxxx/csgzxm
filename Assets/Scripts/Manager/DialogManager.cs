using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class DialogManager : MonoInstance<DialogManager>
{

    public DialogPanel curDialogPanel;

    /// <summary>
    /// 创建普通对话
    /// </summary>
    /// <param name="dialogDataList"></param>
    /// <param name="endCallBack"></param>
    public void CreateDialog(List<DialogData> dialogDataList,Action endCallBack)
    {
        endCallBack();
        return;
        curDialogPanel =PanelManager.Instance.OpenPanel<DialogPanel>(PanelManager.Instance.trans_layer2,DialogType.Common, dialogDataList,endCallBack);
    }

    /// <summary>
    /// 创建选项对话
    /// </summary>
    /// <param name="dialogDataList"></param>
    /// <param name="endCallBack"></param>
    public void CreateDialog(List<DialogData> dialogDataList,List<string> btnStrList,List<Action> btnActionList)
    {
        //for (int i = 0; i < btnActionList.Count; i++)
        //{
        //    btnActionList[i]();
        //}
        return;
        curDialogPanel =PanelManager.Instance.OpenPanel<DialogPanel>(PanelManager.Instance.trans_layer2, DialogType.Choose, dialogDataList, btnStrList, btnActionList);
    }

    /// <summary>
    /// 创建选项对话 带返回值 不一定关闭
    /// </summary>
    /// <param name="dialogDataList"></param>
    /// <param name="endCallBack"></param>
    public void CreateDialog(List<DialogData> dialogDataList, List<string> btnStrList, List<Func<bool>> btnFuncList)
    {
        curDialogPanel = PanelManager.Instance.OpenPanel<DialogPanel>(PanelManager.Instance.trans_layer2, DialogType.Choose, dialogDataList, btnStrList, btnFuncList);
    }
}

public class DialogData
{
   public PeopleData belong;
    public string content;
    public DialogData(PeopleData p, string str)
    {
        belong = p;
        content = str;
    }
}
