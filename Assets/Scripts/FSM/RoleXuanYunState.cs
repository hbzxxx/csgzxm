using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleXuanYunState : IState
{
    public RoleAnim anim;


    public BattlePeopleView role;
    public float cd;
    public RoleXuanYunState(BattlePeopleView role)
    {
        this.role = role;
        this.anim = role.roleAnim;
    }
    /// <summary>
    /// 进入眩晕
    /// </summary>
    public void OnEnter()
    {
        if (role.peopleData.isMyTeam)
        {
            cd = 0.01f * 90;
        }
        else
        {
            cd = 0.01f * (100);

        }
        string houZhui = RandomManager.Next(1, 4).ToString();
        if (anim.ske.SkeletonData.FindAnimation("beida" + houZhui) == null)
            houZhui = "";
        anim.Play("beida"+houZhui, false);

    }

    /// <summary>
    /// 结束眩晕
    /// </summary>
    void OnEndXuanYun()
    {
        BattleManager.Instance.DeBattleBuffRemainHuiHe(role.peopleData);
        BattleManager.Instance.DeXieZhanRemainHuiHe(role.peopleData);
        BattleManager.Instance.DeSpecialRemainHuiHe(role.peopleData);


    }

    public void OnExit()
    {

    }

    /// <summary>
    /// 刷新眩晕时间
    /// </summary>
    public void RefreshXuanYunRecover()
    {
        role.attackCDTimer = 0;
    }




    public void Update(float deltaTime)
    {
        if (!BattleManager.Instance.logicPause)
        {
            role.attackCDTimer += deltaTime;
            EventCenter.Broadcast(TheEventType.AttackCDShow, role.peopleData.onlyId, role.attackCDTimer, cd);

            if (role.attackCDTimer >= cd)
            {
                OnEndXuanYun();
            }
        }
       
    }

  
}
