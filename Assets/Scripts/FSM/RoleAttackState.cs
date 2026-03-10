using cfg;
using Framework.Data;
 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 角色攻击状态
/// </summary>
public class RoleAttackState : IState
{
    public BattlePeopleView role;
    public RoleAnim anim;
    //这是因为animation的event有时候不触发，原因暂未知
    public float attackStatusTime;//该状态时间
    public float attackStatusTimer;//该状态计时器

    //public Owner roleObj;
    bool isActivate;//激活状态

    float skillForeRollTime;//技能前摇
    float skillForeRollTimer;//技能前摇
    bool startSkillForeRoll;//开始技能前摇


    float skillCenterTime;//技能
    float skillCenterTimer;//技能
    bool startSkillCenter;//开始技能

    List<float> skillDamageTimeList = new List<float>();//造成伤害的时间节点
    int finishedDamage = -1;//哪些段伤害已结束
    bool finishedAllAttack = false;//结束了所有多段攻击

    List<float> energyAddTimeList = new List<float>();//加能量的时间节点
    List<float> energyAddNumList = new List<float>();//加能量
    int finishedEnergyAdd = -1;//哪些段能量增加已结束
    SkillSetting skillSetting;
    int curChoosedSkillIndex;//当前选择的技能

    List<AttackResData> thisAttackResList = new List<AttackResData>();//这次攻击造成的伤害

    string attackAnimHouZhui = "";//攻击后缀

