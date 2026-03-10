using Framework.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using cfg;

public class RoleManager
{
    public TDGAProfile profile;

    public GameInfo _CurGameInfo;

    public bool initOk = false;
    private static RoleManager inst = null;

    public static RoleManager Instance
    {
        get
        {
            if (inst == null)
            {
                inst = new RoleManager();
            }
            return inst;
        }

    }

    public void CheckArchive()
    {
        // 先加载本地存档
        ArchiveManager.Instance.LoadAllArchive();
        
        // 在banhaomode下，如果没有本地存档，则尝试从云端获取
        if (Game.Instance.banHaoMode && ArchiveManager.Instance.recentArchive == null)
        {
            Debug.Log("banhaomode模式：本地无存档，将尝试从云端获取");
        }
        else if (Game.Instance.banHaoMode && ArchiveManager.Instance.recentArchive != null)
        {
            Debug.Log("banhaomode模式：找到本地存档，优先使用本地存档");
        }
        
        //无存档
        if (ArchiveManager.Instance.recentArchive != null)
        {
            _CurGameInfo = ArchiveManager.Instance.allArchiveList[ArchiveManager.Instance.recentArchiveIndex];

        }
        else
        {
            _CurGameInfo = new GameInfo();

        }
        ////检查有无存档
        //if (File.Exists(ConstantVal.GetArchiveSavePath(0)))
        //{
        //    newPlayer = false;
        //    _CurGameInfo = ArchiveManager.Instance.Load();
        //    CheckNewChapter();
        //}
        //else
        //{
        //    newPlayer = true;
        //    _CurGameInfo = new GameInfo();


        //}
    }
    
