using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 存档生成器 - 创建全解锁存档
/// </summary>
public class ArchiveGenerator : MonoBehaviour
{
    private static ArchiveGenerator _instance;
    public static ArchiveGenerator Instance
    {
        get
        {
            // 如果实例为空，自动查找/创建
            if (_instance == null)
            {
                _instance = FindObjectOfType<ArchiveGenerator>();

                // 场景中无该组件时，自动创建挂载对象
                if (_instance == null)
                {
                    GameObject singletonObj = new GameObject("ArchiveGenerator_Singleton");
                    _instance = singletonObj.AddComponent<ArchiveGenerator>();
                    // 标记为 DontDestroyOnLoad，切换场景不销毁
                    DontDestroyOnLoad(singletonObj);
                }
            }
            return _instance;
        }
    }
    public GameInfo _gameInfo;
    [Header("存档设置")]
    public int targetArchiveIndex = 0;
    public string savePath = "Assets/StreamingAssets/Archives/archive_0/GameInfo.es3";

    [Header("解锁设置")]
    public int maxLevel = 9999;
    public int maxResource = 999999999;
    public int maxStudentCount = 9999;

    [Header("加密设置")]
    public string encryptionKey = "";

    [ContextMenu("生成全解锁存档")]
    public void GenerateFullUnlockArchive()
    {
        GameInfo gameInfo = CreateFullUnlockGameInfo();
        _gameInfo = new GameInfo();
        _gameInfo =gameInfo;
        //ArchiveManager.Instance.SaveGameInfo(gameInfo);
        //SaveGameInfo(gameInfo);
        Debug.Log($"全解锁存档已生成: {savePath}");
    }
    public GameInfo GetGameInfo()
    {
        return _gameInfo;
    }
    private GameInfo CreateFullUnlockGameInfo()
    {
        GameInfo gameInfo = RoleManager.Instance._CurGameInfo;
        // 玩家数据
        gameInfo.playerPeople = CreateMaxLevelPlayer();
        gameInfo.AllPeopleList = new List<PeopleData> { gameInfo.playerPeople };

        // 时间数据
        gameInfo.timeData = CreateTimeData();


        // 建筑数据
        gameInfo.AllBuildingData = CreateAllBuildings();

        // 地图数据
        gameInfo.AllMapData = CreateAllMaps();

        // 成就数据
        gameInfo.AllAchievementData = CreateAllAchievements();

        // UI解锁状态
        gameInfo.AllUIComponentUnlockStatus = CreateAllUnlockedUI();

        // 研究数据
        gameInfo.AllResearchData = CreateAllResearched();

        // 签到数据
        gameInfo.QianDaoData = CreateSignedInData();

        // 商店数据
        gameInfo.allShopData = CreateShopData();

        // 弟子数据
        //gameInfo.studentData = CreateStudentData();

        // 丹田数据
        gameInfo.allDanFarmData = CreateDanFarmData();

        // 探索数据
        gameInfo.AllExploreData = CreateExploreData();

        // 秘境派遣数据
        gameInfo.AllMiJingPaiQianData = CreateMiJingPaiQianData();

        // 装备数据
        gameInfo.AllEquipmentData = CreateEquipmentData();
        gameInfo.EquipMakeTeamData = new EquipMakeTeamData();

        // 炼丹数据
        gameInfo.LianDanData = CreateLianDanData();

        // 比赛数据
        gameInfo.MatchData = CreateMatchData();

        // 宗门数据
        gameInfo.allZongMenData = CreateZongMenData();
        gameInfo.AllZongMenProduceData = CreateZongMenProduceData();

        // Buff数据
        gameInfo.AllBuffData = new AllBuffData();

        // 新手引导数据（已完成）
        gameInfo.NewGuideData = CreateNewGuideData();

        // NPC数据
        gameInfo.allNPCData = new AllNPCData();

        // 场景数据
        gameInfo.SceneData = CreateSceneData();

        // 仙门大开
        gameInfo.XianMenOpen = true;

        // 地图事件数据
        gameInfo.allMapEventData = new AllMapEventData();

        // 活动数据
        gameInfo.AllActivityData = new ActivityData();

        // 队伍数据
        gameInfo.AllTeamData = CreateTeamData();

        // 冒险手札数据
        gameInfo.AllGuideBookData = CreateGuideBookData();

        // 广告数据
        gameInfo.AllADData = new AllADData();

        // 日常任务数据
        gameInfo.AllDailyTaskData = CreateDailyTaskData();

        // 邮件数据
        gameInfo.AllMailData = new AllMailData();

        // 记录数据
        gameInfo.RecordData = "全解锁测试存档";

        // 深渊数据
        gameInfo.AllShenYuanData = CreateShenYuanData();

        // 抽奖数据
        gameInfo.ChouJiangData = CreateChouJiangData();

        // 兑换码数据
        gameInfo.AllDuiHuanMaData = new AllDuiHuanMaData();

        // 标记人物数据
        gameInfo.NotedPeopleData = new NotedPeopleData();

        return gameInfo;
    }

