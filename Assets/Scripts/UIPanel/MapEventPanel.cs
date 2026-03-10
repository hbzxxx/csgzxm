using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Framework.Data;
using System;
using cfg;

public class MapEventPanel : PanelBase
{
    public Transform trans_nameGrid;
    //public Image img_commonBg;
    //public Image img_exploreBg;
    public MapEventType mapEventType;
    public MapEventSetting mapEventSetting;//地图事件
    public SingleMapEventData singleMapEventData;//地图数据

    public Transform trans_dangerouse;//危险度
    public Text txt_dangerouse;//危险度    

    #region 第一组 
    public Transform trans_first;
    public Button btn1;
    public Button btn2;

    public Text txt_btn1;
    public Text txt_btn2;
    public Button btn_close;//关闭

    public Button btn_quZhu;//驱逐
    #endregion
    public Text txt_des;

    #region 结束
    public Transform trans_finish;
    public Button btn_leave;
    #endregion
    public override void Init(params object[] args)
    {
        base.Init(args);
        singleMapEventData = args[0] as SingleMapEventData;
        mapEventSetting = DataTable.FindMapEventSetting(singleMapEventData.SettingId);
        mapEventType = (MapEventType)mapEventSetting.Type.ToInt32();
        addBtnListener(btn1, () =>
        {
            ExecuteBtn(btn1);
        });

        addBtnListener(btn2, () =>
        {
            ExecuteBtn(btn2);
        });

        addBtnListener(btn_leave, () =>
         {
             PanelManager.Instance.ClosePanel(this);
         });
        addBtnListener(btn_close, () =>
        {
            PanelManager.Instance.PingPongBlackMask(() =>
            {
                PanelManager.Instance.ClosePanel(this);
            }, null);
        });
        addBtnListener(btn_quZhu, () =>
        {
            PanelManager.Instance.PingPongBlackMask(() =>
            {
                MapEventManager.Instance.RemoveMapEvent(singleMapEventData);

                PanelManager.Instance.ClosePanel(this);
            }, null);
        });
        RegisterEvent(TheEventType.ShowMapEventFinish, ReceiveShowFinish);
        RegisterEvent(TheEventType.SuccessRecruit, OnSuccessRecruitStudent);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        ClearCertainParentAllSingle<NameWordView>(trans_nameGrid);
        for (int i = 0; i < mapEventSetting.Name.Length; i++)
        {
            char word = mapEventSetting.Name[i];
            AddSingle<NameWordView>(trans_nameGrid, word);
        }

        //体力减少多少
        string consume = mapEventSetting.Consume;

        string btn2TxtStr = mapEventSetting.Btn2Txt;
        if (mapEventType == MapEventType.CheDiSouSuo)
        {
            btn2TxtStr = btn2TxtStr + "（预计体力-" + consume.ToInt32()*1.5f + ")";
            txt_dangerouse.ShowDangerouseTxt(mapEventSetting.DangerousLevel.ToInt32());
            trans_dangerouse.gameObject.SetActive(true);
        }
        else
        {
            trans_dangerouse.gameObject.SetActive(false);

        }

        ////必须要有体力
        //if (teamData.RemainBuJi <= consume)
        //{
        //        btn2TxtStr
        //}
        if (!string.IsNullOrWhiteSpace(consume))
        {
            txt_btn1.SetText(mapEventSetting.Btn1Txt + "（体力-" + consume + ")");

        }
        else
        {
            txt_btn1.SetText(mapEventSetting.Btn1Txt);
        }

        txt_btn2.SetText(btn2TxtStr);
        if (mapEventSetting.Type.ToInt32() == (int)MapEventType.CheDiSouSuo)
        {
            string[] param2Arr = mapEventSetting.Param2.Split('|');
            int zhanDouLiNeed = param2Arr[1].ToInt32();
            txt_des.SetText(mapEventSetting.Des + "（推荐战斗力：" + zhanDouLiNeed + ")");
        }
        else
            txt_des.SetText(mapEventSetting.Des);

        trans_first.gameObject.SetActive(true);
        trans_finish.gameObject.SetActive(false);

        //if (singleMapEventData.ExploreSettingId != 0)
        //{
        //    img_commonBg.gameObject.SetActive(false);
        //    img_exploreBg.gameObject.SetActive(true);
        
        //}
        //else
        //{
        //    img_commonBg.gameObject.SetActive(true);
        //    img_exploreBg.gameObject.SetActive(false);
        //}
        if(mapEventSetting.Type.ToInt32() == (int)MapEventType.Enemy)
        {
            btn_quZhu.gameObject.SetActive(true);
        }
        else
        {
            btn_quZhu.gameObject.SetActive(false);
        }
    }

