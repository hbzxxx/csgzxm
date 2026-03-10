using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ES3;

/// <summary>
/// 存档生成器 - 创建全解锁存档
/// </summary>
public class ArchiveGenerator : MonoBehaviour
{
    [Header("存档设置")]
    public int targetArchiveIndex = 0;
    public string savePath = "Assets/StreamingAssets/Archives/archive_0/GameInfo.es3";
    
    [Header("解锁设置")]
    public int maxLevel = 100;
    public int maxResource = 999999999;
    
    [ContextMenu("生成全解锁存档")]
    public void GenerateFullUnlockArchive()
    {
        GameInfo gameInfo = CreateFullUnlockGameInfo();
        SaveGameInfo(gameInfo);
        Debug.Log("全解锁存档已生成: " + savePath);
    }
    
    private GameInfo CreateFullUnlockGameInfo()
    {
        GameInfo gameInfo = new GameInfo();
        
        gameInfo.TheGuid = System.Guid.NewGuid().ToString();
        gameInfo.SaveTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        gameInfo.CreateTime = gameInfo.SaveTime;
        gameInfo.VersionName = Application.version;
        gameInfo.NickName = "测试玩家";
        gameInfo.roleId = 1;
        gameInfo.quIndex = 1;
        
        gameInfo.playerPeople = CreateMaxLevelPlayer();
        gameInfo.timeData = CreateTimeData();
        gameInfo.ItemModel = CreateMaxResources();
        gameInfo.AllBuildingData = CreateAllBuildings();
        gameInfo.AllMapData = CreateAllMaps();
        gameInfo.AllAchievementData = CreateAllAchievements();
        gameInfo.AllUIComponentUnlockStatus = CreateAllUnlockedUI();
        gameInfo.AllResearchData = CreateAllResearched();
        gameInfo.QianDaoData = CreateSignedInData();
        gameInfo.AllShopData = CreateShopData();
        
        return gameInfo;
    }
    
    private PeopleData CreateMaxLevelPlayer()
    {
        PeopleData player = new PeopleData();
        player.name = "主角";
        player.level = maxLevel;
        player.exp = maxLevel * 10000;
        player.hp = 99999;
        player.maxHp = 99999;
        player.attack = 9999;
        player.defense = 9999;
        player.speed = 9999;
        player.crit = 9999;
        player.critDamage = 500;
        player.onlyId = 1000001;
        
        return player;
    }
    
    private TimeData CreateTimeData()
    {
        TimeData timeData = new TimeData();
        timeData.year = 9999;
        timeData.month = 12;
        timeData.day = 31;
        timeData.hour = 23;
        timeData.minute = 59;
        timeData.dayCount = 999999;
        
        return timeData;
    }
    
    private ItemModel CreateMaxResources()
    {
        ItemModel itemModel = new ItemModel();
        itemModel.gold = maxResource;
        itemModel.silver = maxResource;
        itemModel.copper = maxResource;
        itemModel.jade = maxResource;
        itemModel.exp = maxResource;
        itemModel.spiritStone = maxResource;
        
        itemModel.itemDataList = new List<ItemData>();
        itemModel.cangKuItemDataList = new List<ItemData>();
        
        return itemModel;
    }
    
    private AllBuildingData CreateAllBuildings()
    {
        AllBuildingData buildingData = new AllBuildingData();
        buildingData.allBuildingList = new List<BuildingData>();
        
        return buildingData;
    }
    
    private AllMapData CreateAllMaps()
    {
        AllMapData mapData = new AllMapData();
        mapData.allMapList = new List<MapData>();
        
        return mapData;
    }
    
    private AllAchievementData CreateAllAchievements()
    {
        AllAchievementData achievementData = new AllAchievementData();
        achievementData.completedAchievementList = new List<int>();
        
        return achievementData;
    }
    
    private UIComponentUnlockStatus CreateAllUnlockedUI()
    {
        UIComponentUnlockStatus unlockStatus = new UIComponentUnlockStatus();
        unlockStatus.unlockedComponents = new List<string>();
        unlockStatus.unlockedComponents.Add("All");
        
        return unlockStatus;
    }
    
    private AllResearchData CreateAllResearched()
    {
        AllResearchData researchData = new AllResearchData();
        researchData.researchedTechList = new List<int>();
        
        return researchData;
    }
    
    private QianDaoData CreateSignedInData()
    {
        QianDaoData qianDaoData = new QianDaoData();
        qianDaoData.signedDays = 365;
        qianDaoData.totalSignedDays = 999;
        qianDaoData.lastSignTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        return qianDaoData;
    }
    
    private AllShopData CreateShopData()
    {
        AllShopData shopData = new AllShopData();
        
        return shopData;
    }
    
    private void SaveGameInfo(GameInfo gameInfo)
    {
        string directory = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        var settings = new ES3Settings(ES3.EncryptionType.AES, "YourEncryptionKey");
        ES3.Save<GameInfo>("GameData", gameInfo, savePath, settings);
        
        Debug.Log($"存档已保存到: {savePath}");
    }
}