    private PeopleData CreateMaxLevelPlayer()
    {
        PeopleData player = new PeopleData();
        player.onlyId = 1000001;
        player.name = "主角";
        player.isPlayer = true;
        player.gender = 1;

        // 属性列表
        player.propertyIdList = new List<int> { 1, 2, 3, 4, 5, 6 }; // 生命、攻击、防御等
        player.propertyList = new List<SinglePropertyData>
        {
            new SinglePropertyData { id = 1, num = 9999, limit = 9999, quality = 5 },
            new SinglePropertyData { id = 2, num = 9999, limit = 9999, quality = 5 },
            new SinglePropertyData { id = 3, num = 9999, limit = 9999, quality = 5 },
            new SinglePropertyData { id = 4, num = 9999, limit = 9999, quality = 5 },
            new SinglePropertyData { id = 5, num = 9999, limit = 9999, quality = 5 },
            new SinglePropertyData { id = 6, num = 9999, limit = 9999, quality = 5 }
        };

        // 弟子相关
        player.studentType = 1;
        player.studentLevel = 9999;
        player.studentCurExp = 99999999;
        player.studentRarity = 5;
        player.studentQuality = 5;
        player.studentCurEnergy = 100;
        player.studentStatusType = 1;

        // 修为

        // 技能数据
        player.allSkillData = CreateSkillData();

        // 装备（6个槽位）
        //player.curEquipItemList = new List<ItemData> { null, null, null, null, null, null };

        // 社交数据
        player.socializationData = new SocializationData
        {
            xingGe = 1,
            socialActivity = 100,
            knowPeopleList = new List<ulong>(),
            haoGanDu = new List<int>()
        };

        // 天赋
        player.talent = 1001;
        player.talentRarity = 5;
        player.yuanSu = 1;
        player.curUnlockedYuanSuList = new List<int> { 1, 2, 3, 4, 5 };
        player.changeTalentNum = 10;

        // 其他
        player.portraitIndexList = new List<int> { 1, 2, 3 };
        player.portraitType = 1;
        player.specialPortrait = false;
        player.curPhase = 10;
        player.totalPhase = 10;
        player.isMyTeam = true;
        player.atTeamIndex = 0;
        player.teamPosIndex = 0;
        player.changeNameNum = 0;
        player.xiSuiRate = 100;
        player.seriousInjury = false;
        player.diGuFuHuoed = true;

        return player;
    }

    private TimeData CreateTimeData()
    {
        TimeData timeData = new TimeData();
        long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        timeData.Year = 9999;
        timeData.Month = 12;
        timeData.Day = 31;
        timeData.Week = 52;
        timeData.TheWeekDay = 5;
        timeData.DayProcess = 0.5f;
        timeData.LastReviveTiliTime = currentTime;
        timeData.TodayADTiliNum = 5;
        timeData.LastADReviveTiliTime = currentTime;
        timeData.LastReceiveOfflineIncomeTime = currentTime;
        timeData.OfflineItemList = new List<ItemData>();
        timeData.LastRecordedTime = currentTime;
        timeData.OffLineTotalMinute = 1440; // 24小时
        timeData.NextRdmEventRemainMonth = 0;
        timeData.LastADWatchTime = currentTime;
        timeData.TotalADWatchNum = 100;
        timeData.TodayParticipatedLiLianStatus = new List<int> { 1, 2, 3 };
        timeData.LastParticipatedLiLianTime = new List<long> { currentTime, currentTime, currentTime };
        timeData.MaxLiLianTimePerDay = 10;
        timeData.LastRecordYuanShenShouSumTime = currentTime;
        timeData.LastRecordCloudArchiveTime = currentTime;
        timeData.TodayParticipateXuanXiuNum = 5;
        timeData.LastUploadArchiveTime = currentTime;
        timeData.TodayChouNum = 50;

        return timeData;
    }

