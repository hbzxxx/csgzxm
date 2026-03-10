using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework.Data;
using System;

public class NPCView : SingleViewBase
{
    public SingleNPCData npcData;
    public NPC npcSetting;
    public Text txt_name;
    public Button btn;

    public Transform trans_portrait;//立绘
    public Image img_icon;//图片
    public Portrait portrait;

    public Vector2 localPos;//本地坐标

    public Transform trans_task_untalked;//未对话
    public Transform trans_task_processing;//进行中
    public Transform trans_task_accomplished;//已完成

    public override void Init(params object[] args)
    {
        base.Init(args);
        npcData = args[0] as SingleNPCData;
        npcSetting = DataTable.FindNPCArrById(npcData.Id);
        if (npcData.LocalPos.Count == 2)
            localPos = new Vector2(npcData.LocalPos[0], npcData.LocalPos[1]);
        else
            localPos = Vector2.zero;
        //点了 就对话并且任务变为进行中
        addBtnListener(btn, () =>
        {
            TaskManager.Instance.OnClickedNPC(npcData);
        });

        RegisterEvent(TheEventType.DisappearNPC, OnCloseThis);
        RegisterEvent(TheEventType.RemoveNPC, OnRemoveNPC);
        RegisterEvent(TheEventType.ChangeTaskStatus, RefreshTaskStatusShow);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        PeopleData p = npcData.PeopleData;

        //if (p.SpecialPortrait)
        //{
        //    trans_portrait.gameObject.SetActive(false);
        //    img_icon.gameObject.SetActive(true);
        //    img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.specialPortraitFolderPath + p.SpecialPortraitName);
        //}
        //else
        //{
        //    trans_portrait.gameObject.SetActive(true);
        //    img_icon.gameObject.SetActive(false);
        //    portrait.Refresh(p);
        //}
        
        if (npcData.SmallPeopleTextureName == "")
        {
            npcData.SmallPeopleTextureName = ConstantVal.smallPeopleMale;
        }
       
        img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.smallPeoplePath + npcData.SmallPeopleTextureName);

        txt_name.SetText(npcData.Name);

        //先强行写死帝姝生成的位置 后续加个任务位置的配置
        if (npcData.Id == (int)NPCIDType.ShenMiShaoNv)
        {
            transform.localPosition = new Vector2(0, 0);
        }
        else
            transform.localPosition = localPos;

        RefreshTaskStatusShow();
    }

    //public void SetSpecialNPCSmallPeopleTexture(string textureName)
    //{

    //}

    /// <summary>
    /// 隐藏npc
    /// </summary>
    void OnHideNPC()
    {

    }

    /// <summary>
    /// 刷新任务状态显示
    /// </summary>
    public void RefreshTaskStatusShow()
    {
        try
        {
            if (npcSetting == null)
            {
                Debug.LogError("npcSetting是null");
            }
            if (trans_task_untalked == null)
            {
                Debug.LogError("trans_task_untalked是null");
                return;
            }
            if (npcSetting.npcType == NPCType.Enemy)
            {
                trans_task_untalked.gameObject.SetActive(false);
                trans_task_processing.gameObject.SetActive(false);
                trans_task_accomplished.gameObject.SetActive(false);
            }
            else
            {
                SingleTaskProtoData taskProtoData = TaskManager.Instance.FindTaskById(npcData, npcData.CurTaskId);
                if (taskProtoData != null)
                {

                    switch ((AccomplishStatus)taskProtoData.AccomplishStatus)
                    {
                        case AccomplishStatus.UnAccomplished:
                            trans_task_untalked.gameObject.SetActive(true);
                            trans_task_processing.gameObject.SetActive(false);
                            trans_task_accomplished.gameObject.SetActive(false);

                            break;
                        case AccomplishStatus.Processing:
                            trans_task_untalked.gameObject.SetActive(false);
                            trans_task_processing.gameObject.SetActive(true);
                            trans_task_accomplished.gameObject.SetActive(false);

                            break;
                        case AccomplishStatus.Accomplished:
                            trans_task_untalked.gameObject.SetActive(false);
                            trans_task_processing.gameObject.SetActive(false);
                            trans_task_accomplished.gameObject.SetActive(true);

                            break;
                        default:
                            trans_task_untalked.gameObject.SetActive(false);
                            trans_task_processing.gameObject.SetActive(false);
                            trans_task_accomplished.gameObject.SetActive(false);
                            break;
                    }
                }
                else
                {
                    trans_task_untalked.gameObject.SetActive(false);
                    trans_task_processing.gameObject.SetActive(false);
                    trans_task_accomplished.gameObject.SetActive(false);
                }
            }
        }catch(Exception e)
        {
            Debug.LogError("NPCView刷新任务状态报错" + e);
        }
    
    }

    public void OnCloseThis(object[] args)
    {
        ulong onlyId = (ulong)args[0];
        if (onlyId == npcData.OnlyId)
            PanelManager.Instance.CloseSingle(this);
    }

    public void OnRemoveNPC(object[] args)
    {
       ulong onlyId = (ulong)args[0];
        if(onlyId==npcData.OnlyId)
            PanelManager.Instance.CloseSingle(this);

    }
}
