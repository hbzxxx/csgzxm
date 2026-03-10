using cfg;
using Framework.Data;
using Google.Protobuf.Collections;
 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : CommonInstance<BattleManager>
{
    public List<List<int>> winnerRangeIndexLeftList=new List<List<int>>();//各阶段左边赢的
    public List<List<int>> winnerRangeIndexRightList=new List<List<int>>();//各阶段右边赢的
    public List<List<SingleOtherZongMenData>> leftMatch = new List<List<SingleOtherZongMenData>>();//左边对战者

    public List<List<SingleOtherZongMenData>> rightMatch = new List<List<SingleOtherZongMenData>>();//右边对战者

    public List<SingleOtherZongMenData> finalMatch = new List<SingleOtherZongMenData>();//左右决战

    public int curPhase = 0;

    public List<PrepareAnimData> nextPrepareAnimDataList = new List<PrepareAnimData>();//发给比赛准备面板的进度移动的

    public SingleOtherZongMenData thisMatchFinalWinner;//总决赛冠军

    public int roleRank = -1;//角色排名
    public List<ItemData> roleAward;//角色获得的奖励
    public List<ItemData> singleBattlePhaseAward;//单局获得的奖励
    public int beforeRankScore;//打之前的jj分
    //public List<SingleOtherZongMenData> allMatchGroup;//本次参赛的所有人
    public SingleOtherZongMenData curMatchEnemyZongeMen;//当前对手宗门
    //public int equipExpAward;//装备经验奖励
    public int studentBeforeExp;//弟子战斗前经验
    //public List<int> beforeStudentExpList;//打之前的弟子经验

    public LevelInfo curLevelInfo;//

    public bool logicPause;//逻辑上暂停
    public bool someOndeDead;//有人死了
   // public bool endBattle;//结束战斗
    public bool test;//是测试

    public MatchPreparePanel matchPreparePanel;

    public BattleType curBattleType;//战斗类型
    public bool studentBattle;//学生代替战斗
    public PeopleData curBattleStudent;//当前代替战斗的弟子

    public List<PeopleData> p1List=new List<PeopleData>();//当前战斗左边
    public List<PeopleData> p2List = new List<PeopleData>();//当前战斗右边

    public int p1Index;//当前战斗是队员的哪一个
    public int p2Index;//当前战斗是队员的哪一个

    public PeopleData winner;//单次战斗左边赢了

    public int logicPauseReasonNum;
    public List<PeopleData> curZhuZhanStudentList = new List<PeopleData>();//当前助战弟子

    //PeopleData aliveP = null;//活着的人
    public PeopleData changePhaseP;//改变形态
    public PeopleData curEscapeP;//当前准备逃跑者
    bool deadQieRenLeft = false;//左边准备死亡切人
    int deadQieRenLeftIndex = -1;//左边死亡切人的下标
    bool deadQieRenRight = false;//右边边准备死亡切人
    int deadQieRenRightIndex = -1;//右边死亡切人的下标

    public List<int> skillSettingHuanCunIdList;
    public List<SkillSetting> skillSettingHuanCun;
    public List<int> skillUpgradeSettingHuanCunIdList;
    public List<SkillUpgradeSetting> skillUpgradeSettingHuanCun;

    public List<BattleBuff> buffList1;
    public List<BattleBuff> buffList2;

    public List<YuanSuType> yuanSuList1;
    public List<YuanSuType> yuanSuList2;

    public int kangXing1;//抗性1
    public int kangXing2;//抗性2

    public bool leftZhuZhanIng;//左边在助战
    public bool rightZhuZhanIng;//右边在助战

    public List<AudioClip> curSkillAudioClip;//技能音效缓存

    public SingleMapEventData curBattleMapEvent;//当前战斗的地图事件

    public List<PeopleData> curZhuZhanPList=new List<PeopleData>();//当前助战的人
    /// <summary>
    /// 开始比赛
    /// </summary>
    public void StartMatch(List<SingleOtherZongMenData> group)
    {
        curBattleType = BattleType.Match;

        GameSceneManager.Instance.GoToScene(SceneType.MatchPrepare);
        winnerRangeIndexLeftList.Clear();
        winnerRangeIndexRightList.Clear();
        leftMatch.Clear();
        rightMatch.Clear();
        finalMatch.Clear();
        //allMatchGroup = group;
        GenerateAllMatchEnemy(group);
        matchPreparePanel= PanelManager.Instance.OpenPersistentPanel<MatchPreparePanel>(PanelManager.Instance.trans_layer2,true);
        curPhase = 1;
        roleRank = 1;
        beforeRankScore = group[0].theR;
        //保底奖励
        //MatchSetting setting = DataTable.FindMatchSetting(RoleManager.Instance._CurGameInfo.MatchData.CurMatchData.SettingId);
        //List<int> awardNumList = CommonUtil.SplitCfgOneDepth(setting.championAward);
        roleAward = new List<ItemData>();// ItemData();
        ItemData award = new ItemData();
        award.settingId = (int)ItemIdType.RongYuBi;
        award.count = ConstantVal.baseMatchRongYuBi;
        roleAward.Add(award);


        //for (int i=0;i< RoleManager.Instance._CurGameInfo.MatchData.SingleMatchDataList.Count; i++)
        //{
        //    SingleMatchData theData = RoleManager.Instance._CurGameInfo.MatchData.SingleMatchDataList[i];
        //    if(theData.SettingId == RoleManager.Instance._CurGameInfo.MatchData.CurMatchData.SettingId)
        //    {
        //        theData.ParticipateNum++;
        //        theData.LastMatchTime = RoleManager.Instance._CurGameInfo.timeData.Year + "|" 
        //            + RoleManager.Instance._CurGameInfo.timeData.Month + "|" 
        //            + RoleManager.Instance._CurGameInfo.timeData.Week;
        //        break;
        //    }
        //}
    }
    /// <summary>
    /// 进入下一阶段
    /// </summary>
    public void NextLayerPhase()
    {
        curPhase++;
        //回满血
        RoleManager.Instance.FullHp(p1List[p1Index]);
        RoleManager.Instance.FullHp(p2List[p2Index]);
        //InitBattlePro(p1List[p1Index],false);
        //InitBattlePro(p2List[p2Index], false);

        //matchPreparePanel.gameObject.SetActive(true);
        if (matchPreparePanel==null||!matchPreparePanel.gameObject.activeInHierarchy)  
            PanelManager.Instance.OpenPersistentPanel<MatchPreparePanel>(PanelManager.Instance.trans_layer2, false);
        EventCenter.Broadcast(TheEventType.NextPhase);
        nextPrepareAnimDataList.Clear();
    }

    /// <summary>
    /// 生成7个敌人
    /// </summary>
    public void GenerateAllMatchEnemy(List<SingleOtherZongMenData> allMatchGroup)
    {
        RoleManager.Instance.ClearAllEnemy();

        for(int i = 0; i < allMatchGroup.Count; i++)
        {
            SingleOtherZongMenData data = allMatchGroup[i];
      

        }

        List<SingleOtherZongMenData> left1Match = new List<SingleOtherZongMenData>();
        left1Match.Add(allMatchGroup[0]);
        left1Match.Add(allMatchGroup[1]);

        List<SingleOtherZongMenData> left2Match = new List<SingleOtherZongMenData>();
        left2Match.Add(allMatchGroup[2]);
        left2Match.Add(allMatchGroup[3]);

        leftMatch.Add(left1Match);
        leftMatch.Add(left2Match);

        List<SingleOtherZongMenData> right1Match = new List<SingleOtherZongMenData>();
        right1Match.Add(allMatchGroup[4]);
        right1Match.Add(allMatchGroup[5]);

        List<SingleOtherZongMenData> right2Match = new List<SingleOtherZongMenData>();
        right2Match.Add(allMatchGroup[6]);
        right2Match.Add(allMatchGroup[7]);

        rightMatch.Add(right1Match);
        rightMatch.Add(right2Match);

        //return res;
    }

    /// <summary>
    /// 根据enemysetting刷新peopleproto的属性 因为后续可能改表，需要把存档的属性给改了
    /// </summary>
    /// <param name="settingId"></param>
    /// <param name="enemySetting"></param>
    /// <returns></returns>
    public void RefreshEnemyProperty(PeopleData p,EnemySetting enemySetting,int enemyLv)
    {
        UpgradeModeSetting upgradeModeSetting = null;
        if (enemyLv > 0)
        {
            upgradeModeSetting = DataTable._upgradeModeList[enemyLv - 1];
            p.trainIndex = enemyLv - 1;
        }
        List<List<int>> proList = CommonUtil.SplitCfg(enemySetting.Property);

        for (int i = 0; i < proList.Count; i++)
        {

            List<int> singlePro = proList[i];
            int id = singlePro[0];
            int num = singlePro[1];
            ////maotest一定要删
            //if (id == 10001||id==10004)
            //{
            //    num =Mathf.RoundToInt(num/ 1f);
            //}else if (id == 10003f)
            //{
            //    num = Mathf.RoundToInt(num/1f);

            //}

            //新改的敌人和lv有关了
            if (enemyLv > 0 )
            {
                float newGuideParam = 1;
                if (enemySetting.Type.ToInt32() == (int)EnemyType.FuBenMonster)
                {
                    newGuideParam = upgradeModeSetting.NewGuideParam.ToFloat();
                }

                if (id == (int)PropertyIdType.Hp)
                {
                    num = (int)MathF.Round(num * upgradeModeSetting.Param1.ToFloat() * newGuideParam);
                }
                else if (id == (int)PropertyIdType.Defense)
                {
                    num = (int)MathF.Round(num * upgradeModeSetting.Param2.ToFloat() * newGuideParam);
                }
                else if (id == (int)PropertyIdType.Attack)
                {
                    num = (int)MathF.Round(num * upgradeModeSetting.Param3.ToFloat() * newGuideParam);
                }
            }

            SinglePropertyData pro = RoleManager.Instance.FindProperty(id, p);
            pro.limit = num;
            pro.num = num;
            //PeopleData.ProQualityList.Add(proQuality);

        }

        //curbattle的血量变为原来的百分比即可
        for(int i = 0; i < p.curBattleProList.Count; i++)
        {
            SinglePropertyData battlePro = p.curBattleProList[i];
            SinglePropertyData pro = RoleManager.Instance.FindProperty(battlePro.id, p);

            if (battlePro.id == (int)PropertyIdType.Hp || battlePro.id == (int)PropertyIdType.MpNum)
            {
                float rate = battlePro.num / (float)battlePro.limit;
                battlePro.limit = pro.limit;
                battlePro.num = (int)Mathf.RoundToInt(battlePro.limit * rate);
                if (battlePro.num >= battlePro.limit)
                    battlePro.num = (int)battlePro.limit;
            }
            else
            {
                battlePro.limit = pro.limit;
                battlePro.num = pro.num;
            }

        }
        //初始技能暂定灵弹
        p.allSkillData = new AllSkillData();
        if (!string.IsNullOrWhiteSpace(enemySetting.YuanSu))
        {
            p.yuanSu = enemySetting.YuanSu.ToInt32();
        }
        if (!string.IsNullOrWhiteSpace(enemySetting.Skill))
        {
            List<List<int>> theSkill = CommonUtil.SplitCfg(enemySetting.Skill);
            for (int i = 0; i < theSkill.Count; i++)
            {
                List<int> settingSkill = theSkill[i];
                SingleSkillData singleSkill = new SingleSkillData();
                singleSkill.skillId = settingSkill[0];
                int skillLv = settingSkill[1];
                if (enemyLv > 0)
                {
                    skillLv = Mathf.RoundToInt(enemyLv * 50 / (float)150);
                    if (skillLv <= 1)
                        skillLv = 1;
                    if (skillLv >= 50)
                        skillLv = 50;
                }
                singleSkill.skillLevel = skillLv;
                p.allSkillData.skillList.Add(singleSkill);
                p.allSkillData.equippedSkillIdList.Add(singleSkill.skillId);
            }
        }
        else
        {
            int skillLv = 1;
            if (enemyLv > 0)
            {
                skillLv = Mathf.RoundToInt(enemyLv * 25 / (float)150);
                if (skillLv <= 1)
                    skillLv = 1;
                if (skillLv >= 25)
                    skillLv = 25;
            }
            SingleSkillData singleSkill = new SingleSkillData();
            singleSkill.skillId = (int)PuGongIdByYuanSu((YuanSuType)p.yuanSu);// (int)SkillIdType.LingDan;
            singleSkill.skillLevel = skillLv;
            p.allSkillData.skillList.Add(singleSkill);
            p.allSkillData.equippedSkillIdList.Add(singleSkill.skillId);
        }

    }

    /// <summary>
    /// 单纯生成敌人
    /// </summary>
    /// <param name="matchLevel"></param>
    /// <param name="enemyLevel"></param>
    /// <returns></returns>
    public PeopleData GenerateEnemy(EnemySetting enemySetting,int curPhase=1,int enemyLv=0)
    {
        try
        {
            PeopleData peopleData = new PeopleData();
            peopleData.onlyId = ConstantVal.SetId;
            peopleData.curPhase = curPhase;
            peopleData.trainIndex = enemyLv-1;
            UpgradeModeSetting upgradeModeSetting = null;
            if (enemySetting == null)
            {

            }
            if (enemyLv>0)
            {
                upgradeModeSetting = DataTable._upgradeModeList[enemyLv - 1];
            }
            if (!string.IsNullOrWhiteSpace(enemySetting.Phase))
            {
                peopleData.totalPhase = enemySetting.Phase.ToInt32();

            }
            else
            {
                peopleData.totalPhase = peopleData.curPhase;
            }

            //PeopleData.Name = "敌人";
            //float addParam = (1 + (matchLevel - 1) * 0.3f) * (1 + (enemyLevel - 1) * 0.1f);
            peopleData.talent = (int)StudentTalent.LianGong;
            List<List<int>> baseBattleProList = CommonUtil.SplitCfg(ConstantVal.baseBattleProperty);

            List<List<int>> proList = CommonUtil.SplitCfg(enemySetting.Property);
            List<int> haveValIdList = new List<int>();
            List<int> haveValValList = new List<int>();
            for (int i = 0; i < proList.Count; i++)
            {
                List<int> thePro = proList[i];
            
                haveValIdList.Add(thePro[0]);
                haveValValList.Add(thePro[1]);
            }
            for (int i = 0; i < baseBattleProList.Count; i++)
            {
                List<int> singlePro = baseBattleProList[i];
                int id = singlePro[0];
                int num = singlePro[1];
         
                if (haveValIdList.Contains(id))
                {
                    int index = haveValIdList.IndexOf(id);
                    num = haveValValList[index];
                }
             
                //新改的敌人和lv有关了
                if (enemyLv>0  )
                {
                    float newGuideParam = 1;
                    if (enemySetting.Type.ToInt32() == (int)EnemyType.FuBenMonster)
                    {
                        newGuideParam = upgradeModeSetting.NewGuideParam.ToFloat();
                    }

                    if (id== (int)PropertyIdType.Hp)
                    {
                        num = (int)MathF.Round(num * upgradeModeSetting.Param1.ToFloat() * newGuideParam);
                    }
                    else if (id == (int)PropertyIdType.Defense)
                    {
                        num = (int)MathF.Round(num * upgradeModeSetting.Param2.ToFloat() * newGuideParam);
                    }
                    else if (id == (int)PropertyIdType.Attack)
                    {
                        num = (int)MathF.Round(num * upgradeModeSetting.Param3.ToFloat() * newGuideParam);
                    }
                }


                SinglePropertyData singlePropertyData = new SinglePropertyData();
                singlePropertyData.id = id;
                singlePropertyData.num = num;
                singlePropertyData.quality = 1;
                
                if (id == (int)PropertyIdType.MpNum)
                {
                    singlePropertyData.limit = 100;
                }
                else if (id == (int)PropertyIdType.Hp)
                {
                    singlePropertyData.limit = singlePropertyData.num;
                }


                peopleData.propertyList.Add(singlePropertyData);
                peopleData.propertyIdList.Add(singlePropertyData.id);

                SinglePropertyData battle_singlePropertyData = new SinglePropertyData();
                battle_singlePropertyData.id = id;
                battle_singlePropertyData.num = singlePropertyData.num;
                battle_singlePropertyData.limit = singlePropertyData.limit;
                battle_singlePropertyData.quality = 1;

                peopleData.curBattleProIdList.Add(id);
                peopleData.curBattleProList.Add(battle_singlePropertyData);
                //PeopleData.ProQualityList.Add(proQuality);
            }
            //性别
            int gender = 0;
            if (!string.IsNullOrWhiteSpace(enemySetting.Gender))
            {
                gender = enemySetting.Gender.ToInt32();
            }else if (string.IsNullOrWhiteSpace(enemySetting.Ske))
            {
                gender= RandomManager.Next(1, 3);
            }
            string name = enemySetting.Name;
            peopleData.name = name;
            peopleData.gender = gender;

            if (!string.IsNullOrWhiteSpace(enemySetting.SpecialPortrait))
            {
                peopleData.specialPortrait = true;
                peopleData.specialPortraitName = enemySetting.SpecialPortrait;
            }
            else
            {
                RoleManager.Instance.RdmFace(peopleData);

            }

            //PeopleData.StudentRarity = matchLevel;
            //PeopleData.StudentQuality = enemyLevel;
            if (!string.IsNullOrWhiteSpace(enemySetting.YuanSu))
            {
                peopleData.yuanSu = enemySetting.YuanSu.ToInt32();
            }
            peopleData.enemySettingId = enemySetting.Id.ToInt32();
            peopleData.studentType = (int)StudentType.Enemy;
            //初始技能暂定灵弹
            peopleData.allSkillData = new AllSkillData();
            if (!string.IsNullOrWhiteSpace(enemySetting.Skill))
            {
                List<List<int>> theSkill = CommonUtil.SplitCfg(enemySetting.Skill);
                for (int i = 0; i < theSkill.Count; i++)
                {
                    List<int> settingSkill = theSkill[i];
                    SingleSkillData singleSkill = new SingleSkillData();
                    singleSkill.skillId = settingSkill[0];
                    int skillLv= settingSkill[1];
                    if (enemyLv > 0)
                    {
                        skillLv =Mathf.RoundToInt(enemyLv * 50 / (float)150);
                        if (skillLv <= 1)
                            skillLv = 1;
                        if (skillLv >= 50)
                            skillLv = 50;
                    }
                    singleSkill.skillLevel = skillLv;
                    peopleData.allSkillData.skillList.Add(singleSkill);
                    peopleData.allSkillData.equippedSkillIdList.Add(singleSkill.skillId);
                }
            }
            else
            {
                int skillLv = 1;
                if (enemyLv > 0)
                {
                    skillLv = Mathf.RoundToInt(enemyLv * 25 / (float)150);
                    if (skillLv <= 1)
                        skillLv = 1;
                    if (skillLv >= 25)
                        skillLv = 25;
                }
                SingleSkillData singleSkill = new SingleSkillData();
                singleSkill.skillId = (int)PuGongIdByYuanSu((YuanSuType)peopleData.yuanSu);// (int)SkillIdType.LingDan;
                singleSkill.skillLevel = skillLv;
                peopleData.allSkillData.skillList.Add(singleSkill);
                peopleData.allSkillData.equippedSkillIdList.Add(singleSkill.skillId);
            }

            //RoleManager.Instance._CurGameInfo.AllPeopleList.Add(PeopleData);//TODO这个改成游戏初始就生成固定的工具人
            return peopleData;
        }catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }

    }

    /// <summary>
    /// 生成玩家队伍成员（协战队友）
    /// </summary>
    /// <param name="name">成员名字</param>
    /// <param name="gender">性别 1男 2女</param>
    /// <param name="yuanSu">元素类型</param>
    /// <param name="hp">血量</param>
    /// <param name="attack">攻击力</param>
    /// <param name="defense">防御力</param>
    /// <param name="skillIdList">技能ID列表（第一个为普攻）</param>
    /// <param name="skillLevelList">技能等级列表</param>
    /// <returns></returns>
    public PeopleData GeneratePlayerTeamMember(string name, int gender, int yuanSu, int hp, int attack, int defense,
        List<int> skillIdList = null, List<int> skillLevelList = null)
    {
        try
        {
            PeopleData peopleData = new PeopleData();
            peopleData.onlyId = ConstantVal.SetId;
            peopleData.curPhase = 1;
            peopleData.totalPhase = 1;
            peopleData.isMyTeam = true;
            peopleData.isPlayer = false; // 不是主角，是队友
            peopleData.name = name;
            peopleData.gender = gender;
            peopleData.yuanSu = yuanSu;
            peopleData.talent = (int)StudentTalent.LianGong;

            // 随机生成外貌
            //RoleManager.Instance.RdmFace(peopleData);

            // 初始化基础战斗属性
            List<List<int>> baseBattleProList = CommonUtil.SplitCfg(ConstantVal.baseBattleProperty);
            for (int i = 0; i < baseBattleProList.Count; i++)
            {
                List<int> singlePro = baseBattleProList[i];
                int id = singlePro[0];
                int num = singlePro[1];

                // 根据传入参数设置属性
                if (id == (int)PropertyIdType.Hp)
                {
                    num = hp;
                }
                else if (id == (int)PropertyIdType.Attack)
                {
                    num = attack;
                }
                else if (id == (int)PropertyIdType.Defense)
                {
                    num = defense;
                }

                SinglePropertyData singlePropertyData = new SinglePropertyData();
                singlePropertyData.id = id;
                singlePropertyData.num = num;
                singlePropertyData.quality = 1;

                if (id == (int)PropertyIdType.MpNum)
                {
                    singlePropertyData.limit = 100;
                }
                else if (id == (int)PropertyIdType.Hp)
                {
                    singlePropertyData.limit = singlePropertyData.num;
                }

                peopleData.propertyList.Add(singlePropertyData);
                peopleData.propertyIdList.Add(singlePropertyData.id);

                // 战斗属性
                SinglePropertyData battle_singlePropertyData = new SinglePropertyData();
                battle_singlePropertyData.id = id;
                battle_singlePropertyData.num = singlePropertyData.num;
                battle_singlePropertyData.limit = singlePropertyData.limit;
                battle_singlePropertyData.quality = 1;

                peopleData.curBattleProIdList.Add(id);
                peopleData.curBattleProList.Add(battle_singlePropertyData);
            }

            // 初始化技能
            peopleData.allSkillData = new AllSkillData();
            if (skillIdList != null && skillIdList.Count > 0)
            {
                for (int i = 0; i < skillIdList.Count; i++)
                {
                    SingleSkillData singleSkill = new SingleSkillData();
                    singleSkill.skillId = skillIdList[i];
                    singleSkill.skillLevel = (skillLevelList != null && i < skillLevelList.Count) ? skillLevelList[i] : 1;
                    peopleData.allSkillData.skillList.Add(singleSkill);
                    peopleData.allSkillData.equippedSkillIdList.Add(singleSkill.skillId);
                }
            }
            else
            {
                // 默认普攻技能
                SingleSkillData singleSkill = new SingleSkillData();
                singleSkill.skillId = (int)PuGongIdByYuanSu((YuanSuType)yuanSu);
                singleSkill.skillLevel = 1;
                peopleData.allSkillData.skillList.Add(singleSkill);
                peopleData.allSkillData.equippedSkillIdList.Add(singleSkill.skillId);
            }

            return peopleData;
        }
        catch (Exception e)
        {
            Debug.LogError("GeneratePlayerTeamMember Error: " + e);
            return null;
        }
    }

    /// <summary>
    /// 生成玩家队伍成员（简化版，使用配置表）
    /// </summary>
    /// <param name="memberSetting">队友配置</param>
    /// <param name="memberLv">队友等级</param>
    /// <returns></returns>
    public PeopleData GeneratePlayerTeamMemberBySetting(EnemySetting memberSetting, int memberLv = 0)
    {
        try
        {
            // 复用GenerateEnemy的逻辑，但修改为玩家队伍
            PeopleData peopleData = GenerateEnemy(memberSetting, 1, memberLv);
            if (peopleData != null)
            {
                peopleData.isMyTeam = true;
                peopleData.isPlayer = false;
                //peopleData.studentType = (int)StudentType.Student;
            }
            return peopleData;
        }
        catch (Exception e)
        {
            Debug.LogError("GeneratePlayerTeamMemberBySetting Error: " + e);
            return null;
        }
    }

    /// <summary>
    /// 生成主线关的敌人
    /// </summary>
    /// <param name="matchLevel"></param>
    /// <param name="enemyLevel"></param>
    /// <returns></returns>
    public List<PeopleData> GenerateMainLevelEnemy(SingleLevelData singleLevelData)
    {
        try
        {
            if (singleLevelData.LevelId == "20016")
            {

            }
            List<PeopleData> res = new List<PeopleData>();
            singleLevelData.Enemy.Clear();
            LevelSetting levelSetting = DataTable.FindLevelSetting(singleLevelData.LevelId);

            if ((LevelType)levelSetting.Leveltype.ToInt32() != LevelType.ZhongZhuanZhan)
            {
                List<int> enemyIdList = CommonUtil.SplitCfgOneDepth(levelSetting.Enemy);
                for (int i = 0; i < enemyIdList.Count; i++)
                {
                    int theId = enemyIdList[i];
                    EnemySetting enemySetting = DataTable.FindEnemySetting(theId);

                    PeopleData PeopleData =GenerateEnemy(enemySetting,1, levelSetting.EnemyLevel.ToInt32());
                    singleLevelData.Enemy.Add(PeopleData);
                    res.Add(PeopleData);
                }

                return res;
            }
        }catch(Exception e)
        {
            Debug.LogError(e);
        }
    

        return null;
         
    }

    /// <summary>
    /// 生成秘境的敌人
    /// </summary>
    /// <param name="matchLevel"></param>
    /// <param name="enemyLevel"></param>
    /// <returns></returns>
    public PeopleData GenerateMiJingBattleEnemy(EnemySetting enemySetting,int enemyLv)
    {
        
        PeopleData peopleData = GenerateEnemy(enemySetting,1,enemyLv);
        return peopleData;

    }

    /// <summary>
    /// 找到某个otherzongmen并且保存
    /// </summary>
    public SingleOtherZongMenData FindOtherZongmenAndSave(SingleOtherZongMenData data)
    {
        SingleOtherZongMenData protoData = data;
        if (data.pList[0].onlyId == MatchManager.Instance.playerZongMenData.simplePList[0].onlyId)
            return MatchManager.Instance.playerZongMenData;

        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList.Count; i++)
        {
            SingleOtherZongMenData theZong = RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList[i];
            if (theZong.simplePList[0].onlyId == protoData.simplePList[0].onlyId)
            {
                theZong = protoData;
                return theZong;
            }
        }
        
        return null;
    }
 
   
    /// <summary>
    /// 开始擂台战斗
    /// </summary>
    public void StartBattle()
    {
        studentBattle = false;
        //打开玩家和电脑战斗面板
        if (leftMatch[0][0].isPlayerZongMen)
        {
            List<PeopleData> p1List = leftMatch[0][0].pList;
            List<PeopleData> p2List = null;
            curMatchEnemyZongeMen = null;
            if (curPhase < 3)
            {
                curMatchEnemyZongeMen = leftMatch[0][1];
            }
            else
            {
                curMatchEnemyZongeMen = rightMatch[0][0];
            }
            p2List = curMatchEnemyZongeMen.pList;

            List<PeopleData> theP1List = new List<PeopleData>();
 
            List<PeopleData> theP2List = new List<PeopleData> ();
            for(int i = 0; i < p1List.Count; i++)
            {
                theP1List.Add(p1List[i]);
            }
            for (int i = 0; i < p2List.Count; i++)
            {
                theP2List.Add(p2List[i]);
            }

            BattleManager.Instance.BattlePrepare(ref theP1List, ref theP2List, false,false);
            BattleViewPrepare();
            GameSceneManager.Instance.GoToScene(SceneType.Battle);

        }
        else
        {
            // 玩家死了 只能是电脑打电脑
            //只有第二轮可能没有玩家
            if (curPhase == 2)
            {
                List<int> leftWinnerIndexList = new List<int>();
                //2组人模拟
                PeopleData winner = MimicBattle(leftMatch[0][0].pList[0], leftMatch[0][1].pList[0]);
                int winnerIndex = 0;
                bool leftWin = false;
                //上面的赢了
                if (winner.onlyId == leftMatch[0][0].pList[0].onlyId)
                {
                    winnerIndex = 0;
                    nextPrepareAnimDataList.Add(new PrepareAnimData(true, curPhase, 1));
                    leftWinnerIndexList.Add(1);
                    leftWin = true;
                }
                else
                {
                    winnerIndex = 1;
                    nextPrepareAnimDataList.Add(new PrepareAnimData(true, curPhase, 2));
                    leftWinnerIndexList.Add(2);
                    leftWin = false;
                }
                //先保存数据 再改变分数
                SingleOtherZongMenData zong1 =FindOtherZongmenAndSave(leftMatch[0][0]);
                SingleOtherZongMenData zong2= FindOtherZongmenAndSave(leftMatch[0][1]);
                MatchManager.Instance.MatchScoreChange(zong1, zong2, leftWin);

                SingleOtherZongMenData winnerZong = leftMatch[0][winnerIndex];
                leftMatch.Clear();
                leftMatch.Add(new List<SingleOtherZongMenData>() { winnerZong });
                winnerRangeIndexLeftList.Add(leftWinnerIndexList);

                List<int> rightWinnerIndexList = new List<int>();

                PeopleData rightWinner1 = MimicBattle(rightMatch[0][0].pList[0], rightMatch[0][1].pList[1]);
                bool leftWin2 = false;
                int rightwinnerIndex = 0;
                if (rightWinner1.onlyId == rightMatch[0][0].pList[0].onlyId)
                {
                    rightwinnerIndex = 0;
                    nextPrepareAnimDataList.Add(new PrepareAnimData(false, curPhase, 1));
                    rightWinnerIndexList.Add(1);
                    leftWin2 = true;
                }
                else
                {
                    rightwinnerIndex = 1;
                    nextPrepareAnimDataList.Add(new PrepareAnimData(false, curPhase, 2));
                    rightWinnerIndexList.Add(2);
                    leftWin2 = false;
                }
                //先保存数据 再改变分数
                SingleOtherZongMenData zong11 = FindOtherZongmenAndSave(rightMatch[0][0]);
                SingleOtherZongMenData zong22 = FindOtherZongmenAndSave(rightMatch[0][1]);
                MatchManager.Instance.MatchScoreChange(zong11, zong22, leftWin2);

                winnerRangeIndexRightList.Add(rightWinnerIndexList);
                SingleOtherZongMenData winnerZongRight = rightMatch[0][rightwinnerIndex];

                rightMatch.Clear();
                rightMatch.Add(new List<SingleOtherZongMenData>() { winnerZongRight });

            }
            else if (curPhase == 3)
            {
                //1组人模拟
                PeopleData winner = MimicBattle(leftMatch[0][0].pList[0], rightMatch[0][0].pList[0]);
                SingleOtherZongMenData winnerZong = null;
                bool leftWin = false;
                if (winner.onlyId == leftMatch[0][0].pList[0].onlyId)
                {
                    winnerZong = leftMatch[0][0];
                    nextPrepareAnimDataList.Add(new PrepareAnimData(true, curPhase, 1));
                    winnerRangeIndexLeftList.Add(new List<int> { 1 });
                    leftWin = true;
                }
                else
                {
                    winnerZong = rightMatch[0][0];
                    nextPrepareAnimDataList.Add(new PrepareAnimData(false, curPhase, 2));
                    winnerRangeIndexRightList.Add(new List<int> { 1});
                    leftWin = false;
                }
                //先保存数据 再改变分数
                SingleOtherZongMenData zong11 = FindOtherZongmenAndSave(leftMatch[0][0]);
                SingleOtherZongMenData zong22 = FindOtherZongmenAndSave(rightMatch[0][0]);
                MatchManager.Instance.MatchScoreChange(zong11, zong22, leftWin);

                thisMatchFinalWinner = winnerZong;

            }
            NextLayerPhase();

        }




    }
    /// <summary>
    /// 战斗场景准备 
    /// </summary>
    public void BattleViewPrepare()
    {
        logicPauseReasonNum = 0;
        logicPause = false;
        //endBattle = false;
        someOndeDead = false;
        curZhuZhanStudentList.Clear();
    }

    /// <summary>
    /// 暂停流动逻辑
    /// </summary>
    public void AddLogicPause()
    {
        logicPauseReasonNum++;
        if (logicPauseReasonNum > 0)
            logicPause = true;
    }
    /// <summary>
    /// 重启流动逻辑
    /// </summary>
    public void RemoveLogicPause()
    {
        logicPauseReasonNum--;
        if (logicPauseReasonNum <= 0)
        {
            logicPauseReasonNum = 0;
            logicPause = false;
        }
        
    }
    /// <summary>
    /// 重启流动逻辑
    /// </summary>
    public void ClearLogicPause()
    {
        logicPauseReasonNum = 0;
        logicPause = false;
    }

    public void BattlePrepare(ref List<PeopleData> theP1List,ref List<PeopleData> theP2List,bool continueBattle1,bool continueBattle2)
    {

        //PeopleData[] pArr = FindBattlePeople(p1.OnlyId, p2.OnlyId);
        //p1 = pArr[0];
        //p2 = pArr[1];
        p1List = theP1List;
        p2List = theP2List;
        p1Index = SequenceChooseHaveHPPIndex(theP1List);
        p2Index = SequenceChooseHaveHPPIndex(theP2List);
        for (int i = 0; i < p1List.Count; i++)
        {
            PeopleData p1 = p1List[i];
            p1.isMyTeam = true;
            InitBattlePro(p1, continueBattle1);

        }
        for (int i = 0; i < p2List.Count; i++)
        {
            PeopleData p2 = p2List[i];
            p2.isMyTeam = false;
            InitBattlePro(p2, continueBattle1);
        }
        ClearHuanCunSkill();
        HuanCunSkillSetting(p1List, p2List);
        InitXieZhanParam(p1List);
        InitXieZhanParam(p2List);
        buffList1 = new List<BattleBuff>();
        buffList2 = new List<BattleBuff>();
        yuanSuList1 = new List<YuanSuType>();
        yuanSuList2 = new List<YuanSuType>();
        kangXing1 = 0;
        kangXing2 = 0;
        deadQieRenLeft = false;
        deadQieRenRight = false;
        deadQieRenLeftIndex = -1;
        deadQieRenRightIndex = -1;
        leftZhuZhanIng = false;
        rightZhuZhanIng = false;

        //curBattleP2.IsMyTeam = false;
        //InitBattlePro(p2, continueBattle2);
    }

    /// <summary>
    /// 初始携战数据
    /// </summary>
    public void InitXieZhanParam(List<PeopleData> pList)
    {
        for(int i = 0; i < pList.Count; i++)
        {
            PeopleData p = pList[i];
            p.xieZhanCDDic.Clear();
            for(int j = 0; j < pList.Count; j++)
            {
                PeopleData p2 = pList[j];
                if (p != p2)
                {
                    if (p.socializationData == null)
                        continue;
                    HaoGanLevelType haoGanLevelType= StudentManager.Instance.GetStudentHaoGanLevelType(p, p2);
                    bool add = false;
                    int cd = 0;
                    if (haoGanLevelType == HaoGanLevelType.Good)
                    {
                        cd = ConstantVal.littleHaoGanXieZhanCD;
                        add = true;
                    }
                    else if(haoGanLevelType == HaoGanLevelType.Great)
                    {
                        cd = ConstantVal.middleHaoGanXieZhanCD;
                        add = true;
                    }else if (haoGanLevelType == HaoGanLevelType.VeryGreat
                        || haoGanLevelType == HaoGanLevelType.Daolv
                        || haoGanLevelType == HaoGanLevelType.DaoYou)
                    {
                        cd = ConstantVal.bigHaoGanXieZhanCD;
                        add = true;
                    }
                    if (add)
                    {
                        if (!p.xieZhanCDDic.ContainsKey(p2.onlyId))
                            p.xieZhanCDDic.Add(p2.onlyId, new List<int>());

                        p.xieZhanCDDic[p2.onlyId].Add(0);
                        p.xieZhanCDDic[p2.onlyId].Add(cd);
                    }
                }
            }

        }
    }

    /// <summary>
    /// 选择有血的那个
    /// </summary>
    public int SequenceChooseHaveHPPIndex(List<PeopleData> pList)
    {
        for(int i = 0; i < pList.Count; i++)
        {
            PeopleData theP = pList[i];
            if(RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, theP).num > 0)
            {
                return i;
            }
        }
        return 0;
    }

    /// <summary>
    /// 缓存技能
    /// </summary>
    public void HuanCunSkillSetting(List<PeopleData> list1,List<PeopleData> list2)
    {
        //skillSettingHuanCun = new List<SkillSetting>();
        //skillUpgradeSettingHuanCun = new List<SkillUpgradeSetting>();
        for(int i = 0; i < list1.Count; i++)
        {
            PeopleData p = list1[i];
            for(int j = 0; j < p.allSkillData.equippedSkillIdList.Count; j++)
            {
                int id = p.allSkillData.equippedSkillIdList[j];
                SkillSetting setting = DataTable.FindSkillSetting(id);
                if (!skillSettingHuanCunIdList.Contains(id))
                {
                    skillSettingHuanCunIdList.Add(id);
                    skillSettingHuanCun.Add(setting);
                    for (int k = 0; k < DataTable._skillUpgradeList.Count; k++)
                    {
                        SkillUpgradeSetting upgradesetting = DataTable._skillUpgradeList[k];
                        int upgradeId = upgradesetting.Id.ToInt32();
                        if (upgradesetting.SkillId.ToInt32() == id
                            && !skillUpgradeSettingHuanCunIdList.Contains(upgradeId))
                        {
                            skillUpgradeSettingHuanCun.Add(upgradesetting);
                            skillUpgradeSettingHuanCunIdList.Add(upgradeId);
                        }
                    }
                }
           
            }
        }
        for (int i = 0; i < list2.Count; i++)
        {
            PeopleData p = list2[i];
            for (int j = 0; j < p.allSkillData.equippedSkillIdList.Count; j++)
            {
                int id = p.allSkillData.equippedSkillIdList[j];
                SkillSetting setting = DataTable.FindSkillSetting(id);
                if (!skillSettingHuanCun.Contains(setting))
                {
                    skillSettingHuanCun.Add(setting);
                    for (int k = 0; k < DataTable._skillUpgradeList.Count; k++)
                    {
                        SkillUpgradeSetting upgradesetting = DataTable._skillUpgradeList[k];
                        if (upgradesetting.SkillId.ToInt32() == id
                            && !skillUpgradeSettingHuanCun.Contains(upgradesetting))
                        {
                            skillUpgradeSettingHuanCun.Add(upgradesetting);

                        }
                    }
                }

            }
        }
    }


    /// <summary>
    /// 清除缓存技能
    /// </summary>
    public void ClearHuanCunSkill()
    {
        skillSettingHuanCunIdList = new List<int>();
        skillUpgradeSettingHuanCunIdList = new List<int>();
        skillSettingHuanCun = new List<SkillSetting>();
        skillUpgradeSettingHuanCun = new List<SkillUpgradeSetting>();
    }

    /// <summary>
    /// 初始化战斗属性
    /// </summary>
    public void InitBattlePro(PeopleData peopleData,bool continueBattle)
    {
 


        List<SinglePropertyData> proList = null;
        if (continueBattle)
        {
            proList = RoleManager.Instance.GetContinueBattlePro(peopleData);
        }
        else
        {
            
            proList = RoleManager.Instance.GetTotalBattlePro(peopleData);
            peopleData.curBattleProIdList.Clear();
            peopleData.curBattleProList.Clear();
            for (int i = 0; i < proList.Count; i++)
            {
                peopleData.curBattleProIdList.Add(proList[i].id);
                peopleData.curBattleProList.Add(proList[i]);
            }
        }
        //技能cd归零
        for(int i = 0; i < peopleData.allSkillData.equippedSkillIdList.Count; i++)
        {
            SingleSkillData data = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(peopleData.allSkillData.equippedSkillIdList[i],
              peopleData.allSkillData);
            data.cd = 0;
            SkillSetting skillSetting = DataTable.FindSkillSetting(data.skillId);
            if (!string.IsNullOrWhiteSpace(skillSetting.YuanSu))
            {
                data.yuanSuType = (YuanSuType)skillSetting.YuanSu.ToInt32();
            }
        }
        peopleData.allSkillData.lastUseSkillIndex = 0;
        peopleData.feiYunCD = 0;
        peopleData.diGuFuHuoed = false;
    }

    /// <summary>
    /// 模拟打到一方胜利为止 暂定p1先手 模拟战斗暂定改成随机战斗
    /// </summary>
    public PeopleData MimicBattle(PeopleData p1, PeopleData p2)
    {
        List<PeopleData> theP1List = new List<PeopleData> { p1 };
        List<PeopleData> theP2List = new List<PeopleData> { p2 };


        BattleManager.Instance.BattlePrepare(ref theP1List, ref theP2List, true, true);
        PeopleData[] pArr = FindBattlePeople(p1.onlyId, p2.onlyId);
        PeopleData curAttackPeople = pArr[0];
        PeopleData curBeattackPeople = pArr[1];



        //BattleManager.Instance.BattlePrepare (ref curAttackPeople,ref curBeattackPeople,true,true);

        //伤害记录
        List<int> people1LoseHpList = new List<int>();
        List<int> people2LoseHpList = new List<int>();
        while (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, curAttackPeople).num>0
            && RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, curBeattackPeople).num > 0)
        {
            AttackResData res= CalcAttack(curAttackPeople, curBeattackPeople);
            if (res.deHpPeople.onlyId == p1.onlyId)
            {
                people1LoseHpList.Add(res.deHp);
            }
            else
            {
                people2LoseHpList.Add(res.deHp);
            }
            //交换
            var tmp = curAttackPeople;
            curAttackPeople = curBeattackPeople;
            curBeattackPeople = tmp;
        }
        //有一方死了
        PeopleData winner = null;
        if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, curAttackPeople).num > 0)
        {
            winner = curAttackPeople;
        }
        else if(RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, curBeattackPeople).num > 0)
        {
            winner = curBeattackPeople;
        }
        EventCenter.Broadcast(TheEventType.MimicBattleRes, people1LoseHpList, people2LoseHpList);
        return winner;
        //EventCenter.Broadcast(TheEventType.BattleEnd, winner,false);
    }

    /// <summary>
    /// 单次攻击 玩家和电脑打
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    public AttackResData SingleAttack(PeopleData p1, SingleSkillData skill=null,int damageIndex=-1,bool xieZhan=false)
    {
        //PeopleData[] pArr = FindBattlePeople(id1, id2);
        //PeopleData p1 = pArr[0];
        //PeopleData p2 = pArr[1];
        PeopleData p2 = null;
        if (CheckIfLeftP(p1))
            p2 = p2List[p2Index];
        else
            p2 = p1List[p1Index];

        AttackResData res= CalcAttack(p1, p2, skill,damageIndex,null,ReactionType.None, xieZhan);


        if (curSkillAudioClip!=null&& curSkillAudioClip.Count > 0)
        {
            AuditionManager.Instance.PlayVoice(Camera.main.transform, curSkillAudioClip[damageIndex]);
        }
      
        EventCenter.Broadcast(TheEventType.YuanSuReaction, res);
        EventCenter.Broadcast(TheEventType.BattleDeHpShow, res);
        EventCenter.Broadcast(TheEventType.BattleBeHit, res);
        
        if (res!=null
            &&res.fanShangResData != null)
        {

            EventCenter.Broadcast(TheEventType.BattleDeHpShow, res.fanShangResData);
            EventCenter.Broadcast(TheEventType.BattleBeHit, res.fanShangResData);
        }
        return res;
        
    }
    ///// <summary>
    ///// 增加战斗能量
    ///// </summary>
    //public void AddBattleEnergy(PeopleData p,int num)
    //{
    //    p.CurBattleEnergy += num;
    //    if (p.CurBattleEnergy >= 100)
    //        p.CurBattleEnergy = 100;
    //    EventCenter.Broadcast(TheEventType.BattleEnergyChange, p);
    //    RoleManager.Instance.DeBattleProperty
    //}
    ///// <summary>
    ///// 减少战斗能量
    ///// </summary>
    //public void DeBattleEnergy(PeopleData p, int num)
    //{
    //    p.CurBattleEnergy += num;
    //    if (p.CurBattleEnergy >= 100)
    //        p.CurBattleEnergy = 100;
    //    if (p.CurBattleEnergy <= 0)
    //        p.CurBattleEnergy = 0;
    //    EventCenter.Broadcast(TheEventType.BattleEnergyChange, p);
    //}
    /// <summary>
    /// 判断在单次战斗中人是否死了
    /// </summary>
    public void JudgeIfPeopleDead()
    {
        if (someOndeDead)
            return;

        //PeopleData[] pArr = FindBattlePeople(p1Id, p2Id);
        PeopleData p1 = p1List[p1Index];
        PeopleData p2 = p2List[p2Index];
        PeopleData deadPeople = null;
        int p1Hp = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p1).num;
        int p2Hp = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p2).num;

      
        //这里处理可能俩人都死情况
        if (p1Hp <= 0
            || p2Hp <= 0)
        {
            if (p1Hp <= 0)
            {
                OnSinglePeopleDead(p1);

            }
            if (p2Hp <= 0)
            {
                OnSinglePeopleDead(p2);

            }

   

    
        }
    }
    /// <summary>
    /// 某人死了
    /// </summary>
    /// <param name="p"></param>
    void OnSinglePeopleDead(PeopleData p)
    {
        someOndeDead = true;
        AddLogicPause();
        bool leftDead = false;
        PeopleData winnerP = null;
        //左边
        if (p.onlyId == p1List[p1Index].onlyId)
        {
            leftDead = true;
            winnerP = p2List[p2Index];
        }
        else if (p.onlyId == p2List[p2Index].onlyId)
        {
            leftDead = false;
            winnerP = p1List[p1Index];
        }
        //RemoveAllBuff(leftDead);

        if (p.totalPhase > 1 && p.curPhase < p.totalPhase)
        {
            //deadPeople.CurPhase++;
            //CheckToNextPhase(deadPeople);
            changePhaseP = p;
            //变化
            EventCenter.Broadcast(TheEventType.BattlePeopleChangePhase, p.onlyId);
            //InitBattlePro(deadPeople, false);

        }
        else
        {

            PeopleData aliveP = null;
            //deadQieRenLeftIndex = -1;
            //deadQieRenRightIndex = -1;
            //左边
            if (p.onlyId == p1List[p1Index].onlyId)
            {

                deadQieRenLeft = true;
                aliveP = CheckAlivePeople(true);

            }       //左边
            else if (p.onlyId == p2List[p2Index].onlyId)
            {
                deadQieRenRight = true;

                aliveP = CheckAlivePeople(false);

            }
            //如1没死则触发buff
            if (winnerP != null)
            {
                List<EquipTaoZhuangType> taoZhuangList = EquipmentManager.Instance.CheckEquipTaoZhuang(winnerP);

                if (taoZhuangList.Count >= 2
                    && taoZhuangList[0] == taoZhuangList[1])
                {
                    //神风
                    if (taoZhuangList.Contains(EquipTaoZhuangType.ShenFeng))
                    {
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[0]);
                        int buffId = taoZhuangSetting.Param2.ToInt32();
                        AddBattleBuff(winnerP, DataTable.FindBattleBuffSetting(buffId));
                 
                    }
                 
                }
            }
            EventCenter.Broadcast(TheEventType.BattlePeopleDead, p.onlyId);
            PanelManager.Instance.CloseTaskGuidePanel();

            //Debug.Log("有人死了");
            
        }
    
    }

    /// <summary>
    /// 找是否死光
    /// </summary>
    /// <param name="left"></param>
    PeopleData CheckAlivePeople(bool left)
    {
        PeopleData aliveP = null;
        if (left)
        {
            for(int i = 0; i < p1List.Count; i++)
            {
                PeopleData p = p1List[i];
                int hp = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p).num;
                if (hp > 0)
                {
                    aliveP=p;
                    deadQieRenLeftIndex = i;
                    break;
                }
                 
            }

        }
        else
        {
            for (int i = 0; i < p2List.Count; i++)
            {
                PeopleData p = p2List[i];
                int hp = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p).num;
                if (hp > 0)
                {
                    aliveP = p;
                    deadQieRenRightIndex = i;

                    break;
                }

            }
        }
        return aliveP;
    }

    /// <summary>
    /// 切换到下个阶段
    /// </summary>
    public void ChangeToNextPhase()
    {
        int mpNum = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, changePhaseP).num;

        changePhaseP.curPhase++;

        changePhaseP.propertyIdList.Clear();
        changePhaseP.propertyList.Clear();
        changePhaseP.curBattleProIdList.Clear();
        changePhaseP.curBattleProList.Clear();

        EnemySetting enemySetting = DataTable.FindEnemySetting(changePhaseP.enemySettingId);

        List<List<int>> baseBattleProList = CommonUtil.SplitCfg(ConstantVal.baseBattleProperty);
        UpgradeModeSetting upgradeModeSetting = null;
        if(changePhaseP.trainIndex>0)
        upgradeModeSetting = DataTable._upgradeModeList[changePhaseP.trainIndex - 1];

        string phasePro = "";
        string phaseSkill = "";

        if (changePhaseP.curPhase == 2)
        {
            phasePro = enemySetting.Phase2Pro;
            phaseSkill = enemySetting.Phase2Skill;
        }
        else if (changePhaseP.curPhase == 3)
        {
            phasePro = enemySetting.Phase3Pro;
            phaseSkill = enemySetting.Phase3Skill;
        }
        List<List<int>> proList = CommonUtil.SplitCfg(phasePro);
        List<int> haveValIdList = new List<int>();
        List<int> haveValValList = new List<int>();
        for (int i = 0; i < proList.Count; i++)
        {
            List<int> thePro = proList[i];
            haveValIdList.Add(thePro[0]);
            haveValValList.Add(thePro[1]);
        }
        for (int i = 0; i < baseBattleProList.Count; i++)
        {
            List<int> singlePro = baseBattleProList[i];
            int id = singlePro[0];
            int num = singlePro[1];
          
            if (haveValIdList.Contains(id))
            {
                int index = haveValIdList.IndexOf(id);
                num = haveValValList[index];
            }
            //新改的敌人和lv有关了
            if (changePhaseP.trainIndex > 0  )
            {
                float newGuideParam = 1;
                if (enemySetting.Type.ToInt32() == (int)EnemyType.FuBenMonster)
                {
                    newGuideParam = upgradeModeSetting.NewGuideParam.ToFloat();
                }
                if (id == (int)PropertyIdType.Hp)
                {
                    num = (int)MathF.Round(num * upgradeModeSetting.Param1.ToFloat() * newGuideParam);
                }
                else if (id == (int)PropertyIdType.Defense)
                {
                    num = (int)MathF.Round(num * upgradeModeSetting.Param2.ToFloat() * newGuideParam);
                }
                else if (id == (int)PropertyIdType.Attack)
                {
                    num = (int)MathF.Round(num * upgradeModeSetting.Param3.ToFloat() * newGuideParam);
                }
            }
            SinglePropertyData singlePropertyData = new SinglePropertyData();
            singlePropertyData.id = id;
            singlePropertyData.num = num;
            singlePropertyData.quality = 1;

            if (id == (int)PropertyIdType.MpNum)
            {
                singlePropertyData.num = mpNum;
                singlePropertyData.limit = 100;
            }
            else if (id == (int)PropertyIdType.Hp)
            {
                singlePropertyData.limit = singlePropertyData.num;
            }


            changePhaseP.propertyList.Add(singlePropertyData);
            changePhaseP.propertyIdList.Add(singlePropertyData.id);

            SinglePropertyData battle_singlePropertyData = new SinglePropertyData();
            battle_singlePropertyData.id = id;
            battle_singlePropertyData.num = singlePropertyData.num;
            battle_singlePropertyData.limit = singlePropertyData.limit;
            battle_singlePropertyData.quality = 1;

            changePhaseP.curBattleProIdList.Add(id);
            changePhaseP.curBattleProList.Add(battle_singlePropertyData);
            //PeopleData.ProQualityList.Add(proQuality);
        }

        //初始技能暂定灵弹
        //改变形态敌人技能
        changePhaseP.allSkillData = new AllSkillData();

        if (!string.IsNullOrWhiteSpace(phaseSkill))
        {
            List<List<int>> theSkill = CommonUtil.SplitCfg(phaseSkill);
            for (int i = 0; i < theSkill.Count; i++)
            {
                List<int> settingSkill = theSkill[i];
                SingleSkillData singleSkill = new SingleSkillData();
                singleSkill.skillId = settingSkill[0];
                int skillLv = settingSkill[1];
                if (changePhaseP.trainIndex > 0)
                {
                    skillLv = Mathf.RoundToInt((changePhaseP.trainIndex+1) * 50 / (float)150);
                    if (skillLv <= 1)
                        skillLv = 1;
                    if (skillLv >= 50)
                        skillLv = 50;
                }
                singleSkill.skillLevel = skillLv;
                changePhaseP.allSkillData.skillList.Add(singleSkill);
                changePhaseP.allSkillData.equippedSkillIdList.Add(singleSkill.skillId);
            }
        }
        else
        {
            int skillLv = 1;
            if (changePhaseP.trainIndex > 0)
            {
                skillLv = Mathf.RoundToInt((changePhaseP.trainIndex + 1) * 25 / (float)150);
                if (skillLv <= 1)
                    skillLv = 1;
                if (skillLv >= 25)
                    skillLv = 25;
            }
            SingleSkillData singleSkill = new SingleSkillData();
            singleSkill.skillId = (int)PuGongIdByYuanSu((YuanSuType)changePhaseP.yuanSu);// (int)SkillIdType.LingDan;
            singleSkill.skillLevel = skillLv;
            changePhaseP.allSkillData.skillList.Add(singleSkill);
            changePhaseP.allSkillData.equippedSkillIdList.Add(singleSkill.skillId);
        }

      

        HuanCunSkillSetting(p1List, p2List);
        someOndeDead = false;
        ClearLogicPause();
        EventCenter.Broadcast(TheEventType.BattlePeopleSuccessfulChangePhase);
    }

    /// <summary>
    /// 死了以后切人，要把暂停的游戏启动
    /// </summary>
    public void DeadQieRen()
    {
        if (!CheckIfSomeTeamAllDead())
        {
            if (deadQieRenLeftIndex != -1 ||
                      deadQieRenRightIndex != -1)
            {
                if (deadQieRenLeftIndex != -1)
                {
                    someOndeDead = false;
                    ClearLogicPause();
                    QieRen(true, deadQieRenLeftIndex,true);
                }
                if (deadQieRenRightIndex != -1)
                {
                    someOndeDead = false;
                    ClearLogicPause();
                    QieRen(false, deadQieRenRightIndex,true);
                }
            }

        }

        else
        {
            OnSingleBattleEnd(p1List[p1Index], p2List[p2Index]);

        }
         
    }

    /// <summary>
    /// 是否有队全死
    /// </summary>
    /// <returns></returns>
    public bool CheckIfSomeTeamAllDead()
    {
        bool leftAllDead = true;
        bool rightAllDead = true;
        for(int i = 0; i < p1List.Count; i++)
        {
            PeopleData p = p1List[i];
            if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p).num > 0)
            {
                leftAllDead = false;
                break;
            }
        }
        for (int i = 0; i < p2List.Count; i++)
        {
            PeopleData p = p2List[i];
            if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p).num > 0)
            {
                rightAllDead = false;
                break;
            }
        }
        if (leftAllDead || rightAllDead)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 是否左队全死
    /// </summary>
    /// <returns></returns>
    public bool CheckIfLeftTeamAllDead()
    {
        bool rightAllDead = true;
        for (int i = 0; i < p2List.Count; i++)
        {
            PeopleData p = p2List[i];
            if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p).num > 0)
            {
                rightAllDead = false;
                break;
            }
        }
        return rightAllDead;
    }
    /// <summary>
    /// 是否右队全死
    /// </summary>
    /// <returns></returns>
    public bool CheckIfRightTeamAllDead()
    {
        bool leftAllDead = true;
        for (int i = 0; i < p1List.Count; i++)
        {
            PeopleData p = p1List[i];
            if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p).num > 0)
            {
                leftAllDead = false;
                break;
            }
        }
        return leftAllDead;
    }
    /// <summary>
    /// 逃跑
    /// </summary>
    public void Escape(ulong onlyId)
    {
        if (onlyId == p1List[p1Index].onlyId)
        {
            curEscapeP = p1List[p1Index];
        }
        else
        {
            curEscapeP = p2List[p2Index];
        }

        EventCenter.Broadcast(TheEventType.StartEscape, onlyId);


    }
    /// <summary>
    /// 播放逃跑动画 此时暂停
    /// </summary>
    public void PlayEscapeAnim(ulong onlyId)
    {
        if (!logicPause)
        {
            AddLogicPause();
            EventCenter.Broadcast(TheEventType.PlayEscapeAnim, onlyId);

        }
    }
    /// <summary>
    /// 逃跑成功
    /// </summary>
    public void SuccessEscape()
    {
        ulong winnerId = 0;
        if (curEscapeP.onlyId == p1List[p1Index].onlyId)
            winnerId = p2List[p2Index].onlyId;
        else
            winnerId = p1List[p1Index].onlyId;


        OnSingleBattleEnd(p1List[p1Index], p2List[p2Index], winnerId);
    }

  
    /// <summary>
    /// 切人下个人
    /// </summary>
    public void QieRen(bool left, int index,bool deadQie) 
    {
        int before = p1Index;
        PeopleData p = null;
        PeopleData p2 = null;
        if (left)
        {
            p = p1List[p1Index];
            p2 = p2List[p2Index];
       
        }
        else
        {
            p = p2List[p2Index];
            p2 = p1List[p1Index];


        }

        //执行火印等切人会强制触发的状态
        if (!deadQie)
        {
            //火印
            List<BattleBuff> huoYinList = FindIDBuffList(p2, BattleBuffIdType.HuoYin);
            if (huoYinList.Count > 0)
            {
                //AttackResData huoYinRes = CalcAttack(p, p2, null, 0, huoYinList[0]);
                //EventCenter.Broadcast(TheEventType.ShowYuanSuReactionNum, huoYinRes);
                //EventCenter.Broadcast(TheEventType.BattleBeHit, huoYinRes);

                //if (huoYinRes.fanShangResData != null)
                //{
                //    EventCenter.Broadcast(TheEventType.BattleDeHpShow, huoYinRes.fanShangResData);
                //    EventCenter.Broadcast(TheEventType.BattleBeHit, huoYinRes.fanShangResData);
                //}
                //BattleManager.Instance.JudgeIfPeopleDead();

                ExecuteHuoYin(p, p2);
            }
            //飞云套切人会触发加攻击buff
            List<EquipTaoZhuangType> taoZhuangList = EquipmentManager.Instance.CheckEquipTaoZhuang(p);
            if (taoZhuangList.Count >= 2
                    && taoZhuangList[0] == taoZhuangList[1])
            {
                //飞云套
                if (taoZhuangList[0] == EquipTaoZhuangType.FeiYun)
                {
                    if (p.feiYunCD <= 0)
                    {
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)taoZhuangList[0]);
                        // 使用 SplitCfgStringOneDepth 保留原始字符串格式
                        List<string> buffParamStrList = CommonUtil.SplitCfgStringOneDepth(taoZhuangSetting.Param2);
                        if (buffParamStrList.Count > 0 && !string.IsNullOrEmpty(buffParamStrList[0]))
                            AddBattleBuff(p, DataTable.FindBattleBuffSetting(buffParamStrList[0]));
                        if (buffParamStrList.Count > 1 && !string.IsNullOrEmpty(buffParamStrList[1]))
                            AddBattleBuff(p, DataTable.FindBattleBuffSetting(buffParamStrList[1]));
                         
                        if (buffParamStrList.Count > 2)
                            p.feiYunCD = buffParamStrList[2].ToInt32();

                    }
                } 
            }
        }
        //死了 移除所有火魂火印
        else
        {
            //火印
            List<BattleBuff> huoYinList = FindIDBuffList(p2, BattleBuffIdType.HuoYin);
            if (huoYinList.Count > 0)
            {
                for (int i = huoYinList.Count - 1; i >= 0; i--)
                {
                    RemoveBuff(p2, huoYinList[i]);
                }
            }
            BattleBuff huoHunBuff = FindIdBuff(p, BattleBuffIdType.HuoHun);
            if (huoHunBuff != null)
            {
                RemoveBuff(p, huoHunBuff);
            }
        }

        if (left)
        {
            p1Index = index;

        }
        else
        {
            p2Index = index;

        }
        Debug.Log(before + "切成" + index);
        EventCenter.Broadcast(TheEventType.BattlePeopleQieRen);// p1List[index]
    }


    /// <summary>
    /// 执行火印
    /// </summary>
    public void ExecuteHuoYin(PeopleData attackP, PeopleData beHitP)
    {
        //火印
        List<BattleBuff> huoYinList = FindIDBuffList(beHitP, BattleBuffIdType.HuoYin);
        if (huoYinList.Count > 0)
        {
            //彼岸炽魂伤害

            AttackResData huoYinRes = CalcAttack(attackP, beHitP, null, 0, huoYinList[0]);
            if (huoYinRes != null)
            {
                EventCenter.Broadcast(TheEventType.ShowYuanSuReactionNum, huoYinRes);
                EventCenter.Broadcast(TheEventType.BattleBeHit, huoYinRes);
                EventCenter.Broadcast(TheEventType.ShowBattleEffect, beHitP, huoYinList[0].buffSetting.Param2);
                if (huoYinRes.fanShangResData != null)
                {
                    EventCenter.Broadcast(TheEventType.BattleDeHpShow, huoYinRes.fanShangResData);
                    EventCenter.Broadcast(TheEventType.BattleBeHit, huoYinRes.fanShangResData);
                }

                BattleManager.Instance.JudgeIfPeopleDead();

            }

            //移除所有火印
            RemoveAllIDBuff(beHitP,BattleBuffIdType.HuoYin);
            //彼岸炽魂回血
            int totalHPNum = (int)RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, attackP).limit;
            int hpNum = (int)(0.05 * (huoYinList.Count * totalHPNum));
            AddHP(attackP, hpNum);
        }
        RemoveAllTypeBuff(attackP, BattleBuffType.HuoHun);
    }





    /// <summary>
    /// 战斗结束
    /// </summary>
    public void OnSingleBattleEnd(PeopleData p1, PeopleData p2,ulong winnerOnlyId=0)
    {
        winner = null;

        if (winnerOnlyId == 0)
        {
            if (CheckIfLeftTeamAllDead())
            {
                winner = p1;
            }
            else if(CheckIfRightTeamAllDead())
            {
                winner = p2;
            }
             
        }
        else
        {
            if (winnerOnlyId == p1.onlyId)
                winner = p1;
            else
                winner = p2;
        }


        if (!BattleManager.Instance.test)
        {
            switch (curBattleType)
            {
                case BattleType.Match:
                    //如果是玩家则给币
                    for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1.Count; i++)
                    {
                        //玩家队伍赢了
                        if(winner.onlyId== RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1[i])
                        {
                            roleRank++;
                            if (curPhase==1)
                                roleAward[0].count = (ulong)ConstantVal.winMatchRongYuBi;
                           else
                                roleAward[0].count += (ulong)ConstantVal.winMatchRongYuBi;

                            break;
                        }
                    }
                    //回满血
                    FullAllBattlePeopleHP();

                    switch (curPhase)
                    {
                        //第一场肯定有玩家
                        case 1:
                            List<int> leftWinnerIndexList = new List<int>();
                            SingleOtherZongMenData winnerZong = null;
                            if (MatchManager.Instance.CheckIfZongMenP(winner.onlyId, leftMatch[0][0]))
                            {
                                winnerZong = leftMatch[0][0];
                                leftWinnerIndexList.Add(1);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(true, curPhase, 1));
                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(leftMatch[0][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(leftMatch[0][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, true);
                            }
                            else
                            {
                                winnerZong = leftMatch[0][1];

                                leftWinnerIndexList.Add(2);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(true, curPhase, 2));
                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(leftMatch[0][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(leftMatch[0][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, false);
                            }


                            //模拟其他场次的对战结果
                            PeopleData winner2 = MimicBattle(leftMatch[1][0].pList[0], leftMatch[1][1].pList[0]);
                            SingleOtherZongMenData winnerZong2 = null;

                            if (winner2.onlyId == leftMatch[1][0].pList[0].onlyId)
                            {
                                winnerZong2 = leftMatch[1][0];
                                leftWinnerIndexList.Add(3);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(true, curPhase, 3));

                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(leftMatch[1][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(leftMatch[1][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, true);

                            }
                            else
                            {
                                winnerZong2 = leftMatch[1][1];

                                leftWinnerIndexList.Add(4);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(true, curPhase, 4));

                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(leftMatch[1][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(leftMatch[1][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, false);

                            }
                            
                            leftMatch.Clear();
                            List<SingleOtherZongMenData> leftMatch2 = new List<SingleOtherZongMenData>() { winnerZong, winnerZong2 };
                            leftMatch.Add(leftMatch2);
                            winnerRangeIndexLeftList.Add(leftWinnerIndexList);

                            List<int> rightWinnerIndexList = new List<int>();

                            PeopleData rightWinner1 = MimicBattle(rightMatch[0][0].pList[0], rightMatch[0][1].pList[0]);
                            SingleOtherZongMenData rightWinnerZong1 = null;
                            if (rightWinner1.onlyId == rightMatch[0][0].pList[0].onlyId)
                            {
                                rightWinnerZong1 = rightMatch[0][0];
                                rightWinnerIndexList.Add(1);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(false, curPhase, 1));

                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(rightMatch[0][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(rightMatch[0][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, true);

                            }
                            else
                            {
                                rightWinnerZong1 = rightMatch[0][1];
                                rightWinnerIndexList.Add(2);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(false, curPhase, 2));
                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(rightMatch[0][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(rightMatch[0][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, false);
                            }

                            PeopleData rightWinner2 = MimicBattle(rightMatch[1][0].pList[0], rightMatch[1][1].pList[0]);
                            SingleOtherZongMenData rightWinnerZong2 = null;
                            if (rightWinner2.onlyId == rightMatch[1][0].pList[0].onlyId)
                            {
                                rightWinnerZong2 = rightMatch[1][0];
                                rightWinnerIndexList.Add(3);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(false, curPhase, 3));
                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(rightMatch[1][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(rightMatch[1][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, true);

                            }
                            else
                            {
                                rightWinnerZong2 = rightMatch[1][1];
                                rightWinnerIndexList.Add(4);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(false, curPhase, 4));

                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(rightMatch[1][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(rightMatch[1][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, false);

                            }

                            rightMatch.Clear();
                            List<SingleOtherZongMenData> rightMatch2 = new List<SingleOtherZongMenData>() { rightWinnerZong1, rightWinnerZong2 };
                            rightMatch.Add(rightMatch2);

                            winnerRangeIndexRightList.Add(rightWinnerIndexList);

                            break;
                        //第二轮
                        case 2:
                            List<int> leftWinnerIndexList2 = new List<int>();
                            SingleOtherZongMenData winnerZong_2 = null;
                            if (MatchManager.Instance.CheckIfZongMenP(winner.onlyId, leftMatch[0][0]))
                            {
                                winnerZong_2 = leftMatch[0][0];
                                leftWinnerIndexList2.Add(1);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(true, curPhase, 1));
                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(leftMatch[0][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(rightMatch[0][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, true);
                            }
                            else
                            {
                                winnerZong_2 = leftMatch[0][1];
                                leftWinnerIndexList2.Add(2);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(true, curPhase, 2));
                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(leftMatch[0][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(rightMatch[0][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, false);

                            }
                            leftMatch.Clear();
                            leftMatch.Add(new List<SingleOtherZongMenData>() { winnerZong_2 });
                            winnerRangeIndexLeftList.Add(leftWinnerIndexList2);
                            SingleOtherZongMenData phase2RightWinnerZong = null;

                            PeopleData phase2RightWinner = MimicBattle(rightMatch[0][0].pList[0], rightMatch[0][1].pList[0]);
                            List<int> rightWinnerIndexList2 = new List<int>();

                            if (winner.onlyId == rightMatch[0][0].pList[0].onlyId)
                            {
                                phase2RightWinnerZong = rightMatch[0][0];
                                rightWinnerIndexList2.Add(1);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(false, curPhase, 1));
                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(rightMatch[0][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(rightMatch[0][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, true);

                            }
                            else
                            {
                                phase2RightWinnerZong = rightMatch[0][1];
                                rightWinnerIndexList2.Add(2);
                                nextPrepareAnimDataList.Add(new PrepareAnimData(false, curPhase, 2));

                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(rightMatch[0][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(rightMatch[0][1]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, false);

                            }
                            rightMatch.Clear();
                            rightMatch.Add(new List<SingleOtherZongMenData>() { phase2RightWinnerZong });
                            winnerRangeIndexRightList.Add(rightWinnerIndexList2);
                            break;
                        //第三轮（总决赛胜利
                        case 3:
                            SingleOtherZongMenData finalWinner = null;
                            if (MatchManager.Instance.CheckIfZongMenP(winner.onlyId, leftMatch[0][0]))
                            {
                                finalWinner = leftMatch[0][0];
                                nextPrepareAnimDataList.Add(new PrepareAnimData(true, curPhase, 1));
                                winnerRangeIndexLeftList.Add(new List<int> { 1 });

                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(leftMatch[0][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(rightMatch[0][0]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, true);

                            }
                            else
                            {
                                finalWinner = rightMatch[0][0];

                                nextPrepareAnimDataList.Add(new PrepareAnimData(false, curPhase, 1));
                                winnerRangeIndexRightList.Add(new List<int> { 1 });

                                //先保存数据 再改变分数
                                SingleOtherZongMenData zong1 = FindOtherZongmenAndSave(leftMatch[0][0]);
                                SingleOtherZongMenData zong2 = FindOtherZongmenAndSave(rightMatch[0][0]);
                                MatchManager.Instance.MatchScoreChange(zong1, zong2, false);

                            }
                            thisMatchFinalWinner = finalWinner;
                            break;
                    }
                  
                    break;
                    //找茬
                case BattleType.ZhaoChaBattle:
              
                        break;
                case BattleType.LevelBattle:
                    bool win = false;
                    if(winner.isMyTeam)
                    {
                        win = true;
                    }
                    else
                    {
                        win = false;
                    }
                    LevelBattleDataResult(win);

                    break;
                case BattleType.OutsideBattle:
                    bool outBattleWin = false;
                    if (winner.isMyTeam)
                    {
                        outBattleWin = true;
                    }
                    else
                    {
                        outBattleWin = false;
                    }
                    OutsideBattleDataResult(outBattleWin);

                    break;
                case BattleType.QieCuoBattle:
                    bool qieCuoBattleWin = false;
                    if (winner.isMyTeam)
                    {
                        qieCuoBattleWin = true;
                    }
                    else
                    {
                        qieCuoBattleWin = false;
                    }
                    QieCuoBattleDataResult(qieCuoBattleWin);

                    break;
                case BattleType.MiJingGuardBattle:
                    bool miJingGuardBattleWin = false;
                    if (winner.isMyTeam)
                    {
                        miJingGuardBattleWin = true;
                    }
                    else
                    {
                        miJingGuardBattleWin = false;
                    }
                    MiJingGuardBattleDataResult(miJingGuardBattleWin);

                    break;
                case BattleType.NPCKillBattle:
                    bool NPCKillBattleWin = false;
                    if (winner.isMyTeam)
                    {
                        NPCKillBattleWin = true;
                    }
                    else
                    {
                        NPCKillBattleWin = false;
                    }
                    NPCKillBattleDataResult(NPCKillBattleWin);

                    break;
                    //第一次和山海宗长老打
                case BattleType.TouZiFirstBattle:
                    bool TouZiFirstBattleWin = false;
                    if (winner.isMyTeam)
                    {
                        TouZiFirstBattleWin = true;
                    }
                    else
                    {
                        TouZiFirstBattleWin = false;
                    }
                    TouziFirstBattleDataResult(TouZiFirstBattleWin);

                    break;
                //第一次和帝姝打
                case BattleType.DiShuFirstBattle:
        
                    DiShuFirstBattleDataResult(false);

                    break;
                //第一次和云海宗掌门打
                case BattleType.LiMaoZhangMenBattle:
                    bool LiMaoZhangMenBattleWin = false;
                    if (winner.isMyTeam)
                    {
                        LiMaoZhangMenBattleWin = true;
                    }
                    else
                    {
                        LiMaoZhangMenBattleWin = false;
                    }
                    LiMaoZhangMenBattleDataResult(LiMaoZhangMenBattleWin);

                    break;
                case BattleType.FixedLevelBattle:
                    bool fixedLevelBattlewin = false;
                    if (winner.isMyTeam)
                    {
                        fixedLevelBattlewin = true;
                    }
                    else
                    {
                        fixedLevelBattlewin = false;
                    }
                    FixedLevelBattleDataResult(fixedLevelBattlewin);

                    break;

                case BattleType.MapEventBattle:
                    bool MapEventBattleWin = false;
                    if (winner.isMyTeam)
                    {
                        MapEventBattleWin = true;
                    }
                    else
                    {
                        MapEventBattleWin = false;
                    }
                    MapEventBattleDataResult(MapEventBattleWin);

                    break;

                case BattleType.PanZongStudentBattle:
                    bool PanZongStudentBattleWin = false;
                    if (winner.isMyTeam)
                    {
                        PanZongStudentBattleWin = true;
                    }
                    else
                    {
                        PanZongStudentBattleWin = false;
                    }
                    PanZongBattleDataResult(PanZongStudentBattleWin);

                    break;
            }
      
        }
        EventCenter.Broadcast(TheEventType.BattleEnd, winner, true);


    }
    /// <summary>
    /// 狸猫掌门战斗结算数据层面
    /// </summary>
    public void LiMaoZhangMenBattleDataResult(bool win)
    {
        if (win)
        {
            PanelManager.Instance.curYieldShowInMainPanelType = YieldShowInMainPanelType.WinLiMaoZhangMen;

        }
        else
        {
            PanelManager.Instance.curYieldShowInMainPanelType = YieldShowInMainPanelType.LoseLiMaoZhangMen;

        }
        //回满血
        FullAllBattlePeopleHP();
        //RoleManager.Instance.FullHp(p1List[p1Index]);
        //RoleManager.Instance.FullHp(p2List[p2Index]);
    }
    /// <summary>
    /// 帝姝第一次战斗结算数据层面
    /// </summary>
    public void DiShuFirstBattleDataResult(bool win)
    {
        PanelManager.Instance.curYieldShowInMainPanelType = YieldShowInMainPanelType.DiShuFirstFarewell;
        //回满血
        //RoleManager.Instance.FullHp(p1List[p1Index]);
        //RoleManager.Instance.FullHp(p2List[p2Index]);
        FullAllBattlePeopleHP();

    }
    /// <summary>
    /// 头子第一次战斗结算数据层面
    /// </summary>
    public void TouziFirstBattleDataResult(bool win)
    {
        if (win)
        {

        }
        else
        {
            //roleAward = null;
        }

        //回满血
        FullAllBattlePeopleHP();
    }

    /// <summary>
    /// 叛宗战斗结算数据层面
    /// </summary>
    public void PanZongBattleDataResult(bool win)
    {
        if (win)
        {
        }
        else
        {
            for(int i = 0; i < p2List.Count; i++)
            {
                RoleManager.Instance.AddNotedPeople(p2List[i], NotedPeopleType.PanZong);

            }

            //失去灵石
            int lingShiDeRate = (int)(RandomManager.Next(15, 30) * 0.01f * ItemManager.Instance.FindItemCount((int)ItemIdType.LingShi));
            ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, (ulong)lingShiDeRate);
        }
        roleAward = null;
        //回满血
        FullAllBattlePeopleHP();
    }
    /// <summary>
    /// 地图事件战斗结算数据层面
    /// </summary>
    public void MapEventBattleDataResult(bool win)
    {
        int tiliConsume = ConstantVal.mapEventBattleTiliConsume;
        if (win)
        {
            roleAward = new List<ItemData>();
            List<List<int>> award = CommonUtil.SplitCfg(DataTable.FindMapEventSetting(curBattleMapEvent.SettingId).Param);

            roleAward = new List<ItemData>();
            for (int i = 0; i < award.Count; i++)
            {
                List<int> singleAward = award[i];
                ItemData itemData = new ItemData();
                itemData.settingId = singleAward[0];
                itemData.count =(ulong)RandomManager.Next(singleAward[1], singleAward[2]);
                if (itemData.count > 0)
                {
                    roleAward.Add(itemData);
                    //给东西
                    ItemManager.Instance.GetItem(itemData.settingId, itemData.count);
                }
            
            }
            MapEventManager.Instance.ExecuteMapEvent(curBattleMapEvent);
            

        }
        else
        {
            roleAward = null;
            tiliConsume = Mathf.CeilToInt(0.01f * tiliConsume);
        }
        RoleManager.Instance.DeProperty(PropertyIdType.Tili, -tiliConsume);

        //回满血
        FullAllBattlePeopleHP();
    }
    /// <summary>
    /// npc生死战斗结算数据层面
    /// </summary>
    public void NPCKillBattleDataResult(bool win)
    {
        if (win)
        {
            //EnemySetting enemySetting = DataTable.FindEnemySetting(curBattleP2.EnemySettingId);
            //List<int> award = CommonUtil.SplitCfgOneDepth(enemySetting.award);
            //ItemData itemData = new ItemData();
            //itemData.SettingId = award[0];
            //itemData.Count = (ulong)award[1];
            //roleAward = itemData;
            ////给东西
            //ItemManager.Instance.GetItem(roleAward.SettingId, roleAward.Count);
            SingleNPCData singleNPCData = TaskManager.Instance.FindNPCByOnlyId(p2List[p2Index].onlyId);
            NPC npcSetting = DataTable.FindNPCArrById(singleNPCData.Id);
            int enemyId=(int)npcSetting.enemyId;
            TaskManager.Instance.RemoveNPC(singleNPCData.OnlyId);
            //EventCenter.Broadcast(TheEventType.WinOutsideBattle, curBattleP2.EnemySettingId);
            //完成taskmanager的任务
            //TaskManager.Instance.GetAchievement(AchievementType.QieCuo, singleNPCData.PeopleData.OnlyId.ToString());
            TaskManager.Instance.GetAchievement(AchievementType.KillEnmey, enemyId.ToString());
        }
        else
        {
            roleAward = null;
        }

        //回满血
        FullAllBattlePeopleHP();
    }
    /// <summary>
    /// 切磋战斗结算数据层面
    /// </summary>
    public void QieCuoBattleDataResult(bool win)
    {
        if (win)
        {
            //EnemySetting enemySetting = DataTable.FindEnemySetting(curBattleP2.EnemySettingId);
            //List<int> award = CommonUtil.SplitCfgOneDepth(enemySetting.award);
            //ItemData itemData = new ItemData();
            //itemData.SettingId = award[0];
            //itemData.Count = (ulong)award[1];
            //roleAward = itemData;
            ////给东西
            //ItemManager.Instance.GetItem(roleAward.SettingId, roleAward.Count);
            SingleNPCData singleNPCData = TaskManager.Instance.FindNPCByOnlyId(p2List[p2Index].onlyId);
            //EventCenter.Broadcast(TheEventType.WinOutsideBattle, curBattleP2.EnemySettingId);
            //完成taskmanager的任务
            TaskManager.Instance.GetAchievement(AchievementType.QieCuo, singleNPCData.PeopleData.onlyId.ToString());
        }
        else
        {
            roleAward = null;
        }


    }
    /// <summary>
    /// 秘境小怪战斗结算数据层面
    /// </summary>
    public void MiJingGuardBattleDataResult(bool win)
    {
        MiJingLevelSetting miJingLevelSetting = DataTable.FindMiJingLevelSetting(MiJingManager.Instance.curBattleMijingLevelId);

        if (win)
        {

            string[] enemyIdArr = null;
            SingleMiJingLevelData level = MiJingManager.Instance.FindMiJingLevelDataById(MiJingManager.Instance.curBattleMijingLevelId);
            SingleMiJingPaiQianData singleMiJingPaiQianData = MiJingManager.Instance.GetSingleMiJingPaiQianDataById(level.MiJingId);
            List<List<int>> award = new List<List<int>>();
            string bookIdStr = "";
            string bookNumStr = "";
            string weekMatIdStr = "";
            string weekMatNumStr = "";

            if (singleMiJingPaiQianData.WeekType == (int)CurWeekType.Week135)
            {
                bookIdStr = miJingLevelSetting.Week135possibleBook;
                bookNumStr = miJingLevelSetting.Week135bookNum;
                weekMatIdStr = miJingLevelSetting.Week135Mat;
                weekMatNumStr = miJingLevelSetting.Week135MatNum;

                enemyIdArr = miJingLevelSetting.Week135enemy.Split('|');
                //award = CommonUtil.SplitCfg(miJingLevelSetting.week135guardAward);
            }
            else if(singleMiJingPaiQianData.WeekType == (int)CurWeekType.Week246)
            {
                bookIdStr = miJingLevelSetting.Week246possibleBook;
                bookNumStr = miJingLevelSetting.Week246bookNum;
                weekMatIdStr = miJingLevelSetting.Week246Mat;
                weekMatNumStr = miJingLevelSetting.Week246MatNum;
                enemyIdArr = miJingLevelSetting.Week246enemy.Split('|');

            }
            else
            {
                bookIdStr = miJingLevelSetting.Week7possibleBook;
                bookNumStr = miJingLevelSetting.Week7bookNum;
                weekMatIdStr = miJingLevelSetting.Week7Mat;
                weekMatNumStr = miJingLevelSetting.Week7MatNum;

                enemyIdArr = miJingLevelSetting.Week7enemy.Split('|');

            }


            int bookNum = 0;

            //先算得几本书
            if (!string.IsNullOrWhiteSpace(bookNumStr))
            {
                List<List<int>> bookParam = CommonUtil.SplitCfg(bookNumStr);
                List<int> weightList = new List<int>();
                List<int> numList = new List<int>();
                for (int i = 0; i < bookParam.Count; i++)
                {
                    List<int> singleBookParam = bookParam[i];
                    int weight = singleBookParam[0];
                    if (singleMiJingPaiQianData.DayliHighNum <= 0)
                        weight /= 10;
                    int num = singleBookParam[1];
                    weightList.Add(weight);
                    numList.Add(num);
                }
                int numindex = RandomManager.Next(0, 100);

                for (int i = weightList.Count - 1; i >= 0; i--)
                {
                    if (numindex < weightList[i])
                    {
                        bookNum = numList[i];
                        break;
                    }
                }
            }
      
            //int numindex = CommonUtil.GetIndexByWeight(weightList);


            //int bookNum = numList[numindex];
            //再分配这几本书
            Dictionary<int, int> awardDic = new Dictionary<int, int>();

            if (!string.IsNullOrWhiteSpace(bookIdStr))
            {
                List<int> bookIdList = CommonUtil.SplitCfgOneDepth(bookIdStr);
                for (int i = 0; i < bookNum; i++)
                {
                    int index = RandomManager.Next(0, bookIdList.Count);
                    int choosedId = bookIdList[index];
                    if (miJingLevelSetting.TaoFaType.ToInt32() == (int)TaoFaType.ShenQiMuDi)
                    {
                        int rarity= MiJingManager.Instance.AwardRarityByMiJingLevel(miJingLevelSetting.Level.ToInt32());
                        List<int> choosedTuZhiList= DataTable.FindTaoZhuangYuanPeiIdListByTaoZhuangType(choosedId, (Rarity)rarity);
                        int tuZhiIndex= RandomManager.Next(0, choosedTuZhiList.Count);
                        if (choosedTuZhiList.Count > 0)
                            choosedId = choosedTuZhiList[tuZhiIndex];
                    }
                     
                    if (!awardDic.ContainsKey(choosedId))
                    {
                        awardDic.Add(choosedId, 0);
                    }
                    awardDic[choosedId]++;
                }
            }
          
            List<int> matIdList = CommonUtil.SplitCfgOneDepth(weekMatIdStr);

            //洗牌
            matIdList = CommonUtil.Shuffle<int>(matIdList);
            int matNum =weekMatNumStr.ToInt32();
            if (singleMiJingPaiQianData.DayliHighNum <= 0)
            {
                matNum /= 10;
            }
            int totalMatNum = matNum;
            //再分配这几个材料
            for (int i = 0; i < matIdList.Count; i++)
            {
                int matId = matIdList[i];
                if (!awardDic.ContainsKey(matId))
                {
                    awardDic.Add(matId, 0);
                }
                if (i < matIdList.Count - 1)
                {
                    if (matNum > 0)
                    {
                        int choosedMat = RandomManager.Next((int)(totalMatNum * 0.8f/matIdList.Count), totalMatNum / matIdList.Count + 1);

                        awardDic[matId] += choosedMat;
                        matNum -= choosedMat;
                    }
                }
                else
                {
                    awardDic[matId] += matNum;
                }
            }
            foreach (KeyValuePair<int, int> kv in awardDic)
            {
                int id = kv.Key;
                int num = kv.Value;
                if(num>0)
                award.Add(new List<int>() { id, num });
            }
            //List<List<int>> award = CommonUtil.SplitCfg(miJingLevelSetting.guardAward);
            roleAward = new List<ItemData>();
            for (int i = 0; i < award.Count; i++)
            {
                List<int> singleAward = award[i];
                if (singleAward[1] == 0)
                    continue;
                ItemData itemData = new ItemData();
                itemData.settingId = singleAward[0];
                itemData.count = (ulong)singleAward[1];
                if (itemData.count > 0)
                {
                    roleAward.Add(itemData);
                    //给东西
                    ItemManager.Instance.GetItem(itemData.settingId, itemData.count);
                }
         
            }
         
            //roleAward = itemData;

            //解锁关卡
            MiJingManager.Instance.BeatedGuard();
            //EventCenter.Broadcast(TheEventType.WinOutsideBattle, curBattleP2.EnemySettingId);
            //完成taskmanager的任务
            //扫荡
            for(int i = 0; i < enemyIdArr.Length; i++)
            {
                TaskManager.Instance.GetAchievement(AchievementType.KillEnmey, enemyIdArr[i]);
            }
            
            TaskManager.Instance.GetAchievement(AchievementType.TaoFa, miJingLevelSetting.TaoFaType);
            TaskManager.Instance.GetAchievement(AchievementType.MaxTaoFa, miJingLevelSetting.TaoFaType+"|"+miJingLevelSetting.Level);
            TaskManager.Instance.GetDailyAchievement(TaskType.TaoFa, miJingLevelSetting.TaoFaType + "|" + miJingLevelSetting.Level);
            TaskManager.Instance.TryAccomplishGuideBook(TaskType.TaoFa);
            TaskManager.Instance.TryAccomplishGuideBook(TaskType.MaxTaoFa);

            TDGAMission.OnCompleted(miJingLevelSetting.Des);

            RoleManager.Instance.DeProperty(PropertyIdType.Tili, -ConstantVal.taoFaNeedTiLi);

        }
        else
        {
            RoleManager.Instance.DeProperty(PropertyIdType.Tili,-(int)(ConstantVal.taoFaNeedTiLi*0.1f));
            TDGAMission.OnFailed(miJingLevelSetting.Des,"");

            roleAward = null;
        }
        //回满血
        FullAllBattlePeopleHP();
    }

    /// <summary>
    /// 支线战斗结算数据层面
    /// </summary>
    public void OutsideBattleDataResult(bool win)
    {
        if (win)
        {
            EnemySetting enemySetting = DataTable.FindEnemySetting(p2List[p2Index].enemySettingId);
            List<List<int>> award = CommonUtil.SplitCfg(enemySetting.Award);

            roleAward = new List<ItemData>();
            for (int i = 0; i < award.Count; i++)
            {
                List<int> singleAward = award[i];
                ItemData itemData = new ItemData();
                itemData.settingId = singleAward[0];
                itemData.count = (ulong)singleAward[1];
                roleAward.Add(itemData);
                //给东西
                ItemManager.Instance.GetItem(itemData.settingId, itemData.count);
            }
            ////给东西
            //ItemManager.Instance.GetItem(roleAward.SettingId, roleAward.Count);
            //EventCenter.Broadcast(TheEventType.WinOutsideBattle, p2List[p2Index].EnemySettingId);
            //完成taskmanager的任务
            TaskManager.Instance.GetAchievement(AchievementType.KillEnmey,enemySetting.Id);
            
        }
        else
        {
            roleAward = null;
        }
        //回满血
        FullAllBattlePeopleHP();



    }
    /// <summary>
    /// 固定关卡战斗结算数据层面
    /// </summary>
    public void FixedLevelBattleDataResult(bool win)
    {
        int tiliConsume = ConstantVal.fixedlevelBattleTiliConsume;
        LevelSetting levelSetting = DataTable.FindLevelSetting(MapManager.Instance.curChoosedLevelId);

        if (win)
        {
            SingleMapData curMapData = MapManager.Instance.FindMapById(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);
            SingleLevelData level = MapManager.Instance.FindFixedLevelById(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId, MapManager.Instance.curChoosedLevelId);


            roleAward = new List<ItemData>();
            if (!string.IsNullOrWhiteSpace(levelSetting.Award))
            {
                List<List<int>> award = CommonUtil.SplitCfg(levelSetting.Award);

                for (int i = 0; i < award.Count; i++)
                {
                    List<int> singleAward = award[i];
                    ItemData itemData = new ItemData();
                    itemData.settingId = singleAward[0];
                    itemData.count = (ulong)singleAward[1];
                    if (itemData.count > 0)
                    {
                        roleAward.Add(itemData);
                        //给东西
                        ItemManager.Instance.GetItem(itemData.settingId, itemData.count);
                    }

                }

            }
        
            //ItemManager.Instance.AddANewGem((int)ItemIdType.PoSuiRedGem, null,false,Quality.White);
            //首次掉落
            if (!level.HaveAccomplished)
            {
                if (!string.IsNullOrWhiteSpace(levelSetting.FirstAward))
                {
                    List<List<int>> firstDiaoLuo = CommonUtil.SplitCfg(levelSetting.FirstAward);
                    for(int i = 0; i < firstDiaoLuo.Count; i++)
                    {
                        List<int> singleDiaoLuo = firstDiaoLuo[i];
                        ItemData firstItem = new ItemData();
                        firstItem.settingId = singleDiaoLuo[0];
                        firstItem.count = (ulong)singleDiaoLuo[1];
                        if (firstItem.count <= 0)
                            continue;

                        roleAward.Add(firstItem);

                        //curMapData.CurAwardList.Add(firstItem);
                    }
                  

                }
            }
            MapManager.Instance.AccomplishFixedLevel(MapManager.Instance.curChoosedLevelId);
            //记录杀敌成就，用于完成 KillEnemy 类型任务
            TaskManager.Instance.GetAchievement(AchievementType.KillEnmey, level.Enemy[0].enemySettingId.ToString());
            //击败山海宗掌门后，触发剧情
            if (level.Enemy[0].enemySettingId == (int)EnemyIdType.ShanHaiZongZhangMen)
            {
                PanelManager.Instance.curYieldShowInMainPanelType = YieldShowInMainPanelType.AfterKillShanHaiZongZhangMen;
            }

        }
        else
        {
            TDGAMission.OnFailed(levelSetting.Level, "");
            tiliConsume = Mathf.CeilToInt(tiliConsume * 0.01f);
            roleAward = null;
        }
        //回满血
        FullAllBattlePeopleHP();
        RoleManager.Instance.DeProperty(PropertyIdType.Tili, -tiliConsume);
    }
    /// <summary>
    /// 战斗结算数据层面 如果输了直接回去
    /// </summary>
    public void LevelBattleDataResult(bool win,bool saoDang=false)
    {

        LevelSetting levelSetting = DataTable.FindLevelSetting(MapManager.Instance.curChoosedLevelId);
        SingleLevelData level = MapManager.Instance.FindLevelById(MapManager.Instance.curChoosedLevelId);
        int tiliConsume = ConstantVal.levelBattleTiliConsume;
        if (win)
        {
            SingleMapData curMapData = MapManager.Instance.FindMapById(RoleManager.Instance._CurGameInfo.AllMapData.CurChoosedMapId);

            List<List<int>> award = CommonUtil.SplitCfg(levelSetting.Award);

            roleAward = new List<ItemData>();
            for (int i = 0; i < award.Count; i++)
            {
                List<int> singleAward = award[i];
                ItemData itemData = new ItemData();
                itemData.settingId = singleAward[0];
                itemData.count = (ulong)singleAward[1];
                if (level.HaveAccomplished)
                    itemData.count =(ulong)(itemData.count* 0.4f);
                if (itemData.count>0)
                {
                    roleAward.Add(itemData);
                    bool haveSame = false;
                   for(int j = 0; j < curMapData.CurAwardList.Count; j++)
                    {
                        ItemData exist = curMapData.CurAwardList[j];
                        if (exist.settingId == itemData.settingId)
                        {
                            exist.count += itemData.count;
                            haveSame = true;
                            break;
                        }
                    }
                    if (!haveSame)
                    {
                        curMapData.CurAwardList.Add(itemData);

                    }
                }
  
                //给东西
                //ItemManager.Instance.GetItem(itemData.SettingId, itemData.Count);
            }
            //TaskManager.Instance.GetAchievement(AchievementType.KillEnmey, curBattleP2.EnemySettingId.ToString());

            //首次掉落
            if (!level.HaveAccomplished)
            {
                if (!string.IsNullOrWhiteSpace(levelSetting.FirstAward))
                {
                    List<List<int>> firstDiaoLuo = CommonUtil.SplitCfg(levelSetting.FirstAward);
                    for (int i = 0; i < firstDiaoLuo.Count; i++)
                    {
                        List<int> singleDiaoLuo = firstDiaoLuo[i];
                        ItemData firstItem = new ItemData();
                        firstItem.settingId = singleDiaoLuo[0];
                        firstItem.count = (ulong)singleDiaoLuo[1];
                        if (firstItem.count <= 0)
                            continue;

                        roleAward.Add(firstItem);

                        curMapData.CurAwardList.Add(firstItem);
                    }



                }
            }

            MapManager.Instance.AccomplishLevel(MapManager.Instance.curChoosedLevelId);
        }
        else
        {
            TDGAMission.OnFailed(levelSetting.Level,"");
            tiliConsume = Mathf.CeilToInt(tiliConsume * 0.01f);
            roleAward = null;
        }
        if (!saoDang)
        {
            //敌人数据保存
            for (int i = 0; i < level.Enemy.Count; i++)
            {
                PeopleData p = level.Enemy[i];

                PeopleData pNeiCun = p2List[i];
                p = pNeiCun;
            }
        }
    
        RoleManager.Instance.DeProperty(PropertyIdType.Tili, -tiliConsume);
    }

    /// <summary>
    /// 攻击1打2
    /// </summary>
    /// <param name="pro1"></param>
    /// <param name="pro2"></param>
    public AttackResData CalcAttack(PeopleData p1, PeopleData p2, SingleSkillData skill = null, int damageIndex = -1, BattleBuff buffAttack = null,ReactionType reactionAttackType=ReactionType.None,bool xieZhan=false)
    {
        //风影免疫

        List<BattleBuff> fengYing = FindTypeBuffList(p2, BattleBuffType.FengYing);
        if (fengYing.Count > 0)
        {
            EventCenter.Broadcast(TheEventType.BattleInfoTxtShow, p2, "免疫");
            return null;
        }


        bool trueDamage = false;
        List<EquipTaoZhuangType> p1TaoZhuangList = EquipmentManager.Instance.CheckEquipTaoZhuang(p1);
        List<EquipTaoZhuangType> p2TaoZhuangList = EquipmentManager.Instance.CheckEquipTaoZhuang(p2);

        bool crit = false;//是否暴击（ui显示要区分）
                          //攻击者buff

        SkillSetting skillSetting = null;
        //float buffAddCritHurt = 150;//暴击伤害 （暂定都是150）

        float levelPressDamageAdd = 1;
        float levelPressDamageDe = 1;
        int levelOffset = p1.trainIndex - p2.trainIndex;
        if (levelOffset > 0)
        {
            //增伤
            levelPressDamageAdd =1+ DataTable._levelPressList[levelOffset - 1].AddDamage.ToFloat()*0.01f;
        }
        else if(levelOffset < 0)
        {
            levelOffset = -levelOffset;
            //减伤
            levelPressDamageDe =1- DataTable._levelPressList[levelOffset - 1].DeDamage.ToFloat() * 0.01f;
        }

        float attack = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Attack, p1).num;

        float defence = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Defense, p2).num;

        float critRate = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.CritRate, p1).num*0.01f;
        float critNum = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.CritNum, p1).num * 0.01f;
        float mianShangNum = RoleManager.Instance.GetCurBattlePropertyNum(PropertyIdType.DeDamage, p2) * 0.01f;
        float poJia = RoleManager.Instance.GetCurBattlePropertyNum(PropertyIdType.PoJia, p2) * 0.01f;
        int jingTong= RoleManager.Instance.GetCurBattlePropertyNum(PropertyIdType.JingTong, p1);

        float skillAdd = 0;

        float jiaGongRate = 0;
        float defenceAddRate = 0;
        float yuanSuDamageAdd = 0;
        float totalResAdd =0.01f* (XueMaiManager.Instance.PerLevelAddNum(XueMaiType.ShangHai)*XueMaiManager.Instance.FindXueMaiLevel(p1,XueMaiType.ShangHai));
        //精通buff
        List<BattleBuff> jingTongBuff = FindBuffTypeBuffList(p2, BattleBuffType.JingTong);
        if (jingTongBuff.Count > 0)
        {
            for (int i = jingTongBuff.Count - 1; i >= 0; i--)
            {
                jingTong += jingTongBuff[i].commonParam.ToInt32();
            }
        }
        //敌人身上有没有遭受暴击率增加的buff
        List<BattleBuff> criteRateAddBuff = FindBuffTypeBuffList(p2, BattleBuffType.BeCritRate);
        if (criteRateAddBuff.Count>0)
        {
            for(int i = criteRateAddBuff.Count - 1; i >= 0; i--)
            {
                critRate += criteRateAddBuff[i].commonParam.ToFloat() * 0.01f;
            }
        }
        //降暴击率的buff
        List<BattleBuff> deAttackerCritRateBuff = FindBuffTypeBuffList(p1, BattleBuffType.ZhangMu);
        if (deAttackerCritRateBuff.Count>0)
        {
            for(int i= deAttackerCritRateBuff.Count - 1; i >= 0; i--)
            {
                critRate += deAttackerCritRateBuff[i].beCritRate * 0.01f;
            }
        }
        //贯注buff
        List<BattleBuff> guanZhuBuff = FindBuffTypeBuffList(p1, BattleBuffType.GuanZhu);
        if (guanZhuBuff.Count>0)
        {
            for(int i = guanZhuBuff.Count - 1; i >= 0; i--)
            {
                critRate += guanZhuBuff[i].commonParam.ToFloat() * 0.01f;
            }
        }
        //果决buff
        List<BattleBuff> guoJueBuff = FindBuffTypeBuffList(p1, BattleBuffType.GuoJue);
        if (guoJueBuff.Count>0)
        {
            for(int i = guoJueBuff.Count - 1; i >= 0; i--)
            {
                critNum += guoJueBuff[i].commonParam.ToFloat() * 0.01f;

            }
        }
        //寡断buff
        List<BattleBuff> guaDuanBuff = FindBuffTypeBuffList(p1, BattleBuffType.GuaDuan);
        if (guaDuanBuff.Count>0)
        {
            for(int i = guaDuanBuff.Count - 1; i >= 0; i--)
            {
                critNum += guaDuanBuff[i].commonParam.ToFloat() * 0.01f;
            }
        }
        //加攻buff
        List<BattleBuff> jiaGongBuff = FindBuffTypeBuffList(p1, BattleBuffType.JiaGong);
        if (jiaGongBuff.Count>0)
        {
            for (int i = jiaGongBuff.Count - 1; i >= 0; i--)
            {
                jiaGongRate += jiaGongBuff[i].jiangGongRate;
            }
        }
        //火魂加攻
        BattleBuff huoHunBuff = FindIdBuff(p1, BattleBuffIdType.HuoHun);
        if (huoHunBuff != null)
        {
            jiaGongRate += huoHunBuff.jiangGongRate;
            critRate += huoHunBuff.critRateAdd;
        }
        //降攻和技能无关等级buff
        List<BattleBuff> jiangGongNoSkillBuffList = FindTypeBuffList(p1, BattleBuffType.JiangGongNoSkillLevel);
        if (jiangGongNoSkillBuffList.Count>0)
        {
            for(int i = 0; i < jiangGongNoSkillBuffList.Count; i++)
            {
                jiaGongRate += jiangGongNoSkillBuffList[i].jiangGongRate;
            }
        }
        //神速降攻buff
        //shensuBuff
        List<BattleBuff> shenSuBuff = FindBuffTypeBuffList(p1, BattleBuffType.ShenSu);
        if (shenSuBuff.Count>0)
        {
            for(int i = shenSuBuff.Count - 1; i >= 0; i--)
            {
                jiaGongRate += shenSuBuff[i].jiangGongRate;
            }
            //res = Mathf.CeilToInt(res * 1.5f);
        }
        //加攻和技能等级无关buff
        List<BattleBuff> JiaGongNoSkillLevelBuff = FindBuffTypeBuffList(p1, BattleBuffType.JiaGongNoSkillLevel);
        if (JiaGongNoSkillLevelBuff.Count>0)
        {
            for(int i= JiaGongNoSkillLevelBuff.Count - 1; i >= 0; i--)
            {
                jiaGongRate += JiaGongNoSkillLevelBuff[i].commonParam.ToInt32();
            }
        }
        //破防buff
        List<BattleBuff> poFangBuff = FindBuffTypeBuffList(p2, BattleBuffType.PoFang);
        if (poFangBuff.Count>0)
        {
            for(int i= poFangBuff.Count - 1; i >= 0; i--)
            {
                defenceAddRate += poFangBuff[i].commonParam.ToInt32();
            }
        }
        //铁壁buff
        List<BattleBuff> tieBiBuff = FindBuffTypeBuffList(p2, BattleBuffType.TieBi);
        if (tieBiBuff.Count>0)
        {
            for(int i = tieBiBuff.Count - 1; i >= 0; i--)
            {
                defenceAddRate += tieBiBuff[i].commonParam.ToInt32();
            }
        }
        //力竭buff
        List<BattleBuff> liJieBuff = FindBuffTypeBuffList(p1, BattleBuffType.LiJie);
        if (liJieBuff.Count>0)
        {
            for(int i = liJieBuff.Count - 1; i >= 0; i--)
            {
                totalResAdd += liJieBuff[i].commonParam.ToFloat() * 0.01f;

            }
        }
        //浩然buff
        List<BattleBuff> haoRanBuff = FindBuffTypeBuffList(p1, BattleBuffType.HaoRan);
        if (haoRanBuff.Count>0)
        {
            for(int i = haoRanBuff.Count - 1; i >= 0; i--)
            {
                totalResAdd += haoRanBuff[i].commonParam.ToFloat() * 0.01f;
            }
        }
       
  

        //破甲buff
        List<BattleBuff> poJiaBuffList = FindBuffTypeBuffList(p1, BattleBuffType.PoJia);
        if (poJiaBuffList.Count>0)
        {
            for(int i = poJiaBuffList.Count - 1; i >= 0; i--)
            {
                poJia += poJiaBuffList[i].poJiaRate * 0.01f;
            }
            //totalResAdd += haoRanBuff.commonParam.ToFloat() * 0.01f;
        }
        attack = attack * (1 + jiaGongRate * 0.01f);
        defence = defence * (1 + defenceAddRate * 0.01f);
        if (attack < 0)
        {
            Debug.Log("攻击为0");
            attack = 0;
        }

        //灼烧buff
        //BattleBuff zhuoShaoBuff = FindBuffTypeBuff(p2, BattleBuffType.ZhuoShao);
        //if (zhuoShaoBuff != null)
        //{
        //    attack = attack * (1 + zhuoShaoBuff.dieValue * 0.01f);

        //}

        if (skill != null)
        {
            yuanSuDamageAdd= GetYuanSuProDamageAdd(skill.yuanSuType, p1);
            //todo防止运算过大 这里放在战斗初始化处 缓存起来
            skillSetting = FindSkillSetting(skill.skillId);
            List<SkillUpgradeSetting> skillUpgradeSettingList =  FindSkillUpgradeListBySkillId(skill.skillId);
            SkillUpgradeSetting choosedSetting = skillUpgradeSettingList[skill.skillLevel - 1];
            string choosedLevelStr = "";
            string[] skillAddArr = null;
            //如果是多重
            if (choosedSetting.Damage.Contains("|"))
            {
                //string[] theSkillArr = skillSetting.upgradeDamagePercentArr.Split('$');
                //choosedLevelStr = theSkillArr[skill.SkillLevel - 1];
                skillAddArr = choosedSetting.Damage.Split('|');
            }
            else
            {
                skillAddArr = choosedSetting.Damage.Split('|');

            }

            //string[] skillAddArr = choosedLevelStr.Split('|');

            skillAdd = 0.01f * skillAddArr[damageIndex].ToFloat();

            //火焰爆轰加暴击率
            if (skill.skillId == (int)SkillIdType.LieHuoFenTian)
            {
                critRate += 0.1f * FindTypeBuffList(p2, BattleBuffType.ZhuoShao).Count;
                   //critRate += 0.3f;
            }
            //霜爆在减速状况下额外增加伤害
            else if (skill.skillId == (int)SkillIdType.ShuangBao)
            {
                if (FindIdBuff(p2, BattleBuffIdType.ChaoShi) != null)
                {
                    skillAdd *= 1.3f;
                }
            }
            if (skillSetting != null && skillSetting.PuGong == "1")
            {
                //易感 buff
                List<BattleBuff> yiGanBuff = FindTypeBuffList(p2, BattleBuffType.YiGan);
                if (yiGanBuff.Count > 0)
                {
                    totalResAdd += yiGanBuff[0].commonParam.ToFloat() * 0.01f;
                }
                //风影buff
                List<BattleBuff> fengYingBuff = FindTypeBuffList(p1, BattleBuffType.FengYing);
                if (fengYingBuff.Count > 0)
                {
                    BattleBuff singleFengYing = fengYingBuff[0];
                    critRate += singleFengYing.buffSetting.Param.ToFloat() * 0.01f;
                    critNum += singleFengYing.buffSetting.Param2.ToFloat() * 0.01f;
                }
            }
        }

        


        float critMul = 1;
        if (RandomManager.Next(0, 100) < critRate * 100)
        {
            crit = true;
            critMul = 1 + critNum;
        }
   
        float skillAddVal = (1 + skillAdd);
        int res = 0;
        ReactionType reactionType = ReactionType.None;
        if (reactionAttackType != ReactionType.None)
        {
            reactionType = reactionAttackType;
        }
        //如果是buff攻击 则计算p1身上的buff（例如灼烧）
        if (buffAttack!=null)
        {
            PeopleData enemy = null;
            List<BattleBuff> buffList = null;
            if (buffAttack.burnVal != 0)
            {
                if (CheckIfLeftP(p2))
                {
                    buffList = buffList1;
                    enemy = p2List[p2Index];
                }
                else
                {
                    buffList = buffList2;
                    enemy = p2List[p2Index];
                }
                int buffAddVal = 0;
                for (int i = 0; i < buffList.Count; i++)
                {
                    BattleBuff buff = buffList[i];
                    if (buff.burnVal != 0)
                    {
                        res += Mathf.CeilToInt((buff.burnVal * 0.01f + 1) * (attack * attack / (attack + defence)));
                    }
               
                    //buffAddVal += buff.burnVal;
                }
            }
            //剧变反应
            else if (buffAttack.reactionType!=ReactionType.None
                &&buffAttack.reactionType == reactionAttackType)
            {
                res += Mathf.CeilToInt(YuanSuReactionManager.Instance.CalcJuBianDamage(p1, buffAttack.reactionDamageRate,jingTong));
            }
            //如果是堕仙造成伤害
            else if (buffAttack.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.HurtEnemy)
            {
                float theAttackVal = attack * (buffAttack.buffSetting.Param.ToInt32() * 0.01f);
                res += (int)theAttackVal;
            }
            //火印伤害
            else if (buffAttack.buffSetting.Id.ToInt32() == (int)BattleBuffIdType.HuoYin)
            {
            
                List<BattleBuff> huoYinList = FindIDBuffList(p2, BattleBuffIdType.HuoYin);
                SingleSkillData huoYinSkill = huoYinList[0].skillData;
                List<SkillUpgradeSetting> upgradeSettingList = FindSkillUpgradeListBySkillId(huoYinSkill.skillId);
                SkillUpgradeSetting choosedSetting = upgradeSettingList[huoYinSkill.skillLevel - 1];
                List<float> param = CommonUtil.SplitCfgFloatOneDepth(choosedSetting.Param2);
                int huoYinVal =(int)MathF.Round( param[0]);
                huoYinVal *= huoYinList.Count;
                res += Mathf.CeilToInt((huoYinVal * 0.01f + 1) * (attack * attack / (attack + defence)));
            }
            //SkillIdType
            //天龙套真伤 普攻就触发 记得判定普攻
            else if (buffAttack.buffSetting.Id.ToInt32() == (int)BattleBuffIdType.TianLongTaoZhenShang)
            {
                float theAttackVal = attack * (buffAttack.buffSetting.Param.ToInt32() * 0.01f);
                res += (int)theAttackVal;
                trueDamage = true;
            }

        }
        //如果是剧变伤害
        else if (buffAttack==null && reactionAttackType != ReactionType.None)
        {
            //爆炸
            if (reactionAttackType == ReactionType.BaoZha)
            {
                //爆炸
                res = YuanSuReactionManager.Instance.CalcJuBianDamage(p1, ConstantVal.BaoZhaBase, jingTong);
                //掉能量
                RoleManager.Instance.DeBattleProperty(PropertyIdType.MpNum, -ConstantVal.BaoZhaEnergyLose,p2);
            }  
            //雷枪
            else if (reactionAttackType == ReactionType.LeiQiang)
            {            
                res = YuanSuReactionManager.Instance.CalcJuBianDamage(p1, ConstantVal.LeiQiangBase, jingTong);
                //麻痹buff
                AddBattleBuff(p2,DataTable.FindBattleBuffSetting((int)BattleBuffIdType.LeiQiangMaBi));
            }
        }
        //正常伤害
        else
        {
            float yuanSuBeiLv=1;//元素倍率（蒸发融化）
            List<YuanSuType> handleYuanSuList = null;
            bool handleLeft = false;
            bool haveYuanSuHuZhao = false;//有元素护罩
            #region 元素反应
            if (p2.onlyId == p2List[p2Index].onlyId)
            {
        

                handleYuanSuList = yuanSuList2;
                handleLeft = false;
            }
            else
            {     
          
                handleYuanSuList = yuanSuList1;
                handleLeft = true;
            }
            //如果有元素盾
            List<BattleBuff> huZhaoBuffList = FindTypeBuffList(p2, BattleBuffType.HuZhao);
            if (huZhaoBuffList != null)
            {
                for (int i = 0; i < huZhaoBuffList.Count; i++)
                {
                    BattleBuff b = huZhaoBuffList[i];
                    if (!string.IsNullOrWhiteSpace(b.buffSetting.Param4))
                    {
                        haveYuanSuHuZhao = true;
                        break;
                    }
                }
            }
            if (handleYuanSuList != null
                &&!haveYuanSuHuZhao)
            {
                if (skill != null)
                {
                    handleYuanSuList.Add(skill.yuanSuType);
                    YuanSuReactionRes yuanSuReactionRes = YuanSuReactionManager.Instance.CheckYuanSuReaction(handleYuanSuList);
                    reactionType = yuanSuReactionRes.reactionType;
                    //if(reactionType==)
                    if (reactionType != ReactionType.None)
                    {
                        for (int i = handleYuanSuList.Count - 1; i >= 0; i--)
                        {
                            for (int j = yuanSuReactionRes.removedYuanSuList.Count - 1; j >= 0; j--)
                            {
                                YuanSuType yuan = yuanSuReactionRes.removedYuanSuList[j];
                                handleYuanSuList.Remove(yuan);
                                yuanSuReactionRes.removedYuanSuList.Remove(yuan);

                            }
                        }
                        handleYuanSuList = yuanSuReactionRes.remainYuanSuList;
                        reactionType = yuanSuReactionRes.reactionType;
                        if (reactionType == ReactionType.BaoZheng)
                        {
                            RdmQuSanAGoodBuff(handleLeft);
                            yuanSuBeiLv = 1.5f;
                        }
                        //融化
                        else if (reactionType == ReactionType.RongHua)
                            yuanSuBeiLv = 2f;
                        else if (reactionType == ReactionType.ChaoShi)
                            AddBattleBuff(p2, DataTable.FindBattleBuffSetting((int)BattleBuffIdType.ChaoShi));
                        //电流
                        else if (reactionType == ReactionType.DianLiu)
                        {
                            AddBattleBuff(p2, DataTable.FindBattleBuffSetting((int)BattleBuffIdType.DianLiu));
                        }
                        //冻僵
                        else if (reactionType == ReactionType.DongJiang)
                        {
                            bool success= AddBattleBuff(p2, DataTable.FindBattleBuffSetting((int)BattleBuffIdType.DongJiang));
                            if(success)
                            EventCenter.Broadcast(TheEventType.OnDongJiang, p2);
                        }
                        //灼烧
                        else if (reactionType == ReactionType.ZhuoShao)
                        {
                            //技能和普攻技能差不多
                            SingleSkillData zhuoShaoSkill = new SingleSkillData();
                            zhuoShaoSkill.skillId = (int)SkillIdType.FeiXueZhiMao;

                            SkillSetting setting = DataTable.FindSkillSetting(zhuoShaoSkill.skillId);
                            if (!skillSettingHuanCunIdList.Contains(zhuoShaoSkill.skillId))
                            {
                                skillSettingHuanCunIdList.Add(zhuoShaoSkill.skillId);
                                skillSettingHuanCun.Add(setting);
                                for (int k = 0; k < DataTable._skillUpgradeList.Count; k++)
                                {
                                    SkillUpgradeSetting upgradesetting = DataTable._skillUpgradeList[k];
                                    int upgradeId = upgradesetting.Id.ToInt32();
                                    if (upgradesetting.SkillId.ToInt32() == zhuoShaoSkill.skillId
                                        && !skillUpgradeSettingHuanCunIdList.Contains(upgradeId))
                                    {
                                        skillUpgradeSettingHuanCun.Add(upgradesetting);
                                        skillUpgradeSettingHuanCunIdList.Add(upgradeId);
                                    }
                                }
                            }

                            zhuoShaoSkill.skillLevel = skill.skillLevel;
                            for (int i = 0; i < 2; i++)
                            {
                                AddBattleBuff(p2, DataTable.FindBattleBuffSetting((int)BattleBuffIdType.ZhuoShao2), zhuoShaoSkill);
                            }
                        }
                        //爆炸
                        else if (reactionType == ReactionType.BaoZha)
                        {
                            AttackResData juBianRes = CalcAttack(p1, p2, null, 0, null, ReactionType.BaoZha);
                            if (juBianRes != null)
                            {
                                EventCenter.Broadcast(TheEventType.ShowYuanSuReactionNum, juBianRes);
                                EventCenter.Broadcast(TheEventType.BattleBeHit, juBianRes);

                                if (juBianRes.fanShangResData != null)
                                {
                                    EventCenter.Broadcast(TheEventType.BattleDeHpShow, juBianRes.fanShangResData);
                                    EventCenter.Broadcast(TheEventType.BattleBeHit, juBianRes.fanShangResData);
                                }
                                BattleManager.Instance.JudgeIfPeopleDead();
                            }
                        
                        }
                        //磁暴
                        else if (reactionType == ReactionType.CiBao)
                        {

                            //封脉
                            if (AddBattleBuff(p2, DataTable.FindBattleBuffSetting((int)BattleBuffIdType.CiBaoFengMai)))
                            {
                                //掉能量
                                RoleManager.Instance.DeBattleProperty(PropertyIdType.MpNum, -ConstantVal.CiBaoEnergyLose, p2);
                            }
                        }
                        //雷枪
                        else if (reactionType == ReactionType.LeiQiang)
                        {
                            AttackResData juBianRes = CalcAttack(p1, p2, null, 0, null, ReactionType.LeiQiang);
                            if (juBianRes != null)
                            {
                                EventCenter.Broadcast(TheEventType.ShowYuanSuReactionNum, juBianRes);
                                EventCenter.Broadcast(TheEventType.BattleBeHit, juBianRes);

                                if (juBianRes.fanShangResData != null)
                                {
                                    EventCenter.Broadcast(TheEventType.BattleDeHpShow, juBianRes.fanShangResData);
                                    EventCenter.Broadcast(TheEventType.BattleBeHit, juBianRes.fanShangResData);
                                }
                                BattleManager.Instance.JudgeIfPeopleDead();
                            }
                     
                        }
                        //冰牢
                        else if (reactionType == ReactionType.BingLao)
                        {
                            bool success=AddBattleBuff(p2, DataTable.FindBattleBuffSetting((int)BattleBuffIdType.BingLao));
                            if(success)
                            EventCenter.Broadcast(TheEventType.OnDongJiang, p2);
                        }
                        //凶煞
                        else if (reactionType == ReactionType.XiongSha)
                        {
                            List<BattleBuffIdType> candidateList = new List<BattleBuffIdType>();
                            for (int i = 0; i < ConstantVal.badBuffPoolList.Count; i++)
                            {
                                candidateList.Add(ConstantVal.badBuffPoolList[i]);
                            }
                            for (int i = 0; i <1; i++)
                            {
                                int index = RandomManager.Next(0, candidateList.Count);
                                AddBattleBuff(p2, DataTable.FindBattleBuffSetting((int)candidateList[index]));
                                candidateList.RemoveAt(index);
                            }
                        }
                        //赐福
                        else if (reactionType == ReactionType.CiFu)
                        {
                            //驱散一个负面buf
                            //RdmQuSanABadBuff(1, CheckIfLeftP(p1));
                            List<BattleBuffIdType> candidateList = new List<BattleBuffIdType>();
                            for (int i = 0; i < ConstantVal.goodBuffPoolList.Count; i++)
                            {
                                candidateList.Add(ConstantVal.goodBuffPoolList[i]);
                            }
                            int index = RandomManager.Next(0, candidateList.Count);
                            AddBattleBuff(p1, DataTable.FindBattleBuffSetting((int)candidateList[index]));
                            candidateList.RemoveAt(index);

                        }
                        //触发反应
                        //4件套
                        if (p1TaoZhuangList.Count >= 2
                                && p1TaoZhuangList[0] == p1TaoZhuangList[1])
                        {
                            //月华套+精通
                            if (p1TaoZhuangList[0] == EquipTaoZhuangType.YueHua)
                            {
                                EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)p1TaoZhuangList[0]);
                                BattleBuffSetting buffSetting = DataTable.FindBattleBuffSetting(taoZhuangSetting.Param2.ToInt32());
                                AddBattleBuff(p1, buffSetting);
                            }
                        }
                    }
                    while (handleYuanSuList.Count > 3)
                    {
                        handleYuanSuList.RemoveAt(0);
                    }
                }
          
                //打完敌人决定是否切人
                //if (!CheckIfLeftP(p1) &&)
            }

            #endregion
            if (trueDamage)
                defence = 0;
            float mianShang = (attack + defence * (1 - poJia));
            if (mianShang < 0)
                mianShang = 0.01f;
            //ConstantVal.baseBattleProperty
            if (xieZhan)
            {
                //有没有原始套
                if (p1TaoZhuangList.Count >= 2
                                 && p1TaoZhuangList[0] == p1TaoZhuangList[1]
                                 &&p1TaoZhuangList[0]==EquipTaoZhuangType.YuanShi)
                {
                    EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)p1TaoZhuangList[0]);
                    totalResAdd+= taoZhuangSetting.Param2.ToFloat() * 0.01f;
                }
            }
            res = Mathf.CeilToInt((1 + yuanSuDamageAdd)*(1+totalResAdd)* skillAddVal * (attack * attack / mianShang) * critMul* yuanSuBeiLv);

        }
        res  =Mathf.CeilToInt( res*(1 - mianShangNum));
        //携战
        if (xieZhan)
        {
            float xieZhanRate = XieZhanRate(p1);
            res =(int)(xieZhanRate* res);
        }

        res  = (int)(res *levelPressDamageAdd * levelPressDamageDe);
        //恶意buff
        BattleBuff eyiBuff = FindIdBuff(p2, BattleBuffIdType.Eyi);
        if (eyiBuff != null&&!trueDamage)
        {
            res *= 2;
        }
        //易伤buff
        BattleBuff yiShangBuff = FindBuffTypeBuff(p2, BattleBuffType.YiShang);
        if (yiShangBuff != null && !trueDamage)
        {
            res  =Mathf.CeilToInt(res* 1.5f);
        }
        //受击者buff
        BattleBuff shieldBuff = FindBuffTypeBuff(p2, BattleBuffType.Shield);
        if (shieldBuff != null)
        {
            float jianShangRate = 1 / 1.5f;
            int initremainNum = shieldBuff.reMainShieldNum;
            int deShieldNum = Mathf.RoundToInt(res - res * jianShangRate);

            int remainNum = initremainNum - deShieldNum;
            //盾爆了
            if (remainNum <= 0)
            {
                //盾作用下的减血量
                res -= initremainNum;
                RemoveBuff(p2, shieldBuff);
            }
            //盾没爆
            else
            {
                shieldBuff.reMainShieldNum -= deShieldNum;
                res -= deShieldNum;
            }

        }
        int showDeHP = -res;
        #region 套装 帝血
        if (p1TaoZhuangList.Count > 0)
        {
            //4件套
            if (p1TaoZhuangList.Count >= 2
                    && p1TaoZhuangList[0] == p1TaoZhuangList[1])
            {
                //帝血
                if (p1TaoZhuangList[0] == EquipTaoZhuangType.DiXue)
                {
                    if (crit)
                    {
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)p1TaoZhuangList[0]);

                        //帝血4件套
                        List<int> paramList2 = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param2);
                        if (critRate >= paramList2[0]*0.01f)
                        {
                            float hpAddRate = paramList2[1] * 0.01f;
                            //吸血
                            BattleManager.Instance.AddHP( p1, hpAddRate*res);
                        }
                     
                    }

                }  
                //天龙套真伤
                else if (p1TaoZhuangList[0] == EquipTaoZhuangType.TianLong)
                {
                    if (skillSetting != null&&skillSetting.PuGong=="1")
                    {
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)p1TaoZhuangList[0]);

                        //天龙4件套
                        //List<int> paramList2 = CommonUtil.SplitCfgOneDepth()
                        BattleBuffSetting buffSetting = DataTable.FindBattleBuffSetting(taoZhuangSetting.Param2.ToInt32());
                        AddBattleBuff(p1, buffSetting);
                    }

                }
            }
        }
        #endregion
        //护罩buff
        BattleBuff huZhaoBuff = FindBuffTypeBuff(p2, BattleBuffType.HuZhao);
        if (huZhaoBuff != null)
        {
            int initremainNum = huZhaoBuff.remainHuZhaoNum;
            //先减去元素反应的量
            int yuanSuDeNum = 0;
            if (skill != null)
            {
                yuanSuDeNum = PoYuanSuDun(skill.yuanSuType, huZhaoBuff);

            }
            int remainNum = initremainNum - yuanSuDeNum;
            remainNum = remainNum - res;
            //盾爆了
            if (remainNum <= 0)
            {
                //盾作用下的减血量
                res -= res;
                RemoveBuff(p2, huZhaoBuff);
                //如果是天音护盾 则施加易伤
                if (huZhaoBuff.buffSetting.Id.ToInt32() == (int)BattleBuffIdType.TianYinHuDun
                    || huZhaoBuff.buffSetting.Id.ToInt32() == (int)BattleBuffIdType.HuoDun
                    || huZhaoBuff.buffSetting.Id.ToInt32() == (int)BattleBuffIdType.GuangDun)
                {
                    RemoveAllBuff(p2);
                    // 使用 SplitCfgStringOneDepth 保留原始字符串格式
                    List<string> yiShangStrList = CommonUtil.SplitCfgStringOneDepth(huZhaoBuff.buffSetting.Param2);
                    if (yiShangStrList.Count > 1 && !string.IsNullOrEmpty(yiShangStrList[1]))
                        AddBattleBuff(p2, DataTable.FindBattleBuffSetting(yiShangStrList[1]));
                }
            }
            //盾没爆
            else
            {
                huZhaoBuff.remainHuZhaoNum = remainNum;
                res -= res;
            }
        }


        
        //反伤buff
        BattleBuff fanShangBuff = FindBuffTypeBuff(p2, BattleBuffType.FanShang);
        AttackResData fanRes = null;
        if (fanShangBuff != null&&!xieZhan)
        {
            int fanNum =(int)( res*(fanShangBuff.buffSetting.Param.ToInt32()*0.01f));
            int limit = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p1).num;
            fanNum = Mathf.Min(fanNum, limit);
            RoleManager.Instance.DeBattleProperty(PropertyIdType.Hp, -fanNum, p1);
            fanRes = new AttackResData(-fanNum, crit, true, p1, skill, damageIndex) ;
            fanRes.showDeHP = -fanNum;
        }
        RoleManager.Instance.DeBattleProperty(PropertyIdType.Hp, -res, p2);

        bool someoneDead = false;
        if(RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p2).num <= 0)
        {
            someoneDead = true;
        }

        //受击加属性
        #region 套装  
        if (p2TaoZhuangList.Count > 0)
        {
            //4件套
            if (p2TaoZhuangList.Count >= 2
                    && p2TaoZhuangList[0] == p2TaoZhuangList[1])
            {
                //玄武 受击加属性
                if (p2TaoZhuangList[0] == EquipTaoZhuangType.XuanWu)
                {
                    EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)p2TaoZhuangList[0]);
                    List<int> paramList2 = CommonUtil.SplitCfgOneDepth(taoZhuangSetting.Param2);
                    for(int i = 0; i < paramList2.Count; i++)
                    {
                        int buffId = paramList2[i];
                        AddBattleBuff(p2, DataTable.FindBattleBuffSetting(buffId));
                    }

                }

            }
        }
        #endregion

        AttackResData attackResData = new AttackResData(-res, crit, someoneDead,p2,skill, damageIndex);
        if (fanRes != null)
            attackResData.fanShangResData = fanRes;
        if (reactionType != ReactionType.None)
        {
            attackResData.reactionType = reactionType;
        }
        attackResData.showDeHP = showDeHP;
        return attackResData;
    }

    /// <summary>
    /// 元素增伤
    /// </summary>
    /// <returns></returns>
    public float GetYuanSuProDamageAdd(YuanSuType yuanSuType,PeopleData p)
    {
        int res = 0;
        switch (yuanSuType) 
        {
            case YuanSuType.Water:
                res =RoleManager.Instance.GetCurBattlePropertyNum(PropertyIdType.WaterDamageAdd, p);
                break;
            case YuanSuType.Fire:
                res = RoleManager.Instance.GetCurBattlePropertyNum(PropertyIdType.FireDamageAdd, p);
                break;
            case YuanSuType.Storm:
                res = RoleManager.Instance.GetCurBattlePropertyNum(PropertyIdType.StormDamageAdd, p);
                break;
            case YuanSuType.Ice:
                res = RoleManager.Instance.GetCurBattlePropertyNum(PropertyIdType.IceDamageAdd, p);
                break;
            case YuanSuType.Light:
                res = RoleManager.Instance.GetCurBattlePropertyNum(PropertyIdType.YangProDamageAdd, p);
                break;
            case YuanSuType.Dark:
                res = RoleManager.Instance.GetCurBattlePropertyNum(PropertyIdType.YinProDamageAdd, p);
                break;
        }
        res += RoleManager.Instance.GetCurBattlePropertyNum(PropertyIdType.TotalProDamageAdd, p);

        return res * 0.0001f;
     } 

    /// <summary>
    /// 一波攻击打完后 决定对手是否切人
    /// </summary>
    /// <returns></returns>
    public int DetermineIfEnemyQie(QieRenPurposeType type)
    {
        if (type == QieRenPurposeType.YuanSuReaction)
        {
            YuanSuType[] arr = yuanSuList1.ToArray();
            List<YuanSuType[]> ListCombination = PermutationAndCombination<YuanSuType>.GetCombination(arr, 2); //求全部的2-2组合
                                                                                                               //2-2组合
            if (ListCombination != null)
            {
                for (int i = 0; i < ListCombination.Count; i++)
                {
                    List<YuanSuType> single = ListCombination[i].ToList();

                    if (single[0] == single[1]
                        && single[0] != YuanSuType.Light
                        && single[0] != YuanSuType.Dark)
                    {
                        //同时 敌人如果有非dark和light的元素
                        PeopleData curP = p2List[p2Index];
                        for (int j = 0; j < p2List.Count; j++)
                        {
                            PeopleData theP = p2List[j];
                            if (curP.yuanSu != theP.yuanSu
                             && curP.yuanSu != (int)YuanSuType.Dark
                             && curP.yuanSu != (int)YuanSuType.Light
                                && theP.yuanSu != (int)YuanSuType.Dark
                                && theP.yuanSu != (int)YuanSuType.Light
                                && RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, theP).num > 0)
                            {
                                return j;
                            }
                        }
                     }
                }
            }
        }
        //放大切人
        else if (type == QieRenPurposeType.FangDa)
        {
            PeopleData curP = p2List[p2Index];
            for(int i = 0; i < p2List.Count; i++)
            {
                PeopleData theP = p2List[i];
                if(theP.onlyId!=curP.onlyId
                    &&RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum,theP).num>=100
                    && RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, curP).num < 100
                    && RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, theP).num > 0)
                {
                    return i;
                }
            }
        }
      
        return -1;
    }
 
    /// <summary>
    /// 结算
    /// </summary>
    public void MatchResult(bool win)
    {
        //EquipmentManager.Instance.DeDurable(RoleManager.Instance._CurGameInfo.playerPeople.CurEquip, -30);

        GetMatchAward();
        GameTimeManager.Instance.GetServiceTime((x) =>
        {
            if (x > 0)
            {
                RoleManager.Instance._CurGameInfo.MatchData.LastParticipateMatchTime = x;

            }
        });
        RoleManager.Instance._CurGameInfo.MatchData.TodayParticipateMatchNum++;
        if (RoleManager.Instance._CurGameInfo.MatchData.TodayParticipateMatchNum >= 3)
        {
            if (RoleManager.Instance._CurGameInfo.MatchData.TodayAwardGetStatusList[2] != (int)AccomplishStatus.GetAward)
            {
                RoleManager.Instance._CurGameInfo.MatchData.TodayAwardGetStatusList[2] = (int)AccomplishStatus.Accomplished;
            }
        }
        if (RoleManager.Instance._CurGameInfo.MatchData.TodayParticipateMatchNum >= 2)
        {
            if (RoleManager.Instance._CurGameInfo.MatchData.TodayAwardGetStatusList[1] != (int)AccomplishStatus.GetAward)
            {
                RoleManager.Instance._CurGameInfo.MatchData.TodayAwardGetStatusList[1] = (int)AccomplishStatus.Accomplished;
            }
        }
        if (RoleManager.Instance._CurGameInfo.MatchData.TodayParticipateMatchNum >= 1)
        {
            if (RoleManager.Instance._CurGameInfo.MatchData.TodayAwardGetStatusList[0] != (int)AccomplishStatus.GetAward)
            {
                RoleManager.Instance._CurGameInfo.MatchData.TodayAwardGetStatusList[0] = (int)AccomplishStatus.Accomplished;
            }
        }

        if (win)
        {
            RoleManager.Instance._CurGameInfo.MatchData.TodayWinNum++;

            for(int i=0;i< RoleManager.Instance._CurGameInfo.MatchData.TodayWinNum; i++)
            {
                if (i > RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList.Count-1)
                    break;
                if (RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList[i]!= (int)AccomplishStatus.GetAward)
                {
                    RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList[i]= (int)AccomplishStatus.Accomplished;
                }

            }
        }
 
    }
    /// <summary>
    /// 获得冠军 解锁下个赛事并记录
    /// </summary>
    public void GetChampion()
    {
       
    
    }

    /// <summary>
    /// 拿奖
    /// </summary>
    public void GetMatchAward()
    {
        if(roleAward!=null)
        ItemManager.Instance.GetItem(roleAward[0].settingId, roleAward[0].count);
        return;


    }

    /// <summary>
    /// 显示技能
    /// </summary>
    public void ShowBattleEffect(PeopleData p,SingleSkillData skill)
    {
        SkillSetting setting = FindSkillSetting(skill.skillId);
        EventCenter.Broadcast(TheEventType.ShowBattleEffect, p, setting.EffectName);
    }

    /// <summary>
    /// 亲自开始秘境守卫战斗
    /// </summary>
    public void StartMiJingGuardBattle(MiJingLevelSetting levelSetting,List<int> enemyIdList)
    {
        if (!GameTimeManager.Instance.connectedToFuWuQiTime)
        {
            PanelManager.Instance.OpenFloatWindow("必须在联网环境下进行");
            return;
        }

        TDGAMission.OnBegin(levelSetting.Des);
        studentBattle = false;
        MiJingManager.Instance.curBattleMijingLevelId = levelSetting.Id.ToInt32();
        //curBattleLevelId = levelId;
        //EnemySetting enemySetting = DataTable.FindEnemySetting(levelSetting.enemyId.ToInt32());
        //默认选第一个装备
        //RoleManager.Instance.SetCurEquipAndSkill();

        curBattleType = BattleType.MiJingGuardBattle;
        //PeopleData p1 = RoleManager.Instance._CurGameInfo.playerPeople;

        List<PeopleData> p2List = new List<PeopleData>();
        for(int i = 0; i < enemyIdList.Count; i++)
        {
            int enemyLv = 0;
            if (!string.IsNullOrWhiteSpace(levelSetting.RelevantLevel))
            {
                enemyLv = levelSetting.RelevantLevel.ToInt32();
            }
            PeopleData p2 = GenerateMiJingBattleEnemy(DataTable.FindEnemySetting(enemyIdList[i]), enemyLv);
            p2List.Add(p2);
        }

        List<PeopleData> theP1List = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1[i];
            if (onlyId <= 0)
                continue;
            if (onlyId ==RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
                theP1List.Add(RoleManager.Instance._CurGameInfo.playerPeople);
            else
                theP1List.Add(StudentManager.Instance.FindStudent(onlyId));

        }
        //List<PeopleData> theP2List = new List<PeopleData> { p2 };

        BattleManager.Instance.BattlePrepare(ref theP1List, ref p2List, true, true);
        BattleViewPrepare();

        GameSceneManager.Instance.GoToScene(SceneType.Battle);
    }


   
    /// <summary>
    /// 亲自开始npc切磋战斗
    /// </summary>
    public void StartQieCuoBattle(ulong npcId)
    {


        studentBattle = false;

        //curBattleLevelId = levelId;

        //默认选第一个装备
        //RoleManager.Instance.SetCurEquipAndSkill();

        curBattleType = BattleType.QieCuoBattle;
        PeopleData p1 =RoleManager.Instance._CurGameInfo.playerPeople;
        SingleNPCData npcData = TaskManager.Instance.FindNPCByOnlyId(npcId);
        PeopleData p2 = npcData.PeopleData;
        RefreshEnemyProperty(p2, DataTable.FindEnemySetting(p2.enemySettingId),p2.trainIndex+1);
        List<PeopleData> theP1List = new List<PeopleData>() { p1 };

        List<PeopleData> theP2List = new List<PeopleData> { p2 };
        BattleManager.Instance.BattlePrepare(ref theP1List, ref theP2List, true, false);
        BattleViewPrepare();

        GameSceneManager.Instance.GoToScene(SceneType.Battle);
    }
    /// <summary>
    /// 亲自开始帝姝第一次战斗
    /// </summary>
    public void StartDiShuFirstBattle(ulong npcId)
    {
        studentBattle = false;

        //curBattleLevelId = levelId;

        //默认选第一个装备
        //RoleManager.Instance.SetCurEquipAndSkill();

        curBattleType = BattleType.DiShuFirstBattle;
        PeopleData p1 =RoleManager.Instance._CurGameInfo.playerPeople;
        SingleNPCData npcData1 = TaskManager.Instance.FindNPCByOnlyId(npcId);
        PeopleData p2 = npcData1.PeopleData;
        p2.enemySettingId = (int)EnemyIdType.DiShu;
        RefreshEnemyProperty(p2, DataTable.FindEnemySetting(p2.enemySettingId),10);

        List<PeopleData> theP1List = new List<PeopleData> { p1 };
        List<PeopleData> theP2List = new List<PeopleData> { p2 };
        BattleManager.Instance.BattlePrepare(ref theP1List, ref theP2List, true, false);
        BattleViewPrepare();

        GameSceneManager.Instance.GoToScene(SceneType.Battle);
    }
    /// <summary>
    /// 亲自开始狸猫掌门战斗
    /// </summary>
    public void StartLiMaoZhangMenBattle(PeopleData p)
    {
        studentBattle = false;

        //curBattleLevelId = levelId;

        //默认选第一个装备
        //RoleManager.Instance.SetCurEquipAndSkill();

        curBattleType = BattleType.LiMaoZhangMenBattle;
        //PeopleData p1 = RoleManager.Instance._CurGameInfo.playerPeople;
        PeopleData p2 = p;
        RefreshEnemyProperty(p2, DataTable.FindEnemySetting(p2.enemySettingId), DataTable.FindEnemySetting(p2.enemySettingId).Level.ToInt32());

        List<PeopleData> theP1List = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1[i];
            if (onlyId <= 0)
                continue;
            if (onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
                theP1List.Add(RoleManager.Instance._CurGameInfo.playerPeople);
            else
                theP1List.Add(StudentManager.Instance.FindStudent(onlyId));

        }
        List<PeopleData> theP2List = new List<PeopleData> { p2 };
        BattleManager.Instance.BattlePrepare(ref theP1List, ref theP2List, true, false);
        BattleViewPrepare();

        GameSceneManager.Instance.GoToScene(SceneType.Battle);
    }
    /// <summary>
    /// 亲自开始头子第一次战斗
    /// </summary>
    public void StartTouziFirstBattle(ulong npcId)
    {
        studentBattle = false;

        //curBattleLevelId = levelId;

        //默认选第一个装备
        //RoleManager.Instance.SetCurEquipAndSkill();

        curBattleType = BattleType.TouZiFirstBattle;
        PeopleData p1 = RoleManager.Instance._CurGameInfo.playerPeople;
        SingleNPCData npcData2 = TaskManager.Instance.FindNPCByOnlyId(npcId);
        PeopleData p2 = npcData2.PeopleData;
        RefreshEnemyProperty(p2, DataTable.FindEnemySetting(p2.enemySettingId), DataTable.FindEnemySetting(p2.enemySettingId).Level.ToInt32());

        List<PeopleData> theP1List = new List<PeopleData> { p1 };
        List<PeopleData> theP2List = new List<PeopleData> { p2 };
        BattleManager.Instance.BattlePrepare(ref theP1List, ref theP2List, true, false);
        BattleViewPrepare();

        GameSceneManager.Instance.GoToScene(SceneType.Battle);
    }

    /// <summary>
    /// 亲自开始npc生死战斗
    /// </summary>
    public void StartNPCKillBattle(ulong npcId)
    {


        studentBattle = false;

        //curBattleLevelId = levelId;

        //默认选第一个装备
        //RoleManager.Instance.SetCurEquipAndSkill();

        curBattleType = BattleType.NPCKillBattle;
        //PeopleData p1 = RoleManager.Instance._CurGameInfo.playerPeople;
        SingleNPCData npcData3 = TaskManager.Instance.FindNPCByOnlyId(npcId);
        PeopleData p2 = npcData3.PeopleData;
        RefreshEnemyProperty(p2, DataTable.FindEnemySetting(p2.enemySettingId), DataTable.FindEnemySetting(p2.enemySettingId).Level.ToInt32());
        List<PeopleData> theP1List = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1[i];
            if (onlyId <= 0)
                continue;
            if (onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
                theP1List.Add(RoleManager.Instance._CurGameInfo.playerPeople);
            else
                theP1List.Add(StudentManager.Instance.FindStudent(onlyId));

        }
        List<PeopleData> theP2List = new List<PeopleData> { p2 };
        BattleManager.Instance.BattlePrepare(ref theP1List, ref theP2List, false, false);
       
        BattleViewPrepare();

        GameSceneManager.Instance.GoToScene(SceneType.Battle);
    }
    /// <summary>
    /// 开始固定关卡战斗
    /// </summary>
    public void StartFixedLevelBattle(string levelId)
    {
        LevelSetting levelSetting = DataTable.FindLevelSetting(levelId);
        TDGAMission.OnBegin(levelSetting.Level);
        studentBattle = false;


        curBattleType = BattleType.FixedLevelBattle;
        PeopleData p1 = RoleManager.Instance._CurGameInfo.playerPeople;
        SingleLevelData fixLevel = MapManager.Instance.FindFixedLevelById(levelId);

        List<PeopleData> theP2List = new List<PeopleData>();

        for (int i = 0; i < fixLevel.Enemy.Count; i++)
        {
            PeopleData p2 =fixLevel.Enemy[i];
            RefreshEnemyProperty(p2, DataTable.FindEnemySetting(p2.enemySettingId), DataTable.FindEnemySetting(p2.enemySettingId).Level.ToInt32());
            theP2List.Add(p2);
        }


        //PeopleData p2 = MapManager.Instance.FindFixedLevelById(levelId).Enemy;
        List<PeopleData> theP1List = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1[i];
            if (onlyId <= 0)
                continue;
            if (onlyId == RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
                theP1List.Add(RoleManager.Instance._CurGameInfo.playerPeople);
            else
                theP1List.Add(StudentManager.Instance.FindStudent(onlyId));

        }
        BattleManager.Instance.BattlePrepare(ref theP1List, ref theP2List, true, true);
        if (levelSetting.Leveltype.ToInt32() == (int)LevelType.BossBattle)
        {
            for (int i = 0; i < theP2List.Count; i++)
            {
                RoleManager.Instance.FullMP(theP2List[i]);

            }

        }
        BattleViewPrepare();

        GameSceneManager.Instance.GoToScene(SceneType.Battle);
    }
    /// <summary>
    /// 开始关卡战斗
    /// </summary>
    public void StartLevelBattle(string levelId)
    {
        LevelSetting levelSetting = DataTable.FindLevelSetting(levelId);
        TDGAMission.OnBegin(levelSetting.Level);
        studentBattle = false;

        curBattleType = BattleType.LevelBattle;
        //PeopleData p1 = RoleManager.Instance._CurGameInfo.playerPeople;
        //PeopleData p2 = MapManager.Instance.FindLevelById(levelId).Enemy;
        List<PeopleData> theP2List = new List<PeopleData>();

        SingleLevelData level = MapManager.Instance.FindLevelById(levelId);
         for (int i=0;i< level.Enemy.Count; i++)
        {
            PeopleData p2 =level.Enemy[i];
            RefreshEnemyProperty(p2, DataTable.FindEnemySetting(p2.enemySettingId), levelSetting.EnemyLevel.ToInt32());
            theP2List.Add(p2);
        }
        //List<PeopleData> theP1List = new List<PeopleData> { p1 };
        List<PeopleData> theP1List = new List<PeopleData>();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1[i];
            if (onlyId <= 0)
                continue;
            if (onlyId ==RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
                theP1List.Add(RoleManager.Instance._CurGameInfo.playerPeople);
            else
                theP1List.Add(StudentManager.Instance.FindStudent(onlyId));

        }
        BattleManager.Instance.BattlePrepare(ref theP1List, ref theP2List,true,true);
        if (levelSetting.Leveltype.ToInt32() == (int)LevelType.BossBattle)
        {
            for(int i = 0; i < theP2List.Count; i++)
            {
                RoleManager.Instance.FullMP(theP2List[i]);

            }

        }
        BattleViewPrepare();

        GameSceneManager.Instance.GoToScene(SceneType.Battle);
    }
    /// <summary>
    /// 开始地图事件战斗
    /// </summary>
    public void StartMapEventBattle(SingleMapEventData singleMapEventData, List<PeopleData> team1,List<PeopleData> team2)
    {

        studentBattle = false;
        curBattleMapEvent = singleMapEventData;
        curBattleType = BattleType.MapEventBattle;
        List<PeopleData> theP1List = team1;
        List<PeopleData> theP2List = team2;
        BattleManager.Instance.BattlePrepare(ref theP1List, ref theP2List, true, true);
        BattleViewPrepare();

        GameSceneManager.Instance.GoToScene(SceneType.Battle);
    }

    /// <summary>
    /// 开始叛宗弟子战斗
    /// </summary>
    public void StartPanZongBattle(List<PeopleData> team1, List<PeopleData> team2)
    {

        studentBattle = false;
        curBattleType = BattleType.PanZongStudentBattle;
        List<PeopleData> theP1List = team1;
        List<PeopleData> theP2List = team2;
        BattleManager.Instance.BattlePrepare(ref theP1List, ref theP2List, true, true);
        BattleViewPrepare();

        GameSceneManager.Instance.GoToScene(SceneType.Battle);
    }

 
 

    public PeopleData[] FindBattlePeople(UInt64 onlyId1,UInt64 onlyId2)
    {
        PeopleData[] res= new PeopleData[2];
        PeopleData p1 = null;
        PeopleData p2 = null;
        for(int i = 0; i < p1List.Count; i++)
        {
            PeopleData theP = p1List[i];
            if (onlyId1 == theP.onlyId)
                p1 = theP;
            else if (onlyId2 ==theP.onlyId)
                p2 = theP;
        }
        for (int i = 0; i < p2List.Count; i++)
        {
            PeopleData theP = p2List[i];
            if (onlyId1 == theP.onlyId)
                p1 = theP;
            else if (onlyId2 == theP.onlyId)
                p2 = theP;
        }
        res[0] = p1;
        res[1] = p2;
        return res;
    }
    public PeopleData FindBattlePeople(UInt64 onlyId1)
    {
        for(int i = 0; i < p1List.Count; i++)
        {
            PeopleData p = p1List[i];
            if(onlyId1== p.onlyId)
            {
                return p;
            }
        }
        for (int i = 0; i < p2List.Count; i++)
        {
            PeopleData p = p2List[i];
            if (onlyId1 == p.onlyId)
            {
                return p;
            }
        }
        return null;
    }

    /// <summary>
    /// 放大招
    /// </summary>
    public void OnBigAttack(int index)
    {
        SinglePropertyData pro= RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, p1List[p1Index]);
        //能放大
        if (pro.num >= pro.limit)
        {
            Debug.Log("能放大");
            if(BattleManager.Instance.FindBuffTypeBuff(p1List[p1Index], BattleBuffType.FengMai) == null)
            {
                EventCenter.Broadcast(TheEventType.ReadyToBig,index);
            }
            else
            {
                 
                PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.已被封脉无法使用功法));
            }
        }
    }

    /// <summary>
    /// 移除一个助战弟子
    /// </summary>
    /// <param name="p"></param>
    public void RemoveZhuZhanStudent(PeopleData p)
    {

        BattleManager.Instance.curZhuZhanStudentList.Remove(p);
        Debug.Log(p.name + "移除助战");
        EventCenter.Broadcast(TheEventType.RemoveZhuZhan,p);
    }
    /// <summary>
    /// 回满血
    /// </summary>
    public void FullAllBattlePeopleHP()
    {
        for(int i = 0; i < p1List.Count; i++)
        {
            RoleManager.Instance.FullHp(p1List[i]);

        }
        for(int i = 0; i < p2List.Count; i++)
        {
            RoleManager.Instance.FullHp(p2List[i]);
        }
    }

    /// <summary>
    /// 找缓存池里的技能
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public SkillSetting FindSkillSetting(int skillId)
    {
        for(int i = 0; i < skillSettingHuanCun.Count; i++)
        {
            SkillSetting setting = skillSettingHuanCun[i];
            if (setting.Id.ToInt32() == skillId)
                return setting;
        }
        return null;
    }
    /// <summary>
    /// 找缓存池里的升级技能
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public List<SkillUpgradeSetting> FindSkillUpgradeListBySkillId(int skillId)
    {
        List<SkillUpgradeSetting> res = new List<SkillUpgradeSetting>();
        
        for (int i = 0; i < skillUpgradeSettingHuanCun.Count; i++)
        {
            SkillUpgradeSetting setting = skillUpgradeSettingHuanCun[i];
            if (setting.SkillId.ToInt32() == skillId)
            {
                res.Add(setting);

            }
        }
        return res;
    }
    public bool CheckIfLeftP(PeopleData p)
    {
        for(int i = 0; i < p1List.Count; i++)
        {
            if (p.onlyId == p1List[i].onlyId)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 增加buf
    /// </summary>
    /// <param name="data"></param>
    public bool AddBattleBuff(PeopleData p, BattleBuffSetting buffSetting, SingleSkillData skillData=null)
    {
        
        bool left = false;
        List<BattleBuff> handleList = null;
        if (CheckIfLeftP(p))
            left = true;
        else
            left = false;

        if (left)
        {
            handleList = buffList1;
            
        }
        else
        {
            handleList = buffList2;

        }
        BattleBuffType type =(BattleBuffType) buffSetting.BuffType.ToInt32();
        BattleBuff weiMiBuff = FindBuffTypeBuff(p, BattleBuffType.WeiMi);
        if (weiMiBuff != null &&string.IsNullOrWhiteSpace(buffSetting.Bad))
        {
            return false;
        }
        //是否有免疫负面buff
        BattleBuff huShenBuff = FindBuffTypeBuff(p, BattleBuffType.HuShen);
        if (huShenBuff != null &&  (buffSetting.Bad=="1"))
        {
            return false;
        }
        //是否有护罩buff
        BattleBuff huZhaoBuff = FindBuffTypeBuff(p, BattleBuffType.HuZhao);
        if (huZhaoBuff != null && (buffSetting.Bad == "1"))
        {
            return false;
        }
        bool haveSameBuff = false;
        bool haveSameIdBuff = false;
        BattleBuff theSameBuff = null; 
        BattleBuff theSameIdBuff = null;

        for (int i = handleList.Count-1; i >=0; i--)
        {
            BattleBuff existedbuff = handleList[i];
            ////有相同buff 刷新
            //if (buff.buffSetting.id.ToInt32() == buffId)
            //{
            //    buff.AddSameBuff();
            //    haveSameBuff = true;
            //    break;
            //}
            //同类buff 可能互相顶掉 比如冰盾和岩盾 比如两个不同量护罩一起加
            if(existedbuff.buffSetting.BuffType == buffSetting.BuffType)
            {
                theSameBuff = existedbuff;
                haveSameBuff = true;
                if (existedbuff.buffSetting.Id == buffSetting.Id)
                {
                    theSameIdBuff = existedbuff;
                    haveSameIdBuff = true;
                }
                  
                //if (!string.IsNullOrWhiteSpace(existedbuff.buffSetting.die))
                //{
                //    existDieValue = existedbuff.dieValue;
                //    existlayer = existedbuff.layer;
                //}
                break;
            }
        }
        bool successAdd = true;

        //抗性与否
        if (!string.IsNullOrWhiteSpace(buffSetting.Kang))
        {
            int kang = 0;
            int kangAdd = buffSetting.Kang.ToInt32();
            int buffKangAdd = 0;
            if (FindIdBuff(p, BattleBuffIdType.ChaoShi) != null)
            {
                buffKangAdd -=30;
            }
            BattleBuff huoHunBuff = FindIdBuff(p, BattleBuffIdType.HuoHun);
            if (huoHunBuff != null)
            {
                buffKangAdd += huoHunBuff.kangAdd;
            }
            kangAdd = (int)(kangAdd * (1 + buffKangAdd * 0.01f));
            if (kangAdd <= 0)
                kangAdd = 0;

            if (left)
            {
                kang = kangXing1;
                kangXing1 += kangAdd;
                if (kangXing1 >= 100)
                    kangXing1 = 100;
            }
            else
            {
                kang = kangXing2;
                kangXing2 += kangAdd;
                if (kangXing2 >= 100)
                    kangXing2 = 100;
            }
            int successRate = RandomManager.Next(0, 100);
            if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.DingShen
                && kang <20)
            {
                //必定成功
                successAdd = true;
            }
            else
            {
                //成功
                if (successRate >= kang)
                {
                    successAdd = true;
                }
                //失败
                else
                {
                    successAdd = false;
                    //如果抵抗掉了 抗性减少相应数值
                    if (left)
                    {
                        kangXing1 -=(int)(buffSetting.Kang.ToFloat()/2);
                        if (kangXing1 <= 0)
                            kangXing1 = 0;
                    }
                    else
                    {
                        kangXing2 -= (int)(buffSetting.Kang.ToFloat()/2);
                        if (kangXing2 <= 0)
                            kangXing2 = 0;
                    }
                }
            }
            //抗性显示数值
            EventCenter.Broadcast(TheEventType.RefreshKangXingNumShow, p);
        }

        if (successAdd)
        {
            bool xu = false;
            if (haveSameBuff)
            {
                int maxNum = 0;
                //不能叠加 
                if (string.IsNullOrWhiteSpace(theSameBuff.buffSetting.Die))
                {
                    //RemoveBuff(p, theSameBuff);
                    maxNum = 1;
                }
                //能叠加，最多叠多少
                else
                {
                    maxNum = theSameBuff.buffSetting.Die.ToInt32();
                }
                List<BattleBuff> sameTypeBuffList = FindTypeBuffList(p, (BattleBuffType)theSameBuff.buffSetting.BuffType.ToInt32());
                while (sameTypeBuffList.Count >= maxNum)
                {
                    RemoveBuff(p, sameTypeBuffList[0]);
                    sameTypeBuffList = FindTypeBuffList(p, (BattleBuffType)theSameBuff.buffSetting.BuffType.ToInt32());
                }
            }
            if (haveSameIdBuff)
            {
                int maxNum = 0;

                //不能自己叠加 
                if (string.IsNullOrWhiteSpace(theSameIdBuff.buffSetting.SelfDie))
                {
                    maxNum = 1;

                    //RemoveBuff(p, theSameIdBuff);
                    //theSameBuff.remainHuiHe = theSameBuff.existHuiHe;
                    //xu = true;
                }
                else
                {
                    maxNum = theSameIdBuff.buffSetting.Die.ToInt32();
                }
                List<BattleBuff> sameIdBuffList = FindIDBuffList(p, (BattleBuffIdType)theSameBuff.buffSetting.Id.ToInt32());
                while (sameIdBuffList.Count >= maxNum)
                {
                    RemoveBuff(p, sameIdBuffList[0]);
                    sameIdBuffList = FindIDBuffList(p, (BattleBuffIdType)theSameBuff.buffSetting.Id.ToInt32());
                }
            }
            //if (!xu)
            //{

            BattleBuff buff = new BattleBuff(buffSetting, p, skillData);

         
                handleList.Add(buff);
                //如果是元素盾
                if (buff.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.HuZhao
                    && !string.IsNullOrWhiteSpace(buff.buffSetting.Param4))
                {
                    if (left)
                        yuanSuList1.Clear();
                    else
                        yuanSuList2.Clear();
                    //元素清空
                    EventCenter.Broadcast(TheEventType.ClearYuanSu, left);
                    //清空负面buff
                    RemoveAllBadBuff(left);
                }
            //如果是丹毁
            if (type == BattleBuffType.ZiBao)
            {
                int ziBaoId = buffSetting.Param2.ToInt32();
                if (!skillSettingHuanCunIdList.Contains(ziBaoId))
                {
                    skillSettingHuanCunIdList.Add(ziBaoId);
                    SkillSetting ziBaoSetting = DataTable.FindSkillSetting(ziBaoId);
                    skillSettingHuanCun.Add(ziBaoSetting);
                    for (int k = 0; k < DataTable._skillUpgradeList.Count; k++)
                    {
                        SkillUpgradeSetting upgradesetting = DataTable._skillUpgradeList[k];
                        int upgradeId = upgradesetting.Id.ToInt32();
                        if (upgradesetting.SkillId.ToInt32() == ziBaoId
                            && !skillUpgradeSettingHuanCunIdList.Contains(upgradeId))
                        {
                            skillUpgradeSettingHuanCun.Add(upgradesetting);
                            skillUpgradeSettingHuanCunIdList.Add(upgradeId);
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(buffSetting.AddAddBuff))
            {
                BattleBuffSetting add = DataTable.FindBattleBuffSetting(buffSetting.AddAddBuff.ToInt32());
                AddBattleBuff(p, add);
                 
            }
                EventCenter.Broadcast(TheEventType.ShowBattleEffect, p, buffSetting.EffectName);

                EventCenter.Broadcast(TheEventType.AddBattleBuff, p, buff);
            //}
            return true;

        }
        else
        {
            EventCenter.Broadcast(TheEventType.BattleInfoTxtShow, p,"抵抗");
            return false;
        }

    }
    

    /// <summary>
    /// buff回合数--
    /// </summary>
    public void DeBattleBuffRemainHuiHe(PeopleData p)
    {
        bool left = false;
        List<BattleBuff> handleList = null;
        PeopleData enemy = null;
        if (CheckIfLeftP(p))
        {
            left = true;
            handleList = buffList1;
            enemy = p2List[p2Index];
        }
        else
        {
            left = false;
            handleList = buffList2;
            enemy = p1List[p1Index];
        }
        AttackResData zhuoShaoRes = null;
        BattleBuff zhuoShaoBuff = null;
        List<BattleBuff> hurtEnemyBuffList = new List<BattleBuff>();
        bool haveZhuoShao = false;
         bool noKangXingBuff = true;//没有能影响抗性的buff
        List<ReactionType> buffReactionTypeList = new List<ReactionType>();
        List<BattleBuff> reactionBuffList = new List<BattleBuff>();
        for (int i = 0; i < handleList.Count; i++)
        {
            BattleBuff buff = handleList[i];
            if (buff.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.ZhuoShao)
            {
                haveZhuoShao = true;
                //break;
                zhuoShaoBuff = buff;
            }
            if (buff.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.HurtEnemy)
            {
                hurtEnemyBuffList.Add(buff);
            }
            if (buff.reactionType !=ReactionType.None
                &&!buffReactionTypeList.Contains(buff.reactionType))
            {
                buffReactionTypeList.Add(buff.reactionType);
                reactionBuffList.Add(buff);
            }
            if (!string.IsNullOrWhiteSpace(buff.buffSetting.Kang))
                noKangXingBuff = false;
          
        }

        //灼烧
        if (haveZhuoShao)
        {
            zhuoShaoRes = CalcAttack(enemy, p, null, 0, zhuoShaoBuff);
            if (zhuoShaoRes != null)
            {
                EventCenter.Broadcast(TheEventType.BattleDeHpShow, zhuoShaoRes);
                EventCenter.Broadcast(TheEventType.BattleBeHit, zhuoShaoRes);
                EventCenter.Broadcast(TheEventType.ShowBuffFunction, p, "灼烧");

                if (zhuoShaoRes.fanShangResData != null)
                {

                    EventCenter.Broadcast(TheEventType.BattleDeHpShow, zhuoShaoRes.fanShangResData);
                    EventCenter.Broadcast(TheEventType.BattleBeHit, zhuoShaoRes.fanShangResData);
                }
                BattleManager.Instance.JudgeIfPeopleDead();
            }


        }
        if (hurtEnemyBuffList.Count > 0)
        {
            for (int i = 0; i < hurtEnemyBuffList.Count; i++)
            {
                AttackResData hurtRes = CalcAttack(p, enemy, null, 0, hurtEnemyBuffList[i]);
                if (hurtRes != null)
                {
                    EventCenter.Broadcast(TheEventType.BattleDeHpShow, hurtRes);
                    EventCenter.Broadcast(TheEventType.BattleBeHit, hurtRes);

                    if (hurtRes.fanShangResData != null)
                    {

                        EventCenter.Broadcast(TheEventType.BattleDeHpShow, hurtRes.fanShangResData);
                        EventCenter.Broadcast(TheEventType.BattleBeHit, hurtRes.fanShangResData);
                    }
                    BattleManager.Instance.JudgeIfPeopleDead();
                }
             
            }

        }
        for (int i = 0; i < buffReactionTypeList.Count; i++)
        {
            AttackResData reactionRes = null;

            reactionRes = CalcAttack(enemy, p, null, 0, reactionBuffList[i], buffReactionTypeList[i]);
            if (reactionRes != null)
            {
                EventCenter.Broadcast(TheEventType.ShowYuanSuReactionNum, reactionRes);
                EventCenter.Broadcast(TheEventType.BattleBeHit, reactionRes);

                if (reactionRes.fanShangResData != null)
                {
                    EventCenter.Broadcast(TheEventType.BattleDeHpShow, reactionRes.fanShangResData);
                    EventCenter.Broadcast(TheEventType.BattleBeHit, reactionRes.fanShangResData);
                }
                BattleManager.Instance.JudgeIfPeopleDead();
            }
  
        }
        //没有抗性buff
        if (noKangXingBuff)
        {
            if (left)
            {
                kangXing1 -= 10;
                if (kangXing1 <= 0)
                    kangXing1 = 0;
            }
            else
            {
                kangXing2 -= 10;
                if (kangXing2 <= 0)
                    kangXing2 = 0;
            }

            //抗性显示数值
            EventCenter.Broadcast(TheEventType.RefreshKangXingNumShow, p);
        }

        for (int i = handleList.Count-1; i >=0; i--)
        {
            BattleBuff buff = handleList[i];
        
            
            buff.remainHuiHe--;
            buff.existHuiHe++;
            EventCenter.Broadcast(TheEventType.DeBattleBuffHuiHe, p, buff);
            if (buff.remainHuiHe <= 0)
            {
                RemoveBuff(p,buff);
                //如果是火魂buff 
                if (buff.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.HuoHun)
                {
                    ExecuteHuoYin(p,enemy);
                }
            }
            //天音存在回合大于5 则给自己回血
            if (buff.buffSetting.Id.ToInt32() == (int)BattleBuffIdType.TianYinHuDun)
            {
                List<int> huiXue =CommonUtil.SplitCfgOneDepth(buff.buffSetting.Param3);
                if (buff.existHuiHe >= huiXue[0]+1)
                {
                    int num =Mathf.RoundToInt(huiXue[1] * 0.01f * RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p).limit);
                    if (FindBuffTypeBuff(p, BattleBuffType.JinLiao) != null)
                    {
                        num = 0;
                    }
                    RoleManager.Instance.AddBattleProperty(PropertyIdType.Hp, num, p);
                    EventCenter.Broadcast(TheEventType.AddBattleHp, p, num);
                    RemoveBuff(p, buff);
                }
            }
            //愈合buff给自己回血
            if (buff.buffSetting.BuffType.ToInt32() == (int)BattleBuffType.YuHe)
            {
                if (FindBuffTypeBuff(p, BattleBuffType.JinLiao) == null)
                {
                    int num = Mathf.RoundToInt(buff.buffSetting.Param.ToInt32() * 0.01f * RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p).limit);
                    RoleManager.Instance.AddBattleProperty(PropertyIdType.Hp, num, p);
                    EventCenter.Broadcast(TheEventType.AddBattleHp, p, num);
                }
           
            }
        }

        EventCenter.Broadcast(TheEventType.RefreshBatlteBuff, p);


    }


    

    /// <summary>
    /// 随机移除一个正面buff
    /// </summary>
    public void RdmQuSanAGoodBuff(bool left)
    {
        List<BattleBuff> handleList = null;
        PeopleData handlePeople = null;
        if (left)
        {
            handleList = buffList1;
            handlePeople = p1List[p1Index];
        }
        else
        {
            handleList = buffList2;
            handlePeople = p2List[p2Index];

        }
        for(int i = 0; i < handleList.Count; i++)
        {
            BattleBuff buff = handleList[i];
            if (string.IsNullOrWhiteSpace(buff.buffSetting.Bad)&&buff.buffSetting.CanQu=="1")
            {
                RemoveBuff(handlePeople, buff);
                break;
            }
        }
       
    }
    //移除所有负面buff
    public void RemoveAllBadBuff(bool left)
    {

        List<BattleBuff> handleList = null;
        PeopleData handlePeople = null;

        if (left)
        {
            handleList = buffList1;
            handlePeople = p1List[p1Index];
        }
        else
        {
            handleList = buffList2;
            handlePeople = p2List[p2Index];

        }
        int curQuNum = 0;
        for (int i = handleList.Count - 1; i >= 0; i--)
        {
            BattleBuff buff = handleList[i];
            if (buff.buffSetting.Bad == "1" && buff.buffSetting.CanQu == "1")
            {
                RemoveBuff(handlePeople, buff);
 
            }
        
        }
    }
    /// <summary>
    /// 随机移除n个负面buff
    /// </summary>
    public void RdmQuSanABadBuff(int num, bool left)
    {
        List<BattleBuff> handleList = null;
        PeopleData handlePeople = null;
        
        if (left)
        {
            handleList = buffList1;
            handlePeople = p1List[p1Index];
        }
        else
        {
            handleList = buffList2;
            handlePeople = p2List[p2Index];

        }
        int curQuNum = 0;
        for (int i = handleList.Count-1; i >=0; i--)
        {
            BattleBuff buff = handleList[i];
            if (buff.buffSetting.Bad == "1" && buff.buffSetting.CanQu == "1")
            {
                RemoveBuff(handlePeople, buff);
                curQuNum++;
                
            }
            if (curQuNum >= num)
                break;
        }

    }
    /// <summary>
    /// 移除所有buf
    /// </summary>
    /// <param name="left"></param>
    public void RemoveAllBuff(PeopleData p)
    {
        List<BattleBuff> handleList = null;
        PeopleData handlePeople = null;
        if (CheckIfLeftP(p))
        {
            handleList = buffList1;
            handlePeople = p1List[p1Index];
        }
        else
        {
            handleList = buffList2;
            handlePeople = p2List[p2Index];

        }
        for (int i = handleList.Count - 1; i >= 0; i--)
        {
            BattleBuff theBuff = handleList[i];
            handleList.Remove(theBuff);
            EventCenter.Broadcast(TheEventType.RemoveBattleBuff, handlePeople, theBuff);
        }

    }
    /// <summary>
    /// 移除所有该类型 buf
    /// </summary>
    /// <param name="left"></param>
    public void RemoveAllTypeBuff(PeopleData p, BattleBuffType battleBuffType)
    {

        List<BattleBuff> handleList = null;
        PeopleData handlePeople = null;
        if (CheckIfLeftP(p))
        {
            handleList = buffList1;
            handlePeople = p1List[p1Index];
        }
        else
        {
            handleList = buffList2;
            handlePeople = p2List[p2Index];

        }
        for (int i = handleList.Count - 1; i >= 0; i--)
        {
            BattleBuff theBuff = handleList[i];
            if (theBuff.buffSetting.BuffType.ToInt32() == (int)battleBuffType)
            {
                handleList.Remove(theBuff);
                EventCenter.Broadcast(TheEventType.RemoveBattleBuff, handlePeople, theBuff);
            }

        }

    }
    /// <summary>
    /// 移除所有该id buf
    /// </summary>
    /// <param name="left"></param>
    public void RemoveAllIDBuff(PeopleData p,BattleBuffIdType battleBuffIdType)
    {
        
  
        List<BattleBuff> handleList = null;
        PeopleData handlePeople = null;
        if (CheckIfLeftP(p))
        {
            handleList = buffList1;
            handlePeople = p1List[p1Index];
        }
        else
        {
            handleList = buffList2;
            handlePeople = p2List[p2Index];

        }
        for (int i = handleList.Count - 1; i >= 0; i--)
        {
            BattleBuff theBuff = handleList[i];
            if (theBuff.buffSetting.Id.ToInt32() == (int)battleBuffIdType)
            {
                handleList.Remove(theBuff);
                EventCenter.Broadcast(TheEventType.RemoveBattleBuff, handlePeople, theBuff);
            }

        }

    }

    /// <summary>
    /// 移除所有buf
    /// </summary>
    /// <param name="left"></param>
    public void RemoveAllBuff(bool left)
    {
        List<BattleBuff> handleList = null;
        PeopleData handlePeople = null;
        if (left)
        {
            handleList = buffList1;
            handlePeople = p1List[p1Index];
        }
        else
        {
            handleList = buffList2;
            handlePeople = p2List[p2Index];

        }
        for (int i = handleList.Count - 1; i >= 0; i--)
        {
            handleList.Remove(handleList[i]);
            EventCenter.Broadcast(TheEventType.RemoveBattleBuff, handlePeople, handleList[i]);
        }

    }

    /// <summary>
    /// 移除buf
    /// </summary>
    /// <param name="battleBuff"></param>
    public void RemoveBuff(PeopleData p,BattleBuff battleBuff)
    {

        List<BattleBuff> handleList = null;

        if (CheckIfLeftP(p))
        {
            handleList = buffList1;
        }
        else
        {
            handleList = buffList2;

        }
        handleList.Remove(battleBuff);
        if (!string.IsNullOrWhiteSpace(battleBuff.buffSetting.RemoveRemoveBuff))
        {
            BattleBuff remove = FindIdBuff(p,(BattleBuffIdType) battleBuff.buffSetting.RemoveRemoveBuff.ToInt32());
            if (remove != null)
            {
                RemoveBuff(p, remove);
            }
           
            
        }
        if (!string.IsNullOrWhiteSpace(battleBuff.buffSetting.RemoveAddBuff))
        {
            AddBattleBuff(p, DataTable.FindBattleBuffSetting(battleBuff.buffSetting.RemoveAddBuff.ToInt32()));


        }
        EventCenter.Broadcast(TheEventType.RemoveBattleBuff, p, battleBuff);

    }
    /// <summary>
    /// 找我有没有该buff类型buff
    /// </summary>
    /// <param name="type"></param>
    public List< BattleBuff> FindBuffTypeBuffList(PeopleData p, BattleBuffType type)
    {
        List<BattleBuff> res = new List<BattleBuff>();
        List<BattleBuff> handleList = new List<BattleBuff>();
        //左边找
        if (CheckIfLeftP(p))
        {
            handleList = buffList1;
        }
        else
        {
            handleList = buffList2;

        }
        for (int i = 0; i < handleList.Count; i++)
        {
            BattleBuff buff = handleList[i];
            if (buff.buffSetting.BuffType.ToInt32() == (int)type)
            {
                res.Add(buff);
            }
        }
        return res;
    }
    /// <summary>
    /// 找我有没有该buff类型buff
    /// </summary>
    /// <param name="type"></param>
    public BattleBuff FindBuffTypeBuff(PeopleData p, BattleBuffType type)
    {
        List<BattleBuff> handleList = new List<BattleBuff>();
        //左边找
        if (CheckIfLeftP(p))
        {
            handleList = buffList1;
        }
        else
        {
            handleList = buffList2;

        }
        for (int i = 0; i < handleList.Count; i++)
        {
            BattleBuff buff = handleList[i];
            if (buff.buffSetting.BuffType.ToInt32() == (int)type)
            {
                return buff;
            }
        }
        return null;
    }
    /// <summary>
    /// 找我有没有该类型buff
    /// </summary>
    /// <param name="type"></param>
    public BattleBuff FindIdBuff(PeopleData p, BattleBuffIdType idType)
    {
        List<BattleBuff> handleList = new List<BattleBuff>();
        //左边找
        if (CheckIfLeftP(p))
        {
            handleList = buffList1;
        }
        else
        {
            handleList = buffList2;

        }
        for (int i = 0; i < handleList.Count; i++)
        {
            BattleBuff buff = handleList[i];
            if (buff.buffSetting.Id.ToInt32() == (int)idType)
            {
                return buff;
            }
        }
        return null;
    }
    /// <summary>
    /// 找我有没有该类型buff
    /// </summary>
    /// <param name="type"></param>
    public List<BattleBuff> FindTypeBuffList(PeopleData p, BattleBuffType buffType)
    {
        List<BattleBuff> res = new List<BattleBuff>();
        List<BattleBuff> handleList = new List<BattleBuff>();
        //左边找
        if (CheckIfLeftP(p))
        {
            handleList = buffList1;
        }
        else
        {
            handleList = buffList2;

        }
        for (int i = 0; i < handleList.Count; i++)
        {
            BattleBuff buff = handleList[i];
            if (buff.buffSetting.BuffType.ToInt32() == (int)buffType)
            {
                res.Add(buff);
            }
        }
        return res;
    }
    /// <summary>
    /// 找我有没有该id buff
    /// </summary>
    /// <param name="type"></param>
    public List<BattleBuff> FindIDBuffList(PeopleData p, BattleBuffIdType buffIdType)
    {
        List<BattleBuff> res = new List<BattleBuff>();
        List<BattleBuff> handleList = new List<BattleBuff>();
        //左边找
        if (CheckIfLeftP(p))
        {
            handleList = buffList1;
        }
        else
        {
            handleList = buffList2;

        }
        for (int i = 0; i < handleList.Count; i++)
        {
            BattleBuff buff = handleList[i];
            if (buff.buffSetting.Id.ToInt32() == (int)buffIdType)
            {
                res.Add(buff);
            }
        }
        return res;
    }

    /// <summary>
    /// 增加MP 吃能量
    /// </summary>
    public void AddMP(PeopleData p,float energyNum)
    {
        int otherAddEnergy = 0;
        float realAddEnergyNum = 0;
        List<PeopleData> handlePList = null;
        ///全队吃能量
        if (p.isMyTeam)
        {
            handlePList = p1List;

          
            //if (p1List.Count)
        }
        else
        {
            handlePList = p2List;

            //realAddEnergyNum = (energyNum * (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MPSpeed, p).Num * 0.01f + 1));

            //RoleManager.Instance.AddBattleProperty(PropertyIdType.MpNum, (int)realAddEnergyNum, p);

        }

        float rate = 1;
        if (handlePList.Count <= 2)
            rate = 0.7f;
        else if (handlePList.Count <= 3)
            rate = 0.6f;
        else if (handlePList.Count <= 4)
            rate = 0.5f;
        else
            rate = 0;

        float luanShenRate = 0;
        //乱神buff
        List<BattleBuff> luanShenBuff = FindBuffTypeBuffList(p, BattleBuffType.LuanShen);
        if (luanShenBuff.Count>0)
        {
            for(int i = luanShenBuff.Count - 1; i >= 0; i--)
            {
                luanShenRate += luanShenBuff[i].commonParam.ToFloat() * 0.01f;
            }
        }
        //集中buff
        List<BattleBuff> jiZhongBuff = FindBuffTypeBuffList(p, BattleBuffType.JiZhong);
        if (jiZhongBuff.Count>0)
        {
            for(int i = jiZhongBuff.Count - 1; i >= 0; i--)
            {
                luanShenRate += jiZhongBuff[i].commonParam.ToFloat() * 0.01f;
            }
        }
        if (luanShenRate < -1)
            luanShenRate = -1;
        for (int i = 0; i < handlePList.Count; i++)
        {
            PeopleData theP = handlePList[i];
            if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, theP).num <= 0)
                continue;
            if (theP.onlyId == p.onlyId)
            {
                realAddEnergyNum = (energyNum * (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MPSpeed, p).num * 0.01f + 1));

            }
            else
            {
                realAddEnergyNum = (energyNum * rate * (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MPSpeed, p).num * 0.01f + 1));

            }
            realAddEnergyNum *=(1+ luanShenRate);
            RoleManager.Instance.AddBattleProperty(PropertyIdType.MpNum, (int)realAddEnergyNum, theP);

        }

    }

    /// <summary>
    /// 增加HP 全队分摊
    /// </summary>
    public void AddHP(PeopleData p, float num)
    {
        int otherAddEnergy = 0;

        ///全队吃血
        //if (p.isMyTeam)
        //{
        List<PeopleData> handleList = null;
        if (CheckIfLeftP(p))
        {
            handleList = p1List;
        }
        else
        {
            handleList = p2List;
        }
        float rate = 1;
        if (handleList.Count <= 2)
            rate = 0.7f;
        else if (handleList.Count <= 3)
            rate = 0.6f;
        else if (handleList.Count <= 4)
            rate = 0.5f;
        else
            rate = 0;
        for (int i = 0; i < handleList.Count; i++)
        {
            PeopleData theP = handleList[i];
            if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, theP).num <= 0)
                continue;
            if (theP.onlyId == p.onlyId)
            {
                //num = (num * (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MPSpeed, p).Num * 0.01f + 1));
            }
            else
            {
                num = (num * rate);

            }
            if (FindBuffTypeBuff(p, BattleBuffType.JinLiao) != null)
            {
                num = 0;
            }
            RoleManager.Instance.AddBattleProperty(PropertyIdType.Hp, (int)num, theP);
            EventCenter.Broadcast(TheEventType.AddBattleHp, theP, (int)num);
        }

        //if (p1List.Count)
        // }
        //else
        //{

        //RoleManager.Instance.AddBattleProperty(PropertyIdType.MpNum, (int)num, p);

        //}
    }
    /// <summary>
    /// 选择某个技能
    /// </summary>
    /// <returns></returns>
    public int EnemyAI(PeopleData p)
    {
        //混乱苏梦岚
        if (p.enemySettingId == (int)EnemyIdType.HunLuanDeSuMengLan)
        {
            SinglePropertyData hpPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p);
            int skillIndex = -1;
            if (hpPro.num / (float)hpPro.limit > 0.4f)
            {
                skillIndex = 1;//天华宝镜
            }
            else
            {
                if (SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[2],
                         p.allSkillData).cd > 0)
                    skillIndex = 3;//噬神守卫buff放过了 直接放攻击
                else
                    skillIndex = 2;
            }
            return skillIndex;
        }else if (p.enemySettingId == (int)EnemyIdType.DaFeng)
        {
            SingleSkillData skill1 = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[1],p.allSkillData);
            SingleSkillData skill2 = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[2], p.allSkillData);
            if (skill1.cd <= 0 && skill2.cd <= 0)
                return 1;
            else
            {
                if (skill1.cd <= 0 && skill2.cd > 0)
                    return 1;
                else
                    return 2;
            }
        }
        //默认
        else
        {
            if (p.allSkillData.equippedSkillIdList.Count >= 2)
            {
                List<int> candidateIndexList = new List<int>();
                for (int i = 1; i < p.allSkillData.equippedSkillIdList.Count; i++)
                {
                    if (SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[i],
                            p.allSkillData).cd <= 0)
                        candidateIndexList.Add(i);
                }
                if (candidateIndexList.Count > 0)
                {
                    int index = RandomManager.Next(0, candidateIndexList.Count);
                    return candidateIndexList[index];
                }
                else
                {
                    Debug.Log(p.name + "所有技能都在cd，放普攻");
                    return 0;

                }

            }

        }

        return -1;
    }

    /// <summary>
    /// 减技能cd
    /// </summary>
    public void DeSkillCD(PeopleData p)
    {
        List<PeopleData> handlePList = null;
        if (CheckIfLeftP(p))
        {
            handlePList = p1List;
        }
        else if (p.onlyId == p2List[p2Index].onlyId)
        {
            handlePList = p2List;

        }

        for (int i = 0; i < handlePList.Count; i++)
        {
            PeopleData theP = handlePList[i];
            //其它技能cd--
            for (int j = 0; j < theP.allSkillData.equippedSkillIdList.Count; j++)
            {
                SingleSkillData data = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(theP.allSkillData.equippedSkillIdList[j],
                   theP.allSkillData);
                if (data.cd > 0)
                    data.cd--;
            }
        }
    }

    #region 元素相关

    /// <summary>
    /// 破元素盾
    /// </summary>
    public int PoYuanSuDun(YuanSuType attackYuanSuType, BattleBuff battleBuff)
    {
        int res = 0;
        float rate = 0;
        //元素盾
        if (!string.IsNullOrWhiteSpace(battleBuff.buffSetting.Param4))
        {
            //克制情况，一次攻击掉0.18
            YuanSuType yuanSuType = (YuanSuType)battleBuff.buffSetting.Param4.ToInt32();
            if (yuanSuType == YuanSuType.Water)
            {
                if (attackYuanSuType == YuanSuType.Fire)
                    rate = 0.18f;
                else if (attackYuanSuType == YuanSuType.Ice)
                    rate = 0.04f;
                else if (attackYuanSuType == YuanSuType.Storm)
                    rate = 0.02f;
            }
            else if (yuanSuType == YuanSuType.Fire)
            {
                if (attackYuanSuType == YuanSuType.Water)
                    rate = 0.18f;
                else if (attackYuanSuType == YuanSuType.Ice)
                    rate = 0.08f;
                else if (attackYuanSuType == YuanSuType.Storm)
                    rate = 0.03f;  
            }
            else if (yuanSuType == YuanSuType.Storm)
            {
                if (attackYuanSuType == YuanSuType.Water)
                    rate = 0.07f;
                else if (attackYuanSuType == YuanSuType.Ice)
                    rate = 0.07f;
                else if (attackYuanSuType == YuanSuType.Fire)
                    rate = 0.07f;
            }
            else if (yuanSuType == YuanSuType.Dark)
            {
                if (attackYuanSuType == YuanSuType.Light)
                    rate = 0.18f;
            }
            else if (yuanSuType == YuanSuType.Light)
            {
                if (attackYuanSuType == YuanSuType.Dark)
                    rate = 0.18f;
            }
            res = (int)(battleBuff.totalHuZhaoNum * rate);
        }
      
        return res;
    }

    /// <summary>
    /// 通过元素得到普攻技能
    /// </summary>
    /// <param name="yuanSuType"></param>
    /// <returns></returns>
    public SkillIdType PuGongIdByYuanSu(YuanSuType yuanSuType)
    {
        SkillIdType skillIdType = SkillIdType.None;
        switch (yuanSuType)
        {
            case YuanSuType.Water:
                skillIdType = SkillIdType.ShuiPuGong;
                break;
            case YuanSuType.Fire:
                skillIdType = SkillIdType.HuoPuGong;
                break;
            case YuanSuType.Storm:
                skillIdType = SkillIdType.LeiPuGong;
                break;
            case YuanSuType.Ice:
                skillIdType = SkillIdType.BingPuGong;
                break;
            case YuanSuType.Light:
                skillIdType = SkillIdType.LingDan;
                break;
            case YuanSuType.Dark:
                skillIdType = SkillIdType.DarkPuGong;
                break;
        }
        return skillIdType;
    }

    /// <summary>
    /// buff增加的抗性
    /// </summary>
    public float KangAddNumByBuff(BattleBuffType type)
    {
        float res = 0;
        //麻痹
        if (type == BattleBuffType.MaBi)
        {
            res = 20;
        }
        //减速
        else if (type == BattleBuffType.JianSu)
        {
            res = 5;
        }
        //定身
        else if (type == BattleBuffType.DingShen)
        {
            res = 40;
        }
        return res;
    }
    #endregion

    #region 携战相关

    /// <summary>
    /// 助战
    /// </summary>
    /// <param name="p"></param>
    public void OnZhuZhan(List<PeopleData> pList)
    {
        if (leftZhuZhanIng || rightZhuZhanIng)
            return;

        curZhuZhanPList.Clear();
        PeopleData curAtSceneP = null;
        for (int i = 0; i < pList.Count; i++)
        {
            curZhuZhanPList.Add(pList[i]);
        }
        if (CheckIfLeftP(pList[0]))
        {
            curAtSceneP = p1List[p1Index];
            if (!leftZhuZhanIng)
            {
                leftZhuZhanIng = true;

            }
            else
            {
                return;
            }
        }
        else
        {
            curAtSceneP = p2List[p2Index];

            if (!rightZhuZhanIng)
            {
                rightZhuZhanIng = true;

            }
            else
            {
                return;
            }
        }
        bool canXieZhan = true;//出bug才是false
        //助战cd变化
        foreach (KeyValuePair<ulong, List<int>> kv in curAtSceneP.xieZhanCDDic)
        {
            PeopleData theP = StudentManager.Instance.FindStudent(kv.Key);
            if (theP != null && theP.xieZhanCDDic.ContainsKey(curAtSceneP.onlyId))
            {
                kv.Value[0] = kv.Value[1];
                theP.xieZhanCDDic[curAtSceneP.onlyId][0] = theP.xieZhanCDDic[curAtSceneP.onlyId][1];
            }
            else
            {
                //canXieZhan = false;
                curZhuZhanPList.Remove(theP);
                //break;
            }
          
        }
        if (curZhuZhanPList.Count>0)
        {
            AddLogicPause();
            StartSingleZhuZhan(curZhuZhanPList[0]);
        }

    }

    /// <summary>
    /// 开始单个助战
    /// </summary>
    void StartSingleZhuZhan(PeopleData p)
    {
        EventCenter.Broadcast(TheEventType.ZhuZhan, p);
    }

    /// <summary>
    /// 助战结束
    /// </summary>
    /// <param name="p"></param>
    public void OnFinishZhuZhan(PeopleData p)
    {
        PeopleData curAtSceneP = null;
        curZhuZhanPList.Remove(p);
        if (curZhuZhanPList.Count > 0)
        {
            StartSingleZhuZhan(curZhuZhanPList[0]);
        }
        else
        {

            if (CheckIfLeftP(p))
            {
                curAtSceneP = p1List[p1Index];
                if (leftZhuZhanIng)
                {
                    leftZhuZhanIng = false;

                }
                else
                {
                    return;
                }
            }
            else
            {
                curAtSceneP = p2List[p2Index];

                if (rightZhuZhanIng)
                {
                    rightZhuZhanIng = false;

                }
                else
                {
                    return;
                }

            }
            RemoveLogicPause();
            EventCenter.Broadcast(TheEventType.RemoveZhuZhan, p);
            //判断对方是否死了
            //buff时间--
            BattleManager.Instance.DeBattleBuffRemainHuiHe(curAtSceneP);
            //携战cd--
            BattleManager.Instance.DeXieZhanRemainHuiHe(curAtSceneP);
            BattleManager.Instance.DeSpecialRemainHuiHe(curAtSceneP);

            //是否死了
            BattleManager.Instance.JudgeIfPeopleDead();

        }

    }

    /// <summary>
    /// 是否携战
    /// </summary>
    /// <returns></returns>
    public bool CheckIfXieZhan(PeopleData p)
    {
        
        List<PeopleData> xieZhanList = new List<PeopleData>();
        List<PeopleData> handleList = null;
        if (CheckIfLeftP(p))
        {
            handleList = p1List;
        }
        else
        {
            handleList = p2List;
        }
        for(int i = 0; i < handleList.Count; i++)
        {
            PeopleData theP = handleList[i];
            //thep可以出战
            if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp,theP).num>0
                &&p.xieZhanCDDic.ContainsKey(theP.onlyId)
                &&p.xieZhanCDDic[theP.onlyId][0] == 0)
            {
                xieZhanList.Add(theP);
            }
        }
        if (xieZhanList.Count > 0)
        {
            OnZhuZhan(xieZhanList);
            return true;
        }
        return false;
        //return xieZhanList;
    }

    /// <summary>
    /// 携战剩余回合-
    /// </summary>
    public void DeXieZhanRemainHuiHe(PeopleData p)
    {
        List<PeopleData> handleList = null;

        if (CheckIfLeftP(p))
        {
            handleList = p1List;
        }
        else
        {
            handleList = p2List;
        }
        for (int i = 0; i < handleList.Count; i++)
        {
            PeopleData theP = handleList[i];
            foreach(KeyValuePair<ulong, List<int>> kv in theP.xieZhanCDDic)
            {
                kv.Value[0]--;
                if (kv.Value[0] < 0)
                    kv.Value[0] = 0;
            }
        }
    }
    /// <summary>
    /// 特殊回合（如飞云回合-
    /// </summary>
    public void DeSpecialRemainHuiHe(PeopleData p)
    {
        List<PeopleData> handleList = null;

        if (CheckIfLeftP(p))
        {
            handleList = p1List;
        }
        else
        {
            handleList = p2List;
        }
        for (int i = 0; i < handleList.Count; i++)
        {
            PeopleData theP = handleList[i];
            if (theP.feiYunCD > 0)
                theP.feiYunCD--;
        }
    }
    /// <summary>
    /// 携战的比例
    /// </summary>
    public float XieZhanRate(PeopleData xieZhanP)
    {
        float res = 0;
        PeopleData curAtSceneP = null;
        if (CheckIfLeftP(xieZhanP))
        {
            curAtSceneP = p1List[p1Index];
        }
        else
        {
            curAtSceneP = p2List[p2Index];
        }
        HaoGanLevelType haoGanLevelType = StudentManager.Instance.GetStudentHaoGanLevelType(curAtSceneP, xieZhanP);
        int index = curAtSceneP.socializationData.knowPeopleList.IndexOf(xieZhanP.onlyId);
        int haoGanDu = curAtSceneP.socializationData.haoGanDu[index];
        //小好感
        if (haoGanLevelType==HaoGanLevelType.Good)
        {
            res = ConstantVal.littleHaoGanXieZhanAtkRate;
        }
        //中好感
        else if (haoGanLevelType==HaoGanLevelType.Great)
        {
            res = ConstantVal.middleHaoGanXieZhanAtkRate;

        }
        //大好感
        else if (haoGanLevelType==HaoGanLevelType.VeryGreat)
        {
            res = ConstantVal.bigHaoGanXieZhanAtkRate;


        }
        //满好感
        else if (haoGanLevelType==HaoGanLevelType.Daolv
            ||haoGanLevelType==HaoGanLevelType.DaoYou)
        {
            res = ConstantVal.fullHaoGanXieZhanAtkRate;


        }
        return res;
    }
    #endregion

  
 


    /// <summary>
    /// 扫荡
    /// </summary>
    public void SaoDang(BattleType battleType,string param)
    {
        if (!GameTimeManager.Instance.connectedToFuWuQiTime)
        {
            PanelManager.Instance.OpenFloatWindow("必须在联网环境下进行");
            return;
        }

        bool success = false;
        ///讨伐
        if (battleType == BattleType.MiJingGuardBattle)
        {
            MiJingManager.Instance.curBattleMijingLevelId = param.ToInt32();
            MiJingGuardBattleDataResult(true);
            success = true;
        }
        else if (battleType == BattleType.LevelBattle)
        {
            SingleLevelData canSaoDangLevel = MapManager.Instance.FindCanSaoDangLieXiLevel();
            if (canSaoDangLevel != null)
            {
                LevelSetting setting = DataTable.FindLevelSetting(canSaoDangLevel.LevelId);
                if (setting.Leveltype.ToInt32() == (int)LevelType.ZhongZhuanZhan)
                {
                    roleAward = null;
                   MapManager.Instance.ZhongZhuanZhanRest(canSaoDangLevel.LevelId);
                       success = true;

                }
                else
                {
                    int tiliConsume = ConstantVal.levelBattleTiliConsume;
                    if (!RoleManager.Instance.CheckIfPropertyEnough((int)PropertyIdType.Tili, tiliConsume))
                    {
                        PanelManager.Instance.OpenPanel<TiliRevivePanel>(PanelManager.Instance.trans_layer2);
                        success = false;
                    }else
                    {

                        success = true;
                        MapManager.Instance.curChoosedLevelId = canSaoDangLevel.LevelId;
                        LevelBattleDataResult(true, true);
                    }

                }
     
            }
     
        }
        if (success)
        {
            if (roleAward != null&&roleAward.Count>0)
            {
                List<int> settingIdList = new List<int>();
                List<ulong> numList = new List<ulong>();
                for (int i = 0; i < roleAward.Count; i++)
                {
                    settingIdList.Add(roleAward[i].settingId);
                    numList.Add(roleAward[i].count);
                }

                ItemManager.Instance.GetItemWithAwardPanel(settingIdList, numList);
            }

            EventCenter.Broadcast(TheEventType.OnSaoDang, battleType, param);
        }
       

    }
}