    //private ItemModel CreateMaxResources()
    //{
    //    ItemModel itemModel = new ItemModel();

    //    // 基础资源
    //    itemModel.fuLingShiNum = maxResource;

    //    // 背包物品列表
    //    itemModel.itemDataList = new List<ItemData>();
    //    itemModel.cangKuItemDataList = new List<ItemData>();

    //    // 添加一些示例物品
    //    for (int i = 1; i <= 10; i++)
    //    {
    //        ItemData item = new ItemData
    //        {
    //            settingId = 1000 + i,
    //            onlyId = (ulong)(2000000 + i),
    //            count = 99,
    //            quality = 5
    //        };
    //        itemModel.itemDataList.Add(item);
    //        itemModel.itemIdList.Add(item.settingId);
    //        itemModel.onlyIdList.Add(item.onlyId);
    //    }

    //    return itemModel;
    //}

    private AllBuildingData CreateAllBuildings()
    {
        AllBuildingData buildingData = new AllBuildingData
        {
            MountainLevel = 9999,
            BuildList = new List<SingleBuildingData>()
        };

        // 创建各种建筑
        int[] buildingTypes = { 101, 102, 103, 104, 105 }; // 不同类型的建筑
        foreach (int typeId in buildingTypes)
        {
            SingleBuildingData building = new SingleBuildingData
            {
                BuildTypeId = typeId,
                CurBuildLevel = 9999,
                SettingId = typeId,
                MaxStudentNum = 20,
                StudentNum = 10,
                MaxNeiMenStudentNumLimit = 10
            };
            buildingData.BuildList.Add(building);
        }

        return buildingData;
    }

    private AllMapData CreateAllMaps()
    {
        AllMapData mapData = new AllMapData
        {
            CurChoosedMapId = 1,
            CurChoosedXianMenMapId = 1,
            MapList = new List<SingleMapData>()
        };

        // 创建地图数据 - 10个地图，每个地图10个关卡，全部解锁
        for (int mapId = 1; mapId <= 10; mapId++)
        {
            SingleMapData map = new SingleMapData
            {
                MapId = mapId,
                MapStatus = 2, // 已解锁
                LieXiMapStatus = 2,
                LevelList = new List<SingleLevelData>(),
                FixedLevelList = new List<SingleLevelData>(),
                CurAwardList = new List<ItemData>()
            };

            // 添加关卡 - 10个关卡全部解锁
            for (int levelId = 1; levelId <= 10; levelId++)
            {
                SingleLevelData level = new SingleLevelData
                {
                    LevelId = $"{mapId}_{levelId}",
                    LevelStatus = 2, // 已解锁
                    HaveAccomplished = true,
                    Enemy = new List<PeopleData>()
                };
                map.LevelList.Add(level);
            }

            mapData.MapList.Add(map);
        }

        return mapData;
    }

    private AllAchievementData CreateAllAchievements()
    {
        AllAchievementData achievementData = new AllAchievementData
        {
            achievementList = new List<SingleAchievementData>(),
            PassedTaskTagIdList = new List<string> { "TAG_1", "TAG_2", "TAG_3" },
            KilledEnemy = "1001,1002,1003,1004,1005",
            PassedMaxMapLevel = 100,
            AccomplishedMapEvent = new List<string> { "EVENT_1", "EVENT_2" },
            AccomplishedOnceGuideList = new List<int> { 1, 2, 3, 4, 5 },
            QuanLiNum = 100,
            TianFuJueXingNum = 50,
            StudentUpgradeCount = 1000,
            PassedMaxLevel = "10_10",
            PassedMaxLieXiLevel = "5_5",
            LiLianNum = 500,
            MakeGemNum = 200
        };

        // 添加成就
        for (int i = 1; i <= 50; i++)
        {
            SingleAchievementData achievement = new SingleAchievementData
            {
                settingId = 1000 + i,
                accomplishStatus = 2, // 已完成
                curProgress = 100
            };
            achievementData.achievementList.Add(achievement);
        }

        return achievementData;
    }

