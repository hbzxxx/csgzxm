using Framework.Data;
using Newtonsoft.Json.Serialization;
 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPLoadFTP;
/// <summary>
/// 游戏时间管理 所有时间在此管理
/// </summary>
public class GameTimeManager:MonoInstance<GameTimeManager>
{
    //private static GameTimeManager inst = null;

    //public static GameTimeManager Instance
    //{
    //    get
    //    {
    //        if (inst == null)
    //        {
    //            inst = new GameTimeManager();
    //        }
    //        return inst;
    //    }

    //}
    public int todayUploadArchiveNum;
    public int todayDownloadArchiveNum;

    public TimeData _CurTimeData;
    public int processTest;

    public int timeBlockCount;//阻止时间的面板

    public bool timeMoving;//正在走

    public float lastAddXiuWeiTimer;//上次增加修为的时间

    public bool connectedToFuWuQiTime;//获取到了服务器时间
    public long curFuWuQiTime;//当前服务器时间
    public override void Init()
    {
        lastAddXiuWeiTimer = 0;
        _CurTimeData = RoleManager.Instance._CurGameInfo.timeData;

        int singlePhase = 100 / 9;
        int left = 0;
        int right = 0;
        for (int i = 0; i < 8; i++)
        {
            right += singlePhase;
            dayPhaseRangeArr[i] = new Vector2Int(left, right);
            left += singlePhase;
        }
        dayPhaseRangeArr[8] = new Vector2Int(left, 100);
        //lastDayPhaseIndex =CommonUtil.GetPhaseIndex((int)_CurTimeData.DayProcess, dayPhaseRangeArr);

        ////时间开始走暂时放在这里
        //if(RoleManager.Instance._CurGameInfo.CurGameModule==(int)GameModuleType.WeekDay)
        //    startMove = true;
        StartMove();
        DayStartEvent();

        if (!PlayerPrefs.HasKey("todayUploadArchiveNum"))
        {
            PlayerPrefs.SetInt("todayUploadArchiveNum", 0);
        }
        todayUploadArchiveNum = PlayerPrefs.GetInt("todayUploadArchiveNum");
        if (!PlayerPrefs.HasKey("todayDownloadArchiveNum"))
        {
            PlayerPrefs.SetInt("todayDownloadArchiveNum", 0);
        }
        todayDownloadArchiveNum = PlayerPrefs.GetInt("todayDownloadArchiveNum");
    }

    //public int curYear;//第几年
    //public int curMoon;//第几月

    public float dayProcessSpeed=1;//走一天的时间（单位秒 后续配表）
    float realdayProcessSpeed;//真实的速度 受到速度控制的影响
    public bool startMove = false;//开始走
    public Vector2Int[] dayPhaseRangeArr=new Vector2Int[9];//一天的阶段（暂时平均分成9段）
    public int lastDayPhaseIndex =-1;//上个阶段（这个值改变代表上了一节课）
 
    /// <summary>
    /// 时间开始走
    /// </summary>
    public void StartMove()
    {
        startMove = true;
    }
    /// <summary>
    /// 时间停止开始走
    /// </summary>
    public void StopMove()
    {
        startMove = false ;
    }

    /// <summary>
    /// 结束今天
    /// </summary>
    public void EndDay()
    {
        EquipmentManager.Instance.OnEquipMakeProcess();
        DayPlus();
        //是周六，回家


        //一天开始时 丹进度显示
        LianDanManager.Instance.StartProcessDanFarm();
    }
    /// <summary>
    /// 开始新的一天
    /// </summary>
    public void StartNewDay()
    {

        //Action finishMask = delegate ()
        //{
        //    PanelManager.Instance.ClosePanel(PanelManager.Instance.GetPanel<BlackMaskPanel>());
        //    lastDayPhaseIndex = 0;
        //    startMove = true;
        //    //EventCenter.Broadcast(TheEventType.OnNewDayStart);

        //};
        ////GameObject.Find("WorkDayPanel").GetComponent<WorkDayPanel>().OnNewDayStart();

        //PanelManager.Instance.OpenPanel<BlackMaskPanel>(PanelManager.Instance.trans_commonPanelParent, BlackMaskType.Open, finishMask);

    }

