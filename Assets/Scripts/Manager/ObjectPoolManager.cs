using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 对象池单元
/// </summary>
public enum ObjectPoolSingle
{
    None = 0,
    SinglePropertyView,//属性
    SingleConsumeView,//消耗
    TrainPanel,//训练面板
    MainPanel,//主面板
    BattlePeopleView,//战斗的人
    YiYangZhiEffect,//一阳指特效
    BattlePanel,//战斗面板
    MatchPreparePanel,//比赛对阵面板
    HitEffect,//打中的效果
    DeHpTxtView,//掉血特效
    FloatWindowPanel,//提示框
    EquipMakePanel,//装备制作面板
    AddProTxtAnimView,//增加的数字
    PropertyCompareView,//属性比较
    OnlyOkHintPanel,//只有ok的弹窗
    CommonHintPanel,//普通弹窗
    MatchForecastView,//比赛预报
    MatchChoosePanel,//选择比赛
    StartPanel,//开始菜单
    LeafView,//叶子
    LeafReceiveView,//收集叶子
    GetLeafEffectView,//得到叶子特效
    SingleColdZoneEffect,//极寒领域
    jiuShuiLianDaZhao,//月耀八荒 斩四下一个爆
    DeHpGroupView,//一组掉血特效
    RingAttack,//灵弹攻击特效
    SingleSkillView,//技能
    SingleEquipView,//单个装备图标
    BattleScenePanel,//战斗场景面板
    ReceiveLeafPanel,//收叶子
    ChooseBtnView,//选择按钮
    DialogPanel,//对话面板
    HousePanel,//房间
    SingleBuffView,//buff
    BlackMaskPanel,//黑幕
    ItemView,//物品
    KnapsackPanel,
    KnapsackItemView,//背包
    DistributeSliderPanel,//分配弟子
    MountainPanel,//后山宗门
    EquipMakeBuildingPanel,//炼器房
    LianDanBuildingPanel,//炼丹房
    LianDanStrengthView,//炼丹强度
    TopPanel,//顶端面板
    MaiLineView,//筋脉闪电
    ClickMai,//点击筋脉
    NewGuideCanvas,//新手引导
    Effect_jinFengYuLu,//金风玉露
    SkillEffect_SwordAttack,//剑普攻
    GetAwardPanel,//获得物品
    KnapsackDownBtnView,//背包下面的按钮
    EquipPictureView,//图纸
    SingleStudentView,//学生
    SkillCompareView,//技能升级前后
    ItemShowView,//物品显示
    EquipIntensePanel,//装备强化
    WithCountItemView,//带数量的
    EquipAddGemPanel,//宝石镶嵌面板
    GemKnapsackItemView,//宝石镶嵌面板底下的宝石
    InlayedGemItemView,//已镶嵌的宝石
    GemCompositePanel,//宝石合成面板
    GemCompositeMatItemView,//宝石合成的材料
    EquipIntenseKnapsackItemView,//装备强化界面中可供选择的装备
    EquipUpgradeResPanel,//升级结果
    GemPropertyView,//宝石属性
    GemCompositeResPanel,//合成结果
    PeopleProPanel,//人物属性面板（以后会包含称号等
    PlayerPeoplePanel,//主面板
    TxtFlyUpAnimView,//飞行的文字
    WorldMapPanel,//大地图
    SingleMapScenePanel,//单个地图
    EquipMainPropertyView,//主属性
    EquipVicePropertyView,//次要属性
    StudentTagScrollView,//带标签的一系列弟子
    CandidateStudentView,//待招募弟子
    ReplaceStudentView,//用来代替的弟子
    ShanMenPanel,//山门
    GemPictureView,//宝石图纸
    GemTagScrollView,//宝石炼制标签页
    ItemTipsPanel,//tips
    BigChooseStudentView,//大的选择学生
    ChooseStudentPanel,//选择学生面板
    ReceiveLeafScenePanel,//收叶子场景
    MiJingPanel,//秘境
    SingleUpgradeStudentView,//学生升级
    StudentHandlePanel,//操作弟子升级的面板
    SingleHandleStudentView,//操作弟子的单个view
    SingleZongMenProduceView,//单个宗门生产
    ZongMenPanel,//宗门面板
    StudentBattlePanel,//战斗
    BattleSmallView,//战斗小view
    StudentTipsPanel,//弟子tips
    GoToMainWorldPreparePanel,//去主线的准备界面
    MainWorldPrepareStudentView,//去主线的准备弟子
    GoToMainWorldPrepareKnapsackItemView,//去主线准备装备
    BattlePreparePanel,//去备战
    BattlePreparePeopleView,//备战者
    SingleChooseMapView,//选择地图
    GetAwardWithStudentUpgradePanel,//结算拿奖励并且弟子升级
    GetAwardWithCloseActionPanel,//带回调的结算面板
    GetAwardCloseActionPanel,//结算拿奖励 关闭有回调
    SingleMatchPeopleView,//参
    MainMapUIPanel,//地图ui
    ShowTipsItemView,//显示tips的物品
    TaskGuidePanel,//任务引导
    NPCView,//npc
    OutsidePanel,//外出
    NPCTaskPanel,//npc任务面板
    OutMapTipsPanel,//外部地图具体内容tips
    ReceiveItemView,//外部地图爆物品
    GetItemFlyUpAnimView,//捡东西特效
    ProcessingPanel,//进度面板
    TrainDanItemView,//突破成功率丹物品
    OfflineIncomePanel,//离线收益面板
    ZhanDouLiChangeShowPanel,//战斗力面板
    MiJingLevelView,//秘境关卡
    SingleMiJingChoosePanel,//单个秘境关卡总面板
    PaiQianHarvestPanel,//派遣收获面板
    EquipMakeFinishPanel,//装备制作完毕面板
    CommonDanTagScrollView,//普通丹制作列表
    CommonDanPictureView,//普通丹图纸
    EquipFixPanel,//修器面板
    SingleMapScenePanel_0,//第一个地图
    SingleMapScenePanel_1,//地图
    SingleMapScenePanel_2,//地图
    SingleMapScenePanel_3,//地图
    SingleMapScenePanel_4,//地图
    SingleMapScenePanel_5,//地图
    SingleDanFarmView,//丹田
    NewDanFarmSingleView,//新丹田
    SingleTongZhiView,//通知
    ResearchPanel,//研究面板
    SingleResearchView,//单个研究
    TongZhiPanel,//通知面板
    SkillPanel,//技能面板
    MapEventView,//地图事件
    PangBaiPanel,//旁白
    CloudPanel,//云层
    MapEventPanel,//地图事件
    DanFarmProductChooseView,//炼丹房产品选择
    ChooseFarmProductMatPanel,//炼丹房产品选择面板
    FixedMainMapUIPanel,//固定地图的ui
    WorldTypeChoosePanel,//选择进入什么样的世界
    GotoFixedMainWorldPreparePanel,//固定地图的世界准备面板
    FixedSingleMapScenePanel_0,//固定地图0
    FixedSingleMapScenePanel_1,//固定地图1
    FixedSingleMapScenePanel_2,//固定地图2
    FixedSingleMapScenePanel_3,//固定地图3
    FixedSingleMapScenePanel_4,//固定地图4
    FixedSingleMapScenePanel_5,//固定地图5
    UnlockFunctionPanel,//解锁新功能
    GotoExplorePreparePanel,//弟子秘境探险准备面板
    SingleChooseExploreView,//选择某个秘境
    ExplorePrepareStudentView,//去秘境的弟子
    ExplorePanel_0,//探险场景0
    ExplorePanel_1,//探险场景0
    ExplorePanel_2,//探险场景0
    ExplorePanel_3,//探险场景0
    ExplorePanel_4,//探险场景0
    ExplorePanel_5,//探险场景0