    private UIComponentUnlockStatus CreateAllUnlockedUI()
    {
        UIComponentUnlockStatus unlockStatus = new UIComponentUnlockStatus
        {
            UIComponentTypeList = new List<int>(),
            UIComponentStatusList = new List<int>()
        };

        // 解锁所有UI组件（假设有50个UI组件）
        for (int i = 1; i <= 50; i++)
        {
            unlockStatus.UIComponentTypeList.Add(i);
            unlockStatus.UIComponentStatusList.Add(1); // 1=已解锁
        }

        return unlockStatus;
    }

    private AllResearchData CreateAllResearched()
    {
        AllResearchData researchData = new AllResearchData
        {
            ResearchList = new List<SingleResearchData>(),
            ResearchingQueue = new List<int>()
        };

        // 完成所有研究
        for (int i = 1; i <= 30; i++)
        {
            SingleResearchData research = new SingleResearchData
            {
                SettingId = 2000 + i,
                CurLevel = 9999,
                LevelLimit = 9999,
                IsResearching = false,
                TotalTime = 100,
                RemainTime = 0
            };
            researchData.ResearchList.Add(research);
        }

        return researchData;
    }

    private QianDaoData CreateSignedInData()
    {
        long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        QianDaoData qianDaoData = new QianDaoData
        {
            SevenDayQianDaoIndex = 7,
            CanSevenDayQianDaoIndex = 7,
            ThirtyDayQianDaoIndex = 30,
            CanThirtyDayQianDaoIndex = 30,
            LastQianDaoTime = currentTime
        };

        return qianDaoData;
    }

    private AllShopData CreateShopData()
    {
        AllShopData shopData = new AllShopData
        {
            ShopList = new List<SingleShopData>(),
            rmbBuyRecordList = new List<int> { 1, 2, 3, 4, 5 },
            totalChargeNum = 10000,
            accomplishStatusList = new List<int> { 1, 1, 1, 1, 1 },
            shouChongAwardGet = true
        };

        // 创建商店
        int[] shopTypes = { 1, 2, 3, 4 }; // 普通商店、神秘商店等
        long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        foreach (int shopType in shopTypes)
        {
            SingleShopData shop = new SingleShopData
            {
                ShopType = shopType,
                LastBrushTime = currentTime,
                TodayBrushNum = 10,
                LastBuyTime = currentTime,
                ShouChonged = new List<int> { 1001, 1002 },
                ShopItemList = new List<ShopItemData>()
            };

            // 添加商品
            for (int i = 1; i <= 10; i++)
            {
                ShopItemData item = new ShopItemData
                {
                    Id = 3000 + i,
                    ItemId = 4000 + i,
                    RemainCount = 9999,
                    moonCardReachTime = currentTime + 2592000 // 30天
                };
                shop.ShopItemList.Add(item);
            }

            shopData.ShopList.Add(shop);
        }

        return shopData;
    }

    //private StudentData CreateStudentData()
    //{
    //    StudentData studentData = new StudentData
    //    {
    //        MaxStudentNum = maxStudentCount,
    //        CurStudentNum = maxStudentCount,
    //        CurFreeStudentNum = 10,
    //        allStudentList = new List<PeopleData>(),
    //        recruitCandidateStudent = new List<PeopleData>(),
    //        thisYearRemainCanRecruitStudentNum = 10,
    //        thisYearRecruitedStudentNum = maxStudentCount,
    //        thisYearBrushStudentNum = 20,
    //        thisYearWatchedADNum = true,
    //        lastNewStudentYear = 9999,
    //        todayRecruitStudentNum = 5,
    //        lastRecruitStudentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
    //        totalRecruitStudentNum = maxStudentCount
    //    };