    private void Update()
    {
   
        if (startMove
            &&timeBlockCount<=0
            )
        {

            //return;
            timeMoving = true;
            DayProcess(Time.deltaTime);
        }
        else
        {
            timeMoving = false;
        }

        ////现实时间
        //if (RoleManager.Instance.initOk)
        //{
        //    lastAddXiuWeiTimer += Time.deltaTime;
        //    if (lastAddXiuWeiTimer >= 6)
        //    {
        //        lastAddXiuWeiTimer = 0;
        //        RoleManager.Instance.TryAddXiuWei();
        //    }
        //    EventCenter.Broadcast(TheEventType.OnProcessXiuwei);

        //}
    }
     /// <summary>
    /// 日期过程
    /// </summary>
    void DayProcess(float deltaTime)
    {
        if (Game.Instance.gameSpeedAdd)
        {
            realdayProcessSpeed = dayProcessSpeed / 4f;
        }
        else
        {
            realdayProcessSpeed = dayProcessSpeed;
        }
        float theProcessAdd = (100 / realdayProcessSpeed) * deltaTime;
        _CurTimeData.DayProcess += (theProcessAdd);

        //一天过去了 结算
        if (_CurTimeData.DayProcess >= 100)
        {
            EndDay();
            
        }
        else
        {
          
        }



    }

    /// <summary>
    /// 一天过去
    /// </summary>
    public void DayPlus()
    {
        _CurTimeData.DayProcess = 0;
        lastDayPhaseIndex = 0;

        _CurTimeData.Day++;
        _CurTimeData.TheWeekDay++;

       

     
        //buff剩余天数减少
        RoleManager.Instance.BuffPassDay();
 
        //炼丹
         LianDanManager.Instance.OnProcessingDanFarm();
  
        //仓库里的弟子走进度
        StudentManager.Instance.OnDayPassed();
        //秘境探险
        MapManager.Instance.OnExploreTeamMoving();
        if (_CurTimeData.Day >= 31)
        {
            MoonPlus();
        }
        if (_CurTimeData.TheWeekDay >= 8)
        {
            WeekPlus();
        }




        //自动修理装备
        EquipmentManager.Instance.AutoFixEquipment();

  
        DayStartEvent();

    }

    /// <summary>
    /// 一天开始时的事件
    /// </summary>
    public void DayStartEvent()
    {     
        //到了进世界的那天
        if (_CurTimeData.Month % 12 == 0
            &&_CurTimeData.Day==1
            &&RoleManager.Instance._CurGameInfo.AllMapData.MapList[0].MapStatus==(int)AccomplishStatus.Accomplished)
        {

            PanelManager.Instance.OpenOnlyOkHint(LanguageUtil.GetLanguageText((int)LanguageIdType.界域动荡一年一度的界域裂隙已经打开), () =>
            {
                MapManager.Instance.XianMenOpen();
   
            });
        }

  

    }

