using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Framework.Data;
using cfg;

public class MatchManager : CommonInstance<MatchManager>
{
    public Thread generateAllZongMenThread;
    public SingleOtherZongMenData playerZongMenData;
    //List<SingleOtherZongMenData> allList = new List<SingleOtherZongMenData>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Init()
    {
        base.Init();
        if (RoleManager.Instance._CurGameInfo.MatchData.TodayAwardGetStatusList.Count <= 0)
        {
            RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum = 0;
            RoleManager.Instance._CurGameInfo.MatchData.TodayParticipateMatchNum = 0;
            RoleManager.Instance._CurGameInfo.MatchData.TodayWinNum = 0;
            RoleManager.Instance._CurGameInfo.MatchData.TodayAwardGetStatusList.Clear();
            for (int i = 0; i < 3; i++)
            {
                RoleManager.Instance._CurGameInfo.MatchData.TodayAwardGetStatusList.Add((int)AccomplishStatus.UnAccomplished);
            }
            RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList.Clear();
            for (int i = 0; i < 3; i++)
            {
                RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList.Add((int)AccomplishStatus.UnAccomplished);
            }
            RoleManager.Instance._CurGameInfo.MatchData.WatchedADAddParticipateTime = false;
            RoleManager.Instance._CurGameInfo.MatchData.GetJieSuanAward = false;
        }
     
    }
    /// <summary>
    /// 电脑自由匹配
    /// </summary>
    public void AIAutoPiPei()
    {
        Mimic();

        //if (generateAllZongMenThread != null && generateAllZongMenThread.ThreadState != ThreadState.Stopped)
        //{
        //    return;
        //}
        //if (autoPiPeiThread != null && autoPiPeiThread.ThreadState != ThreadState.Stopped)
        //{
        //    return;
        //}
        //autoPiPeiThread = new Thread(new ThreadStart(AuToPiPeiThread));

        //autoPiPeiThread.Start();
    }
    void AuToPiPeiThread()
    {
        lock (this)
        {
          
           Mimic();
            
        }
    }
    /// <summary>
    /// 匹配
    /// </summary>
    public void PiPei()
    {
        List<SingleOtherZongMenData> zongMenList = new List<SingleOtherZongMenData>();
        int fromRankLevel = 0;
        int toRankLevel = 0;
        //List<SingleOtherZongMenData> candidateProto;
        List<SingleOtherZongMenData> candidate = new List<SingleOtherZongMenData>();
        if (RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel<=2)
        {
            fromRankLevel = 1;
            toRankLevel = 1;
            //candidateProto= ChooseZongMenByRankLevel(fromRankLevel, toRankLevel);
            //for(int i = 0; i < candidateProto.Count; i++) 
            //{
            //    candidate.Add(NeiCunModel.Instance.LoadSingleOtherZongmenNeiCunData(candidateProto[i]));
            //}
            candidate = ChooseZongMenByRankLevel(fromRankLevel, toRankLevel);

            candidate = CommonUtil.Shuffle<SingleOtherZongMenData>(candidate);

        }
        else if (RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel >= 12)
        {
            fromRankLevel = 10;
            toRankLevel = 12;
            //candidateProto = ChooseZongMenByRankLevel(fromRankLevel, toRankLevel);
            //for (int i = 0; i < candidateProto.Count; i++)
            //{
            //    candidate.Add(NeiCunModel.Instance.LoadSingleOtherZongmenNeiCunData(candidateProto[i]));
            //}
            candidate = ChooseZongMenByRankLevel(fromRankLevel, toRankLevel);

            candidate = CommonUtil.Shuffle<SingleOtherZongMenData>(candidate);
        }
        else
        {
            //candidateProto = ChooseZongMenByRankLevel(fromRankLevel, toRankLevel);
            //for (int i = 0; i < candidateProto.Count; i++)
            //{
            //    candidate.Add(NeiCunModel.Instance.LoadSingleOtherZongmenNeiCunData(candidateProto[i]));
            //}
            candidate = ChooseZongMenByRankLevel(fromRankLevel, toRankLevel);

            candidate = CommonUtil.Shuffle<SingleOtherZongMenData>(candidate);

            fromRankLevel = RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel - 1;
            toRankLevel= RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel + 1;
            List<SingleOtherZongMenData> candidate1 = ChooseZongMenByRankLevel(fromRankLevel, fromRankLevel);
           
            List<SingleOtherZongMenData> candidate2 = ChooseZongMenByRankLevel(RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel, RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel);
          
            List<SingleOtherZongMenData> candidate3 = ChooseZongMenByRankLevel(toRankLevel, toRankLevel);
        


            int num1 =(int)(candidate1.Count * 0.5f);
            int num2 = (int)(candidate2.Count * 0.4f);
            int num3 = (int)(candidate3.Count * 0.1f);

            for(int i = 0; i < num1; i++)
            {
                candidate.Add(candidate1[i]);
            }
            for (int i = 0; i < num2; i++)
            {
                candidate.Add(candidate2[i]);
            }
            for (int i = 0; i < num3; i++)
            {
                candidate.Add(candidate3[i]);
            }
             candidate = CommonUtil.Shuffle<SingleOtherZongMenData>(candidate);

        }


        //选玩家+七个进入比赛
        playerZongMenData = new SingleOtherZongMenData();
        playerZongMenData.zongMenName = RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenName;
        playerZongMenData.isPlayerZongMen = true;
        playerZongMenData.curRankLevel = RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel;
        playerZongMenData.theR = RoleManager.Instance._CurGameInfo.allZongMenData.TheR;
        playerZongMenData.curStar = RoleManager.Instance._CurGameInfo.allZongMenData.CurStar;
        for(int i=0;i < RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1.Count; i++)
        {
            ulong onlyId = RoleManager.Instance._CurGameInfo.AllTeamData.TeamList1[i];
            if (onlyId <= 0)
                continue;
            if(onlyId==RoleManager.Instance._CurGameInfo.playerPeople.onlyId)
                playerZongMenData.simplePList.Add(GenerateSimplePByPeople(RoleManager.Instance._CurGameInfo.playerPeople));
            else
                playerZongMenData.simplePList.Add(GenerateSimplePByPeople(StudentManager.Instance.FindStudent(onlyId)));
        }
        zongMenList.Add(playerZongMenData);

        for(int i = 0; i < 7; i++)
        {
            zongMenList.Add(candidate[i]);
        }

        EventCenter.Broadcast(TheEventType.StartPiPei, zongMenList);
     }

