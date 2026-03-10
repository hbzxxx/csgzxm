using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using UnityEngine.UI;
using DG.Tweening;
using Framework.Data;

/// <summary>
/// 战斗准备面板
/// </summary>
public class BattlePreparePanel : PanelBase
{
    public Transform trans_possibleDiaoLuo;//可能掉落

    public Transform iconStartPos;
    public Transform iconCenterPos;
    public Transform icon;

    public Transform trans_enemyParent;//敌人父物体
    public List<PeopleData> enemyList;//敌人
    public Text txt_enemyZhanDouLi;

    public Transform trans_myGrid;//我的父物体
    public List<PeopleData> myList;// 我的队伍
    public Text txt_myZhanDouLi;//我的战斗力

    public BattleType battleType;
    //public int levelId;//关卡id

    public Button btn_startBattle;
    SingleMapEventData singleMapEventData = null;
    public GameObject obj_consume;
    public Text txt_consumeNum;
    int tiliConsume;

    public Transform play0;
    public Transform enemy0;
    public override void Init(params object[] args)
    {
        base.Init(args);
        battleType = (BattleType)args[0];

        myList= args[1] as List<PeopleData>;
        enemyList = args[2] as List<PeopleData>;
        if (battleType == BattleType.MapEventBattle)
        {
            singleMapEventData = args[3] as SingleMapEventData;
        }

        addBtnListener(btn_startBattle, () =>
        {
            bool tiliEnough = false;
            if (tiliConsume > 0)
            {
                if (RoleManager.Instance.CheckIfPropertyEnough((int)PropertyIdType.Tili, tiliConsume))
                {
                    tiliEnough = true;
                }
                else
                {
                    tiliEnough = false;
                }
            }
            else
            {
                tiliEnough = true;

            }
            if (tiliEnough)
            {
                switch (battleType)
                {
                    case BattleType.LevelBattle:

                        BattleManager.Instance.StartLevelBattle(MapManager.Instance.curChoosedLevelId);
                        break;
                    case BattleType.FixedLevelBattle:
                        BattleManager.Instance.StartFixedLevelBattle(MapManager.Instance.curChoosedLevelId);
                        break;
                    case BattleType.MapEventBattle:
                        BattleManager.Instance.StartMapEventBattle(singleMapEventData, myList, enemyList);
                        break;
                    case BattleType.Match:
                        BattleManager.Instance.StartBattle();
                        break;
                    default:
                        //BattleManager.Instance.StudentBattle(battleType, choosedpeopleView.PeopleData);
                        break;

                }
            }
            else
            {
                PanelManager.Instance.OpenPanel<TiliRevivePanel>(PanelManager.Instance.trans_layer2);
            }

 

        });

        RegisterEvent(TheEventType.CloseBattlePreparePanel, CloseThePanel);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        long totalMyZhanDouLi = 0;

        ClearCertainParentAllSingle<BattlePreparePeopleView>(trans_myGrid);
        ClearCertainParentAllSingle<BattlePreparePeopleZhangmen>(play0);

        for (int i = 0; i < myList.Count; i++)
        {
            PeopleData p = myList[i];
            if (p.isPlayer || p.teamPosIndex == 0)
            {
                BattlePreparePeopleZhangmen viewZhangme = PanelManager.Instance.OpenSingle<BattlePreparePeopleZhangmen>(play0,p);
            }
            else
            {
                BattlePreparePeopleView view = PanelManager.Instance.OpenSingle<BattlePreparePeopleView>(trans_myGrid, p);
            }
            totalMyZhanDouLi += RoleManager.Instance.CalcZhanDouLi(p);
        }
        txt_myZhanDouLi.SetText(totalMyZhanDouLi.ToString());


        ClearCertainParentAllSingle<BattlePreparePeopleView>(trans_enemyParent);
        ClearCertainParentAllSingle<BattlePreparePeopleZhangmen>(enemy0);
        long totalEnemyZhanDouLi = 0;
        for(int i = 0; i < enemyList.Count; i++)
        {
            PeopleData enemy = enemyList[i];
            if (enemy.teamPosIndex == 0)
            {
                BattlePreparePeopleZhangmen viewZhangme = PanelManager.Instance.OpenSingle<BattlePreparePeopleZhangmen>(enemy0, enemy);
            }
            else
            {
                BattlePreparePeopleView enemyView = AddSingle<BattlePreparePeopleView>(trans_enemyParent, enemy);
            }
           totalEnemyZhanDouLi+= RoleManager.Instance.CalcZhanDouLi(enemy);

        }
        txt_enemyZhanDouLi.SetText(totalEnemyZhanDouLi.ToString());


 

        icon.DOKill();
        icon.localScale = new Vector3(1, 1, 1);
        icon.localPosition = iconStartPos.transform.localPosition;
        icon.DOLocalMoveX(iconCenterPos.localPosition.x, 0.5f);
        ShowPossibleDiaoLuo();
        if (tiliConsume > 0)
        {
            obj_consume.SetActive(true);
            txt_consumeNum.SetText((-tiliConsume).ToString());
        }
        else
        {
            obj_consume.SetActive(false);

        }
    }