    public RoleAttackState(BattlePeopleView role)
    {
        this.role = role;
        
    }

 
    /// <summary>
    /// 进入攻击
    /// </summary>
    public void OnEnter()
    {
        //role.finishedAttack = false;
        //try
        //{
        //role.attackCDTimer = 0;//进入攻击时 cd归零

        if (BattleManager.Instance.FindBuffTypeBuff(role.peopleData, BattleBuffType.MaBi)!=null)
        {
            role.endAttack = true;
            role.attackCDTimer = 0;
        }
        else
        {
            role.endAttack = false;
            role.attackCDTimer = 0;//进入攻击时 cd归零
            BattleManager.Instance.AddLogicPause();//进入攻击时 时间暂停

            isActivate = true;
            attackStatusTimer = 0;

            //Debug.Log(role.peopleData.name + "把logicpause打开并进入Attack");
            //能量足够，则进入qte经脉
            if (role.peopleData.isMyTeam)
            {
                if (role.readyToBig
                    &&RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum,role.peopleData).num>=100)
                {
                    //role.curChoosedSkill = role.PeopleData.AllSkillData.SkillList[1];
                    EventCenter.Broadcast(TheEventType.StartQTE,role.curDaZhaoIndex);
                    //StartAttack(role.curDaZhaoIndex);

                }
                else
                {
                    StartAttack(0);
                }
                return;

            }
            //如果是敌人
            else
            {
                SinglePropertyData pro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, role.peopleData);

                if (role.peopleData.allSkillData.equippedSkillIdList.Count >= 2)
                {
                    //能放大 且没被封脉
                    if (pro.num >= pro.limit
                    && BattleManager.Instance.FindBuffTypeBuff(role.peopleData, BattleBuffType.FengMai) == null)
                    {
                        //AI写在这里
                        int theSkillIndex = BattleManager.Instance.EnemyAI(role.peopleData);
                        StartAttack(theSkillIndex);

                    }
                    else
                    {
                        StartAttack(0);

                    }
                }
                else
                {
                    StartAttack(0);

                }

                //int bigSkillId = role.PeopleData.AllSkillData.SkillList[1].SkillId;
                //SkillSetting skillSetting = DataTable.FindSkillSetting(bigSkillId);
                //int energyNeed = skillSetting.energyNeed.ToInt32();
                //if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, role.PeopleData).Num >= 30)
                //{

                //    RoleManager.Instance.DeBattleProperty(PropertyIdType.MpNum, -energyNeed, role.PeopleData);
                //    role.curChoosedSkill = role.PeopleData.AllSkillData.SkillList[1];
                //    EventCenter.Broadcast(TheEventType.StartQTE);
                //}
                //else
                //{
                //    StartAttack(0);
                //}
            }
        }
        
        //}
        //catch(Exception e)
        //{
        //    Debug.LogError(e);
        //}
    


        //role.UseSkill(role.skillList[role.curUseSkillIndex], null, null);

        //这里执行技能


    }

    /// <summary>
    /// 开始发招
    /// </summary>
    public void StartAttack(int skillIndex)
    {
        try
        {
            int voiceXingGe = (int)XingGeType.AiJia;
            if (role.peopleData.socializationData != null)
            {
                voiceXingGe = role.peopleData.socializationData.xingGe;
            }
            else
            {
                voiceXingGe = RandomManager.Next(1, (int)XingGeType.End);
            }
            if (role.peopleData.gender !=(int) Gender.None)
            {
                AuditionManager.Instance.PlayVoice(Camera.main.transform,
                AuditionManager.Instance.JiaoAudio(voiceXingGe, (Gender)(int)role.peopleData.gender));
            }
            else
            {
                AuditionManager.Instance.PlayVoice(Camera.main.transform, AuditionManager.Instance.MonsterJiaoAudio());
            }
        

            thisAttackResList = new List<AttackResData>();
            curChoosedSkillIndex = skillIndex;
            //Debug.Log("在StartAttack方法中，role为" + role.peopleData.name);
            role.readyToBig = false;
            if (skillIndex >= 1)
            {
                //扣能量
                RoleManager.Instance.DeBattleProperty(PropertyIdType.MpNum, (int)-100, role.peopleData);

            }

            role.curChoosedSkill = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(role.peopleData.allSkillData.equippedSkillIdList[skillIndex], role.peopleData.allSkillData);




            skillSetting = BattleManager.Instance.FindSkillSetting(role.curChoosedSkill.skillId);

            BattleManager.Instance.curSkillAudioClip = new List<AudioClip>();
            if (!string.IsNullOrWhiteSpace(skillSetting.HitAudio))
            {
                List<string> theList = CommonUtil.SplitCfgStringOneDepth(skillSetting.HitAudio);
                for (int i = 0; i < theList.Count; i++)
                {
                    BattleManager.Instance.curSkillAudioClip.Add(ResourceManager.Instance.GetObj<AudioClip>(ConstantVal.audioFolderPath + theList[i]));
                }
            }

            skillForeRollTime = skillSetting.ForeRollTime.ToFloat();
            skillForeRollTimer = 0;
            startSkillForeRoll = true;
            //Debug.Log(role.peopleData.name + "在RoleAttackState中，由于开始攻击，startSkillForeRoll值为" + startSkillForeRoll);

            skillCenterTime = skillSetting.CenterTime.ToFloat();
            skillCenterTimer = 0;
            startSkillCenter = false;
           // Debug.Log(role.peopleData.name + "在RoleAttackState中，由于开始攻击，startSkillCenter值为" + startSkillCenter);

            skillDamageTimeList.Clear();
            finishedDamage = -1;

            if (skillSetting.HaveAttack == "1")
            {
                string[] skillDamageTimeArr = skillSetting.DamageTimeArr.Split('|');
                for (int i = 0; i < skillDamageTimeArr.Length; i++)
                {
                    skillDamageTimeList.Add(skillDamageTimeArr[i].ToFloat());
                }
            }

            energyAddTimeList.Clear();
            energyAddNumList.Clear();

            //加能量时间点
            if (skillSetting.EnergyAddTimeArr != null)
            {
                string[] energyAddTimeArr = skillSetting.EnergyAddTimeArr.Split('|');
                for (int i = 0; i < energyAddTimeArr.Length; i++)
                {
                    energyAddTimeList.Add(energyAddTimeArr[i].ToFloat());
                }
                //加多少能量
                string[] energyAddArr = skillSetting.EnergyAddArr.Split('|');
                for (int i = 0; i < energyAddArr.Length; i++)
                {
                    energyAddNumList.Add(energyAddArr[i].ToFloat());
                }
            }

            finishedEnergyAdd = -1;
            finishedAllAttack = false;
            if (role.peopleData.gender != (int)Gender.None)
            {
                attackAnimHouZhui = RandomManager.Next(1, 4).ToString();
            }
            else
            {
                attackAnimHouZhui = "";
            }
            if (role.roleAnim.ske.SkeletonData.FindAnimation("qianyao" + attackAnimHouZhui) == null)
                attackAnimHouZhui = "";

            role.roleAnim.Play("qianyao"+ attackAnimHouZhui, false);

            if (skillIndex > 0)
                EventCenter.Broadcast(TheEventType.BlackBattleScene);
            //其它技能cd--
            //for(int i = 0; i < role.PeopleData.AllSkillData.EquippedSkillIdList.Count; i++)
            //{
            //    SingleSkillData data = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(role.PeopleData.AllSkillData.EquippedSkillIdList[i],
            //        role.PeopleData.AllSkillData);
            //    if (data.Cd > 0)
            //        data.Cd--;
            //}
            BattleManager.Instance.DeSkillCD(role.peopleData);
            //技能cd
            if (!string.IsNullOrWhiteSpace(skillSetting.Cd))
            {
                role.curChoosedSkill.cd = skillSetting.Cd.ToInt32();
            }
            if (role.peopleData.isMyTeam)
                EventCenter.Broadcast(TheEventType.SkillAttack);
            //放技能 乙木长生
            if (skillIndex > 0)
            {
                List<EquipTaoZhuangType> p1TaoZhuangList = EquipmentManager.Instance.CheckEquipTaoZhuang(role.peopleData);

                //4件套
                if (p1TaoZhuangList.Count >= 2
                        && p1TaoZhuangList[0] == p1TaoZhuangList[1])
                {
                    //乙木长生 加血
                    if (p1TaoZhuangList[0] == EquipTaoZhuangType.YiMu)
                    {
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)p1TaoZhuangList[0]);
                        int hpNum =(int)MathF.Round(taoZhuangSetting.Param2.ToFloat() * 0.01f * RoleManager.Instance.GetCurBattlePropertyNum(PropertyIdType.Hp,role.peopleData));
                        BattleManager.Instance.AddHP(role.peopleData, hpNum);
                    }   
                    //洪荒 上buff
                    else if (p1TaoZhuangList[0] == EquipTaoZhuangType.HongHuang)
                    {
                        EquipTaoZhuangSetting taoZhuangSetting = DataTable.FindEquipTaoZhuangSetting((int)p1TaoZhuangList[0]);
                        // 使用 SplitCfgStringOneDepth 保留原始字符串格式
                        List<string> buffIdStrList = CommonUtil.SplitCfgStringOneDepth(taoZhuangSetting.Param2);
                        if (buffIdStrList.Count > 0 && !string.IsNullOrEmpty(buffIdStrList[0]))
                            BattleManager.Instance.AddBattleBuff(role.peopleData, DataTable.FindBattleBuffSetting(buffIdStrList[0]));
                        if (buffIdStrList.Count > 1 && !string.IsNullOrEmpty(buffIdStrList[1]))
                            BattleManager.Instance.AddBattleBuff(role.peopleData, DataTable.FindBattleBuffSetting(buffIdStrList[1]));
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void Update(float deltaTime)
    {
        if (startSkillForeRoll)
        {
            skillForeRollTimer += deltaTime;
            if (skillForeRollTimer >= skillForeRollTime)
            {
                //前摇结束
                startSkillCenter = true;
                startSkillForeRoll = false;
               // Debug.Log(role.peopleData.name + "在RoleAttackState中，由于前摇结束，startSkillForeRoll值为" + startSkillForeRoll);
               // Debug.Log(role.peopleData.name + "在RoleAttackState中，由于前摇结束，startSkillCenter值为" + startSkillCenter);

                BattleManager.Instance.ShowBattleEffect(role.peopleData, role.curChoosedSkill);
                role.roleAnim.Play("sheji"+attackAnimHouZhui, false);
                //彼岸炽魂
                if (skillSetting.Id.ToInt32() == (int)SkillIdType.BiAnChiHun)
                {
                    int hpNum = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, role.peopleData).num;
                    int deHpNum = (int)(hpNum * 0.3f);
                    RoleManager.Instance.DeBattleProperty(PropertyIdType.Hp, -deHpNum);
                    EventCenter.Broadcast(TheEventType.AddBattleHp, role.peopleData, -deHpNum);

                }

                //加buff
                if (!string.IsNullOrWhiteSpace(skillSetting.BeforeAddBuff))
                {
                    List<int> buffIdList = CommonUtil.SplitCfgOneDepth(skillSetting.BeforeAddBuff);
                    for(int i = 0; i < buffIdList.Count; i++)
                    {
                        BattleManager.Instance.AddBattleBuff(role.peopleData, DataTable.FindBattleBuffSetting(buffIdList[i]));
                    }
                }
            }
        }
        if (startSkillCenter)
        {
            skillCenterTimer += deltaTime;
            //技能结束了
            if (skillCenterTimer >= skillCenterTime)
            {


                //有buff 则触发buff
                if (!string.IsNullOrWhiteSpace(skillSetting.HaveBuff))
                {

                    List<int> buffIdList = CommonUtil.SplitCfgOneDepth(skillSetting.Param);
                    List<int> buffNumList = CommonUtil.SplitCfgOneDepth(skillSetting.HaveBuff);
                    for(int i = 0; i < buffNumList.Count; i++)
                    {
                        int buffId = buffIdList[i];
                        int buffNum = buffNumList[i];
                        BattleBuffSetting buffSetting = DataTable.FindBattleBuffSetting(buffId);
                        PeopleData buffedP = null;
                        if (buffSetting.ToSelf == "1")
                        {
                            buffedP = role.peopleData;
                        }
                        else
                        {
                            buffedP = BattleManager.Instance.FindBattlePeople(role.enemyOnlyId);
                        }
                        BattleManager.Instance.AddBattleBuff(buffedP, buffSetting, role.curChoosedSkill);
                    }
                    //int buffNum = skillSetting.haveBuff.ToInt32();
                    //for(int i = 0; i < buffNum; i++)
                    //{
                    //    BattleManager.Instance.AddBattleBuff(buffedP, buffSetting, role.curChoosedSkill);

                    //}
                }
                //驱散
                if (skillSetting.SkillType.ToInt32() == (int)SkillType.QuSan)
                {
                    //回血
                    List<SkillUpgradeSetting> skillUpgradeSettingList = BattleManager.Instance.FindSkillUpgradeListBySkillId(role.curChoosedSkill.skillId);
                    SkillUpgradeSetting choosedSetting = skillUpgradeSettingList[role.curChoosedSkill.skillLevel - 1];
                    List<float> param = CommonUtil.SplitCfgFloatOneDepth(choosedSetting.Param2);
                    //加血
                    int num =(int)MathF.Round( param[0]);
                    num += Mathf.RoundToInt(RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, role.peopleData).limit * param[1] * 0.01f);
                    //全队回血
                    BattleManager.Instance.AddHP(role.peopleData, num);

                    List<BattleBuff> handleBuffList = null;
                    if (BattleManager.Instance.CheckIfLeftP(role.peopleData))
                    {
                        handleBuffList = BattleManager.Instance.buffList1;
                    }
                    else
                    {
                        handleBuffList = BattleManager.Instance.buffList2;
                    }
                    for (int i = handleBuffList.Count - 1; i >= 0; i--)
                    {
                        BattleBuff buff = handleBuffList[i];
                        if (buff.buffSetting.Bad == "1"&&buff.buffSetting.CanQu=="1")
                        {
                            BattleManager.Instance.RemoveBuff(role.peopleData, buff);
                        }
                    }
                  
                    //RoleManager.Instance.AddBattleProperty(PropertyIdType.Hp, num, role.PeopleData);
                }
                if (role.curChoosedSkill.skillId == (int)SkillIdType.ShiShenShouWei)
                {
                    BattleManager.Instance.AddMP(role.peopleData, 100);
                }
                //三花聚顶决
                if (role.curChoosedSkill.skillId == (int)SkillIdType.SanHuaJuDingJue)
                {
                    skillSetting = BattleManager.Instance.FindSkillSetting(role.curChoosedSkill.skillId);
                    List<SkillUpgradeSetting> skillUpgradeList = BattleManager.Instance.FindSkillUpgradeListBySkillId(role.curChoosedSkill.skillId);
                    float param = skillUpgradeList[role.curChoosedSkill.skillLevel - 1].Param2.ToFloat();

                    BattleManager.Instance.AddMP(role.peopleData, param);

                }
                //罡气冲击
                if (role.curChoosedSkill.skillId == (int)SkillIdType.GangQiChongJi)
                {
                    bool crit = false;
                    for(int i = 0; i < thisAttackResList.Count; i++)
                    {
                        AttackResData theRes = thisAttackResList[i];
                        if (theRes!=null&& theRes.ifCrit)
                        {
                            crit = true;
                            PeopleData buffedP = BattleManager.Instance.FindBattlePeople(role.enemyOnlyId);
                            BattleManager.Instance.AddBattleBuff(buffedP, DataTable.FindBattleBuffSetting((int)BattleBuffIdType.DingShen));
                            break;
                        }
                    }

                }
                bool ultraWait = false;
                int ultraHurt = 0;
                //堕仙剑 额外伤害
                if (skillSetting.HaveAttack=="1"
                    &&role.peopleData.curEquipItemList[0]!=null
                    &&role.peopleData.curEquipItemList[0].settingId == (int)EquipIdType.DuoXianJian)
                {
                    EquipmentSetting setting = DataTable.FindEquipSetting(role.peopleData.curEquipItemList[0].settingId);
                    List<List<int>> equipParam = CommonUtil.SplitCfg(setting.Param);
                    List<int> equipParam2 = CommonUtil.SplitCfgOneDepth(setting.Param2);
                    int randomIndex = RandomManager.Next(0, 100);
                    if (randomIndex < equipParam[0][0])
                    {
                        ultraWait = true;
                        ultraHurt = equipParam[1][0];
                        BattleManager.Instance.AddBattleBuff(role.peopleData, DataTable.FindBattleBuffSetting(equipParam2[0]));
                    }
                   
                }
               
         

                //火魂
                BattleBuff huoHunBuff = BattleManager.Instance.FindBuffTypeBuff(role.peopleData, BattleBuffType.HuoHun);
                if (skillSetting.HaveAttack == "1"
                    && huoHunBuff != null)
                {
                    BattleBuffSetting setting = huoHunBuff.buffSetting;
                    PeopleData enemy = BattleManager.Instance.FindBattlePeople(role.enemyOnlyId);
                    int skillLevel = huoHunBuff.skillData.skillLevel;
                    BattleBuffSetting huoYinBuff = DataTable.FindBattleBuffSetting((int)BattleBuffIdType.HuoYin);
                    BattleManager.Instance.AddBattleBuff(enemy, huoYinBuff, huoHunBuff.skillData);
                }
                //结束技能
                role.endAttack = true;
                    role.curChoosedSkill = null;
                    startSkillCenter = false;

                    //切人
                    if (!BattleManager.Instance.CheckIfLeftP(role.peopleData)
                        && BattleManager.Instance.p2List.Count > 1
                        &&BattleManager.Instance.curBattleType==BattleType.Match)
                    {
                    
                        //是否放大切人
                        if (BattleManager.Instance.DetermineIfEnemyQie(QieRenPurposeType.FangDa) != -1)
                        {
                            role.readyToQieRen = true;
                            role.qieRenIndex = BattleManager.Instance.DetermineIfEnemyQie(QieRenPurposeType.FangDa);
                        }
                        
                        //是否因为元素反应切人
                        else if (BattleManager.Instance.DetermineIfEnemyQie(QieRenPurposeType.YuanSuReaction) != -1)
                        {
                            role.readyToQieRen = true;
                            role.qieRenIndex = BattleManager.Instance.DetermineIfEnemyQie(QieRenPurposeType.YuanSuReaction);
                        }
                        //role.readyToQieRen = false;
                    }

                    if (!BattleManager.Instance.CheckIfXieZhan(role.peopleData))
                    {
                        //判断对方是否死了
                        BattleManager.Instance.JudgeIfPeopleDead();
                    }
                
               
                //Debug.Log(role.peopleData.name + "在RoleAttackState中，由于技能结束，startSkillCenter值为" + startSkillCenter);

            }
            else
            {
                if (skillSetting.HaveAttack == "1")
                {
                    //特定时间节点造成杀伤
                    if (!finishedAllAttack)
                    {
                        int index = CommonUtil.GetPhaseIndex(skillCenterTimer, skillDamageTimeList);
                        if (index > finishedDamage)
                        {
                            finishedDamage = index;
                            AttackResData resData= BattleManager.Instance.SingleAttack(role.peopleData, role.curChoosedSkill, index);
                            thisAttackResList.Add(resData);
                        }
                        //该技能所有伤害都打完了
                        if (index >= skillDamageTimeList.Count - 1)
                        {
                            finishedAllAttack = true;
                        }

                        ///可以攒能量
                        if (energyAddTimeList.Count > 0)
                        {
                            int energyIndex = CommonUtil.GetPhaseIndex(skillCenterTimer, energyAddTimeList);

                            if (energyIndex > finishedEnergyAdd)
                            {
                                finishedEnergyAdd = energyIndex;
                                float energyNum = energyAddNumList[energyIndex];
                                //energyNum *= (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MPSpeed, role.PeopleData).Num * 0.01f + 1);
                                BattleManager.Instance.AddMP(role.peopleData, energyNum);

                                if (role.peopleData.allSkillData.equippedSkillIdList.Count > 1)
                                {
                                    //不是测试场景
                                    if (RoleManager.Instance._CurGameInfo.SceneData != null)
                                    {

 

                                    }
                                    


                                }

                            }

                        }
                    }
                }

            }
        }

        //Debug.Log(role.PeopleData.Name+ "在RoleAttackState中，startSkillForeRoll值为" + startSkillForeRoll);
        //Debug.Log(role.PeopleData.Name + "在RoleAttackState中，startSkillCenter值为" + startSkillCenter);

    }










    public void OnExit()
    {
        //Debug.Log("结束攻击");
        if (!BattleManager.Instance.someOndeDead)
            BattleManager.Instance.RemoveLogicPause();


        //如果正在助战，则把事情放到助战后面做
        if (BattleManager.Instance.curZhuZhanPList.Count > 0)
        {

        }
        else
        {
            //buff时间--
            BattleManager.Instance.DeBattleBuffRemainHuiHe(role.peopleData);
            //携战cd--
            BattleManager.Instance.DeXieZhanRemainHuiHe(role.peopleData);
            BattleManager.Instance.DeSpecialRemainHuiHe(role.peopleData);

        }


        //技能cd--


        //熄灭qte
        EventCenter.Broadcast(TheEventType.DisappearQTE);

 
    }
}
