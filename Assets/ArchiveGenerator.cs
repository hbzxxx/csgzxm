using System.Collections.Generic;
using UnityEngine;
using Framework.Data;
using cfg;

/// <summary>
/// 存档生成器 - 在保存存档时修改玩家等级、主城等级、新手教程、地图解锁
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

    public GameInfo GetGameInfo()
    {
        if (Instance == null)
        {
            var go = new GameObject("ArchiveGenerator");
            go.AddComponent<ArchiveGenerator>();
        }
        
        var gameInfo = RoleManager.Instance._CurGameInfo;
        ModifyGameInfo(gameInfo);
        return gameInfo;
    }

    private void ModifyGameInfo(GameInfo gameInfo)
    {
        if (gameInfo == null) return;

        // 玩家等级
        if (gameInfo.playerPeople != null)
        {
            gameInfo.playerPeople.studentLevel = playerLevel;
        }

        // 主城等级
        if (gameInfo.AllBuildingData != null)
        {
            gameInfo.AllBuildingData.MountainLevel = mountainLevel;
        }

        // 新手教程 - 全部完成
        gameInfo.NewGuideData = CreateCompletedNewGuide();

        // 地图数据 - 全部解锁
        gameInfo.AllMapData = CreateUnlockedMaps();

        // 装备制造数据 - 清空避免空引用
        if (gameInfo.AllEquipmentData != null)
        {
            gameInfo.AllEquipmentData.curEquipMakeData = null;
        }
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