/// <summary>
/// 战斗结果数据
/// </summary>
public class AttackResData
{
    public PeopleData deHpPeople;//掉血的人
    public int showDeHP;//显示掉多少血 负数 因为护罩挡住 也要显示掉血
    public int deHp;//掉多少血 负数
    public bool ifCrit;//是否暴击
    public bool someoneDead;//有人死了
    public int damageIndex;
    public SingleSkillData skill;
    public AttackResData fanShangResData;//反伤
    public ReactionType reactionType;//元素反应
    public AttackResData(int deHp,bool ifCrit,bool someoneDead, PeopleData deHpPeople,SingleSkillData skill, int damageIndex)
    {
        this.deHp = deHp;
        this.ifCrit = ifCrit;
        this.someoneDead = someoneDead;
        this.deHpPeople = deHpPeople;
        this.skill = skill;
        this.damageIndex = damageIndex;
    }
}


/// <summary>
/// 战斗类型
/// </summary>
public enum BattleType 
{ 
    None=0,
    Match=1,//擂台
    ZhaoChaBattle=2,//找茬
    LevelBattle=3,//关卡战斗
    OutsideBattle=4,//支线战斗
    QieCuoBattle=5,//切磋战斗
    MiJingGuardBattle=6,//秘境守卫战斗
    NPCKillBattle=7,//npc生死战
    TouZiFirstBattle = 8,//头子第一次战斗
    DiShuFirstBattle=9,//帝姝第一次战斗
    LiMaoZhangMenBattle=10,//狸猫掌门战斗
    FixedLevelBattle=11,//固定主线战斗
    MapEventBattle=12,//地图事件战斗
    PanZongStudentBattle=13,//叛宗弟子战斗
}