    public SimpleP GenerateSimplePByPeople(PeopleData p)
    {
        SimpleP simp = new SimpleP();
        simp.name = p.name;
        simp.onlyId = p.onlyId;
        simp.gender = p.gender;
        for(int i = 0; i < p.portraitIndexList.Count; i++)
        {
            simp.portraitIndexList.Add(p.portraitIndexList[i]);

        }
        simp.isPlayer = p.isPlayer;
        return simp;
    }

    /// <summary>
    /// 通过simplep生成p
    /// </summary>
    /// <param name="simpleP"></param>
    /// <returns></returns>
    public PeopleData GenerateMatchP(SimpleP simpleP,int index)
    {
        float hpOffset = (ConstantVal.matchNPC_finalHp - ConstantVal.matchNPC_initHP) / 2000;
        float defenseOffset = (ConstantVal.matchNPC_finalDefense - ConstantVal.matchNPC_initDefense) / 2000;
        float attackOffset = (ConstantVal.matchNPC_finalAttack - ConstantVal.matchNPC_initAttack) / 2000;
        float critRateOffset = (ConstantVal.matchNPC_finalCritRate - ConstantVal.matchNPC_initCritRate) / 2000;
        float critNumOffset = (ConstantVal.matchNPC_finalCritNum - ConstantVal.matchNPC_initCritNum) / 2000;
        float mpSpeedOffset = (ConstantVal.matchNPC_finalMPSpeed - ConstantVal.matchNPC_initMPSpeed) / 2000;
        float skillLevel1Offset = (ConstantVal.matchNPC_finalSkillLevel1 - ConstantVal.matchNPC_initSkillLevel1) / 2000;
        float skillLevel2Offset = (ConstantVal.matchNPC_finalSkillLevel2 - ConstantVal.matchNPC_initSkillLevel2) / 2000;
        float xueMaiLevelOffset= (ConstantVal.matchNPC_finalXueMaiLevel - ConstantVal.matchNPC_initXueMaiLevel) / 2000;
        float trainIndexOffSet = (ConstantVal.matchNPC_finalTrainIndex - ConstantVal.matchNPC_initTrainIndex) / 2000;


        PeopleData p = new PeopleData();
        p.isPlayer = simpleP.isPlayer;
        p.talent = (int)StudentTalent.LianGong;
        p.gender = simpleP.gender;
        for(int i = 0; i < simpleP.portraitIndexList.Count; i++)
        {
            p.portraitIndexList.Add(simpleP.portraitIndexList[i]);
        }
        p.trainIndex= ConstantVal.matchNPC_initTrainIndex + (int)(index * trainIndexOffSet);
        p.portraitType = (int)PortraitType.ChangeFace;
        p.name = simpleP.name;
        p.onlyId = simpleP.onlyId;
        int deRate = RandomManager.Next(90, 110);
        float proDeRate = 0.01f * deRate;

        SinglePropertyData hpPro = new SinglePropertyData();
        hpPro.id = (int)PropertyIdType.Hp;
        hpPro.num =ConstantVal.matchNPC_initHP + (int)(index * hpOffset);
        hpPro.num = (int)(hpPro.num * proDeRate);
        hpPro.limit = hpPro.num;
        p.propertyIdList.Add(hpPro.id);
        p.propertyList.Add(hpPro);

        SinglePropertyData defensePro = new SinglePropertyData();
        defensePro.id = (int)PropertyIdType.Defense;
        defensePro.num =ConstantVal.matchNPC_initDefense + (int)(index * defenseOffset);
        defensePro.num = (int)(defensePro.num * proDeRate);
        p.propertyIdList.Add(defensePro.id);
        p.propertyList.Add(defensePro);

        SinglePropertyData attackPro = new SinglePropertyData();
        attackPro.id = (int)PropertyIdType.Attack;
        attackPro.num =ConstantVal.matchNPC_initAttack + (int)(index * attackOffset);
        attackPro.num = (int)(attackPro.num * proDeRate);
        p.propertyIdList.Add(attackPro.id);
        p.propertyList.Add(attackPro);


        SinglePropertyData critRatePro = new SinglePropertyData();
        critRatePro.id = (int)PropertyIdType.CritRate;
        critRatePro.num = ConstantVal.matchNPC_initCritRate + (int)(index * critRateOffset);
        critRatePro.num = (int)(critRatePro.num * proDeRate);
        p.propertyIdList.Add(critRatePro.id);
        p.propertyList.Add(critRatePro);

        SinglePropertyData critNumPro = new SinglePropertyData();
        critNumPro.id = (int)PropertyIdType.CritNum;
        critNumPro.num =ConstantVal.matchNPC_initCritNum+ (int)(index * critNumOffset);
        critNumPro.num = (int)(critNumPro.num * proDeRate);
        p.propertyIdList.Add(critNumPro.id);
        p.propertyList.Add(critNumPro);

        SinglePropertyData mpSpeedPro = new SinglePropertyData();
        mpSpeedPro.id = (int)PropertyIdType.MPSpeed;
        mpSpeedPro.num = ConstantVal.matchNPC_initMPSpeed + (int)(index * mpSpeedOffset);
        mpSpeedPro.num = (int)(mpSpeedPro.num * proDeRate);
        p.propertyIdList.Add(mpSpeedPro.id);
        p.propertyList.Add(mpSpeedPro);


        SinglePropertyData mpPro = new SinglePropertyData();
        mpPro.id = (int)PropertyIdType.MpNum;
        mpPro.num = 0;
        mpPro.limit = 100;
        p.propertyIdList.Add(mpPro.id);
        p.propertyList.Add(mpPro);

        SinglePropertyData jingTongPro = new SinglePropertyData();
        jingTongPro.id = (int)PropertyIdType.JingTong;
        jingTongPro.num = 0;
         p.propertyIdList.Add(jingTongPro.id);
        p.propertyList.Add(jingTongPro);

        for (int k = 0; k < p.propertyList.Count; k++)
        {
            SinglePropertyData pro = p.propertyList[k];
            SinglePropertyData battlePro = new SinglePropertyData();
            battlePro.id = pro.id;
            battlePro.num = pro.num;
            battlePro.limit = pro.limit;
            p.curBattleProIdList.Add(battlePro.id);
            p.curBattleProList.Add(battlePro);
        }
        p.yuanSu = RandomManager.Next(1, (int)YuanSuType.End);
        int xueMaiLevel = (int)(index * xueMaiLevelOffset);
        p.xueMai = new XueMaiData();
        p.xueMai.xueMaiTypeList.Add(XueMaiType.ShangHai);
        p.xueMai.xueMaiLevelList.Add(xueMaiLevel);
        p.xueMai.xueMaiTypeList.Add(XueMaiType.JingTong);
        p.xueMai.xueMaiLevelList.Add(xueMaiLevel);

        //int yuanSu = RandomManager.Next(1, (int)YuanSuType.End);
        p.allSkillData = new AllSkillData();
        SingleSkillData singleSkill1 = new SingleSkillData();
        singleSkill1.skillId = (int)BattleManager.Instance.PuGongIdByYuanSu((YuanSuType)p.yuanSu);
        singleSkill1.skillLevel =ConstantVal.matchNPC_initSkillLevel1 + (int)(index * skillLevel1Offset);

        SingleSkillData singleSkill2 = new SingleSkillData();
  
        List<SkillSetting> candidateList = DataTable.FindCanStudySkillListByYuanSu((YuanSuType)p.yuanSu);
        int skillIndex = RandomManager.Next(0, candidateList.Count);
        int skillId2 =  candidateList[skillIndex].Id.ToInt32();
        singleSkill2.skillId = skillId2;
        singleSkill2.skillLevel =ConstantVal.matchNPC_initSkillLevel2+ (int)(index * skillLevel2Offset);
        p.allSkillData.skillList.Add(singleSkill1);
        p.allSkillData.skillList.Add(singleSkill2);
        p.allSkillData.equippedSkillIdList.Add(singleSkill1.skillId);
        p.allSkillData.equippedSkillIdList.Add(singleSkill2.skillId);
        return p;
    }

