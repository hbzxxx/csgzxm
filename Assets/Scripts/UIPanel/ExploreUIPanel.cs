using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using Framework.Data;
using UnityEngine.UI;
using Spine.Unity;

public class ExploreUIPanel : PanelBase
{
    public SingleExploreData exploreData;
    public Transform trans_studetnGrid;
    public Image img_bujiBar;//体力
    public Text txt_buji;//弟子体力

    public Button btn_back;//召回
    public Transform trans_itemGrid;
    public Text txt_zhanDouLi;//战斗力
    public Button btn_guWu;//鼓舞
    public SkeletonGraphic ske;
    public Text txt_remain;


    #region 目的地细节
    public Transform trans_detail;//细节
    public Text txt_desName;//细节名
    public Text txt_needDay;//需要多少天
    public SingleMapEventData curEventData;
    public Button btn_go;//出发
    #endregion

    public override void Init(params object[] args)
    {
        base.Init(args);
        exploreData = MapManager.Instance.FindSingleExploreDataById((int)args[0]);
        //exploreData.ExploreTeamData.StudentOnlyIdList
        RegisterEvent(TheEventType.RefreshExploreBuJiShow, ShowBuji);
        RegisterEvent(TheEventType.GetExploreItem, ShowItems);
        RegisterEvent(TheEventType.StudentStatusChange, OnStudentStatusChange);

        addBtnListener(btn_back, () =>
        {
            PanelManager.Instance.OpenCommonHint("确定召回吗？", () =>
            {
                //MapManager.Instance.OnLeaveExplore(exploreData,exploreData.ExploreTeamData);
                List<ItemData> itemList = new List<ItemData>();

                for(int i=0;i< exploreData.ExploreTeamData.ItemList.Count; i++)
                {
                    itemList.Add(exploreData.ExploreTeamData.ItemList[i]);
                }
                PanelManager.Instance.OpenPanel<MiJingExploreResultPanel>(PanelManager.Instance.trans_layer2, itemList, exploreData);

            }, null);
        });
        addBtnListener(btn_guWu, () =>
        {
            AuditionManager.Instance.PlayVoice(AudioClipType.Gu);
            ske.AnimationState.SetAnimation(0, "donghua", false);
            MapManager.Instance.OnExploreTeamMoving();
        });
        addBtnListener(btn_go, () =>
        {
            trans_detail.gameObject.SetActive(false);
            MapManager.Instance.OnGoMiJingExplore(exploreData, curEventData);
        });
        RegisterEvent(TheEventType.ShowExploreDetail, OnShowDetail);
        RegisterEvent(TheEventType.RemoveMapEvent, OnRemoveMapEvent);
        
    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        
        for(int i = 0; i < exploreData.ExploreTeamData.StudentOnlyIdList.Count; i++)
        {
            PeopleData student = StudentManager.Instance.FindStudent(exploreData.ExploreTeamData.StudentOnlyIdList[i]);
            if (student != null)
            {
                SingleStudentView view = AddSingle<SingleStudentView>(trans_studetnGrid, student);
            }
        }
        txt_zhanDouLi.SetText("战斗力："+MapManager.Instance.CalcExploreTeamZhanDouLi(exploreData.ExploreTeamData));
        ShowBuji();
        ShowItems();
        trans_detail.gameObject.SetActive(false);

        ShowRemainEventNum();
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="args"></param>
    void OnRemoveMapEvent(object[] args)
    {
        ShowRemainEventNum();
    }

    /// <summary>
    /// 剩余多少事件
    /// </summary>
    void ShowRemainEventNum()
    {
        int remain = MapManager.Instance.FindAllExploreMapEventById(exploreData.SettingId).Count;
        txt_remain.SetText(remain + "/" + exploreData.AllEventNum);
    }

    /// <summary>
    /// 显示细节
    /// </summary>
    /// <param name="args"></param>
    void OnShowDetail(object[] args)
    {
        SingleExploreData singleExploreData = args[0] as SingleExploreData;
        if (singleExploreData.SettingId == exploreData.SettingId)
        {
            SingleMapEventData eventData = args[1] as SingleMapEventData;
            curEventData = eventData;
            bool show = (bool)args[2];
            if (show)
            {
                MapEventSetting mapEventSetting = DataTable.FindMapEventSetting(eventData.SettingId);
                trans_detail.gameObject.SetActive(true);
                int dayNeed = MapManager.Instance.GetExploreRemainDayNum(singleExploreData, eventData);
                int weekNeed = dayNeed / 7;
                if (eventData.WenHao)
                {
                    txt_desName.SetText("未知");
                }
                else
                {
                    txt_desName.SetText(mapEventSetting.Name);
                }
                int logicPosCount =MapManager.Instance.GetDistanceByLogicPos(new Vector2Int(exploreData.ExploreTeamData.LogicPos[0], exploreData.ExploreTeamData.LogicPos[1]),
                new Vector2Int(eventData.LogicPos[0], eventData.LogicPos[1]));
                int consume = logicPosCount * 2;

                if (exploreData.ExploreTeamData.LogicPosIndex == eventData.PosIndex)
                {
                    btn_go.gameObject.SetActive(false);
                    txt_needDay.SetText("已到达目的地");
                }
                else
                {
                    btn_go.gameObject.SetActive(true);
                    txt_needDay.SetText("需要" + consume + "体力");
                }
            }
            else
            {
                trans_detail.gameObject.SetActive(false);

            }
        }
    }

    public void OnStudentStatusChange(object[] args)
    {
        txt_zhanDouLi.SetText("战斗力：" + MapManager.Instance.CalcExploreTeamZhanDouLi(exploreData.ExploreTeamData));

    }

    /// <summary>
    /// 显示物品
    /// </summary>
    void ShowItems()
    {
        ClearCertainParentAllSingle<WithCountItemView>(trans_itemGrid);
        for(int i = 0; i < exploreData.ExploreTeamData.ItemList.Count; i++)
        {
            WithCountItemView view = AddSingle<WithCountItemView>(trans_itemGrid,exploreData.ExploreTeamData.ItemList[i]);
        }
    }

    /// <summary>
    /// 显示补给
    /// </summary>
    public void ShowBuji()
    {
        img_bujiBar.fillAmount = exploreData.ExploreTeamData.RemainBuJi / (float)100;
        txt_buji.SetText(exploreData.ExploreTeamData.RemainBuJi.ToString());
    }
}