/// <summary>
/// 经脉id
/// </summary>
public enum JingMaiIDType
{
    Up,//最上 爆发
    LeftShoulder,//左肩 集气
    RightShoulder,//右肩 驱散
    Waist,//腰 增益
    LeftLeg,//左腿 攻速
    RightLeg,//右腿 护盾
    LeftFoot,//左脚 定身
    RightFoot,//右脚 削弱
    End,
}
/// <summary>
/// 技能id
/// </summary>
public enum SkillIdType
{
    None=0,

    GuangDan=211010,//光弹

    JiuTianXuanLeiJue=141001,//九天玄雷诀
    LianTaiShouShenJue=660001,//莲台守身诀
    ShenNianGongJi=770001,//神念攻击
    MiHuanWuZhang=110002,//迷幻雾瘴
    TaiShangJingShenJue=330001,//太上净神诀
    ZhiHuanGuangShu=110003,//致幻光束水影分光
    GuiTianYunZhang=110004,//鬼天陨掌
    ShiHaiBaoDong=880001,//识海暴动
    LingDan= 211010,//灵弹
    SwordCommon= 212010,//剑普攻
    LieHuoFenTian= 110001,//火焰爆轰
    ShiShenShouWei= 440002,//噬神守卫
    ZhenYuanChiYangJue=440003,//真元赤阳决
    PianHongLingDongJue=550001,//翩鸿灵动决
    JinFengYuLu=211011,//金风玉露
    SanHuaJuDingJue= 220001,//三花聚顶决
    ShuangBao=110008,//霜爆
    RealLieHuoFenTian = 110007,//真烈火焚天
    YueYaoBaHuang=212011,//月曜八荒
    FeiXueZhiMao = 3,//沸血之矛
    BiAnChiHun= 110011,//彼岸炽魂

