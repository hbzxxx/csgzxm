using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;

public class ExploreTeamView : SingleViewBase
{

    public ExploreTeamData exploreTeamData;
    bool moving = false;
    public ExplorePanel parentPanel;
    ExplorePointView targetView;

    public Image img_buji;//补给
    public Text txt_bujiNum;//补给数量

    public Button btn;
    public Spine.Unity.SkeletonGraphic ske;
    public override void Init(params object[] args)
    {
        base.Init(args);
        exploreTeamData = args[0] as ExploreTeamData;
        transform.localPosition = (Vector2)args[1];
        parentPanel = args[2] as ExplorePanel;

        RegisterEvent(TheEventType.GoMiJingPointExplore, OnStartExplore);
        RegisterEvent(TheEventType.OnTeamExploreMoving, OnExploreMoving);
        RegisterEvent(TheEventType.RefreshExploreBuJiShow, OnRefreshBuJiShow);

        addBtnListener(btn, OnChoose);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        SetExploreTargetView();
        ShowBuJi();
        ske.AnimationState.SetAnimation(0, "daiji", true);

    }


    public void OnChoose()
    {
        //玩家就在这里，触发事件
        if (exploreTeamData.TargetEventOnlyId != 0&&exploreTeamData.Status==(int)ExploreTeamStatus.Idling)
        {
            SingleMapEventData eventData = MapEventManager.Instance.FindMapEventDataByOnlyId(exploreTeamData.TargetEventOnlyId);
            //Vector2Int teamlogicPos = new Vector2Int(parentPanel.exploreData.ExploreTeamData.LogicPos[0], parentPanel.exploreData.ExploreTeamData.LogicPos[1]);

            //if (CommonUtil.GetLogicIndexByLogicPos(teamlogicPos, parentPanel.setting.lieNum.ToInt32()) == eventData.PosIndex)
            //{
            //}
            SingleExploreData exploreData= MapManager.Instance.FindSingleExploreDataById(exploreTeamData.ExploreId);
            EventCenter.Broadcast(TheEventType.ShowExploreDetail, exploreData, eventData,false);
            MapEventManager.Instance.ExploreHandleMapEvent(eventData,exploreTeamData);

        }

    }

    /// <summary>
    /// 显示补给
    /// </summary>
    public void ShowBuJi()
    {
        txt_bujiNum.SetText(exploreTeamData.RemainBuJi.ToString());
        img_buji.fillAmount = exploreTeamData.RemainBuJi / (float)100;
    }

    /// <summary>
    /// 目的地
    /// </summary>
    void SetExploreTargetView()
    {
        if (exploreTeamData.Status == (int)ExploreTeamStatus.Moving)
        {
            ExplorePointView theView = parentPanel.FindExplorePointByOnlyId(exploreTeamData.TargetEventOnlyId);
            if (theView != null)
            {
                targetView = theView;
            }
        }
    }

    /// <summary>
    /// 开始探险
    /// </summary>
    void OnStartExplore(object[] args)
    {
        SingleExploreData exploreData = args[0] as SingleExploreData;
        if (exploreData.ExploreTeamData.OnlyId == exploreTeamData.OnlyId)
        {
            exploreTeamData = exploreData.ExploreTeamData;
            SetExploreTargetView();
            ShowBuJi();
        }
    }

    /// <summary>
    /// 探险中
    /// </summary>
    /// <param name="args"></param>
    void OnExploreMoving(object[] args)
    {
        SingleExploreData data = args[0] as SingleExploreData;
        if (data.SettingId == parentPanel.exploreData.SettingId)
        {
            if (exploreTeamData.Status == (int)ExploreTeamStatus.Moving)
            {
                if (targetView != null)
                {
                    //float rate = (exploreTeamData.TotalDay - exploreTeamData.RemainDay) / (float)exploreTeamData.TotalDay;

                    //Vector2 beforeMovePos = new Vector2(exploreTeamData.LocalPosBeforeMove[0], exploreTeamData.LocalPosBeforeMove[1]);
                    //Vector2 theVec = (Vector2)targetView.transform.localPosition - beforeMovePos;
                    //theVec *= rate;
                    //Vector2 newPos = beforeMovePos + theVec;
                    Vector2 newPos = new Vector2(exploreTeamData.Pos[0], exploreTeamData.Pos[1]);

                    transform.DOLocalMove(newPos, 1f).SetEase(Ease.Linear);
                    //exploreTeamData.Pos[0] = newPos.x;
                    //exploreTeamData.Pos[1] = newPos.y;
                }
            }

        }

    }


    void OnRefreshBuJiShow(object[] args)
    {
        ExploreTeamData data = args[0] as ExploreTeamData;
        if (data.OnlyId == exploreTeamData.OnlyId)
        {
            exploreTeamData = data;
            ShowBuJi();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        RoutePointView route = collision.GetComponent<RoutePointView>();
        if (route != null)
        {
            PanelManager.Instance.CloseSingle(route);
        }
    }


}