    //    // 添加弟子
    //    for (int i = 1; i <= maxStudentCount; i++)
    //    {
    //        PeopleData student = CreateStudent(i);
    //        studentData.allStudentList.Add(student);
    //    }

    //    return studentData;
    //}

    private PeopleData CreateStudent(int index)
    {
        PeopleData student = new PeopleData();
        student.onlyId = (ulong)(2000000 + index);
        student.name = $"弟子{index}";
        student.isPlayer = false;
        student.gender = index % 2 + 1;

        // 属性
        student.propertyIdList = new List<int> { 1, 2, 3, 4, 5 };
        student.propertyList = new List<SinglePropertyData>();
        foreach (int propId in student.propertyIdList)
        {
            student.propertyList.Add(new SinglePropertyData
            {
                id = propId,
                num = 9999,
                limit = 9999,
                quality = 5
            });
        }

        // 弟子数据
        student.studentType = 1;
        student.studentLevel = 9999;
        student.studentCurExp = 99999999;
        student.studentRarity = 5;
        student.studentQuality = 5;
        student.studentCurEnergy = 9999;
        student.studentStatusType = 1;

        // 修为
        student.curXiuwei = 999999999;
        student.lastXiuweiAddTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // 技能
        student.allSkillData = new AllSkillData
        {
            skillList = new List<SingleSkillData>(),
            unlockedTypeList = new List<int> { 1, 2, 3 },
            unlockedSkillPos = 3,
            equippedSkillIdList = new List<int> { 1001, 1002, 1003 }
        };

        // 社交
        student.socializationData = new SocializationData
        {
            xingGe = index % 5 + 1,
            socialActivity = 50 + index,
            knowPeopleList = new List<ulong>(),
            haoGanDu = new List<int>()
        };

        // 天赋
        student.talent = 2000 + index % 10;
        student.talentRarity = 3;
        student.yuanSu = index % 5 + 1;
        student.curUnlockedYuanSuList = new List<int> { 1, 2, 3 };

        // 队伍位置
        student.isMyTeam = index <= 6;
        if (student.isMyTeam)
        {
            student.atTeamIndex = 0;
            student.teamPosIndex = index - 1;
        }

        return student;
    }

    private AllDanFarmData CreateDanFarmData()
    {
        AllDanFarmData danFarmData = new AllDanFarmData
        {
            DanFarmList = new List<SingleDanFarmData>(),
            DanFarmZuoZhenStudentLimit = 10,
            UnlockedDanFarmId = new List<int>(),
            UnlockedDanFarmNumLimit = 20,
            UnlockedFarmNum = 10,
            ReBuildingIndexList = new List<int>(),
            UnlockFarmNeedLingShiNum = 0
        };

        // 创建丹田
        for (int i = 1; i <= 10; i++)
        {
            SingleDanFarmData farm = new SingleDanFarmData
            {
                OnlyId = (ulong)(3000000 + i),
                SettingId = 4000 + i,
                CurLevel = 9999,
                IsEmpty = false,
                Index = i,
                Status = 3, // 工作中
                RemainTime = 0,
                ProcessDanTimer = 100,
                RebuildTotalTime = 0,
                ZuoZhenStudentIdList = new List<ulong>(),
                OpenQuanLi = true,
                QuanLiTotalTime = 1000,
                QuanliRemainTime = 0,
                DanFarmType = i % 3 + 1,
                ProcessSpeed = 100,
                ProductSettingId = 5000 + i,
                ProductRemainNum = 9999,
                DanFarmWorkType = 1,
                Unlocked = true,
                TalentType = i % 5 + 1,
                UnlockedProductIdList = new List<int>(),
                StudentUseCangKuDataList = new List<SingleStudentUseCangKuData>(),
                ProductItemList = new List<ItemData>(),
                ProductTotalNum = 9999,
                LocalPos = new Vector2(i * 100, 100),
                HandleStop = false,
                NeedForeItemId = 0,
                SingleDanPrice = 1000
            };

            // 添加产品
            for (int j = 1; j <= 5; j++)
            {
                farm.UnlockedProductIdList.Add(5000 + i * 10 + j);
            }

            danFarmData.DanFarmList.Add(farm);
            danFarmData.UnlockedDanFarmId.Add(farm.SettingId);
        }

        return danFarmData;
    }