    ExploreTeamView,//探险队
    ExplorePointView,//秘境探索点
    RoutePointView,//路径点
    ExploreTeamStatusView,//探险队显示在ui的状态
    ExploreUIPanel,//探险队ui
    SingleEconomyView,//单个经济
    EconomyPanel,//经济面板
    DanFarmUpgradePanel,
    NewDanFarmBuildPanel,//新丹田
    ZongMenUpgradePanel,//宗门升级面板
    UpgradeDesTxtView,//升级介绍文字
    SingleDanFarmDetailPanel,//田信息面板
    TalentTestStudentView,//天赋测试的弟子
    TalentTestPanel,//天赋测试
    SingleTalentView,//单个天赋
    MagicSoftExplosionGreen,//天赋觉醒点石头
    FlyTalentView,//天赋飞行动画
    SingleHaoGanDuStudentView,//好感
    SocializationPanel,//社交属性
    SingleSocializationRecordVew,//记录
    SingleStudentInfluenceRateView,//弟子加成概率
    EquipDissolveKnapsackItemView,//分解装备背包物品
    EquipDissolvePanel,//分解面板
    StudentProAddShowView,//弟子加成动画txt
    SetNamePanel,//取名
    XiuWeiDanItemView,//修为丹
    PoJingDanItemView,//破境丹
    GuideSlideTalentCanvas,//滑动选择天赋
    QieRenStudentView,//切人

