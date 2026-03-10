 using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;

public class ADManager : MonoInstance<ADManager>
{
    public ADType curADType;
     public bool finishWathAD;
  
    public void WatchAD(ADType aDType)
    {
         
        finishWathAD = false;
        curADType = aDType;
        
        // 编辑器模式或banHaoMode下直接完成广告观看
#if UNITY_EDITOR
        OnFinishWatchAD();
        return;
#endif
        if (Game.Instance.banHaoMode)
        {
            OnFinishWatchAD();
            return;
        }
        
        if (GameTimeManager.Instance.connectedToFuWuQiTime
            &&ShopManager.Instance.CheckIfHaveMoonCard3(GameTimeManager.Instance.curFuWuQiTime)!=null)
        {
            OnFinishWatchAD();
        }
        else
        {
            if (ItemManager.Instance.CheckIfHaveItemBySettingId((int)ItemIdType.FreeAD))
            {
                if (ItemManager.Instance.LoseItem((int)ItemIdType.FreeAD, 1))
                {
                    OnFinishWatchAD();
                }
            }
            else
            {
                //判断是否到了新的一天
                GameTimeManager.Instance.GetServiceTime((x) =>
                {
                    if (x > 0)
                    {
                        //到了新的一天
                        if (x - RoleManager.Instance._CurGameInfo.timeData.LastADWatchTime >= CGameTime.Instance.GetTo24TimeStampByTimeStamp(RoleManager.Instance._CurGameInfo.timeData.LastADWatchTime))
                        {
                            RoleManager.Instance._CurGameInfo.timeData.TotalADWatchNum = 0;
                        }

                        if (RoleManager.Instance._CurGameInfo.timeData.TotalADWatchNum >= ConstantVal.maxADNumPerDay)
                        {
                            PanelManager.Instance.OpenFloatWindow("今日广告次数已耗尽");
                        }
                        else
                        {
                            //判断两个广告间隔
                            if (x - RoleManager.Instance._CurGameInfo.timeData.LastADWatchTime < ConstantVal.adTimeOffset)
                            {
                                PanelManager.Instance.OpenFloatWindow("您观看广告太频繁了，请稍候片刻");
                            }
                            else
                            {
                                SDKManager.Instance.showVideo();
                            }
                        }
                    }


                });
            }
        }
       
  
     
    }
    private void Update()
    {
        if (finishWathAD)
        {
            OnFinishWatchAD();
            finishWathAD = false;
        }
    }

