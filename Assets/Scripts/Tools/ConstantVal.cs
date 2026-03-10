

using COSXML.Model.Bucket;
using Framework.Data;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConstantVal
{
    static UInt64 onlyId { get; set; }

    static UInt16 growId;//自增长

    //编辑器自动赋值id
    public static UInt64 editorId
    {
        get
        {
            //RoleManager.Instance._CurGameInfo.TheId++;
            //return RoleManager.Instance._CurGameInfo.TheId;
            growId++;
            onlyId = (UInt64)(CGameTime.Instance.GetTimeStamp() * 100000000 + growId * 1000 + UnityEngine.Random.Range(0, 1000));
            return onlyId;
        }
    }


    public static UInt64 SetId
    {
        get
        {
            RoleManager.Instance._CurGameInfo.TheId++;
            return RoleManager.Instance._CurGameInfo.TheId;
            //growId++;
            //onlyId = (UInt64)(CGameTime.Instance.GetTimeStamp() * 100000000 + growId * 1000 + UnityEngine.Random.Range(0, 1000));
            //return onlyId;
        }
    }
    /// <summary>
    /// 所有panel暂时放该文件夹
    /// </summary>
    public const string PanelPath = "UIPanel";
    public const string propertyIconFolderPath = "TestRes/Common/Property/";//属性icon文件夹
    public const string bigMapFolderPath = "TestRes/BigMap/";//大地图icon文件夹
    public const string actionSceneFolderPath = "TestRes/ActionScene/";//行动场景文件夹
    public const string verticalDrawFolderPath = "TestRes/Common/PeopleVerticalDraw/";//人物立绘文件夹
    public const string maleIcon = "icon_man";//男头像
    public const string femaleIcon = "icon_girl";//女头像

    public const string battleHitEffectPath = "TestRes/Battle/effect/BattleHitEffect";//受击
    public const string loseHPEffectPath = "TestRes/Battle/effect/LoseHPEffect";//掉血
    public const string critNumMaterialPath = "Font/newnumberCrit";//掉血数字
    public const string commonNumMaterialPath = "Font/newnumber 1";
    public const string icon_guangGaoPlayBtn = "img_guanggao_1";//广告播放按钮
    // public const string EquipIconPath = "Equip/";

    public const string ItemIconPath = "Item/";//物品
    public const string PeopleTouxiang = "Touxiang/";//角色头像

    public const string ShopIconPath = "Shop/";//商店物品

    public static Color32 color_choosed = new Color32(190, 201, 54, 255);

    public const int equipMakeTotalDay = 30;//戒指总共要做30天

    public const string baseEnemyPro = "10001|400|420$10002|0|1$10003|50|60$10004|50|60$10005|5|6$10006|20|21$10007|0|1$10008|0$10017|0$10018|0";//敌人基础属性

    //public const string baseLianGongStudentPro = "10001|400|420$10002|0|1$10003|50|60$10004|50|60$10005|5|6$10006|20|21";//练功弟子基础属性
    //public const string baseLianGongStudentPro_huang = "10001|400|420$10002|0|1$10003|50|60$10004|50|60$10005|5|6$10006|20|21";//练功弟子基础属性
    //public const string baseLianGongStudentPro_xuan = "10001|500|520$10002|0|1$10003|70|80$10004|70|80$10005|5|6$10006|20|21";//练功弟子基础属性
    //public const string baseLianGongStudentPro_di = "10001|600|620$10002|0|1$10003|60|70$10004|60|70$10005|5|6$10006|20|21";//练功弟子基础属性
    //public const string baseLianGongStudentPro_tian = "10001|700|720$10002|0|1$10003|70|80$10004|70|80$10005|5|6$10006|20|21";//练功弟子基础属性

    public const string baseBattleProperty = "10001|0$10004|0$10003|0$10007|0$10005|5$10006|30$10002|0$10008|0$10017|$10018|0";

    public const string baseEquipMakeStudentPro = "20001|40$20002|40";//炼器师基础属性
    public const string baseEquipMakeStudentPro_huang = "20001|40$20002|40";//炼器师基础属性
    public const string baseEquipMakeStudentPro_xuan = "20001|45$20002|45";//炼器师基础属性
    public const string baseEquipMakeStudentPro_di = "20001|50$20002|50";//炼器师基础属性
    public const string baseEquipMakeStudentPro_tian = "20001|55$20002|55";//炼器师基础属性

    public const string baseLianDanStudentPro = "30001|40$30002|40";//炼丹师基础属性
    public const string baseLianDanStudentPro_huang = "30001|40$30002|40";//炼丹师基础属性
    public const string baseLianDanStudentPro_xuan = "30001|45$30002|45";//炼丹师基础属性
    public const string baseLianDanStudentPro_di = "30001|50$30002|50";//炼丹师基础属性
    public const string baseLianDanStudentPro_tian = "30001|55$30002|55";//炼丹师基础属性

    public const string EquipmakeStudentUpgradeNum_huang = "4";//黄级炼器师升级加成
    public const string EquipmakeStudentUpgradeNum_xuan = "6";//玄级炼器师升级加成
    public const string EquipmakeStudentUpgradeNum_di = "8";//地级炼器师升级加成
    public const string EquipmakeStudentUpgradeNum_tian = "10";//天级炼器师升级加成

    public const string baseEquipMakeStudentProId = "20001|20002";//炼器师基础属性id
    public const string baseLianDanStudentProId = "30001|30002";//炼丹师基础属性id
    public const string baseLianGongStudentPro = "10001|666$10004|133$10003|55";//炼功师基础属性

    public const float workStudentInitProperty = 22;//工作弟子基础属性
    public const float workStudentPropertyPerLevelAdd = 2.4f;//工作弟子每级属性最少增加这么多 后续可以通过随机事件让他达到满 然后一些气运可以让他直接到满
    public const string LianGongStudentPropertyPerLevelAdd = "10001|100$10003|16$10004|16";//练功师属性增加 最少加这么多 根据属性质量有关

    public const string LianGongUpgradeAdd_huang = "10001|30$10003|10$10004|10";//练功师黄级属性增加
    public const string LianGongUpgradeAdd_xuan = "10001|40$10003|13$10004|13";//练功师玄级属性增加
    public const string LianGongUpgradeAdd_di = "10001|50$10003|16$10004|16";//练功师地级属性增加
    public const string LianGongUpgradeAdd_tian = "10001|60$10003|20$10004|20";//练功师天级属性增加



    public const string archiveFileName = "GameInfo.es3";

    public const string ReceiveLeafPath = "Train/";//收集树叶玩法
    public const string matFolderPath = "Material/";//材质路径

    public const string mat_faGuang = "Material/UI_FaGuang";//发光材质

    public const string skillFolderPath = "Skill/";//技能图标

    public const string newGuideCanvasPath = "NewGuide/NewGuideCanvas";
    public const string guideSlideCanvasPath = "NewGuide/GuideSlideTalentCanvas";
    public const string resCommonPath = "Common/";
    public const string fontDataPath = "Assets/Resources/Font/FontData.asset";//字体配置文件路径
   
    public const int cruitStudentNeedLingShiNum = 0;//招募弟子需要的灵石数量
    public const int studentRent = 100;//弟子每月花销
    //public const string mat_burn=""
    //public const string big_itemGreenBgName = "ui 17";//绿
    //public const string big_itemBlueBgName = "ui 14";//蓝
    //public const string big_itemPurpleBgName = "ui 16";//紫
    //public const string big_itemOrangeBgName = "ui 15";//橙

    public const string sprt_shopBtnOn = "btn_anniu_shop2";//坊市点开
    public const string sprt_shopBtnOff = "btn_anniu_shop1";//坊市未点开


    public const string itemGreenBgName = "img_zhuangbei_ui_1";//绿
    public const string itemBlueBgName = "img_zhuangbei_ui_2";//蓝
    public const string itemPurpleBgName = "img_zhuangbei_ui_3";//紫
    public const string itemOrangeBgName = "img_zhuangbei_ui_4";//橙
    public const string itemGoldBgName = "img_zhuangbei_ui_5";//金

    public const string specialPortraitFolderPath = "Face/SpecialPortrait/";//特殊立绘路径
    public const string bing = "4";
    public const string huo = "2";
    public const string lei = "3";
    public const string ying = "6";
    public const string guang = "5";
    public const string shui = "1";


    public const string allNPCPath = "NPCs/";//所有npc路径
    public const string allMapEventSOFolderPath = "MapEventSO/";//所有mapevent路径

    public const string femalePortraitPath = "Face/FemaleFace/";//女的立绘路径
    public const int femalehoufaNum = 6;//后发
    public const int femalezhongfaNum = 5;//中发
    public const int femalefaceNum = 5;//脸
    public const int femalebiziNum = 9;//鼻子
    public const int femalemeimaoNum = 10;//眉毛
    public const int femaleeyeNum = 14;//眼睛
    public const int femalezuiNum = 19;//嘴
    public const int femalebodyNum = 12;//身体
    public const int femalelianshiNum = 18;//脸饰
    public const int femaleqianfaNum = 6;//前发


    public const string malePortraitPath = "Face/MaleFace/";//男的立绘路径
    public const int malehoufaNum = 8;//后发
    public const int malezhongfaNum = 5;//中发
    public const int malefaceNum = 6;//脸
    public const int malebiziNum = 5;//鼻子
    public const int malemeimaoNum = 5;//眉毛
    public const int maleeyeNum = 11;//眼睛
    public const int malezuiNum = 5;//嘴 
    public const int malebodyNum = 7;//身体

    public const int malelianshiNum = 11;//脸饰
    public const int maleqianfaNum = 7;//前发

    public const string houfaPath = "houfa/";//后发
    public const string zhongfaPath = "zhongfa/";//中发
    public const string facePath = "face/";//脸
    public const string biziPath = "bizi/";//鼻子
    public const string meimaoPath = "meimao/";//眉毛
    public const string eyePath = "eye/";//眼睛
    public const string zuiPath = "zui/";//嘴
    public const string bodyPath = "body/";//身体
    public const string lianshiPath = "lianshi/";//脸饰品
    public const string qianfaPath = "qianfa/";//前发

    public const int allFarmNum = 55;//暂定多少个farm

    public const int quanLiCost = 5;//全力生产需要源力

    public const string smallPeoplePath = "SmallPeople/";//npc小人路径
    public const string yunHaiZongZaYiSmallPeople1 = "img_npcyunhaizzayi";//云海宗杂役小人
    public const string yunHaiZongZaYiSmallPeople2 = "img_npcyunhaizzayi2";//云海宗杂役小人
    public const string smallPeopleMale = "img_dizinpc";

    public const string mountainUIPath = "Mountain/farm/";
    public const string bg_lingshu_5 = "bg_lingshu_5";


    public const string battlePeoplePath = "People/";//战斗用小人
    #region 天赋觉醒特效
    public const string talentTestPath = "TianFuJueXing/";//天赋觉醒路径
    public const string talent_green_fly = "green_fly";//天赋觉醒飞行特效
    public const string talent_blue_fly = "blue_fly";//天赋觉醒飞行特效
    public const string talent_purple_fly = "purple_fly";//天赋觉醒飞行特效
    public const string talent_orange_fly = "orange_fly";//天赋觉醒飞行特效
    public const string talent_gold_fly = "gold_fly";//天赋觉醒飞行特效

    public const string talent_green_boom = "green_boom";//天赋觉醒爆炸特效
    public const string talent_blue_boom = "blue_boom";//天赋觉醒爆炸特效
    public const string talent_purple_boom = "purple_boom";//天赋觉醒爆炸特效
    public const string talent_orange_boom = "orange_boom";//天赋觉醒爆炸特效
    public const string talent_gold_boom = "gold_boom";//天赋觉醒爆炸特效
    #endregion

    public const string audioFolderPath = "Audio/";//声音

    public const string audio_clickMai = "clickMai";//点脉
    public const string audio_electronic = "electronic";//电流声

    public const int eventOffset = 205;//事件事件间的举例

    public const string mapFolderPath = "Map/";//地图文件夹
    public const string sprt_battleLevel = "img_guaiwu3";
    public const string sprt_jingYingBattleLevel = "img_guaiwu2";
    public const string sprt_bossBattleLevel = "img_guaiwu4";
    public const string sprt_zhongZhuanZhanLevel = "img_guaiwu5";

    public const string mapEventWenHao = "Map/img_mijwenhao";//问号

    public const string whiteDuiHuaKuang = "Common/bg_duihuak";//对话框
    public const string blueDuiHuaKuang = "Common/bg_duihuak_zhangmen";

    public const string liLianFolderPath = "LiLian/";//历练文件夹
    public const string liLianMaleSke = "man_juese1_yujianfeixing_SkeletonData";//历练男角色
    public const string liLianFemaleSke = "woman_SkeletonData";//历练女角色

    public const string guankStatus = "guankaStatusImg/";

    public static Color32 color_fan = new Color32(0, 255, 58, 255);
    public static Color32 color_huang = new Color32(0, 238, 255, 255);
    public static Color32 color_xuan = new Color32(178, 0, 255, 255);
    public static Color32 color_di = new Color32(255, 87, 0, 255);
    public static Color32 color_tian = new Color32(255, 220, 0, 255);

    public static Color32 color_blackfan = new Color32(14, 73, 80, 255);
    public static Color32 color_blackhuang = new Color32(0, 19, 48, 255);
    public static Color32 color_blackxuan = new Color32(39, 0, 60, 255);
    public static Color32 color_blackdi = new Color32(73, 12, 0, 255);
    public static Color32 color_blacktian = new Color32(116, 68, 20, 255);

    public const int exploreSpeed = 1;//探索速度 越小越快

    public const string commonBattleLevelSke = "jian_1_SkeletonData";//普通关卡点
    public const string jingYingBattleLevelSke = "jian_2_SkeletonData";//精英关卡点
    public const string bossBattleLevelSke = "jian_3_SkeletonData";//boss关卡点

    public const int tiliReviveMinute = 5;//体力恢复5分钟

    public const int tianJingReviveTiliNum = 120;//天晶获取体力量

    public const int dayliWatchTiliLimit = 5;//看广告获取体力的每日限制
    public const int taoFaNeedTiLi = 20;//讨伐需要多少体力
    public const int miJingExploreNeedTiLi = 20;//秘境探索需要多少体力
    public const int lieXiNeedYuanLi = 30;//裂隙需要多少体力

    public const int maxOfflineIncomeHour = 6;//离线收益最多几小时
    public const float offlineIncomeRate = 0.02f;//离线收益比例

    public const int adOfflineIncomeBeiLv = 2;//看广告离线收益倍率

    public const int talentTestMaxADNum = 3;//天赋测试最多看几次广告
    //public const string baseBattleProperty = "10001|0$10004|0$10003|0$10005|0$10006|0$10007|0";

    public const int levelBattleTiliConsume = 5;//闯关体力消耗
    public const int fixedlevelBattleTiliConsume = 5;//闯关体力消耗
    public const int mapEventBattleTiliConsume = 5;//闯关体力消耗

    public const int farmBuildMaxQueueNum = 3;//建造最大队列

    public const int baseMatchRongYuBi = 5;//比赛最少获得荣誉币
    public const int winMatchRongYuBi = 10;//比赛赢一场获得荣誉币

    public const int maxMatchParticipatePerDay = 5;//一天最多几次比赛

    public const int maxMatchWinNumPerDay = 3;//一天最多几次比赛奖励

    public const int adAddParticipateMatchNum = 5;//广告刷新参赛几次

    public const int bigBoxRate = 5;//大宝箱概率千分之五

    public const int maxStudentRecruitNumPerDay = 20;//每天最大招募弟子次数

    public const int hourBeforeNextStudent = 8;//几小时可以再次招募弟子

    public const int maxCanEatTiaoXiDanNum = 2;//最大可用调息丹

    public static Vector2Int rdmEventTimeRange = new Vector2Int(24, 36);

    public const string matchDailyAward_1win = "260001|20$152001|2";
    public const string matchDailyAward_2win = "260001|30$230001|10$152001|3";
    public const string matchDailyAward_3win = "260001|50$230001|20$152001|5";


    public const string defaultPortraitName = "img_yingzi";

    public const string battleBuffGoodBg = "Battle/img_zhandou_g";
    public const string battleBuffBadBg = "Battle/img_zhandou_b";
    public const string biaoQingBaoPath = "Chat/";
    public const string MaiDianEvent_GuideTask = "新手引导";

    public const string commonTouXiangKuangDi = "bg_touxiang1";
    public const string commonTouXiangKuang = "bg_touxiang2";

    public const string monsterJiaoFolderPath = "Audio/monsterJiao/";

    public const string haoGanXin1 = "img_lilian_xing1";
    public const string haoGanXin2 = "img_lilian_xing2";
    public const string haoGanXin3 = "img_lilian_xing3";
    public const string chouHenIcon = "img_lilian_chouheng";

    public const string guangGaoLingShopBrushCost = "10001|10000$365001|10,10001|50000$365001|20,10001|100000$365001|40,365001|80,365001|160";//广告令刷新消耗

    public const int maxADNumPerDay = 70;//一天最大广告次数
    public const int adTimeOffset = 10;//看广告必须间隔10秒
    public static int adNeedMinTime = 5;//
    //public const int minADTime=10

    public const int BaoZhaBase = 375;//爆炸伤害基数
    public const int LeiQiangBase = 100;//雷枪伤害基数

    public const int BaoZhaEnergyLose = 16;//爆炸掉多少能量
    public const int CiBaoEnergyLose = 26;//磁暴掉多少能量
    public const int XueMaiMaxLevel = 70;//血脉最高等级

    public const int littleHaoGanDu = 30;//小好感
    public const int middleHaoGanDu = 60;//中好感
    public const int bigHaoGanDu = 90;//大好感
    public const int fullHaoGanDu = 100;//满好感

    public const int littleHaoGanXieZhanCD = 5;//小好感携战cd
    public const int middleHaoGanXieZhanCD = 4;//中好感携战cd
    public const int bigHaoGanXieZhanCD = 3;//大好感度携战cd

    public const float littleHaoGanXieZhanAtkRate = 0.5f;//小好感携战数值
    public const float middleHaoGanXieZhanAtkRate = 0.7f;//中好感携战cd
    public const float bigHaoGanXieZhanAtkRate = 0.9f;//大好感度携战cd
    public const float fullHaoGanXieZhanAtkRate = 1f;//大好感度携战cd

    public const int maxLiLianTimePerDay = 1;//一天最多参加几次历练

    public const int moonCard1OnceSend = 300;//月卡1一次性天晶
    public const int moonCard1PerDaySend = 50;//月卡1每天赠送
    public const int moonCard2OnceSend = 250;//月卡2赠送
    public const int moonCard3OnceSend = 60;//月卡3赠送

    public const int maxMatchTianJingBrushNum = 5;//大比最多刷几次

    public const int matchTianJingBrushAddNum = 5;//刷一次大比增加几次次数

    public const int dailyEatXiuWeiDanLimit = 175;//每日服丹上限

    public const string dailyTaskProcessAward = "335001|5$152001|1$365001|5,335001|10$152001|2$365001|5,335001|20$152001|3$365001|5,335001|25$152001|4$365001|5,335001|40$152001|10$270001|1$270002|1$270003|1$270004|1$270005|1$365001|10";//每日任务奖励

    public const string equipIntenseAdd_Fan = "10003|189$10004|189$10001|975$10005|5$10006|10$10002|6$10008|25";
    public const string equipIntenseAdd_Huang = "10003|315$10004|315$10001|1581$10005|10$10006|20$10002|12$10008|31";
    public const string equipIntenseAdd_Xuan = "10003|432$10004|432$10001|2178$10005|15$10006|30$10002|18$10008|39";
    public const string equipIntenseAdd_Di = "10003|555$10004|555$10001|2781$10005|20$10006|40$10002|25$10008|48";
    public const string equipIntenseAdd_Tian = "10003|669$10004|669$10001|3375$10005|24$10006|48$10002|32$10008|61";

    public const string liLianUltraPossibleItem = "10001|2|70$10001|80|20$81001|5|1$82001|5|1$83001|5|1$84001|5|1$85001|5|1$111001|5|1$112001|5|1$113001|5|1$114001|5|1$115001|5|1$152001|1|1";

    public const int maxParticipateXuanXiuNumPerDay=2;//每天最大参加选秀次数

    #region 比赛敌人数据
    public const int matchNPC_initHP = 6000;
    public const int matchNPC_finalHp = 161928;
    public const int matchNPC_initDefense = 1200;
    public const int matchNPC_finalDefense = 32385;
    public const int matchNPC_initAttack = 1200;
    public const int matchNPC_finalAttack = 32385;
    public const int matchNPC_initCritRate = 5;
    public const int matchNPC_finalCritRate = 95;
    public const int matchNPC_initCritNum = 10;
    public const int matchNPC_finalCritNum = 450;
    public const int matchNPC_initMPSpeed = 0;
    public const int matchNPC_finalMPSpeed = 97;

    public const int matchNPC_initSkillLevel1 = 1;
    public const int matchNPC_finalSkillLevel1 = 50;

    public const int matchNPC_initSkillLevel2 = 1;
    public const int matchNPC_finalSkillLevel2 = 50;

    public const int matchNPC_initXueMaiLevel = 0;
    public const int matchNPC_finalXueMaiLevel = 0;

    public const int matchNPC_initTrainIndex = 0;
    public const int matchNPC_finalTrainIndex = 199;

    public const int matchNPC_initProAdd = 0; 
    public const int matchNPC_finalProAdd = 0;


    #endregion
    public const string mm = "37036568";

    public const string remoteAddress = "http://101.35.87.105:8888/";

    public const int maxMailStayDay = 7;//邮件保留最大天数

    public static Color availableGreen = new Color32(72, 114, 97, 255);
    public static Color unAvailableRed = new Color32(111, 15, 0, 255);

    public const string artResPath = "ArtRes/";

    public const int maxUploadArchiveNum=3;//每天最多上传次数

    public const int hongMengLingShuEfficientAdd = 2;//鸿蒙灵树效率+
    public const int juLingZhenAdd = 10;//聚灵阵灵气+
    public const int hongMengKuangLingEfficientAdd = 2;//鸿蒙矿灵效率+
    public const int biXieJinLianDeBaiFen = 20;//辟邪金莲减少元神受损时间
    public const int youBiBaiLianAddBaiFen = 1;//幽碧白莲增加丹修为
    public const int juBaoPenEfficientAdd = 1;//聚宝盆产量+
    public const int hongMengTianDaoEfficientAdd = 2;//鸿蒙天道效率+
    public const int shangGuPiXiuEfficientAdd = 2;//上古貔貅产量+
    public const int qianKunJingAdd = 5;//乾坤井离线增加
    public const int jingShiQingLianAddBaiFen = 3;//净室青莲增加丹修为
    public const int jieZiXuMiAdd = 10;//芥子须弥离线收益增加
    public const int xianZhuJinChanEfficientAdd = 3;//衔珠金蟾产量+
    public const int hongMengTianDiEfficientAdd = 5;//鸿蒙天地效果
    public const int caiYuanGunGun = 10;//财源滚滚
    public const int sanLianBingDiDeBaiFen = 20;//三莲并蒂


    public const string shouChongAward = "215001|1$381001|1";//首充奖励

    /// <summary>
    /// 增加大比次数需要天晶
    /// </summary>
    public static int MatchAddCountNeedTianJing
    {
        get
        {
            if (RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum <= 0)
            {
                return 100;
            }
            else if (RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum <= 1)
            {
                return 150;
            }
            else if (RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum <= 2)
            {
                return 200;
            }
            else if (RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum <= 3)
            {
                return 280;
            }
            else if (RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum <= 4)
            {
                return 360;
            }
            return 360;
        }
    }


    public static int ReviveTiLiNeedTianJing
    {
        get
        {
            if (RoleManager.Instance._CurGameInfo.timeData.TodayADTiliNum <= 0)
            {
                return 40;
            }
            else if (RoleManager.Instance._CurGameInfo.timeData.TodayADTiliNum <= 1)
            {
                return 60;
            }
            else if (RoleManager.Instance._CurGameInfo.timeData.TodayADTiliNum <= 2)
            {
                return 100;
            }
            else if (RoleManager.Instance._CurGameInfo.timeData.TodayADTiliNum <= 3)
            {
                return 140;
            }
            else if (RoleManager.Instance._CurGameInfo.timeData.TodayADTiliNum <= 4)
            {
                return 180;
            }
            return 180;
        }
    }


    /// <summary>
    /// 弟子影响的圈
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static Sprite StudentInfluenceIcon(Quality quality)
    {
        return ResourceManager.Instance.GetObj<Sprite>(ItemIconPath + "img_quan" + (int)quality);
    }

    /// <summary>
    /// 通过元素得到念珠id
    /// </summary>
    /// <returns></returns>
    public static int NianZhuIdByYuanSu(YuanSuType yuanSu)
    {
        ItemIdType itemIdType = 0;
        switch (yuanSu)
        {
            case YuanSuType.Water:
                itemIdType = ItemIdType.FanJiShuiNianZhu;
                break;
            case YuanSuType.Fire:
                itemIdType = ItemIdType.FanJiHuoNianZhu;
                break;
            case YuanSuType.Storm:
                itemIdType = ItemIdType.FanJiLeiNianZhu;
                break;
            case YuanSuType.Ice:
                itemIdType = ItemIdType.FanJiBingNianZhu;
                break;
            case YuanSuType.Light:
                itemIdType = ItemIdType.FanJiYangNianZhu;
                break;
            case YuanSuType.Dark:
                itemIdType = ItemIdType.FanJiYinNianZhu;
                break;
        }
        return (int)itemIdType;
    }

    /// <summary>
    /// 新手选择元素
    /// </summary>
    /// <returns></returns>
    public static Sprite NewGuideChooseYuanSuTxtImg()
    {
        string newGuidePath = "NewGuide/";
        return ResourceManager.Instance.GetObj<Sprite>(newGuidePath + "img_xuemai1");

    }

 
    public static Sprite HaoGanBg()
    {
        return ResourceManager.Instance.GetObj<Sprite>(liLianFolderPath + "img_haogandu");
    }
    public static Sprite ChouHenBg()
    {
        return ResourceManager.Instance.GetObj<Sprite>(liLianFolderPath + "img_chouhengzhi");

    }

    /// <summary>
    /// 好感度icon
    /// </summary>
    /// <returns></returns>
    public static Sprite HaoGanIcon(int haoGan,Gender gender1,Gender gender2)
    {
        string iconName = "img_haogan";
        int index = 0;
        if (haoGan >= 0)
        {
            if (haoGan < 80)
            {
                index = haoGan / 10;

            }
            else
            {
                if (haoGan >= 80 && haoGan < 90)
                {
                    if (gender1 == gender2)
                    {
                        index = 13;
                    }
                    else
                    {
                        index = 8;
                    }
                }
                else if (haoGan >= 90 && haoGan < 100)
                {
                    if (gender1 == gender2)
                    {
                        index = 12;
                    }
                    else
                    {
                        index = 10;
                    }
                }else if (haoGan == 100)
                {
                    if (gender1 == gender2)
                    {
                        index = 14;
                    }
                    else
                    {
                        index = 11;
                    }
                }
            }
        }
        else
        {
            if (haoGan > -30)
            {
                index = 0;
            }else if (haoGan <= -30 && haoGan > -60)
            {
                index = 16;
            }else if (haoGan <= -60 && haoGan > -90)
            {
                index = 17;
            }
            else if (haoGan <= -90 && haoGan > -100)
            {
                index = 18;
            }
        }
        return ResourceManager.Instance.GetObj<Sprite>(ConstantVal.liLianFolderPath + iconName + index);
    }

    /// <summary>
    /// 元素描述图片
    /// </summary>
    /// <returns></returns>
    public static Sprite YuanSuDesImg(YuanSuType yuanSuType)
    {
        string newGuidePath = "NewGuide/";
        switch (yuanSuType)
        {
            case YuanSuType.Water:
                return ResourceManager.Instance.GetObj<Sprite>( newGuidePath + "img_xuemai_shui");
            case YuanSuType.Fire:
                return ResourceManager.Instance.GetObj<Sprite>(newGuidePath + "img_xuemai_huo");
            case YuanSuType.Ice:
                return ResourceManager.Instance.GetObj<Sprite>(newGuidePath + "img_xuemai_bing");
            case YuanSuType.Storm:
                return ResourceManager.Instance.GetObj<Sprite>(newGuidePath + "img_xuemai_lei");
        }
        return null;
    }

    /// <summary>
    /// 元素icon
    /// </summary>
    /// <returns></returns>
    public static Sprite YuanSuIcon(YuanSuType yuanSuType)
    {
        string folderPath = "YuanSu/";
        string iconName = "";
        switch (yuanSuType)
        {
            case YuanSuType.Water:
                iconName = "img_yuansu_shui1";
                break;
            case YuanSuType.Fire:
                iconName = "img_yuansu_huo1";
                break;
            case YuanSuType.Storm:
                iconName = "img_yuansu_lei1";
                break;
            case YuanSuType.Ice:
                iconName = "img_yuansu_bing1";
                break;
            case YuanSuType.Light:
                iconName = "img_yuansu_yang1";
                break;
            case YuanSuType.Dark:
                iconName = "img_yuansu_ying1";
                break;

        }
        iconName += "_1";
        return ResourceManager.Instance.GetObj<Sprite>(folderPath + iconName);
    }

    public static Sprite BarIcon(YuanSuType yuanSuType)
    {
        string folderPath = "YuanSu/";
        string iconName = "";
        switch (yuanSuType)
        {
            case YuanSuType.Water:
                iconName = "img_yuansu_shui_1";
                break;
            case YuanSuType.Fire:
                iconName = "img_yuansu_huo_1";
                break;
            case YuanSuType.Storm:
                iconName = "img_yuansu_lei_1";
                break;
            case YuanSuType.Ice:
                iconName = "img_yuansu_bing_1";
                break;
            case YuanSuType.Light:
                iconName = "img_yuansu_yang_1";
                break;
            case YuanSuType.Dark:
                iconName = "img_yuansu_ying_1";
                break;

        }
        return ResourceManager.Instance.GetObj<Sprite>(folderPath + iconName);
    }
    /// <summary>
    /// 战斗中的元素icon
    /// </summary>
    /// <returns></returns>
    public static Sprite YuanSuInBattleIcon(YuanSuType yuanSuType)
    {
        string folderPath = "YuanSu/";
        string iconName = "";
        switch (yuanSuType)
        {
            case YuanSuType.Water:
                iconName = "img_yuansu_shui";
                break;
            case YuanSuType.Fire:
                iconName = "img_yuansu_huo";
                break;
            case YuanSuType.Storm:
                iconName = "img_yuansu_lei";
                break;
            case YuanSuType.Ice:
                iconName = "img_yuansu_bing";
                break;
            case YuanSuType.Light:
                iconName = "img_yuansu_yang";
                break;
            case YuanSuType.Dark:
                iconName = "img_yuansu_ying";
                break;
  
        }
        return ResourceManager.Instance.GetObj<Sprite>(folderPath + iconName);
    }

    /// <summary>
    /// 通过升级物品id得到血脉类型
    /// </summary>
    /// <returns></returns>
    public static XueMaiType GetXueMaiTypeByUpgradeItemId(int id)
    {
        foreach (var xueMaiUpgradeSetting in DataTable.table.TbXueMaiUpgrade.DataList)
        {
            List<List<List<int>>> itemNeedParam = CommonUtil.SplitThreeCfg(xueMaiUpgradeSetting.NeedItem);
            List<List<int>> singleParam= itemNeedParam[0];
            for(int j = 0; j < singleParam.Count; j++)
            {
                List<int> single = singleParam[j];
                int theId = single[0];
                if (theId == id)
                {
                    return (XueMaiType)xueMaiUpgradeSetting.Type.ToInt32();
                }
            }
        }
        return XueMaiType.None;
    }

    /// <summary>
    /// 通过稀有度得到装备强化的属性
    /// </summary>
    /// <returns></returns>
    public static string EquipIntenseAddByRarity(Rarity rarity)
    {
        string res = "";
        switch (rarity) 
        {
            case Rarity.Fan:
                res= equipIntenseAdd_Fan;
                break;
            case Rarity.Huang:
                res = equipIntenseAdd_Huang;
                break;
            case Rarity.Xuan:
                res = equipIntenseAdd_Xuan;
                break;
            case Rarity.Di:
                res = equipIntenseAdd_Di;
                break;
            case Rarity.Tian:
                res = equipIntenseAdd_Tian;
                break;
        }
        return res;
    }

    /// <summary>
    /// 男性格叫
    /// </summary>
    /// <param name="xingGe"></param>
    /// <returns></returns>
    public static string MaleXingGeJiaoFolderPath(int xingGe)
    {
        string str = "Audio/nanJiao/nanJiao" + xingGe;
        if (ResourceManager.Instance.GetObj<AudioClip>(str+"/1")!=null)
        {

        }
        else
        {
              str = "Audio/nanJiao/nanJiao1";

        }
        return str;

    }


    /// <summary>
    /// 女性格叫
    /// </summary>
    /// <param name="xingGe"></param>
    /// <returns></returns>
    public static string FeMaleXingGeJiaoFolderPath(int xingGe)
    {
        string str = "Audio/nvJiao/nvJiao" + xingGe;
        if (ResourceManager.Instance.GetObj<AudioClip>(str + "/1") != null)
        {

        }
        else
        {
            str = "Audio/nvJiao/nvJiao1";

        }
        return str;

    }


    /// <summary>
    /// 通过元素得到能力
    /// </summary>
    /// <param name="yuanSuType"></param>
    /// <returns></returns>
    public static string Guide_AbilityNameByYuanSu(YuanSuType yuanSuType)
    {
        string str = "";
        switch (yuanSuType)
        {
            case YuanSuType.Water:
                str = "流水";
                break;
            case YuanSuType.Fire:
                str = "烈焰";
                break;
            case YuanSuType.Storm:
                str = "惊雷";
                break;
            case YuanSuType.Ice:
                str = "冰霜";
                break;
        }
        return str;
    }

    /// <summary>
    /// 正面buff池
    /// </summary>
    public static List<BattleBuffIdType> goodBuffPoolList = new List<BattleBuffIdType>()
    {
        BattleBuffIdType.LiLiang1,
     
        BattleBuffIdType.YuHe,
        BattleBuffIdType.TieBi1,
     
        BattleBuffIdType.GuanZhu,
        BattleBuffIdType.GuoJue,
        BattleBuffIdType.HaoRan1,
       
        BattleBuffIdType.HaoRan3,
        BattleBuffIdType.JiZhong,
        BattleBuffIdType.ZhenFen1,
        

    };


    /// <summary>
    /// 负面buff池
    /// </summary>
    public static List<BattleBuffIdType> badBuffPoolList = new List<BattleBuffIdType>()
    {
        //BattleBuffIdType.ZhuoShao,


        BattleBuffIdType.Piruo1,
        BattleBuffIdType.PoFang1,
        BattleBuffIdType.FengMai,
        BattleBuffIdType.MaBi,
        BattleBuffIdType.ZhangMu,
        BattleBuffIdType.GuaDuan,
        BattleBuffIdType.JinLiao,
        BattleBuffIdType.ChiHuan2,
        BattleBuffIdType.LiJie1,
        BattleBuffIdType.LuanShen,

    };


    /// <summary>
    /// 元素名
    /// </summary>
    /// <returns></returns>
    public static string YuanSuName(YuanSuType yuanSuType)
    {
        switch (yuanSuType)
        {
            case YuanSuType.Dark:
                return "暗";
            case YuanSuType.Light:
                return "光";
            case YuanSuType.Fire:
                return "火";
            case YuanSuType.Ice:
                return "冰";
            case YuanSuType.Water:
                return "水";
            case YuanSuType.Storm:
                return "雷";
        }
        return "";
    }
    /// <summary>
    /// 元素颜色
    /// </summary>
    /// <returns></returns>
    public static Color YuanSuColor(YuanSuType yuanSuType)
    {
        switch (yuanSuType)
        {
            case YuanSuType.Dark:
                return Color.black;
            case YuanSuType.Light:
                return Color.white;
            case YuanSuType.Fire:
                return Color.red;
            case YuanSuType.Ice:
                return Color.blue;
            case YuanSuType.Water:
                return Color.green;
            case YuanSuType.Storm:
                return new Color32(255,0,255,255);
        }
        return Color.white;
    }
    /// <summary>
    /// 元素反应名称
    /// </summary>
    /// <returns></returns>
    public static string YuanSuReactionName(ReactionType reactionType)
    {
        // switch (reactionType)
        // {
        //     case ReactionType.BaoZheng:
        //         return "爆蒸";
        //     case ReactionType.DianLiu:
        //         return "电流";
        //     case ReactionType.DongJiang:
        //         return "冻僵";
        //     case ReactionType.BaoZha:
        //         return "爆炸";
        //     case ReactionType.RongHua:
        //         return "融化";
        //     case ReactionType.LeiQiang:
        //         return "雷枪";
        //     case ReactionType.ChaoShi:
        //         return "潮湿";
        //     case ReactionType.ZhuoShao:
        //         return "灼烧";
        //     case ReactionType.BingLao:
        //         return "冰牢";
        //     case ReactionType.CiBao:
        //         return "磁暴";
        //     case ReactionType.CiFu:
        //         return "赐福";
        //     case ReactionType.XiongSha:
        //         return "凶煞";
        // }
        return "";
    }



    public static string TalentFunction(StudentTalent studentTalent)
    {
        string res = "";
        switch (studentTalent)
        {
            case StudentTalent.LianJing:
                res = "驻守炼丹房可提高炼制出高级丹药的概率";
                break;
            case StudentTalent.DuanZhao:
                res = "驻守炼器房可提高装备炼制，装备强化的优化程度，装备分解返还的材料数量";
                break;
            case StudentTalent.LianGong:
                res = "只有修武弟子可上阵队伍及派遣到秘境";
                break;
            case StudentTalent.CaiKuang:
                res = "驻守矿场可提高矿场产量";
                break;
            case StudentTalent.ChaoYao:
                res = "驻守灵树可提高灵树产量";
                break;
            case StudentTalent.JingWen:
                res = "驻守藏经阁可提高藏经阁产量";
                break;
            case StudentTalent.BaoShi:
                res = "驻守八卦炉可提高宝石的优化率";
                break;
            case StudentTalent.JingShang:
                res = "驻守灵田可提高灵田产量";
                break;
        }
        return res;
    }


    ///// <summary>
    ///// 爆发增益护盾攻速可学习的buff
    ///// </summary>
    //public static List<SkillIdType> canLingWuSkillIdList1 = new List<SkillIdType>
    //{
    //    SkillIdType.JiuTianXuanLeiJue,
    //     SkillIdType.LieHuoFenTian,
    //    SkillIdType.MiHuanWuZhang,
    //    SkillIdType.ZhiHuanGuangShu,
    //    SkillIdType.GuiTianYunZhang,
    //   SkillIdType.ZhenYuanChiYangJue,
    //   SkillIdType.LianTaiShouShenJue,
    //   SkillIdType.PianHongLingDongJue,//翩鸿灵动决
    //   SkillIdType.JinFengYuLu,
    //   SkillIdType.YueYaoBaHuang,
    //   SkillIdType.ShuangBao,
    //   SkillIdType.TianGangQiJin,
    //   SkillIdType.TianGangZhenYa,
    //   SkillIdType.GangQiChongJi,
    //  SkillIdType.RealLieHuoFenTian,//烈火
    //  SkillIdType.BiAnChiHun,
    //};
    ///// <summary>
    ///// 驱散集气定身削弱可学习的buff
    ///// </summary>
    //public static List<SkillIdType> canLingWuSkillIdList2 = new List<SkillIdType>
    //{
    //    SkillIdType.ShiHaiBaoDong,//识海暴动
    //    SkillIdType.ShenNianGongJi,//神念攻击
    //    SkillIdType.TaiShangJingShenJue,//太上净神决
    //    SkillIdType.SanHuaJuDingJue,//三花聚顶决
    //};

  

    /// <summary>
    /// 通过关卡等级得到关卡等级的str
    /// </summary>
    /// <returns></returns>
    public static int GetLevelByLevelStr(string levelStr)
    {
        string[] levelList =  levelStr.Split('-');
        if (levelList.Length <= 1)
            return 0;
        int logicLevel = levelList[0].ToInt32() * 1000 + levelList[1].ToInt32();
        return logicLevel;
    }
  
    /// <summary>
    /// 外向程度
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public static string SocialActivityName(int val)
    {
        //社恐
        if (val < 20)
        {
            return "社交恐惧";
        }else if (val < 40)
        {
            return "内向";
        }else if (val < 80)
        {
            return "外向";
        }
        else
        {
            return "社交牛逼";
        }
    }

    /// <summary>
    /// 奖励物品动画名
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    public static string awardItemAnimName(Rarity rarity)
    {
        string res = "";
        switch (rarity)
        {
            case Rarity.Tian:
                res = "tian";
                break;
            case Rarity.Di:
                res = "di";
                break;
            case Rarity.Xuan:
                res = "xuan";
                break;
            case Rarity.Huang:
                res = "huang";
                break;
            case Rarity.Fan:
                res = "fan";
                break;
        }
        return res;
    }

    /// <summary>
    /// 通过质量得到名字
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static string QualityName(int quality)
    {
        if (quality <= 0)
            quality = 1;
        switch (quality)
        {
            case 1:
                return "凡";
            case 2:
                return "黄";
            case 3:
                return "玄";
            case 4:
                return "地";
            case 5:
                return "天";
        }
        return "";
    }
    /// <summary>
    /// 通过质量得到名字
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static string QualityUIName(int quality)
    {
        if (quality <= 0)
            quality = 1;
        switch (quality)
        {
            case 1:
                return "img_icon_ui_f";
            case 2:
                return "img_icon_ui_c";
            case 3:
                return "img_icon_ui_b";
            case 4:
                return "img_icon_ui_a";
            case 5:
                return "img_icon_ui_s";
        }
        return "";
    }
    /// <summary>
    /// 排位段位名字
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static string MatchRankName(int rank)
    {
 
        return "";
    }

    /// <summary>
    /// 版本
    /// </summary>
    /// <returns></returns>
    public static int VersionNum(string versionName)
    {
        string[] arr= versionName.Split('.');
        int res = arr[0].ToInt32() * 1000 + arr[1].ToInt32() * 100 + arr[2].ToInt32()*10;
        return res;
    }

    /// <summary>
    /// 出现类型1的事件发生位置
    /// </summary>
    /// <returns></returns>
    public static List<int> Appear1EventPos(SceneType sceneType,int mapId)
    {
        List<int> res = new List<int>();
        if (sceneType == SceneType.FixedMap)
        {
            //云海宗
            if (mapId == 10000)
            {
                res.Add(3);
                res.Add(18);
                res.Add(40);
                res.Add(74);
                res.Add(77);
                res.Add(69);
                res.Add(56);

            }  //季狸国
            else if (mapId == 10001)
            {
                res.Add(21);
                res.Add(65);
                res.Add(83);
                res.Add(87);
                res.Add(78);
                res.Add(63);
                res.Add(43);
                res.Add(37);

            }
            //厌火国
            else if (mapId == 10002)
            {
                res.Add(0);
                res.Add(17);
                res.Add(60);
                res.Add(107);
                res.Add(113);
                res.Add(70);
                res.Add(54);
                res.Add(60);

            } 
            //白民国
            else if (mapId == 10003)
            {
                res.Add(0);
                res.Add(36);
                res.Add(39);
                res.Add(59);
                res.Add(62);
                res.Add(49);
                res.Add(83);
                res.Add(68);

            }
            //这里羽民国
            else if (mapId == 10004)
            {
                res.Add(19);
                res.Add(34);
                res.Add(35);
                res.Add(50);
                res.Add(70);
                res.Add(104);
                res.Add(110);
                res.Add(125);

            }
        }
        return res;
    }

    ///// <summary>
    ///// 天赋ui名
    ///// </summary>
    ///// <returns></returns>
    //public static string TalentAnimName(Rarity rarity)
    //{
    //   return  (int)rarity
    //}

    /// <summary>
    /// 通过稀有度得到加成
    /// </summary>
    /// <param name="rarity"></param>
    /// <returns></returns>
    public static float GetValAddByRarity(Rarity rarity)
    {
        return  (1 + ((int)rarity - 1) * 0.2f);
    }

    /// <summary>
    /// 存档文件夹
    /// </summary>
    /// <param name="archiveIndex"></param>
    /// <returns></returns>
    public static string GetArchiveSaveFolder(int archiveIndex)
    {
        return Application.persistentDataPath + "/New2Archives/" + "archive_" + archiveIndex;
    }


    public static string GetArchiveSavePath(int archiveIndex)
    {
        string ArchiveSavePath = Application.persistentDataPath + "/New2Archives/" + "archive_" + archiveIndex+"/"+ archiveFileName;


        return ArchiveSavePath;
    }
    /// <summary>
    /// 备份存档文件夹
    /// </summary>
    /// <param name="archiveIndex"></param>
    /// <returns></returns>
    public static string GetArchiveBeiFenSaveFolder(int archiveIndex)
    {
        return Application.persistentDataPath + "/New2BeiFenArchives/" + "archive_" + archiveIndex;
    }
    public static string GetArchiveBeiFenSavePath(int archiveIndex)
    {
        string ArchiveSavePath = Application.persistentDataPath + "/New2BeiFenArchives/" + "archive_" + archiveIndex + "/" + archiveFileName;


        return ArchiveSavePath;
    }

    /// <summary>
    /// 通过Panel名字获取路径
    /// </summary>
    /// <param name="panelName"></param>
    /// <returns></returns>
    public static string GetPanelPath(string panelName)
    {
        return PanelPath + "/" + panelName;
    }
    /// <summary>
    /// 下载的垃圾路径
    /// </summary>
    /// <returns></returns>
    public static string GetDownLoadRubbishPath()
    {
        return Application.persistentDataPath + "/Download";
    } 
    /// <summary>
      /// 下载的垃圾路径
      /// </summary>
      /// <returns></returns>
    public static string GetDownLoadRubbishPath2()
    {
        return Application.persistentDataPath + "/ksadsdk";
    }
    /// <summary>
    /// 获取文件在流目录的路径
    /// </summary>
    /// <returns></returns>
    public static string GetFileInStreamPath(string file)
    {
#if UNITY_IOS
        return "file://"+ Application.streamingAssetsPath + "/" + file;
#else
        return Application.streamingAssetsPath + "/" + file;
#endif
    }

    /// <summary>
    /// 获取文件在持久化目录的路径
    /// </summary>
    /// <returns></returns>
    public static string GetFileInPersistentPath(string file)
    {
        //#if UNITY_IOS
        // return "file://" + Application.persistentDataPath + "/" + file;
        //#else
        return Application.persistentDataPath + "/" + file;
        //#endif
    }

    /// <summary>
    /// 获取version的持久化目录
    /// </summary>
    /// <returns></returns>
    public static string GetVersionPersistentPath()
    {
        //#if UNITY_IOS
        return Application.persistentDataPath + "/theVersion.txt";

        //#else
        //return Application.persistentDataPath + "/theVersion.txt";
        //#endif
    }
    /// <summary>
    /// 获取version的流目录
    /// </summary>
    /// <returns></returns>
    public static string GetVersionStreamPath()
    {
#if UNITY_IOS
        return "file://" +Application.streamingAssetsPath + "/theVersion.txt";
#else
        return Application.streamingAssetsPath + "/theVersion.txt";
#endif
    }

    /// <summary>
    /// 获取特殊存档的流目录
    /// </summary>
    /// <returns></returns>
    public static string GetSpecialArchiveStreamPath(int index)
    {
#if UNITY_IOS
        return "file://" +Application.streamingAssetsPath + "/New2Archives/archive_" + index + "/GameInfo.es3";
#else
        return Application.streamingAssetsPath + "/New2Archives/archive_" + index + "/GameInfo.es3";
#endif
    }

    /// <summary>
    /// 获取version的持久化目录
    /// </summary>
    /// <returns></returns>
    public static string GetSpecialArchivePersistentPath()
    {
        //#if UNITY_IOS
        return Application.persistentDataPath + "/New2Archives/archive_0/GameInfo";

        //#else
        //return Application.persistentDataPath + "/theVersion.txt";
        //#endif
    }
      
}