    BattleTeamMemberView,//切人修改

    DeathFogView,//死特效
    baiming_tiangangzhengya,//天罡镇压
    zhandouCD,//战斗cd
    TeamPrepareStudentView,//阵营
    TeamPanel,//队伍面板
    SkillEffectSingleParent,//技能父物体
    skill_liantaishoushenjie,//莲台守身决
    skill_liantaishoushenjue_chixu,//莲台守身决持续
    yanhuo_firefengtian,//焰火焚天
    skill_ljiutianxuanleijie,//九天玄雷
    qiehuanrenwutexiao,//切人
    jili_pugong,//季狸普攻
    ZuoZhenDiZiView,
    FarmPrepareStudentView,//建筑选择的弟子
    jili_shenniangongji_ji,//神念攻击
    jili_shenniangongji_ballxuanhuan,//神念攻击buff
    BuffTxtView,
    jili_mihuanwuzhang,//迷幻雾瘴
    SendKnapsackPanel,//赠送装备面板
    SendKnapsackItemView,//赠送装备view
    skill_taishangjingshenjue,//太上净神诀
    jili_zhihuanguangshu_1,//致幻光束
    jili_guitianyunzhang,//鬼天陨掌
    jili_tianbaohuaj,//天宝华镜
    tianbaohuajing,//新版天宝华镜
    FanShangBuff,//反伤buf
    jili_shishenhuwei,//噬神守卫
    jili_shishenhuwei_11,
    jili_shishenhuwei_22,
    jili_shishenhuwei_33,
    jili_tianyinhudun,//天音护盾
    jili_shihaibaodong_1,//识海暴动
    jili_shihaibaodong_2,//晕眩buff
    jili_xuruo1,//易伤buff
    StudentGetNewSkillPanel,//弟子获得新技能
    DanMuTxtView,//弹幕
    DanMuPanel,//弹幕面板
    NotedPeoplePanel,//标记的人
    StudentEventPanel,//弟子事件
    skill_zhenyuanchiyangjue,//真元赤阳决
    buff_jiagong,//加攻buff
    skill_sanhuajudingjue,//三花聚顶决
    ActivityCountdownView,//活动倒计时
    ActivityBaoMingPanel,//活动报名
    SingleActivityBaoMingView,//单个活动报名
    NameWordView,//名字带底图
    BigSkillView,//大的技能view
    InfoBigStudentView,//弟子界面的弟子选择
    SellSliderPanel,//卖东西
    MiJingExploreResultPanel,//秘境探险结算面板
    FollowPView,
    EventRecruitStudentPanel,//事件招募弟子
    CangKuPanel,//仓库
    ChooseItemSliderPanel,//选择物品通用slider
    NewEquipPreparePanel,//选择新装备制造
    ChapterPanel,//章节
    danyao_tuowei,//丹药拖尾
    dujie_xishou,//渡劫吸收
    levelupp,//突破成功
    FadeTxtAnimView,//突破成功显示的属性数值
    TuPoShiBaiSingle,//突破失败
    dizidujie_dujieshibai,//弟子突破失败
    dizidujie_xishou,//弟子突破吸收
    dujie_dizidujiechenggong,//弟子突破成功
    TiliRevivePanel,//加体力面板
    WithoutKuangConsumeView,//无框的消耗
    GetAwardWithDoubleLingQuPanel,//离开地图提供双倍结算
    TipsPanel,
    NoTransparentTipsPanel,//不会穿透的tips
    AddStudentRecruitNumADPanel,
    SingleComefromView,//物品来自哪里
    DuJiePanel,
    AccomplishTaskPanel,//完成任务ui
    GemItemView,//宝石界面用来选择去合成的
    ArchivePanel,//存档面板
    SingleArchiveView,//单个存档view
    yanhuo_huoyangongji,//火焰攻击
    yanhuo_firebaolei,//火焰爆轰
    BuffFunctionTxtView,
    skill_pianhonglingdongjue,//翩鸿灵动决
    SingleGuideBookView,//单个手札任务
    ChapterProcessView,//单个手札进程
    GuideBookPanel,
    MatchPanel,//匹配
    SingleMatchZongMenView,//宗门匹配的人
    PeopleTipsPanel,//点人的tips
    SingleRankView,//排名
    MatchParticipateTimeAddADPanel,//增加匹配次数看广告
    BuySliderPanel,//购买滑动条
    ShopItemView,//商店物品
    SingleLayerMatchShopGroupView,//一层商店
    SkillTipsPanel,//技能tips
    SingleChatView,//聊天
    ChatPanel,//聊天面板
    SingleBigChatView,//聊天
    GemProTxtView,//宝石属性
    ChangeNamePanel,//改名
    YuanLiJieJingItemView,//源力结晶物品
    SingleTopDesView,//顶端描述
    TopDesPanel,//顶端描述面板
    PoChanShowPanel,//破产动画
    SingleYuanSuPointView,//单个元素点
    YuanSuReactionTxtView,//元素反应的文字说明
    PS_IceAttack_TUT,//霜爆
    XueMaiPanel,//血脉面板
    XieZhanPanel,//携战面板
    SingleLiLianDesView,//历练描述
    LiLianPanel,//历练面板
    LiLianPrepareStudentView,//历练准备弟子
    yuansufanyi_3water,//元素反应水
    yuansufanyi_1,//元素反应火
    yuansufanyi_2ice,//冰攻击
    yuansufanyi_4light,//雷攻击
    yuansufanyi_cibao,//磁暴
    yuansufanyi_bingqiang,//冰枪
    XueMaiChoosePanel,//选择您的血脉
    XueMaiChangePanel,//改变血脉
    ShopPanel,//商店
    FenGeView,//分隔线
    SingleDailyTaskView,//每日任务
    SingleSevenDayQianDaoView,//7日签到小view
    SevenDayQianDaoPanel,//7日签到
    ThirtyDayQianDaoPanel,//30日签到
    SingleThirtyDayQianDaoView,//30日签到小view
    LiLianUltraAwardView,//历练额外奖励
    GetAwardItemView,//得奖的物品
    SkillBtnView,//技能按钮
    ZhanDouLiPropertyView,//战力属性
    DetailPropertyPanel,//详细属性面板
    BigStudentSinglePropertyView,//弟子页面的属性
    StudentAddExpPanel,//弟子加经验
    InEquipGemPropertyView,//装备上的宝石
    InfoPanelStudentView,//弟子页面中的view
    SingleProductProView,//介绍
    ProductPropertyDesPanel,//介绍
    dun_guangdun,//光盾
    dun_guangdun_1,
    dun_huodun,//火盾
    dun_huodun_1,//火盾
    yuansufanyi_baoza,//爆炸
    SingleHaoGanDesView,
    HaoGanDesPanel,
    SingleDetailPropertyView,//详细属性
    MailPanel,//邮件
    SingleMailView,//单个邮件
    skill_bianchihun1,//彼岸炽魂1
    skill_bianchihun2,//彼岸炽魂1
    skill_bianchihun3,//彼岸炽魂1
    RarityDesPanel,//稀有度描述
    DownloadArchivePanel,//下载
    ChongZhiShopItemView,//充值
    MoonCardShopItemView,//月卡物品
    LiBaoShopItemView,//礼包
    GuangGaoShopItemView,//广告商店物品
    BuildingModePanel,//建造模式
    BuildingSingleView,
    SingleFarmView,
    EquipPanel,
    EquipChangePanel,
    EquipItemView,
    EquipPosPropertyView,//装备槽位属性显示
    HandleItemTipsPanel,
    BreakThroughResPanel,
    EquipJingLianPanel,
    UpItemTipsPanel,
    LoadingPanel,
    EquipHandlePanel,
    jinsewuping,
    YunCunDangPanel,
    RoleRangePanel,
    SingleMyRoleRangeView,
    SingleRoleRangeView,
    TiaoXiDanItemView,//调息丹
    NoKuangItemView,
    ChouJiangPanel,
    EquipTaoZhuangGroupView,
    OtherFunctionFarmDetailPanel,
    SingleTouXiangView,
    TouXiangChangePanel,
    SingleLeiChongView,
    LeiChongPanel,
    ShouChongPanel,
    ZhengRongPanel,
    DuiHuanMaPanel,
    ShiLingTiShiPanel,
    SingleRankTouXiangView,
    HuoDongPanel,
    SingleFuView,
    LabelTxtView,
    SingleXuanXiuGroupView,
    XuanXiuPanel,
    SingleXuanXiuStudentPosView,
    XuanXiuPrepareStudentView,
    dafeng_texiao_1,
    SharePanel,
    SunView,
    FlyTxtView,
    XuanXiuSkillEffectView,
    XuanXiuConditionView,
    FinishXuanXiuEffectView,
    RegisterPanel,
    JianKangPanel,