    /// <summary>
    /// 删除所有本地存档（用于banhaomode）
    /// </summary>
    private void DeleteAllLocalArchives()
    {
        try
        {
            int maxArchiveNum = 1;
            for (int i = 0; i < maxArchiveNum; i++)
            {
                string archivePath = ConstantVal.GetArchiveSavePath(i);
                if (System.IO.File.Exists(archivePath))
                {
                    System.IO.File.Delete(archivePath);
                    Debug.Log($"banhaomode: 删除本地存档 {i}: {archivePath}");
                }
                
                // 同时删除备份存档
                string backupArchivePath = ConstantVal.GetArchiveBeiFenSavePath(i);
                if (System.IO.File.Exists(backupArchivePath))
                {
                    System.IO.File.Delete(backupArchivePath);
                    Debug.Log($"banhaomode: 删除备份存档 {i}: {backupArchivePath}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"banhaomode: 删除本地存档时出错: {e.Message}");
        }
    }

    /// <summary>
    /// 公共方法：删除所有本地存档（用于退出登录时）
    /// </summary>
    public void ClearAllLocalArchives()
    {
        DeleteAllLocalArchives();
        // 重新加载存档列表
        ArchiveManager.Instance.LoadAllArchive();
        Debug.Log("已清除所有本地存档，方便切换账号");
    }

    public void Init(int index)
    {
        bool newPlayer = false;
        if (ArchiveManager.Instance.allArchiveList[index] == null)
            newPlayer = true;

        if (newPlayer)
        {
            string theGuid = "";
            int theRoleId =0;
            //服务器给了id和roleid
            if (_CurGameInfo != null)
            {
                if(!string.IsNullOrWhiteSpace(_CurGameInfo.TheGuid))
                    theGuid = _CurGameInfo.TheGuid;
               
                theRoleId =  _CurGameInfo.roleId;
            }
            _CurGameInfo = new GameInfo();
            _CurGameInfo.TheGuid = theGuid;
            _CurGameInfo.roleId = theRoleId;
            _CurGameInfo.quIndex = index;
            _CurGameInfo.CreateTime = CGameTime.Instance.GetTimeStamp();//创号时间
        } 
        else
        {
            _CurGameInfo = ArchiveManager.Instance.allArchiveList[index];
            // 配置表已加载，为所有 ItemData/EquipProtoData 重新赋值 setting
            ArchiveManager.Instance.RestoreAllSettings(_CurGameInfo);
        }
        TDGALogin();
        InitAllUIComponentUnlockStatus(newPlayer);
        InitAllPeople(newPlayer);
        InitPlayerPeople(newPlayer);
        InitRoleSkill(newPlayer);
        InitAllNPC(newPlayer);
        InitItem(newPlayer);
        InitAllAchievement(newPlayer);
        InitAllEquipmentData(newPlayer);
        InitAllMatch(newPlayer);
        InitAllBuff(newPlayer);
         InitStudent(newPlayer);
        InitNotedPeople(newPlayer);
        InitLianDanData(newPlayer);
        InitAllMap(newPlayer);
        InitMiJing(newPlayer);
        InitDanFarm(newPlayer);
        InitAllMapEvent(newPlayer);
        InitAllExploreData(newPlayer);
        InitAllZongmenData(newPlayer);
        InitAllOtherZongMenData(newPlayer);
        InitTime(newPlayer);
        InitQianDao(newPlayer);
        InitShop();
        ShopManager.Instance.Init();
        DuiHuanMaManager.Instance.Init();

        InitMail(newPlayer);
        TeamManager.Instance.Init();
        InitAllTeamData(newPlayer);
        InitAllADData(newPlayer);
         ChouJiangManager.Instance.Init();
        RangeManager.Instance.Init();
        InitScene(newPlayer);
        LianDanManager.Instance.Init();
        MailManager.Instance.Init();
 
        ShareManager.Instance.Init();
        InitAllDailyTaskData(newPlayer);
        InitAllGuideBookData(newPlayer);
        BuildingManager.Instance.Init();
        RedPointManager.Instance.Init();
        InitNewGuide(newPlayer);
        TaskManager.Instance.Init();
        //赠送一个装备制作师傅 一个炼丹师傅 和2张图纸 
        if (newPlayer)
        {
            SendItem();
            SendEquipPicture();
             // SendLianDanStudent();
        }
        InitVersionName();
        initOk = true;
        //新手引导
        NewGuideManager.Instance.Init();
        //FixBug();
    }

    ///// <summary>
    ///// 修复bug
    ///// </summary>
    //void FixBug()
    //{
    //    //修复装备丢失的bug
    //    if (_CurGameInfo.playerPeople.CurEquipItem != null)
    //    {
    //        ulong onlyId = RoleManager.Instance._CurGameInfo.playerPeople.curEquipItem.onlyId;

    //        if (!RoleManager.Instance._CurGameInfo.ItemModel.onlyIdList.Contains(onlyId))
    //        {
    //            ItemManager.Instance.AddANewItem(RoleManager.Instance._CurGameInfo.playerPeople.curEquipItem);

    //        }


    //    }
    //}

    public void TDGALogin()
    {
        //登陆talking
        profile = TDGAProfile.SetProfile(_CurGameInfo.TheGuid);
        profile.SetProfileType(ProfileType.REGISTERED);
    }

    /// <summary>
    /// 初始化版本名
    /// </summary>
    void InitVersionName()
    {
        _CurGameInfo.VersionName = Application.version;
    }

    /// <summary>
    /// 初始化冒险手札数据
    /// </summary>
    public void InitAllGuideBookData(bool newPlayer)
    {
        if (_CurGameInfo.AllGuideBookData == null)
        {
            _CurGameInfo.AllGuideBookData = new AllGuideBookData();
        }
        List<int> cfgChapterList = new List<int>();
        List<int> cfgIdList = new List<int>();
        //当前配置的列表
        for (int i = 0; i < DataTable._guideBookList.Count; i++)
        {
            cfgIdList.Add(DataTable._guideBookList[i].Id.ToInt32());
            if (!cfgChapterList.Contains(DataTable._guideBookList[i].Chapter.ToInt32()))
            {
                cfgChapterList.Add(DataTable._guideBookList[i].Chapter.ToInt32());
            }
        }
        List<int> savedIdList = new List<int>();
        List<int> savedChapterList = new List<int>();

        for (int i = _CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList.Count - 1; i >= 0; i--)
        {
            SingleGuideBookTaskData savedData = _CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList[i];
            GuideBookSetting setting = null;
            //不存在 移除
            if (!cfgIdList.Contains(savedData.settingId))
            {
                _CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList.RemoveAt(i);
            }
            //存在
            else
            {
                setting = DataTable.FindGuideBookSetting(savedData.settingId);
                savedIdList.Add(savedData.settingId);
            }
            if (setting != null)
            {
                if (!savedChapterList.Contains(setting.Chapter.ToInt32()))
                {
                    savedChapterList.Add(setting.Chapter.ToInt32());
                }
            }
        

        }

        //把新加的加进去
        for (int i = 0; i < cfgIdList.Count; i++)
        {
            int cfgId = cfgIdList[i];
            //该研究
            if (!savedIdList.Contains(cfgId))
            {
                GuideBookSetting setting = DataTable.FindGuideBookSetting(cfgId);
                SingleGuideBookTaskData newData = new SingleGuideBookTaskData();
                newData.settingId = cfgId;
                //没有前置任务 他就是第一个
                if (string.IsNullOrWhiteSpace(setting.ForeTask))
                {
                    //新章节 除了第一章 都要锁住
                    if (!savedChapterList.Contains(setting.Chapter.ToInt32()))
                        newData.accomplishStatus = (int)AccomplishStatus.Locked;
                    else
                        newData.accomplishStatus = (int)AccomplishStatus.Processing;

                }
                else
                {
                    newData.accomplishStatus = (int)AccomplishStatus.Locked;

                }
                //如果有前置任务，则为未完成
                _CurGameInfo.AllGuideBookData.singleGuideBookTaskDataList.Add(newData);
            }
        }
        //新加章节 要加新的
        for (int i = 0; i < cfgChapterList.Count; i++)
        {
            int chapter = cfgChapterList[i];
            if (!savedChapterList.Contains(chapter))
            {
                SingleChapterGuideBookData singleChapterGuideBookData = new SingleChapterGuideBookData();
                singleChapterGuideBookData.chapter = chapter;
                for (int j = 0; j < 10; j++)
                {
                    singleChapterGuideBookData.processAccomplishStatus.Add((int)AccomplishStatus.UnAccomplished);

                }
                _CurGameInfo.AllGuideBookData.singleChapterList.Add(singleChapterGuideBookData);
            }
        }
        TaskManager.Instance.RevealChapterGuideBookTask(1);
        TaskManager.Instance.RefreshAllGuideBookStatus();
        TaskManager.Instance.InitRedPoint();
        TaskManager.Instance.TryAccomplishAllGuideBook();
    }

    /// <summary>
    /// 初始化日常任务数据
    /// </summary>
    public void InitAllDailyTaskData(bool newPlayer)
    {
        if (_CurGameInfo.AllDailyTaskData == null)
        {
            _CurGameInfo.AllDailyTaskData = new AllDailyTaskData();
            for(int i = 0; i < 5; i++)
            {
                _CurGameInfo.AllDailyTaskData.activeAwardGetStatusList.Add((int)AccomplishStatus.Processing);
            }
        }
         List<int> cfgIdList = new List<int>();
        //当前配置的列表
        for(int i = 0; i < DataTable._taskList.Count; i++)
        {
            cfgIdList.Add(DataTable._taskList[i].Id.ToInt32());

        }

        List<int> savedIdList = new List<int>();
 
        for (int i = _CurGameInfo.AllDailyTaskData.dailyTaskList.Count - 1; i >= 0; i--)
        {
            SingleDailyTaskData savedData = _CurGameInfo.AllDailyTaskData.dailyTaskList[i];
            TaskSetting setting = null;
            //不存在 移除
            if (!cfgIdList.Contains(savedData.settingId))
            {
                _CurGameInfo.AllDailyTaskData.dailyTaskList.RemoveAt(i);
            }
            //存在
            else
            {
                setting = DataTable.FindTaskSetting(savedData.settingId);
                savedIdList.Add(savedData.settingId);
            }
        }

        //把新加的加进去
        for (int i = 0; i < cfgIdList.Count; i++)
        {
            int cfgId = cfgIdList[i];
            //该研究
            if (!savedIdList.Contains(cfgId))
            {
                TaskSetting setting = DataTable.FindTaskSetting(cfgId);
                SingleDailyTaskData newData = new SingleDailyTaskData();
                newData.settingId = cfgId;
                newData.accomplishStatus = (int)AccomplishStatus.Processing;

                //如果有前置任务，则为未完成
                _CurGameInfo.AllDailyTaskData.dailyTaskList.Add(newData);
            }
        }
 

    }


    /// <summary>
    /// 初始化宗门
    /// </summary>
    public void InitAllZongmenData(bool newPlayer)
    {
        if (_CurGameInfo.allZongMenData == null)
        {
            
            _CurGameInfo.allZongMenData = new ZongMenData();
            _CurGameInfo.allZongMenData.ZongMenLevel = 1;
            _CurGameInfo.allZongMenData.TiliLimit = 5;
            
            // 检查配置表是否为空
            if (DataTable._zongMenUpgradeList == null || DataTable._zongMenUpgradeList.Count == 0)
            {
                Debug.LogError("InitAllZongmenData: DataTable._zongMenUpgradeList 为空或未加载");
                return;
            }
            
            ZongMenUpgradeSetting setting = DataTable._zongMenUpgradeList[0];
            RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmNumLimit = setting.FarmNumLimit.ToInt32()+ RoleManager.Instance._CurGameInfo.allZongMenData.SendFarmNumLimitAddNum;
            RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum = RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedDanFarmNumLimit;
            RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockFarmNeedLingShiNum = (RoleManager.Instance._CurGameInfo.allDanFarmData.UnlockedFarmNum - 3) * 500;
        }
        if (_CurGameInfo.allZongMenData.CurRankLevel == 0)
        {
            _CurGameInfo.allZongMenData.CurRankLevel = 1;
            _CurGameInfo.allZongMenData.TheR = 800;
            _CurGameInfo.allZongMenData.CurStar = 1;
        }

    }

    /// <summary>
    /// 广告数据
    /// </summary>
    /// <param name="newPlayer"></param>
    public void InitAllADData(bool newPlayer)
    {
        if (_CurGameInfo.AllADData == null)
        {
            _CurGameInfo.AllADData = new AllADData();
            _CurGameInfo.AllADData.getAwardTotalADWatchNumIndex = -1;
        }
    }

    /// <summary>
    /// 所有队伍数据
    /// </summary>
    /// <param name="newPlayer"></param>
    public void InitAllTeamData(bool newPlayer)
    {
        if (_CurGameInfo.AllTeamData == null)
        {
            _CurGameInfo.AllTeamData = new TeamData();
            _CurGameInfo.AllTeamData.TeamList1.Add(_CurGameInfo.playerPeople.onlyId);

        }

        if (_CurGameInfo.AllTeamData.TeamList1.Count < 4)
        {
            int posNumAdd = 4 - _CurGameInfo.AllTeamData.TeamList1.Count;
            for (int i = 0; i < posNumAdd; i++)
            {
                _CurGameInfo.AllTeamData.TeamList1.Add(0);
            }
        }
        if (_CurGameInfo.AllTeamData.TeamList2.Count < 4)
        {
            int posNumAdd = 4 - _CurGameInfo.AllTeamData.TeamList2.Count;

            for (int i = 0; i < posNumAdd; i++)
            {
                _CurGameInfo.AllTeamData.TeamList2.Add(0);
            }
        }
    }
    /// <summary>
    /// 初始化ui解锁状态
    /// </summary>
    public void InitAllUIComponentUnlockStatus(bool newPlayer)
    {
        if (_CurGameInfo.AllUIComponentUnlockStatus == null)
        {
            _CurGameInfo.AllUIComponentUnlockStatus = new UIComponentUnlockStatus();
        }
        //当前配置的列表
        List<int> cfgIdList = new List<int>();

        foreach (int theType in Enum.GetValues(typeof(UIComponentType)))
        {
            cfgIdList.Add(theType);
        }

        List<int> savedIdList = new List<int>();

        for (int i = _CurGameInfo.AllUIComponentUnlockStatus.UIComponentTypeList.Count - 1; i >= 0; i--)
        {
            int savedData = _CurGameInfo.AllUIComponentUnlockStatus.UIComponentTypeList[i];
            //不存在 移除
            if (!cfgIdList.Contains(savedData))
            {
                _CurGameInfo.AllUIComponentUnlockStatus.UIComponentTypeList.RemoveAt(i);
            }
            //该事件存在
            else
            {
                savedIdList.Add(savedData);
            }
        }

        //把新加的加进去
        for (int i = 0; i < cfgIdList.Count; i++)
        {
            int cfgId = cfgIdList[i];
            //该研究
            if (!savedIdList.Contains(cfgId))
            {
                savedIdList.Add(cfgId);

                _CurGameInfo.AllUIComponentUnlockStatus.UIComponentTypeList.Add(cfgId);
                _CurGameInfo.AllUIComponentUnlockStatus.UIComponentStatusList.Add((int)UnlockType.UnShow);

            }
        }

    }

    /// <summary>
    /// 初始化所有地图事件
    /// </summary>
    public void InitAllMapEvent(bool newPlayer)
    {
        if (_CurGameInfo.allMapEventData == null)
        {
            _CurGameInfo.allMapEventData = new AllMapEventData();

        }

        List<int> cfgIdList = new List<int>();
        //当前配置的列表
        for (int i = 0; i < DataTable._mapEventList.Count; i++)
        {
            cfgIdList.Add(DataTable._mapEventList[i].Id.ToInt32());
        }
        List<int> savedIdList = new List<int>();

        for (int i = _CurGameInfo.allMapEventData.SingleMapEventList.Count - 1; i >= 0; i--)
        {
            SingleMapEventData savedData = _CurGameInfo.allMapEventData.SingleMapEventList[i];
            //该事件不存在 移除
            if (!cfgIdList.Contains(savedData.SettingId))
            {
                _CurGameInfo.allMapEventData.SingleMapEventList.RemoveAt(i);
            }
            //该事件存在
            else
            {
                savedIdList.Add(savedData.SettingId);
            }
        }

 
        MapEventManager.Instance.Init();


    }

  
    /// <summary>
    /// 初始化弟子秘境探险数据
    /// </summary>
    public void InitAllExploreData(bool newPlayer)
    {
        if (_CurGameInfo.AllExploreData == null)
        {
            _CurGameInfo.AllExploreData = new AllExploreData();
        }

        List<int> cfgIdList = new List<int>();
        //当前配置的列表
        for (int i = 0; i < DataTable._exploreMapList.Count; i++)
        {
            cfgIdList.Add(DataTable._exploreMapList[i].Id.ToInt32());
        }
        List<int> savedIdList = new List<int>();

        for (int i = _CurGameInfo.AllExploreData.ExploreList.Count - 1; i >= 0; i--)
        {
            SingleExploreData savedData = _CurGameInfo.AllExploreData.ExploreList[i];
            //不存在 移除
            if (!cfgIdList.Contains(savedData.SettingId))
            {
                _CurGameInfo.AllExploreData.ExploreList.RemoveAt(i);
            }
            //存在
            else
            {
                savedIdList.Add(savedData.SettingId);
            }
        }

        //把新加的加进去
        for (int i = 0; i < cfgIdList.Count; i++)
        {
            int cfgId = cfgIdList[i];
            //该研究
            if (!savedIdList.Contains(cfgId))
            {
                ExploreMapSetting setting = DataTable.FindExploreMapSetting(cfgId);
                SingleExploreData newData = new SingleExploreData();

                newData.SettingId = cfgId;
                _CurGameInfo.AllExploreData.ExploreList.Add(newData);
            }
        }
        ////处理老存档的问题，如果没有该学生 则为0
        //for (int i = 0; i < _CurGameInfo.AllExploreData.ExploreList.Count; i++)
        //{
        //    SingleExploreData data = _CurGameInfo.AllExploreData.ExploreList[i];
        //    if (data.ExploreTeamData != null)
        //    {
        //        for (int j = data.ExploreTeamData.StudentOnlyIdList.Count - 1; j >= 0; j--)
        //        {
        //            ulong onlyId = data.ExploreTeamData.StudentOnlyIdList[j];
        //            if (onlyId > 0)
        //            {
        //                if (StudentManager.Instance.FindStudent(onlyId) == null)
        //                    data.ExploreTeamData.StudentOnlyIdList.RemoveAt(j);
        //            }
        //        }
        //    }
        //}
    }

    /// <summary>
    /// 初始化丹田
    /// </summary>
    public void InitDanFarm(bool newPlayer)
    {
        if (_CurGameInfo.allDanFarmData == null)
        {
            _CurGameInfo.allDanFarmData = new AllDanFarmData();
            _CurGameInfo.allDanFarmData.DanFarmZuoZhenStudentLimit = 1;
            _CurGameInfo.allDanFarmData.UnlockedDanFarmId.Add(10001);
            //for (int i = 0; i < ConstantVal.allFarmNum; i++)
            //{
            //    LianDanManager.Instance.AddADanFarm();

            //}
            //解锁数量
            for (int i = 0; i < 4; i++)
            {
                ZongMenManager.Instance.UnlockDanFarmPos();

            }
        }

        ////把新加的加进去
        //if (_CurGameInfo.allDanFarmData.DanFarmList.Count < ConstantVal.allFarmNum)
        //{
        //    int addedCount = ConstantVal.allFarmNum - _CurGameInfo.allDanFarmData.DanFarmList.Count;
        //    for (int i = 0; i < addedCount; i++)
        //    {
        //        LianDanManager.Instance.AddADanFarm();
        //    }
        //}
        //解决老存档bug 有可能丹炉升级了没解锁弟子位置
        for (int i = 0; i < _CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
            SingleDanFarmData data = _CurGameInfo.allDanFarmData.DanFarmList[i];
            if (data.SettingId != 0)
            {
                if (data.SettingId == 70001) continue;
                DanFarmSetting setting = DataTable.FindDanFarmSetting(data.SettingId);
                LianDanManager.Instance.UnlockStudentPos(data, setting);
            }


        }
        ////解锁数量
        //for(int i = 0; i < 8; i++)
        //{
        //    LianDanManager.Instance.UnlockDanFarmPos(i);

        //}
    }
 
    /// <summary>
    /// 初始化离线收益
    /// </summary>
    public void InitOffLineIncome()
    {
        GameTimeManager.Instance.RealityTimeProcess((x) =>
        {
            bool haveItem = false;
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.timeData.OfflineItemList.Count; i++)
            {
                if (RoleManager.Instance._CurGameInfo.timeData.OfflineItemList[i].count > 0)
                {
                    haveItem = true;
                    break;
                }
            }
            if (haveItem && PanelManager.Instance.offlineIncomePanel == null)
            {
                PanelManager.Instance.OpenOfflinePanel();
                //时间为您减少了XX

            }

            //签到也刷新 7日签到未结束
            if (RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex == 0)
            {
                RoleManager.Instance._CurGameInfo.QianDaoData.CanSevenDayQianDaoIndex = RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex + 1;
            }
            //7日签到结束了
            else
            {
                if (RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex == 0)
                {
                    RoleManager.Instance._CurGameInfo.QianDaoData.CanThirtyDayQianDaoIndex = RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex + 1;
                }
            }
            if (RoleManager.Instance._CurGameInfo.QianDaoData.SevenDayQianDaoIndex >= 7)
            {
                if (GameTimeManager.Instance.connectedToFuWuQiTime
                && RoleManager.Instance._CurGameInfo.QianDaoData.ThirtyDayQianDaoIndex+1== RoleManager.Instance._CurGameInfo.QianDaoData.CanThirtyDayQianDaoIndex)
                    PanelManager.Instance.OpenThirtyDayQianDaoPanel();
              
            }

            //初始化历练最大次数
            if (x > 0)
            {
                LiLianManager.Instance.InitMaxLiLianTime(x);
                MiJingManager.Instance.RefreshTaoFaLimit(x);
            }
        });


    }

    /// <summary>
    /// 初始化所有成就
    /// </summary>
    /// <param name="newPlayer"></param>
    void InitAllAchievement(bool newPlayer)
    {
        if (_CurGameInfo.AllAchievementData == null)
        {
            _CurGameInfo.AllAchievementData = new AllAchievementData();
        }
    }

    /// <summary>
    /// 初始化所有npc
    /// </summary>
    /// <param name="newPlayer"></param>
    void InitAllNPC(bool newPlayer)
    {
        if (_CurGameInfo.allNPCData == null)
        {
            _CurGameInfo.allNPCData = new AllNPCData();
        }

        List<ulong> cfgIdList = new List<ulong>();
        //当前配置的npc列表
        for (int i = 0; i < DataTable.NPCArr.Length; i++)
        {
            NPC theNPC = DataTable.NPCArr[i];

            cfgIdList.Add((ulong)theNPC.id);
        }
        List<ulong> savedNPCIdList = new List<ulong>();

        for (int i = _CurGameInfo.allNPCData.AllNPCList.Count - 1; i >= 0; i--)
        {
            SingleNPCData savedData = _CurGameInfo.allNPCData.AllNPCList[i];
            //该npc不存在 移除
            if (!cfgIdList.Contains((ulong)savedData.Id))
            {
                ulong theOnlyId = savedData.OnlyId;
                if (_CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Contains(theOnlyId))
                {
                    _CurGameInfo.allNPCData.CurShowNPCOnlyIdList.Remove(theOnlyId);
                    //int index = _CurGameInfo.allNPCData.CurShowNPCOnlyIdList.IndexOf(theOnlyId);

                }
                _CurGameInfo.allNPCData.AllNPCList.RemoveAt(i);
            }
            //该npc存在 把不在的任务删了
            else
            {
                savedNPCIdList.Add((ulong)savedData.Id);
                NPC setting = DataTable.FindNPCArrById(savedData.Id);
                //当前配置的任务列表
                List<ulong> cfgTaskIdList = new List<ulong>();
                if (setting.tasks == null)
                {
                    savedData.AllTaskList.Clear();
                }
                else
                {
                    for (int j = 0; j < setting.tasks.Count; j++)
                    {
                        SingleTask taskSetting = setting.tasks[j];
                        cfgTaskIdList.Add(taskSetting.theId);
                    }
                    List<ulong> savedTaskIdList = new List<ulong>();
                    //把不在配置中的任务删了
                    for (int j = savedData.AllTaskList.Count - 1; j >= 0; j--)
                    {
                        if (!cfgTaskIdList.Contains(savedData.AllTaskList[j].TheId))
                        {
                            savedData.AllTaskList.RemoveAt(j);
                        }
                        else
                        {
                            savedTaskIdList.Add(savedData.AllTaskList[j].TheId);
                        }
                    }
                    //把该npc新加的任务加进去
                    for (int j = 0; j < cfgTaskIdList.Count; j++)
                    {
                        ulong theId = cfgTaskIdList[j];
                        if (!savedTaskIdList.Contains(theId))
                        {
                            SingleTask taskSetting = TaskManager.Instance.FindTaskSettingById(setting, theId);
                            SingleTaskProtoData taskData = new SingleTaskProtoData();
                            taskData.TaskIndex = j;
                            taskData.LastAccomplishTime = 0;
                            taskData.TheId = taskSetting.theId;
                            taskData.AccomplishStatus = (int)AccomplishStatus.Locked;
                            taskData.NpcId = setting.id;
                            savedData.AllTaskList.Add(taskData);
                        }
                    }
                }



            }
        }



        //把新加的npc加进去
        for (int i = 0; i < cfgIdList.Count; i++)
        {
            ulong cfgId = cfgIdList[i];
            NPC theNPC = DataTable.NPCArr[i];
            if (theNPC.initAdd)
            {
                if (!_CurGameInfo.allNPCData.RemovedNPCSettingIdList.Contains((int)cfgId))
                {
                    //该npc
                    if (!savedNPCIdList.Contains(cfgId))
                    {
                        savedNPCIdList.Add(cfgId);
                        TaskManager.Instance.AddNPC(theNPC);
                    }
                }
            }


        }

        //然后把所有npc的task按照先后排个序
        for (int i = 0; i < _CurGameInfo.allNPCData.AllNPCList.Count; i++)
        {
            SingleNPCData singleNPCData = _CurGameInfo.allNPCData.AllNPCList[i];
            NPC npcSetting = DataTable.FindNPCArrById(singleNPCData.Id);
            for (int j = 0; j < singleNPCData.AllTaskList.Count - 1; j++)
            {
                for (int k = 0; k < singleNPCData.AllTaskList.Count - 1 - j; k++)
                {
                    //前面的大于后面的，则二者交换
                    SingleTaskProtoData fore = singleNPCData.AllTaskList[k];
                    SingleTaskProtoData after = singleNPCData.AllTaskList[k + 1];

                    SingleTask settingFore = TaskManager.Instance.FindTaskSettingById(npcSetting, fore.TheId);
                    SingleTask settingAfter = TaskManager.Instance.FindTaskSettingById(npcSetting, after.TheId);

                    if (npcSetting.tasks.IndexOf(settingFore) > npcSetting.tasks.IndexOf(settingAfter))
                    {
                        SingleTaskProtoData temp = singleNPCData.AllTaskList[k];
                        singleNPCData.AllTaskList[k] = singleNPCData.AllTaskList[k + 1];
                        singleNPCData.AllTaskList[k + 1] = temp;
                    }


                }
            }
        }

    }
    /// <summary>
    /// 初始化npc属性
    /// </summary>
    public void InitNPCPro(SingleNPCData singleNPCData)
    {
        PeopleData p = singleNPCData.PeopleData;
        NPC npcSetting = DataTable.FindNPCArrById(singleNPCData.Id);

        string proStr = "";
        YuanSuType yuanSuType = YuanSuType.None;

        if (npcSetting.enemyId != 0)
        {
            EnemySetting enemySetting = DataTable.FindEnemySetting((int)npcSetting.enemyId);
            proStr = enemySetting.Property;
            if (!string.IsNullOrWhiteSpace(enemySetting.YuanSu))
            {
                yuanSuType = (YuanSuType)enemySetting.YuanSu.ToInt32();
            }

        }
        else
        {
            proStr = ConstantVal.baseLianGongStudentPro;

        }
        if (yuanSuType == YuanSuType.None)
        {
            yuanSuType = (YuanSuType)RandomManager.Next(1, (int)YuanSuType.Dark);
        }
        List<List<int>> baseBattleProList = CommonUtil.SplitCfg(ConstantVal.baseBattleProperty);

        List<List<int>> proList = CommonUtil.SplitCfg(proStr);
        List<int> haveValIdList = new List<int>();
        List<int> haveValValList = new List<int>();
        for (int i = 0; i < proList.Count; i++)
        {
            List<int> thePro = proList[i];
            haveValIdList.Add(thePro[0]);
            haveValValList.Add(thePro[1]);
        }
        for (int i = 0; i < baseBattleProList.Count; i++)
        {
            List<int> singlePro = baseBattleProList[i];
            int id = singlePro[0];
            int num = singlePro[1];

            if (haveValIdList.Contains(id))
            {
                int index = haveValIdList.IndexOf(id);
                num = haveValValList[index];
            }

            SinglePropertyData singlePropertyData = new SinglePropertyData();
            singlePropertyData.id = id;
            singlePropertyData.num = num;
            singlePropertyData.quality = 1;

            if (id == (int)PropertyIdType.MpNum)
            {
                singlePropertyData.limit = 100;
            }
            else if (id == (int)PropertyIdType.Hp)
            {
                singlePropertyData.limit = singlePropertyData.num;
            }


            p.propertyList.Add(singlePropertyData);
            p.propertyIdList.Add(singlePropertyData.id);

            SinglePropertyData battle_singlePropertyData = new SinglePropertyData();
            battle_singlePropertyData.id = id;
            battle_singlePropertyData.num = singlePropertyData.num;
            battle_singlePropertyData.limit = singlePropertyData.limit;
            battle_singlePropertyData.quality = 1;

            p.curBattleProIdList.Add(id);
            p.curBattleProList.Add(battle_singlePropertyData);
            //PeopleData.ProQualityList.Add(proQuality);
        }
        
        //初始技能暂定灵弹
        p.allSkillData = new AllSkillData();
        SingleSkillData singleSkill = new SingleSkillData();
        singleSkill.skillId =(int) BattleManager.Instance.PuGongIdByYuanSu(yuanSuType);
        singleSkill.skillLevel = 1;
        p.allSkillData.skillList.Add(singleSkill);
        p.allSkillData.equippedSkillIdList.Add(singleSkill.skillId);

        List<SkillSetting> candidateList = DataTable.FindCanStudySkillListByYuanSu((YuanSuType)p.yuanSu);
        if (candidateList.Count > 0)
        {
            int skillId2 = candidateList[RandomManager.Next(0, candidateList.Count)].Id.ToInt32();
            SingleSkillData data2 = new SingleSkillData();
            data2.skillId = skillId2;
            data2.skillLevel = 1;
            p.allSkillData.skillList.Add(data2);
            p.allSkillData.equippedSkillIdList.Add(skillId2);
        }
  
    }

    /// <summary>
    /// 初始化场景数据
    /// </summary>
    /// <param name="newPlayer"></param>
    void InitScene(bool newPlayer)
    {
        if (_CurGameInfo.SceneData == null)
        {
            _CurGameInfo.SceneData = new SceneData();
            _CurGameInfo.SceneData.CurSceneType = (int)SceneType.Mountain;
        }
    }

  
    /// <summary>
    /// 初始化秘境
    /// </summary>
    void InitMiJing(bool newPlayer)
    {
        if (_CurGameInfo.AllMiJingPaiQianData == null)
        {
            _CurGameInfo.AllMiJingPaiQianData = new AllMiJingPaiQianData();
            //for(int i = 0; i < DataTable._miJingList.Count; i++)
            //{
            //    SingleMiJingPaiQianData singleMiJingPaiQianData = new SingleMiJingPaiQianData();
            //    singleMiJingPaiQianData.SettingId = DataTable._miJingList[i].id.ToInt32();

            //    MiJingSetting miJingSetting = DataTable.FindMiJingSetting(singleMiJingPaiQianData.SettingId);
            //    List<int> levelList = CommonUtil.SplitCfgOneDepth(miJingSetting.param);
            //    for(int j = 0; j < levelList.Count; j++)
            //    {
            //        int levelId = levelList[j];
            //        MiJingLevelSetting levelSetting = DataTable.FindMiJingLevelSetting(levelId);
            //        SingleMiJingLevelData levelData = new SingleMiJingLevelData();
            //        levelData.MiJingId = singleMiJingPaiQianData.SettingId;
            //        levelData.LevelId = levelId;
            //        if (j == 0)
            //            levelData.AccomplishStatus = (int)MiJingLevelAccomplishType.UnAccomplished;
            //        else
            //            levelData.AccomplishStatus = (int)MiJingLevelAccomplishType.Locked;
            //        singleMiJingPaiQianData.LevelList.Add(levelData);
            //    }

            //    _CurGameInfo.AllMiJingPaiQianData.PaiqianList.Add(singleMiJingPaiQianData);

            //}
        }
        List<int> cfgIdList = new List<int>();
        //当前配置的列表
        for (int i = 0; i < DataTable._miJingList.Count; i++)
        {
            cfgIdList.Add(DataTable._miJingList[i].Id.ToInt32());
        }
        List<int> savedIdList = new List<int>();

        for (int i = _CurGameInfo.AllMiJingPaiQianData.PaiqianList.Count - 1; i >= 0; i--)
        {
            SingleMiJingPaiQianData savedData = _CurGameInfo.AllMiJingPaiQianData.PaiqianList[i];
            //该事件不存在 移除
            if (!cfgIdList.Contains(savedData.SettingId))
            {
                _CurGameInfo.AllMiJingPaiQianData.PaiqianList.RemoveAt(i);
            }
            //该事件存在
            else
            {
                savedIdList.Add(savedData.SettingId);
                //观察savedData里面的mijinglevelsetting是否和配置一样
                MiJingSetting miJingSetting = DataTable.FindMiJingSetting(savedData.SettingId);
                // 使用 SplitCfgStringOneDepth 保留原始字符串格式
                List<string> cfgLevelIdStrList = CommonUtil.SplitCfgStringOneDepth(miJingSetting.Param);
                List<int> cfgLevelIdList = new List<int>();
                foreach (var s in cfgLevelIdStrList)
                {
                    if (!string.IsNullOrEmpty(s) && s != "0")
                        cfgLevelIdList.Add(s.ToInt32());
                }
                List<int> savedLevelIdList = new List<int>();
                for (int j = savedData.LevelList.Count - 1; j >= 0; j--)
                {
                    SingleMiJingLevelData level = savedData.LevelList[j];
                    //移除配置中没有的
                    if (!cfgLevelIdList.Contains(level.LevelId))
                        savedData.LevelList.RemoveAt(j);
                    else
                    {
                        //配置中有
                        savedLevelIdList.Add(level.LevelId);
                    }
                }
                for (int j = 0; j < cfgLevelIdList.Count; j++)
                {
                    int cfgLevelId = cfgLevelIdList[j];
                    if (!savedLevelIdList.Contains(cfgLevelId))
                    {
                        SingleMiJingLevelData levelData = new SingleMiJingLevelData();
                        levelData.MiJingId = savedData.SettingId;
                        levelData.LevelId = cfgLevelId;
                        if (j == 0)
                            levelData.AccomplishStatus = (int)MiJingLevelAccomplishType.UnAccomplished;
                        else
                            levelData.AccomplishStatus = (int)MiJingLevelAccomplishType.Locked;
                        savedData.LevelList.Add(levelData);
                    }
                }

            }
        }

        //把新加的加进去
        for (int i = 0; i < cfgIdList.Count; i++)
        {
            int cfgId = cfgIdList[i];
            //该研究
            if (!savedIdList.Contains(cfgId))
            {
                savedIdList.Add(cfgId);
                //初始化
                SingleMiJingPaiQianData singleMiJingPaiQianData = new SingleMiJingPaiQianData();
                singleMiJingPaiQianData.SettingId = DataTable._miJingList[i].Id.ToInt32();

                MiJingSetting miJingSetting = DataTable.FindMiJingSetting(singleMiJingPaiQianData.SettingId);
                // 使用 SplitCfgStringOneDepth 保留原始字符串格式
                List<string> levelStrList = CommonUtil.SplitCfgStringOneDepth(miJingSetting.Param);
                for (int j = 0; j < levelStrList.Count; j++)
                {
                    string levelIdStr = levelStrList[j];
                    if (string.IsNullOrEmpty(levelIdStr) || levelIdStr == "0")
                        continue;
                    int levelId = levelIdStr.ToInt32();
                    MiJingLevelSetting levelSetting = DataTable.FindMiJingLevelSetting(levelIdStr);
                    SingleMiJingLevelData levelData = new SingleMiJingLevelData();
                    levelData.MiJingId = singleMiJingPaiQianData.SettingId;
                    levelData.LevelId = levelId;
                    if (j == 0)
                        levelData.AccomplishStatus = (int)MiJingLevelAccomplishType.UnAccomplished;
                    else
                        levelData.AccomplishStatus = (int)MiJingLevelAccomplishType.Locked;
                    singleMiJingPaiQianData.LevelList.Add(levelData);
                }

                _CurGameInfo.AllMiJingPaiQianData.PaiqianList.Add(singleMiJingPaiQianData);


            }
        }
   
        for (int i = 0; i < _CurGameInfo.AllMiJingPaiQianData.PaiqianList.Count; i++)
        {
            SingleMiJingPaiQianData singleMiJingPaiQianData = _CurGameInfo.AllMiJingPaiQianData.PaiqianList[i];

            //守卫数量

            long lastKillMonsterTime = singleMiJingPaiQianData.LastKillMonsterTime;
            long refreshTime = lastKillMonsterTime + CGameTime.Instance.GetTo24TimeStampByTimeStamp(lastKillMonsterTime);

            GameTimeManager.Instance.GetServiceTime((x) =>
            {
                if (x >= refreshTime)
                {
                    MiJingManager.Instance.RefreshSingleTaoFaLimit(singleMiJingPaiQianData, x);

                    singleMiJingPaiQianData.LastKillMonsterTime = x;
                    singleMiJingPaiQianData.DayliHighNum = singleMiJingPaiQianData.MaxDayliHighNum;
                    System.DayOfWeek dayOfWeek = CGameTime.Instance.GetDayOfWeekByTimeStamp(x);
                    CurWeekType weekType = MiJingManager.Instance.GetWeekTypeByWeekDay(dayOfWeek);
                    singleMiJingPaiQianData.WeekType = (int)weekType;
                    EventCenter.Broadcast(TheEventType.RefreshMiJingGuardShow);
                }
            });
        }

        
    }

    /// <summary>
    /// 初始化所有地图
    /// </summary>
    void InitAllMap(bool newPlayer)
    {
        if (_CurGameInfo.AllMapData == null)
        {
            AllMapData allMapData = new AllMapData();
            for (int i = 0; i < DataTable._mapList.Count; i++)
            {
                MapSetting setting = DataTable._mapList[i];

                SingleMapData singleMapData = new SingleMapData();
                singleMapData.MapId = DataTable._mapList[i].Id.ToInt32();
                if (setting.IsFirstMap == "1")
                {
                    singleMapData.MapStatus = (int)AccomplishStatus.UnAccomplished;
                }
                else
                {
                    singleMapData.MapStatus = (int)AccomplishStatus.Locked;
                }
                singleMapData.LieXiMapStatus = (int)AccomplishStatus.Locked;
                List<string> levelIdList = CommonUtil.SplitCfgStringOneDepth(setting.LevelIdList);
                List<string> fixedLevelIdCfgList = CommonUtil.SplitCfgStringOneDepth(setting.FixedLevelIdList);
                for (int j = 0; j < levelIdList.Count; j++)
                {
                    string levelIdStr = levelIdList[j];
                    if (string.IsNullOrEmpty(levelIdStr) || levelIdStr == "0")
                        continue;
                    LevelSetting levelSetting = DataTable.FindLevelSetting(levelIdStr);
                    if (levelSetting == null)
                    {
                        Debug.LogError($"[InitAllMap] LevelSetting 不存在, levelId={levelIdStr}, mapId={setting.Id}, LevelIdList={setting.LevelIdList}");
                        continue;
                    }
                    SingleLevelData singleLevelData = new SingleLevelData();
                    singleLevelData.LevelId = levelIdStr;

                    if (levelSetting.IsFirst == "1")
                    {
                        singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
                    }
                    else
                    {
                        singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                    }
                    singleMapData.LevelList.Add(singleLevelData);
                }
                for (int j = 0; j < fixedLevelIdCfgList.Count; j++)
                {
                    string levelIdStr = fixedLevelIdCfgList[j];
                    if (string.IsNullOrEmpty(levelIdStr) || levelIdStr == "0")
                        continue;
                    LevelSetting levelSetting = DataTable.FindLevelSetting(levelIdStr);
                    if (levelSetting == null)
                    {
                        Debug.LogError($"[InitAllMap] FixedLevel LevelSetting 不存在, levelId={levelIdStr}, mapId={setting.Id}, FixedLevelIdList={setting.FixedLevelIdList}");
                        continue;
                    }
                    SingleLevelData singleLevelData = new SingleLevelData();
                    singleLevelData.LevelId = levelIdStr;

                    if (levelSetting.IsFirst == "1")
                    {
                        singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
                    }
                    else
                    {
                        singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                    }
                    singleMapData.FixedLevelList.Add(singleLevelData);
                }

                allMapData.MapList.Add(singleMapData);
            }
            _CurGameInfo.AllMapData = allMapData;
        }
        //把配置表里面不存在的关卡删了
        for (int i = _CurGameInfo.AllMapData.MapList.Count - 1; i >= 0; i--)
        {
            SingleMapData map = _CurGameInfo.AllMapData.MapList[i];
            if (!DataTable.CheckIfHaveIdMap(map.MapId))
            {
                _CurGameInfo.AllMapData.MapList.RemoveAt(i);
                continue;
            }
            //for (int j = map.LevelList.Count - 1; j >= 0; j--)
            //{
            //    SingleLevelData level = map.LevelList[j];
            //    if (!DataTable.CheckIfHaveIdLevel(level.LevelId))
            //    {
            //        map.LevelList.RemoveAt(j);
            //        continue;
            //    }
            //}
            //for (int j = map.FixedLevelList.Count - 1; j >= 0; j--)
            //{
            //    SingleLevelData level = map.FixedLevelList[j];
            //    if (!DataTable.CheckIfHaveIdLevel(level.LevelId))
            //    {
            //        map.FixedLevelList.RemoveAt(j);
            //        continue;
            //    }
            //}
        }
        //把配置表里面的新的地图加进去
        for (int i = DataTable._mapList.Count - 1; i >= 0; i--)
        {
            MapSetting mapSetting = DataTable._mapList[i];
            // 使用 SplitCfgStringOneDepth 保留原始字符串格式（如 "00001"），避免前导零丢失
            List<string> levelIdStrList = CommonUtil.SplitCfgStringOneDepth(mapSetting.LevelIdList);
            List<string> fixedLevelIdStrList = CommonUtil.SplitCfgStringOneDepth(mapSetting.FixedLevelIdList);
            SingleMapData singleMapData;

            //如果是新图
            if (!MapManager.Instance.CheckIfHaveIdMap(mapSetting.Id.ToInt32()))
            {
                singleMapData = new SingleMapData();
                singleMapData.MapId = DataTable._mapList[i].Id.ToInt32();

                if (MapManager.Instance.CheckIfUnlockMap(singleMapData.MapId))
                {
                    singleMapData.MapStatus = (int)AccomplishStatus.UnAccomplished;
                }
                else
                {
                    singleMapData.MapStatus = (int)AccomplishStatus.Locked;
                }
                singleMapData.LieXiMapStatus = (int)AccomplishStatus.Locked;




                for (int j = 0; j < levelIdStrList.Count; j++)
                {
                    string levelIdStr = levelIdStrList[j];
                    if (string.IsNullOrEmpty(levelIdStr) || levelIdStr == "0")
                        continue;
                    SingleLevelData singleLevelData = new SingleLevelData();
                    singleLevelData.LevelId = levelIdStr;

                    singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                    singleMapData.LevelList.Add(singleLevelData);
                }
                for (int j = 0; j < fixedLevelIdStrList.Count; j++)
                {
                    string levelIdStr = fixedLevelIdStrList[j];
                    if (string.IsNullOrEmpty(levelIdStr) || levelIdStr == "0")
                        continue;
                    SingleLevelData singleLevelData = new SingleLevelData();
                    singleLevelData.LevelId = levelIdStr;
                    LevelSetting levelSetting = DataTable.FindLevelSetting(levelIdStr);
                    singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                    singleMapData.FixedLevelList.Add(singleLevelData);
                }
                _CurGameInfo.AllMapData.MapList.Add(singleMapData);
                continue;
            }
            else
            {
                //该地图是老图有没有新关卡

                //先找到该图
                singleMapData = MapManager.Instance.FindMapById(mapSetting.Id.ToInt32());
                // 转换为 string 列表用于 Contains 比较（存档中的 LevelId 现在是 string）
                List<string> fixedLevelIdCfgList = new List<string>();
                foreach (var s in fixedLevelIdStrList)
                {
                    if (!string.IsNullOrEmpty(s) && s != "0")
                        fixedLevelIdCfgList.Add(s);
                }
                List<string> levelIdList = new List<string>();
                foreach (var s in levelIdStrList)
                {
                    if (!string.IsNullOrEmpty(s) && s != "0")
                        levelIdList.Add(s);
                }
                //把配置表中不存在的关卡删了
                for (int j = singleMapData.FixedLevelList.Count - 1; j >= 0; j--)
                {
                    SingleLevelData fixedlevel = singleMapData.FixedLevelList[j];
                    if (!fixedLevelIdCfgList.Contains(fixedlevel.LevelId))
                    {
                        singleMapData.FixedLevelList.RemoveAt(j);
                        continue;
                    }
                }
                //把配置表中不存在的关卡删了
                for (int j = singleMapData.FixedLevelList.Count - 1; j >= 0; j--)
                {
                    SingleLevelData fixedlevel = singleMapData.FixedLevelList[j];
                    if (!fixedLevelIdCfgList.Contains(fixedlevel.LevelId))
                    {
                        singleMapData.FixedLevelList.RemoveAt(j);
                        continue;
                    }
                }
                for (int j = singleMapData.LevelList.Count - 1; j >= 0; j--)
                {
                    SingleLevelData level = singleMapData.LevelList[j];
                    if (!levelIdList.Contains(level.LevelId))
                    {
                        singleMapData.LevelList.RemoveAt(j);
                        continue;
                    }
                }
                for (int j = 0; j < levelIdStrList.Count; j++)
                {
                    string levelIdStr = levelIdStrList[j];
                    //如果有新关卡
                    if (!MapManager.Instance.CheckIfHaveIdLevel(mapSetting.Id.ToInt32(), levelIdStr))
                    {
                        if (string.IsNullOrEmpty(levelIdStr) || levelIdStr == "0")
                            continue;
                        SingleLevelData singleLevelData = new SingleLevelData();
                        singleLevelData.LevelId = levelIdStr;
                        singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                        singleMapData.LevelList.Add(singleLevelData);
                    }

                }
                for (int j = 0; j < fixedLevelIdStrList.Count; j++)
                {
                    string levelIdStr = fixedLevelIdStrList[j];
                    if (string.IsNullOrEmpty(levelIdStr) || levelIdStr == "0")
                        continue;
                    //如果有新关卡
                    if (!MapManager.Instance.CheckIfHaveFixedIdLevel(mapSetting.Id.ToInt32(), levelIdStr))
                    {
                        SingleLevelData singleLevelData = new SingleLevelData();
                        singleLevelData.LevelId = levelIdStr;
                        singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                        singleMapData.FixedLevelList.Add(singleLevelData);
                    }

                }
                //注册完所有新关卡以后刷新一下关卡状态
                for (int j = 0; j < singleMapData.LevelList.Count; j++)
                {
                    SingleLevelData singleLevelData = singleMapData.LevelList[j];
                    if (singleLevelData.LevelStatus == (int)AccomplishStatus.Locked)
                    {
                        if (MapManager.Instance.CheckIfUnlockLevel(singleMapData.MapId, singleLevelData.LevelId))
                        {
                            singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
                        }
                    }
                }
                //注册完所有新关卡以后刷新一下固定关卡状态
                for (int j = 0; j < singleMapData.FixedLevelList.Count; j++)
                {
                    SingleLevelData singleLevelData = singleMapData.FixedLevelList[j];
                    if (singleLevelData.LevelStatus == (int)AccomplishStatus.Locked)
                    {
                        if (MapManager.Instance.CheckIfUnlockFixedLevel(singleMapData.MapId, singleLevelData.LevelId))
                        {
                            singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
                        }
                    }
                }
            }


        }

        //然后把所有map按照先后排个序

        for (int j = 0; j < _CurGameInfo.AllMapData.MapList.Count - 1; j++)
        {
            for (int k = 0; k < _CurGameInfo.AllMapData.MapList.Count - 1 - j; k++)
            {
                //前面的大于后面的，则二者交换
                SingleMapData fore = _CurGameInfo.AllMapData.MapList[k];
                SingleMapData after = _CurGameInfo.AllMapData.MapList[k + 1];

                MapSetting settingFore = DataTable.FindMapSetting(fore.MapId);
                MapSetting settingAfter = DataTable.FindMapSetting(after.MapId);

                if (DataTable._mapList.IndexOf(settingFore) > DataTable._mapList.IndexOf(settingAfter))
                {
                    SingleMapData temp = _CurGameInfo.AllMapData.MapList[k];
                    _CurGameInfo.AllMapData.MapList[k] = _CurGameInfo.AllMapData.MapList[k + 1];
                    _CurGameInfo.AllMapData.MapList[k + 1] = temp;
                }


            }
        }


    }

    void InitAllPeople(bool newPlayer)
    {
        _CurGameInfo.AllPeopleList.Clear();

    }

    void InitNewGuide(bool newPlayer)
    {
        if (_CurGameInfo.NewGuideData == null)
        {
            _CurGameInfo.NewGuideData = new NewGuideData();
        }
        //先把配置表里面不存在的新手引导删了
        DeleteUnconfigureNewGuide(_CurGameInfo.NewGuideData);
        //如果有新增的也加进去
        AddNewConfigureGuide(_CurGameInfo.NewGuideData);
    }

    /// <summary>
    /// 删掉无配置的新手引导
    /// </summary>
    void DeleteUnconfigureNewGuide(NewGuideData newGuide)
    {
        int count = newGuide.IdList.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            int savedId = newGuide.IdList[i];
            if (!DataTable.newGuideDic.ContainsKey(savedId.ToString()))
            {
                newGuide.IdList.RemoveAt(i);
                newGuide.AccomplishStatus.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// 增加新增配置的引导
    /// </summary>
    void AddNewConfigureGuide(NewGuideData newGuide)
    {
        for (int i = 0; i < DataTable._newGuideList.Count; i++)
        {
            NewGuideSetting theGuide = DataTable._newGuideList[i];
            int theId = Convert.ToInt32(theGuide.Id);
            if (!newGuide.IdList.Contains(theId))
            {
                newGuide.IdList.Add(theId);
                int newType = (int)TaskStatusType.UnAccomplished;
                newGuide.AccomplishStatus.Add(newType);
            }
        }
    }

   

    /// <summary>
    /// 初始化炼丹
    /// </summary>
    /// <param name="newPlayer"></param>
    void InitLianDanData(bool newPlayer)
    {
        if (_CurGameInfo.LianDanData == null)
        {
            _CurGameInfo.LianDanData = new LianDanData();


            //LianDanBuildingUpgradeSetting setting = DataTable._lianDanBuildingUpgradeList[0];
            //List<int> validMoneyDanSettingIdList = CommonUtil.SplitCfgOneDepth(setting.moneyDan);
            //for(int i = 0; i < validMoneyDanSettingIdList.Count; i++)
            //{
            //    _CurGameInfo.LianDanData.CurValidMoneyDanSettingIdList.Add(validMoneyDanSettingIdList[i]);
            //}
            SingleBuildingData data = BuildingManager.Instance.FindBuildingDataBySettingId((int)BuildingIdType.LianDanFang);
            if(data!=null)
                data.CurBuildLevel = 1;
        }
    }
 
    /// <summary>
    /// 初始化弟子
    /// </summary>
    void InitStudent(bool newPlayer)
    {
        if (_CurGameInfo.studentData == null)
        {
            _CurGameInfo.studentData = new StudentData();
            _CurGameInfo.studentData.MaxStudentNum = 3;//TODO最大弟子数和升级宗门有关
            _CurGameInfo.studentData.thisYearRemainCanRecruitStudentNum = 3;//可招3人
        }
        for (int i = 0; i < _CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = _CurGameInfo.studentData.allStudentList[i];
            if (p.socializationData == null)
            {
                p.socializationData = new SocializationData();
                p.socializationData.socialActivity = RandomManager.Next(0, 100);
                p.socializationData.xingGe = StudentManager.Instance.RdmXingGe();
            }



        }
    }

    void InitNotedPeople(bool newPlayer)
    {
        if (_CurGameInfo.NotedPeopleData == null)
        {
            _CurGameInfo.NotedPeopleData = new NotedPeopleData();
        }
    }

    /// <summary>
    /// 初始化物品
    /// </summary>
    void InitItem(bool newPlayer)
    {
        if (_CurGameInfo.ItemModel == null)
            _CurGameInfo.ItemModel = new ItemModel();

        for (int i = 0; i < _CurGameInfo.ItemModel.cangKuItemDataList.Count; i++)
        {
            ItemData cangKuItem = _CurGameInfo.ItemModel.cangKuItemDataList[i];
            if (cangKuItem.equipProtoData != null)
            {
                //EquipmentManager.Instance.EquipRefreshPro(cangKuItem);
            }
        }
    }

    /// <summary>
    /// 初始化buff
    /// </summary>
    /// <param name="newPlayer"></param>
    void InitAllBuff(bool newPlayer)
    {
        if (_CurGameInfo.AllBuffData == null)
            _CurGameInfo.AllBuffData = new AllBuffData();

    }

    /// <summary>
    /// 初始化所有比赛
    /// </summary>
    void InitAllMatch(bool newPlayer)
    {
        if (newPlayer)
        {
            _CurGameInfo.MatchData = new MatchData();
            for (int i = 0; i < DataTable._matchList.Count; i++)
            {
                MatchSetting setting = DataTable._matchList[i];
                SingleMatchData singleData = new SingleMatchData();
                singleData.SettingId = setting.Id.ToInt32();

            }
            //解锁第一个
        }

    }

    /// <summary>
    /// 初始化装备
    /// </summary>
    void InitAllEquipmentData(bool newPlayer)
    {
        if (newPlayer)
        {
            _CurGameInfo.AllEquipmentData = new AllEquipmentData();
        }
        for (int i = 0; i < _CurGameInfo.ItemModel.itemDataList.Count; i++)
        {
            ItemData data = _CurGameInfo.ItemModel.itemDataList[i];
            ItemSetting setting = DataTable.FindItemSetting(data.settingId);
            if (setting!=null && setting.ItemType.ToInt32() == (int)ItemType.Equip)
            {
                for(int j=0;j< _CurGameInfo.playerPeople.curEquipItemList.Count; j++)
                {
                    ItemData theData = _CurGameInfo.playerPeople.curEquipItemList[j];
                    if (theData != null
                      && theData.onlyId == data.onlyId)
                    {
                        theData = data;

                    }
                }
               

                if (_CurGameInfo.EquipMakeTeamData != null
                   && _CurGameInfo.EquipMakeTeamData.curFixedEquip != null
                   && _CurGameInfo.EquipMakeTeamData.curFixedEquip.onlyId == data.onlyId)
                {
                    _CurGameInfo.EquipMakeTeamData.curFixedEquip = data.equipProtoData;
                }
            }

        }

    }

    /// <summary>
    /// 商店
    /// </summary>
    /// <param name="newPlayer"></param>
    void InitShop()
    {

        List<int> settingShopTypeList = new List<int>();
        for (int i = 0; i < DataTable._shopList.Count; i++)
        {
            ShopSetting setting = DataTable._shopList[i];
            if (!settingShopTypeList.Contains(setting.Type.ToInt32()))
            {
                settingShopTypeList.Add(setting.Type.ToInt32());
            }
        }
        if (_CurGameInfo.allShopData == null)
        {
            _CurGameInfo.allShopData = new AllShopData();
         
            for(int i = 0; i < settingShopTypeList.Count; i++)
            {
                SingleShopData data = new SingleShopData();
                int type= settingShopTypeList[i];
                data.ShopType = type;
                _CurGameInfo.allShopData.ShopList.Add(data);
            }
        }
        List<int> existedShopTypeList = new List<int>();
        //删掉表格中没有的
        for(int i = _CurGameInfo.allShopData.ShopList.Count - 1; i >= 0; i--)
        {
            SingleShopData data= _CurGameInfo.allShopData.ShopList[i];
            if (!settingShopTypeList.Contains(data.ShopType))
            {
                _CurGameInfo.allShopData.ShopList.Remove(data);
            }
            else
            {
                //删掉表格中没有的
                for(int j = data.ShopItemList.Count-1; j >=0 ; j--)
                {
                    ShopItemData shopItemData = data.ShopItemList[j];
                    if (!DataTable.shopDic.ContainsKey(shopItemData.Id.ToString()))
                    {
                        data.ShopItemList.Remove(shopItemData);
                    }
                }

                existedShopTypeList.Add(data.ShopType);

            }
        }
        //新加的加进去
        for (int i = 0; i < settingShopTypeList.Count; i++)
        {
            int settingType = settingShopTypeList[i];
            if (!existedShopTypeList.Contains(settingType))
            {

                SingleShopData data = new SingleShopData();
                int type = settingShopTypeList[i];
                data.ShopType = type;
                _CurGameInfo.allShopData.ShopList.Add(data);
            }
        }

        //月卡中新的月卡加进去
        SingleShopData moonCardShop = ShopManager.Instance.FindSingleShopDataByType(ShopType.MoonCard);
        List<ShopSetting> shopSettingList = DataTable.FindShopSettingListByType((int)ShopType.MoonCard);

        List<int> existedMoonCardIdList = new List<int>();
        for(int i = 0; i < moonCardShop.ShopItemList.Count; i++)
        {
            existedMoonCardIdList.Add(moonCardShop.ShopItemList[i].Id);
        }
        for(int i=0;i< shopSettingList.Count; i++)
        {
            ShopSetting setting = shopSettingList[i];
            if (!existedMoonCardIdList.Contains(setting.Id.ToInt32()))
            {
                //有新的
                ShopItemData shopItemData = new ShopItemData();
                shopItemData.Id = setting.Id.ToInt32();
                shopItemData.RemainCount = setting.MaxCount.ToInt32();
                List<int> theItem = CommonUtil.SplitCfgOneDepth(setting.Item);
                shopItemData.ItemId = theItem[0];
                moonCardShop.ShopItemList.Add(shopItemData);

            }
        }
        //每日礼包中新的礼包加进去
        SingleShopData dailyLiBaoShop = ShopManager.Instance.FindSingleShopDataByType(ShopType.DailyLiBao);
        List<ShopSetting> shopSettingList2 = DataTable.FindShopSettingListByType((int)ShopType.DailyLiBao);

        List<int> existedDailyLiBaoIdList = new List<int>();
        for (int i = 0; i < dailyLiBaoShop.ShopItemList.Count; i++)
        {
            existedDailyLiBaoIdList.Add(dailyLiBaoShop.ShopItemList[i].Id);
        }
        for (int i = 0; i < shopSettingList2.Count; i++)
        {
            ShopSetting setting = shopSettingList2[i];
            if (!existedDailyLiBaoIdList.Contains(setting.Id.ToInt32()))
            {
                //有新的
                ShopItemData shopItemData = new ShopItemData();
                shopItemData.Id = setting.Id.ToInt32();
                shopItemData.RemainCount = setting.MaxCount.ToInt32();
                List<int> theItem = CommonUtil.SplitCfgOneDepth(setting.Item);
                shopItemData.ItemId = theItem[0];
                dailyLiBaoShop.ShopItemList.Add(shopItemData);

            }
        }

        //永久礼包中新的礼包加进去
        SingleShopData xinShouLiBaoShop = ShopManager.Instance.FindSingleShopDataByType(ShopType.XinShouLiBao);
        List<ShopSetting> shopSettingList3 = DataTable.FindShopSettingListByType((int)ShopType.XinShouLiBao);

        List<int> existedXinShouLiBaoIdList = new List<int>();
        for (int i = 0; i < xinShouLiBaoShop.ShopItemList.Count; i++)
        {
            existedXinShouLiBaoIdList.Add(xinShouLiBaoShop.ShopItemList[i].Id);
        }
        for (int i = 0; i < shopSettingList3.Count; i++)
        {
            ShopSetting setting = shopSettingList3[i];
            if (!existedXinShouLiBaoIdList.Contains(setting.Id.ToInt32()))
            {
                //有新的
                ShopItemData shopItemData = new ShopItemData();
                shopItemData.Id = setting.Id.ToInt32();
                shopItemData.RemainCount = setting.MaxCount.ToInt32();
                List<int> theItem = CommonUtil.SplitCfgOneDepth(setting.Item);
                shopItemData.ItemId = theItem[0];
                xinShouLiBaoShop.ShopItemList.Add(shopItemData);

            }
        }

    }


    /// <summary>
    /// 邮件
    /// </summary>
    /// <param name="newPlayer"></param>
    void InitMail(bool newPlayer)
    {
        if (_CurGameInfo.AllMailData == null)
        {
            _CurGameInfo.AllMailData = new AllMailData();
        }
    }

    /// <summary>
    /// 初始化时间
    /// </summary>
    void InitTime(bool newPlayer)
    {
        if (newPlayer)
        {
            TimeData timeData = new TimeData();
            timeData.Year = 1;
            timeData.Month = 1;
            timeData.Week = 1;
            timeData.Day = 1;
            _CurGameInfo.timeData = timeData;

            GameTimeManager.Instance.GetServiceTime((x) =>
            {
                if (x > 0)
                {
                    _CurGameInfo.timeData.LastReviveTiliTime = x;
                    _CurGameInfo.timeData.LastADReviveTiliTime = x;
                    //上次记录的时间
                    if (RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime <= 0)
                    {
                        RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime = RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime;
                    }
                }
            });

            if (_CurGameInfo.timeData.NextRdmEventRemainMonth == 0)
            {
                _CurGameInfo.timeData.NextRdmEventRemainMonth = RandomManager.Next(ConstantVal.rdmEventTimeRange[0], ConstantVal.rdmEventTimeRange[1]);
            }
        }
        //离线体力收益

        GameTimeManager.Instance.GetServiceTime((x) =>
        {
            if (x <= 0)
                return;
            TimeAddTiLi(RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime, x);

            //抵消建筑时间

        });
    }

    /// <summary>
    /// 初始化签到
    /// </summary>
    /// <param name="newPlayer"></param>
    void InitQianDao(bool newPlayer)
    {
        if (_CurGameInfo.QianDaoData == null)
        {
            _CurGameInfo.QianDaoData = new QianDaoData();
        }
    }


    /// <summary>
    /// 时间行走加体力
    /// </summary>
    /// <param name="before"></param>
    /// <param name="after"></param>
    public void TimeAddTiLi(long before, long after)
    {
        long offset = (after - before);
        long tiliNum = offset / (ConstantVal.tiliReviveMinute * 60);
        if (tiliNum > 0)
        {
            long addTiLi = tiliNum;
            int curTiLi = RoleManager.Instance.FindMyProperty((int)PropertyIdType.Tili).num;
            if (curTiLi + tiliNum >= RoleManager.Instance._CurGameInfo.allZongMenData.TiliLimit)
            {
                addTiLi = RoleManager.Instance._CurGameInfo.allZongMenData.TiliLimit - curTiLi;
            }
            //防止扣
            if (addTiLi <= 0)
            {
                addTiLi = 0;
            }
            RoleManager.Instance.AddProperty(PropertyIdType.Tili, (int)addTiLi);
            long theSec = offset % 60;
            //上次记录的时间
            if (RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime <= 0)
            {
                RoleManager.Instance._CurGameInfo.timeData.LastRecordedTime = RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime;
            }
            RoleManager.Instance._CurGameInfo.timeData.LastReviveTiliTime = after - theSec;
         
        }

    }

    /// <summary>
    /// 时间行走加离线收益
    /// </summary>
    /// <param name="before"></param>
    /// <param name="after"></param>
    public void TimeAddOfflineIncome(long before, long after)
    {
        long offset = (after - before);
        long maxOffset = OfflineManager.Instance.MaxOfflineTime();
        //最大x小时
        if (offset >= maxOffset)
            offset = maxOffset;
        long totalMinute = offset / 60;
        long totalFakeMonth = totalMinute * 2;
        List<int> idList = new List<int>();
        List<ulong> numList = new List<ulong>();
        for (int i = 0; i < _CurGameInfo.timeData.OfflineItemList.Count; i++)
        {
            idList.Add(_CurGameInfo.timeData.OfflineItemList[i].settingId);
            numList.Add(_CurGameInfo.timeData.OfflineItemList[i].count);
        }
         
        for (int i = 0; i < _CurGameInfo.allDanFarmData.DanFarmList.Count; i++)
        {
        
            SingleDanFarmData singleDanFarmData = _CurGameInfo.allDanFarmData.DanFarmList[i];
     
            if (singleDanFarmData.Status == (int)DanFarmStatusType.Working
                && singleDanFarmData.DanFarmWorkType == (int)DanFarmWorkType.Common)
            {
                DanFarmSetting setting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);

                int id = singleDanFarmData.ProductSettingId;
                int numPerMonth = LianDanManager.Instance.CalcDanFarmProducePerMonth(singleDanFarmData,true);

                long num = (int)(numPerMonth * totalFakeMonth * ConstantVal.offlineIncomeRate);
                if (num > 0)
                {
                    if (!idList.Contains(id))
                    {
                        ItemData item = new ItemData();
                        item.settingId = id;
                        item.count = (ulong)num;
                        idList.Add(id);
                        _CurGameInfo.timeData.OfflineItemList.Add(item);
                    }
                    else
                    {
                        int index = idList.IndexOf(id);
                        _CurGameInfo.timeData.OfflineItemList[index].count += (ulong)num;
                    }
                }
            }
        }
        // long num = (int)(numPerMonth * totalFakeMonth * ConstantVal.offlineIncomeRate);

        //离线房租
        int rent = (int)(RentManager.Instance.CalcRentConsume() * totalFakeMonth * ConstantVal.offlineIncomeRate);
        for (int i = _CurGameInfo.timeData.OfflineItemList.Count - 1; i >= 0; i--)
        {
            ItemData item = _CurGameInfo.timeData.OfflineItemList[i];
            if (item.settingId == (int)ItemIdType.LingShi)
            {
                if (item.count < (ulong)rent)
                {
                    _CurGameInfo.timeData.OfflineItemList.RemoveAt(i);
                    break;
                }
                else
                {
                    item.count -= (ulong)rent;
                }
            }
        }
        _CurGameInfo.timeData.OffLineTotalMinute = (int)totalMinute;

        _CurGameInfo.timeData.LastReceiveOfflineIncomeTime = after;
        EventCenter.Broadcast(TheEventType.RefreshOfflineShow);
    }

    /// <summary>
    /// 接收离线收益
    /// </summary>
    public void ReceiveOfflineIncome(bool ad = false)
    {
        GameTimeManager.Instance.GetServiceTime((x) =>
        {
            if (x <= 0)
                return;
            List<int> settingIdList = new List<int>();
            List<ulong> numList = new List<ulong>();
            for (int i = 0; i < _CurGameInfo.timeData.OfflineItemList.Count; i++)
            {
                ItemData data =_CurGameInfo.timeData.OfflineItemList[i];
                if (data.count <= 0)
                    continue;
                if (ad)
                {
                    data.count *= ConstantVal.adOfflineIncomeBeiLv;
                }
                settingIdList.Add(data.settingId);
                numList.Add(data.count);
                //ItemManager.Instance.GetItem(data.settingId, (ulong)data.count);
                //PanelManager.Instance.AddTongZhi(TongZhiType.Consume, "", ConsumeType.Item, data.settingId, (int)(int)(ulong)data.count);
            }
            ItemManager.Instance.GetItemWithAwardPanel(settingIdList, numList);
            _CurGameInfo.timeData.OfflineItemList.Clear();
            RoleManager.Instance._CurGameInfo.timeData.LastReceiveOfflineIncomeTime = x;
            EventCenter.Broadcast(TheEventType.RefreshOfflineShow);
        });

    }

    /// <summary>
    /// 送灵石
    /// </summary>
    void SendItem()
    {
        ItemManager.Instance.GetItem((int)ItemIdType.LingShi, 200);

        //ItemManager.Instance.GetItem((int)ItemIdType.YueLingKuang_Fan, 6000);
        //送100召集令
        //for(int i = 0; i < 100; i++)
        //{

        //    ItemManager.Instance.GetItem((int)ItemIdType.ZhaoJiLing, 1);

        //}
        //ItemManager.Instance.GetItem((int)ItemIdType.ZhaoJiLing, 1);
        //ItemManager.Instance.GetItem((int)ItemIdType.JiuTianXuanLeiJue, 1);

    }

    /// <summary>
    /// 赠送一个炼丹师傅
    /// </summary>
    void SendLianDanStudent()
    {
        // LianDanManager.Instance.AddALianDanStudent(p);
        //LianDanManager.Instance.AddALianDanStudent();
        //LianDanManager.Instance.AddALianDanStudent();

    }

 

    /// <summary>
    /// 送图纸
    /// </summary>
    void SendEquipPicture()
    {
        EquipmentManager.Instance.AddEquipPicture((int)EquipIdType.JieCaoJie);
        EquipmentManager.Instance.AddEquipPicture((int)EquipIdType.TaoMuJian);

    }

    public string rdmName(Gender gender)
    {
        string name = "";
        List<string> familyNameList = new List<string>();
        int count = DataTable._rdmNameList.Count;
        List<string> maleNameList = new List<string>();
        List<string> femaleNameList = new List<string>();

        for (int i = 0; i < count; i++)
        {
            string familyName = DataTable._rdmNameList[i].FamilyName;
            string maleName = DataTable._rdmNameList[i].Male;
            string femaleName = DataTable._rdmNameList[i].Female;

            if (!string.IsNullOrWhiteSpace(familyName))
            {
                familyNameList.Add(familyName);
            }
            if (!string.IsNullOrWhiteSpace(femaleName))
            {
                femaleNameList.Add(femaleName);
            }
            if (!string.IsNullOrWhiteSpace(maleName))
            {
                maleNameList.Add(maleName);
            }
        }
        int familyIndex = RandomManager.Next(0, familyNameList.Count);
        name = familyNameList[familyIndex];

        switch (gender)
        {
            case Gender.Male:
                int maleIndex = RandomManager.Next(0, maleNameList.Count);
                string maleName = maleNameList[maleIndex];
                name += maleName;
                break;
            case Gender.Female:
                int femaleIndex = RandomManager.Next(0, femaleNameList.Count);
                string femaleName = femaleNameList[femaleIndex];
                name += femaleName;
                break;
        }
        return name;
    }

    public void InitPlayerPeople(bool newPlayer)
    {
        if (newPlayer)
        {
            PeopleData playerPeople = new PeopleData();
            playerPeople.isPlayer = true;
            playerPeople.name = "";
            playerPeople.onlyId = ConstantVal.SetId;
            playerPeople.talent = (int)StudentTalent.LianGong;
            //四维
            for (int i = 0; i < DataTable._propertyList.Count; i++)
            {
                PropertySetting propertySetting = DataTable._propertyList[i];
                if (propertySetting.InitRdm == "1")
                {
                    SinglePropertyData singlePropertyData = new SinglePropertyData();
                    singlePropertyData.id = propertySetting.Id.ToInt32();
                    switch ((PropertyIdType)singlePropertyData.id)
                    {
                        case PropertyIdType.Tili:
                            singlePropertyData.num = 5;
                            singlePropertyData.limit = 999;
                            break;
                        default:
                            int num = 0;
                            if ((PropertyIdType)singlePropertyData.id == PropertyIdType.Hp)
                            {
                                num = 1200;
                                singlePropertyData.limit = num;
                            }
                            else if ((PropertyIdType)singlePropertyData.id == PropertyIdType.MpNum)
                            {
                                num = 0;
                                singlePropertyData.limit = 100;
                            }
                            else if ((PropertyIdType)singlePropertyData.id == PropertyIdType.MPSpeed)
                            {
                                num = 0;
                            }
                            else if ((PropertyIdType)singlePropertyData.id == PropertyIdType.Defense)
                                num = 240;
                            else if ((PropertyIdType)singlePropertyData.id == PropertyIdType.Attack)
                                num = 100;
                            else if ((PropertyIdType)singlePropertyData.id == PropertyIdType.CritNum)
                                num = 30;
                            else if ((PropertyIdType)singlePropertyData.id == PropertyIdType.CritRate)
                                num = 5;
                            else
                                num = 0;
                            singlePropertyData.num = num;
                            break;
                    }
                    playerPeople.propertyList.Add(singlePropertyData);
                    playerPeople.propertyIdList.Add(singlePropertyData.id);


                    SinglePropertyData battlePro = new SinglePropertyData();
                    battlePro.id = singlePropertyData.id;
                    battlePro.num = singlePropertyData.num;
                    battlePro.limit = singlePropertyData.limit;
                    if (propertySetting.BattlePro == "1")
                    {
                        playerPeople.curBattleProIdList.Add(battlePro.id);
                        playerPeople.curBattleProList.Add(battlePro);
                    }

                }
            }
            //血脉
            playerPeople.xueMai = new XueMaiData();
            for(int i = 0; i < 5; i++)
            {
                playerPeople.xueMai.xueMaiTypeList.Add((XueMaiType)(i+1));
                playerPeople.xueMai.xueMaiLevelList.Add(0);
            }
            _CurGameInfo.playerPeople = playerPeople;

            _CurGameInfo.AllPeopleList.Add(playerPeople);
        }



    }

    /// <summary>
    /// 初始化技能
    /// </summary>
    public void InitRoleSkill(bool newPlayer)
    {
        if (newPlayer)
        {    //技能
            _CurGameInfo.playerPeople.allSkillData = new AllSkillData();
            SingleSkillData singleSkillData = SkillManager.Instance.AddProtoSkill((int)SkillIdType.LingDan, _CurGameInfo.playerPeople);
            SkillManager.Instance.EquipProtoSkill(_CurGameInfo.playerPeople, singleSkillData);

            //暂时解锁 测试技能
            _CurGameInfo.playerPeople.allSkillData.unlockedSkillPos = 3;
            _CurGameInfo.playerPeople.allSkillData.unlockedTypeList.Add((int)UnlockType.UnLocked);
            _CurGameInfo.playerPeople.allSkillData.unlockedTypeList.Add((int)UnlockType.UnLocked);
            _CurGameInfo.playerPeople.allSkillData.unlockedTypeList.Add((int)UnlockType.UnLocked);
            _CurGameInfo.playerPeople.allSkillData.unlockedTypeList.Add((int)UnlockType.UnLocked);
            _CurGameInfo.playerPeople.allSkillData.unlockedTypeList.Add((int)UnlockType.UnLocked);
            _CurGameInfo.playerPeople.allSkillData.unlockedTypeList.Add((int)UnlockType.UnLocked);

            _CurGameInfo.playerPeople.allSkillData.unlockedTypeList.Add((int)UnlockType.UnLocked);
            _CurGameInfo.playerPeople.allSkillData.unlockedTypeList.Add((int)UnlockType.UnLocked);

        }

    }

    public SinglePropertyData FindMyProperty(int id)
    {
        if (RoleManager.Instance._CurGameInfo.playerPeople.propertyIdList.Contains(id))
        {
            int index = RoleManager.Instance._CurGameInfo.playerPeople.propertyIdList.IndexOf(id);
            SinglePropertyData data = RoleManager.Instance._CurGameInfo.playerPeople.propertyList[index];
            return data;
        }

        Debug.LogError("我没有id为" + id + "的属性");
        return null;
    }

    /// <summary>
    /// 得到装备
    /// </summary>
    /// <param name="equipProtoData"></param>
    public ItemData GetEquipment(EquipProtoData equipData, int quality)
    {
        ItemData item = ItemManager.Instance.GetItem(equipData, 1, (Quality)quality);
        return item;
    }

    ///// <summary>
    ///// 找到训练师的属性
    ///// </summary>
    ///// <param name="id"></param>
    ///// <returns></returns>
    //public SinglePropertyData FindProperty(int id,EquipMakeTeamData teamData)
    //{

    //    PeopleData p = RoleManager.Instance.FindEquipMakeStudentByOnlyId(teamData.PeopleOnlyId);
    //    if (p.PropertyIdList.Contains(id))
    //    {
    //        int index = p.PropertyIdList.IndexOf(id);
    //        SinglePropertyData data = p.PropertyList[index];
    //        return data;
    //    }

    //    Debug.LogError("团队没有id为" + id + "的属性");
    //    return null;
    //}

    /// <summary>
    /// 找到属性
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SinglePropertyData FindProperty(int id, PeopleData p)
    {

        if (p.propertyIdList.Contains(id))
        {
            int index = p.propertyIdList.IndexOf(id);
            SinglePropertyData data = p.propertyList[index];
            return data;
        }

        Debug.LogError("没有id为" + id + "的属性");
        return null;
    }

    /// <summary>
    /// 增加能力值 传了人就是给人加
    /// </summary>
    /// <param name="propertyIdType"></param>
    /// <param name="num"></param>
    public void AddProperty(PropertyIdType propertyIdType, int num, PeopleData people = null)
    {
        if (people == null)
        {
            people = RoleManager.Instance._CurGameInfo.playerPeople;
        }
        for (int i = 0; i < people.propertyList.Count; i++)
        {
            SinglePropertyData pro = people.propertyList[i];
            if (pro.id == (int)propertyIdType)
            {
                PropertySetting setting = DataTable.FindPropertySetting(pro.id);
                int limit = setting.Limit.ToInt32();
                int realAdd = num;
                //如果该属性存在最大限制
                if (limit > 0)
                {
                    if (realAdd + pro.num >= limit)
                    {
                        realAdd = limit - pro.num;
                    }
                }
                pro.num += realAdd;
            }
        }

        EventCenter.Broadcast(TheEventType.PropertyAdd);
    }

    /// <summary>
    /// 增加源力上限
    /// </summary>
    /// <param name="tiliLimit"></param>
    public void AddTiLiLimit(int tiliLimit)
    {
        SinglePropertyData tiliPro = FindProperty((int)PropertyIdType.Tili, RoleManager.Instance._CurGameInfo.playerPeople);
        _CurGameInfo.allZongMenData.TiliLimit = tiliLimit;
        //tiliPro.Limit = tiliLimit;
    }

    /// <summary>
    /// 满体力
    /// </summary>
    public void FullTiLi()
    {
        SinglePropertyData tiliPro = FindProperty((int)PropertyIdType.Tili, RoleManager.Instance._CurGameInfo.playerPeople);
        int num = RoleManager.Instance._CurGameInfo.allZongMenData.TiliLimit;
        //if (num > 0)
        //{
        AddProperty(PropertyIdType.Tili, num);

        //}

    }

    /// <summary>
    /// 减少能力值
    /// </summary>
    /// <param name="propertyIdType"></param>
    /// <param name="num"></param>
    public void DeProperty(PropertyIdType propertyIdType, int num, PeopleData people = null)
    {
        if (num > 0)
            return;
        if (people == null)
        {
            people = RoleManager.Instance._CurGameInfo.playerPeople;
        }
        for (int i = 0; i < people.propertyList.Count; i++)
        {
            SinglePropertyData singleData = people.propertyList[i];
            if (singleData.id == (int)propertyIdType)
            {
                singleData.num += num;
                //如果该属性小于0 则等于0
                if (singleData.num <= 0)
                {
                    singleData.num = 0;
                }
            }

        }
        EventCenter.Broadcast(TheEventType.PropertyDecrease);

    }

    /// <summary>
    /// 增加战斗能力值
    /// </summary>
    /// <param name="propertyIdType"></param>
    /// <param name="num"></param>
    public void AddBattleProperty(PropertyIdType propertyIdType, int num, PeopleData people = null)
    {
        if (num < 0)
            return;
        if (people == null)
        {
            people = RoleManager.Instance._CurGameInfo.playerPeople;
        }
        for (int i = 0; i < people.curBattleProList.Count; i++)
        {
            SinglePropertyData singleData = people.curBattleProList[i];
            if (singleData.id == (int)propertyIdType)
            {
                singleData.num += num;
                //如果该属性小于0 则等于0
                if (singleData.id == (int)PropertyIdType.Hp || singleData.id == (int)PropertyIdType.MpNum)
                {
                    if (singleData.num >= singleData.limit)
                    {
                        singleData.num = (int)singleData.limit;
                    }
                }

            }

        }
        EventCenter.Broadcast(TheEventType.AddBattleProperty, people, propertyIdType);
        EventCenter.Broadcast(TheEventType.StudentStatusChange, people);

    }
    /// <summary>
    /// 减少战斗能力值
    /// </summary>
    /// <param name="propertyIdType"></param>
    /// <param name="num"></param>
    public void DeBattleProperty(PropertyIdType propertyIdType, int num, PeopleData people = null)
    {
        if (num > 0)
        {
            Debug.LogError("扣了正的属性");
            return;

        }
        if (people == null)
        {
            people = RoleManager.Instance._CurGameInfo.playerPeople;
        }
        for (int i = 0; i < people.curBattleProList.Count; i++)
        {
            SinglePropertyData singleData = people.curBattleProList[i];
            if (singleData.id == (int)propertyIdType)
            {
                singleData.num += num;
                //如果该属性小于0 则等于0
                if (singleData.num <= 0)
                {
                    singleData.num = 0;
                    if (singleData.id == (int)PropertyIdType.Hp)
                    {
                        bool fuhuoed = false;
                        List<BattleBuff> fuHuoList = BattleManager.Instance.FindTypeBuffList(people, BattleBuffType.FuHuo);
                        if (fuHuoList.Count > 0)
                        {
                            for(int j = 0; j < fuHuoList.Count; j++)
                            {
                                BattleBuff singleFuHuo = fuHuoList[j];
                                int addHPNum = (int)(singleFuHuo.buffSetting.Param.ToFloat() * 0.01f * GetCurBattleProperty(PropertyIdType.Hp, people).limit);

                                RoleManager.Instance.AddBattleProperty(PropertyIdType.Hp, addHPNum, people);
                                BattleManager.Instance.RemoveBuff(people, singleFuHuo);
                                EventCenter.Broadcast(TheEventType.BattleInfoTxtShow, people, "复活");
                                fuhuoed = true;
                            }
                    
                        }
                        if (!fuhuoed)
                        {
                            
                            List<EquipTaoZhuangType> taoZhuangList = EquipmentManager.Instance.CheckEquipTaoZhuang(people);
                            if (taoZhuangList.Count >= 2
                                    && taoZhuangList[0] == taoZhuangList[1])
                            {
                                //帝骨套
                                if (taoZhuangList[0] == EquipTaoZhuangType.DiGu
                                    &&!people.diGuFuHuoed)
                                {
                                    
                                    EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[0]);
                                    int addHPNum = (int)(taoZhuangSetting.Param2.ToFloat() * 0.01f * GetCurBattleProperty(PropertyIdType.Hp, people).limit);
                                    RoleManager.Instance.AddBattleProperty(PropertyIdType.Hp, addHPNum, people);
                                    EventCenter.Broadcast(TheEventType.BattleInfoTxtShow, people, "复活");

                                    people.diGuFuHuoed = true;
                                    fuhuoed=true;
                                }
                            }
                        }
                        if (!fuhuoed)
                        {
                            List<BattleBuff> ziBaoList = BattleManager.Instance.FindTypeBuffList(people, BattleBuffType.ZiBao);
                            if (ziBaoList.Count > 0)
                            {
                                BattleBuff singleZiBao = ziBaoList[0];
                                SingleSkillData ziBaoSkill = new SingleSkillData();
                                ziBaoSkill.yuanSuType =(YuanSuType)people.yuanSu;
                                ziBaoSkill.skillId = singleZiBao.buffSetting.Param2.ToInt32();
                                ziBaoSkill.skillLevel = SkillManager.Instance.FindSkillById(people.allSkillData.equippedSkillIdList[0], people).skillLevel*2;
                                BattleManager.Instance.SingleAttack(people, ziBaoSkill,0);
                                BattleManager.Instance.RemoveBuff(people, singleZiBao);
                                
                            }

                        }

                    }

                }
            }

        }
        EventCenter.Broadcast(TheEventType.DeBattleProperty, people, propertyIdType);

    }


    /// <summary>
    /// 是否属性足够
    /// </summary>
    /// <returns></returns>
    public bool CheckIfPropertyEnough(int proId, int needNum)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.playerPeople.propertyList.Count; i++)
        {
            SinglePropertyData data = RoleManager.Instance._CurGameInfo.playerPeople.propertyList[i];
            if (data.id == proId)
            {
                if (data.num < needNum)
                    return false;
                else
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 地图灵气充沛处修炼
    /// </summary>
    public void OnMapEventXiuLian(SingleMapEventData singleMapEventData)
    {
        //MapEventSetting mapEventSetting = DataTable.FindMapEventSetting(singleMapEventData.SettingId);


        MapEventManager.Instance.ExecuteMapEvent(singleMapEventData);

    }
    /// <summary>
    /// 修为丹修炼
    /// </summary>
    public void OnDanXiuLian(PeopleData p, ItemData item)
    {
       
        //if(p.todayEatXiuWeiDanNum>= ConstantVal.dailyEatXiuWeiDanLimit)//去除服用丹药上限
        //{
        //    PanelManager.Instance.OpenFloatWindow("今日服丹数量已达上限");
        //    return;
        //}
        //int xiuweiDanId = (int)ItemIdType.FanJiYiQiDan;
        ItemSetting itemSetting = DataTable.FindItemSetting(item.settingId);

        if (!ItemManager.Instance.CheckIfItemEnough(item.settingId, 1))
        {
            PanelManager.Instance.OpenFloatWindow(itemSetting.Name + "不够");
            return;
        }
        int giantLevel = GiantLevel(p);// _CurGameInfo.playerPeople.CurTrainIndex / 30 + 1;

        if (itemSetting.Param3.ToInt32() != giantLevel)
        {

             
            PanelManager.Instance.OpenFloatWindow(itemSetting.Name + LanguageUtil.GetLanguageText((int)LanguageIdType.不符合境界需求));
            return;
        }

        ItemManager.Instance.LoseItem(item.settingId, 1);
        int param = itemSetting.Param.ToInt32();
        p.curXiuwei +=(ulong)ItemManager.Instance.XiuWeiDanXiuWeiAdd(item);
        p.todayEatXiuWeiDanNum++;
    
        EventCenter.Broadcast(TheEventType.SuccessXiuLian);
    }
    
 


    public int GiantLevel(PeopleData p)
    {
        return p.trainIndex / 30 + 1;
    }

    /// <summary>
    /// 中境界
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public int FindCenterLevel(PeopleData p)
    {
        return p.trainIndex / 10 + 1;
    }

    /// <summary>
    /// 得到当前战斗属性
    /// </summary>
    /// <returns></returns>
    public SinglePropertyData GetCurBattleProperty(PropertyIdType propertyIdType, PeopleData peopleData)
    {

        if (peopleData == null)
        {

        }
        for (int i = 0; i < peopleData.curBattleProList.Count; i++)
        {
            int theId = peopleData.curBattleProIdList[i];
            if (theId == (int)propertyIdType)
            {
                return peopleData.curBattleProList[i];
            }
        }
        //没有该属性 做一个该属性为0的新属性
        return null;
    }
    /// <summary>
    /// 得到当前战斗属性
    /// </summary>
    /// <returns></returns>
    public int GetCurBattlePropertyNum(PropertyIdType propertyIdType, PeopleData peopleData)
    {
        int res = 0;
        if (peopleData == null)
        {

        }
        for (int i = 0; i < peopleData.curBattleProList.Count; i++)
        {
            int theId = peopleData.curBattleProIdList[i];
            if (theId == (int)propertyIdType)
            {
                res= peopleData.curBattleProList[i].num;
                break;
            }
        }
        //没有该属性 做一个该属性为0的新属性
        return res;
    }

    ///// <summary>
    ///// 通过唯一id找人 找茬和参赛
    ///// </summary>
    ///// <param name="onlyId"></param>
    ///// <returns></returns>
    //public PeopleData FindPeopleByOnlyId(UInt64 onlyId)
    //{
    //    if (onlyId == _CurGameInfo.playerPeople.OnlyId)
    //    {
    //        return _CurGameInfo.playerPeople;
    //    }
    //    for (int i = 0; i < _CurGameInfo.AllPeopleList.Count; i++)
    //    {
    //        PeopleData theData = _CurGameInfo.AllPeopleList[i];
    //        if (onlyId == theData.OnlyId)
    //        {
    //            return theData;
    //        }
    //    }
    //    return null;
    //}




    /// <summary>
    /// 获取下次比赛的时间
    /// </summary>
    /// <returns></returns>
    public int GetWeekBeforeNextMatch(MatchSetting setting)
    {
        int theWeek = setting.Week.ToInt32();

        int curWeek = _CurGameInfo.timeData.Week;
        int weekNum = theWeek - curWeek;
        if (curWeek > theWeek)
        {
            //下个月
            int val = 4 - curWeek;
            weekNum = val + theWeek;
        }
        return weekNum;
    }


    ///// <summary>
    ///// 设置当前装备和技能
    ///// </summary>
    //public void SetCurEquipAndSkill()
    //{
    //    if (_CurGameInfo.playerPeople.CurEquip == null)
    //    {
    //        _CurGameInfo.playerPeople.CurEquip = EquipmentManager.Instance.myEquipList[0].equipProtoData;

    //    }
    //    //技能
    //    //_CurGameInfo.playerPeople.AllSkillData.SkillList.Clear();
    //    EquipmentSetting equipmentSetting = DataTable.FindEquipSetting(_CurGameInfo.playerPeople.CurEquip.SettingId);

    //    string[] skillIdArr = equipmentSetting.skillId.Split('|');
    //    int id1 = skillIdArr[0].ToInt32();
    //    int id2 = skillIdArr[1].ToInt32();

    //    SingleSkillData data1 = new SingleSkillData();
    //    data1.SkillId = id1;
    //    data1.SkillLevel = _CurGameInfo.playerPeople.CurEquip.CurLevel;

    //    _CurGameInfo.playerPeople.AllSkillData.EquippedSkillList[0]=data1;


    //    //SingleSkillData data2 = new SingleSkillData();
    //    //data2.SkillId = id2;
    //    //data2.SkillLevel = _CurGameInfo.playerPeople.CurEquip.CurLevel / 5 + 1;
    //    //_CurGameInfo.playerPeople.AllSkillData.SkillList.Add(data2);
    //}
 
    /// <summary>
    /// buff经过一天
    /// </summary>
    public void BuffPassDay()
    {
        //如果有，则刷新时间
        for (int i = _CurGameInfo.AllBuffData.BuffList.Count - 1; i >= 0; i--)
        {
            SingleBuffData data = _CurGameInfo.AllBuffData.BuffList[i];
            data.RemainDay -= 1;
            if (data.RemainDay <= 0)
            {
                _CurGameInfo.AllBuffData.BuffList.RemoveAt(i);
                EventCenter.Broadcast(TheEventType.RemoveBuff);

            }
        }
    }

    //清除所有人
    public void ClearAllEnemy()
    {
        int count = _CurGameInfo.AllPeopleList.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            PeopleData p = _CurGameInfo.AllPeopleList[i];
            if (!p.isPlayer)
                _CurGameInfo.AllPeopleList.RemoveAt(i);
        }
    }


    ///// <summary>
    ///// 升级山门
    ///// </summary>
    //public void UpgradeMountain()
    //{
    //    RoleManager.Instance._CurGameInfo.StudentData.MaxStudentNum = RoleManager.Instance._CurGameInfo.AllBuildingData.MountainLevel * 2;
    //}


    /// <summary>
    /// 刷新战斗属性 装备强化 宝石镶嵌 宝石合成 装备安装卸下 角色升级 弟子升级 耐久度下降需要调用 主要是刷新血量和蓝量
    /// </summary>
    /// <returns></returns>
    public void RefreshBattlePro(PeopleData p)
    {
        //如果没有battlePro 那么是第一次战斗 需要直接赋值
        List<SinglePropertyData> newPro = GetTotalBattlePro(p);

        if (p.curBattleProIdList.Count == 0)
        {
            for (int i = 0; i < newPro.Count; i++)
            {
                SinglePropertyData thePro = newPro[i];
                SinglePropertyData theBattlePro = new SinglePropertyData();
                theBattlePro.limit = thePro.limit;
                theBattlePro.id = thePro.id;

                theBattlePro.num = thePro.num;

                p.curBattleProIdList.Add(theBattlePro.id);
                p.curBattleProList.Add(theBattlePro);
            }
        }
        else
        {

            for (int i = 0; i < newPro.Count; i++)
            {
                //属性得完全覆盖 血量和蓝量按比例更新
                SinglePropertyData thePro = newPro[i];
                SinglePropertyData theBattlePro = GetCurBattleProperty((PropertyIdType)(int)thePro.id, p);//  p.CurBattleProList[i];
                int beforeNum = theBattlePro.num;
                int beforeLimit = (int)theBattlePro.limit;
                theBattlePro.limit = thePro.limit;

                if (thePro.id == (int)PropertyIdType.Hp
                    || thePro.id == (int)PropertyIdType.MpNum)
                {
                    float rate = beforeNum / (float)beforeLimit;
                    theBattlePro.num = Mathf.RoundToInt(rate * thePro.limit);
                }
                else
                {
                    theBattlePro.num = thePro.num;
                }

            }
        }

    }


    /// <summary>
    /// 获取继续战斗属性 血条回满
    /// </summary>
    /// <param name="PeopleData"></param>
    /// <returns></returns>
    public List<SinglePropertyData> GetContinueBattlePro(PeopleData peopleData)
    {
        //后续为把战斗中buff去掉后算的属性
        List<SinglePropertyData> res = new List<SinglePropertyData>();
        #region 先找到当前mp属性
        //MP需要在应得的位置
        List<SinglePropertyData> newPro = GetTotalBattlePro(peopleData);
        SinglePropertyData mpPro = null;
        for (int i = 0; i < newPro.Count; i++)
        {
            SinglePropertyData thePro = newPro[i];
            if (thePro.id == (int)PropertyIdType.MpNum)
            {
                mpPro = thePro;
            }
        }
        #endregion
        //再把当前战斗属性赋值给当前战斗属性
        for (int i = 0; i < peopleData.curBattleProList.Count; i++)
        {
            SinglePropertyData singlePropertyData = peopleData.curBattleProList[i];
            if (singlePropertyData.id == (int)PropertyIdType.MpNum)
            {
                res.Add(mpPro);
            }
            else
            {
                res.Add(singlePropertyData);

            }
        }
        return res;
    }


    /// <summary>
    /// 获取战斗用最终属性
    /// </summary>
    /// <returns></returns>
    public List<SinglePropertyData> GetTotalBattlePro(PeopleData peopleData)
    {
        int totalProDamageAdd = 0;
        List<SinglePropertyData> res = new List<SinglePropertyData>();

        List<int> equipProAddIdList = new List<int>();
        List<SinglePropertyData> equipProAddList = new List<SinglePropertyData>();

        //宝石加成
        List<int> gemProAddIdList = new List<int>();
        List<SinglePropertyData> gemProAddList = new List<SinglePropertyData>();
       
        //血脉加成
        List<int> xueMaiAddIdList = new List<int>();
        List<SinglePropertyData> xueMaiAddList = new List<SinglePropertyData>();
        if (peopleData.xueMai != null)
        {
            for (int i = 0; i < peopleData.xueMai.xueMaiTypeList.Count; i++)
            {
                XueMaiType xueMaiType = peopleData.xueMai.xueMaiTypeList[i];
                int level = peopleData.xueMai.xueMaiLevelList[i];
                int num = XueMaiManager.Instance.PerLevelAddNum(xueMaiType) * level;
                if (xueMaiType == XueMaiType.Xue)
                {
                    xueMaiAddIdList.Add((int)PropertyIdType.Hp);
                    SinglePropertyData pro = new SinglePropertyData();
                    pro.id = (int)PropertyIdType.Hp;
                    pro.num = num;
                    xueMaiAddList.Add(pro);
                }
                else if (xueMaiType == XueMaiType.Gong)
                {
                    xueMaiAddIdList.Add((int)PropertyIdType.Attack);
                    SinglePropertyData pro = new SinglePropertyData();
                    pro.id = (int)PropertyIdType.Attack;
                    pro.num = num;
                    xueMaiAddList.Add(pro);
                }
                else if (xueMaiType == XueMaiType.Fang)
                {
                    xueMaiAddIdList.Add((int)PropertyIdType.Defense);
                    SinglePropertyData pro = new SinglePropertyData();
                    pro.id = (int)PropertyIdType.Defense;
                    pro.num = num;
                    xueMaiAddList.Add(pro);
                }
                else if (xueMaiType == XueMaiType.JingTong)
                {
                    xueMaiAddIdList.Add((int)PropertyIdType.JingTong);
                    SinglePropertyData pro = new SinglePropertyData();
                    pro.id = (int)PropertyIdType.JingTong;
                    pro.num = num;
                    xueMaiAddList.Add(pro);
                }
            }
        }
       for(int a = 0; a < peopleData.curEquipItemList.Count; a++)
        {
            ItemData ItemData =peopleData.curEquipItemList[a];
            if (ItemData != null)
            {
                EquipProtoData EquipProtoData = ItemData.equipProtoData;
                for (int i = 0; EquipProtoData!=null && i < EquipProtoData.propertyIdList.Count; i++)
                {
                    int theId = EquipProtoData.propertyIdList[i];
                    int theNum = EquipProtoData.propertyList[i].num;
                    if (!equipProAddIdList.Contains(theId))
                    {
                        equipProAddIdList.Add(theId);
                        SinglePropertyData newData = new SinglePropertyData();
                        newData.id = theId;
                        newData.num = theNum;
                        equipProAddList.Add(newData);
                    }
                    else
                    {
                        int index = equipProAddIdList.IndexOf(theId);
                        equipProAddList[index].num += theNum;
                    }

                }
                for (int i = 0; EquipProtoData!=null && i < EquipProtoData.gemList.Count; i++)
                {
                    ItemData gem = EquipProtoData.gemList[i];
                    if (gem == null || gem.onlyId <= 0)
                        continue;
                    UInt64 gemOnlyId = EquipProtoData.gemList[i].onlyId;
                    if (gemOnlyId > 0)
                    {

                        ItemData gemItem = EquipProtoData.gemList[i];
                        for (int j = 0; j < gemItem.gemData.propertyIdList.Count; j++)
                        {
                            int proId = gemItem.gemData.propertyList[j].id;
                            SinglePropertyData data = gemItem.gemData.propertyList[j];
                            if (!gemProAddIdList.Contains(proId))
                            {
                                gemProAddIdList.Add(proId);
                                SinglePropertyData newData = new SinglePropertyData();
                                newData.id = proId;
                                newData.num = data.num;
                                gemProAddList.Add(newData);
                            }
                            else
                            {
                                int index = gemProAddIdList.IndexOf(proId);
                                gemProAddList[index].num += data.num;
                            }
                        }

                    }
                }
            }
    
        }
        List<EquipTaoZhuangType> taoZhuangList = EquipmentManager.Instance.CheckEquipTaoZhuang(peopleData);


        for (int i = 0; i < peopleData.propertyIdList.Count; i++)
        {
            int id = peopleData.propertyIdList[i];
            SinglePropertyData pro = peopleData.propertyList[i];

            PropertySetting setting = DataTable.FindPropertySetting(id);
            if (setting.BattlePro == "1")
            {
                SinglePropertyData newPro = new SinglePropertyData();
                newPro.id = id;
                newPro.num = pro.num;
                newPro.quality = pro.quality;
                //血脉绿值加成
                if (xueMaiAddIdList.Contains(newPro.id))
                {
                    int addPro = 0;
                    int index = xueMaiAddIdList.IndexOf(newPro.id);
                    SinglePropertyData xueMaiPro = xueMaiAddList[index];
                    if (xueMaiPro.id != (int)PropertyIdType.JingTong)
                        addPro = Mathf.RoundToInt(pro.num * 0.01f * xueMaiPro.num);
                    else
                        addPro = xueMaiPro.num;
                    newPro.num += addPro;

                }

                //宝石绿值加成
                if (gemProAddIdList.Contains(newPro.id))
                {
                    int addPro = 0;
                    int index = gemProAddIdList.IndexOf(newPro.id);
                    SinglePropertyData gemPro = gemProAddList[index];

                    switch ((PropertyIdType)(int)newPro.id)
                    {
                        //锐
                        case PropertyIdType.CritRate:
                            addPro = Mathf.RoundToInt(gemPro.num);
                            break;
                        //伤
                        case PropertyIdType.CritNum:
                            addPro = Mathf.RoundToInt(gemPro.num);
                            break;
                        //气
                        case PropertyIdType.MPSpeed:
                            addPro = Mathf.RoundToInt(gemPro.num);
                            break;
                        //精通
                        case PropertyIdType.JingTong:
                            addPro = gemPro.num;
                            break;
                        default:
                            addPro = Mathf.RoundToInt(pro.num * 0.01f * gemPro.num);
                            break;
                    }
                    newPro.num += addPro;
                }

                if (taoZhuangList.Count > 0)
                {      
                    //帝血
                    if (taoZhuangList.Contains(EquipTaoZhuangType.DiXue))
                    {
                        int index = taoZhuangList.IndexOf(EquipTaoZhuangType.DiXue);
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[index]);
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param);
                        if (newPro.id == (int)PropertyIdType.CritRate)
                        {
                            newPro.num += paramList[0];
                        }
                        else if (newPro.id == (int)PropertyIdType.CritNum)
                        {
                            newPro.num += paramList[1];
                        }
                    }   
                    //天龙
                    if (taoZhuangList.Contains(EquipTaoZhuangType.TianLong))
                    {
                        int index = taoZhuangList.IndexOf(EquipTaoZhuangType.TianLong);
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[index]);
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param);
                        if (newPro.id == (int)PropertyIdType.Attack)
                        {
                            int addPro = Mathf.RoundToInt(pro.num * 0.01f * paramList[0]);

                            newPro.num += addPro;
                        }
                        //else if (newPro.id == (int)PropertyIdType.CritNum)
                        //{
                        //    newPro.num += paramList[1];
                        //}
                    }  
                    //神风
                    if (taoZhuangList.Contains(EquipTaoZhuangType.ShenFeng))
                    {
                        int index = taoZhuangList.IndexOf(EquipTaoZhuangType.ShenFeng);
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[index]);
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param);
                        if (newPro.id == (int)PropertyIdType.CritNum)
                        {
                            int addPro =  paramList[0];

                            newPro.num += addPro;
                        }
                        //else if (newPro.id == (int)PropertyIdType.CritNum)
                        //{
                        //    newPro.num += paramList[1];
                        //}
                    }   
                    //原始
                    if (taoZhuangList.Contains(EquipTaoZhuangType.YuanShi))
                    {
                        int index = taoZhuangList.IndexOf(EquipTaoZhuangType.YuanShi);
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[index]);
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param);
                        if (newPro.id == (int)PropertyIdType.TotalProDamageAdd)
                        {
                            int addPro = Mathf.RoundToInt(paramList[0]);

                            newPro.num += addPro;
                        }
                
                   
                    }
                    //玄武
                    if (taoZhuangList.Contains(EquipTaoZhuangType.XuanWu))
                    {
                        int index = taoZhuangList.IndexOf(EquipTaoZhuangType.XuanWu);
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[index]);
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param);
                        if (newPro.id == (int)PropertyIdType.Defense)
                        {
                            int addPro = Mathf.RoundToInt(pro.num * 0.01f * paramList[0]);
                            newPro.num += addPro;
                        }
                    }
                    //飞云
                    if (taoZhuangList.Contains(EquipTaoZhuangType.FeiYun))
                    {
                        int index = taoZhuangList.IndexOf(EquipTaoZhuangType.FeiYun);
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[index]);
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param);
                        if (newPro.id == (int)PropertyIdType.MPSpeed)
                        {
                            int addPro = paramList[0];
                            newPro.num += addPro;
                        }
                    }
                    //月华
                    if (taoZhuangList.Contains(EquipTaoZhuangType.YueHua))
                    {
                        int index = taoZhuangList.IndexOf(EquipTaoZhuangType.YueHua);
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[index]);
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param);
                        if (newPro.id == (int)PropertyIdType.JingTong)
                        {
                            int addPro = paramList[0];
                            newPro.num += addPro;
                        }

                    }      
                    //乙木
                    if (taoZhuangList.Contains(EquipTaoZhuangType.YiMu))
                    {
                        int index = taoZhuangList.IndexOf(EquipTaoZhuangType.YiMu);
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[index]);
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param);
                        if (newPro.id == (int)PropertyIdType.Hp)
                        {
                            int addPro = Mathf.RoundToInt(pro.num * 0.01f * paramList[0]);
                            newPro.num += addPro;
                        }

                    }    
                    //帝骨
                    if (taoZhuangList.Contains(EquipTaoZhuangType.DiGu))
                    {
                        int index = taoZhuangList.IndexOf(EquipTaoZhuangType.DiGu);
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[index]);
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param);
                        if (newPro.id == (int)PropertyIdType.Attack)
                        {
                            int addPro = Mathf.RoundToInt(pro.num * 0.01f * paramList[0]);
                            newPro.num += addPro;
                        }
                    }
                    //洪荒
                    if (taoZhuangList.Contains(EquipTaoZhuangType.HongHuang))
                    {
                        int index = taoZhuangList.IndexOf(EquipTaoZhuangType.HongHuang);
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[index]);
                        List<int> paramList = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param);
                        if (newPro.id == (int)PropertyIdType.TotalProDamageAdd)
                        {
                            int addPro = Mathf.RoundToInt( paramList[0]);
                            newPro.num += addPro;
                        }
                    }
                    ////4件套
                    //if (taoZhuangList.Count >= 2
                    //        &&taoZhuangList[0] == taoZhuangList[1])
                    //{
                    //    //帝血
                    //    if (taoZhuangList[0] == EquipTaoZhuangType.DiXue)
                    //    {
                    //        //帝血2件套
                    //        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[0]);
                    //        List<int> paramList = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.param);
                    //        if (newPro.id == (int)PropertyIdType.CritRate)
                    //        {
                    //            newPro.num += paramList[0];
                    //        }
                    //        else if (newPro.id == (int)PropertyIdType.CritNum)
                    //        {
                    //            newPro.num += paramList[1];
                    //        }
                    //        ////帝血4件套
                    //        //List<int> paramList2 = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.param2);
                    //        //if (newPro.id == (int)PropertyIdType.CritRate)
                    //        //{
                    //        //    newPro.num += paramList2[0];
                    //        //}
                    //    }
                    //}
                    ////2件套 分别看
                    //else
                    //{     



                    //}
                }

                //装备加成
                if (equipProAddIdList.Contains(newPro.id))
                {
                    int index = equipProAddIdList.IndexOf(newPro.id);
                    newPro.num += equipProAddList[index].num;
                }
                if (id == (int)PropertyIdType.Hp)
                {
                    newPro.limit = newPro.num;
                }
                else if (id == (int)PropertyIdType.MpNum)
                {
                    newPro.limit = 100;
                    //气保持在上次的气
                    if (GetCurBattleProperty(PropertyIdType.MpNum, peopleData) == null)
                    {
                        newPro.num = 0;
                    } else
                        newPro.num = GetCurBattleProperty(PropertyIdType.MpNum, peopleData).num;
                    ////初始气最大为60点
                    //if (newPro.Num >= 60)
                    //    newPro.Num = 60;
                }
                else
                {
                    newPro.limit = -1;
                }
                //计算完了以后看原始四件套

   
                res.Add(newPro);
            }
        }
    
        return res;
    }
    /// <summary>
    /// 获取显示的属性list
    /// </summary>
    /// <returns></returns>
    public List<SinglePropertyData> GetShowProList(PeopleData p)
    {
        List<SinglePropertyData> res = new List<SinglePropertyData>();
        List<SinglePropertyData> candidate = GetTotalBattlePro(p);
        for(int i = 0; i < candidate.Count; i++)
        {
            SinglePropertyData data = candidate[i];
            if ((PropertyIdType)(int)data.id == PropertyIdType.Hp
                || (PropertyIdType)(int)data.id == PropertyIdType.Attack
                || (PropertyIdType)(int)data.id == PropertyIdType.Defense)
                res.Add(data);
        }
        return res;
    }

 

    /// <summary>
    /// 初始化所有竞技场宗门数据容器（不再自动生成宗门，由 MatchManager 按需生成）
    /// </summary>
    /// <param name="newPlayer"></param>
    public void InitAllOtherZongMenData(bool newPlayer)
    {
        //只初始化容器，不生成宗门数据
        if (_CurGameInfo.AllOtherZongMenData == null)
        {
            _CurGameInfo.AllOtherZongMenData = new OtherZongMenData();
        }
    }

    /// <summary>
    /// 获取属性显示
    /// </summary>
    /// <returns></returns>
    public string GetPropertyShow(int id,int num)
    {
        switch ((PropertyIdType)id)
        {
            case PropertyIdType.CritNum:
                return num +"%";
            case PropertyIdType.CritRate:
                return num + "%";
            default:
                return num.ToString();

        }
    }

    /// <summary>
    /// 查找是我还是弟子
    /// </summary>
    /// <returns></returns>
    public bool CheckIfMeOrStudent(UInt64 onlyId)
    {
        if (onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
            return true;

        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];

            if (p.onlyId == onlyId)
                return true;
        }
        
        return false;
    }


    /// <summary>
    /// 回满血
    /// </summary>
    public void FullHp(PeopleData p)
    {
        p.seriousInjury = false;
        SinglePropertyData data = GetCurBattleProperty(PropertyIdType.Hp, p);
        data.num = (int)data.limit;
        EventCenter.Broadcast(TheEventType.RefreshPeopleShow);

    }
    /// <summary>
    /// 能量回满
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    public void FullMP(PeopleData p)
    {
        SinglePropertyData data = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, p);
        data.num = (int)data.limit;
    }

    /// <summary>
    /// 随机一张脸
    /// </summary>
    public void RdmFace(PeopleData p)
    {
        p.portraitIndexList.Clear();
        int houfaNum = 0;
        int zhongfaNum = 0;
        int faceNum = 0;
        int biziNum = 0;
        int meimaoNum = 0;
        int eyeNum = 0;
        int zuiNum = 0;
        int bodyNum = 0;
        int lianshiNum = 0;
        int qianfaNum = 0;

        switch ((Gender)(int)p.gender)
        {
            case Gender.Male:
                houfaNum = ConstantVal.malehoufaNum;
                zhongfaNum = ConstantVal.malezhongfaNum;
                faceNum = ConstantVal.malefaceNum;
                biziNum = ConstantVal.malebiziNum;
                meimaoNum = ConstantVal.malemeimaoNum;
                eyeNum = ConstantVal.maleeyeNum;
                zuiNum = ConstantVal.malezuiNum;
                bodyNum = ConstantVal.malebodyNum;
                lianshiNum = ConstantVal.malelianshiNum;
                qianfaNum = ConstantVal.maleqianfaNum;
                break;

            case Gender.Female:
                houfaNum = ConstantVal.femalehoufaNum;
                zhongfaNum = ConstantVal.femalezhongfaNum;
                faceNum = ConstantVal.femalefaceNum;
                biziNum = ConstantVal.femalebiziNum;
                meimaoNum = ConstantVal.femalemeimaoNum;
                eyeNum = ConstantVal.femaleeyeNum;
                zuiNum = ConstantVal.femalezuiNum;
                bodyNum = ConstantVal.femalebodyNum;
                lianshiNum = ConstantVal.femalelianshiNum;
                qianfaNum = ConstantVal.femaleqianfaNum;
                break;
        }

        int houfaIndex = RandomManager.Next(0, houfaNum);
        int zhongfaIndex = RandomManager.Next(0, zhongfaNum);
        int faceIndex = RandomManager.Next(0, faceNum);
        int biziIndex = RandomManager.Next(0, biziNum);
        int meimaoIndex = RandomManager.Next(0, meimaoNum);
        int eyeIndex = RandomManager.Next(0, eyeNum);
        int zuiIndex = RandomManager.Next(0, zuiNum);
        int bodyIndex = RandomManager.Next(0, bodyNum);
        int lianshiIndex = RandomManager.Next(0, lianshiNum);
        int qianfaIndex = RandomManager.Next(0, qianfaNum);
        p.portraitIndexList.Add(houfaIndex);
        p.portraitIndexList.Add(zhongfaIndex);
        p.portraitIndexList.Add(faceIndex);
        p.portraitIndexList.Add(biziIndex);
        p.portraitIndexList.Add(meimaoIndex);
        p.portraitIndexList.Add(eyeIndex);
        p.portraitIndexList.Add(zuiIndex);
        p.portraitIndexList.Add(bodyIndex);
        p.portraitIndexList.Add(lianshiIndex);
        p.portraitIndexList.Add(qianfaIndex);

        p.portraitType = (int)PortraitType.ChangeFace;
    }

    /// <summary>
    /// 尝试涨修为
    /// </summary>
    public void TryAddXiuWei()
    {
        ////尝试联网保存一下上次修为增加时间
        //GameTimeManager.Instance.GetServiceTime((x =>
        //{
        //    RoleManager.Instance._CurGameInfo.playerPeople.LastXiuweiAddTime = x;

        //}));
        //TrainSetting setting = DataTable._trainList[RoleManager.Instance._CurGameInfo.playerPeople.CurTrainIndex];
        //RoleManager.Instance._CurGameInfo.playerPeople.CurXiuwei +=(ulong)( setting.xiuWeiAddSpeed.ToFloat());
    }
    /// <summary>
    /// 减修为
    /// </summary>
    public bool DeXiuWei(PeopleData p, int num)
    {
        TrainSetting trainSetting = DataTable._trainList[RoleManager.Instance._CurGameInfo.playerPeople.trainIndex];

        int lastXiuWeiNeed = 0;
        if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex >= 1)
        {
            TrainSetting lastTrainSetting = DataTable._trainList[RoleManager.Instance._CurGameInfo.playerPeople.trainIndex - 1];
            lastXiuWeiNeed = lastTrainSetting.XiuWeiNeed.ToInt32();
        }
        if ((int)(ulong)p.curXiuwei - num < lastXiuWeiNeed)
        {
            PanelManager.Instance.OpenFloatWindow("您的修为不足，无法再减少了");
            return false;
        }
        p.curXiuwei -= (ulong)(ulong)num;
        return true;
        //int totalXiuWei = trainSetting.xiuWeiNeed.ToInt32() - lastXiuWeiNeed;
        //int remainXiuWei = trainSetting.xiuWeiNeed.ToInt32() - (int)(ulong)RoleManager.Instance._CurGameInfo.playerPeople.curXiuwei;
        //int theNum = totalXiuWei / 10;
        //if (theNum > remainXiuWei)
        //    theNum = remainXiuWei;
    }

    /// <summary>
    /// 使用一次突破丹
    /// </summary>
    public void UseBreakDan(PeopleData p, ItemData itemData)
    {
        return;

        // int successRate = GetSuccessRate(p);
        // if (successRate >= 100)
        // {
        //     PanelManager.Instance.OpenFloatWindow("当前成功率为100%，无需服用丹药");
        //     return;
        // }
        // if (p.curEatedDanNum >= 2)
        // {
        //     PanelManager.Instance.OpenFloatWindow("当前丹药服用已达上限");
        //     return;
        // }
        // if (!ItemManager.Instance.CheckIfItemEnough(itemData.settingId,1))
        // {
        //     return;
        // }
        // ItemManager.Instance.LoseItem(itemData.settingId, 1);
        // //消耗该丹药，提高成功率
        // p.curEatedDanNum++;
        // EventCenter.Broadcast(TheEventType.UsedBreakDan);
    }

    /// <summary>
    /// 获取突破成功率
    /// </summary>
    /// <returns></returns>
    public int GetSuccessRate(PeopleData p)
    {
        TrainSetting setting = DataTable._trainList[p.trainIndex];

        int res = 0;
        int baseRate = setting.SuccessRate.ToInt32();
        int addRate = p.curEatedDanNum * 10;
        addRate += p.nextBreakThroughAdd;
        res = baseRate + addRate;
        if (res >= 100)
            res = 100;
        return res;
    }

    ///// <summary>
    ///// 判断是否能训练突破
    ///// </summary>
    ///// <returns></returns>
    //public bool JudgeIfCanTrainBreak()
    //{

    //}

    /// <summary>
    /// 计算战斗力
    /// </summary>
    public long CalcZhanDouLi(PeopleData p = null)
    {
        long res = 0;
        if (p == null)
            p = RoleManager.Instance._CurGameInfo.playerPeople;
        if (p.talent == (int)StudentTalent.LianGong
            || p.isMyTeam)
        {
            float hpNum = GetCurBattleProperty(PropertyIdType.Hp, p).limit;
            float defenceNum = GetCurBattleProperty(PropertyIdType.Defense, p).num;
            float atkNum = GetCurBattleProperty(PropertyIdType.Attack, p).num;
            float critRate = GetCurBattleProperty(PropertyIdType.CritRate, p).num;
            float critNum = GetCurBattleProperty(PropertyIdType.CritNum, p).num;
            float mpSpeed = GetCurBattleProperty(PropertyIdType.MPSpeed, p).num;
            float yuanSuDamageAdd = BattleManager.Instance.GetYuanSuProDamageAdd((YuanSuType)p.yuanSu, p);

            res = (long)(
                atkNum * (1 + critRate * 0.01f * critNum * 0.01f)*(1+yuanSuDamageAdd)
                + hpNum + defenceNum + 
                5 * atkNum *(1+ mpSpeed*0.01) * (1 + yuanSuDamageAdd));//这个5改成技能加成
        }
        else
        {
            if(p.propertyList.Count>0)
            res = p.propertyList[0].num;
            else
            res = 0;
        }

        return res;
        
    }

    /// <summary>
    /// 计算宗门战斗力
    /// </summary>
    public long ZongMenTotalZhanDouLi()
    {
        long res = CalcZhanDouLi(RoleManager.Instance._CurGameInfo.playerPeople);
       for(int i = 0;i< RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            res += CalcZhanDouLi(p);
        }
        return res;

    }




    /// <summary>
    /// 设置脸
    /// </summary>
    public void SetFace(PeopleData p,List<int> indexList)
    {
        p.portraitIndexList.Clear();
   
        for(int i = 0; i < indexList.Count; i++)
        {
            p.portraitIndexList.Add(indexList[i]);

        }


        p.portraitType = (int)PortraitType.ChangeFace;

        SendTouXiangAndKuangInfoToServer();

    }

    /// <summary>
    /// 战斗队
    /// </summary>
    /// <returns></returns>
    public List<PeopleData> FindMyBattleTeamList(bool shenYuan, int index)
    {
        List<PeopleData> myList = new List<PeopleData>();
        if (!shenYuan)
        {
            for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1.Count; i++)
            {
                ulong onlyId = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1[i];
                if (onlyId <= 0)
                    continue;
                if (onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
                    myList.Add(RoleManager.Instance._CurGameInfo.playerPeople);
                else
                    myList.Add(StudentManager.Instance.FindStudent(onlyId));
            }
        }
    
        return myList;
    }


    /// <summary>
    /// 增加一个人的记录
    /// </summary>
    public void AddNotedPeople(PeopleData p,NotedPeopleType notedPeopleType)
    {
        for(int i = 0; i < _CurGameInfo.NotedPeopleData.NotedList.Count; i++)
        {
            PeopleData theP = _CurGameInfo.NotedPeopleData.NotedList[i].P;
            if (theP.onlyId == p.onlyId)
                return;
        }
        SocializationRecordData recordData = new SocializationRecordData();
        recordData.time.Add(RoleManager.Instance._CurGameInfo.timeData.Year);
        recordData.time.Add(RoleManager.Instance._CurGameInfo.timeData.Month);
        recordData.time.Add(RoleManager.Instance._CurGameInfo.timeData.Week);
        string str = "";
        switch (notedPeopleType)
        {
            //叛宗
            case NotedPeopleType.PanZong:
                str = "于" + recordData.time[0]+"年" + recordData.time[1] + "月" + recordData.time[2] + "周叛宗离去";
                break;
        }
        recordData.content = str;
        SingleNotedPeople singleNotedPeople = new SingleNotedPeople();
        singleNotedPeople.Record.Add(recordData);
        singleNotedPeople.NotedType =(int)notedPeopleType;
        singleNotedPeople.P =p;
        _CurGameInfo.NotedPeopleData.NotedList.Add(singleNotedPeople);
    }

    /// <summary>
    /// 玩家允许的元素数量
    /// </summary>
    public int CandidateYuanSuNum()
    {
        if (RoleManager.Instance._CurGameInfo.playerPeople.trainIndex >= 149)
        {
            return 6;
        }
        return GiantLevel(RoleManager.Instance._CurGameInfo.playerPeople);
    }
    /// <summary>
    /// 玩家允许的元素数量
    /// </summary>
    public int CandidateYuanSuNum(int trainIndex)
    {
        if (trainIndex >= 149)
        {
            return 6;
        }
        return  trainIndex / 30 + 1;
    }
    /// <summary>
    /// 解锁元素
    /// </summary>
    /// <param name="p"></param>
    /// <param name="yuanSuType"></param>
    public void UnlockYuanSu(PeopleData p,YuanSuType yuanSuType)
    {
        if (CandidateYuanSuNum() > p.curUnlockedYuanSuList.Count
            && !p.curUnlockedYuanSuList.Contains((int)yuanSuType))
        {
            p.curUnlockedYuanSuList.Add((int)yuanSuType);
            EventCenter.Broadcast(TheEventType.UnlockYuanSu);
        }
    }

    /// <summary>
    /// 改变属性
    /// </summary>
    public void ChangeYuanSu(PeopleData p,YuanSuType yuanSuType)
    {
        if (p.curUnlockedYuanSuList.Contains((int)yuanSuType))
        {
            p.yuanSu = (int)yuanSuType;

            //第一个技能要是该元素的技能
            int puGongId = (int)BattleManager.Instance.PuGongIdByYuanSu(yuanSuType);
            for(int i = 0; i < p.allSkillData.skillList.Count; i++)
            {
                p.allSkillData.skillList[i].isEquipped = false;
            }
      
            p.allSkillData.equippedSkillIdList.Clear();
            p.allSkillData.equippedSkillIdList.Add(puGongId);

            bool havePuGong = false;
            for(int i = 0; i < p.allSkillData.skillList.Count; i++)
            {
                SingleSkillData data = p.allSkillData.skillList[i];
                if (data.skillId == puGongId)
                {
                    havePuGong = true;
                    break;
                }
            }

            if (!havePuGong)
            {
                SingleSkillData data = new SingleSkillData();
                data.skillId = puGongId;
                if (p.curEquipItemList[0] != null && p.curEquipItemList[0].equipProtoData != null)
                {
                    data.skillLevel = p.curEquipItemList[0].equipProtoData.curLevel;
                }
                else
                {
                    data.skillLevel = 1;

                }
                p.allSkillData.skillList.Add(data);
            }

            EventCenter.Broadcast(TheEventType.ChangeYuanSu);
        }
    }

    /// <summary>
    /// 改名
    /// </summary>
    /// <param name="p"></param>
    public void ChangeName(PeopleData p,string newName)
    {
        bool needItem = false;
        if (p.changeNameNum >= 1)
        {
            needItem = true;
        }
        if(needItem)
        {
            if (!ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.ChangePNameCard, 1))
            {
                PanelManager.Instance.OpenFloatWindow("改名卡不够");
                return;
            }
        }
        if (DataTable.IsScreening(newName))
        {
            PanelManager.Instance.OpenFloatWindow("名字包含敏感字\n请重新输入");
            return;
        }
        if (string.IsNullOrWhiteSpace(newName))
        {
            PanelManager.Instance.OpenFloatWindow("名字不能为空");
            return;
        }

        ItemManager.Instance.LoseItem((int)ItemIdType.ChangePNameCard, 1);

        p.changeNameNum++;
        p.name = newName;
        PanelManager.Instance.OpenFloatWindow("修改成功");
 
        EventCenter.Broadcast(TheEventType.ChangeZhangMenName);
        EventCenter.Broadcast(TheEventType.ChangeStudentName);


    }

    /// <summary>
    /// 改头像
    /// </summary>
    public void ChangeTouXiang(ItemSetting setting)
    {
        if (!ItemManager.Instance.CheckIfHaveItemBySettingId(setting.Id.ToInt32()))
        {
            PanelManager.Instance.OpenFloatWindow("未获得该头像");
            return;
        }
        string touXiangId = setting.Id;
        //普通头像
        if ( setting.Id.ToInt32()==(int)ItemIdType.PuTongTouXiang)
        {
            touXiangId = "";
        }

        if (string.IsNullOrWhiteSpace(_CurGameInfo.TouXiang))
        {
            _CurGameInfo.TouXiang = touXiangId + "|";
        }
        else
        {
            string[] arr = _CurGameInfo.TouXiang.Split('|');
            arr[0] = touXiangId;
            _CurGameInfo.TouXiang = arr[0] + "|" + arr[1];
        }
      
        EventCenter.Broadcast(TheEventType.ChangeTouXiang);
        PanelManager.Instance.OpenFloatWindow("修改成功");

        SendTouXiangAndKuangInfoToServer();

    }

    /// <summary>
    /// 改头像框
    /// </summary>
    public void ChangeTouXiangKuang(ItemSetting setting)
    {
        if (!ItemManager.Instance.CheckIfHaveItemBySettingId(setting.Id.ToInt32()))
        {
            PanelManager.Instance.OpenFloatWindow("未获得该头像框");
            return;
        }
        if (string.IsNullOrWhiteSpace(_CurGameInfo.TouXiang))
        {
            _CurGameInfo.TouXiang = "|"+ setting.Id;
        }
        else
        {
            string[] arr = _CurGameInfo.TouXiang.Split('|');
            arr[1] = setting.Id;
            _CurGameInfo.TouXiang = arr[0] + "|" + arr[1];
        }
        EventCenter.Broadcast(TheEventType.ChangeTouXiang);
        PanelManager.Instance.OpenFloatWindow("修改成功");
        SendTouXiangAndKuangInfoToServer();
    }
    /// <summary>
    /// 头像和框的信息送给服务器
    /// </summary>
    public void SendTouXiangAndKuangInfoToServer()
    {
        string touXiang = "";
        string kuang = "";
        //掌门有头像
        if (!string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TouXiang))
        {
            if (!string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TouXiang.Split('|')[0]))
            {
                touXiang =  RoleManager.Instance._CurGameInfo.TouXiang.Split('|')[0];
            }
            if (!string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TouXiang.Split('|')[1]))
            {
                kuang =  RoleManager.Instance._CurGameInfo.TouXiang.Split('|')[1];
            }
           
        }
        List<int> faceIndexList = new List<int>();

        for(int i = 0; i < RoleManager.Instance._CurGameInfo.playerPeople.portraitIndexList.Count; i++)
        {
            faceIndexList.Add(RoleManager.Instance._CurGameInfo.playerPeople.portraitIndexList[i]);
        }
     }
    /// <summary>
    /// 头像路径
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string TouXiangPath(int id)
    {
        ItemSetting setting = DataTable.FindItemSetting(id);
        string[] arr = setting.Param.Split('|');
        if (RoleManager.Instance._CurGameInfo.playerPeople.gender == (int)Gender.Male)
        {
            return arr[0];
        }
        else
        {
            return arr[1];
        }
    }
    public void Clear()
    {
        _CurGameInfo = null;
    }

    /// <summary>
    /// 是否装着该头像
    /// </summary>
    /// <param name="settingId"></param>
    public bool CheckIfHaveTouXiang(int settingId)
    {
        //无头像
        if (string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TouXiang))
        {
            if (settingId == (int)ItemIdType.PuTongTouXiang)
                return true;
            else
                return false;
        }
        else
        {
            string[] arr = _CurGameInfo.TouXiang.Split('|');
            if (arr[0].ToInt32() == settingId
                ||(string.IsNullOrWhiteSpace(arr[0])&&settingId==(int)ItemIdType.PuTongTouXiang))
            {
                return true;
            }
            return false;
         }
    }
    /// <summary>
    /// 是否装着该头像
    /// </summary>
    /// <param name="settingId"></param>
    public bool CheckIfHaveTouXiangKuang(int settingId)
    {
        //无头像框
        if (string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TouXiang))
        {
            if (settingId == (int)ItemIdType.PuTongTouXiangKuang)
                return true;
            else
                return false;
        }
        else
        {
            string[] arr = _CurGameInfo.TouXiang.Split('|');
            if (arr[1].ToInt32() == settingId)
            {
                return true;
            }
            return false;
        }
    }
}

