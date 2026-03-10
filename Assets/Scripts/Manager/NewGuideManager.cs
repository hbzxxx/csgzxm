using Framework.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
public class NewGuideManager : MonoBehaviour
{
    public NewGuideStatus newGuideStatus;
    //public int waitConditionId;//当前等待哪个引导执行条件
    public NewGuideIdType runningId;//正在执行哪个引导

    public List<NewGuideIdType> waitConditionIdList = new List<NewGuideIdType>();//哪些引导正在等待条件触发队列

    public NewGuideCanvas newGuideCanvas;//引导面板

    static NewGuideManager inst;

    public bool initOk;


    public static NewGuideManager Instance
    {
        get
        {
            if (inst == null)
            {
                GameObject obj = new GameObject();
                obj.name = "NewGuideManager";
                inst = obj.AddComponent<NewGuideManager>();
                return inst;
            }
            return inst;
        }
    }

    public void Init()
    {
        //return;
        if (!Game.Instance.openNewGuide)
            return;
        DontDestroyOnLoad(this.gameObject);
        newGuideStatus = NewGuideStatus.Search;
        waitConditionIdList = new List<NewGuideIdType>();
        newGuideCanvas = null;
        runningId = 0;


        EventCenter.Register(TheEventType.AccomplishNewGuide, AccomplishNewGuide);

        initOk = true;
    }

    /// <summary>
    /// 完成了某个引导 
    /// </summary>
    /// <param name="newGuideIdType"></param>
    void AccomplishNewGuide(object[] param)
    {
        NewGuideIdType newGuideIdType = (NewGuideIdType)param[0];

        if (this == null || gameObject == null)
            return;
        
        if (newGuideStatus == NewGuideStatus.OpeningPanel
            && runningId== newGuideIdType)
        {
            int index = RoleManager.Instance._CurGameInfo.NewGuideData.IdList.IndexOf((int)newGuideIdType);
            //该引导没做完
            TaskStatusType type = (TaskStatusType)RoleManager.Instance._CurGameInfo.NewGuideData.AccomplishStatus[index];
            if (type == TaskStatusType.UnAccomplished)
            {
                //完成该引导，并进入到检测下一个引导的状态
                RoleManager.Instance._CurGameInfo.NewGuideData.AccomplishStatus[index] = (int)TaskStatusType.Accomplished;
                newGuideCanvas.Close();
                newGuideCanvas = null;
                newGuideStatus = NewGuideStatus.Search;
            }
        }
    
    }
    
    public void Update()
    {
        //return;
        if (!initOk)
        {
            return;
        }
            if (RoleManager.Instance.initOk)
            {
                if (newGuideStatus == NewGuideStatus.Search)
                {
                    Search();
                }
                else if (newGuideStatus == NewGuideStatus.Running)
                {
                    Running();
                }
            }

        

    }
    

    /// <summary>
    /// 寻找需要触发的新手引导
    /// </summary>
    void Search()
    {
        int count = RoleManager.Instance._CurGameInfo.NewGuideData.IdList.Count;
        waitConditionIdList.Clear();
        for (int i = 0; i < count; i++)
        {
            int theId = RoleManager.Instance._CurGameInfo.NewGuideData.IdList[i];
            int accomplishStatus = RoleManager.Instance._CurGameInfo.NewGuideData.AccomplishStatus[i];
            //若找到第一个存在 该任务没有做完，没有上一个任务，或上一个任务做完了的引导任务（该引导待触发），则跳出循环，判断该任务是否满足触发条件，若满足，则执行引导
 
            if (accomplishStatus == (int)TaskStatusType.UnAccomplished)
            {

                NewGuideSetting newGuideSetting = Framework.Data.DataTable.FindNewGuideSetting(theId);
                int lastId =0;

                //没有上个任务，且没做完
                if (lastId == 0)
                {
                    //newGuideStatus = NewGuideStatus.WaitCondition;
                    //break;
                    if(!waitConditionIdList.Contains((NewGuideIdType)theId))
                    waitConditionIdList.Add((NewGuideIdType)theId);
                }
                //有上个任务，判断上个任务是否做完
                else
                {
                    int lastIndex = RoleManager.Instance._CurGameInfo.NewGuideData.IdList.IndexOf(lastId);
                    if (RoleManager.Instance._CurGameInfo.NewGuideData.AccomplishStatus[lastIndex] == (int)TaskStatusType.Accomplished)
                    {
                        if (!waitConditionIdList.Contains((NewGuideIdType)theId))
                            waitConditionIdList.Add((NewGuideIdType)theId);
                    }
                }
            }
        }

        int waitCount = waitConditionIdList.Count;
        for(int i=0; i < waitCount; i++)
        {
            NewGuideIdType idType = waitConditionIdList[i];
            if (CheckIfTriggerGuide(idType))
            {
                runningId = idType;
                newGuideStatus = NewGuideStatus.Running;
                break;
            }
        }
    }