    /// <summary>
    /// 选择低于xx分的宗门
    /// </summary>
    public List<SingleOtherZongMenData> ChooseZongMenBelowScore(int score)
    {
        //12个等级 每个等级5个
        List<SingleOtherZongMenData> res = new List<SingleOtherZongMenData>();
        //2000-5000分来算
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList.Count; i++)
        {
            SingleOtherZongMenData single = RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList[i];
            if (single.theR<=score)
            {
                res.Add(single);
            }
        }
        return res;
    }
    /// <summary>
    /// 通过等级选择宗门
    /// </summary>
    public List<SingleOtherZongMenData> ChooseZongMenByRankLevel(int rankLevelFrom,int rankLevelTo)
    {
        //12个等级 每个等级5个
        List<SingleOtherZongMenData> res = new List<SingleOtherZongMenData>();
        //800-3000分来算
        for(int i = 0; i < RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList.Count; i++)
        {
            SingleOtherZongMenData single = RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList[i];
            if (single.curRankLevel >= rankLevelFrom && single.curRankLevel <= rankLevelTo)
            {
                res.Add(single);
            }
        }
        return res;

    }



    /// <summary>
    /// 生成所有宗门
    /// </summary>
    public void GenerateAllZongMenSyn()
    {
        generateAllZongMenThread = new Thread(new ThreadStart(GenerateAllZongMen));

        generateAllZongMenThread.Start();
    }

    /// <summary>
    /// 生产所有宗门
    /// </summary>
    public void GenerateAllZongMen()
    {
        lock (this)
        {
            int baseR = 800;
            int maxR = 3000;
            float rOffset = (maxR - baseR) / 2000;
            //int initHP = 2000;
            //int finalHp = 16900;
            //int initDefense = 400;
            //int finalDefense = 3380;
            //int initAttack = 400;
            //int finalAttack = 3380;
            //int initCritRate = 5;
            //int finalCritRate = 75;
            //int initCritNum = 10;
            //int finalCritNum = 150;
            //int initMPSpeed = 0;
            //int finalMPSpeed = 97;

            //int initSkillLevel1 = 1;
            //int finalSkillLevel1 = 25;

            //int initSkillLevel2 = 1;
            //int finalSkillLevel2 = 50;
            
            //todo 增伤
            float hpOffset = (ConstantVal.matchNPC_finalHp - ConstantVal.matchNPC_initHP) / 2000;
            float defenseOffset = (ConstantVal.matchNPC_finalDefense - ConstantVal.matchNPC_initDefense) / 2000;
            float attackOffset = (ConstantVal.matchNPC_finalAttack - ConstantVal.matchNPC_initAttack) / 2000;
            float critRateOffset = (ConstantVal.matchNPC_finalCritRate - ConstantVal.matchNPC_initCritRate) / 2000;
            float critNumOffset = (ConstantVal.matchNPC_finalCritNum - ConstantVal.matchNPC_initCritNum) / 2000;
            float mpSpeedOffset = (ConstantVal.matchNPC_finalMPSpeed - ConstantVal.matchNPC_initMPSpeed) / 2000;
            float skillLevel1Offset = (ConstantVal.matchNPC_finalSkillLevel1 - ConstantVal.matchNPC_initSkillLevel1) / 2000;
            float skillLevel2Offset = (ConstantVal.matchNPC_finalSkillLevel2 - ConstantVal.matchNPC_initSkillLevel2) / 2000;
         
            float trainIndexOffset = (ConstantVal.matchNPC_finalTrainIndex - ConstantVal.matchNPC_initTrainIndex) / 2000;

            for (int i = 0; i < 2000; i++)
            {
                int theR = baseR + Mathf.RoundToInt(i * rOffset);
                int num = 0;
                if (theR >= 800 && theR < 1240)
                {
                    num = 1;
                }
                else if (theR >= 1240 && theR < 1640)
                {
                    num = 2;
                }
                else if (theR >= 1640 && theR < 2080)
                {
                    num = 4;
                }
                else if (theR >= 2080 && theR < 2520)
                {
                    num = 2;
                }
                else
                {
                    num = 1;
                }
                for (int n = 0; n < num; n++)
                {
                    SingleOtherZongMenData theData = new SingleOtherZongMenData();
                    theData.theR = theR;
                    theData.zongMenName = ZongMenManager.Instance.RdmZongMenName();
                    List<int> rank = RankLevelByScore(theData.theR);
                    theData.curRankLevel = rank[0];
                    theData.curStar = rank[1];
                    for (int j = 0; j < 4; j++)
                    {
                        PeopleData p = new PeopleData();
                        p.talent = (int)StudentTalent.LianGong;
                        p.gender = RandomManager.Next(1, 3);
                        RoleManager.Instance.RdmFace(p);
                  
                        p.portraitType = (int)PortraitType.ChangeFace;

                        p.name = RoleManager.Instance.rdmName((Gender)p.gender);
                        p.onlyId = ConstantVal.SetId;

                        float proDeRate = 1;
                        if (j == 0)
                        {
                            int deRate = RandomManager.Next(90, 100);
                            proDeRate = 0.01f * deRate;
                        }
                        else
                        {
                            int deRate = RandomManager.Next(70, 90);
                            proDeRate = 0.01f * deRate;
                        }
                        p.trainIndex = ConstantVal.matchNPC_initTrainIndex + (int)(i * trainIndexOffset);

                        SinglePropertyData hpPro = new SinglePropertyData();
                        hpPro.id = (int)PropertyIdType.Hp;
                        hpPro.num = ConstantVal.matchNPC_initHP + (int)(i * hpOffset);
                        hpPro.num = (int)(hpPro.num * proDeRate);
                        hpPro.limit = hpPro.num;
                        p.propertyIdList.Add(hpPro.id);
                        p.propertyList.Add(hpPro);

                        SinglePropertyData defensePro = new SinglePropertyData();
                        defensePro.id = (int)PropertyIdType.Defense;
                        defensePro.num = ConstantVal.matchNPC_initDefense + (int)(i * defenseOffset);
                        defensePro.num = (int)(defensePro.num * proDeRate);
                        p.propertyIdList.Add(defensePro.id);
                        p.propertyList.Add(defensePro);

                        SinglePropertyData attackPro = new SinglePropertyData();
                        attackPro.id = (int)PropertyIdType.Attack;
                        attackPro.num = ConstantVal.matchNPC_initAttack + (int)(i * attackOffset);
                        attackPro.num = (int)(attackPro.num * proDeRate);
                        p.propertyIdList.Add(attackPro.id);
                        p.propertyList.Add(attackPro);


                        SinglePropertyData critRatePro = new SinglePropertyData();
                        critRatePro.id = (int)PropertyIdType.CritRate;
                        critRatePro.num = ConstantVal.matchNPC_initCritRate + (int)(i * critRateOffset);
                        critRatePro.num = (int)(critRatePro.num * proDeRate);
                        p.propertyIdList.Add(critRatePro.id);
                        p.propertyList.Add(critRatePro);

                        SinglePropertyData critNumPro = new SinglePropertyData();
                        critNumPro.id = (int)PropertyIdType.CritNum;
                        critNumPro.num = ConstantVal.matchNPC_initCritNum + (int)(i * critNumOffset);
                        critNumPro.num = (int)(critNumPro.num * proDeRate);
                        p.propertyIdList.Add(critNumPro.id);
                        p.propertyList.Add(critNumPro);

                        SinglePropertyData mpSpeedPro = new SinglePropertyData();
                        mpSpeedPro.id = (int)PropertyIdType.MPSpeed;
                        mpSpeedPro.num = ConstantVal.matchNPC_initMPSpeed + (int)(i * mpSpeedOffset);
                        mpSpeedPro.num = (int)(mpSpeedPro.num * proDeRate);
                        p.propertyIdList.Add(mpSpeedPro.id);
                        p.propertyList.Add(mpSpeedPro);


                        SinglePropertyData mpPro = new SinglePropertyData();
                        mpPro.id = (int)PropertyIdType.MpNum;
                        mpPro.num = 0;
                        mpPro.limit = 100;
                        p.propertyIdList.Add(mpPro.id);
                        p.propertyList.Add(mpPro);

                        for (int k = 0; k < p.propertyList.Count; k++)
                        {
                            SinglePropertyData pro = p.propertyList[k];
                            SinglePropertyData battlePro = new SinglePropertyData();
                            battlePro.id = pro.id;
                            battlePro.num = pro.num;
                            battlePro.limit = pro.limit;
                            p.curBattleProIdList.Add(battlePro.id);
                            p.curBattleProList.Add(battlePro);
                        }
                        p.allSkillData = new AllSkillData();
                        SingleSkillData singleSkill1 = new SingleSkillData();
                        singleSkill1.skillId = (int)SkillIdType.LingDan;
                        singleSkill1.skillLevel = ConstantVal.matchNPC_initSkillLevel1 + (int)(i * skillLevel1Offset);

                        SingleSkillData singleSkill2 = new SingleSkillData();
                        List<int> candidateList = new List<int>();

                        for (int k = 0; k < DataTable._skillList.Count; k++)
                        {
                            SkillSetting setting = DataTable._skillList[k];
                            if (!string.IsNullOrWhiteSpace(setting.CanStudy))
                            {
                                candidateList.Add(setting.Id.ToInt32());

                            }
                        }
                        //for (int k = 0; k < ConstantVal.canLingWuSkillIdList2.Count; k++)
                        //{
                        //    candidateList.Add((int)ConstantVal.canLingWuSkillIdList2[k]);
                        //}

                        int skillIndex = RandomManager.Next(0, candidateList.Count);
                        int skillId2 = (int)candidateList[skillIndex];
                        singleSkill2.skillId = skillId2;
                        singleSkill2.skillLevel = ConstantVal.matchNPC_initSkillLevel2 + (int)(i * skillLevel2Offset);
                        p.allSkillData.skillList.Add(singleSkill1);
                        p.allSkillData.skillList.Add(singleSkill2);
                        p.allSkillData.equippedSkillIdList.Add(singleSkill1.skillId);
                        p.allSkillData.equippedSkillIdList.Add(singleSkill2.skillId);

                        SimpleP simpleP = new SimpleP();
                        simpleP.name = p.name;
                        simpleP.gender = p.gender;
                        simpleP.onlyId = p.onlyId;
                        for(int k = 0; k < p.portraitIndexList.Count; k++)
                        {
                            simpleP.portraitIndexList.Add(p.portraitIndexList[k]);
                        }
                        theData.simplePList.Add(simpleP);
                        theData.totalZhanDouLi += (int)RoleManager.Instance.CalcZhanDouLi(p);
                    }
                    theData.pList.Clear();
                    theData.peopleProIndex = i;

                    RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList.Add(theData);
                }
            }
        }
      
    }

    public void Mimic()
    {
        if (RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList.Count == 0)
        {
            GenerateAllZongMen();
        }
        CommonUtil.Shuffle<SingleOtherZongMenData>(RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList);
        int zuNum = RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList.Count / 8;
        int index = 0;
        for(int i = 0; i < zuNum; i++)
        {
            List<SingleOtherZongMenData> group = new List<SingleOtherZongMenData>();
            for(int j = 0; j < 8; j++)
            {
                group.Add(RoleManager.Instance._CurGameInfo.AllOtherZongMenData.zongMenList[index]);
                index++;
            }
            MimicPiPeiMatch(group);
        }



    }

    //大家初始都是2000分 然后随机匹配 8组人
    void MimicPiPeiMatch(List<SingleOtherZongMenData> otherList)
    {
        SingleOtherZongMenData win1 = null;
        SingleOtherZongMenData win2 = null;

        SingleOtherZongMenData win3 = null;
        SingleOtherZongMenData win4 = null;

        SingleOtherZongMenData z1 = otherList[0];
        SingleOtherZongMenData z2 = otherList[1];

        SingleOtherZongMenData z3 = otherList[2];
        SingleOtherZongMenData z4 = otherList[3];

        SingleOtherZongMenData z5 = otherList[4];
        SingleOtherZongMenData z6 = otherList[5];

        SingleOtherZongMenData z7 = otherList[6];
        SingleOtherZongMenData z8 = otherList[7];

        win1 = MimicSingleGroupPiPeiMatch(z1, z2);
        win2= MimicSingleGroupPiPeiMatch(z3, z4);
        win3 = MimicSingleGroupPiPeiMatch(z5, z6);
        win4 = MimicSingleGroupPiPeiMatch(z7, z8);

        SingleOtherZongMenData win11 = MimicSingleGroupPiPeiMatch(win1, win2);
        SingleOtherZongMenData win22 = MimicSingleGroupPiPeiMatch(win3, win4);


        SingleOtherZongMenData winner = MimicSingleGroupPiPeiMatch(win11, win22);


    }

    SingleOtherZongMenData MimicSingleGroupPiPeiMatch(SingleOtherZongMenData pA, SingleOtherZongMenData pB)
    {
        SingleOtherZongMenData winner = null;

        float EA = 1 / (1 + Mathf.Pow(10, (pB.totalZhanDouLi - pA.totalZhanDouLi) / 400));
        float EB1=1 -EA;
        //float ScoreChangeEA = 1 / (1 + Mathf.Pow(10, (pB.TheR-pA.TheR) / 400));
        //float ScoreChangeEB = 1 - ScoreChangeEA;

        int val = RandomManager.Next(0, 100);
        //a赢了
        if (val < EA * 100)
        {
            int scoreChange = (int)(32 * (1 - EA));
            if (scoreChange > 0)
            {

            }
            //pA.TotalZhanDouLi += scoreChange;
            //pB.TotalZhanDouLi -= scoreChange;
            pA.theR += scoreChange;
            pB.theR -= scoreChange;
            winner = pA;
        }
        //b赢了
        else
        {
            int scoreChange = (int)(32 * (1 - EB1));
            if (scoreChange > 0)
            {

            }
            //pA.TotalZhanDouLi += scoreChange;
            //pB.TotalZhanDouLi -= scoreChange;
            pB.theR += scoreChange;
            pA.theR -= scoreChange;
            winner = pB;
        }
        List<int> rankLevelA = RankLevelByScore(pA.theR);
        pA.curRankLevel = rankLevelA[0];
        pA.curStar = rankLevelA[1];
        List<int> rankLevelB = RankLevelByScore(pB.theR);
        pB.curRankLevel = rankLevelB[0];
        pB.curStar = rankLevelB[1];
        return winner;
    }

    public int MatchScoreChange(SingleOtherZongMenData pA, SingleOtherZongMenData pB,bool leftWin)
    {
        SingleOtherZongMenData winner = null;

        float EA = 1 / (1 + Mathf.Pow(10, (pB.totalZhanDouLi - pA.totalZhanDouLi) / 400));

        float ScoreChangeEA = 1 / (1 + Mathf.Pow(10, (pB.theR - pA.theR) / 400));
        float ScoreChangeEB = 1 - ScoreChangeEA;

        int val = RandomManager.Next(0, 100);
        int scoreChange = 0;
        //a赢了
        if (leftWin)
        {
            scoreChange = (int)(32 * (1 - ScoreChangeEA));
            if(pA.isPlayerZongMen
                && pA.curRankLevel >= PlayerMaxRankLevel())
            {
                pA.curRankLevel = PlayerMaxRankLevel();
            }
            else
            {
                pA.theR += scoreChange;
                pB.theR -= scoreChange;
            }

            winner = pA;
        }
        //b赢了
        else
        {
            scoreChange = (int)(32 * (1 - ScoreChangeEB));
            pB.theR += scoreChange;
            pA.theR -= scoreChange;
            winner = pB;
        }
        List<int> rankLevelA = RankLevelByScore(pA.theR);
        pA.curRankLevel = rankLevelA[0];
        pA.curStar = rankLevelA[1];
        List<int> rankLevelB = RankLevelByScore(pB.theR);
        pB.curRankLevel = rankLevelB[0];
        pB.curStar = rankLevelB[1];
        return scoreChange;
     }

    /// <summary>
    /// 获取等级|星数 当分数大于等于5000 则为13段位 返回12段位 当分数小于等于2000 则为1段位
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public List<int> RankLevelByScore(int score)
    {
        //12个等级 每个等级5个

        //800-3000分
        int bigRangeOffset= (3000 - 800) / 12;
        int startLevel = 1;

        int curRangeScore = 800;
        for(int i = 0; i < 11; i++)
        {
            curRangeScore += bigRangeOffset;
            if (score < curRangeScore)
            {
                //说明就是当前段位
                break;
            }
            else
            {
                //段位要加
                startLevel++;
            }
        }

        if (startLevel >= 12)
            startLevel = 12;

        int theStar = 1;
        if (curRangeScore > score)
            curRangeScore -= bigRangeOffset;
        int curSmallScore = score- curRangeScore;
        int startSmallScore = 0;
        int smallScoreOffset = bigRangeOffset/5;
        //具体是哪个星
        for (int i = 0; i < 5; i++)
        {
            startSmallScore += smallScoreOffset;
            if (curSmallScore < startSmallScore)
            {
                //说明就是当前星
                break;
            }
            else
            {
                //星要加
                theStar++;
            }
        }
        if (theStar >= 5)
            theStar = 5;

        return new List<int>() { startLevel, theStar };
    }

    /// <summary>
    /// 找是否宗门的人
    /// </summary>
    /// <param name="onlyId"></param>
    /// <returns></returns>
    public bool CheckIfZongMenP(ulong onlyId,SingleOtherZongMenData data)
    {
        for(int i = 0; i < data.pList.Count; i++)
        {
            if (data.pList[i].onlyId == onlyId)
                return true;
        }
        return false;
    }
    /// <summary>
    /// 增加大比次数
    /// </summary>
    public void OnADAddMatchNum()
    {
        int watchedNum = RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum;

        if (watchedNum < ConstantVal.maxMatchTianJingBrushNum)
        {
            int needNum = ConstantVal.MatchAddCountNeedTianJing;
            if (ItemManager.Instance.CheckIfItemEnough((int)ItemIdType.LingJing, (ulong)needNum))
            {
                ItemManager.Instance.LoseItem((int)ItemIdType.LingJing, (ulong)needNum);

                RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum++;
                EventCenter.Broadcast(TheEventType.OnAddMatchParticipateNum);
            }
        }
      


    }
    /// <summary>
    /// 刷新现实时间
    /// </summary>
    public void RefreshRoleMatchTime(long x)
    {
        RoleManager.Instance._CurGameInfo.MatchData.AddedDaBiParticipateNum = 0;
        RoleManager.Instance._CurGameInfo.MatchData.LastParticipateMatchTime = x;
        RoleManager.Instance._CurGameInfo.MatchData.TodayParticipateMatchNum = 0;
        RoleManager.Instance._CurGameInfo.MatchData.TodayWinNum = 0;
        RoleManager.Instance._CurGameInfo.MatchData.TodayAwardGetStatusList.Clear();
        for(int i = 0; i < 3; i++)
        {
            RoleManager.Instance._CurGameInfo.MatchData.TodayAwardGetStatusList.Add((int)AccomplishStatus.UnAccomplished);
        }
        RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList.Clear();
        for (int i = 0; i < 3; i++)
        {
            RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList.Add((int)AccomplishStatus.UnAccomplished);
        }
        RoleManager.Instance._CurGameInfo.MatchData.WatchedADAddParticipateTime = false;
        RoleManager.Instance._CurGameInfo.MatchData.GetJieSuanAward = false;

 
    }

    /// <summary>
    /// 领取结算奖励
    /// </summary>
    public void GetJieSuanAward()
    {
        if (!RoleManager.Instance._CurGameInfo.MatchData.GetJieSuanAward)
        {
            RoleManager.Instance._CurGameInfo.MatchData.GetJieSuanAward = true;
            int num = JieSuanAwardNum(RoleManager.Instance._CurGameInfo.allZongMenData.CurRankLevel, RoleManager.Instance._CurGameInfo.allZongMenData.CurStar);
            ItemManager.Instance.GetItemWithTongZhiPanel((int)ItemIdType.RongYuBi, (ulong)num);
            EventCenter.Broadcast(TheEventType.GetMatchDailyJieSuanAward);
        }

    }


    public int JieSuanAwardNum(int level,int star)
    {
        int res = 0;
        switch (level)
        {
            case 1:
                res = 20 + 2 * (star - 1);
                break;
            case 2:
                res = 50 + 2 * (star - 1);
                break;
            case 3:
                res = 80 + 2 * (star - 1);
                break;
            case 4:
                res = 130 + 2 * (star - 1);
                break;
            case 5:
                res = 170 + 2 * (star - 1);
                break;
            case 6:
                res = 200 + 2 * (star - 1);
                break;
            case 7:
                res = 250 + 2 * (star - 1);
                break;
            case 8:
                res = 300 + 2 * (star - 1);
                break;
            case 9:
                res = 350 + 2 * (star - 1);
                break;
            case 10:
                res = 400 + 2 * (star - 1);
                break;
            case 11:
                res = 450 + 2 * (star - 1);
                break;
            case 12:
                res = 500 + 2 * (star - 1);
                break;
        }
        return res;
    }
  
    /// <summary>
    /// 敌人打之前说
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public string EnemyBeforeMatchMatchDialog(SingleOtherZongMenData other)
    {
       return "";
    }

 


    /// <summary>
    /// 首胜得奖
    /// </summary>
    public void OnGetDailiWinMatchAward(int index)
    {
        if (RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList[index] == (int)AccomplishStatus.Accomplished)
        {
            RoleManager.Instance._CurGameInfo.MatchData.TodayWinAwardGetStatusList[index] = (int)AccomplishStatus.GetAward;

            string awardStr = "";
            if (index == 0)
            {
                awardStr = ConstantVal.matchDailyAward_1win;
            }else if (index == 1)
            {
                awardStr = ConstantVal.matchDailyAward_2win;
            }
            else if(index==2)
            {
                awardStr = ConstantVal.matchDailyAward_3win;

            }

            List<List<int>> award = CommonUtil.SplitCfg(awardStr);
            for(int i = 0; i < award.Count; i++)
            {
                List<int> single = award[i];
                ItemManager.Instance.GetItemWithTongZhiPanel(single[0],(ulong) single[1]);
            }
            EventCenter.Broadcast(TheEventType.OnGetDailyWinAward);

        }
      
    }

    /// <summary>
    /// 最大段位受宗门等级限制
    /// </summary>
    /// <returns></returns>
    public int PlayerMaxRankLevel()
    {
         int res = (RoleManager.Instance._CurGameInfo.allZongMenData.ZongMenLevel / 10) * 3;
         return res;
    }

    /// <summary>
    /// 达到某段位需要多少宗门
    /// </summary>
    /// <returns></returns>
    public int PlayerRankLevelNeedZongMenLevel(int level)
    {
        return ( (level-1) / 3  + 1)*10;
    }
}
//最强一击 最高暴击 类似原神的深渊总结 杀敌最多 mvp

public enum MatchDialogType
{

}