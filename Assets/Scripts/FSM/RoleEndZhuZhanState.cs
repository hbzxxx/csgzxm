using DG.Tweening;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 结束助战
/// </summary>
public class RoleEndZhuZhanState : IState
{
    public RoleAnim anim;
   

    public BattlePeopleView role;

    public float deadAnimTimer=0;
    public float deadAnimTime = 0.5f;//死亡动画时间
    public RoleEndZhuZhanState(BattlePeopleView role)
    {
        this.role = role;
        this.anim = role.roleAnim;
    }
    /// <summary>
    /// 进入结束助战
    /// </summary>
    public void OnEnter()
    {
        //deadAnimTimer = 0;
        //TimerManager.Instance.AddTimerTask(0.2f, OnEndHit,"角色结束被打");
        //float offset = 60;
        //if (role.left)
        //    offset *= -1;
        role.transform.DOKill();
        role.transform.DOLocalMoveY(700, .5f).OnComplete(() =>
        {
            PanelManager.Instance.CloseSingle(role);

            BattleManager.Instance.RemoveZhuZhanStudent(role.peopleData);

        });
        //role.img.DOKill();
        //role.img.DOFade(0, deadAnimTime);
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
    }
}
