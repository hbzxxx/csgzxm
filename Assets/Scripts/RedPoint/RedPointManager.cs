 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPointManager
{
    static RedPointManager inst;

    public static RedPointManager Instance
    {
        get
        {
            if (inst == null)
                inst = new RedPointManager();
            return inst;
        }
    }
    List<RedPoint> RedPointList = new List<RedPoint>();
    Dictionary<RedPointType,List<RedPoint>> RedPointDic = new Dictionary<RedPointType, List<RedPoint>>();
    public bool initOk = false;
    /// <summary>
    /// 清空
    /// </summary>
    public void Clear()
    {
        RedPointDic = new Dictionary<RedPointType, List<RedPoint>>();
    }

    public void Init()
    {
        Clear();
        StudentManager.Instance.InitRedPoint();
        TaskManager.Instance.InitRedPoint();
        SkillManager.Instance.InitRedPoint();
        XueMaiManager.Instance.InitRedPoint();
        initOk = true;
    }

    /// <summary>
    /// dic里面增加一种红点
    /// </summary>
    public RedPoint AddRedPointToDic(RedPointType redPointType,int id)
    {
        RedPoint point = new RedPoint();
        point.id = id;
        point.redPointType = redPointType;
        if (!RedPointDic.ContainsKey(redPointType))
        {
            RedPointDic.Add(redPointType, new List<RedPoint>());
        }
        RedPointDic[redPointType].Add(point);
        //AddRedPointToDic()
        return point;
    }

    /// <summary>
    /// 从红点dic里面取到一个红点
    /// </summary>
    /// <returns></returns>
    public RedPoint GetRedPointFromDic(RedPointType redPointType, int id)
    {
        if (!RedPointDic.ContainsKey(redPointType))
        {
            return AddRedPointToDic(redPointType, id);
        }
        else
        {
            //字典该类有这个id
            bool haveSameId = false;
            List<RedPoint> redPointList = RedPointDic[redPointType];
            for (int i = 0; i < redPointList.Count; i++)
            {
                RedPoint thePoint = redPointList[i];
                int theId = thePoint.id;
                if (id == theId)
                {
                    haveSameId = true;
                    return thePoint;
                }
            }
            //字典该类没有id
            if (!haveSameId)
            {
                return AddRedPointToDic(redPointType, id);
            }
        }
        return null;

    }

    /// <summary>
    /// 移除一个红点类型
    /// </summary>
    /// <param name="point"></param>
    public void ClearTypeRedPoint(RedPointType redPointType)
    {
        if(RedPointDic.ContainsKey(redPointType))
        RedPointDic[redPointType].Clear();
    }

    /// <summary>
    /// 绑定两个红点
    /// </summary>
    public void BindRedPoint(RedPoint parent,RedPoint sun)
    {
        if(!parent.sunList.Contains(sun))   
            parent.sunList.Add(sun);
        if (!sun.ParentList.Contains(parent))
        {
            sun.ParentList.Add(parent);
        }
    }


    /// <summary>
    /// 改变某个红点状态
    /// </summary>
    public void ChangeRedPointStatus(RedPointType redPointType,int id,bool status)
    {
        if (RedPointDic.ContainsKey(redPointType))
        {
            List<RedPoint> redPointList = RedPointDic[redPointType];
            for(int i = 0; i < redPointList.Count; i++)
            {
                RedPoint thePoint = redPointList[i];
                int theId = thePoint.id;
                if (id == theId)
                {
                
                    thePoint.nodeVal = status;
                    UpdateStatus(thePoint);
                }
            }
        }
        
    }

    /// <summary>
    /// 设置ui的显示
    /// </summary>
    /// <param name="obj"></param>
    /// <param name=""></param>
    public void SetRedPointUI(GameObject obj,RedPointType redPointType, int id)
    {
        if (RedPointDic.ContainsKey(redPointType))
        {
            List<RedPoint> redPointList = RedPointDic[redPointType];
            bool find = false;
            for (int i = 0; i < redPointList.Count; i++)
            {
                RedPoint thePoint = redPointList[i];
                int theId = thePoint.id;
                if (id == theId)
                {
                    obj.gameObject.SetActive(thePoint.nodeVal);
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                
                obj.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("字典没有注册该红点就永不显示" + redPointType);
            obj.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 设置ui的显示
    /// </summary>
    /// <param name="obj"></param>
    /// <param name=""></param>
    public bool GetRedPointVal(RedPointType redPointType, int id)
    {
        bool res = false;
        if (RedPointDic.ContainsKey(redPointType))
        {
            List<RedPoint> redPointList = RedPointDic[redPointType];
            bool find = false;
            for (int i = 0; i < redPointList.Count; i++)
            {
                RedPoint thePoint = redPointList[i];
                int theId = thePoint.id;
                if (id == theId)
                {
                    res = thePoint.nodeVal;
                }
            }
            if (!find)
            {

                res = false;
            }
        }
        else
        {
            res = false;

        }
        return res;
    }

    ///// <summary>
    ///// 绑定红点
    ///// </summary>
    //public void BindRedPoint(RedPoint parent, RedPoint son)
    //{
    //    if (son == null
    //       || parent == null
    //       || son.PointObj == null
    //       || parent.PointObj == null)
    //        return;
    //    //子物体
    //    if (parent.SonDic == null)
    //        parent.SonDic = new Dictionary<int, RedPoint>();

    //    if (parent.SonDic.ContainsKey(son.PointObj.GetInstanceID()))
    //        return;
    //    parent.SonDic.Add(son.PointObj.GetInstanceID(), son);
    //    son.Parent = parent;

    //}

    ///// <summary>
    ///// 移除红点与父对象绑定
    ///// </summary>
    //public void RemoveBinding(RedPoint point)
    //{
    //    if (point.Parent == null)
    //        return;
    //    if (point.Parent.SonDic == null
    //        || point.Parent.SonDic.Count == 0
    //        ||!point.Parent.SonDic.ContainsKey(point.PointObj.GetInstanceID()))
    //        return;
    //    point.Parent.SonDic.Remove(point.PointObj.GetInstanceID());
    //    point.Parent = null;

    //}

    /// <summary>
    /// 更新红点状态,根据子物体改变它的父物体
    /// </summary>
    public void UpdateStatus(RedPoint redPoint)
    {    
        if (redPoint == null)
            return;

        int sunCount = redPoint.sunList.Count;
        if (sunCount != 0)
        {
            bool val = false;
            for (int i = sunCount - 1; i >= 0; i--)
            {
                RedPoint sunPoint = redPoint.sunList[i];
                if (sunPoint.nodeVal)
                {
                    val = true;
                    break;
                }
            }
            if (redPoint.id == 10051)
            {
                Debug.Log(10051);

            }
            redPoint.nodeVal = val;
        }


        if (redPoint.ParentList.Count>0)
        {
            for(int i = 0; i < redPoint.ParentList.Count; i++)
            {
                UpdateStatus(redPoint.ParentList[i]);

            }
        }
    }
    /// <summary>
    /// 更新UI
    /// </summary>
    /// <param name="redPoint"></param>
    public void UpdateUI(RedPoint redPoint)
    {
        if (redPoint.PointObj != null)
            redPoint.PointObj.SetActive(redPoint.nodeVal);
    }



    /// <summary>
    /// 改变状态
    /// </summary>
    public void ChangeRedPointStatus(RedPoint redPoint,bool change)
    {
        redPoint.PointObj.SetActive(change);
    }

}

public class RedPoint
{
    public int id;//id
    public List<RedPoint> sunList = new List<RedPoint>();//儿子红点
    public RedPointType redPointType;//任务类型
    public bool nodeVal { get; set; }//红点显示与否
   public GameObject PointObj { get; set; }//红点物体
    public List<RedPoint> ParentList = new List<RedPoint>();//父红点

}

public enum RedPointType
{
    None=0,
    MainPanel_Btn_Student,
    MainPanel_Btn_Student_BigTag,
    MainPanel_Btn_Student_BigTag_SmallTag,
    MainPanel_Btn_Student_InfoBigStudentView,
    MainPanel_Btn_Student_InfoBigStudentViewInfo,//信息处红点
    MainPanel_Btn_Student_InfoPanelStudentViewInfo,//弟子小红点

    MainPanel_Btn_Student_InfoBigStudentView_SkillUpgrade,
    MainPanel_Btn_Student_InfoBigStudentView_XueMai,
 

    //任务
    MainPanel_Btn_Task,
    MainPanel_Btn_Task_GuideBookTask,//手札
    MainPanel_Btn_Task_GuideBookTask_SingleChapterTask,//某章
    MainPanel_Btn_Task_GuideBookTask_SingleChapterProcessTask,//某章的阶段奖励
    MainPanel_Btn_Task_GuideBookTask_SingleGuideBookTask,//某个手札任务

    MainPanel_Btn_Task_DailyTask,//日常任务
    MainPanel_Btn_Task_DailyTask_Process,//日常任务活跃度阶段奖励
    MainPanel_Btn_Task_DailyTask_SingleDailyTask,//单个日常任务

    //背包
    MainPanel_Btn_Knapsack,
    MainPanel_Btn_Knapsack_SkillTag,//技能
    MainPanel_Btn_Knapsack_SkillTag_Mai,//哪个脉
    MainPanel_Btn_Knapsack_SkillTag_Mai_Skill,//哪个技能
    MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Upgrade,//可升级
    MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Find,//发现

    //掌门
    MainPanel_Btn_ZhangMen,
    MainPanel_Btn_ZhangMen_XueMai,//掌门血脉
}