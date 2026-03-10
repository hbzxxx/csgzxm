
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoleIdleState : IState
{
    public RoleAnim roleAnim;
    public BattlePeopleView role;

    public float attackCD = 0;

    public RoleIdleState(BattlePeopleView role)
    {
        this.role = role;
        this.roleAnim = role.roleAnim;
    }

    public void OnEnter()
    {
//        //选择技能
//#if UNITY_EDITOR
//        if (SceneManager.GetActiveScene().name == "SkillEffect")
//        {
//            role.curChoosedSkill = RoleManager.Instance._CurGameInfo.playerPeople.AllSkillData.SkillList[0];

//        }
//#endif
        //Debug.Log("进入idle attackcdtimer为"+role.peopleData.name+ role.attackCDTimer);
        this.roleAnim.Play("daiji");
        float speedAdd = 0;
        //神速
        List<BattleBuff> shenSuBuff = BattleManager.Instance.FindBuffTypeBuffList(role.peopleData, BattleBuffType.ShenSu);
        if (shenSuBuff.Count>0)
        {
            for(int i = shenSuBuff.Count - 1; i >= 0; i--)
            {
                speedAdd = shenSuBuff[i].speedAddRate * 0.01f;
            }
        }
        //振奋
        List<BattleBuff> zhenFenBuff = BattleManager.Instance.FindBuffTypeBuffList(role.peopleData, BattleBuffType.ZhenFen);
        if (zhenFenBuff.Count>0)
        {
            for(int i = zhenFenBuff.Count - 1; i >= 0; i--)
            {
                speedAdd += zhenFenBuff[i].speedAddRate * 0.01f;

            }
        }
        List <BattleBuff> jianSuBuff = BattleManager.Instance.FindBuffTypeBuffList(role.peopleData, BattleBuffType.JianSu);
        if (jianSuBuff.Count>0)
        {
            for(int i = jianSuBuff.Count - 1; i >= 0; i--)
            {
                speedAdd += jianSuBuff[i].speedAddRate * 0.01f;
            }
        }
        if (role.peopleData.isMyTeam)
        {
            attackCD = 0.01f * 90/(1+speedAdd);
        }
        else
        {
            attackCD = 0.01f * (100) / (1 + speedAdd);

        }
       
     

        if (role.readyToEscape)
        {
            BattleManager.Instance.PlayEscapeAnim(role.peopleData.onlyId);
        }
        else if (role.readyToBig)
        {
            role.attackCDTimer = attackCD;
        }
        else if (role.readyToQieRen)
        {
            bool left = true;
            if (BattleManager.Instance.CheckIfLeftP(role.peopleData))
            {
                left = true;
            }
            else
            {
                left = false;
            }
            BattleManager.Instance.QieRen(left, role.qieRenIndex,false);
            role.readyToQieRen = false;
        }
        else
        {
            role.attackCDAccomplished = false;

        }



        //attackCDTimer = 0;
    }

    public void Update(float deltaTime)
    {
        if (!BattleManager.Instance.logicPause)
        {
        

            if (role.readyToEscape)
            {
                BattleManager.Instance.PlayEscapeAnim(role.peopleData.onlyId);
            }
            else if (role.readyToBig)
            {
                role.attackCDTimer = attackCD;
            }
            else if (role.readyToQieRen)
            {
                BattleManager.Instance.QieRen(true, role.qieRenIndex,false);
                role.readyToQieRen = false;
            }
            else
            {
                role.attackCDAccomplished = false;

            }


            role.attackCDTimer += deltaTime;
            EventCenter.Broadcast(TheEventType.AttackCDShow, role.peopleData.onlyId, role.attackCDTimer, attackCD);

            if (role.attackCDTimer >= attackCD)
            {
                role.attackCDAccomplished = true;

            }

        }

    }




    public void OnExit()
    {

    }

}
