using DG.Tweening;
using Framework.Data;
using cfg;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BlueToolkit;

/// <summary>
/// 战斗界面底部/顶部队伍成员头像View
/// 显示角色头像、携带技能图标
/// 玩家可携带3个技能，协战队友只能携带1个技能
/// 当前战斗角色高亮显示
/// 死亡后头像变灰
/// </summary>
public class BattleTeamMemberView : SingleStudentView
{
    public Image img_highlight;       // 当前战斗高亮边框
    public Image img_grayMask;        // 死亡灰色遮罩
    public Transform trans_dead;      // 死亡标记

    public Transform trans_skillGrid; // 技能图标容器
    public List<Image> skillIconList = new List<Image>(); // 技能图标列表

    public Text txt_level;            // 等级文字
    //public Text txt_cd;               // CD文字

    //public Image img_hpBar;           // 血条
    //public Image img_daZhaoCD;        // 大招CD/能量条
    public Image img_big;             // 大招图标

    public bool isPlayer;             // 是否是玩家方
    public bool isDead;               // 是否死亡
    public bool isCurrent;            // 是否是当前战斗角色

    private BattlePanel battlePanel;

    public Transform tran_skill;
    public Text text_zhanli;

    public override void Init(params object[] args)
    {
        base.Init(args);

        peopleData = args[0] as PeopleData;
        battlePanel = args[1] as BattlePanel;
        isPlayer = (bool)args[2];

        isDead = false;
        isCurrent = false;

        // 注册事件（参考QieRenStudentView）
        RegisterEvent(TheEventType.BattlePeopleDead, OnDead);
        RegisterEvent(TheEventType.AddBattleProperty, OnBattleProChange);
        RegisterEvent(TheEventType.DeBattleProperty, OnBattleProChange);
        RegisterEvent(TheEventType.BattleBeHit, OnSkillAttack);
        //RegisterEvent(TheEventType.BattlePeopleQieRen, OnQieRen);

        // 点击切换角色
        addBtnListener(btn, () =>
        {
            if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, peopleData).num <= 0)
            {
                PanelManager.Instance.OpenFloatWindow("战败者无法出战");
            }
            else
            {
                if (isPlayer)
                    battlePanel.OnReadyToQieRen(this);
                //OnClickMember();
            }
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        if (isPlayer)
        {
            btn.enabled = true;
            //transform.localScale = Vector3.one;

            // 设置元素图标
            if (img_yuanSu != null && peopleData.allSkillData != null && peopleData.allSkillData.equippedSkillIdList.Count > 0)
            {
                YuanSuType yuanSuType = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(
                    peopleData.allSkillData.equippedSkillIdList[0], peopleData.allSkillData).yuanSuType;
                img_yuanSu.sprite = ConstantVal.YuanSuIcon(yuanSuType);
            }

            // 设置大招图标
            if (img_big != null && peopleData.allSkillData != null && peopleData.allSkillData.equippedSkillIdList.Count > 1)
            {
                SkillSetting skillSetting = DataTable.FindSkillSetting(peopleData.allSkillData.equippedSkillIdList[1]);
                if (skillSetting != null)
                {
                    img_big.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.skillFolderPath + skillSetting.IconName);
                }
            }

            // 判断是否是当前战斗角色
            if (BattleManager.Instance.p1List[BattleManager.Instance.p1Index].onlyId == peopleData.onlyId)
            {
                SetAsCurrent(true);
            }
            else
            {
                SetAsCurrent(false);
            }
        }
        else
        {
            // 敌人方
            if (img_big != null)
                img_big.gameObject.SetActive(false);
            btn.enabled = false;

            // 判断是否是当前战斗角色
            if (BattleManager.Instance.p2List[BattleManager.Instance.p2Index].onlyId == peopleData.onlyId)
            {
                SetAsCurrent(true);
            }
            else
            {
                SetAsCurrent(false);
            }
        }

        // 设置技能图标
        RefreshSkillIcons();