    /// <summary>
    /// 一周过去
    /// </summary>
    public void WeekPlus()
    {
        _CurTimeData.TheWeekDay = 1;
        _CurTimeData.Week++;
        if (_CurTimeData.Week >= 4)
        {
            _CurTimeData.Week = 4;
        }
        //技能红点每周刷新
        SkillManager.Instance.RefreshAllRedPointShow();

        ////第四周可能出现找茬的人
        //if (_CurTimeData.Week == 4)
        //{
        //    ZhaoChaPeopleManager.Instance.GenerateZhaoChaPeople();
        //}
        //弟子去仓库找吃的
        StudentManager.Instance.OnChooseStudentToCangKu();
        //宗门生产
         StudentManager.Instance.NakedStudentAddExp();
 
        //npc任务
        if (Game.Instance.openNewGuide)
        TaskManager.Instance.CheckIfNPCAppear();
        EventCenter.Broadcast(TheEventType.WeekPlus);
        //_CurTimeData.wee
    }
    /// <summary>
    /// 一个月过去
    /// </summary>
    public void MoonPlus()
    {
        _CurTimeData.Day = 1;
        _CurTimeData.Month++;
        _CurTimeData.Week = 1;
        
        //防止连续跳2星期
        if (_CurTimeData.TheWeekDay >= 8)
        {
            _CurTimeData.TheWeekDay = 1;
        }
 
        MapEventManager.Instance.OnMonthPassed();

        EventCenter.Broadcast(TheEventType.MoonPlus);

        //弟子社交
        //if(_CurTimeData.Month % 2 == 0)
        StudentManager.Instance.OnStudentSocialization();

        //弟子自言自语
         //交租
        if (_CurTimeData.Month % 12 == 0)
        {
            RentManager.Instance.RentConsume();
        }
        //npc自己打一次
        if (_CurTimeData.Month % 3 == 0)
        {
            MatchManager.Instance.AIAutoPiPei();
        }
        if (_CurTimeData.Month >= 13)
        {
            YearPlus();
        }
#if !UNITY_EDITOR
        ArchiveManager.Instance.SaveArchive();
#endif
    }
    /// <summary>
    /// 一年过去
    /// </summary>
    public void YearPlus()
    {   
        //加2点源力
        //RoleManager.Instance.AddProperty(PropertyIdType.Tili, 2);
        _CurTimeData.Year++;
        _CurTimeData.Month = 1;
        if (_CurTimeData.Year > RoleManager.Instance._CurGameInfo.studentData.lastNewStudentYear)
        {
            StudentManager.Instance.YearStartGenerateStudents();

        }
        //清理掉十年前的弟子社交记录
        StudentManager.Instance.ClearSocializationData10YearBefore();
        EventCenter.Broadcast(TheEventType.YearPlus);
        //TalkingDataGA.OnEvent("游戏年份", new Dictionary<string, object>() { { "", _CurTimeData.Year } });

    }

    public void AddTimeBlockCount()
    {
        timeBlockCount++;
    }

    public void DeTimeBlockCount()
    {
        timeBlockCount--;
        if (timeBlockCount < 0)
            timeBlockCount = 0;
    }