    private AllExploreData CreateExploreData()
    {
        AllExploreData exploreData = new AllExploreData
        {
            ExploreList = new List<SingleExploreData>()
        };

        long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // 创建探索数据
        for (int i = 1; i <= 5; i++)
        {
            SingleExploreData explore = new SingleExploreData
            {
                SettingId = 6000 + i,
                IsExploring = false,
                Unlocked = true,
                AllEventNum = 20,
                ExploreTeamData = new ExploreTeamData
                {
                    OnlyId = (ulong)(4000000 + i),
                    ExploreId = 6000 + i,
                    StudentOnlyIdList = new List<ulong>(),
                    ExploreDuration = 3600,
                    ItemList = new List<ItemData>(),
                    LogicPos = new List<int> { i * 10, i * 10 },
                    LogicPosIndex = i,
                    RemainBuJi = 100,
                    TargetEventOnlyId = 0,
                    Status = 0,
                    TotalDay = 10,
                    RemainDay = 0,
                    Pos = new List<float> { i * 10f, i * 10f },
                    LocalPosBeforeMove = new List<float> { i * 10f, i * 10f }
                }
            };

            exploreData.ExploreList.Add(explore);
        }

        return exploreData;
    }

    private AllMiJingPaiQianData CreateMiJingPaiQianData()
    {
        AllMiJingPaiQianData miJingData = new AllMiJingPaiQianData
        {
            PaiqianList = new List<SingleMiJingPaiQianData>()
        };

        long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // 创建秘境数据
        for (int i = 1; i <= 3; i++)
        {
            SingleMiJingPaiQianData miJing = new SingleMiJingPaiQianData
            {
                SettingId = 7000 + i,
                StartPaiQianTime = currentTime,
                IsPaiQian = false,
                LastKillMonsterTime = currentTime,
                DayliHighNum = 100,
                MaxDayliHighNum = 100,
                WeekType = i,
                HighestLevelLevel = 50,
                LevelList = new List<SingleMiJingLevelData>()
            };

            // 添加关卡
            for (int j = 1; j <= 10; j++)
            {
                SingleMiJingLevelData level = new SingleMiJingLevelData
                {
                    MiJingId = 7000 + i,
                    LevelId = j,
                    AccomplishStatus = 2 // 已完成
                };
                miJing.LevelList.Add(level);
            }

            miJingData.PaiqianList.Add(miJing);
        }

        return miJingData;
    }

    private AllEquipmentData CreateEquipmentData()
    {
        AllEquipmentData equipmentData = new AllEquipmentData
        {
            curEquippedEquipList = new List<EquipProtoData>(),
            pictureList = new List<SingleEquipPictureData>(),
            curEquipMakeData = null
        };

        // 创建一些装备
        for (int i = 1; i <= 10; i++)
        {
            EquipProtoData equip = new EquipProtoData
            {
                onlyId = (ulong)(5000000 + i),
                settingId = 8000 + i,
                curLevel = 9999,
                curExp = 10000,
                curDurability = 100,
                propertyIdList = new List<int> { 1, 2, 3 },
                propertyList = new List<SinglePropertyData>(),
                gemSaveList = new List<GemSaveData>(),
                isEquipped = i <= 6,
                belongP = i <= 6 ? 1000001UL : 0,
                isPrepareToWorld = false,
                youHuaLv = 100,
                jingLianLv = 10
            };

            // 属性
            equip.propertyList.Add(new SinglePropertyData { id = 1, num = 1000 * i, quality = 5 });
            equip.propertyList.Add(new SinglePropertyData { id = 2, num = 100 * i, quality = 5 });
            equip.propertyList.Add(new SinglePropertyData { id = 3, num = 100 * i, quality = 5 });

            equipmentData.curEquippedEquipList.Add(equip);
        }

        // 图纸
        for (int i = 1; i <= 20; i++)
        {
            SingleEquipPictureData picture = new SingleEquipPictureData
            {
                equipId = 8000 + i,
                status = 1 // 已制作
            };
            equipmentData.pictureList.Add(picture);
        }

        return equipmentData;
    }