    void ExecuteBtn(Button btn)
    {
        string param = mapEventSetting.Param;
        if (btn == btn1)
        {
            if (!string.IsNullOrWhiteSpace(mapEventSetting.Consume))
            {
                ExploreTeamData teamData = MapManager.Instance.FindExploreTeamDataByOnlyId(singleMapEventData.ExploreTeamOnlyId);
                if (teamData.RemainBuJi < mapEventSetting.Consume.ToInt32())
                {
                    PanelManager.Instance.OpenFloatWindow("体力不够");
                    return;
                }
            }
            switch (mapEventType)
            {
          

                //修炼
                case MapEventType.XiuLian:
                    PanelManager.Instance.PingPongBlackMask(() =>
                    {
                        PanelManager.Instance.ClosePanel(this);
                        //TrainPanel trainPanel= PanelManager.Instance.OpenPanel<TrainPanel>(PanelManager.Instance.trans_layer2,singleMapEventData);
                        //trainPanel.ShowXiuLianPanel();
                        MapEventManager.Instance.ExecuteMapEvent(singleMapEventData);

                    }, ShowFinish);
                    break;
                //矿
                case MapEventType.Kuang:
                    PanelManager.Instance.PingPongBlackMask(() =>
                    {
                        MapEventManager.Instance.ExecuteMapEvent(singleMapEventData);
                        ShowFinish();
                    }, ()=>
                    {
                       
                    });
                    break;
                //探险休息点
                case MapEventType.Rest:
                    PanelManager.Instance.PingPongBlackMask(() =>
                    {
                        MapEventManager.Instance.ExecuteMapEvent(singleMapEventData);
                        ShowFinish();
                    }, 
                    () =>
                    {
                      
                    }
                    
                    );
                    break;
                    //彻底搜索(选简单搜索）
                case MapEventType.CheDiSouSuo:

                    MapEventManager.Instance.ExecuteMapEvent(singleMapEventData);
                    break;
                    //弟子增加修为
                case MapEventType.Explore_XiuWei:
                    PanelManager.Instance.PingPongBlackMask(() =>
                    {
                        MapEventManager.Instance.ExecuteMapEvent(singleMapEventData);
                        ShowFinish();
                    },
                    () =>
                      {

                      }
                          );
                    break;
                //神算子
                case MapEventType.ShenSuanZi:
                    PanelManager.Instance.PingPongBlackMask(() =>
                    {
                        //PanelManager.Instance.ClosePanel(this);
                        //PanelManager.Instance.OpenPanel<TrainPanel>(PanelManager.Instance.trans_layer2, singleMapEventData);
                    }, ShowFinish);
                    break;
                //敌人
                case MapEventType.Enemy:
                    //PanelManager.Instance.PingPongBlackMask(() =>
                    //{
                    //    MapEventManager.Instance.ExecuteMapEvent(singleMapEventData);

                    //}, () =>
                    //{
                    //    //ShowFinish();
                    //});
                    List<PeopleData> enemyList = new List<PeopleData>();
                    for(int i = 0; i < singleMapEventData.PeopleList.Count; i++)
                    {
                        enemyList.Add(singleMapEventData.PeopleList[i]);
                    }
                    PanelManager.Instance.OpenPanel<BattlePreparePanel>(PanelManager.Instance.trans_layer2,
                        BattleType.MapEventBattle,
                        RoleManager.Instance.FindMyBattleTeamList(false, 0),
                        enemyList,
                        singleMapEventData
                        );
 
                    break;

                case MapEventType.RecruitStudent:
                    List<DialogData> diaLogList = new List<DialogData>();
                    DialogData dialogData = new DialogData(singleMapEventData.PeopleList[0], mapEventSetting.Param2);
                    diaLogList.Add(dialogData);
                    DialogManager.Instance.CreateDialog(diaLogList, () =>
                    {
                        PanelManager.Instance.OpenPanel<EventRecruitStudentPanel>(PanelManager.Instance.trans_layer2,singleMapEventData.PeopleList[0]);
                    });

                    break;
                case MapEventType.Reveal:
                    MapEventManager.Instance.ExecuteMapEvent(singleMapEventData);
                    PanelManager.Instance.ClosePanel(this);
                    break;
            }
        }
        else if (btn == btn2)
        {
            switch (mapEventType)
            {
                case MapEventType.XiuLian:
                    PanelManager.Instance.PingPongBlackMask(() =>
                    {
                        PanelManager.Instance.ClosePanel(this);
                    }, null);
                    break;
                    //选择打 出现敌人
                case MapEventType.CheDiSouSuo:
                    ExploreTeamData teamData = MapManager.Instance.FindSingleExploreDataById(singleMapEventData.ExploreSettingId).ExploreTeamData;
                 
                    if(teamData.RemainBuJi< mapEventSetting.Consume.ToInt32() * 1.5f)
                    {
                        PanelManager.Instance.OpenFloatWindow("补给不够");
                        return;
                    }
                    string[] param2Arr= mapEventSetting.Param2.Split('|');
                    List<DialogData> dialogList = new List<DialogData>();
                    DialogData data = new DialogData(null, param2Arr[0] + "（推荐战斗力:" + param2Arr[1] + ")");
                    dialogList.Add(data);
                    int zhanDouLiNeed = mapEventSetting.Param2.Split('|')[1].ToInt32();
                    //战斗力不足一半 则直接失败
                    int myZhan = MapManager.Instance.CalcExploreTeamZhanDouLi(teamData);
                    float rate = (myZhan - zhanDouLiNeed / 2) / (float)(zhanDouLiNeed / 2);
                    int theRate = (int)(rate * 100);
                    if (theRate >= 100)
                        theRate = 100;
                    if (theRate <= 0)
                        theRate = 0;
                    string btn1Str = "仓皇逃走(体力-" + mapEventSetting.Consume.ToInt32() * 1.5f + ")" ;
                    string btn2Str = "上前迎战(体力-" + mapEventSetting.Consume.ToInt32() * 1.5f + "，成功率" + theRate + "%)";
                    List<string> btnStrList = new List<string>() { btn1Str, btn2Str };
                    List<Action> actionList = new List<Action>() { Escape, YingZhan };
                    DialogManager.Instance.CreateDialog(dialogList, btnStrList, actionList);
                    break;
                default:
                    PanelManager.Instance.PingPongBlackMask(() =>
                    {
                        PanelManager.Instance.ClosePanel(this);
                    }, null);
                    break;
            }
        }
    }