    /// <summary>
    /// 下个月是否为节点
    /// </summary>
    /// <returns></returns>
    public bool CheckIfNextMonthIsJoint()
    {
        int curMonth = _CurTimeData.Month;
        int nextMonth = curMonth + 1;
        if (nextMonth > 12)
            nextMonth = 1;
        if (nextMonth % 3 == 0)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// 输入时间 输出该时间距今的周数
    /// </summary>
    /// <returns></returns>
    public int GetWeekNumFromTheTimeToNow(int year,int moon,int week)
    {
        int nowYear = _CurTimeData.Year;
        int nowMonth = _CurTimeData.Month;
        int nowWeek = _CurTimeData.Week;

        int res = 0;
        if (nowYear > year)
        {
            int yearNum = nowYear - year;
            if (yearNum > 1)
            {
                res += 36;
            }
          
                //月份数肯定不一样
            int monthToEnd= 12 - moon;
            int weekToEnd= monthToEnd * 4 + 4 - week;

            int thisYearWeek = 4 * (nowMonth - 1) + nowWeek;
            res += weekToEnd + thisYearWeek;
        }
        else if (nowMonth > moon)
        {
            int totalMonthWeek = (nowMonth - moon - 1) * 4;
            res = totalMonthWeek + 4 - week + nowWeek;
        }
        else
        {
            res = nowWeek - week;
        }
        return res;
    }

    /// <summary>
    /// 输入时间 输出该时间距今的天数
    /// </summary>
    /// <returns></returns>
    public int GetDayNumFromTheTimeToNow(int year, int moon, int day)
    {
        int nowYear = _CurTimeData.Year;
        int nowMonth = _CurTimeData.Month;
        int nowDay = _CurTimeData.Day;

        int res = 0;
        if (nowYear > year)
        {
            int yearNum = nowYear - year;
            if (yearNum > 1)
            {
                res += 360;
            }

            //月份数肯定不一样
            int monthToEnd = 12 - moon;
            int dayToEnd = monthToEnd * 30 + 30 - day;

            int thisYearDay = 30 * (nowMonth - 1) + nowDay;
            res += dayToEnd + thisYearDay;
        }
        //年份一样 月份不一样
        else if (nowMonth > moon)
        {
            int totalMonthDay = (nowMonth - moon - 1) * 30;
            res = totalMonthDay + 30 - day + nowDay;
        }
        else
        {
            res = nowDay - day;
        }
        return res;
    }

    /// <summary>
    /// 通过日得到周
    /// </summary>
    /// <returns></returns>
    public int GetWeekByDay(int day)
    {
        return day / 7;
    }

    /// <summary>
    /// 输入日得到年月周
    /// </summary>
    /// <returns></returns>
    public string GetYearMonthWeekByDay(int day)
    {
        string res = "";
        int year = day / 360;
        if (year > 0)
        {
            res += year + "年";
            day -= year * 360;
        }
        int month = day / 30;
        if (month > 0)
        {
            res += month + "月";
            day -= month * 30;
        }
        int week = day / 7;
        res += week + "周";
        return res;

    }


    public void GetServiceTime(Action<long> callBack)
    {
        StartCoroutine(CGameTime.Instance.GetServiceTimeSyn((x) =>
        {
            callBack(x);
        }));
    }

    /// <summary>
    /// 现实流逝一分钟
    /// </summary>
    public void RealityTimeProcess(System.Action<long> callBack=null)
    {
        bool forceUpgradeYunCunDang = false; ;//强制上传云存档
        GameTimeManager.Instance.GetServiceTime((x) =>
        {
            if (x <= 0)
            {
                connectedToFuWuQiTime = false;
                return;
            }
            connectedToFuWuQiTime = true;
            curFuWuQiTime = x;
            RoleManager.Instance._CurGameInfo.timeData.LastRecordCloudArchiveTime = x;
            //如果没有体力恢复的数据 那么尝试连接
            if (RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime <= 0)
            {
                RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime = x;
            }
            //如果没有上次看广告恢复体力时间的数据，那么尝试连接
            if (RoleManager.Instance._CurGameInfo.timeData.LastADReviveTiliTime <= 0)
            {
                RoleManager.Instance._CurGameInfo.timeData.LastADReviveTiliTime = x;
            }
            //如果没有上次获得离线收益的数据，那么尝试连接
            if (RoleManager.Instance._CurGameInfo.timeData.LastReceiveOfflineIncomeTime <= 0)
            {
                RoleManager.Instance._CurGameInfo.timeData.LastReceiveOfflineIncomeTime = x;
            }
            //上次记录的时间
            if (RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime <= 0)
            {
                RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime = RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime;
            }

            // 到了新的一天
            if (x - (RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime + CGameTime.Instance.GetTo24TimeStampByTimeStamp(RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime)) > 0)
            {
                forceUpgradeYunCunDang = true;
                RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime = x;
                RoleManager.Instance._CurGameInfo.timeData.LastADReviveTiliTime = x;
                RoleManager.Instance._CurGameInfo.timeData.TodayADTiliNum = 0;

                //任务也刷新
                RoleManager.Instance._CurGameInfo.AllDailyTaskData.lastBrushTime = x;
                RoleManager.Instance._CurGameInfo.AllDailyTaskData.curActiveNum = 0;
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList.Count; i++)
                {
                    RoleManager.Instance._CurGameInfo.AllDailyTaskData.activeAwardGetStatusList[i] = (int)AccomplishStatus.Processing;
                }
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllDailyTaskData.dailyTaskList.Count; i++)
                {
                    SingleDailyTaskData singleTask = RoleManager.Instance._CurGameInfo.AllDailyTaskData.dailyTaskList[i];
                    singleTask.accomplishStatus = (int)AccomplishStatus.Processing;
                    singleTask.curNum = 0;
                }


                //签到也刷新 7日签到未结束
                if (RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex < 7)
                {
                    RoleManager.Instance._CurGameInfo.QianDaoData.CanSevenDayQianDaoIndex = RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex + 1;
                }
                //7日签到结束了
                else
                {
                    RoleManager.Instance._CurGameInfo.QianDaoData.CanThirtyDayQianDaoIndex = RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex + 1;
                    if (RoleManager.Instance._CurGameInfo.QianDaoData.CanThirtyDayQianDaoIndex >= 31)
                    {
                        RoleManager.Instance._CurGameInfo.QianDaoData.CanThirtyDayQianDaoIndex = 1;
                        RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex = 0;
                    }
                }
                //jjc刷新
                MatchManager.Instance.RefreshRoleMatchTime(x);

                if (!PlayerPrefs.HasKey("todayUploadArchiveNum"))
                {
                    PlayerPrefs.SetInt("todayUploadArchiveNum", 0);
                }
                todayUploadArchiveNum = PlayerPrefs.GetInt("todayUploadArchiveNum");
                PlayerPrefs.SetInt("todayUploadArchiveNum", todayUploadArchiveNum);
                if (!PlayerPrefs.HasKey("todayDownloadArchiveNum"))
                {
                    PlayerPrefs.SetInt("todayDownloadArchiveNum", 0);
                }
                todayDownloadArchiveNum = PlayerPrefs.GetInt("todayDownloadArchiveNum");
                PlayerPrefs.SetInt("todayDownloadArchiveNum", todayDownloadArchiveNum);

                //每日服丹上限
                for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
                {
                    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
                    p.todayEatXiuWeiDanNum = 0;
                }
                RoleManager.Instance._CurGameInfo.playerPeople.todayEatXiuWeiDanNum = 0;
                //刷新每日参加选秀次数
                RoleManager.Instance._CurGameInfo.timeData.TodayParticipateXuanXiuNum = 0;
                //刷新每日抽奖次数
                RoleManager.Instance._CurGameInfo.timeData.TodayChouNum = 0;
            }

            ////竞技场是否到了刷新的一天
            //if (x - (RoleManager.Instance._CurGameInfo.MatchData.LastParticipateMatchTime + CGameTime.Instance.GetTo24TimeStampByTimeStamp(RoleManager.Instance._CurGameInfo.MatchData.LastParticipateMatchTime)) > 0)
            //{
            //    MatchManager.Instance.RefreshRoleMatchTime(x);
            //}
            //商店是否到了刷新的一天
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.allShopData.ShopList.Count; i++)
            {
                SingleShopData singleShopData = RoleManager.Instance._CurGameInfo.allShopData.ShopList[i];


                if (x - (singleShopData.LastBrushTime + CGameTime.Instance.GetTo24TimeStampByTimeStamp(singleShopData.LastBrushTime)) > 0)
                {
                    singleShopData.LastBrushTime = x;
                    singleShopData.TodayBrushNum = 0;
                    if (singleShopData.ShopType != (int)ShopType.MoonCard
                    && singleShopData.ShopType != (int)ShopType.XinShouLiBao)
                    {
                        ShopManager.Instance.BrushShop((ShopType)singleShopData.ShopType);

                    }
                    else
                    {
                        //月卡
                        if (singleShopData.ShopType == (int)ShopType.MoonCard)
                        {
                            for (int j = 0; j < singleShopData.ShopItemList.Count; j++)
                            {  //月卡领钻
                                ShopItemData data = singleShopData.ShopItemList[j];
                                if (data.Id == (int)ShopIDType.MoonCard1)
                                {
                                    //月卡未过期
                                    if (data.moonCardReachTime >= x)
                                    {
                                        ItemManager.Instance.GetItemWithAwardPanel(new List<int> { (int)ItemIdType.LingJing }, new List<ulong> { ConstantVal.moonCard1PerDaySend });

                                    }
                                }

                            }
                        }

                    }

                }
            }
            //弟子招募是否到了8小时以后
            if (x - (RoleManager.Instance._CurGameInfo.studentData.lastRecruitStudentTime + ConstantVal.hourBeforeNextStudent * 60 * 60) > 0)
            {
                StudentManager.Instance.RefreshTodayRecruitStudentTime(x);
            }
            //历练是否到了新的一天
            if (RoleManager.Instance._CurGameInfo.timeData.LastParticipatedLiLianTime.Count == 0)
            {
                LiLianManager.Instance.RefreshTodayLiLianTime(x);
            }
            else
            {
                if (x - (RoleManager.Instance._CurGameInfo.timeData.LastParticipatedLiLianTime[0] + CGameTime.Instance.GetTo24TimeStampByTimeStamp(RoleManager.Instance._CurGameInfo.timeData.LastParticipatedLiLianTime[0])) > 0)
                {
                    LiLianManager.Instance.RefreshTodayLiLianTime(x);
                }
            }

            RoleManager.Instance.TimeAddOfflineIncome(RoleManager.Instance._CurGameInfo.timeData.LastReceiveOfflineIncomeTime, x);
            //秘境时间流逝
            MiJingManager.Instance.TimeProcess(x);

            ////删掉一周以内的邮件
            //for (int i = RoleManager.Instance._CurGameInfo.AllMailData.MailList.Count-1; i >=0; i--)
            //{
            //    SingleMailData mail = RoleManager.Instance._CurGameInfo.AllMailData.MailList[i];
            //    if (mail.GetAward)
            //    {
            //        if (x - mail.ReceiveTime >= ConstantVal.maxMailStayDay*24*60*60)
            //        {
            //            MailManager.Instance.RemoveMail(mail);
            //        }
            //    }
            //}
            callBack?.Invoke(x);

            EventCenter.Broadcast(TheEventType.RealityOneMinProcess);
            EventCenter.Broadcast(TheEventType.RefreshRedPointShow);

            // if (forceUpgradeYunCunDang)
            // {
            //     UpLoadFiles.OnUploadArchive(ConstantVal.GetArchiveSavePath(Game.Instance.curServerIndex), RoleManager.Instance._CurGameInfo.TheGuid);
            // }
          
            #if !UNITY_EDITOR
            ArchiveManager.Instance.SaveArchive(Game.Instance.curServerIndex);
            
            if(x-RoleManager.Instance._CurGameInfo.timeData.LastUploadArchiveTime>1800)
            {
                string uploadFile = ArchiveManager.Instance.CreateUploadSnapshot();
                UpLoadFiles.OnUploadArchive(uploadFile, ArchiveManager.Instance.ArchiveUploadName());
                //UpLoadFiles.OnUploadArchive(ConstantVal.GetArchiveSavePath(ArchiveManager.Instance.archiveIndex), RoleManager.Instance._CurGameInfo.roleId.ToString());
                RoleManager.Instance._CurGameInfo.timeData.LastUploadArchiveTime = x;

            }
 
#endif
        });
    }

 

    /// <summary>
    /// 现实流逝一秒钟
    /// </summary>
    public void RealitySecondProcess()
    {
   
        GameTimeManager.Instance.GetServiceTime((x) =>
        {
            if (x <= 0)
                return;
            curFuWuQiTime = x;
            //RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime = x;
            if (RoleManager.Instance._CurGameInfo.timeData.LastRecordYuanShenShouSumTime <= 0)
            {
                RoleManager.Instance._CurGameInfo.timeData.LastRecordYuanShenShouSumTime = x;
            }
            StudentManager.Instance.TimeHuiFuYuanShen(RoleManager.Instance._CurGameInfo.timeData.LastRecordYuanShenShouSumTime, x);
            RoleManager.Instance.TimeAddTiLi(RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime, x);
            EventCenter.Broadcast(TheEventType.RealitySecondPassed);
        });
    }
    public string sec_to_hms(long duration)
    {
        TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(duration));
        string str = "";
        if (ts.Days > 0)
        {

        }
        if (ts.Hours > 0)
        {
            str = String.Format("{0:00}", ts.Hours + ts.Days * 24) + ":" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
        }
        if (ts.Hours == 0 && ts.Minutes > 0)
        {
            str = "00:" + String.Format("{0:00}", ts.Minutes) + ":" + String.Format("{0:00}", ts.Seconds);
        }
        if (ts.Hours == 0 && ts.Minutes == 0)
        {
            str = "00:00:" + String.Format("{0:00}", ts.Seconds);
        }
        return str;
    }

}
