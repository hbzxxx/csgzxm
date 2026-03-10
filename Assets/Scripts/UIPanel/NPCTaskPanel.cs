using Framework.Data;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCTaskPanel : PanelBase
{
    public Text txt;
    public Transform trans_awardGrid;
    public Button btn_go;
    public Button btn_battle;//战斗
    public Button btn_cancel;//再想想
    public SingleNPCData singleNpcData;
    NPC npcSetting;
    public int taskIndex;
    public override void Init(params object[] args)
    {
        base.Init(args);
        singleNpcData = args[0] as SingleNPCData;
        npcSetting = DataTable.FindNPCArrById(singleNpcData.Id);
        addBtnListener(btn_go, () =>
        {
            TaskManager.Instance.StartGuide(singleNpcData.OnlyId);
            PanelManager.Instance.ClosePanel(this);
        });
        addBtnListener(btn_battle, () =>
        {
            if(npcSetting.npcType!=NPCType.Enemy)
            {
                singleNpcData.PeopleData.enemySettingId = (int)npcSetting.enemyId;
                BattleManager.Instance.StartQieCuoBattle(singleNpcData.OnlyId);

            }
            else
                BattleManager.Instance.StartNPCKillBattle(singleNpcData.OnlyId);
        });

        addBtnListener(btn_cancel, () =>
        {
            PanelManager.Instance.ClosePanel(this);
        });
    
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        if (npcSetting.npcType == NPCType.Enemy)
        {
            btn_go.gameObject.SetActive(false);
            btn_battle.gameObject.SetActive(true);
            txt.SetText(npcSetting.des);

        }
        else if(npcSetting.npcType==NPCType.BranchLine
            ||npcSetting.npcType==NPCType.None)
        {
            SingleTask curTaskSetting = TaskManager.Instance.FindTaskSettingById(npcSetting, singleNpcData.CurTaskId);
            if (curTaskSetting != null)
            {
                taskIndex = curTaskSetting.index;
                switch (curTaskSetting.taskType)
                {
                    case TaskType.QieCuo:
                        btn_go.gameObject.SetActive(false);
                        btn_battle.gameObject.SetActive(true);
                        break;
                    default:
                        btn_go.gameObject.SetActive(true);
                        btn_battle.gameObject.SetActive(false);
                        break;
                }
                txt.SetText(curTaskSetting.des);
                if (curTaskSetting.awardList.Count> 0)
                {
                    foreach(string str in curTaskSetting.awardList)
                    {
                        string[] arr = str.Split('|');
                        ItemData itemData = new ItemData();
                        itemData.settingId = arr[0].ToInt32();
                        itemData.count = arr[1].ToUInt64();
                        AddSingle<WithCountItemView>(trans_awardGrid, itemData);
                    }
             
                }
  
            }
        }

        
      
  
    }

    public override void Clear()
    {
        base.Clear();

    }

    public override void OnClose()
    {
        base.OnClose();
        //任务变为进行中，并判断是否完成
       if(singleNpcData.CurTaskId != 0)
        {
            SingleTaskProtoData taskData = TaskManager.Instance.FindTaskById(singleNpcData, singleNpcData.CurTaskId);
            if (taskData.AccomplishStatus == (int)AccomplishStatus.UnAccomplished)
            {
                 SingleTask taskSetting = TaskManager.Instance.FindTaskSettingById(npcSetting, taskData.TheId);
                TDGAMission.OnBegin(taskSetting.trackingName);
                TaskManager.Instance.ChangeTaskStatus(taskData, AccomplishStatus.Processing);

            }
            TaskManager.Instance.TryAccomplishTask(singleNpcData);
        }
    }
}