    /// <summary>
    /// 事件中成功招募了弟子
    /// </summary>
    public void OnSuccessRecruitStudent(object[] args)
    {
        PeopleData p = args[0] as PeopleData;
        if(singleMapEventData.PeopleList.Count>0
            && singleMapEventData.PeopleList[0].onlyId == p.onlyId)
        {
            MapEventManager.Instance.ExecuteMapEvent(singleMapEventData);
            ShowFinish();
        }
    }
    /// <summary>
    /// 迎战 出结果 掉体力
    /// </summary>
    public void YingZhan()
    {
        MapEventManager.Instance.ExecuteMapEvent(singleMapEventData, true,2);
    }

    /// <summary>
    /// 逃跑 掉体力
    /// </summary>
    public void Escape()
    {
        MapEventManager.Instance.ExecuteMapEvent(singleMapEventData, false,2);

    }
    /// <summary>
    /// 结束
    /// </summary>
    void ReceiveShowFinish(object[] args)
    {
        string str = (string)args[0];
        trans_first.gameObject.SetActive(false);
        trans_finish.gameObject.SetActive(true);
        txt_des.SetText(str);

    }
    /// <summary>
    /// 结束
    /// </summary>
    void ShowFinish()
    {
        trans_first.gameObject.SetActive(false);
        trans_finish.gameObject.SetActive(true);
        txt_des.SetText(mapEventSetting.FinishShow);

    }
}

