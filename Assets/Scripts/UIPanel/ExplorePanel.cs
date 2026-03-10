using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using Framework.Data;
using UnityEngine.UI;
using DG.Tweening;

public class ExplorePanel : PanelBase
{
    public ExploreMapSetting setting;
    public Transform trans_startPos;//开始位置
    public Transform trans_exploreTeamParent;//探险队位置
    public Transform trans_exploreFollowPParent;//探险队跟随人位置

    public Transform trans_mapEventParent;//地图事件父物体


    public List<ExplorePointView> explorePointViewList = new List<ExplorePointView>();//事件点
    public ExploreTeamView exploreTeamView;//探险队

    public Transform trans_routeParent;//路径
    public float routeDistance = 25;//路径点距离

    public SingleExploreData exploreData;//秘境探险数据

    public ExplorePointView curChooseExplorPointView;//当前选择的事件点

    public Button btn_empty;//空白
    List<FollowPView> followPViewList=new List<FollowPView>();
    Vector3 lastTimePos;
    public float pDistance = 100;//隔了100才开始动
    bool initOk = false;
    public ScrollViewNevigation scrollViewNevigation;//定位

    public override void Init(params object[] args)
    {
        base.Init(args);
        int id = (int)args[0];
        exploreData=MapManager.Instance.FindSingleExploreDataById(id);
        setting = DataTable.FindExploreMapSetting(id);

        addBtnListener(btn_empty, () =>
        {
            if (exploreTeamView.exploreTeamData.Status == (int)ExploreTeamStatus.Idling)
            {
                ClearRoute();
                for (int i = 0; i < explorePointViewList.Count; i++)
                {
                    explorePointViewList[i].OnChoose(false);
                }
            }

        });
        RegisterEvent(TheEventType.RemoveMapEvent, OnRemoveMapEvent);
        RegisterEvent(TheEventType.OnTeamExploreArrived, OnTeamExploreArrived);
        RegisterEvent(TheEventType.GoMiJingPointExplore, OnStartExplore);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        List<SingleMapEventData> mapEventList= MapManager.Instance.FindAllExploreMapEventById(setting.Id.ToInt32());
        Vector2 startPos = trans_startPos.transform.localPosition;
        Debug.Log(startPos);
        int hangNum = setting.HangNum.ToInt32();
        int lieNum = setting.LieNum.ToInt32();
        List<int> offset = CommonUtil.SplitCfgOneDepth(setting.Distance);
        for(int i=0;i< mapEventList.Count; i++)
        {
            SingleMapEventData data = mapEventList[i];
            int index = data.PosIndex;
            //第几行 从0数
            Vector2Int logicPos = CommonUtil.GetLogicPosByLogicIndex(data.PosIndex,lieNum);
            //int hangIndex = index / hangNum;
            ////第几列 从0数
            //int lieIndex = index % lieNum;
            Vector2 localPos = new Vector2(startPos.x + offset[0] * logicPos[0], startPos.y + offset[1] * logicPos[1]);
            //return localPos;
            if (data.Pos.Count == 0)
            {
                data.Pos.Add(Mathf.RoundToInt(localPos.x));
                data.Pos.Add(Mathf.RoundToInt(localPos.y));

            }
            else
            {
                data.Pos[0] = Mathf.RoundToInt(localPos.x);
                data.Pos[1] = Mathf.RoundToInt(localPos.y);

            }
            ExplorePointView view= AddSingle<ExplorePointView>(trans_mapEventParent, data,this);
            explorePointViewList.Add(view);
        }

        //生成队伍
        if (exploreData.ExploreTeamData.Pos.Count <= 0)
        {
            //如果没有赋值，则在起点给他赋值  位置应该在当前点 而不是保存点
            Vector2 teamPos = new Vector2(startPos.x + offset[0] * exploreData.ExploreTeamData.LogicPos[0], startPos.y + offset[1] * exploreData.ExploreTeamData.LogicPos[1]);
            exploreData.ExploreTeamData.Pos.Add(teamPos[0]);
            exploreData.ExploreTeamData.Pos.Add(teamPos[1]);
        }
        exploreTeamView = AddSingle<ExploreTeamView>(trans_exploreTeamParent, exploreData.ExploreTeamData,
            new Vector2(exploreData.ExploreTeamData.Pos[0], exploreData.ExploreTeamData.Pos[1]),
            this);
        if (exploreData.ExploreTeamData.StudentOnlyIdList.Count > 1)
        {
            for(int i=1;i< exploreData.ExploreTeamData.StudentOnlyIdList.Count; i++)
            {
                FollowPView followPView = AddSingle<FollowPView>(trans_exploreFollowPParent, new Vector2(exploreData.ExploreTeamData.Pos[0], exploreData.ExploreTeamData.Pos[1]));
                followPViewList.Add(followPView);
                followPView.DOKill();

                if (exploreTeamView.exploreTeamData.Status == (int)ExploreTeamStatus.Moving)
                {
                    followPView.transform.position = exploreTeamView.transform.position;
                    //动画idle
                    followPView.ske.AnimationState.SetAnimation(0, "zoulu", true);
                }
            }
        }

        //如果有目的地 显示路径
        if(exploreTeamView.exploreTeamData.TargetEventOnlyId != 0
            &&exploreTeamView.exploreTeamData.Status==(int)ExploreTeamStatus.Moving)
        {
            ShowRoute(exploreTeamView.exploreTeamData.TargetEventOnlyId);
            exploreTeamView.ske.AnimationState.SetAnimation(0, "zoulu", true);

        }
        RevealMapPoint();

        //定位过去
        scrollViewNevigation.NevigateImmediately(exploreTeamView.GetComponent<RectTransform>());

        for(int i=0;i< explorePointViewList.Count; i++)
        {
            if (exploreTeamView.exploreTeamData.LogicPosIndex == explorePointViewList[i].eventData.PosIndex)
            {
                explorePointViewList[i].RevealWenHao();
            }
        }
    
        initOk = true;
    }