    ShuiPuGong =4,//水普攻
    HuoPuGong=5,//火普攻
    LeiPuGong=6,//雷普攻
    BingPuGong=7,//冰普攻
    DarkPuGong=2,//暗普攻

    GangQiChongJi=110009,//罡气冲击
    TianGangQiJin=110010,//天罡气劲
    TianGangZhenYa=213001,//天罡镇压
}
/// <summary>
/// 战斗buff
/// </summary>
public class BattleBuff
{
    public SingleSkillData skillData;
    List<SkillUpgradeSetting> skillUpgradeList;
    public SkillSetting skillSetting;
    public BattleBuffSetting buffSetting;
    PeopleData people;

    public int existHuiHe;//持续回合
    public int remainHuiHe;//剩余回合
    public int reMainShieldNum;//剩余盾量

    public int remainHuZhaoNum;//剩余护罩量
    public int totalHuZhaoNum;//总护罩量

    //降攻buff
    public int jiangGongRate = 0;//降攻多少
    public int speedAddRate = 0;//加速多少
    public int critRateAdd = 0;//暴击率加多少
 
    public int burnVal = 0;//燃烧的数值
    //受爆buff
    public int beCritRate = 0;
    public int reactionDamageRate = 0;//剧变反应伤害系数
    public int kangAdd = 0;//抗性增加
    public ReactionType reactionType;
    public string commonParam;
    public int poJiaRate = 0;//破甲
    //public List< SinglePropertyData> addedProList=new List<SinglePropertyData>();
    public BattleBuff(BattleBuffSetting buffSetting, PeopleData p, SingleSkillData skilldata=null)
    {
        this.skillData = skilldata;
        this.buffSetting = buffSetting;

        people = p;
        remainHuiHe = buffSetting.HuiHe.ToInt32() + 1;

        //有受skilldata的影响的
        if (skillData != null)
        {
            skillSetting = BattleManager.Instance.FindSkillSetting(skillData.skillId);
            skillUpgradeList = BattleManager.Instance.FindSkillUpgradeListBySkillId(skillData.skillId);
            if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.Shield)
            {
              
                string param = skillUpgradeList[skillData.skillLevel - 1].Param2;

                List<float> paramList = CommonUtil.SplitCfgFloatOneDepth(param);

                int hpNum = (int)RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, people).limit;
                 
                int shiledHpAddParam = Mathf.RoundToInt(hpNum * (paramList[1] * 0.01f));
                reMainShieldNum =(int)MathF.Round( paramList[0] + shiledHpAddParam);
            }
            //神速降攻
            else if(buffSetting.BuffType.ToInt32() == (int)BattleBuffType.ShenSu)
            {
                string param = skillUpgradeList[skillData.skillLevel - 1].Damage;
                List<float> paramList = CommonUtil.SplitCfgFloatOneDepth(param);
                jiangGongRate = -(int)MathF.Round( paramList[1]);
                speedAddRate =(int)MathF.Round( paramList[0]);
            }      
            //加攻buff
            else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.JiaGong)
            {
                string param = skillUpgradeList[skillData.skillLevel - 1].Damage;
                List<float> paramList = CommonUtil.SplitCfgFloatOneDepth(param);
                jiangGongRate =(int)MathF.Round( paramList[0]);
            }
            //灼烧buff
            else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.ZhuoShao)
            {
                string param = skillUpgradeList[skillData.skillLevel - 1].Param2;
                 
                burnVal +=(int)MathF.Round( param.ToFloat());
            }
        

        }

        //火魂
        if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.HuoHun)
        {
            string param = buffSetting.Param;

            List<int> paramList = CommonUtil.SplitCfgOneDepth(param);
            jiangGongRate = paramList[0];
             critRateAdd = paramList[1];
            kangAdd = paramList[2];
        }

        if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.HuZhao)
        {
            string param = buffSetting.Param;
            totalHuZhaoNum =(int)( param.ToInt32() * 0.01 * RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, p).limit);
            remainHuZhaoNum = totalHuZhaoNum;
        }
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.JuBian)
        {
            string param = buffSetting.Param;
            reactionDamageRate = param.ToInt32();
        }
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.JianSu)
        {
            string param = buffSetting.Param;
            speedAddRate = -param.ToInt32();
        }
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.JiangGongNoSkillLevel)
        {
            string param = buffSetting.Param;
            jiangGongRate = -param.ToInt32();
        }
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.PoFang)
        {
            commonParam  ="-"+ buffSetting.Param;
        }
        //障目
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.ZhangMu)
        {
            beCritRate = -buffSetting.Param.ToInt32();
        }
        //寡断
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.GuaDuan)
        {
            commonParam = "-"+buffSetting.Param.ToInt32();
        }
        //力竭
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.LiJie)
        {
            commonParam = "-"+buffSetting.Param;
        } 
        //乱神
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.LuanShen)
        {
            commonParam = "-" + buffSetting.Param;
        }
        //加攻
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.JiaGongNoSkillLevel)
        {
            commonParam = buffSetting.Param;
        }
        //铁壁
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.TieBi)
        {
            commonParam = buffSetting.Param;
        }
        //贯注
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.GuanZhu)
        {
            commonParam = buffSetting.Param;
        }
        //遭暴
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.BeCritRate)
        {
            commonParam = buffSetting.Param;
        }
        //果决
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.GuoJue)
        {
            commonParam = buffSetting.Param;
        }
        //浩然
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.HaoRan)
        {
            commonParam = buffSetting.Param;
        } 
        //集中
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.JiZhong)
        {
            commonParam = buffSetting.Param;
        }
        //振奋
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.ZhenFen)
        {
            speedAddRate = buffSetting.Param.ToInt32();
        }
        //破甲
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.PoJia)
        {

            poJiaRate = buffSetting.Param.ToInt32();
        }
        //精通
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.JingTong)
        {
            commonParam = buffSetting.Param;
        } 
        //易感
        else if (buffSetting.BuffType.ToInt32() == (int)BattleBuffType.YiGan)
        {
            commonParam = buffSetting.Param;
        }
        if (buffSetting.Id.ToInt32() == (int)BattleBuffIdType.DianLiu)
        {
            reactionType =  ReactionType.DianLiu;
        }
    }

    /// <summary>
    /// 继续增益
    /// </summary>
    public void AddSameBuff()
    {
    }



}


