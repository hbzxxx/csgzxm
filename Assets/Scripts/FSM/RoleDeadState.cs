using DG.Tweening;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 角色死亡状态
/// </summary>
public class RoleDeadState : IState
{
    public RoleAnim anim;
   

    public BattlePeopleView role;

    public float deadAnimTimer=0;
    public float deadAnimTime = 0.2f;//死亡动画时间
    public RoleDeadState(BattlePeopleView role)
    {
        this.role = role;
        this.anim = role.roleAnim;
    }
    /// <summary>
    /// 进入被打
    /// </summary>
    public void OnEnter()
    {
        role.xuanYun = false;
        deadAnimTimer = 0;
        //TimerManager.Instance.AddTimerTask(0.2f, OnEndHit,"角色结束被打");
        float offset = 60;
        if (role.left)
            offset *= -1;
        role.transform.DOKill();
        role.transform.DOLocalMoveX(role.transform.localPosition.x + offset, deadAnimTime);
        if (role.img != null)
        {
            role.img.DOKill();
            role.img.DOFade(0, deadAnimTime);
        }else if (role.roleAnim.ske != null)
        {
            role.roleAnim.ske.DOFade(0, deadAnimTime);
        }

    }

    /// <summary>
    /// 结束
    /// </summary>
    void OnEndDead()
    {

       // role.beHit = false;

    }
 

    public void OnExit()
    {
      
    }





    public void Update(float deltaTime)
    {
        deadAnimTimer += deltaTime;
        if (deadAnimTimer >= deadAnimTime)
        {
            OnEndDead();
        }
    }

   
}
