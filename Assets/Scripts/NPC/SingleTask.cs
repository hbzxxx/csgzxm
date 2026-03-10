
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
//verticalgroup 声明一下 然后在字符串代表的组里面往下加
//horizontalgroudp 声明一下 然后在字符串代办的组里面往右加
public class SingleTask
{
    [LabelText("禁用")]
    public bool block;
    public ulong theId;
    public int index;//第几个位置
    public SingleTask()
    {
        theId = ConstantVal.editorId;
    }
    //[HorizontalGroup("split", 355, LabelWidth = 70)]
    //public const string LEFT_VERTICAL_GROUP = "Split/Left";
    //protected const string STATS_BOX_GROUP = "Split/Left/Stats";
    //protected const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings/Split/Right";

    [HorizontalGroup("Split", LabelWidth = 70)]

    [VerticalGroup("Split/Left")]
    [BoxGroup("Split/Left/标志性id")]
    [EnumAttirbute("标志性id")]
    public string tagId;

    [VerticalGroup("Split/Left")]
    [BoxGroup("Split/Left/统计名字")]
    [EnumAttirbute("统计名字")]
    public string trackingName;


    [VerticalGroup("Split/Left")]
    [BoxGroup("Split/Left/NPC出现位置")]
    [EnumAttirbute("位置")]
    public NPCAppearPosType NPCPos;

    [BoxGroup("Split/Left/条件")]
    public List<TaskCondition> condition;

    [BoxGroup("Split/Left/任务执行引导")]
    [EnumAttirbute("引导id")]
    public int TaskGuideId;

    [BoxGroup("Split/Left/任务类型")]
    [EnumAttirbute("任务类型")]
    public TaskType taskType;

    //[VerticalGroup("Split/Left/丹炉数量")]
    //[EnumAttirbute("丹炉数量")]
    //public int danLuNum; 

    [BoxGroup("Split/Left/重复性质")]

    [VerticalGroup("Split/Left/重复性质/a")]
    [EnumAttirbute("重复性质")]
    public TaskRepeatType taskRepeatType;

    [VerticalGroup("Split/Left/重复性质/a"),ShowIf("taskRepeatType",TaskRepeatType.Repeat)]
    [LabelText("现实时间（分钟)")]
    public int repeatTime; 

    [VerticalGroup("Split/Left/需要物品")]
    [LabelText("需要物品")]
    public bool isNeedItem;
    [HorizontalGroup("Split/Left/需要物品/Split", LabelWidth = 80)]
    [VerticalGroup("Split/Left/需要物品/Split/是否回收物品"), ShowIf("isNeedItem", true)]
    [LabelText("是否回收物品")]
    public bool needReceiveItem;

    [VerticalGroup("Split/Left/需要物品/Split/物品id"),ShowIf("isNeedItem",true)]
    [EnumAttirbute("物品id")]
    public ItemIdType needItem;
    [VerticalGroup("Split/Left/需要物品/Split/物品数量"), ShowIf("isNeedItem", true)]
    [LabelText("需要数量")]
    public int needNum;


    [VerticalGroup("Split/Left/需要杀敌")]
    [LabelText("需要杀敌")]
    public bool isNeedKillEnemy;
    [HorizontalGroup("Split/Left/需要杀敌/Split", LabelWidth = 80)]

    [VerticalGroup("Split/Left/需要杀敌/Split/敌人id"), ShowIf("isNeedKillEnemy", true)]
    [EnumAttirbute("敌人id")]
    public EnemyIdType needKillEnemy;
    [VerticalGroup("Split/Left/需要杀敌/Split/敌人数量"), ShowIf("isNeedKillEnemy", true)]
    [LabelText("需要数量")]
    public int needKillEnemyNum;


    [VerticalGroup("Split/Left/需要丹炉数量"),ShowIf("taskType", TaskType.DanFarmNum)]
    [LabelText("需要丹炉数量")]
    public bool isNeedDanFarmNum;
    [HorizontalGroup("Split/Left/需要丹炉数量/Split", LabelWidth = 80)]
    [VerticalGroup("Split/Left/需要丹炉数量/Split/丹炉id"), ShowIf("isNeedDanFarmNum", true)]
    [EnumAttirbute("丹炉id")]
    public DanFarmIdType needDanFarmId;
    [VerticalGroup("Split/Left/需要丹炉数量/Split/丹炉数量"), ShowIf("isNeedDanFarmNum", true)]
    [LabelText("需要数量")]
    public int needDanFarmNum;

