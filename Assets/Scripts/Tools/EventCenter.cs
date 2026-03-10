using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件中心
/// </summary>
public class EventCenter 
{
    private static EventCenter inst = null;

    public static EventCenter Instance
    {
        get
        {
            if (inst == null)
            {
                inst = new EventCenter();
            }
            return inst;
        }
    }

    //
    public static Dictionary<TheEventType,List<Action>> eventDic = new Dictionary<TheEventType, List<Action>>();

    public static Dictionary<TheEventType,List<Action<object[]>>> eventDicWithParam = new Dictionary<TheEventType, List<Action<object[]>>>();

    /// <summary>
    /// 移除不带参数的消息
    /// </summary>
    /// <param name="theType"></param>
    /// <param name="callBack"></param>
    public static void Remove(TheEventType theType, Action callBack)
    {
        if (eventDic.ContainsKey(theType))
        {
            List<Action> theTypeList = eventDic[theType];
            if (theTypeList.Contains(callBack))
            {
                theTypeList.Remove(callBack);
            }
            if (theTypeList.Count == 0)
                eventDic.Remove(theType);
        }    
    }

    /// <summary>
    /// 移除带参数的消息
    /// </summary>
    /// <param name="theType"></param>
    /// <param name="callBack"></param>
    public static void Remove(TheEventType theType, Action<object[]> callBack)
    {
        if (eventDicWithParam.ContainsKey(theType))
        {
            List<Action<object[]>> theTypeList = eventDicWithParam[theType];
            if (theTypeList.Contains(callBack))
            {
                theTypeList.Remove(callBack);
            }
            if (theTypeList.Count == 0)
                eventDicWithParam.Remove(theType);
        }
    }
    public static void Register(TheEventType theType, Action callBack)
    {
        if (!eventDic.ContainsKey(theType))
            eventDic.Add(theType, new List<Action>());

        if (!eventDic[theType].Contains(callBack))
        {
            eventDic[theType].Add(callBack);
        }
    }

    public static void Register(TheEventType theType, Action<object[]> callBack)
    {
        if (!eventDicWithParam.ContainsKey(theType))
            eventDicWithParam.Add(theType, new List<Action<object[]>>());
        if (!eventDicWithParam[theType].Contains(callBack))
        {
            eventDicWithParam[theType].Add(callBack);
        }
    }

    public static void Broadcast(TheEventType type)
    {
        if (eventDic.ContainsKey(type))
        {
            List<Action> theCallBackList = eventDic[type];

            for(int i = theCallBackList.Count-1; i >=0; i--)
            {
                theCallBackList[i]();
            }
        }
    }
    /// <summary>
    /// d
    /// </summary>
    /// <param name="type"></param>
    public static void Broadcast(TheEventType type ,params object[] param)
    {
        if (eventDicWithParam.ContainsKey(type))
        {
            List<Action<object[]>> theCallBackList = eventDicWithParam[type];

            for (int i = theCallBackList.Count-1; i >=0; i--)
            {
                theCallBackList[i](param);
            }

        }
        if (eventDic.ContainsKey(type))
        {
            List<Action> theCallBackList = eventDic[type];

            for (int i = theCallBackList.Count-1; i >=0 ; i--)
            {
                theCallBackList[i]();
            }
        }
    }
    /// <summary>
    /// 清除所有事件
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
        eventDicWithParam.Clear();
    }

    ///// <summary>
    ///// 用法
    ///// </summary>

    //public void RegisterTest()
    //{
    //    EventCenter.Register(EventType.None, Foo);
    //}


    //public void Foo(params object[] t)
    //{

    //}

    //public void BroadcastTest()
    //{
    //    int a = 0;
    //    int b = 1;
    //    EventCenter.Broadcast(EventType.None, a, b);
    //}
}


