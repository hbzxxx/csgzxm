 using Framework.Data;
 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;

public class MapManager : CommonInstance<MapManager>
{

    //public int curChoosedMapId;
    public int curMapIndex=1;//当前在地图的哪个index

    public string curChoosedLevelId;//当前选择的关卡id

    public ulong  curBattledStudentOnlyId;//当前打过的弟子
    public int curBattledStudentBeforeExp;//当前打过的弟子升级前的经验

    public int curChoosedExploreId;//当前选择的探险id


    /// <summary>
    /// 找可扫荡的裂隙关
    /// </summary>
    /// <returns></returns>
    public SingleLevelData FindCanSaoDangLieXiLevel()
    {
        SingleLevelData choosedLevel = null;
        SingleMapData singleMapData = MapManager.Instance.FindMapById(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
        //全通过后可以扫荡
        bool passedMap = true;
        for (int i = 0; i < singleMapData.LevelList.Count; i++)
        {
            SingleLevelData singleLevelData = singleMapData.LevelList[i];
            LevelSetting levelSetting = DataTable.FindLevelSetting(singleLevelData.LevelId);

            //如果是首张图 
            if (levelSetting.IsFirst == "1"&&singleLevelData.HaveAccomplished)
            {
                choosedLevel = singleLevelData;
            }
            if (!singleLevelData.HaveAccomplished)
            {
                passedMap = false;
            }
        }
        if (passedMap)
        {
            if (choosedLevel != null)
            {
                while (choosedLevel.HaveAccomplished)
                {
                    LevelSetting levelSetting = DataTable.FindLevelSetting(choosedLevel.LevelId);

                    //当前未完成 可以扫
                    if (choosedLevel.LevelStatus != (int)AccomplishStatus.Accomplished
                        && choosedLevel.HaveAccomplished)
                    {
                        break;
                    }
                    //当前是最后一关
                    else if (levelSetting.IsEndLevel == "1")
                    {
                        choosedLevel = null;
                        break;
                    }
                    //当前已完成，往下找
                    else
                    {
                        LevelSetting setting = DataTable.FindLevelSetting(choosedLevel.LevelId);

                        if (!string.IsNullOrWhiteSpace(setting.NextLevel))
                        {
                            choosedLevel = FindLevelById(setting.NextLevel);
                        }

                    }

                }
            }
            if (choosedLevel != null)
            {
                if (!choosedLevel.HaveAccomplished || choosedLevel.LevelStatus != (int)AccomplishStatus.UnAccomplished)
                {
                    choosedLevel = null;

                }
            }
        }
        else
        {
            choosedLevel = null;
        }

 
        return choosedLevel;
    }

    /// <summary>
    /// 初始化该地图所有固定关卡
    /// </summary>
    public void InitAllFixedLevel(int MapId)
    {
        SingleMapData map = FindMapById(MapId);

        // 先重置所有关卡状态：已通关的保持，未通关的根据是否首关设置状态
        for (int i = 0; i < map.FixedLevelList.Count; i++)
        {
            SingleLevelData singleLevelData = map.FixedLevelList[i];
            LevelSetting levelSetting = DataTable.FindLevelSetting(singleLevelData.LevelId);

            // 只处理未通关的关卡
            if (singleLevelData.LevelStatus != (int)AccomplishStatus.Accomplished)
            {
                // 首关解锁，非首关锁定
                if (levelSetting.IsFirst == "1")
                {
                    singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
                }
                else
                {
                    // 检查前置关卡是否已通关
                    if (!string.IsNullOrWhiteSpace(levelSetting.ForeLevel))
                    {
                        SingleLevelData foreLevel = FindFixedLevelById(levelSetting.ForeLevel);
                        if (foreLevel != null && foreLevel.LevelStatus == (int)AccomplishStatus.Accomplished)
                        {
                            singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
                        }
                        else
                        {
                            singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                        }
                    }
                    else
                    {
                        singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                    }
                }
            }

            BattleManager.Instance.GenerateMainLevelEnemy(singleLevelData);
        }
    }

    /// <summary>
    /// 接着上次没打过的从中转站打起
    /// </summary>
    /// <param name="MapId"></param>
    public void InitAllLieXiLevel(int MapId)
    {
        SingleMapData map = FindMapById(MapId);
        SingleLevelData choosedLevel = null;
        bool allAccomplished = true;

        // 先重置所有关卡状态：已通关的保持，未通关的根据是否首关设置状态
        for (int i = 0; i < map.LevelList.Count; i++)
        {
            SingleLevelData singleLevelData = map.LevelList[i];
            LevelSetting levelSetting = DataTable.FindLevelSetting(singleLevelData.LevelId);

            // 只处理未通关的关卡
            if (singleLevelData.LevelStatus != (int)AccomplishStatus.Accomplished)
            {
                // 首关解锁，非首关锁定
                if (levelSetting.IsFirst == "1")
                {
                    singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
                }
                else
                {
                    // 检查前置关卡是否已通关
                    if (!string.IsNullOrWhiteSpace(levelSetting.ForeLevel))
                    {
                        SingleLevelData foreLevel = FindLevelById(levelSetting.ForeLevel);
                        if (foreLevel != null && foreLevel.LevelStatus == (int)AccomplishStatus.Accomplished)
                        {
                            singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
                        }
                        else
                        {
                            singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                        }
                    }
                    else
                    {
                        singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                    }
                }
            }

            if (levelSetting.IsEndLevel == "1")
            {
                choosedLevel = singleLevelData;
            }
            if (singleLevelData.LevelStatus != (int)AccomplishStatus.Accomplished)
            {
                allAccomplished = false;
            }
        }
        if (!allAccomplished)
        {
            bool find = false;
            while (!find)
            {
                LevelSetting setting = DataTable.FindLevelSetting(choosedLevel.LevelId);
                if (setting.IsFirst == "1")
                {
                    BattleManager.Instance.GenerateMainLevelEnemy(choosedLevel);
                    choosedLevel.LevelStatus = (int)AccomplishStatus.UnAccomplished;
                    find = true;
                }
                //找到起点 只好从起点开始
                else
                {
                    //如果该关卡未通过关
                    if (choosedLevel.LevelStatus != (int)AccomplishStatus.Accomplished)
                    {
                        //if (setting.leveltype.ToInt32() != (int)LevelType.ZhongZhuanZhan
                        //    && choosedLevel.Enemy.Count == 0)
                        //{
                        //    BattleManager.Instance.GenerateMainLevelEnemy(choosedLevel);

                        //}
                     
                            BattleManager.Instance.GenerateMainLevelEnemy(choosedLevel);

                        
                        choosedLevel = FindLevelById(setting.ForeLevel);
                   
                    }
                    //该关卡通过关
                    else
                    {
                        //如果正在找中转站

                        if (setting.Leveltype.ToInt32() == (int)LevelType.ZhongZhuanZhan)
                        {
                            if (choosedLevel.HaveAccomplished)
                            {

                            }
                            find = true;
                            //下一关unlock
                            if (!string.IsNullOrWhiteSpace(setting.NextLevel))
                            {
                                FindLevelById(setting.NextLevel).LevelStatus = (int)AccomplishStatus.UnAccomplished;
                            }
                        }
                        else
                        {
                            choosedLevel.LevelStatus = (int)AccomplishStatus.Locked;
                            BattleManager.Instance.GenerateMainLevelEnemy(choosedLevel);

                        }
 
                    }
                }

            }
            curChoosedLevelId = choosedLevel.LevelId;
        }
        else
        {
            InitAllLevel(MapId);
        }



       // BattleManager.Instance.GenerateMainLevelEnemy(singleLevelData);
    }

    /// <summary>
    /// 初始化该地图所有关卡
    /// </summary>
    public void InitAllLevel(int MapId)
    {
        SingleMapData map = FindMapById(MapId);
        //奖励先清空
        map.CurAwardList.Clear();
        MapSetting mapSetting = DataTable.FindMapSetting(MapId);

        for(int i = 0; i < map.LevelList.Count; i++)
        {
            SingleLevelData singleLevelData = map.LevelList[i];
            LevelSetting levelSetting = DataTable.FindLevelSetting(singleLevelData.LevelId);

            //地图未完成，除了首关都锁上
            //if (map.MapStatus != (int)AccomplishStatus.Accomplished)
            //{

                //如果是首张图 则解锁
                if (levelSetting.IsFirst == "1")
                {
                    singleLevelData.LevelStatus = (int)AccomplishStatus.UnAccomplished;
                curChoosedLevelId = singleLevelData.LevelId;
                }
                else
                {
                    singleLevelData.LevelStatus = (int)AccomplishStatus.Locked;
                }

            //}
            BattleManager.Instance.GenerateMainLevelEnemy(singleLevelData);
        }
    }

    /// <summary>
    /// 完成固定关卡
    /// </summary>
    public void AccomplishFixedLevel(string levelId)
    {
        //解锁关卡
        SingleLevelData level = FindFixedLevelById(levelId);
        level.LevelStatus = (int)AccomplishStatus.Accomplished;

        LevelSetting levelSetting = DataTable.FindLevelSetting(levelId);


        TaskManager.Instance.GetAchievement(AchievementType.PassedMaxLevel, levelSetting.Level);

        int mapId = levelSetting.BelongMap.ToInt32();
        MapSetting mapSetting = DataTable.FindMapSetting(mapId);
        if (!string.IsNullOrWhiteSpace(levelSetting.NextLevel))
        {
            string nextLevelId = levelSetting.NextLevel;
            //下一关解锁
            if (FindFixedLevelById(nextLevelId).LevelStatus == (int)AccomplishStatus.Locked)
                FindFixedLevelById(nextLevelId).LevelStatus = (int)AccomplishStatus.UnAccomplished;
        }
        //没有下一关，判断是否最后一关
        else
        {
            if (levelSetting.IsEndLevel == "1")
            {
                SingleMapData map = FindMapById(mapId);
                map.MapStatus = (int)AccomplishStatus.Accomplished;

                string mapLevel = DataTable.FindMapSetting(map.MapId).MapLevel;
                TaskManager.Instance.GetAchievement(AchievementType.PassedMaxMap, mapLevel);

                map.LieXiMapStatus = (int)AccomplishStatus.UnAccomplished;

                //TalkingDataGA.OnEvent("通关世界", new Dictionary<string, object>() { { mapSetting.name, 1 } });
                TDGAMission.OnCompleted(mapSetting.Name);

            }
        }
        TDGAMission.OnCompleted(levelSetting.Level);
        TaskManager.Instance.TryAccomplishGuideBook(TaskType.PassLevel);

        EventCenter.Broadcast(TheEventType.RefreshLevelShow);
    }

    /// <summary>
    /// 完成裂隙关卡
    /// </summary>
    public void AccomplishLevel(string levelId)
    {
        //解锁关卡
        SingleLevelData level = FindLevelById(levelId);
        level.LevelStatus = (int)AccomplishStatus.Accomplished;
        level.HaveAccomplished = true;
        LevelSetting levelSetting = DataTable.FindLevelSetting(levelId);
        int mapId = levelSetting.BelongMap.ToInt32();
        MapSetting mapSetting = DataTable.FindMapSetting(mapId);
        TaskManager.Instance.GetAchievement(AchievementType.PassedMaxLieXiLevel,mapSetting.MapLevel+"-"+ levelSetting.Level);

        if (!string.IsNullOrWhiteSpace(levelSetting.NextLevel))
        {
            string nextLevelId = levelSetting.NextLevel;
            //下一关解锁
            if (FindLevelById(nextLevelId).LevelStatus==(int)AccomplishStatus.Locked)
                FindLevelById(nextLevelId).LevelStatus = (int)AccomplishStatus.UnAccomplished;
        }
        //没有下一关，判断是否最后一关
        else
        {
            if (levelSetting.IsEndLevel == "1")
            {
                //完成地图
                SingleMapData map = FindMapById(mapId);
                map.LieXiMapStatus = (int)AccomplishStatus.Accomplished;
 
                //如果有下个地图
                if (!string.IsNullOrWhiteSpace(mapSetting.NextMap))
                {

                    MapSetting nextMapSetting = DataTable.FindMapSetting(mapSetting.NextMap.ToInt32());

                    int nextMapStatus = FindMapById(nextMapSetting.Id.ToInt32()).MapStatus;
                    if (nextMapStatus == (int)AccomplishStatus.Locked)
                    {

                        TDGAMission.OnBegin(nextMapSetting.Name);

                        FindMapById(nextMapSetting.Id.ToInt32()).MapStatus = (int)AccomplishStatus.UnAccomplished;

                        PanelManager.Instance.OpenFloatWindow("解锁了新的世界");
                        //解锁下一关野怪
                        //MapEventManager.Instance.UnlockMapWaitToAppearList(nextMapSetting.MapLevel.ToInt32());
                        //解锁下一关秘境
                        MapManager.Instance.UnlockExploreMap(nextMapSetting.Id.ToInt32());
                        //解锁讨伐
                        if (map.MapId == 10000)
                        {
                            PanelManager.Instance.curYieldShowInMainPanelType = YieldShowInMainPanelType.ShowTaoFaUnlock;
                        }
                    }

  
                    //TalkingDataGA.OnEvent("通关裂隙", new Dictionary<string, object>() { { mapSetting.name, 1 } });
 

                }

            }

        }
        TaskManager.Instance.TryAccomplishGuideBook(TaskType.PassLevel);
        TaskManager.Instance.TryAccomplishGuideBook(TaskType.PassedMaxLieXiLevel);
        TaskManager.Instance.GetDailyAchievement(TaskType.PassedMaxLieXiLevel,"1");

        TDGAMission.OnCompleted(levelSetting.Level);
        EventCenter.Broadcast(TheEventType.RefreshLevelShow);
    }

 
    /// <summary>
    /// 进入主线图
    /// </summary>
    /// <param name="mapId"></param>
    public void EnterFixedMap(int mapId)
    {
        RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId = mapId;
        GameSceneManager.Instance.GoToScene(SceneType.FixedMap);

    }
    /// <summary>
    /// 查看是否解锁该固定关卡条件
    /// </summary>
    /// <returns></returns>
    public bool CheckIfUnlockFixedLevel(string levelId)
    {
        for (int i = 0; i < DataTable._levelList.Count; i++)
        {
            LevelSetting levelSetting = DataTable._levelList[i];
            if (levelSetting.Id == levelId)
            {
                int mapId = levelSetting.BelongMap.ToInt32();
                //如果是首张图 则解锁
                if (levelSetting.IsFirst == "1")
                {
                    return true;
                }
                else
                {
                    string foreLevelId = levelSetting.ForeLevel;
                    if (FindFixedLevelById(mapId, foreLevelId).LevelStatus == (int)AccomplishStatus.Accomplished)
                    {
                        return true;
                    }

                }
            }
        }
        return false;
    }
    /// <summary>
    /// 查看是否解锁该关卡条件
    /// </summary>
    /// <returns></returns>
    public bool CheckIfUnlockLevel( string levelId)
    {
        for (int i = 0; i < DataTable._levelList.Count; i++)
        {
            LevelSetting levelSetting = DataTable._levelList[i];
            if (levelSetting.Id == levelId)
            {
                int mapId = levelSetting.BelongMap.ToInt32();
                //如果是首张图 则解锁
                if (levelSetting.IsFirst == "1")
                {
                    return true;
                }
                else
                {
                    string foreLevelId = levelSetting.ForeLevel;
                    if (FindLevelById(mapId, foreLevelId).LevelStatus == (int)AccomplishStatus.Accomplished)
                    {
                        return true;
                    }

                }
            }
        }
        return false;
    }

    /// <summary>
    /// 查看是否解锁该固定关卡条件
    /// </summary>
    /// <returns></returns>
    public bool CheckIfUnlockFixedLevel(int mapId, string levelId)
    {
        for (int i = 0; i < DataTable._levelList.Count; i++)
        {
            LevelSetting levelSetting = DataTable._levelList[i];
            if (levelSetting.Id == levelId)
            {
                //如果是首张图 则解锁
                if (levelSetting.IsFirst == "1")
                {
                    return true;
                }
                else
                {
                    string foreLevelId = levelSetting.ForeLevel;

                    if (FindFixedLevelById(mapId, foreLevelId).LevelStatus == (int)AccomplishStatus.Accomplished)
                    {
                        return true;
                    }

                }
            }
        }
        return false;
    }
    /// <summary>
    /// 查看是否解锁该关卡条件
    /// </summary>
    /// <returns></returns>
    public bool CheckIfUnlockLevel(int mapId, string levelId)
    {
        for (int i = 0; i < DataTable._levelList.Count; i++)
        {
            LevelSetting levelSetting = DataTable._levelList[i];
            if (levelSetting.Id == levelId)
            {
                //如果是首张图 则解锁
                if (levelSetting.IsFirst == "1")
                {
                    return true;
                }
                else
                {
                    string foreLevelId = levelSetting.ForeLevel;
                 
                    if (FindLevelById(mapId, foreLevelId).LevelStatus == (int)AccomplishStatus.Accomplished)
                    {
                        return true;
                    }

                }
            }
        }
        return false;
    }

    /// <summary>
    /// 查看是否解锁该地图条件
    /// </summary>
    /// <returns></returns>
    public bool CheckIfUnlockMap(int mapId)
    {
        for(int i = 0; i < DataTable._mapList.Count; i++)
        {
            MapSetting mapSetting = DataTable._mapList[i];
            if (mapSetting.Id.ToInt32() == mapId)
            {
                //如果是首张图 则解锁
                if (mapSetting.IsFirstMap == "1")
                {
                    return true;
                }
                else
                {
                    int foreMapId = mapSetting.ForeMap.ToInt32();
                    if (FindMapById(foreMapId).LieXiMapStatus == (int)AccomplishStatus.Accomplished)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    } 
    /// <summary>
      /// 通过id找固定关卡
      /// </summary>
      /// <param name="mapId"></param>
      /// <param name="id"></param>
      /// <returns></returns>
    public SingleLevelData FindFixedLevelById(string id)
    {
        LevelSetting levelSetting = DataTable.FindLevelSetting(id);
        int mapId = levelSetting.BelongMap.ToInt32();

        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllMapData.MapList.Count; i++)
        {
            SingleMapData map = RoleManager.Instance._CurGameInfo.AllMapData.MapList[i];
            if (mapId == map.MapId)
            {
                for (int j = 0; j < map.FixedLevelList.Count; j++)
                {
                    SingleLevelData level = map.FixedLevelList[j];
                    if (level.LevelId == id)
                    {
                        return level;
                    }
                }
            }
        }
        return null;
    }
    /// <summary>
    /// 通过id找关卡
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public SingleLevelData FindLevelById( string id)
    {
        LevelSetting levelSetting = DataTable.FindLevelSetting(id);
        int mapId = levelSetting.BelongMap.ToInt32();

        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllMapData.MapList.Count; i++)
        {
            SingleMapData map = RoleManager.Instance._CurGameInfo.AllMapData.MapList[i];
            if (mapId == map.MapId)
            {
                for (int j = 0; j < map.LevelList.Count; j++)
                {
                    SingleLevelData level = map.LevelList[j];
                    if (level.LevelId == id)
                    {
                        return level;
                    }
                }
            }
        }
        return null;
    }
    /// <summary>
    /// 通过id找固定关卡
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public SingleLevelData FindFixedLevelById(int mapId, string id)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllMapData.MapList.Count; i++)
        {
            SingleMapData map = RoleManager.Instance._CurGameInfo.AllMapData.MapList[i];
            if (mapId == map.MapId)
            {
                for (int j = 0; j < map.FixedLevelList.Count; j++)
                {
                    SingleLevelData level = map.FixedLevelList[j];
                    if (level.LevelId == id)
                    {
                        return level;
                    }
                }
            }
        }
        return null;
    }
    /// <summary>
    /// 通过id找关卡
    /// </summary>
    /// <param name="mapId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public SingleLevelData FindLevelById(int mapId, string id)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllMapData.MapList.Count; i++)
        {
            SingleMapData map = RoleManager.Instance._CurGameInfo.AllMapData.MapList[i];
            if (mapId == map.MapId)
            {
                for(int j = 0; j < map.LevelList.Count; j++)
                {
                    SingleLevelData level = map.LevelList[j];
                    if (level.LevelId == id)
                    {
                        return level;
                    }
                }
            }
        }
        return null;
    }

    public SingleMapData FindMapById(int id)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllMapData.MapList.Count; i++)
        {
            if (id == RoleManager.Instance._CurGameInfo.AllMapData.MapList[i].MapId)
                return RoleManager.Instance._CurGameInfo.AllMapData.MapList[i];
        }
        return null;
    }

    /// <summary>
    /// 找是否有某个id的图
    /// </summary>
    /// <returns></returns>
    public bool CheckIfHaveIdMap(int id)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllMapData.MapList.Count; i++)
        {
            if (id == RoleManager.Instance._CurGameInfo.AllMapData.MapList[i].MapId)
                return true;
        }
        return false;
    }
    /// <summary>
    /// 找是否有某个id的关卡
    /// </summary>
    /// <returns></returns>
    public bool CheckIfHaveIdLevel(int mapId, string id)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllMapData.MapList.Count; i++)
        {
            SingleMapData mapData = RoleManager.Instance._CurGameInfo.AllMapData.MapList[i];
            if (mapData.MapId == mapId)
            {
                for(int j = 0; j < mapData.LevelList.Count; j++)
                {
                    SingleLevelData level = mapData.LevelList[j];
                    if (level.LevelId == id)
                        return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 找是否有某个id的固定关卡
    /// </summary>
    /// <returns></returns>
    public bool CheckIfHaveFixedIdLevel(int mapId, string id)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllMapData.MapList.Count; i++)
        {
            SingleMapData mapData = RoleManager.Instance._CurGameInfo.AllMapData.MapList[i];
            if (mapData.MapId == mapId)
            {
                for (int j = 0; j < mapData.FixedLevelList.Count; j++)
                {
                    SingleLevelData level = mapData.FixedLevelList[j];
                    if (level.LevelId == id)
                        return true;
                }
            }
        }
        return false;
    }

 

    /// <summary>
    /// 找可以前往秘境探险的弟子
    /// </summary>
    public List<PeopleData> CheckIfStudentCanExplore()
    {
        List<PeopleData> res = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.talent == (int)StudentTalent.LianGong
                && p.studentStatusType == (int)StudentStatusType.None
                && !p.seriousInjury)
            {
                res.Add(p);

            }
        }
        return res;
    }
    /// <summary>
    /// 找可以前往主线的弟子
    /// </summary>
    public List<PeopleData> CheckIfStudentCanUpMainWorld()
    {
        List<PeopleData> res = new List<PeopleData>();
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count; i++)
        {
            PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
            if (p.talent == (int)StudentTalent.LianGong
                &&p.studentStatusType==(int)StudentStatusType.None
                &&!p.seriousInjury)
            {
                res.Add(p);

            }
            //if (p.IsPaiQian)
            //{
            //    continue;
            //}
            //if (p.SeriousInjury)
            //{
            //    continue;
            //}
            //if (p.IsExplore)
            //{
            //    continue;
            //}
        }
        return res;
    }
 


    /// <summary>
    /// 离开固定地图
    /// </summary>
    public void LeaveFixedMap()
    {
        LogicLeaveFixedMap();
        GameSceneManager.Instance.GoToScene(SceneType.Mountain);

    }

    /// <summary>
    /// 逻辑上离开固定地图
    /// </summary>
    public void LogicLeaveFixedMap()
    {    
        RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId = 0;
    }

    /// <summary>
    /// 离开仙门地图
    /// </summary>
    public void LeaveMap()
    {
        SingleMapData singleMapData = MapManager.Instance.FindMapById(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
        MapSetting mapSetting = DataTable.FindMapSetting(singleMapData.MapId);
        for (int i = 0; i < singleMapData.CurAwardList.Count; i++)
        {
             //if (ad)
             //   singleMapData.CurAwardList[i].Count *= 2;
            ItemManager.Instance.GetItem(singleMapData.CurAwardList[i].settingId, singleMapData.CurAwardList[i].count);
        }

        //角色不重伤
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1[i];
            if (onlyId <= 0)
                continue;
            PeopleData handleP = null;
            if (onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
                handleP = RoleManager.Instance._CurGameInfo.playerPeople;
            else
                handleP = StudentManager.Instance.FindStudent(onlyId);

            RoleManager.Instance.FullHp(handleP);
        }
        RoleManager.Instance._CurGameInfo.XianMenOpen = false;

        //时间走
        GameTimeManager.Instance.StartMove();
        //时间过一天
        GameTimeManager.Instance.DayPlus();
        GameSceneManager.Instance.GoToScene(SceneType.Mountain);
        //获得物品弹窗
        for (int i = 0; i < singleMapData.CurAwardList.Count; i++)
        {
            PanelManager.Instance.AddTongZhi(TongZhiType.Consume, "", ConsumeType.Item, (int)singleMapData.CurAwardList[i].settingId, (int)(int)(ulong)singleMapData.CurAwardList[i].count);

        }
        singleMapData.CurAwardList.Clear();
    }

    /// <summary>
    /// 显示地图结算 然后退出地图
    /// </summary>
    public void MapResult()
    {
        List<ItemData> res = new List<ItemData>();
        SingleMapData singleMapData = MapManager.Instance.FindMapById(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
        for(int i = 0; i < singleMapData.CurAwardList.Count; i++)
        {
            res.Add(singleMapData.CurAwardList[i]);
        }
        res = ItemManager.Instance.CombineItemList(res);
        Action LeaveMapAction = LeaveMap;
        PanelManager.Instance.OpenPanel<GetAwardWithCloseActionPanel>(PanelManager.Instance.trans_layer2, res, LeaveMapAction);

     }

    /// <summary>
    /// 仙门大开
    /// </summary>
    public void XianMenOpen()
    {
        RoleManager.Instance._CurGameInfo.XianMenOpen = true;
        EventCenter.Broadcast(TheEventType.XianMenOpen);
    }


    /// <summary>
    /// 中转站休息，回复所有血量
    /// </summary>
    public void ZhongZhuanZhanRest(string levelId)
    {
        List<PeopleData> team = RoleManager.Instance.FindMyBattleTeamList(false, 0);
        for(int i = 0; i < team.Count; i++)
        {
            RoleManager.Instance.FullHp(team[i]);
            RoleManager.Instance.FullMP(team[i]);

        }
        //RoleManager.Instance.FullHp(RoleManager.Instance._CurGameInfo.playerPeople);
        //for(int i = 0; i < RoleManager.Instance._CurGameInfo.studentData.allStudentList.Count;i++)
        //{
        //    PeopleData p = RoleManager.Instance._CurGameInfo.studentData.allStudentList[i];
        //    if (p.talent == (int)StudentTalent.LianGong)
        //    {
        //        RoleManager.Instance.FullHp(p);

        //    }
        //}
        AccomplishLevel(levelId);
    }

    /// <summary>
    /// 找所有探索地图事件
    /// </summary>
    public List<SingleMapEventData> FindAllExploreMapEventById(int id)
    {
        List<SingleMapEventData> mapEventList = new List<SingleMapEventData>();
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Count; i++)
        {
            SingleMapEventData data = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList[i];
            if (data.ExploreSettingId == id)
            {
                mapEventList.Add(data);

            }
        }
        return mapEventList;
    }

    /// <summary>
    /// 通过id找探险数据
    /// </summary>
    /// <returns></returns>
    public SingleExploreData FindSingleExploreDataById(int settingId)
    {
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList.Count; i++)
        {
            SingleExploreData data = RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList[i];
            if (data.SettingId == settingId)
            {
                return data;

            }
        }
        return null;
    }

    /// <summary>
    /// 解锁探索地图
    /// </summary>
    public void UnlockExploreMap(int settingId)
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList.Count; i++)
        {
            SingleExploreData data = RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList[i];
            if (data.SettingId == settingId)
            {
                data.Unlocked = true;

            }
        }
    }

    /// <summary>
    /// 进入新秘境探险
    /// </summary>
    /// <param name="exploreId"></param>
    public void EnterNewExplore(int exploreId,List<ulong> studentList)
    {

        //生成新事件
        ExploreMapSetting exploreMapSetting = DataTable.FindExploreMapSetting(exploreId);

        int hangNum = exploreMapSetting.HangNum.ToInt32();
        int lieNum = exploreMapSetting.LieNum.ToInt32();
        
     
        List<int> eventTypeList = CommonUtil.SplitCfgOneDepth(exploreMapSetting.PossibleEventId);
        List<int> eventWeightList = CommonUtil.SplitCfgOneDepth(exploreMapSetting.PossibleEventWeight);
        List<int> test = new List<int>();
        int allEventNum = 0;
        for (int i=1;i< hangNum; i++)
        {
            int generateNum = RandomManager.Next(1, lieNum);
            List<int> candidateLieIndexList = new List<int>();
            for (int j = 0; j < lieNum; j++)
            {
                candidateLieIndexList.Add(j);
            }
            for (int j = 0; j < generateNum; j++)
            {
                if (candidateLieIndexList.Count <= 0)
                    continue;
                //列的位置
                int lieIndex = RandomManager.Next(0, candidateLieIndexList.Count);
                //生成
                int eventIndex= CommonUtil.GetIndexByWeight(eventWeightList);
                int eventId = eventTypeList[eventIndex];
                int totalIndex = i * lieNum + candidateLieIndexList[lieIndex];
                test.Add(totalIndex);
                candidateLieIndexList.RemoveAt(lieIndex);
                SingleMapEventData data= MapEventManager.Instance.AddMapEventAtIndex(eventId, SceneType.MiJingExplore, totalIndex);
                data.ExploreSettingId = exploreId;
                data.IsHide = true;
                allEventNum++;
            }

        }
        //探险队数据
        SingleExploreData singleExploreData = FindSingleExploreDataById(exploreId);
        singleExploreData.ExploreTeamData = new ExploreTeamData();
        singleExploreData.ExploreTeamData.OnlyId = ConstantVal.SetId;
        singleExploreData.AllEventNum = allEventNum;
        for (int i=0;i< studentList.Count; i++)
        {
            PeopleData p = StudentManager.Instance.FindStudent(studentList[i]);
            p.curAtExploreId = exploreId;
            p.studentStatusType = (int)StudentStatusType.AtExplore;
            singleExploreData.ExploreTeamData.StudentOnlyIdList.Add(studentList[i]);
        }
        singleExploreData.ExploreTeamData.RemainBuJi = 100;
        singleExploreData.ExploreTeamData.LogicPos.Add(0);
        singleExploreData.ExploreTeamData.LogicPos.Add(0);
        singleExploreData.ExploreTeamData.LogicPosIndex = 0;
        singleExploreData.ExploreTeamData.Status = (int)ExploreTeamStatus.Idling;
        singleExploreData.ExploreTeamData.ExploreId = singleExploreData.SettingId;
        GameSceneManager.Instance.GoToScene(SceneType.MiJingExplore, true);
 
    }


    /// <summary>
    /// 获取探险需要的剩余时间
    /// </summary>
    /// <returns></returns>
    public int GetExploreRemainDayNum(SingleExploreData curExploreData, SingleMapEventData singleMapEventData)
    {
        Vector2Int logicPos1 = new Vector2Int(curExploreData.ExploreTeamData.LogicPos[0], curExploreData.ExploreTeamData.LogicPos[1]);
        Vector2Int logicPos2 = new Vector2Int(singleMapEventData.LogicPos[0],singleMapEventData.LogicPos[1]);
        int distance = GetDistanceByLogicPos(logicPos1, logicPos2);
        return distance * ConstantVal.exploreSpeed;
    }

    /// <summary>
    /// 通过逻辑坐标得到距离
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public int GetDistanceByLogicPos(Vector2Int pos1,Vector2Int pos2)
    {
        int x = Mathf.Abs(pos1.x - pos2.x);
        int y = Mathf.Abs(pos1.y - pos2.y);
        return x + y;
    }


    /// <summary>
    /// 前往秘境探险
    /// </summary>
    public void OnGoMiJingExplore(SingleExploreData curExploreData, SingleMapEventData singleMapEventData)
    {
        if (curExploreData.ExploreTeamData.Status == (int)ExploreTeamStatus.Idling
            && curExploreData.ExploreTeamData.LogicPos != singleMapEventData.LogicPos)
        {
            MapEventSetting setting = DataTable.FindMapEventSetting(singleMapEventData.SettingId);

            int logicPosCount = GetDistanceByLogicPos(new Vector2Int(curExploreData.ExploreTeamData.LogicPos[0], curExploreData.ExploreTeamData.LogicPos[1]),
               new Vector2Int(singleMapEventData.LogicPos[0], singleMapEventData.LogicPos[1]));
            int consume = logicPosCount * 2;
            if (curExploreData.ExploreTeamData.RemainBuJi < consume)
            {
                PanelManager.Instance.OpenFloatWindow("体力不够！");
                return;

               
            }
            if (curExploreData.ExploreTeamData.TargetEventOnlyId != 0)
            {
                SingleMapEventData eventData = MapEventManager.Instance.FindMapEventDataByOnlyId(curExploreData.ExploreTeamData.TargetEventOnlyId);
                eventData.ExploreTeamOnlyId = 0;
                curExploreData.ExploreTeamData.TargetEventOnlyId = 0;
            }
            DeExploreEnergy(curExploreData.ExploreTeamData,consume);

            curExploreData.ExploreTeamData.Status = (int)ExploreTeamStatus.Moving;
            curExploreData.ExploreTeamData.TargetEventOnlyId = singleMapEventData.OnlyId;
            curExploreData.ExploreTeamData.RemainDay = GetExploreRemainDayNum(curExploreData,singleMapEventData);// logicPosCount * 30;//剩余天
            curExploreData.ExploreTeamData.TotalDay = curExploreData.ExploreTeamData.RemainDay;
            curExploreData.ExploreTeamData.LocalPosBeforeMove.Clear();
            curExploreData.ExploreTeamData.LocalPosBeforeMove.Add(curExploreData.ExploreTeamData.Pos[0]);
            curExploreData.ExploreTeamData.LocalPosBeforeMove.Add(curExploreData.ExploreTeamData.Pos[1]);

            EventCenter.Broadcast(TheEventType.GoMiJingPointExplore, curExploreData);
        }
    }
    /// <summary>
    /// 增加秘境探险精力
    /// </summary>
    public void AddExploreEnergy(ExploreTeamData exploreTeamData, int consume)
    {
        exploreTeamData.RemainBuJi += consume;
        if (exploreTeamData.RemainBuJi >= 100)
        {
            exploreTeamData.RemainBuJi = 100;
        }
        EventCenter.Broadcast(TheEventType.RefreshExploreBuJiShow, exploreTeamData);
    }
    /// <summary>
    /// 减少秘境探险精力
    /// </summary>
    public void DeExploreEnergy(ExploreTeamData exploreTeamData,int consume)
    {
        exploreTeamData.RemainBuJi -= consume;
        if (exploreTeamData.RemainBuJi <= 0)
        {
            exploreTeamData.RemainBuJi = 0;
        }
        EventCenter.Broadcast(TheEventType.RefreshExploreBuJiShow, exploreTeamData);
    }

    /// <summary>
    /// 时间走 探险队走
    /// </summary>
    public void OnExploreTeamMoving()
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList.Count; i++)
        {
            SingleExploreData singleExploreData = RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList[i];
            
            if (singleExploreData.ExploreTeamData!=null)
            {
                if (singleExploreData.ExploreTeamData.Status == (int)ExploreTeamStatus.Moving)
                {
                    singleExploreData.ExploreTeamData.RemainDay--;
                    //坐标需要变化
                    float rate = (singleExploreData.ExploreTeamData.TotalDay - singleExploreData.ExploreTeamData.RemainDay) / (float)singleExploreData.ExploreTeamData.TotalDay;

                    Vector2 beforeMovePos = new Vector2(singleExploreData.ExploreTeamData.LocalPosBeforeMove[0], singleExploreData.ExploreTeamData.LocalPosBeforeMove[1]);
                    SingleMapEventData targetEventData = MapEventManager.Instance.FindMapEventDataByOnlyId(singleExploreData.ExploreTeamData.TargetEventOnlyId);
                    Vector2 theVec = new Vector2(targetEventData.Pos[0],targetEventData.Pos[1]) - beforeMovePos;
                    theVec *= rate;
                    Vector2 newPos = beforeMovePos + theVec;
                    singleExploreData.ExploreTeamData.Pos[0] = newPos.x;
                    singleExploreData.ExploreTeamData.Pos[1] = newPos.y;

                    EventCenter.Broadcast(TheEventType.OnTeamExploreMoving, singleExploreData);
                    if (singleExploreData.ExploreTeamData.RemainDay <= 0)
                    {
                        ExploreArrived(singleExploreData);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 探险到达了目的地
    /// </summary>
    public void ExploreArrived(SingleExploreData singleExploreData)
    {
        singleExploreData.ExploreTeamData.Status = (int)ExploreTeamStatus.Idling;
        SingleMapEventData eventData = MapEventManager.Instance.FindMapEventDataByOnlyId(singleExploreData.ExploreTeamData.TargetEventOnlyId);
        singleExploreData.ExploreTeamData.Pos.Clear();
        singleExploreData.ExploreTeamData.Pos.Add(eventData.Pos[0]);
        singleExploreData.ExploreTeamData.Pos.Add(eventData.Pos[1]);

        singleExploreData.ExploreTeamData.LogicPosIndex = eventData.PosIndex;

        singleExploreData.ExploreTeamData.LogicPos.Clear();
        singleExploreData.ExploreTeamData.LogicPos.Add(eventData.LogicPos[0]);
        singleExploreData.ExploreTeamData.LogicPos.Add(eventData.LogicPos[1]);
        EventCenter.Broadcast(TheEventType.OnTeamExploreArrived, singleExploreData);
    }

    /// <summary>
    /// 通过探险队唯一id找探险队
    /// </summary>
    /// <param name="onlyId"></param>
    /// <returns></returns>
    public ExploreTeamData FindExploreTeamDataByOnlyId(ulong onlyId)
    {
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList.Count;i++)
        {
            SingleExploreData data = RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList[i];
            if(data.ExploreTeamData!=null
                && onlyId == data.ExploreTeamData.OnlyId)
    
            {
                return data.ExploreTeamData;
            }
        }
        return null;
    }


    /// <summary>
    /// 离开探险
    /// </summary>
    public void OnLeaveExplore(SingleExploreData singleExploreData,ExploreTeamData teamData)
    {
        ExploreMapSetting setting = DataTable.FindExploreMapSetting(singleExploreData.SettingId);
        for (int i = 0; i < singleExploreData.ExploreTeamData.StudentOnlyIdList.Count; i++)
        {
            ulong onlyId = singleExploreData.ExploreTeamData.StudentOnlyIdList[i];
            PeopleData p = StudentManager.Instance.FindStudent(onlyId);
            if(p!=null)
            p.studentStatusType = (int)StudentStatusType.None;

        }

        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList.Count; i++)
        {
            SingleExploreData data = RoleManager.Instance._CurGameInfo.AllExploreData.ExploreList[i];
            if (data.SettingId == singleExploreData.SettingId)
            {
                data.ExploreTeamData = null;
            }
     
        }
        //事件清掉
        for (int i = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList.Count-1; i>=0 ; i--)
        {
            SingleMapEventData data = RoleManager.Instance._CurGameInfo.allMapEventData.SingleMapEventList[i];
            if (data.ExploreSettingId == singleExploreData.SettingId)
            {
                MapEventManager.Instance.RemoveMapEvent(data);
            }
        }
        TaskManager.Instance.GetAchievement(AchievementType.Explore, setting.Id);
        TaskManager.Instance.TryAccomplishGuideBook(TaskType.Explore);
        TaskManager.Instance.GetDailyAchievement(TaskType.Explore, "1");
        GameSceneManager.Instance.GoToScene(SceneType.Mountain);

 
    }

    /// <summary>
    /// 计算队伍战斗力
    /// </summary>
    /// <param name="exploreTeamData"></param>
    /// <returns></returns>
    public int CalcExploreTeamZhanDouLi(ExploreTeamData exploreTeamData)
    {
        int res = 0;
        for(int i = 0; i < exploreTeamData.StudentOnlyIdList.Count; i++)
        {
            PeopleData p =StudentManager.Instance.FindStudent(exploreTeamData.StudentOnlyIdList[i]);
            res += (int)RoleManager.Instance.CalcZhanDouLi(p);
        }
        return res;
    }

    /// <summary>
    /// 通过的最高地图
    /// </summary>
    /// <returns></returns>
    public SingleMapData PassedMaxMap()
    {
        SingleMapData res = null;
        for (int i = 0; i < DataTable._mapList.Count; i++)
        {
            MapSetting mapSetting = DataTable._mapList[i];
            SingleMapData map = FindMapById(mapSetting.Id.ToInt32());
            if (map.MapStatus == (int)AccomplishStatus.Accomplished)
            {
                res = map;
            }
            else
            {
                break;
            }
        }
        return res;
    }

}

public enum AccomplishStatus
{
    None=0,
    Locked=1,
    UnAccomplished=2,
    Processing=3,//执行中
    Accomplished=4,
    GetAward=5,//拿过奖了 关闭
}

public enum LevelType
{
    None=0,
    Battle=1,//战斗
    ZhongZhuanZhan=2,//中转站
    JingYingBattle=3,//精英
    BossBattle=4,//boss

}
/// <summary>
/// 关卡id
/// </summary>
public enum LevelIdType
{
    None=0,
    ShanHaiZongZhangMen= 1010,
}

/// <summary>
/// 弟子上阵的类型
/// </summary>
public enum StudentUpType
{
    None=0,
    FixedMap=1,
    XianMenMap=2,
    Explore=3,
}