/// <summary>
/// 技能类型
/// </summary>
public enum SkillType
{
    BaoFa=1,
    QuSan=3,
    Shield=6,//护盾
    XueRuo=7,//削弱
    DingShen=8,//定身
}

public enum BattleBuffIdType
{
    Eyi=10002,//恶意 被打两倍伤害
   // FanShang= 10003,//反伤
    TianYinHuDun=10006,//天音护盾
    ChaoShi=10042,//潮湿
    DianLiu= 10043,//电流
    DongJiang=10044,//冻僵
    ZhuoShao2= 10045,//灼烧
    CiBaoFengMai=10046,//封脉
    LeiQiangMaBi=10047,//雷枪麻痹
    BingLao=10048,//冰牢

    ZhuoShao=10010,//灼烧
    Piruo1=10011,//疲弱1
    Piruo3=10013,//疲弱3
    PoFang1=10014,//破防1
    PoFang3=10016,//破防3
    FengMai=10017,//封脉
    MaBi=10018,//麻痹
    ZhangMu=10019,//障目
    GuaDuan=10020,//寡断
    JinLiao=10021,//禁疗
    ChiHuan2=10023,//迟缓2
    LiJie1=10024,//力竭1
    LiJie3=10026,//力竭
    LuanShen=10027,//乱神