/// <summary>
/// 事件类型
/// </summary>
public enum TheEventType
{
    None = 0,
    PropertyAdd,//增加属性
    PropertyDecrease,//减少属性
    WeekPlus,//一周过去
    MoonPlus,//一月过去
    YearPlus,//一年过去
    BattleEnd,//战斗结束
    BattleBeHit,//被打
    BattleDeHpShow,//显示掉血
    BattleNextRound,//下一回合
    NextPhase,//下一阶段
    StartMakeEquip,//开始做装备
    MakeEquipProcessing,//装备进度增加
    FinishMakeEquip,//做好了
    OnEnrolledMatch,//报名成功
    FixEquip,//修装备
    FinishFixEquip,//修装备结束
    ShowBattleEffect,//显示技能特效
    BattlePeopleDead,//死亡
    BattlePeopleChangePhase,//改变形态
    BattlePeopleSuccessfulChangePhase,//成功改变形态
    BattlePeopleQieRen,//战斗切人
    ReadyToQieRen,//准备切人
    CancelReadyToQieRen,//取消切人准备
    AttackCDShow,//攻击cd
    CloseBattleScene,//关闭战斗场景
    CloseMainPanel,//关闭主场景
    AddBuff,//增加buff
    RemoveBuff,//减少buff
    GetItem,//得到物品
    LoseItem,//失去物品
    GetCangKuItem,//得到仓库物品
    LoseCangKuItem,//失去仓库物品

    ChangeBuildingStudentNum,//改变建筑弟子数量
    ChangeStudentNum,//改变弟子数量
    BuildingUpgrade,//建筑升级
    LianDanProcess,//炼丹进度
    HarvestDan,//收获丹药
    ChangeLianDanStrength,//改变炼丹强度
    ChangeLianDanStatus,//炼丹状态改变
    LianDanDuZhenQi,//渡真气炼丹
    BattleEnergyChange,//能量变化
    LoseClickMai,//点脉失败
    StartQTE,//开始qte
    SkillAttack,//大招
    DisappearQTE,//筋脉消失
    BlackBattleScene,//开大招 屏幕变暗
    AccomplishNewGuide,//完成新手引导
    NewZhaoChaCome,//来了个找茬的
    ZhaoChaResolved,//找茬的走了
    OnEquip,//装备
    OnUnEquip,//卸下
    EquipIntense,//装备强化
    OnInlayGem,//装备镶嵌宝石
    OnOffLayGem,//装备卸下宝石
    CompositeGem,//合成宝石
    SuccessBreakThrough,//突破成功
    FailBreakThrough,//突破失败
    AddBattleProperty,//增加战斗属性
    DeBattleProperty,//减少战斗属性
    LianDanMoneyFull,//炼丹满了
    OpenReplaceStudentPanel,//替换面板
    SuccessRecruit,//成功招募
    GeneratedNewStudent,//生成了新学生
    CloseCurDialog,//关闭当前dialog
    CloseReceiveLeafScene,//关闭神树场景
    RefreshPaiQianStatus,//刷新派遣状态
    EndPaiQian,//结束派遣
    HarvestPaiQian,//收获派遣
    StudentStatusChange,//学生状态改变
    ReadyToBig,//准备抬手放大
    HandleStudent,//操作弟子
    StudentBreakthroughSuccess,//弟子突破
    UpgradeZongMenProduce,//宗门生产升级
    RefreshZongMenProduceShow,//刷新宗门生产show
    MimicBattleRes,//模拟战斗的结果
    StudentPrepareGoToMainWorld,//弟子准备去主线
    EquipPrepareGoToMainWorld,//装备准备去主线
    CloseBattlePreparePanel,//关闭战斗准备面板
    CloseStudentBattlePanel,//关闭弟子战斗准备面板
    RefreshLevelShow,//刷新关卡显示
    LevelResult,//关卡结算
    RefreshMainMapUIPanelShow,//刷新主线ui页面
    RefreshPeopleShow,//刷新显示
    ShowNPC,//显示npc
    WinOutsideBattle,//完成了支线战斗
    DisappearNPC,//npc消失
    OutMapBreakItem,//外部地图爆物品
    AccomplishTask,//完成任务
    GetItemFlyAnim,//获得物品飞物品特效
    RefreshMiJingGuardShow,//刷新秘境派遣的守卫
    XianMenOpen,//仙门大开
    SelfLianDan,//炼丹
    OnGetStudentExp,//得到弟子经验
    OnProcessXiuwei,//修为增加
    UsedBreakDan,//使用了突破丹
    UpgradeDanFarm,//丹田升级
    DanFarmRebuildProcess,//丹田升级或建造过程
    StopDanFarmRebuildProcess,//停止丹田升级或建造过程
    AddTongZhi,//增加一条通知
    ResearchProcess,//正在研究
    AccomplishResearchProcess,//完成研究
    DanFarmProduceProcess,//丹田生产
    DanFarmProduceAItem,//丹田生产出东西了
    OnZuoZhenStudent,//坐镇了弟子
    QuanLiDanFarm,//全力产丹
    QuanLiChanDanProcessing,//全力产丹过程中
    EndQuanLiDanFarm,//结束全力炼丹
    StudentRelaxProcess,//弟子休息中
    RemoveDanFarm,//移除丹房
    StopZuoZhenStudent,//取消坐镇
    CloseStopZuoZhenStudentChoosePanel,//关闭取消坐镇的页面
    OnEquipSkill,//装配技能
    OnUnEquipSkill,//卸下技能
    AddMapEvent,//地图事件
    RemoveNPC,//移除npc
    SuccessXiuLian,//修炼成功
    RemoveMapEvent,//移除地图事件
    StartMatDanFarmProduce,//需要材料的丹炉开始生产
    StopMatDanFarmProduce,//需要材料的丹炉停止生产
    ZhuZhan,//弟子助战
    RemoveZhuZhan,//移除一个助战弟子
    StudentPrepareExplore,//弟子准备去探险
    GoMiJingPointExplore,//弟子前去秘境点探险