    [VerticalGroup("Split/Left/需要境界等级"), ShowIf("taskType", TaskType.PlayerLevel)]
    [LabelText("需要境界等级")]
    public int needPlayerLevel;

    [VerticalGroup("Split/Left/通用完成条件")]
    [LabelText("通用完成条件")]
    public string commonAccomplishCondition;

    [BoxGroup("Split/Left/任务要求描述")]
    [HideLabel, TextArea(1, 1)]
    public string des;


    [BoxGroup("Split/Left/任务奖励")]
    public List<string> awardList;
    //[BoxGroup("Split/Left/General Settings")]

    //[VerticalGroup("Split/Left/傻逼")]
    //[LabelText("有傻逼")]
    //public bool have;
    //[HorizontalGroup("Split/Left/傻逼/Split", LabelWidth = 80)]

    //[VerticalGroup("Split/Left/傻逼/Split/a")] 
    //public int sb11;
    //[VerticalGroup("Split/Left/傻逼/Split/b")]
    //public int sb12;

    [VerticalGroup("Split/Right")]




    [VerticalGroup("Split/Right")]
    [LabelText("出现是否提示")]
    public bool ifAppearInform;

    [VerticalGroup("Split/Right"), ShowIf("ifAppearInform", true)]
    [HideLabel, TextArea(1, 2)]
    [LabelText("出现时提示文字")]
    public string appearTxt;

    [VerticalGroup("Split/Right")]
    [LabelText("是否前置对话")]
    public bool ifDialogBefore;

    [BoxGroup("Split/Right/前置对话列表"),PropertyTooltip("执行任务前的对话"),ShowIf("ifDialogBefore", true)]
    public List<DialogEditorData> dialogList=new List<DialogEditorData>();

    [VerticalGroup("Split/Right")]
    [LabelText("是否后置对话")]
    public bool ifDialogAfter;
    [BoxGroup("Split/Right/后置对话列表"), ShowIf("ifDialogAfter", true)]
    public List<DialogEditorData> dialogListAfter = new List<DialogEditorData>();




}

public class DialogEditorData
{
    [HorizontalGroup("split", LabelWidth = 70)]

    [VerticalGroup("split/a")]
    public TaskDialogBelongType type;

    [VerticalGroup("split/a")]
    [HideLabel, TextArea(1, 1)]
    public string content;

    [VerticalGroup("split/a")]
    [EnumAttirbute("玩家是否回复")]
    public bool ifPlayerAnswer;

    [VerticalGroup("split/a"),ShowIf("ifPlayerAnswer",true)]
    [EnumAttirbute("玩家回复")]
    public List<string> answerList;

    //[VerticalGroup("split/a"), ShowIf("ifPlayerAnswer", true)]
    //[EnumAttirbute("npc再次回复")]
    //public List<string> npcAnswerList;
}

/// <summary>
/// npc出现位置
/// </summary>
[LabelText("位置")]
public enum NPCAppearPosType
{
    None=0,
    [EnumAttirbute("山门")]
    ShanMen=1,
    [EnumAttirbute("迷雾森林")]
    MiWuSenLin=2,
    [EnumAttirbute("幽冥洞穴")]
    YouMingDongXue = 3,
    [EnumAttirbute("炼丹房空余丹炉位")]
    LianDanFangEmptyDanFarmPos=4,
    [EnumAttirbute("招募弟子处")]
    ZhaoMuDiZiPos,
    [EnumAttirbute("坐镇丹房")]
    ZuoZhenDanFarm,
    [EnumAttirbute("炼器房")]
    EquipmakeBuilding,
    [EnumAttirbute("山下")]
    ShanXia,
    [EnumAttirbute("特定炼丹炉")]
    LianDanLu,
    [EnumAttirbute("洞府修炼")]
    DongFuXiuLian,
}