    private void Update()
    {
        if (!initOk)
            return;
        if (exploreTeamView.exploreTeamData.Status == (int)ExploreTeamStatus.Moving)
        {
            for(int i = 0; i < followPViewList.Count; i++)
            {
                FollowPView followPView = followPViewList[i];
                if (Vector2.Distance(followPView.transform.localPosition, exploreTeamView.transform.localPosition) >= pDistance)
                {
                    Vector3 direction = exploreTeamView.transform.localPosition - followPView.transform.localPosition;
                    followPView.transform.localPosition = exploreTeamView.transform.localPosition - direction.normalized * pDistance;
                }
            }
           
        }
      
      
        lastTimePos = exploreTeamView.transform.position;
    }

    /// <summary>
    /// 设置跟随的路径
    /// </summary>
    void SetFolowPPos()
    {

    }
   

    /// <summary>
    /// 显示路径
    /// </summary>
    public void ShowRoute(ulong eventOnlyId)
    {
        ClearCertainParentAllSingle<RoutePointView>(trans_routeParent);
        ExplorePointView explorePointView = FindExplorePointViewByOnlyId(eventOnlyId);

        Vector2 finalPos = explorePointView.transform.localPosition;
        Vector2 startPos = exploreTeamView.transform.localPosition;

        float totalDistance = Vector2.Distance(finalPos, startPos);

        int count =(int)(totalDistance / routeDistance) + 1;

        float yOffset = (finalPos.y - startPos.y) / (float)(count - 1);
        float xOffset = (finalPos.x - startPos.x) / (float)(count - 1);

        Vector2 thePos = startPos;
        for(int i = 0; i < count; i++)
        {
            AddSingle<RoutePointView>(trans_routeParent, thePos);
            thePos = new Vector2(thePos.x + xOffset, thePos.y + yOffset);
        }
    }


    ExplorePointView FindExplorePointViewByOnlyId(ulong onlyId)
    {
        for(int i = 0; i < explorePointViewList.Count; i++)
        {
            ExplorePointView theView = explorePointViewList[i];
            if (theView.eventData.OnlyId == onlyId)
                return theView;
        }
        return null;
    }

    /// <summary>
    /// 点击了一个事件点
    /// </summary>
    public void OnClickedSingleExplorePoint(ExplorePointView view)
    {
        if (exploreData.ExploreTeamData.Status == (int)ExploreTeamStatus.Moving)
        {
            PanelManager.Instance.OpenFloatWindow("正在前往目的地，请等待");
            return;
        }
        curChooseExplorPointView = view;
        //先清掉所有route
        ClearRoute();
        
        //显示路径
        for (int i = 0; i < explorePointViewList.Count; i++)
        {
            ExplorePointView theView = explorePointViewList[i];
            if (view.eventData.OnlyId == theView.eventData.OnlyId)
            {
                theView.OnChoose(true);
                //显示路径点
                ShowRoute(theView.eventData.OnlyId);
            }
            //else
            //{
            //    theView.OnChoose(false);
            //}

        }

        
    }
    /// <summary>
    /// 秘境探险
    /// </summary>
    public void OnGoMiJingExplore(SingleMapEventData eventData)
    {
        //ClearRoute();
        for (int i = 0; i < explorePointViewList.Count; i++)
        {
            explorePointViewList[i].OnChoose(false);
        }
        MapManager.Instance.OnGoMiJingExplore(exploreData, eventData);
    }

