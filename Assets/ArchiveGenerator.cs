using System.Collections.Generic;
using UnityEngine;
using Framework.Data;
using cfg;

/// <summary>
/// 存档生成器 - 修改玩家等级、主城等级、新手教程、地图解锁
/// </summary>
public class ArchiveGenerator : MonoBehaviour
{
    public static ArchiveGenerator Instance { get; private set; }
    public GameInfo _gameInfo;

    [Header("玩家等级")]
    public int playerLevel = 9999;

    [Header("主城等级")]
    public int mountainLevel = 9999;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 获取游戏数据并进行修改
    /// </summary>
    public GameInfo GetGameInfo()
    {
        var gameInfo = RoleManager.Instance._CurGameInfo;
        ModifyGameInfo(gameInfo);
        return gameInfo;
    }

    /// <summary>
    /// 修改当前游戏数据
    /// </summary>
    public void ModifyCurrentGameInfo()
    {
        ModifyGameInfo(RoleManager.Instance._CurGameInfo);
        Debug.Log($"[ArchiveGenerator] 修改完成: 玩家等级={playerLevel}, 主城等级={mountainLevel}");
    }

    /// <summary>
    /// 保存当前游戏数据为新存档
    /// </summary>
    public void SaveAsNewArchive(int archiveIndex)
    {
        var gameInfo = GetGameInfo();
        ArchiveManager.Instance.SaveArchive(archiveIndex);
    }

    private void ModifyGameInfo(GameInfo gameInfo)
    {
        if (gameInfo == null) return;

        Debug.Log($"[ArchiveGenerator] 修改前 - 玩家等级: {gameInfo.playerPeople?.studentLevel}, 主城等级: {gameInfo.AllBuildingData?.MountainLevel}");

        // 玩家等级
        if (gameInfo.playerPeople != null)
        {
            gameInfo.playerPeople.studentLevel = playerLevel;
            Debug.Log($"[ArchiveGenerator] 设置玩家等级为: {playerLevel}");
        }
        else
        {
            Debug.LogWarning("[ArchiveGenerator] playerPeople 为空!");
        }

        // 主城等级 - 确保 AllBuildingData 存在
        if (gameInfo.AllBuildingData == null)
        {
            gameInfo.AllBuildingData = new AllBuildingData();
            Debug.Log("[ArchiveGenerator] 创建新的 AllBuildingData");
        }
        gameInfo.AllBuildingData.MountainLevel = mountainLevel;
        Debug.Log($"[ArchiveGenerator] 设置主城等级为: {mountainLevel}");

        // 新手教程 - 全部完成
        gameInfo.NewGuideData = CreateCompletedNewGuide();
        Debug.Log($"[ArchiveGenerator] 设置新手教程完成, 已完成数量: {gameInfo.NewGuideData.finishedGuideIdList.Count}");

        // 地图数据 - 全部解锁
        gameInfo.AllMapData = CreateUnlockedMaps();
        int unlockedMaps = 0;
        int unlockedLevels = 0;
        foreach (var map in gameInfo.AllMapData.MapList)
        {
            if (map.MapStatus == 2) unlockedMaps++;
            if (map.LevelList != null)
            {
                foreach (var level in map.LevelList)
                {
                    if (level.LevelStatus == 2) unlockedLevels++;
                }
            }
        }
        Debug.Log($"[ArchiveGenerator] 设置地图解锁, 地图数量: {gameInfo.AllMapData.MapList.Count}, 解锁地图: {unlockedMaps}, 解锁关卡: {unlockedLevels}");

        // 装备制造数据 - 清空避免空引用
        if (gameInfo.AllEquipmentData != null)
        {
            gameInfo.AllEquipmentData.curEquipMakeData = null;
        }

        Debug.Log($"[ArchiveGenerator] 修改后 - 玩家等级: {gameInfo.playerPeople?.studentLevel}, 主城等级: {gameInfo.AllBuildingData?.MountainLevel}");
    }

    private NewGuideData CreateCompletedNewGuide()
    {
        var guideData = new NewGuideData
        {
            finishedGuideIdList = new List<int>(),
            IdList = new List<int>(),
            AccomplishStatus = new List<int>(),
            curGuideId = 0,
            curGuideStep = 0
        };

        for (int i = 1; i <= 100; i++)
        {
            guideData.finishedGuideIdList.Add(i);
            guideData.IdList.Add(i);
            guideData.AccomplishStatus.Add(2);
        }

        return guideData;
    }

    private AllMapData CreateUnlockedMaps()
    {
        var mapData = new AllMapData
        {
            CurChoosedMapId = 1,
            CurChoosedXianMenMapId = 1,
            MapList = new List<SingleMapData>()
        };

        if (DataTable._mapList == null || DataTable._mapList.Count == 0)
            return mapData;

        foreach (var setting in DataTable._mapList)
        {
            var map = new SingleMapData
            {
                MapId = setting.Id.ToInt32(),
                MapStatus = 2,
                LieXiMapStatus = 2,
                LevelList = new List<SingleLevelData>(),
                FixedLevelList = new List<SingleLevelData>(),
                CurAwardList = new List<ItemData>()
            };

            foreach (var levelId in CommonUtil.SplitCfgStringOneDepth(setting.LevelIdList))
            {
                if (string.IsNullOrEmpty(levelId) || levelId == "0") continue;
                map.LevelList.Add(new SingleLevelData
                {
                    LevelId = levelId,
                    LevelStatus = 2,
                    HaveAccomplished = true,
                    Enemy = new List<PeopleData>()
                });
            }

            foreach (var levelId in CommonUtil.SplitCfgStringOneDepth(setting.FixedLevelIdList))
            {
                if (string.IsNullOrEmpty(levelId) || levelId == "0") continue;
                map.FixedLevelList.Add(new SingleLevelData
                {
                    LevelId = levelId,
                    LevelStatus = 2,
                    HaveAccomplished = true,
                    Enemy = new List<PeopleData>()
                });
            }

            mapData.MapList.Add(map);
        }

        return mapData;
    }
}