    private LianDanData CreateLianDanData()
    {
        LianDanData lianDanData = new LianDanData
        {
            unlockedDanFangIdList = new List<int>(),
            danFangProficiencyList = new List<int>(),
            MaxGemRarity = 5
        };

        // 解锁所有丹方
        for (int i = 1; i <= 50; i++)
        {
            lianDanData.unlockedDanFangIdList.Add(9000 + i);
            lianDanData.danFangProficiencyList.Add(100);
        }

        return lianDanData;
    }

    private MatchData CreateMatchData()
    {
        long currentTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        MatchData matchData = new MatchData
        {
            LastParticipateMatchTime = currentTime,
            TodayParticipateMatchNum = 10,
            TodayAwardGetStatusList = new List<int> { 1, 1, 1 },
            TodayWinNum = 50,
            TodayWinAwardGetStatusList = new List<int> { 1, 1, 1, 1, 1 },
            WatchedADAddParticipateTime = true,
            GetJieSuanAward = true,
            AddedDaBiParticipateNum = 5,
            SingleMatchDataList = new List<SingleMatchData>(),
            EnrolledSingleMatchData = null,
            CurMatchData = null
        };

        // 比赛记录
        for (int i = 1; i <= 5; i++)
        {
            SingleMatchData singleMatch = new SingleMatchData
            {
                SettingId = 10000 + i,
                ParticipateNum = 100,
                ChampionNum = 20
            };
            matchData.SingleMatchDataList.Add(singleMatch);
        }

        return matchData;
    }

    private ZongMenData CreateZongMenData()
    {
        ZongMenData zongMenData = new ZongMenData
        {
            ZongMenName = "测试宗门",
            ZongMenLevel = 9999,
            CurRankLevel = 10,
            CurStar = 5,
            TheR = 99999999,
            TiliLimit = 9999,
            SendFarmNumLimitAddNum = 10,
            ChangeNameNum = 0
        };

        return zongMenData;
    }

    private ZongMenProduceData CreateZongMenProduceData()
    {
        ZongMenProduceData produceData = new ZongMenProduceData
        {
            SingleZongMenProduceDataList = new List<SingleZongMenProduceData>(),
            TotalStudentNum = 9999,
            FreeStudentNum = 10
        };

        // 生产项目
        for (int i = 1; i <= 5; i++)
        {
            SingleZongMenProduceData produce = new SingleZongMenProduceData
            {
                SettingId = 11000 + i,
                CurLevel = 9999,
                CurStudentNum = 5,
                CurStudentLimit = 9999,
                CurProductLimit = 99999999
            };
            produceData.SingleZongMenProduceDataList.Add(produce);
        }

        return produceData;
    }

    private AllSkillData CreateSkillData()
    {
        AllSkillData skillData = new AllSkillData
        {
            skillList = new List<SingleSkillData>(),
            unlockedTypeList = new List<int> { 1, 2, 3, 4, 5 },
            unlockedSkillPos = 6,
            equippedSkillIdList = new List<int>(),
            lastUseSkillIndex = 0
        };

        // 创建技能
        for (int i = 1; i <= 20; i++)
        {
            SingleSkillData skill = new SingleSkillData
            {
                skillId = 12000 + i,
                skillLevel = 9999,
                damagePercentList = new List<float> { 100f, 150f, 200f },
                isEquipped = i <= 6,
                cd = 0,
                show = true,
                yuanSuType = (YuanSuType)(i % 5 + 1)
            };

            if (skill.isEquipped)
            {
                skillData.equippedSkillIdList.Add(skill.skillId);
            }

            skillData.skillList.Add(skill);
        }

        return skillData;
    }

    private NewGuideData CreateNewGuideData()
    {
        NewGuideData guideData = new NewGuideData
        {
            finishedGuideIdList = new List<int>(),
            IdList = new List<int>(),
            AccomplishStatus = new List<int>(),
            curGuideId = 0,
            curGuideStep = 0
        };

        // 完成所有新手引导
        for (int i = 1; i <= 50; i++)
        {
            guideData.finishedGuideIdList.Add(i);
            guideData.IdList.Add(i);
            guideData.AccomplishStatus.Add(2); // 已完成
        }

        return guideData;
    }

