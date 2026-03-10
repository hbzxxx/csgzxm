using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using UnityEngine.UI;
using Framework.Data;

public class SingleDailyTaskView : SingleViewBase
{
    public SingleDailyTaskData singleDailyTaskData;
    public TaskSetting taskSetting;

    public Text txt_name;
    public Button btn_go;//前往
    public Button btn_getAward;//领奖
    public Transform trans_haveGetAward;
    public Text txt_process;
    
    public override void Init(params object[] args)
    {
        base.Init(args);
        singleDailyTaskData = args[0] as SingleDailyTaskData;

        taskSetting = DataTable.FindTaskSetting(singleDailyTaskData.settingId);
        
        // 添加前往按钮点击事件
        addBtnListener(btn_go, () =>
        {
            TaskType taskType = (TaskType)taskSetting.TaskType.ToInt32();
            TaskManager.Instance.TryAccomplishDailyTask(taskType);
        });
        
        addBtnListener(btn_getAward, () =>
        {
            TaskManager.Instance.OnGetDailyTaskAward(singleDailyTaskData);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_name.SetText(taskSetting.Des);
        if (singleDailyTaskData.accomplishStatus == (int)AccomplishStatus.Processing)
        {
            btn_go.gameObject.SetActive(true);
            btn_getAward.gameObject.SetActive(false);
            trans_haveGetAward.gameObject.SetActive(false);
            txt_process.SetText("0/1");
        }
        else if(singleDailyTaskData.accomplishStatus == (int)AccomplishStatus.Accomplished)
        {
            btn_go.gameObject.SetActive(false);
            btn_getAward.gameObject.SetActive(true);
            trans_haveGetAward.gameObject.SetActive(false);
            txt_process.SetText("1/1");
        }
        else
        {
            btn_go.gameObject.SetActive(false);
            btn_getAward.gameObject.SetActive(false);
            trans_haveGetAward.gameObject.SetActive(true);
            txt_process.SetText("1/1");
        }
    }

}