    OnTeamExploreMoving,//弟子探险走动
    OnTeamExploreArrived,//弟子探险到达
    RefreshExploreBuJiShow,//探险补给刷新

    TargetSightToCertainObj,//移动视角到某物
    FinishTargetSightToCertainObj,//结束移动视角到某物
    StartFarmBuild,//开始建造田
    ZongMenLevelUpgrade,//宗门等级增加
    ChangeTaskStatus,//改变任务状态
    UnlockDanFarm,//解锁生产位
    CloseZuoZhenStudentChoosePanel,//关闭坐镇弟子页面
    RareStudentExpFull,//弟子经验已满
    TalentStudentExpFull,//天赋弟子经验已满

    Appear3Talent,//出现3个天赋
    EndowTalent,//赋予天赋
    SuccessStudentBreakThrough,//弟子突破成功
    FailStudentBreakThrough,//弟子突破失败

    SkillUpgrade,//技能升级
    AddedRecruitStudentNumLimit,//增加了招募弟子人数限制
    AddTopRecord,//顶端记录

    DissolvedEquip,//分解了装备
    RemoveBattleBuff,//移除战斗buff
    StudentPrepareTeam,//弟子准备上阵
    RefreshBatlteBuff,//刷新buff显示
    StartEscape,//开始逃跑
    PlayEscapeAnim,//播放逃跑动画
    ShowUnlockFarmPosStatus,//显示可解锁空地状态
    AddBattleBuff,//增加战斗buff
    BuffTxtView,//buff显示的文字
    OnSendItem,//赠送装备
    AddBattleHp,//战斗增加血
    CancelMai,//取消脉
    DeBattleBuffHuiHe,//减少buff回合
    OnActivityBaoMing,//活动报名
    RefreshActivityCountdownShow,//活动倒计时
    RefreshStudentRedPoint,
    RefreshGuideBookRedPoint,
    GetExploreItem,//得到探索物品
    ShowMapEventFinish,//显示事件结束
    RevealWenHao,//揭露问号

    OnStudentCangKuProcess,//弟子在仓库强化过程
    OnStudentCangKuStopWork,//弟子在仓库结束工作

    NevigateToLevel,//导航到某一关
    ShowAccomplishLevelAnim,//完成关卡动画显示

