using Framework.Data;
 using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTest : MonoBehaviour
{
     
    public TestSceneManager testSceneManager;



    public BattlePeopleView p1View;
    public BattlePeopleView p2View;
    public int testSkillId;
    public int testSkillId2;//技能id2
    public string testRoleSkill;//角色技能
    public int testEnemySkillId;
    [EnumAttirbute("敌人1技能等级")]
    public int enemySkillLevel1 = 1;
    public string testEnemySkill;//技能id2


    public BattlePanel battlePanel;
    public BattleScenePanel battleScenePanel;

    //敌人测试属性
    [EnumAttirbute("敌人血|防|攻")]
    public string enemyTestPro = "10001|100$10004|50|60$10003|50|60";//敌人基础属性

    [EnumAttirbute("玩家血|防|攻")]
    public string playerTestPro;//玩家基础属性
    public int playerLevel;//玩家等级

    [EnumAttirbute("玩家模型")]
    public BattleTestType playerTestType;
    [EnumAttirbute("敌人测试模式")]
    public BattleTestType enemyTestType;
    [EnumAttirbute("敌人模型")]
    public BaseEnemyModelType baseEnemyModelType;//敌人模型
    public int enemyLevel;//敌人等级





    [EnumAttirbute("敌人有几个阶段")]
    public int phase;//

    [EnumAttirbute("分阶段的敌人的配置id")]
    public int enemySettingId;//

    public static SkillTest Instance;

    public List<TestBattleStudentData> myBattleStudentList = new List<TestBattleStudentData>();
    public List<TestBattleStudentData> enemyStudentList = new List<TestBattleStudentData>();
    public List<PeopleData> p1List=new List<PeopleData>();
    public List<PeopleData> p2List = new List<PeopleData>();


    public SkeletonGraphic ske;
    private void Awake()
    {


    


    }
    private void OnEnable()
    {

        Instance = this;
        testSceneManager.SetTable();
        PanelManager.Instance.trans_layer2 = GameObject.Find("Canvas/Panel/Layer2").transform;

        RoleManager.Instance._CurGameInfo = new GameInfo();
        //RoleManager.Instance._CurGameInfo.playerPeople = GenerateTestSingle(false);
        //RoleManager.Instance._CurGameInfo.playerPeople.portraitType = (int)PortraitType.ChangeFace;
        //RoleManager.Instance._CurGameInfo.playerPeople.gender = (int)Gender.Male;
        ////PeopleData p = RoleManager.Instance._CurGameInfo.playerPeople;
        //RoleManager.Instance.RdmFace(RoleManager.Instance._CurGameInfo.playerPeople);
        //for(int i = 0; i < RoleManager.Instance._CurGameInfo.playerPeople.portraitIndexList.Count; i++)
        //{
        //    //RoleManager.Instance._CurGameInfo.playerPeople.PortraitIndexList.Add(RoleManager.Instance._CurGameInfo.playerPeople.portraitIndexList[i]);
        //}
        RoleManager.Instance._CurGameInfo.MatchData = new MatchData();


        //PeopleData p2 = GenerateTestSingle(true);

        //RoleManager.Instance.AddProperty(PropertyIdType.Hp, 16000);
        //RoleManager.Instance.AddProperty(PropertyIdType.Hp, 16000, p2);


        //RoleManager.Instance.AddBattleProperty(PropertyIdType.Attack,600, p2);
        //if (phase >= 2)
        //{
        //    p2.totalPhase = phase;
        //    p2.curPhase = 1;
        //}
        //p2.enemySettingId = enemySettingId;

        //BattleManager.Instance.p1List.Add(RoleManager.Instance._CurGameInfo.playerPeople);
        //BattleManager.Instance.p2List.Add(p2);
        RoleManager.Instance._CurGameInfo.studentData = new StudentData();

        if (myBattleStudentList.Count > 0)
        {
            for (int i = 0; i < myBattleStudentList.Count; i++)
            {
                TestBattleStudentData testData = myBattleStudentList[i];
                PeopleData peopleData = new PeopleData();
                peopleData.yuanSu = (int)testData.yuanSuType;
                peopleData.gender = (int)testData.gender;
                peopleData.isMyTeam = true;
                peopleData.onlyId = ConstantVal.SetId;
                peopleData.portraitType = (int)PortraitType.ChangeFace;
                if(peopleData.gender==(int)Gender.None)
                peopleData.gender = RandomManager.Next(1, 3);
                RoleManager.Instance.RdmFace(peopleData);
               
                peopleData.name = testData.name;
                List<List<int>> proList = CommonUtil.SplitCfg(ConstantVal.baseEnemyPro);
                for (int j = 0; j < proList.Count; j++)
                {

                    SinglePropertyData singlePropertyData = new SinglePropertyData();
                    List<int> thePro = proList[j];
                    singlePropertyData.id = thePro[0];
                    singlePropertyData.num = SetProNumByStr((PropertyIdType)(int)singlePropertyData.id, testData.pro);

                    if (singlePropertyData.id == (int)PropertyIdType.MpNum)
                    {
                        singlePropertyData.limit = 100;
                        singlePropertyData.num = 100;
                    }
                    else if (singlePropertyData.id == (int)PropertyIdType.Hp)
                    {
                        singlePropertyData.limit = singlePropertyData.num;
                    }
                    //弟子给20暴击率 好看测试
                    else if (singlePropertyData.id == (int)PropertyIdType.CritRate)
                    {
                        singlePropertyData.num = SetProNumByStr((PropertyIdType)(int)singlePropertyData.id, testData.pro);

                        if(singlePropertyData.num == 0)
                        {
                            singlePropertyData.num = 20;
                        }
                        singlePropertyData.limit = singlePropertyData.num;
                    }
                    int proQuality = RandomManager.Next(1, 6);
                    singlePropertyData.quality = proQuality;

                    peopleData.propertyList.Add(singlePropertyData);
                    peopleData.propertyIdList.Add(singlePropertyData.id);

                    SinglePropertyData battle_singlePropertyData = new SinglePropertyData();
                    battle_singlePropertyData.id = singlePropertyData.id;
                    battle_singlePropertyData.num = singlePropertyData.num;
                    battle_singlePropertyData.limit = singlePropertyData.limit;
                    battle_singlePropertyData.quality = proQuality;
                    peopleData.curBattleProIdList.Add(singlePropertyData.id);
                    peopleData.curBattleProList.Add(battle_singlePropertyData);
                    //PeopleData.ProQualityList.Add(proQuality);
                    peopleData.trainIndex = testData.trainIndex;
                    peopleData.xueMai = new XueMaiData();
                    peopleData.xueMai.xueMaiTypeList.Add(XueMaiType.JingTong);
                    peopleData.xueMai.xueMaiTypeList.Add(XueMaiType.ShangHai);
                    peopleData.xueMai.xueMaiLevelList.Add((testData.trainIndex / 10) * 5);
                    peopleData.xueMai.xueMaiLevelList.Add((testData.trainIndex / 10) * 5);

                }



                //BattleManager.Instance.p1List.Add(peopleData);
                if (!string.IsNullOrWhiteSpace(testData.skill))
                {
                    List<List<int>> skill = CommonUtil.SplitCfg(testData.skill);
                    for (int j = 0; j < skill.Count; j++)
                    {
                        List<int> singleSkill = skill[j];
                        int id = singleSkill[0];
                        int level = singleSkill[1];
                        InitSkill(peopleData, id, level);

                    }
                }
                else
                {
                    int skillId = testSkillId;
                    int level = 1;
                    InitSkill(peopleData, skillId, level);

                }

                p1List.Add(peopleData);
                if (i > 0)
                {
                    RoleManager.Instance._CurGameInfo.studentData.allStudentList.Add(peopleData);
                }
                //BattleManager.Instance.InitBattlePro(peopleData, false);

            }
        }
        if (enemyStudentList.Count > 0)
        {
            for (int i = 0; i < enemyStudentList.Count; i++)
            {
                TestBattleStudentData testData = enemyStudentList[i];
                PeopleData peopleData = new PeopleData();
                peopleData.enemySettingId = testData.settingId;
                peopleData.gender = (int)testData.gender;
                peopleData.isMyTeam = false;
                peopleData.onlyId = ConstantVal.SetId;
                peopleData.portraitType = (int)PortraitType.ChangeFace;
                //peopleData.gender = RandomManager.Next(1, 3);
                RoleManager.Instance.RdmFace(peopleData);
                peopleData.name = testData.name;
                List<List<int>> proList = CommonUtil.SplitCfg(ConstantVal.baseEnemyPro);
                for (int j = 0; j < proList.Count; j++)
                {

                    SinglePropertyData singlePropertyData = new SinglePropertyData();
                    List<int> thePro = proList[j];
                    singlePropertyData.id = thePro[0];
                    singlePropertyData.num = SetProNumByStr((PropertyIdType)(int)singlePropertyData.id, testData.pro);

                    if (singlePropertyData.id == (int)PropertyIdType.MpNum)
                    {
                        singlePropertyData.limit = 100;
                    }
                    else if (singlePropertyData.id == (int)PropertyIdType.Hp)
                    {
                        singlePropertyData.limit = singlePropertyData.num;
                    }
                    //弟子给20暴击率 好看测试
                    else if (singlePropertyData.id == (int)PropertyIdType.CritRate)
                    {
                        singlePropertyData.num = 20;
                        singlePropertyData.limit = singlePropertyData.num;
                    }
                    int proQuality = RandomManager.Next(1, 6);
                    singlePropertyData.quality = proQuality;

                    peopleData.propertyList.Add(singlePropertyData);
                    peopleData.propertyIdList.Add(singlePropertyData.id);

                    SinglePropertyData battle_singlePropertyData = new SinglePropertyData();
                    battle_singlePropertyData.id = singlePropertyData.id;
                    battle_singlePropertyData.num = singlePropertyData.num;
                    battle_singlePropertyData.limit = singlePropertyData.limit;
                    battle_singlePropertyData.quality = proQuality;
                    peopleData.curBattleProIdList.Add(singlePropertyData.id);
                    peopleData.curBattleProList.Add(battle_singlePropertyData);
                    //PeopleData.ProQualityList.Add(proQuality);
                }



                //BattleManager.Instance.p2List.Add(peopleData);
                if (!string.IsNullOrWhiteSpace(testData.skill))
                {
                    List<List<int>> skill = CommonUtil.SplitCfg(testData.skill);
                    for (int j = 0; j < skill.Count; j++)
                    {
                        List<int> singleSkill = skill[j];
                        int id = singleSkill[0];
                        int level = singleSkill[1];
                        InitSkill(peopleData, id, level);

                    }
                }
                else
                {
                    int skillId = testSkillId;
                    int level = enemySkillLevel1;
                    InitSkill(peopleData, skillId, level);

                }

                if (phase >= 2)
                {
                    peopleData.totalPhase = phase;
                    peopleData.curPhase = 1;
                }
                peopleData.studentType = (int)StudentType.Enemy;
                //peopleData.enemySettingId = enemySettingId;
                peopleData.yuanSu = (int)testData.yuanSuType;
                peopleData.trainIndex = testData.trainIndex;
                p2List.Add(peopleData);

            }
        }
        RoleManager.Instance._CurGameInfo.playerPeople = p1List[0];
        //好感度
        for (int i = 0; i < p1List.Count; i++)
        {
            PeopleData p = p1List[i];
            for(int j = 0; j < p1List.Count; j++)
            {
                PeopleData theP = p1List[j];
                if (theP != p)
                {
                    if (p.socializationData == null)
                        p.socializationData = new SocializationData();
                    if (!p.socializationData.knowPeopleList.Contains(theP.onlyId))
                    {
                        p.socializationData.knowPeopleList.Add(theP.onlyId);
                        p.socializationData.haoGanDu.Add(0);
                    }
                    int index = p.socializationData.knowPeopleList.IndexOf(theP.onlyId);
                    p.socializationData.haoGanDu[index] = 0;
                }
            }
            p.socializationData.xingGe = RandomManager.Next(1, (int)XingGeType.End);
        }
        //测bug
        PeopleData haoGanP1 = p1List[1];
        PeopleData haoGanP2 = p1List[2];
        int indexHaoGan = haoGanP1.socializationData.knowPeopleList.IndexOf(haoGanP2.onlyId);
        haoGanP1.socializationData.haoGanDu[indexHaoGan] = -100;

        for (int i = 0; i < p2List.Count; i++)
        {
            PeopleData p = p2List[i];
            p.socializationData = new SocializationData();
            p.socializationData.xingGe = RandomManager.Next(1, (int)XingGeType.End);

        }


        BattleManager.Instance.BattlePrepare(ref p1List,ref p2List,false,false);
        //BattleManager.Instance.InitBattlePro(RoleManager.Instance._CurGameInfo.playerPeople, false);
        ////RoleManager.Instance.AddBattleProperty(PropertyIdType.Attack, 600, RoleManager.Instance._CurGameInfo.playerPeople);

        //BattleManager.Instance.InitBattlePro(p2, false);
        //BattleManager.Instance.ClearHuanCunSkill();
        //BattleManager.Instance.HuanCunSkillSetting(BattleManager.Instance.p1List, BattleManager.Instance.p2List);

        //BattleManager.Instance.buffList1 = new List<BattleBuff>();
        //BattleManager.Instance.buffList2 = new List<BattleBuff>();

        //BattleManager.Instance.BattleViewPrepare()
        battlePanel.Init(p1List[0], p2List[0]);
        battlePanel.OnOpenIng();
        battleScenePanel.Init();


        p1View.Init(p1List[0], p2List[0].onlyId, true, 0);
        p1View.enemyOnlyId = p2List[0].onlyId;
        p2View.Init(p2List[0], p1List[0].onlyId, false, 0);
        p2View.enemyOnlyId = p1List[0].onlyId;
        p1View.OnOpenIng();
        p2View.OnOpenIng();
    }
    // Start is called before the first frame update
    void Start()
    {
        //ColdZone();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //BattleManager.Instance.OnZhuZhan(new List<PeopleData> { BattleManager.Instance.p1List[1], BattleManager.Instance.p1List[2] });
            //BattleManager.Instance.AddBattleBuff(BattleManager.Instance.p1List[BattleManager.Instance.p1Index],DataTable.FindBattleBuffSetting((int)BattleBuffIdType.DianLiu));
            ske.initialSkinName = "l";
            ske.Initialize(true);
            ske.AnimationState.SetAnimation(0,"feixing1",false);

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            //BattleManager.Instance.OnZhuZhan(new List<PeopleData> { BattleManager.Instance.p2List[1], BattleManager.Instance.p2List[2] });
            ske.initialSkinName = "pifu2_l";
            ske.Initialize(true);
            ske.AnimationState.SetAnimation(0, "feixing1", false);

        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

    }


    void InitSkill(PeopleData p, int skillId,int level)
    {
        if(p.allSkillData==null)
        p.allSkillData = new AllSkillData();
        SingleSkillData singleSkillData = new SingleSkillData();
        //if (p.IsPlayer)
        //{
        //    singleSkillData.SkillLevel = roleSkillLevel1;

        //}
        //else
        //{
        //    singleSkillData.SkillLevel = enemySkillLevel1;

        //}

        //skillId=(int)SkillManager.Instance.SkillIdByYuanSu((YuanSuType)p.yuanSu);
        singleSkillData.skillLevel = level;
        singleSkillData.skillId = skillId;
        p.allSkillData.skillList.Add(singleSkillData);
        p.allSkillData.equippedSkillIdList.Add(singleSkillData.skillId);


    }


    ///// <summary>
    ///// 生成测试小兵 标准兵 enemylevel 0玩家 1小兵 2精英 3boss 小兵消耗5%的主角血 打10个小兵主角掉1半血 然后打1个精英怪消耗1半血 打boss消耗全部血
    ///// </summary>
    //public PeopleData GenerateTestSingle(bool enemy)
    //{
    //    PeopleData peopleData = new PeopleData();
    //    peopleData.onlyId = ConstantVal.SetId;
    //    //PeopleData.Name = "敌人";
    //    int modelType = 0;
    //    int level = 0;
    //    if (enemy)
    //    {
    //        //性别
    //        int gender = RandomManager.Next(1, 3);
    //        string name = RoleManager.Instance.rdmName((Gender)gender);
    //        peopleData.gender = gender;

    //        peopleData.name = name;
    //        modelType = (int)baseEnemyModelType;
    //        level = enemyLevel;
    //    }
    //    else
    //    {
    //        modelType = 0;
    //        level = playerLevel;
    //        peopleData.name = "叶凡";
    //        peopleData.isPlayer = true;
    //        peopleData.isMyTeam = true;
    //    }
        
    //    List<List<int>> proList = CommonUtil.SplitCfg(ConstantVal.baseEnemyPro);
    //    for (int i = 0; i < proList.Count; i++)
    //    {

    //        SinglePropertyData singlePropertyData = new SinglePropertyData();
    //        List<int> thePro = proList[i];
    //        singlePropertyData.id = thePro[0];

    //        //如果是敌人
    //        if (enemy)
    //        {
    //            if (enemyTestType == BattleTestType.BaseLevel)
    //            {
    //                //if(PeopleData.IsPlayer&&playertestt)
    //                singlePropertyData.num = SetProNum((PropertyIdType)(int)singlePropertyData.id, modelType, level);

    //            }
    //            else
    //            {
    //                if (!peopleData.isPlayer)
    //                {
    //                    singlePropertyData.num = SetProNum((PropertyIdType)(int)singlePropertyData.id);
    //                }
    //                else
    //                {
    //                    singlePropertyData.num = SetProNum((PropertyIdType)(int)singlePropertyData.id, modelType, level);

    //                }

    //            }
    //        }
    //        //玩家
    //        else
    //        {
    //            if (playerTestType == BattleTestType.BaseLevel)
    //            {
    //                singlePropertyData.num = SetProNum((PropertyIdType)(int)singlePropertyData.id, modelType, level);

    //            }
    //            else
    //            {
    //                singlePropertyData.num = SetPlayerProNumByInput((PropertyIdType)(int)singlePropertyData.id);

    //            }
    //            if (singlePropertyData.id == (int)PropertyIdType.MpNum)
    //            {
    //                singlePropertyData.num = 100;
    //            }
    //        }
            
           

    //        if (singlePropertyData.id == (int)PropertyIdType.MpNum)
    //        {
    //            singlePropertyData.limit = 100;
    //        }
    //        else if (singlePropertyData.id == (int)PropertyIdType.Hp)
    //        {
    //            singlePropertyData.limit = singlePropertyData.num;
    //        }
    //        int proQuality = RandomManager.Next(1, 6);
    //        singlePropertyData.quality = proQuality;

    //        peopleData.propertyList.Add(singlePropertyData);
    //        peopleData.propertyIdList.Add(singlePropertyData.id);

    //        SinglePropertyData battle_singlePropertyData = new SinglePropertyData();
    //        battle_singlePropertyData.id = singlePropertyData.id;
    //        battle_singlePropertyData.num = singlePropertyData.num;
    //        battle_singlePropertyData.limit = singlePropertyData.limit;
    //        battle_singlePropertyData.quality = proQuality;
    //        peopleData.curbattleProIdList.Add(singlePropertyData.id);
    //        peopleData.curBattleProList.Add(battle_singlePropertyData);
           
    //        //PeopleData.ProQualityList.Add(proQuality);
    //    }


    //    //PeopleData.StudentRarity = matchLevel;
    //    peopleData.studentQuality = enemyLevel;
    //    peopleData.studentType = (int)StudentType.Enemy;
    //    //初始技能暂定灵弹
    //    //PeopleData.AllSkillData = new AllSkillData();
    //    //SingleSkillData singleSkill = new SingleSkillData();
    //    //singleSkill.SkillId = (int)SkillIdType.LingDan;
    //    //singleSkill.SkillLevel = 1;
    //    //PeopleData.AllSkillData.SkillList.Add(singleSkill);
    //    //PeopleData.AllSkillData.EquippedSkillIdList.Add(singleSkill.SkillId);
    //    //RoleManager.Instance._CurGameInfo.AllPeopleList.Add(PeopleData);//TODO这个改成游戏初始就生成固定的工具人
    //    return peopleData;
    //}
    /// <summary>
    /// 根据配置设置属性
    /// </summary>
    /// <returns></returns>
    public int SetPlayerProNumByInput(PropertyIdType idType)
    {
        List<List<int>> pro = CommonUtil.SplitCfg(playerTestPro);
        for (int i = 0; i < pro.Count; i++)
        {
            List<int> thePro = pro[i];
            if (thePro[0] == (int)idType)
            {
                return thePro[1];
            }
        }
        return 0;
    }

    /// <summary>
    /// 根据配置设置属性
    /// </summary>
    /// <returns></returns>
    public int SetProNum(PropertyIdType idType)
    {
        List<List<int>> pro = CommonUtil.SplitCfg(enemyTestPro);
        for(int i = 0; i < pro.Count; i++)
        {
            List<int> thePro = pro[i];
            if (thePro[0] == (int)idType)
            {
                return thePro[1];
            }
        }
        return 0;
    }

    public int SetProNum(PropertyIdType idType,int enemyType,int curLevel)
    {
        int baseNum = 0;
        float addPerLevel = 1;//每级增加
        switch (enemyType)
        {   
            //玩家
            case 0:
                switch (idType)
                {
                    case PropertyIdType.Hp:
                        //if (playerTestType == PlayerTestType.Rare)
                        //{
                        //    baseNum = 1200;
                        //    addPerLevel = 60;
                        //}
                        //else if(playerTestType == PlayerTestType.Full)
                        //{
                        //    baseNum = 2000;
                        //    addPerLevel = 100;
                        //}
                        baseNum = 2000;
                        addPerLevel = 100;
                        break;
                    case PropertyIdType.Defense:
                        //if (playerTestType == PlayerTestType.Rare)
                        //{
                        //    baseNum = 240;
                        //    addPerLevel = 12;
                        //}
                        //else if(playerTestType == PlayerTestType.Full)
                        //{
                        //    baseNum = 400;
                        //    addPerLevel = 20;
                        //}
                        baseNum = 400;
                        addPerLevel = 20;
                        break;
                    case PropertyIdType.Attack:
                        //if (playerTestType == PlayerTestType.Rare)
                        //{
                        //    baseNum = 100;
                        //    addPerLevel = 5;
                        //}
                        //else if(playerTestType == PlayerTestType.Full)
                        //{
                        //    baseNum = 400;
                        //    addPerLevel = 20;
                        //}
                        baseNum = 400;
                        addPerLevel = 20;
                        break;
                }
                break;
            //小怪
            case 1:
                switch (idType)
                {
                    case PropertyIdType.Hp:
                        baseNum = 80;
                        addPerLevel = 40;
                        break;
                    case PropertyIdType.Defense:
                        baseNum = 20;
                        addPerLevel = 10;
                        break;
                    case PropertyIdType.Attack:
                        baseNum = 20;
                        addPerLevel = 10;
                        break;

                }
                break;
            //精英怪
            case 2:
                switch (idType)
                {
                    case PropertyIdType.Hp:
                        baseNum = 266;
                        addPerLevel = 133f;
                        break;
                    case PropertyIdType.Defense:
                        baseNum = 20;
                        addPerLevel = 10;
                        break;
                    case PropertyIdType.Attack:
                        baseNum = 30;
                        addPerLevel = 15f;
                        break;
                }
                break;
            //boss
            case 3:
                switch (idType)
                {
                    case PropertyIdType.Hp:
                        baseNum = 6000;
                        addPerLevel = 300;
                        break;
                    case PropertyIdType.Defense:
                        baseNum = 500;
                        addPerLevel = 25f;
                        break;
                    case PropertyIdType.Attack:
                        baseNum = 200;
                        addPerLevel = 10;
                        break;
                }
                break;
        }
        return Mathf.RoundToInt((curLevel - 1) * addPerLevel + baseNum);
    }


    /// <summary>
    /// 根据配置设置属性
    /// </summary>
    /// <returns></returns>
    public int SetProNumByStr(PropertyIdType idType,string str)
    {
        List<List<int>> pro = CommonUtil.SplitCfg(str);
        for (int i = 0; i < pro.Count; i++)
        {
            List<int> thePro = pro[i];
            if (thePro[0] == (int)idType)
            {
                return thePro[1];
            }
        }
        return 0;
    }
}

/// <summary>
/// 用于测试的弟子
/// </summary>
[System.Serializable]
public class TestBattleStudentData
{
    public int settingId;
    public string name;
    public string pro;
    public string skill;//技能
    public Gender gender;
    public YuanSuType yuanSuType;//什么元素的
    public int trainIndex;//等级
}

/// <summary>
/// 敌人类型
/// </summary>
public enum BaseEnemyModelType
{
    None=0,
    [EnumAttirbute("小怪")]
    Small=1,
    [EnumAttirbute("精英怪")]
    Middle = 2,
    [EnumAttirbute("Boss")]
    Boss = 3,
}

public enum BattleTestType
{
    None=0,
    [EnumAttirbute("基于等级")]
    BaseLevel,//基于等级
    [EnumAttirbute("基于属性")]
    BasePro,//基于属性
}
/// <summary>
/// 主角测试模型
/// </summary>
public enum PlayerTestType
{
    None=0,
    [EnumAttirbute("裸属性")]
    Rare,
    [EnumAttirbute("顶配")]
    Full,
    [EnumAttirbute("根据输入")]
    ByInput,
    
}