    /// <summary>
    /// 可能掉落
    /// </summary>
    void ShowPossibleDiaoLuo()
    {
        ClearCertainParentAllSingle<ShowTipsItemView>(trans_possibleDiaoLuo);
        switch (battleType)
        {
            case BattleType.LevelBattle:
                LevelSetting levelSetting = DataTable.FindLevelSetting(MapManager.Instance.curChoosedLevelId);
                List<List<int>> award = CommonUtil.SplitCfg(levelSetting.Award);
                for(int i = 0; i < award.Count; i++)
                {
                    List<int> single = award[i];
                    ItemData item = new ItemData();
                    item.settingId = single[0];
                    item.count = (ulong)single[1];
                    if (item.count > 0)
                        AddSingle<ShowTipsItemView>(trans_possibleDiaoLuo, item);
                }
                tiliConsume = ConstantVal.levelBattleTiliConsume;
                btn_emptyClose.gameObject.SetActive(true);
                 break;
            case BattleType.FixedLevelBattle:
                LevelSetting levelSetting2 = DataTable.FindLevelSetting(MapManager.Instance.curChoosedLevelId);
                if (!string.IsNullOrWhiteSpace(levelSetting2.Award))
                {
                    List<List<int>> award2 = CommonUtil.SplitCfg(levelSetting2.Award);
                    for (int i = 0; i < award2.Count; i++)
                    {
                        List<int> single = award2[i];
                        ItemData item = new ItemData();
                        item.settingId = single[0];
                        item.count = (ulong)single[1];
                        if (item.count > 0)
                            AddSingle<ShowTipsItemView>(trans_possibleDiaoLuo, item);
                    }
                }
            
                tiliConsume = ConstantVal.fixedlevelBattleTiliConsume;
                btn_emptyClose.gameObject.SetActive(true);

                break;
            case BattleType.MapEventBattle:
                MapEventSetting eventSetting = DataTable.FindMapEventSetting(singleMapEventData.SettingId);
                List<List<int>> award3 = CommonUtil.SplitCfg(eventSetting.Param);
                for (int i = 0; i < award3.Count; i++)
                {
                    List<int> single = award3[i];
                    ItemData item = new ItemData();
                    item.settingId = single[0];
                    item.count = (ulong)single[1];
                        AddSingle<ShowTipsItemView>(trans_possibleDiaoLuo, item);
                }
                tiliConsume = ConstantVal.mapEventBattleTiliConsume;
                btn_emptyClose.gameObject.SetActive(true);


                break;
            case BattleType.Match:
                tiliConsume = 0;
                btn_emptyClose.gameObject.SetActive(false);
                break;
            default:
                tiliConsume = 0;
                btn_emptyClose.gameObject.SetActive(true);

                break;
        }
    }

    public void CloseThePanel()
    {
        PanelManager.Instance.ClosePanel(this);
    }


    public override void Clear()
    {
        base.Clear();
        //PanelManager.Instance.CloseAllSingle(trans_studentGrid);
        //prepareBattleViewList.Clear();

        //PanelManager.Instance.CloseAllSingle(trans_choosedPeopleParentGrid);
        //PanelManager.Instance.CloseAllSingle(trans_choosedPeopleProGrid);

        PanelManager.Instance.CloseAllSingle(trans_myGrid);
        PanelManager.Instance.CloseAllSingle(play0);
        
        PanelManager.Instance.CloseAllSingle(trans_enemyParent);
        PanelManager.Instance.CloseAllSingle(enemy0);
        //PanelManager.Instance.CloseAllSingle(trans_enemyProGrid);

    }


}
