using cfg;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 角色助战状态
/// </summary>
public class RoleZhuZhanAttackState : IState
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

    public RoleZhuZhanAttackState(BattlePeopleView role)
    {
        this.role = role;
        
    }

 
    /// <summary>
    /// 进入攻击
    /// </summary>
    public void OnEnter()
    {
        //role.finishedAttack = false;
        role.endZhuZhanAttack = false;
        role.attackCDTimer = 0;//进入攻击时 cd归零

        isActivate = true;
        attackStatusTimer = 0;

   
            StartAttack(0);

        


        //role.UseSkill(role.skillList[role.curUseSkillIndex], null, null);

        //这里执行技能


    }

    /// <summary>
    /// 开始发招
    /// </summary>
    public void StartAttack(int skillIndex)
    {

        role.readyToBig = false;

        
        role.curChoosedSkill =SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(role.peopleData.allSkillData.equippedSkillIdList[skillIndex],role.peopleData.allSkillData);

        SkillSetting skillSetting = DataTable.FindSkillSetting(role.curChoosedSkill.skillId);


        skillForeRollTime = skillSetting.ForeRollTime.ToFloat();
        skillForeRollTimer = 0;
        startSkillForeRoll = true;

        skillCenterTime = skillSetting.CenterTime.ToFloat();
        skillCenterTimer = 0;
        startSkillCenter = false;

        string[] skillDamageTimeArr = skillSetting.DamageTimeArr.Split('|');
        skillDamageTimeList.Clear();
        finishedDamage = -1;
        for (int i = 0; i < skillDamageTimeArr.Length; i++)
        {
            skillDamageTimeList.Add(skillDamageTimeArr[i].ToFloat());
        }


        finishedAllAttack = false;
        role.roleAnim.Play("qianyao",false);

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
                BattleManager.Instance.ShowBattleEffect(role.peopleData, role.curChoosedSkill);
                role.roleAnim.Play("sheji", false);

            }
        }
        if (startSkillCenter)
        {
            skillCenterTimer += deltaTime;
            if (skillCenterTimer >= skillCenterTime)
            {
                //结束技能
                role.endZhuZhanAttack = true;
                role.curChoosedSkill = null;
                startSkillCenter = false;
            }
            else
            {
                //特定时间节点造成杀伤
                if (!finishedAllAttack)
                {
                    int index = CommonUtil.GetPhaseIndex(skillCenterTimer, skillDamageTimeList);
                    if (index > finishedDamage)
                    {
                        finishedDamage = index;
                        BattleManager.Instance.SingleAttack(role.peopleData, role.curChoosedSkill, index,true);
                    }
                    //该技能所有伤害都打完了
                    if (index >= skillDamageTimeList.Count - 1)
                    {
                        finishedAllAttack = true;
                    
                    }


                }
            }
        }



    }








    public void OnExit()
    {
        Debug.Log("结束攻击");
        //播放飞走动画
    }
}