    /// <summary>
    /// 执行新手引导 进入寻找聚焦按钮状态
    /// </summary>
    void Running()
    {
        NewGuideSetting newGuideSetting = Framework.Data.DataTable.FindNewGuideSetting((int)runningId);
        //没有这个按钮 视为不满足条件，break
        if (GameObject.Find(newGuideSetting.HighLightObjPath) == null
            || !GameObject.Find(newGuideSetting.HighLightObjPath).activeInHierarchy)
        {
            runningId = 0;
            newGuideStatus = NewGuideStatus.Search;
        }
        //打开引导面板，进入引导面板状态 并聚焦按钮
        else
        {
            newGuideStatus = NewGuideStatus.OpeningPanel;
            //如果有对话，则先对话
           PanelManager.Instance.OpenNewGuideCanvas(newGuideSetting);
        }
    }
    /// <summary>
    /// 该引导是否满足触发条件
    /// </summary>
    bool CheckIfTriggerGuide(NewGuideIdType idType)
    {
     
    
        NewGuideSetting newGuideSetting = DataTable.FindNewGuideSetting((int)idType);
        ////前置npc对话必须说完
        //int npcId = newGuideSetting.AfterNPCIndex.ToInt32();
        //if(npcId!=-1 && Guide.S.Guides[npcId] == 0)
        //{
        //    return false;
        //}
     
        switch (idType)
        {
            case NewGuideIdType.GoToMountain://去山门
                if (GameTimeManager.Instance.timeMoving)
                    return true;
                else
                    return false;
            case NewGuideIdType.RecruitStudent://招募学生
                if (GameTimeManager.Instance.timeMoving)
                    return true;
                else
                    return false;
            case NewGuideIdType.DistributeStudent://分配学生
                if (GameTimeManager.Instance.timeMoving
                    && RoleManager.Instance._CurGameInfo.studentData.CurFreeStudentNum>0)
                    return true;
                else
                    return false;
            case NewGuideIdType.ClickDistributeStudentBtn://点击分配学生按钮
                    return true;
            case NewGuideIdType.EquipMake://炼器
                if (GameTimeManager.Instance.timeMoving
                  && !EquipmentManager.Instance.IfHaveAnyEquip())
                    return true;
                else
                    return false;
            case NewGuideIdType.ChooseEquipMakeBtn://选择炼器按钮
                return true;
            case NewGuideIdType.ChooseEquipMakeTeam://选择炼器师傅按钮
                return true;
      
            default:
                return false;
        }
    }

 


}

/// <summary>
/// 引导Id类型
/// </summary>
public enum NewGuideIdType
{
    None=0,
    PointHunHun=10000,//点混混
    GoToMountain=10001,//前往后山
    RecruitStudent=10002,//招募弟子
    DistributeStudent=10003,//分配弟子
    ClickDistributeStudentBtn=10004,//点击分配弟子按钮
    EquipMake=10005,//炼器
    ChooseEquipMakeBtn=10006,//选择炼器按钮
    ChooseEquipMakeTeam = 10007,//选择炼器师傅
     QieRen=10008,//切人
     LiLian=10009,//历练
     QiangHuaXueMai=10010,//强化血脉
    ChangeYuanSu=10011,//改变元素引导
}

/// <summary>
/// 新手任务触发条件
/// </summary>
public enum NewGuideTriggerCondition
{
    None = 0,
    Task_DaGao,//打稿触发条件
}

/// <summary>
/// 引导状态
/// </summary>
public enum NewGuideStatus
{
    None=0,
    Search=1,//寻找是否有新引导需要被触发 并加入等待
    //WaitCondition=2,//找到了需要被触发的引导 等待触发条件
    Running,//正在执行某个引导中。。
    OpeningPanel,//引导面板打开中
}

/// <summary>
/// 完成类型
/// </summary>
public enum TaskStatusType
{
    None = 0,
    UnAccomplished,//未完成
    Accomplished,//已完成未领奖
    GotAward,//已领奖
}