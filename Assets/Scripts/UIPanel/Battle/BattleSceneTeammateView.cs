using Framework.Data;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗场景中未上阵队友视图
/// 显示在上阵队友后面，死亡时变灰
/// </summary>
public class BattleSceneTeammateView : SingleStudentView
{
    public Image img_grayMask;        // 死亡灰色遮罩
    public bool isDead;               // 是否死亡
    public bool isPlayer;             // 是否是玩家方

    public override void Init(params object[] args)
    {
        base.Init(args);
        
        isPlayer = (bool)args[1];
        isDead = false;

        RegisterEvent(TheEventType.BattlePeopleDead, OnDead);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        // 初始化灰色遮罩状态
        RefreshDeadShow();
    }

    /// <summary>
    /// 死亡事件回调
    /// </summary>
    void OnDead(object[] args)
    {
        ulong onlyId = (ulong)args[0];
        if (onlyId == peopleData.onlyId)
        {
            SetDead(true);
        }
    }

    /// <summary>
    /// 刷新死亡状态显示
    /// </summary>
    public void RefreshDeadShow()
    {
        if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, peopleData).num <= 0)
        {
            SetDead(true);
        }
        else
        {
            SetDead(false);
        }
    }

    /// <summary>
    /// 设置死亡状态（变灰）
    /// </summary>
    public void SetDead(bool dead)
    {
        isDead = dead;

        if (img_grayMask != null)
        {
            img_grayMask.gameObject.SetActive(dead);
        }

        // 头像变灰
        if (img_icon != null)
        {
            if (dead)
            {
                img_icon.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
            else
            {
                img_icon.color = Color.white;
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        isDead = false;
        isPlayer = false;
        
        // 重置头像颜色
        if (img_icon != null)
        {
            img_icon.color = Color.white;
        }
        if (img_grayMask != null)
        {
            img_grayMask.gameObject.SetActive(false);
        }
    }
}