/// <summary>
/// 记录的人
/// </summary>
public enum NotedPeopleType
{
    None=0,
    PanZong=1,//叛宗者
}

public enum Gender
{
    None=0,
    Male=1,
    Female=2,
}

public enum BuffIdType
{
    None=0,
    ZhongShang=10001,//重伤
}

///// <summary>
///// 敌人类型
///// </summary>
//public enum EnemyType
//{
//    None=0,
//    ZhaoCha=1,//找茬
//    Match=2,//比赛
//    //TaskAppear=3,//任务才出
//    MiJing=4,//秘境守卫
//    Main=5,//主线
//}

/// <summary>
/// 属性类型
/// </summary>
public enum PropertyType
{
    None=0,
    LianQi=3,//炼器
}
/// <summary>
/// 是否换脸
/// </summary>
public enum PortraitType
{
    None=0,
    ChangeFace=1,//换脸
}

/// <summary>
/// 解锁状态
/// </summary>
public enum UnlockType
{
    None=0,
    UnShow,//不显示
    Locked,//锁定
    UnLocked,//解锁
}

/// <summary>
/// 血脉类型
/// </summary>
public enum XueMaiType
{
    None=0,
    Xue=1,//血
    Fang=2,//防
    Gong=3,//攻
    JingTong=4,//精通
    ShangHai=5,//所有伤害
    End=6,//终点
}