/// <summary>
/// 任务对话属于谁
/// </summary>
public enum TaskDialogBelongType
{
    None=0,
    Player,
    Self,
}
/// <summary>
/// 任务类型
/// </summary>
public enum TaskType
{
    None=0,
    [EnumAttirbute("杀怪拿东西")]
    KillMonsterGetItem=1,//杀怪拿东西
    [EnumAttirbute("杀怪多少只")]
    KillEnemy=2,
    [EnumAttirbute("收集物品")]
    ReceiveItem=3,
    [EnumAttirbute("切磋")]
    QieCuo=4,
    [EnumAttirbute("丹炉数量")]
    DanFarmNum=5,
    [EnumAttirbute("引导—招募一名外门弟子")]
    ZhaoMuDiZi=6,
    [EnumAttirbute("升级丹炉实验室次数")]
    UpgradeResearchNum = 7,
    [EnumAttirbute("招募弟子")]
    ZhaoMuStudent = 8,
    [EnumAttirbute("加速生产次数")]
    QuanLiNum=9,
    [EnumAttirbute("炼制法器数量")]
    EquipNum = 10,
    [EnumAttirbute("新手_解决俩混混")]
    Guide_KillHunHun = 11,
  
    [EnumAttirbute("完成地图事件")]
    AccomplishMapEvent = 12,
    [EnumAttirbute("玩家提升境界到")]
    PlayerLevel = 13,

    [EnumAttirbute("学习技能")]
    StudySkill=14,

    [EnumAttirbute("提升宗门等级")]
    UpgradeZongMen = 15,

    [EnumAttirbute("使用某类型物品多少次")]
    UseSpecialTypeItemCount = 16,
    [EnumAttirbute("驻守建筑")]
    StudentZuoZhen = 17,

    [EnumAttirbute("拥有a个b级c建筑")]
    HaveABLevelCFarm = 18,

    [EnumAttirbute("天赋觉醒次数")]
    TianFuJueXingNum=19,//弟子天赋觉醒次数


    [EnumAttirbute("携带技能")]
    EquipSkill = 20,

    [EnumAttirbute("升级技能")]
    UpgradeSkill = 21,

    [EnumAttirbute("升级法器")]
    UpgradeEquip = 22,

    [EnumAttirbute("弟子升级次数")]
    StudentUpgradeCount=23,

    [EnumAttirbute("解锁空地数量")]
    UnlockFarmPosNum = 24,

    [EnumAttirbute("炼丹")]
    LianDan = 25,

    [EnumAttirbute("通过关卡")]
    PassLevel=26,

    [EnumAttirbute("弟子上阵")]
    ShangZhen=27,

    [EnumAttirbute("装备法器")]
    EquipEquip=30,
    [EnumAttirbute("炼制宝石")]
    MakeGem=31,
    [EnumAttirbute("讨伐")]
    TaoFa=32,
    [EnumAttirbute("炼丹成就")]
    LianDan2=33,
    [EnumAttirbute("炼制某级别法器数量")]
    RarityEquipNum = 34,

    [EnumAttirbute("升级某级别法器")]
    UpgradeRarityEquip = 35,

    [EnumAttirbute("镶嵌某级别宝石")]
    InlayRarityGem = 36,

    [EnumAttirbute("拥有a名b天赋弟子")]
    HaveATalentBStudent = 37,

    [EnumAttirbute("坐镇弟子总数")]
    StudentZuoZhenTotalNum = 38,
    [EnumAttirbute("探索秘境次数")]
    Explore = 40,
    [EnumAttirbute("最大讨伐等级")]
    MaxTaoFa = 41,
    [EnumAttirbute("通过哪一关裂隙")]
    PassedMaxLieXiLevel=42,

    [EnumAttirbute("历练")]
    LiLian=44,//历练

    CompositeGem=45,//合成宝石
    [EnumAttirbute("宗门改名")]
    ChangeZongMenName = 46,//改宗门名字
    [EnumAttirbute("强化血脉")]
    UpgradeXueMai=47,

    [EnumAttirbute("弟子拥有法器")]
    StudentHaveEquip=48,
    [EnumAttirbute("弟子镶嵌某level宝石")]
    StudentHaveLevelGem=49,
    [EnumAttirbute("弟子拥有a个b等级法器")]
    StudentHaveANumBLevelEquip = 50,
    [EnumAttirbute("拥有a名b等级的生产弟子")]
    HaveABLevelProductStudent = 51,
    [EnumAttirbute("拥有a个技能等级为b的弟子")]
    HaveABLevelSkillStudentNum = 52,
    [EnumAttirbute("坐镇a建筑的弟子数b")]
    ZuoZhenAFarmBStudentNum = 53,
    [EnumAttirbute("弟子和掌门所有的宝石数量")]
    TotalGemNum=54,
    [EnumAttirbute("弟子拥有超过a级别b等级c数量的法器")]
    StudentHaveARarityBLevelCNumEquip,
    [EnumAttirbute("队伍中有a个b元素的人")]
    TeamHaveABYuanSuP,
    [EnumAttirbute("添加物品到仓库")]
    AddItemToCangKu,
}
[LabelText("是否重复")]
public enum TaskRepeatType
{
    None=0,
    [EnumAttirbute("重复")]
    Repeat,
    [EnumAttirbute("一次性")]
    Once,
}

