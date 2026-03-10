using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
public class ShenYuanManager : CommonInstance<ShenYuanManager>
{
    public GameInfo gameInfo;
    public int maxShenYuanLevelNum = 12;
    public override void Init()
    {
        base.Init();
        gameInfo = RoleManager.Instance._CurGameInfo;

        if (gameInfo.AllShenYuanData == null)
        {
            gameInfo.AllShenYuanData = new AllShenYuanData();
        }
        if(gameInfo.AllShenYuanData.ShenYuanList.Count< maxShenYuanLevelNum)
        {
            int num = maxShenYuanLevelNum - gameInfo.AllShenYuanData.ShenYuanList.Count;
            for(int i = 0; i < num; i++)
            {
                int theIndex = i;
                SingleShenYuanData data = new SingleShenYuanData();
                for(int j = 0; j < 4; j++)
                {
                    data.Layer1EnemyList.Add( GenerateMatchP(theIndex));

                }
            }
        }
    }

 

    /// <summary>
    /// 通过simplep生成p todo 和宗门等级和玩家最高角色战力相关
    /// </summary>
    /// <param name="simpleP"></param>
    /// <returns></returns>
    public PeopleData GenerateMatchP(int index)
    {
        float hpOffset = (ConstantVal.matchNPC_finalHp - ConstantVal.matchNPC_initHP) / 2000;
        float defenseOffset = (ConstantVal.matchNPC_finalDefense - ConstantVal.matchNPC_initDefense) / 2000;
        float attackOffset = (ConstantVal.matchNPC_finalAttack - ConstantVal.matchNPC_initAttack) / 2000;
        float critRateOffset = (ConstantVal.matchNPC_finalCritRate - ConstantVal.matchNPC_initCritRate) / 2000;
        float critNumOffset = (ConstantVal.matchNPC_finalCritNum - ConstantVal.matchNPC_initCritNum) / 2000;
        float mpSpeedOffset = (ConstantVal.matchNPC_finalMPSpeed - ConstantVal.matchNPC_initMPSpeed) / 2000;
        float skillLevel1Offset = (ConstantVal.matchNPC_finalSkillLevel1 - ConstantVal.matchNPC_initSkillLevel1) / 2000;
        float skillLevel2Offset = (ConstantVal.matchNPC_finalSkillLevel2 - ConstantVal.matchNPC_initSkillLevel2) / 2000;
        float xueMaiLevelOffset = (ConstantVal.matchNPC_finalXueMaiLevel - ConstantVal.matchNPC_initXueMaiLevel) / 2000;
        float trainIndexOffSet = (ConstantVal.matchNPC_finalTrainIndex - ConstantVal.matchNPC_initTrainIndex) / 2000;

        PeopleData p = new PeopleData();
        p.isPlayer = false;
        p.talent = (int)StudentTalent.LianGong;
        p.gender =  RandomManager.Next(1, 3);
        RoleManager.Instance.RdmFace(p);
        
        p.portraitType = (int)PortraitType.ChangeFace;
        p.name = RoleManager.Instance.rdmName((Gender)(int)p.gender);
        p.onlyId = ConstantVal.SetId;
        p.trainIndex = ConstantVal.matchNPC_initTrainIndex + (int)(index * trainIndexOffSet);
        int deRate = RandomManager.Next(90, 110);
        float proDeRate = 0.01f * deRate;

        SinglePropertyData hpPro = new SinglePropertyData();
        hpPro.id = (int)PropertyIdType.Hp;
        hpPro.num = ConstantVal.matchNPC_initHP + (int)(index * hpOffset);
        hpPro.num = (int)(hpPro.num * proDeRate);
        hpPro.limit = hpPro.num;
        p.propertyIdList.Add(hpPro.id);
        p.propertyList.Add(hpPro);

        SinglePropertyData defensePro = new SinglePropertyData();
        defensePro.id = (int)PropertyIdType.Defense;
        defensePro.num = ConstantVal.matchNPC_initDefense + (int)(index * defenseOffset);
        defensePro.num = (int)(defensePro.num * proDeRate);
        p.propertyIdList.Add(defensePro.id);
        p.propertyList.Add(defensePro);

        SinglePropertyData attackPro = new SinglePropertyData();
        attackPro.id = (int)PropertyIdType.Attack;
        attackPro.num = ConstantVal.matchNPC_initAttack + (int)(index * attackOffset);
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
        critNumPro.num = ConstantVal.matchNPC_initCritNum + (int)(index * critNumOffset);
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
            p.curBattleProList.Add(pro);
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
        singleSkill1.skillLevel = ConstantVal.matchNPC_initSkillLevel1 + (int)(index * skillLevel1Offset);

        SingleSkillData singleSkill2 = new SingleSkillData();

        List<SkillSetting> candidateList = DataTable.FindCanStudySkillListByYuanSu((YuanSuType)p.yuanSu);
        int skillIndex = RandomManager.Next(0, candidateList.Count);
        int skillId2 = candidateList[skillIndex].Id.ToInt32();
        singleSkill2.skillId = skillId2;
        singleSkill2.skillLevel = ConstantVal.matchNPC_initSkillLevel2 + (int)(index * skillLevel2Offset);
        p.allSkillData.skillList.Add(singleSkill1);
        p.allSkillData.skillList.Add(singleSkill2);
        p.allSkillData.equippedSkillIdList.Add(singleSkill1.skillId);
        p.allSkillData.equippedSkillIdList.Add(singleSkill2.skillId);
        return p;
    }
}
