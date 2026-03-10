using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakThroughResPanel : PanelBase
{
    public TrainSetting curTrainSetting;
    PeopleData p;
    List<SinglePropertyData> proAdd;
    public Transform trans_breakThroughProChangeGrid;
    public Transform trans_newXueMaiUpgrade;
    public Transform trans_newYuanSuStudy;
    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
        proAdd = args[1] as List<SinglePropertyData>;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        int curIndex = RoleManager.Instance._CurGameInfo.playerPeople.trainIndex;
        curTrainSetting = DataTable._trainList[curIndex];
        //List<SinglePropertyData> proAdd = args[0] as List<SinglePropertyData>;
        //long before = (long)args[1];
        //long after = (long)args[2];
        //StartCoroutine(ShowTxtAddAnim(proAdd, before,after));
        //中境界突破成功 等级-1为trainindex
        if (curIndex % 10 == 0)
        {
           ClearCertainParentAllSingle<SingleViewBase>(trans_breakThroughProChangeGrid);

            Debug.Log("中境界突破成功");
            //大境界突破成功
            if (curIndex % 30 == 0)
            {
                Debug.Log("大境界突破成功");

            }


            List<int> proIdList = new List<int>();
            for (int i = 0; i < proAdd.Count; i++)
            {
                SinglePropertyData pro = proAdd[i];
                int id = pro.id;
                int num = pro.num;

                int curNum = RoleManager.Instance.FindMyProperty(id).num;
                int beforeNum = curNum - num;

                string curNumShow = RoleManager.Instance.GetPropertyShow(id, curNum);
                string beforeNumShow = RoleManager.Instance.GetPropertyShow(id, beforeNum);
                AddSingle<PropertyCompareView>(trans_breakThroughProChangeGrid, id, beforeNumShow, curNumShow);
            }

            //trans_breakThroughProChangePanel.gameObject.SetActive(true);
            int beforeTrainIndex = curIndex - 1;
            if (XueMaiManager.Instance.limitLevel(beforeTrainIndex) < XueMaiManager.Instance.limitLevel(curIndex))
            {
                trans_newXueMaiUpgrade.gameObject.SetActive(true);
            }
            else
            {
                trans_newXueMaiUpgrade.gameObject.SetActive(false);
            }
            if (p.isPlayer
                &&RoleManager.Instance.CandidateYuanSuNum(beforeTrainIndex) < RoleManager.Instance.CandidateYuanSuNum(curIndex))
            {
                trans_newYuanSuStudy.gameObject.SetActive(true);
            }
            else
            {
                trans_newYuanSuStudy.gameObject.SetActive(false);
            }
        }
    }

    public override void OnClose()
    {
        base.OnClose();
       ClearCertainParentAllSingle<SingleViewBase>(trans_breakThroughProChangeGrid);
 

   
        ////这里可能出现引导去历练
        //else if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex == 10)
        //{
        //    if (TaskManager.Instance.FindAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.LiLian).ToString()).ToInt32() == 0)
        //    {
        //        PanelManager.Instance.OpenNewGuideCanvas(DataTable.FindNewGuideSetting((int)NewGuideIdType.LiLian));
        //        TaskManager.Instance.GetAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.LiLian).ToString());
        //    }
        //}
    }
}