    public void OnFinishWatchAD()
    {
        GameTimeManager.Instance.GetServiceTime((x) =>
        {
            if (x > 0)
            {
                if (x - RoleManager.Instance._CurGameInfo.timeData.LastADWatchTime >= ConstantVal.adNeedMinTime)
                {
                    switch (curADType)
                    {
                        //case ADType.ReviveTiLi:
                        //    RoleManager.Instance.AddProperty(PropertyIdType.Tili, ConstantVal.adReviveTiliNum);
                        //    RoleManager.Instance._CurGameInfo.timeData.TodayADTiliNum++;
                          
                        //    RoleManager.Instance._CurGameInfo.timeData.LastADReviveTiliTime = x;
                          
                        //    TDGAItem.OnPurchase("广告_买体力", ConstantVal.adReviveTiliNum, 1);
                        //    break;
                        case ADType.ReceiveOfflineIncome:
                            RoleManager.Instance.ReceiveOfflineIncome(true);
                            PanelManager.Instance.CloseOfflineIncomePanel();
                            TDGAItem.OnPurchase("广告_双倍离线收益", 1, 1);
                            break;
                        //case ADType.JieSuanLieXi:
                        //    MapManager.Instance.LeaveMap(true);
                        //    TDGAItem.OnPurchase("广告_双倍裂隙收益", 1, 1);
                        //    break;
                        //case ADType.TalentTest:
                        //    EventCenter.Broadcast(TheEventType.OnADRerdm);
                        //    TDGAItem.OnPurchase("广告_改天赋", 1, 1);
                        //    break;
                        //case ADType.MiJingDouble:
                        //    EventCenter.Broadcast(TheEventType.OnADMiJingDouble);
                        //    TDGAItem.OnPurchase("广告_双倍秘境收益", 1, 1);
                        //    break;
                        //case ADType.ADBrushStudent:
                        //    StudentManager.Instance.OnSuccessfulADBrushStudent();
                        //    TDGAItem.OnPurchase("广告_刷新弟子", 1, 1);
                        //    break;
                        case ADType.ADRecruitStudentNum:
                            StudentManager.Instance.ADAddRecruitStudentLimit();
                            TDGAItem.OnPurchase("广告_增加弟子招募上限", 1, 1);
                            break;
                        //case ADType.AddMatchParticipateNum:
                        //    MatchManager.Instance.OnADAddMatchNum();
                        //    TDGAItem.OnPurchase("广告_增加大比次数", 1, 1);
                        //    break;
                        case ADType.DailyFuli:
                            ShopManager.Instance.OnDailyADBuy();
                            TDGAItem.OnPurchase("广告_每日福利观看", 1, 1);
                            break;
                        case ADType.JieSuanLiLian:
                            LiLianManager.Instance.OnGetAward(true);
                            TDGAItem.OnPurchase("广告_历练双倍领取", 1, 1);
                            break;
                    }

                    RoleManager.Instance._CurGameInfo.timeData.TotalADWatchNum++;
                    RoleManager.Instance._CurGameInfo.AllADData.allTotalADWatchNum++;
                    ItemManager.Instance.GetItem((int)ItemIdType.GuangGaoLing, 1);
                    EventCenter.Broadcast(TheEventType.OnSuccessWatchGuangGao);
                    if (ConstantVal.maxADNumPerDay - RoleManager.Instance._CurGameInfo.timeData.TotalADWatchNum == 10)
                    {
                        PanelManager.Instance.OpenOnlyOkHint("今日广告次数还剩" + (ConstantVal.maxADNumPerDay - RoleManager.Instance._CurGameInfo.timeData.TotalADWatchNum) + "次，请注意护肝养肝。", null);
                    }

                }
                else
                {
                    Game.Instance.errBeng = true;
                }
            }
        });

    }

    /// <summary>
    /// 领取总广告次数奖励福利
    /// </summary>
    public void OnLingQuTotalADAward()
    {
        TotalADAwardSetting nextAwardSetting = null;

        //总次数
        for (int i = 0; i < DataTable._totalADAwardList.Count; i++)
        {
            if (RoleManager.Instance._CurGameInfo.AllADData.getAwardTotalADWatchNumIndex + 1 == i)
            {
                nextAwardSetting = DataTable._totalADAwardList[i];
            }
        }
        if (nextAwardSetting != null)
        {
            if(RoleManager.Instance._CurGameInfo.AllADData.allTotalADWatchNum >= nextAwardSetting.Id.ToInt32())
            {
                List<List<int>> award = CommonUtil.SplitCfg(nextAwardSetting.Award);
                for (int i = 0; i < award.Count; i++)
                {
                    List<int> singleAward = award[i];
                    ItemManager.Instance.GetItemWithTongZhiPanel(singleAward[0], (ulong)singleAward[1]);

                }
                RoleManager.Instance._CurGameInfo.AllADData.getAwardTotalADWatchNumIndex = DataTable._totalADAwardList.IndexOf(nextAwardSetting);
                EventCenter.Broadcast(TheEventType.RefreshAllGuangGaoAwardShow);
                //TalkingDataGA.OnEvent("领取广告次数奖励", new Dictionary<string, object>() { { nextAwardSetting.Id, 1 } });

            }


        }
    }
}


/// <summary>
/// 看什么类型广告
/// </summary>
public enum ADType
{
    None=0,
    ReviveTiLi,//恢复源力
    ReceiveOfflineIncome,//离线收益
    JieSuanLieXi,//结算裂隙双倍
    TalentTest,//天赋觉醒
    MiJingDouble,//秘境双倍领取
    ADBrushStudent,//广告刷弟子
    ADRecruitStudentNum,//弟子招募上限
    AddMatchParticipateNum,//增加大比次数
    DailyFuli,//每日福利广告
    JieSuanLiLian,//结算历练
}