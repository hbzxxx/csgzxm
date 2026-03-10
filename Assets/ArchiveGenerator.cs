using System.Collections.Generic;
using UnityEngine;
using Framework.Data;
using cfg;

/// <summary>
/// 存档生成器 - 解锁所有地图和关卡，设置玩家等级
/// </summary>
public class ArchiveGenerator : MonoBehaviour
{
    public static ArchiveGenerator Instance { get; private set; }
    public GameInfo _gameInfo;

    [Header("玩家等级")]
    public int playerLevel = 9999;

    [ContextMenu("生成存档")]
    public void GenerateArchive()
    {
        _gameInfo = ModifyGameInfo(RoleManager.Instance._CurGameInfo);
        Debug.Log($"存档已生成，玩家等级: {playerLevel}");
    }

    private GameInfo ModifyGameInfo(GameInfo gameInfo)
    {
        if (gameInfo.playerPeople != null)
        {
            gameInfo.playerPeople.studentLevel = playerLevel;
        }
        gameInfo.AllMapData = CreateUnlockedMaps();
        return gameInfo;
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