/// <summary>
/// 属性id
/// </summary>
public enum PropertyIdType
{
    None = 0,
    Tili = 40001,//体力
    Hp = 10001,//血
    MPSpeed = 10002,//气 集气效率
    Attack = 10003,//攻 攻击
    Defense = 10004,//守 防御
    CritRate = 10005,//锐 暴击概率

    CritNum = 10006,//伤 暴击伤害
    MpNum=10007,//蓝量
    JingTong=10008,//精通
    RdmProDamageAdd=10009,//随机属性增伤
    WaterDamageAdd = 10010,//水属性增伤
    FireDamageAdd = 10011,//火属性增伤
    StormDamageAdd = 10012,//雷属性增伤
    IceDamageAdd = 10013,//冰属性增伤
    YangProDamageAdd = 10014,//阳属性增伤
    YinProDamageAdd=10015,//阴属性增伤
    TotalProDamageAdd = 10016,//总属性增伤
    DeDamage = 10017,//免伤
    PoJia=10018,//破甲

    ShiWu = 20001,//炼器

    ZaoHua=20002,//造化

    JieDan=30001,//炼丹

    LianShi=30002,//宝石
    CaiKuang=30003,//采矿
    JingWen=30004,//经文
    LingShi=30005,//种田
}

///// <summary>
///// 历练需要条件
///// </summary>
// public enum LiLianConditionType
//{
//    StudentTalent,//需要修武/种田....弟子
//    StudentXingGe,//需要xx性格的弟子
//    StudentXueMai,//需要xx属性的弟子
//    StudentGender,//需要
//}