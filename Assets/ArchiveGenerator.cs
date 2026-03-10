using System.Collections.Generic;
using UnityEngine;
using Framework.Data;
using cfg;

/// <summary>
/// 存档生成器 - 在保存存档时修改地图为全部解锁，玩家等级最高
/// </summary>
public class ArchiveGenerator : MonoBehaviour
{
    public static ArchiveGenerator Instance { get; private set; }
    public GameInfo _gameInfo;

    [Header("玩家等级")]
    public int playerLevel = 9999;

    private void Awake()
    {
        Instance = this;
    }

    public GameInfo GetGameInfo()
    {
        var gameInfo = RoleManager.Instance._CurGameInfo;
        ModifyGameInfo(gameInfo);
        return gameInfo;
    }

    private void ModifyGameInfo(GameInfo gameInfo)
    {
        if (gameInfo == null) return;

        if (gameInfo.playerPeople != null)
        {
            gameInfo.playerPeople.studentLevel = playerLevel;
        }

        gameInfo.AllMapData = CreateUnlockedMaps();
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