    LiLiang1 = 10029,//力量3
    LiLiang2 = 10030,//力量3
    LiLiang3 = 10031,//力量3
    YuHe=10032,//愈合
    TieBi1=10033,//铁壁1
    TieBi2=10034,//铁壁2
    TieBi3=10035,//铁壁3
    GuanZhu=10036,//贯注
    GuoJue=10037,//果决
    HaoRan1=10038,//浩然1
    HaoRan2=10039,//浩然2
    HaoRan3=10040,//浩然3
    JiZhong=10041,//集中
    ZhenFen1 = 10049,//振奋
    ZhenFen2 = 10050,//振奋
    DingShen=10005,//定身
    HuoDun=10051,//火盾
    GuangDun= 10058,//光盾
    DuoXian_1=10059,//1级堕仙
    HuoYin=10060,//火印
    HuoHun=10061,//火魂
    TianLongTaoZhenShang=10062,//天龙套真伤
}

/// <summary>
/// 战斗buff类型
/// </summary>
public enum BattleBuffType
{
    DingShen=8,//定身
    Shield=6,//护盾
    HuZhao=66,//护罩
    FanShang = 67,//反伤
    YiShang = 77,//易伤
    ShenSu=55,//神速（降攻增速）
    JiaGong=33,//加攻
    ZhuoShao=11,//灼烧
    BeCritRate=771,//遭受暴击率增加
    JuBian=12,//剧变反应
    JianSu=13,//减速
    FengMai=772,//封脉
    MaBi=773,//麻痹
    JiangGong=774,//降攻
    JiangGongNoSkillLevel=775,//和skilllevel无关的降攻buff
    PoFang=776,//破防
    ZhangMu=777,//障目 暴击率下降
    JinLiao=778,//禁疗
    LiJie=779,//力竭 
    LuanShen=7710,//乱神
    WeiMi=7711,//无法遭受任何强化效果
    GuaDuan = 7712,//寡断 暴击伤害下降
    JiaGongNoSkillLevel = 34,//和skillLevel无关的加攻buff
    YuHe=35,//愈合
    TieBi=36,//铁壁
    GuanZhu=37,//贯注
    GuoJue=38,//果决
    HaoRan=39,//浩然
    JiZhong=331,//集中
    ZhenFen=332,//振奋
    HuShen=333,//护身
    HurtEnemy=7713,//对敌人造成伤害
    HuoHun=551,//火魂
    ZhenShang=7714,//真伤
    FuHuo=7715,//复活
    ZiBao=7716,//自爆
    PoJia=7717,//破甲
    JingTong=7718,//属性精通
    FengYing=7719,//风影
    YiGan=7720,//易感
 }
/// <summary>
/// 为什么切人
/// </summary>
public enum QieRenPurposeType
{
    None=0,
    YuanSuReaction,
    FangDa,
}

public enum EnemyType
{
    None=0,
    FuBenMonster=1,//副本怪s
}