/// <summary>
/// 怪物类型
/// </summary>
public enum EnemyIdType
{
    [EnumAttirbute("逐灵鸟")]
    ZhuLingNiao=30001,//逐灵鸟
    [EnumAttirbute("噩魂蛛")]
    EHunZhu = 30002,
    [EnumAttirbute("五色彩鳞蟒")]
    WuSeCaiLinMang=30003,
    [EnumAttirbute("混混")]
    HunHun = 30004,
    [EnumAttirbute("山海宗门人")]
    ShanHaiZongMenRen = 30005,
    [EnumAttirbute("山海宗长老")]
    HunHunTouZi = 30006,
    [EnumAttirbute("山海宗掌门")]
    ShanHaiZongZhangMen=50301,
    [EnumAttirbute("狸猫人掌门")]
    LiMaoZhangMen=50302,
    [EnumAttirbute("帝姝")]
    DiShu=30007,
    [EnumAttirbute("混乱的苏梦岚")]
    HunLuanDeSuMengLan= 51324,
    [EnumAttirbute("送技能的山贼")]
    SendSkillShanZei = 31020,
    //新手引导元素反应的帝姝
    NewGuideYuanSuDiShu=110001,
    DaFeng=54120,
}

/// <summary>
/// 丹炉id类型
/// </summary>
public enum DanFarmIdType
{
    [EnumAttirbute("任意类型")]
    Any = 0,
    [EnumAttirbute("灵田")]
    JinDanLu=10001,
    [EnumAttirbute("绝品金丹炉")]
    JuePinJinDanLu=10002,
    [EnumAttirbute("凡级矿场")]
    YueLingKuangLu = 20001,
    [EnumAttirbute("炼丹炉")]
    LianDanLu=10006,//炼丹炉
    [EnumAttirbute("炼器房")]
    EquipMake = 50001,//炼器房
    [EnumAttirbute("凡级灵树")]
    FanJiLingShu=30001,
    [EnumAttirbute("凡级藏经阁")]
    FanJiCangJingGe = 40001,
    BaGuaLu=60001,//八卦炉
    HongMengShuLing=80001,//鸿蒙树灵
    JuLingZhen= 90001,//聚灵阵
    HongMengKuangLing=100001,//鸿蒙矿灵
    BiXieJinLian=110001,//辟邪金莲
    YouBiBaiLian=120001,//幽碧白莲
    JuBaoPen=130001,//聚宝盆
    HongMengTianDao=140001,//鸿蒙天道
    ShangGuPiXiu=150001,//上古貔貅
    QianKunJing=160001,//乾坤井
    JingShiQingLian=170001,//净室青莲
    JieZiXuMi=180001,//芥子须弥
    XianZhuJinChan=200001,//衔珠金蟾
}

/// <summary>
/// 任务条件
/// </summary>
public class TaskCondition
{
    [HorizontalGroup("split",LabelWidth =130)]

    [VerticalGroup("split/a")]
    [EnumAttirbute("任务条件")]
    public TaskConditionType taskConditionType;
    [VerticalGroup("split/a")]
    [LabelText("任务参数")]
    public string param;
    //[VerticalGroup("条件/写着玩")]
    //public TaskConditionType taskConditionType2;
}

/// <summary>
/// 任务条件类型
/// </summary>
[LabelText("任务条件")]
public enum TaskConditionType
{
    None=0,
    [EnumAttirbute("玩家修炼等级大于等于")]
    PlayerTrainLevel=1,//玩家修炼等级
    [EnumAttirbute("玩家修炼等级在某个范围")]
    PlayerTrainLevelRange=3,
    [EnumAttirbute("完成了上个任务")]
    AccomplishedLastTask=4,
    [EnumAttirbute("完成了某个主线任务")]
    AccomplishedMainLineTask,
    [EnumAttirbute("物品数量大于等于")]
    ItemCountMoreThan,
    [EnumAttirbute("弟子感悟值满")]
    RareStudentExpFull,
    [EnumAttirbute("觉醒后弟子感悟值满")]
    StudentExpFull,
}