    SingleStudentViewpos,
    SingleConsumeView1,
    ItemViewE,
    EquipVicePropertyView1,
    EquipMainPropertyView1,
    SingleSevenDayQianDaoE,//七日签到
    SingleConsumeJineng,//技能学习
    LiLianShangZhenStudent,//弟子上阵面板
    OutCityMapView,//出城地图逻辑
    SingleStudentHaoganduView,//好感度人物头像
    BattlePreparePeopleZhangmen,//第一个战斗角色
    BattleSceneTeammateView,//战斗场景未上阵队友
    BattleSkillView,//战斗技能


    SingleStudentViewItem,
    SingleSkillViewE,
    TeamPrepareStudentViewE,


    CandidateStudentView1,
    SingleStudentViewzm,
    SingleStudentChuZhanView,
    WithCountItemViewLeichong,
    ItemViewbs,
    EquipMainPropertyView2,
    EquipItemposView,
    MinePanel,//挖矿玩法面板
    MineBlockView,//矿洞格子视图
    MineCharacterView,//矿洞角色视图
    WanfaDatingPanel,//玩法大厅面板
    WanfaView,//玩法视图
    CopyPanel,
    SingleMiJingView,
    SingleLiLianView
}

    public class TempItem
    {
       public GameObject obj;
       public long storeTime;
    }

    public class ObjectPoolManager : MonoBehaviour
    {

    static ObjectPoolManager inst;
    public static ObjectPoolManager Instance
    {
        get
        {
            if (inst == null)
            {
                GameObject obj = new GameObject();
                obj.name = "ObjectPoolManager";
                inst = obj.AddComponent<ObjectPoolManager>();
                return inst;
            }
            return inst;
        }
    }
    private Dictionary<Type, Stack<MonoBehaviour>> ObjectDic = new Dictionary<Type, Stack<MonoBehaviour>>();
        private Dictionary<ObjectPoolSingle, List<GameObject>> EnumIndexObjectDic = new Dictionary<ObjectPoolSingle, List<GameObject>>();//如无必要，永久存储的对象
        private Dictionary<ObjectPoolSingle, List<TempItem>> tempEnumIndexObjectDic = new Dictionary<ObjectPoolSingle, List<TempItem>>();//临时存储的对象

        Dictionary<ObjectPoolSingle, List<GameObject>> des_Queue = new Dictionary<ObjectPoolSingle, List<GameObject>>();
        Dictionary<ObjectPoolSingle, List<TempItem>> tempDes_Queue = new Dictionary<ObjectPoolSingle, List<TempItem>>();

        public const int objCount = 100;

        const int tempObjStoreTime=60;//临时存储对象，60秒后销毁

        private void Awake()
        {
            //Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        /// <summary>
        /// 从对象池里面拿东西 枚举 物体名（路径） 是否临时对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectName">物体的全路径</param>
        /// <returns></returns>
        public GameObject GetObjcectFromPool(ObjectPoolSingle objType, string objectName,bool isTemp)
        {
            object obj = null;
        GameObject gameObj = null;
            if (!isTemp)
            {
                if (EnumIndexObjectDic.ContainsKey(objType))
                {
                    if (EnumIndexObjectDic[objType].Count == 0)
                    {
                        if (Resources.Load(objectName) == null)
                        {
                            Debug.LogError(string.Format("找不到名字为{0}的物体", objectName));
                            return null;
                        }
                        obj = GameObject.Instantiate(Resources.Load(objectName));
                    }
                    if (EnumIndexObjectDic[objType].Count > 0)
                    {
                        //拿走该对象
                        obj = EnumIndexObjectDic[objType][0];
                        EnumIndexObjectDic[objType].RemoveAt(0);
                    }
                }
                else
                {
                    //先从回收站取
                    if (des_Queue.ContainsKey(objType) && des_Queue[objType].Count > 0)
                    {
                        int index = des_Queue[objType].Count;
                        obj = des_Queue[objType][index - 1];
                    }
                    else
                    {
                        if (Resources.Load(objectName) == null)
                        {
                            Debug.LogError(string.Format("找不到名字为{0}的物体", objectName));
                            return null;
                        }
                        obj = GameObject.Instantiate(Resources.Load(objectName));

                    }
                }
        }
            else
            {
                if (tempEnumIndexObjectDic.ContainsKey(objType))
                {
                    if (tempEnumIndexObjectDic[objType].Count == 0)
                    {
                        obj = GameObject.Instantiate(Resources.Load(objectName));
                    }
                    if (tempEnumIndexObjectDic[objType].Count > 0)
                    {
                        //拿走该对象
                        obj = tempEnumIndexObjectDic[objType][0].obj;
                        tempEnumIndexObjectDic[objType].RemoveAt(0);
                    }
                }
                else
                {
                    //先从回收站取
                    if (tempDes_Queue.ContainsKey(objType) && tempDes_Queue[objType].Count > 0)
                    {
                        int index = tempDes_Queue[objType].Count;
                        obj = tempDes_Queue[objType][index - 1].obj;
                    }
                    else
                    {
                        obj = GameObject.Instantiate(Resources.Load(objectName));
                    }
                }

           
            }
        gameObj = obj as GameObject;

        //GameObject gameObj = obj as GameObject;
        if (gameObj==null)
            gameObj = GameObject.Instantiate(Resources.Load(objectName)) as GameObject;
            gameObj.SetActive(true);
            return gameObj;
        }


        float gcTime = 1.2f;

        float ticker = 0;

        
        //float clearTempTicker = 0;

        private void LateUpdate()
        {
            ticker += Time.deltaTime;

            //if(haveTempItem)
            //clearTempTicker+=Time.deltaTime;

            if (ticker > gcTime)
            {
                foreach (var t in des_Queue.Keys)
                {
                    CheckPool(t);
                    CheckTempPool(t);
                }

                foreach (var t in des_Queue.Values)
                {
                    if (t.Count > 0)
                    {
                        var temp = t[0];
                        if (temp != null)
                        {
                            DestroyObject(temp);
                        }
                        t.RemoveAt(0);
                        break;
                    }
                }

                foreach (var t in tempDes_Queue.Values)
                {
                    if (t.Count > 0)
                    {
                        var temp = t[0];
                        if(CGameTime.Instance.GetTimeStamp()- temp.storeTime > tempObjStoreTime)
                        {
                            if (temp != null)
                            {
                                DestroyObject(temp.obj);
                            }
                            t.RemoveAt(0);
                            break;
                        }
                    }
                }
                foreach (var t in tempEnumIndexObjectDic.Values)
                {
                    if (t.Count > 0)
                    {
                        var temp = t[0];
                        if (CGameTime.Instance.GetTimeStamp() - temp.storeTime > tempObjStoreTime)
                        {
                            if (temp != null)
                            {
                                DestroyObject(temp.obj);
                            }
                            t.RemoveAt(0);
                            break;
                        }
                    }
                }
                ticker = 0;
            }


     


        }

        /// <summary>
        /// 这里加参数
        /// </summary>
        void CheckTempPool(ObjectPoolSingle type)
        {
            if (tempEnumIndexObjectDic.ContainsKey(type))
            {
                if (tempEnumIndexObjectDic[type].Count > objCount)
                {
                    var obj = tempEnumIndexObjectDic[type][0];
                    tempEnumIndexObjectDic[type].RemoveAt(0);
                    tempDes_Queue[type].Add(obj);
                }
            }
        }
        /// <summary>
        /// 这里加参数
        /// </summary>
        void CheckPool(ObjectPoolSingle type)
        {
            if (EnumIndexObjectDic.ContainsKey(type))
            {
                if (EnumIndexObjectDic[type].Count > objCount)
                {
                    var obj = EnumIndexObjectDic[type][0];
                    EnumIndexObjectDic[type].RemoveAt(0);
                    des_Queue[type].Add(obj);
                }
            }
        }

        /// <summary>
        /// 关闭对象，返还给对象池默认可以存5个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ctrl"></param>
        /// <param name="objCount"></param>
        public void DisappearObjectToPool(ObjectPoolSingle type,GameObject obj,bool isTempObj)
        {
            long storeTime = CGameTime.Instance.GetTimeStamp();
            if (!isTempObj)
            {
                if (!EnumIndexObjectDic.ContainsKey(type))
                {
                    EnumIndexObjectDic.Add(type, new List<GameObject>());
                    des_Queue.Add(type, new List<GameObject>());
                }

                if (obj == null)
                {
                    Debug.LogError("exist null pool obj!");
                }
                //对象返回池子

                obj.SetActive(false);
                obj.transform.SetParent(this.transform, false);
                //防止点快了
                bool canAddToPool = true;
                foreach (GameObject existObj in EnumIndexObjectDic[type])
                {
                    if (existObj.GetInstanceID() == obj.GetInstanceID())
                    {
                        canAddToPool = false;
                        break;
                    }
                }
                if (canAddToPool)
                {
                    EnumIndexObjectDic[type].Add(obj);
                }

            }
            else
            {
                if (!tempEnumIndexObjectDic.ContainsKey(type))
                {
                    tempEnumIndexObjectDic.Add(type, new List<TempItem>());
                    tempDes_Queue.Add(type, new List<TempItem>());
                }

                if (obj == null)
                {
                    Debug.LogError("exist null pool obj!");
                }
                //对象返回池子

                obj.SetActive(false);
                obj.transform.SetParent(this.transform, false);
                //防止点快了
                bool canAddToPool = true;
                foreach (TempItem existTemp in tempEnumIndexObjectDic[type])
                {
                    if (existTemp.obj.GetInstanceID() == obj.GetInstanceID())
                    {
                        canAddToPool = false;
                        break;
                    }
                }
                if (canAddToPool)
                {
                    TempItem tempItem = new TempItem();
                    tempItem.obj = obj;
                    tempItem.storeTime = storeTime;
                    tempEnumIndexObjectDic[type].Add(tempItem);
                }
            }
           

        }



     
    }






    //public interface IPoolObject
    //{
    //    /// <summary>
    //    /// init
    //    /// </summary>
    //    void Spawn();
    //    /// <summary>
    //    /// Finallize
    //    /// </summary>
    //    void UnSpawn();
    //}

