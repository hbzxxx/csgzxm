using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using UnityEngine.UI;
using Framework.Data;
using System;

public class MiJingLevelView : SingleViewBase
{
    public SingleMiJingLevelData levelData;
    public MiJingLevelSetting setting;
    public Transform trans_lock;//锁
    public Text txt_name;//名字

    public Button btn_guard;//守卫
    public Button btn_saoDang;//扫荡
    public Text txt_guardNum;//守卫数量

    public Text txt_guardNextTime;//守卫需要时间

 
    public Transform trans_paiQianIng;//正在派遣的父物体
    public Image img_dayBar;//剩余多少天
    public Transform trans_studentShowGrid;//显示弟子
    public Button btn_paiQianDetail;//派遣详情
    public Transform trans_itemGrid;//奖励

    public CurWeekType curWeekType;

    public Text txt_daily;//每日高额
    public SingleMiJingPaiQianData mijingPaiQianData;

    public Transform trans_consumeGrid;

    public override void Init(params object[] args)
    {
        base.Init(args);
        levelData = args[0] as SingleMiJingLevelData;
        mijingPaiQianData = MiJingManager.Instance.GetSingleMiJingPaiQianDataById(levelData.MiJingId);
        setting = DataTable.FindMiJingLevelSetting(levelData.LevelId);
        curWeekType = (CurWeekType)MiJingManager.Instance.GetSingleMiJingPaiQianDataById(levelData.MiJingId).WeekType;
        //打守卫
        addBtnListener(btn_guard, () =>
        {
            //体力
            if(!RoleManager.Instance.CheckIfPropertyEnough((int)PropertyIdType.Tili, ConstantVal.taoFaNeedTiLi))
            {
                PanelManager.Instance.OpenPanel<TiliRevivePanel>(PanelManager.Instance.trans_layer2);

            }
            else
            {
                string enemy = "";
                if (curWeekType == CurWeekType.Week135)
                {
                    enemy = setting.Week135enemy;
                }
                else if (curWeekType == CurWeekType.Week246)
                {
                    enemy = setting.Week246enemy;
                }
                else
                {
                    enemy = setting.Week7enemy;

                }
                List<int> enemyIdList = CommonUtil.SplitCfgOneDepth(enemy);
                //for(int i=0;i< enemyIdList.Count; i++)
                //{
                //    EnemySetting enemySetting = DataTable.FindEnemySetting(setting.enemyId.ToInt32());

                //}
                BattleManager.Instance.StartMiJingGuardBattle(setting, enemyIdList);
            }


    
            
        });

        addBtnListener(btn_paiQianDetail, () =>
        {
            PanelManager.Instance.OpenPanel<PaiQianHarvestPanel>(PanelManager.Instance.trans_layer2, levelData);
        });

        addBtnListener(btn_saoDang, () =>
        {
            if (!RoleManager.Instance.CheckIfPropertyEnough((int)PropertyIdType.Tili, ConstantVal.taoFaNeedTiLi))
            {
                PanelManager.Instance.OpenPanel<TiliRevivePanel>(PanelManager.Instance.trans_layer2);

            }
            else
            {
                BattleManager.Instance.SaoDang(BattleType.MiJingGuardBattle, setting.Id);

            }

        });

        RegisterEvent(TheEventType.OnSaoDang, OnSaoDang);

    }