    NevigateToMountainPos,//导航到山门地点
    BuildNewFarmClick,//点击新田
    SetFaceAndName,//捏脸成功
    RealitySecondPassed,//现实过去1秒
    OnADRerdm,//广告改命
    FinishCloudMove,//结束云层移动
    OnADMiJingDouble,//秘境双倍领取
    OnRemoveStudent,//逐出
    OnDeleteArchive,//删除存档
    ShowBuffFunction,//显示buff作用（灼烧）
    NevigateScrollAutoPoint,//智能导航到达
    OnGetGuideBookAward,//领奖手札
    RemoveTalent,//天赋移除

    StartPiPei,//开始匹配
    OnAddMatchParticipateNum,//增加大比参赛次数

    OnBuyItem,//买物品
    OnGetDailyWinAward,//获得每日胜利奖品
    RefreshSkillRedPoint,//技能红点刷新

    OnAddShowChat,//显示一条聊天
    OnRemoveShowChat,//移除显示的聊天

    ShowExploreDetail,//显示探险详情
    GetMatchDailyJieSuanAward,//领取每日结算
    ChangeZhangMenName,//改掌门名
    ChangeZongMenName,//改宗门名
    ChangeStudentName,//改弟子名
    RefreshOfflineShow,//刷新离线收益
    OnSuoYao,//索要
    YuanSuReaction,//元素反应
    ShowYuanSuReactionNum,//显示元素反应伤害
    OnDongJiang,//冻僵
    BattleInfoTxtShow,//显示信息
    RefreshKangXingNumShow,//刷新抗性显示
    OnUpgradeXueMai,//提升血脉等级
    RefreshLiLianShow,//完成历练
    OnAddQieRenP,//增加一个切的人（引导
    UnlockYuanSu,//解锁元素
    OnBtnClick,//点击任意按钮
    ClearYuanSu,//清空元素
    ChangeXingGe,//改变性格
    BrushShop,//刷新商店
    OnSuccessWatchGuangGao,//看了广告
    RefreshAllGuangGaoAwardShow,//刷新总广告次数商店显示

    OnGetDailyTaskAward,//获奖每日任务
    OnGetDailyTaskProcessAward,//获奖每日任务进度
    RefreshDailyTaskRedPoint,//每日任务红点
    RealityOneMinProcess,//现实过了1分钟
    OnSaoDang,//扫荡
    RefreshZhangMenRedPoint,//刷新掌门红点
    ChangeYuanSu,//改变元素
    RefreshMail,//刷新邮件
    OnRMBBuy,//人民币购买
    RefreshGuideBtnShow,//刷新引导按钮
    HideNPC,//隐藏npc
    StartMakeGem,//开始做宝石

    Building_overlapSearch,//是否重叠
    Building_overlapRes,//重叠结果
    OnMountainScrollMove,//山门的scroll在拖动
    EnterBuildingMode,//进入建造模式
    EnterOnlyMoveBuildingMode,//进入只移动的建造模式

    QuitBuildingMode,//离开建造模式
    OnClickedSingleFarm,//点击一个建筑
    Moving_overlapSearch,//是否重叠
    Moving_overlapRes,//重叠结果
    OnSearchedMyAllMail,
    OnReadMail,
    OnGetMailAward,
    ReceiveMail,
    OnGetAllMailAward,
    OnDeleteMail,
    FinishCheckEdition,
    SuccessLogIn,
    ShowRoleId,
    SuccessSDKLogin,//sdk登录
    OnRequestGMDownload,
    TableOK,
    StudySkill,//学技能
    JingLianEquip,//精炼
    StartLogin,//开始登录
    CloseHandleItemTipsPanel,
    CloseItemTipsPanel,
    ChooseQu,//选区
    SuccessDownloadArchive,//成功下载
    ReceivedRoleRank,
    UseTiaoXiDan,//使用调息丹
    OnChou,//抽卡
    ZhengRong,//整容
    ChangeTouXiang,
    ChangeTouXiangKuang,
    GetLeiChongAward,//累充奖励
    GetShouChongAward,//首充奖励
    SuccessUploadArchive,
    RefreshRedPointShow,//刷新红点显示
    RefreshMainPanelBtnShow,//主页面按钮
    OnFinishRangeXuanXiu,//结束一轮
    FinishXuanXiu,//结束选秀
    OnStartPlayerSingleRangeXuanXiu,//玩家选秀
    FinishLoading,
}
