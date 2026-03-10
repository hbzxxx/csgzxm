using System.Collections.Generic;
using UnityEngine;
using Framework.Data;

/// <summary>
/// 存档生成器 - 修改玩家等级和地图解锁状态
/// </summary>
public class ArchiveGenerator : MonoBehaviour
{
    private static ArchiveGenerator _instance;
    public static ArchiveGenerator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ArchiveGenerator>();
                if (_instance == null)
                {
                    GameObject singletonObj = new GameObject("ArchiveGenerator_Singleton");
                    _instance = singletonObj.AddComponent<ArchiveGenerator>();
                    DontDestroyOnLoad(singletonObj);
                }
            }
            return _instance;
        }
    }

    public GameInfo _gameInfo;

    [Header("玩家等级设置")]
    public int playerLevel = 9999;

    [ContextMenu("生成存档")]
    public void GenerateFullUnlockArchive()
    {
        GameInfo gameInfo = CreateModifiedGameInfo();
        _gameInfo = gameInfo;
        Debug.Log($"存档已生成，玩家等级: {playerLevel}");
    }

    public GameInfo GetGameInfo()
    {
        return _gameInfo;
    }

    private GameInfo CreateModifiedGameInfo()
    {
        GameInfo gameInfo = RoleManager.Instance._CurGameInfo;

        gameInfo.playerPeople = CreateMaxLevelPlayer(gameInfo.playerPeople);
        gameInfo.AllMapData = CreateAllUnlockedMaps();

        return gameInfo;
    }

    private PeopleData CreateMaxLevelPlayer(PeopleData originalPlayer)
    {
        if (originalPlayer != null)
        {
            originalPlayer.studentLevel = playerLevel;
            if (originalPlayer.curEquipItemList == null)
            {
                originalPlayer.curEquipItemList = new List<ItemData> { null, null, null, null, null, null };
            }
        }
        return originalPlayer;
    }

    private AllMapData CreateAllUnlockedMaps()
    {
        AllMapData mapData = new AllMapData
        {
            CurChoosedMapId = 1,
            CurChoosedXianMenMapId = 1,
            MapList = new List<SingleMapData>()
        };

        if (DataTable._mapList == null || DataTable._mapList.Count == 0)
        {
            return mapData;
        }

        for (int i = 0; i < DataTable._mapList.Count; i++)
        {
            MapSetting setting = DataTable._mapList[i];
            SingleMapData map = new SingleMapData
            {
                MapId = setting.Id.ToInt32(),
                MapStatus = 2,
                LieXiMapStatus = 2,
                LevelList = new List<SingleLevelData>(),
                FixedLevelList = new List<SingleLevelData>(),
                CurAwardList = new List<ItemData>()
            };

            List<string> levelIdList = CommonUtil.SplitCfgStringOneDepth(setting.LevelIdList);
            for (int j = 0; j < levelIdList.Count; j++)
            {
                string levelIdStr = levelIdList[j];
                if (string.IsNullOrEmpty(levelIdStr) || levelIdStr == "0")
                    continue;

                SingleLevelData level = new SingleLevelData
                {
                    LevelId = levelIdStr,
                    LevelStatus = 2,
                    HaveAccomplished = true,
                    Enemy = new List<PeopleData>()
                };
                map.LevelList.Add(level);
            }

            List<string> fixedLevelIdList = CommonUtil.SplitCfgStringOneDepth(setting.FixedLevelIdList);
            for (int j = 0; j < fixedLevelIdList.Count; j++)
            {
                string levelIdStr = fixedLevelIdList[j];
                if (string.IsNullOrEmpty(levelIdStr) || levelIdStr == "0")
                    continue;

                SingleLevelData level = new SingleLevelData
                {
                    LevelId = levelIdStr,
                    LevelStatus = 2,
                    HaveAccomplished = true,
                    Enemy = new List<PeopleData>()
                };
                map.FixedLevelList.Add(level);
            }

            mapData.MapList.Add(map);
        }

        return mapData;
    }
}
