using DG.Tweening;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 角色被打状态
/// </summary>
public class RoleBeHitState : IState
{
    public RoleAnim anim;
   

    public BattlePeopleView role;

    public float hitTimer=0;
    //public float hitTime = 0.2f;//被打持续时间
    public RoleBeHitState(BattlePeopleView role)
    {
        this.role = role;
        this.anim = role.roleAnim;
    }
    /// <summary>
    /// 进入被打
    /// </summary>
    public void OnEnter()
    {

        hitTimer = 0;
        string houZhui = RandomManager.Next(1, 4).ToString();
        if (anim.ske.SkeletonData.FindAnimation("beida" + houZhui) == null)
            houZhui = "";
        //TimerManager.Instance.AddTimerTask(0.2f, OnEndHit,"角色结束被打");
        anim.Play("beida"+houZhui,false);

    }

    /// <summary>
    /// 结束被打
    /// </summary>
    void OnEndHit()
    {

        role.beHit = false;

    }
 

    public void OnExit()
    {
      
    }

    /// <summary>
    /// 刷新硬直时间
    /// </summary>
    public void RefreshHitRecover()
    {
        hitTimer = 0;
    }




    public void Update(float deltaTime)
    {
        hitTimer += deltaTime;
        if (hitTimer >= role.curBeHitCD)
        {
            OnEndHit();
        }
    }

   
}