    public override void OnOpenIng()
    {
        base.OnOpenIng();
        PanelManager.Instance.CloseAllSingle(trans_itemGrid);

        string awardStr = "";
        string str1 = "";
        string str2 = "";
        if (curWeekType == CurWeekType.Week135)
        {
            //awardStr = setting.week135possibleBook + "|" + setting.week135Mat;
            str1 = setting.Week135possibleBook+"|";
            str2 = setting.Week135Mat;
        }
        else if (curWeekType == CurWeekType.Week246)
        {
            //awardStr = setting.week246possibleBook + "|" + setting.week246Mat;
            str1 = setting.Week246possibleBook + "|";
            str2 = setting.Week246Mat;


        }
        else
        {
            //awardStr = setting.week7possibleBook + "|" + setting.week7Mat;
            str1 = setting.Week7possibleBook+"|";
            str2 = setting.Week7Mat;

        }
        if (setting.TaoFaType.ToInt32() == (int)TaoFaType.ShenQiMuDi)
        {
        
            int rarity = MiJingManager.Instance.AwardRarityByMiJingLevel(setting.Level.ToInt32());

            List<int> paramList = CommonUtil.SplitCfgOneDepth(str1);
            str1 = "";
            for (int i = 0; i < paramList.Count; i++)
            {
                int param = paramList[i];
                if (param == 0)
                    continue;
                List<int> choosedTuZhiList = DataTable.FindTaoZhuangYuanPeiIdListByTaoZhuangType(param, (Rarity)rarity);
                int tuZhiIndex = RandomManager.Next(0, choosedTuZhiList.Count);
                if (tuZhiIndex >= choosedTuZhiList.Count)
                {

                }
                if (choosedTuZhiList.Count > 0)
                {
                    int choosedId = choosedTuZhiList[tuZhiIndex];
                    str1 += choosedId + "|";
                }
            }
        }
        awardStr = str1 + str2;
        List<int> award = CommonUtil.SplitCfgOneDepth(awardStr);
        
        for(int i = 0; i < award.Count; i++)
        {
            ItemData item = new ItemData();
            int itemId = award[i];
            if (itemId == 0)
                continue;
            item.settingId = itemId;
            item.count = 0;
            PanelManager.Instance.OpenSingle<ShowTipsItemView>(trans_itemGrid, item);
        }


        if (levelData.AccomplishStatus == (int)MiJingLevelAccomplishType.Locked)
        {
            trans_lock.gameObject.SetActive(true);
            btn_saoDang.gameObject.SetActive(false);

            btn_guard.gameObject.SetActive(false);
            txt_guardNum.gameObject.SetActive(false);
            txt_guardNextTime.gameObject.SetActive(false);
             trans_paiQianIng.gameObject.SetActive(false);
        }
        //解锁了但没通过关
        else if(levelData.AccomplishStatus == (int)MiJingLevelAccomplishType.UnAccomplished)
        {
            trans_lock.gameObject.SetActive(false);
            btn_saoDang.gameObject.SetActive(false);

            btn_guard.gameObject.SetActive(true);
            txt_guardNum.gameObject.SetActive(true);
            txt_guardNum.SetText("首次");
            txt_guardNextTime.gameObject.SetActive(false);
             trans_paiQianIng.gameObject.SetActive(false);

        }
        //通过关
        else
        {
            trans_lock.gameObject.SetActive(false);
            btn_saoDang.gameObject.SetActive(true);
            ////是最高关才有守卫
            //if (MiJingManager.Instance.GetSingleMiJingPaiQianDataById(setting.miJingId.ToInt32()).HighestLevelLevel == setting.level.ToInt32())
            //{

            //}
            btn_guard.gameObject.SetActive(true);
            SingleMiJingPaiQianData mijingData = MiJingManager.Instance.GetSingleMiJingPaiQianDataById(levelData.MiJingId);
            //有守卫
            if (mijingData.DayliHighNum > 0)
            {
                txt_guardNum.gameObject.SetActive(true);
                txt_guardNum.SetText("每日高额（"+mijingData.DayliHighNum + "/" +mijingData.MaxDayliHighNum+"）");

                txt_guardNextTime.gameObject.SetActive(false);
             }
            else
            {
                txt_guardNum.gameObject.SetActive(true);
                txt_guardNum.SetText("每日高额（" + mijingData.DayliHighNum + "/" + mijingData.MaxDayliHighNum + "）");

                long guardNextTimeDistance = CGameTime.Instance.GetTo24TimeStampByTimeStamp(mijingData.LastKillMonsterTime);
                long theNextTimeStamp = guardNextTimeDistance + mijingData.LastKillMonsterTime;
                long nowToNextTimeDistance = theNextTimeStamp - CGameTime.Instance.GetTimeStamp();
                long hour = nowToNextTimeDistance / 3600;
                long min = (nowToNextTimeDistance - hour * 3600) / 60;
                long sec = nowToNextTimeDistance - hour * 3600 - min * 60;
                txt_guardNextTime.SetText("下次刷新："+ hour + "时" + min + "分");

                txt_guardNextTime.gameObject.SetActive(true);
                //btn_guard.gameObject.SetActive(false);

                //没守卫判断显示派遣按钮与否
                //如果该位置没弟子
            }


        }

        PanelManager.Instance.CloseAllSingle(trans_consumeGrid);
        PanelManager.Instance.OpenSingle<WithoutKuangConsumeView>(trans_consumeGrid, PropertyIdType.Tili, ConstantVal.taoFaNeedTiLi,ConsumeType.Property);
        txt_name.SetText(setting.Des);

        //txt_daily.SetText(mijingPaiQianData.DayliHighNum + "/" + 2);
    }

   
    /// <summary>
    /// 扫荡
    /// </summary>
    /// <param name="args"></param>
    void OnSaoDang(object[] args)
    {
        BattleType type = (BattleType)args[0];
        if (type == BattleType.MiJingGuardBattle)
            OnOpenIng();
    }


    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.CloseAllSingle(trans_studentShowGrid);
        PanelManager.Instance.CloseAllSingle(trans_itemGrid);
        PanelManager.Instance.CloseAllSingle(trans_consumeGrid);
    }
}


/// <summary>
/// 讨伐类型
/// </summary>
public enum TaoFaType
{
    YunHu=1,
    XingChenJian=2,
    QingLingYu=3,
    ShenQiMuDi=4,//神器墓地
}