    private SceneData CreateSceneData()
    {
        SceneData sceneData = new SceneData
        {
            CurSceneType = 1, // 主场景
            LastSceneType = 1
        };

        return sceneData;
    }

    private TeamData CreateTeamData()
    {
        TeamData teamData = new TeamData
        {
            TeamList1 = new List<ulong>(),
            TeamList2 = new List<ulong>()
        };

        // 队伍1：主角 + 5个弟子
        teamData.TeamList1.Add(1000001); // 主角
        for (int i = 1; i <= 5; i++)
        {
            teamData.TeamList1.Add((ulong)(2000000 + i));
        }

        // 队伍2：6个弟子
        for (int i = 6; i <= 11; i++)
        {
            teamData.TeamList2.Add((ulong)(2000000 + i));
        }

        return teamData;
    }

    private AllGuideBookData CreateGuideBookData()
    {
        AllGuideBookData guideBookData = new AllGuideBookData
        {
            singleGuideBookTaskDataList = new List<SingleGuideBookTaskData>(),
            singleChapterList = new List<SingleChapterGuideBookData>()
        };

        // 冒险手札任务
        for (int i = 1; i <= 100; i++)
        {
            SingleGuideBookTaskData task = new SingleGuideBookTaskData
            {
                settingId = 13000 + i,
                accomplishStatus = 2,
                curProgress = 100
            };
            guideBookData.singleGuideBookTaskDataList.Add(task);
        }

        // 章节
        for (int chapter = 1; chapter <= 10; chapter++)
        {
            SingleChapterGuideBookData chapterData = new SingleChapterGuideBookData
            {
                chapter = chapter,
                processAccomplishStatus = new List<int>(),
                reveal = true
            };

            for (int i = 1; i <= 10; i++)
            {
                chapterData.processAccomplishStatus.Add(2);
            }

            guideBookData.singleChapterList.Add(chapterData);
        }

        return guideBookData;
    }

    private AllDailyTaskData CreateDailyTaskData()
    {
        AllDailyTaskData dailyTaskData = new AllDailyTaskData
        {
            dailyTaskList = new List<SingleDailyTaskData>(),
            curActiveNum = 100,
            activeAwardGetStatusList = new List<int>(),
            lastBrushTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        // 日常任务
        for (int i = 1; i <= 10; i++)
        {
            SingleDailyTaskData task = new SingleDailyTaskData
            {
                settingId = 14000 + i,
                accomplishStatus = 2,
                curNum = 1
            };
            dailyTaskData.dailyTaskList.Add(task);
        }

        // 活跃度奖励
        for (int i = 1; i <= 5; i++)
        {
            dailyTaskData.activeAwardGetStatusList.Add(1); // 已领取
        }

        return dailyTaskData;
    }

    private AllShenYuanData CreateShenYuanData()
    {
        AllShenYuanData shenYuanData = new AllShenYuanData
        {
            curFloor = 9999,
            maxFloor = 9999,
            todayChallengeCount = 9999,
            lastChallengeTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ShenYuanList = new List<SingleShenYuanData>()
        };

        // 深渊层级
        for (int i = 1; i <= 9999; i++)
        {
            SingleShenYuanData level = new SingleShenYuanData
            {
                LevelId = i,
                IsCleared = true,
                Layer1EnemyList = new List<PeopleData>()
            };
            shenYuanData.ShenYuanList.Add(level);
        }

        return shenYuanData;
    }

    private ChouJiangData CreateChouJiangData()
    {
        ChouJiangData chouJiangData = new ChouJiangData
        {
            baoDi10Num = 10,
            baoDi50Num = 50,
            baoDi100Num = 100
        };

        return chouJiangData;
    }

    private void SaveGameInfo(GameInfo gameInfo)
    {
        string directory = Path.GetDirectoryName(savePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var settings = new ES3Settings(ES3.EncryptionType.AES, encryptionKey);
        ES3.Save<GameInfo>("GameData", gameInfo, savePath, settings);

        Debug.Log($"存档已保存到: {savePath}");
        Debug.Log($"存档大小: {new FileInfo(savePath).Length} 字节");
    }
}