    /// <summary>
    /// 通过唯一id找秘境点
    /// </summary>
    /// <param name="onlyId"></param>
    /// <returns></returns>
    public ExplorePointView FindExplorePointByOnlyId(ulong onlyId)
    {
        for(int i = 0; i < explorePointViewList.Count; i++)
        {
            ExplorePointView view = explorePointViewList[i];
            if (view.eventData.OnlyId == onlyId)
            {
                return view;
            }
        }
        return null;
    }

    /// <summary>
    /// 清掉路径
    /// </summary>
    public void ClearRoute()
    {
        ClearCertainParentAllSingle<RoutePointView>(trans_routeParent);
    }

    /// <summary>
    /// 移除地图事件
    /// </summary>
    /// <param name="args"></param>
    void OnRemoveMapEvent(object[] args)
    {
        SingleMapEventData data = args[0] as SingleMapEventData;
        for(int i = 0; i < explorePointViewList.Count; i++)
        {
            ExplorePointView view = explorePointViewList[i];
            SingleMapEventData theData = view.eventData;
            if (theData.OnlyId == data.OnlyId)
            {
                PanelManager.Instance.CloseSingle(view);
                explorePointViewList.Remove(view);
                break;
            }
        }
    }


    void OnTeamExploreArrived(object[] args)
    {
        SingleExploreData data = args[0] as SingleExploreData;
        if(data.SettingId== exploreData.SettingId)
        {
            RevealMapPoint();
            for(int i = 0; i < followPViewList.Count; i++)
            {
                FollowPView followPView = followPViewList[i];
                int offsetX = RandomManager.Next(-50, 50);
                int offsetY = RandomManager.Next(-50, 50);
                List<int> offset = CommonUtil.SplitCfgOneDepth(setting.Distance);
                Vector2 startPos = trans_startPos.transform.localPosition;

                Vector2 teamPos = new Vector2(startPos.x + offset[0] * exploreData.ExploreTeamData.LogicPos[0], startPos.y + offset[1] * exploreData.ExploreTeamData.LogicPos[1]);

                Vector2 targetPos = new Vector2(teamPos.x+offsetX, teamPos.y + offsetY);
                followPView.transform.DOLocalMove(targetPos, 0.5f).OnComplete(() =>
                {
                    //动画idle
                    followPView.ske.AnimationState.SetAnimation(0, "daiji", true);
                    exploreTeamView.ske.AnimationState.SetAnimation(0, "daiji", true);
                });
            }

            if(gameObject.activeInHierarchy)
            exploreTeamView.btn.onClick.Invoke();
        }
  

    }
    /// <summary>
    /// 开始探险
    /// </summary>
    /// <param name="args"></param>
    void OnStartExplore(object[] args)
    {
        SingleExploreData data = args[0] as SingleExploreData;
        if (data.SettingId == exploreData.SettingId)
        {
            for(int i = 0; i < followPViewList.Count; i++)
            {
                FollowPView followPView = followPViewList[i];
                followPView.DOKill();
                followPView.transform.position = exploreTeamView.transform.position;
                //动画idle
                followPView.ske.AnimationState.SetAnimation(0, "zoulu", true);
            }
            exploreTeamView.ske.AnimationState.SetAnimation(0, "zoulu", true);

        }
    }

    /// <summary>
    /// 显示未隐藏的地图点
    /// </summary>
    public void RevealMapPoint()
    {
        //解开地图
        for (int i = 0; i < explorePointViewList.Count; i++)
        {
            ExplorePointView view = explorePointViewList[i];
            if (view.eventData.LogicPos[1] - exploreTeamView.exploreTeamData.LogicPos[1] <= 1)
            {
                view.Reveal();
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        explorePointViewList.Clear();
        followPViewList.Clear();
        initOk = false;
    }
}

/// <summary>
/// 探险队的情况
/// </summary>
public enum ExploreTeamStatus
{
    None=0,
    Moving=1,
    Idling=2,
}