        // 设置等级/战斗力
        if (txt_level != null)
        {
            txt_level.SetText(RoleManager.Instance.CalcZhanDouLi(peopleData).ToString());
        }
        
        // 设置战力显示
        if (text_zhanli != null)
        {
            text_zhanli.SetText(RoleManager.Instance.CalcZhanDouLi(peopleData).ToString());
        }

        // 初始化状态
        if (img_grayMask != null)
            img_grayMask.gameObject.SetActive(false);

        // 刷新血量、能量、死亡状态
        //RefreshHPShow();
        RefreshDeadShow();
        ShowStudentKuang();
        //RefreshEnergyShow();
        ShowBigButton();
        img_bgk.gameObject.SetActive(true);
        if (!isPlayer)
            img_icon.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 180, 0);
        img_yuanSu.gameObject.SetActive(false);
    }


public override void ShowStudentKuang(){
    base.ShowStudentKuang();
    if (peopleData.isPlayer)
    {
        int rarity = peopleData.studentRarity;
        if (rarity == 0)
            rarity = 1;
        img_bg.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_dizik_" + rarity);
    }
}

    /// <summary>
    /// 刷新技能图标显示
    /// 只显示已经装备的技能（跳过普攻）
    /// 玩家主战斗角色可携带3个技能，协战队友只能携带1个技能
    /// </summary>
    void RefreshSkillIcons()
    {
        // 清空现有技能视图
        PanelManager.Instance.CloseAllSingle(tran_skill);

        // 验证技能数据
        if (peopleData == null || peopleData.allSkillData == null || peopleData.allSkillData.equippedSkillIdList == null)
            return;

        // 装备的技能列表，索引0是普攻，从索引1开始才是主动技能
        List<int> equippedSkills = peopleData.allSkillData.equippedSkillIdList;
        
        // 如果没有装备任何技能，直接返回
        if (equippedSkills.Count <= 1) // 只有普攻或没有技能
            return;

        // 计算可显示的技能数量（跳过普攻索引0）
        int availableSkillCount = equippedSkills.Count - 1;
        
        // 根据角色类型限制显示数量
        int maxDisplayCount = IsMainBattler() ? 3 : 1; // 主战斗角色3个，协战队友1个
        int skillCount = Mathf.Min(availableSkillCount, maxDisplayCount);

        // 显示已装备的技能
        for (int i = 0; i < skillCount; i++)
        {
            int skillIndex = i + 1; // 跳过普攻（索引0）
            if (skillIndex < equippedSkills.Count)
            {
                int skillId = equippedSkills[skillIndex];
                SkillSetting skillSetting = DataTable.FindSkillSetting(skillId);
                
                if (skillSetting != null)
                {
                    // 创建 BattleSkillView 显示已装备的技能
                    BattleSkillView skillView = PanelManager.Instance.OpenSingle<BattleSkillView>(
                        tran_skill, peopleData, skillSetting, SkillViewType.Show,true);
                }
            }
        }
    }

    /// <summary>
    /// 判断是否是主战斗角色（非协战）
    /// </summary>
    bool IsMainBattler()
    {
        if (isPlayer)
        {
            return BattleManager.Instance.p1List.IndexOf(peopleData) == 0;
        }
        else
        {
            return BattleManager.Instance.p2List.IndexOf(peopleData) == 0;
        }
    }

    /// <summary>
    /// 点击成员头像
    /// </summary>
    //void OnClickMember()
    //{
    //    if (isCurrent) return;

    //    // 触发切人事件
    //    if (isPlayer)
    //    {
    //        int index = BattleManager.Instance.p1List.IndexOf(peopleData);
    //        if (index >= 0)
    //        {
    //            EventCenter.Broadcast(TheEventType.ReadyToQieRen,
    //                BattleManager.Instance.p1List[BattleManager.Instance.p1Index].onlyId, index);
    //        }
    //    }
    //}

    /// <summary>
    /// 切人事件回调
    /// </summary>
    //void OnQieRen()
    //{
    //    if (isPlayer)
    //    {
    //        if (peopleData.onlyId == BattleManager.Instance.p1List[BattleManager.Instance.p1Index].onlyId)
    //        {
    //            SetAsCurrent(true);
    //        }
    //        else
    //        {
    //            SetAsCurrent(false);
    //        }
    //    }
    //    else
    //    {
    //        if (peopleData.onlyId == BattleManager.Instance.p2List[BattleManager.Instance.p2Index].onlyId)
    //        {
    //            SetAsCurrent(true);
    //        }
    //        else
    //        {
    //            SetAsCurrent(false);
    //        }
    //    }
    //}

    /// <summary>
    /// 设置为当前战斗角色（高亮显示）
    /// </summary>
    public void SetAsCurrent(bool current)
    {
        isCurrent = current;
        if (img_highlight != null)
        {
            img_highlight.gameObject.SetActive(current);
        }

        // 当前战斗角色放大显示
        //if (current)
        //{
        //    transform.localScale = new Vector3(1.3f, 1.3f, 1f);
        //}
        //else
        //{
        //    transform.localScale = Vector3.one;
        //}
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
    void RefreshDeadShow()
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

        if (trans_dead != null)
        {
            trans_dead.gameObject.SetActive(dead);
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

    /// <summary>
    /// 战斗属性改变事件回调
    /// </summary>
    void OnBattleProChange(object[] param)
    {
        PeopleData p = param[0] as PeopleData;
        PropertyIdType idType = (PropertyIdType)param[1];

        if (p.onlyId == peopleData.onlyId)
        {
            peopleData = p;
            if (idType == PropertyIdType.MpNum)
            {
                RefreshEnergyShow();
                ShowBigButton();
            }
            RefreshHPShow();
        }
    }

    /// <summary>
    /// 技能攻击事件回调
    /// </summary>
    void OnSkillAttack(object[] args)
    {
        // 可以在这里刷新CD显示
    }

    /// <summary>
    /// 刷新血量显示
    /// </summary>
    void RefreshHPShow()
    {
        //if (img_hpBar != null)
        //{
        //    SinglePropertyData pro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, peopleData);
        //    img_hpBar.fillAmount = pro.num / (float)pro.limit;
        //}
    }

    /// <summary>
    /// 刷新能量显示
    /// </summary>
    void RefreshEnergyShow()
    {
        //if (img_daZhaoCD != null)
        //{
        //    SinglePropertyData mpPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, peopleData);
        //    img_daZhaoCD.DOFillAmount(mpPro.num / (float)mpPro.limit, .5f);
        //}
    }

    /// <summary>
    /// 显示大招按钮
    /// </summary>
    void ShowBigButton()
    {
        if (img_big == null) return;

        if (peopleData.isMyTeam)
        {
            if (peopleData.allSkillData.equippedSkillIdList.Count > 1)
            {
                SinglePropertyData mpPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, peopleData);
                if (mpPro.num >= (float)mpPro.limit)
                {
                    img_big.gameObject.SetActive(true);
                }
                else
                {
                    img_big.gameObject.SetActive(false);
                }
            }
            else
            {
                img_big.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 刷新CD显示
    /// </summary>
    public void RefreshCD(float cdTimer, float cdTime)
    {
        //if (txt_cd != null)
        //{
        //    if (cdTime > 0)
        //    {
        //        int remainCD = Mathf.CeilToInt(cdTime - cdTimer);
        //        txt_cd.text = remainCD.ToString();
        //    }
        //    else
        //    {
        //        txt_cd.text = "";
        //    }
        //}
    }

    public override void Clear()
    {
        base.Clear();
        
        // 清理技能视图
        if (tran_skill != null)
        {
            PanelManager.Instance.CloseAllSingle(tran_skill);
        }
        
        battlePanel = null;
        isDead = false;
        isCurrent = false;
    }
}
