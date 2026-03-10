using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using Framework.Data;
using cfg;
public class MiJingManager : CommonInstance<MiJingManager>
{
    public int curBattleMijingLevelId = 0;//当前是打哪个秘境守卫


    /// <summary>
    /// 通过id得到派遣数据
    /// </summary>
    /// <returns></returns>
    public SingleMiJingPaiQianData GetSingleMiJingPaiQianDataById(int id)
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllMiJingPaiQianData.PaiqianList.Count; i++)
        {
            SingleMiJingPaiQianData data = RoleManager.Instance._CurGameInfo.AllMiJingPaiQianData.PaiqianList[i];
            if (data.SettingId == id)
                return data;
        }
        return null;
    }



    ///// <summary>
    ///// 所通过的最高关卡
    ///// </summary>
    ///// <returns></returns>
    //public int GetAccomplishedHigheastLevel(int MiJingId)
    //{
    //    int highestLevel = 0;
    //    for(int i = 0; i < DataTable._miJingLevelList.Count; i++)
    //    {
    //        MiJingLevelSetting singleLevelSetting = DataTable._miJingLevelList[i];
    //        if (singleLevelSetting.miJingId.ToInt32() == MiJingId)
    //        {
    //            if (FindMiJingLevelDataById(singleLevelSetting.id.ToInt32()).AccomplishStatus
    //                == (int)MiJingLevelAccomplishType.Accomplished)
    //            {
    //                highestLevel=
    //            }
    //        }
    //    }
    //}
    /// <summary>
    /// 通过id找单个秘境关卡
    /// </summary>
    /// <returns></returns>
    public SingleMiJingLevelData FindMiJingLevelDataById(int levelId)
    {
        MiJingLevelSetting miJingLevelSetting = DataTable.FindMiJingLevelSetting(levelId);
        int miJingId = miJingLevelSetting.MiJingId.ToInt32();
        SingleMiJingPaiQianData singleMiJingData = GetSingleMiJingPaiQianDataById(miJingId);

        for(int i = 0; i < singleMiJingData.LevelList.Count; i++)
        {
            if (singleMiJingData.LevelList[i].LevelId == levelId)
                return singleMiJingData.LevelList[i];
        }
        return null;
    }

    /// <summary>
    /// 时间流逝一分钟
    /// </summary>
   public void TimeProcess(long curTime)
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllMiJingPaiQianData.PaiqianList.Count; i++)
        {
            SingleMiJingPaiQianData singleMiJingPaiQianData = RoleManager.Instance._CurGameInfo.AllMiJingPaiQianData.PaiqianList[i];
            int higheastLevel = singleMiJingPaiQianData.HighestLevelLevel;
            if (higheastLevel <= 0)
                continue;

            long lastKillMonsterTime = singleMiJingPaiQianData.LastKillMonsterTime;
            long refreshTime = lastKillMonsterTime + CGameTime.Instance.GetTo24TimeStampByTimeStamp(lastKillMonsterTime);

            if (curTime >= refreshTime)
            {
                singleMiJingPaiQianData.LastKillMonsterTime = curTime;
            
 
                singleMiJingPaiQianData.DayliHighNum = singleMiJingPaiQianData.MaxDayliHighNum;
                System.DayOfWeek dayOfWeek = CGameTime.Instance.GetDayOfWeekByTimeStamp(curTime);
                CurWeekType weekType = GetWeekTypeByWeekDay(dayOfWeek);
                singleMiJingPaiQianData.WeekType = (int)weekType;
                EventCenter.Broadcast(TheEventType.RefreshMiJingGuardShow);

            }
            RefreshTaoFaLimit(curTime);
            //for (int j = 0; j < higheastLevel; j++)
            //{
            //    SingleMiJingLevelData levelData = singleMiJingPaiQianData.LevelList[j];
            //    //不锁 则看角色通过的最高关卡是否本关

            //    //有可能出现守卫
            //    long lastKillMonsterTime = levelData.LastKillMonsterTime;
            //    long refreshTime = lastKillMonsterTime + CGameTime.Instance.GetTo24TimeStampByTimeStamp(lastKillMonsterTime);
            //    //刷新
            //    if (CGameTime.Instance.GetTimeStamp() >= refreshTime)
            //    {
            //        levelData.LastKillMonsterTime = CGameTime.Instance.GetTimeStamp();
            //        levelData.GuardNum = 2;
            //        //刷新
            //    }
            //}



        }
    }
    /// <summary>
    /// 月卡增加讨伐次数
    /// </summary>
    public void OnMoonCardAddTaoFaLimit(bool newAdd)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllMiJingPaiQianData.PaiqianList.Count; i++)
        {
            SingleMiJingPaiQianData data = RoleManager.Instance._CurGameInfo.AllMiJingPaiQianData.PaiqianList[i];
            data.MaxDayliHighNum = 3;
            if (newAdd)
            {
                data.DayliHighNum++;
                if (data.DayliHighNum >= data.MaxDayliHighNum)
                    data.DayliHighNum = data.MaxDayliHighNum;
            }
        }

    }
    /// <summary>
    /// 刷新讨伐次数
    /// </summary>
    public void RefreshTaoFaLimit(long curTime)
    {
        if (curTime > 0)
        {
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllMiJingPaiQianData.PaiqianList.Count; i++)
            {
                SingleMiJingPaiQianData singleData = RoleManager.Instance._CurGameInfo.AllMiJingPaiQianData.PaiqianList[i];

                RefreshSingleTaoFaLimit(singleData, curTime);

            }
        }
    }
    public void RefreshSingleTaoFaLimit(SingleMiJingPaiQianData singleData,long curTime)
    {
        //月卡
        if (ShopManager.Instance.CheckIfHaveMoonCard2(curTime) != null)
        {
            singleData.MaxDayliHighNum = 3;
        }
        else
        {
            singleData.MaxDayliHighNum = 2;
        }

    }
    /// <summary>
    /// 打败守卫
    /// </summary>
    public void BeatedGuard()
    {
        SingleMiJingLevelData data = FindMiJingLevelDataById(curBattleMijingLevelId);
        SingleMiJingPaiQianData singleMiJingData = GetSingleMiJingPaiQianDataById(data.MiJingId);


        //未通关
        if (data.AccomplishStatus ==(int)MiJingLevelAccomplishType.UnAccomplished)
        {
            data.AccomplishStatus = (int)MiJingLevelAccomplishType.Accomplished;
            //解锁下一关
            //SingleMiJingPaiQianData miJingData = GetSingleMiJingPaiQianDataById(data.MiJingId);
            MiJingLevelSetting miJingLevelSetting = DataTable.FindMiJingLevelSetting(data.LevelId);
            singleMiJingData.HighestLevelLevel = miJingLevelSetting.Level.ToInt32();
            //如果有下一关
            if (singleMiJingData.HighestLevelLevel < singleMiJingData.LevelList.Count)
            {
                singleMiJingData.LevelList[singleMiJingData.HighestLevelLevel].AccomplishStatus =(int)MiJingLevelAccomplishType.UnAccomplished;
            }
        }
        //已通关
        //else
        //{
            if (singleMiJingData.DayliHighNum > 0)
                singleMiJingData.DayliHighNum--;
        //}
        GameTimeManager.Instance.GetServiceTime((x) =>
        {
            if(x>0)
            singleMiJingData.LastKillMonsterTime = x;

        }
 );
    }

    public CurWeekType GetWeekTypeByWeekDay(System.DayOfWeek dayOfWeek)
    {
        if(dayOfWeek==System.DayOfWeek.Monday
            || dayOfWeek == System.DayOfWeek.Wednesday
            ||  dayOfWeek == System.DayOfWeek.Friday)
        {
            return CurWeekType.Week135;
        }else if(dayOfWeek == System.DayOfWeek.Tuesday
            || dayOfWeek == System.DayOfWeek.Thursday
            || dayOfWeek == System.DayOfWeek.Saturday)
        {
            return CurWeekType.Week246;

        }
        return CurWeekType.Week7;
    }


    public int AwardRarityByMiJingLevel(int level)
    {
        return (level-1) / 3+1;
    }
}

public enum MiJingLevelAccomplishType
{
    None=0,
    Locked=1,//锁定
    UnAccomplished=2,//未完成可以打
    Accomplished=3,//已完成
}

/// <summary>
/// 当前星期类型
/// </summary>
public enum CurWeekType
{
    None=0,
    Week135,
    Week246,
    Week7,
}