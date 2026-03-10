 using DG.Tweening;
using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 切人
/// </summary>
public class QieRenStudentView : SingleStudentView
{
    BattlePanel parentPanel;
    public Transform trans_dead;
    public Image img_daZhaoCD;
    public Image img_big;//大招
    bool isMyTeam;
    public Text txt_cd;
    public Image img_hpBar;
    public Transform trans_EnemyChoosed;
    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[1] as BattlePanel;
        isMyTeam = (bool)args[2];
        addBtnListener(btn, () =>
        {
            if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, peopleData).num <= 0)
            {
                PanelManager.Instance.OpenFloatWindow("战败者无法出战");
            }
            else
            {
                //parentPanel.OnReadyToQieRen(this);

            }
        });
        RegisterEvent(TheEventType.BattlePeopleDead, OnDead);
        RegisterEvent(TheEventType.AddBattleProperty, OnBattleProChange);
        RegisterEvent(TheEventType.DeBattleProperty, OnBattleProChange);
        RegisterEvent(TheEventType.BattleBeHit, OnSkillAttack);
        RegisterEvent(TheEventType.BattlePeopleQieRen, OnQieRen);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        if (isMyTeam)
        {
            btn.enabled = true;
            transform.localScale = new Vector3(1, 1, 1);
            this.name = parentPanel.qieRenStudentViewList.Count.ToString();
            YuanSuType yuanSuType = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(peopleData.allSkillData.equippedSkillIdList[0], peopleData.allSkillData).yuanSuType;
            img_yuanSu.sprite = ConstantVal.YuanSuIcon(yuanSuType);

            if (peopleData.allSkillData.equippedSkillIdList.Count > 1)
            {
                SkillSetting skillSetting = DataTable.FindSkillSetting(peopleData.allSkillData.equippedSkillIdList[1]);
                img_big.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.skillFolderPath + skillSetting.IconName);
            }
            if (BattleManager.Instance.p1List[BattleManager.Instance.p1Index].onlyId == peopleData.onlyId)
            {
                OnChoose(true);
            }
            else
            {
                OnChoose(false);
            }
            trans_EnemyChoosed.gameObject.SetActive(false);
        }
        else
        {
            img_big.gameObject.SetActive(false);
            btn.enabled = false;
            if (BattleManager.Instance.CheckIfLeftP(peopleData))
            {
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        RefreshHPShow();
        RefreshDeadShow();
        RefreshEnergyShow();
        ShowBigButton();
        //img_bg.enabled=(false);

 
     }

    void OnQieRen()
    {
        if (!isMyTeam)
        {
            if (peopleData.onlyId == BattleManager.Instance.p2List[BattleManager.Instance.p2Index].onlyId)
            {
                trans_EnemyChoosed.gameObject.SetActive(true);
            }
            else
            {
                trans_EnemyChoosed.gameObject.SetActive(false);

            }
        }
    }

    public void OnChoose(bool choose)
    {
        if(choose)
        transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
    else
            transform.localScale = new Vector3(1, 1, 1);

    }
    void RefreshDeadShow()
    {
        if (RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, peopleData).num <= 0)
        {
            trans_dead.gameObject.SetActive(true);

        }
        else
        {
            trans_dead.gameObject.SetActive(false);

        }
    }

    void OnDead(object[] args)
    {
        ulong onlyId = (ulong)args[0];
        if(onlyId== peopleData.onlyId)
        trans_dead.gameObject.SetActive(true);
    }


    /// <summary>
    /// 能量条改变
    /// </summary>
    /// <param name="param"></param>
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

    void RefreshHPShow()
    {
        SinglePropertyData pro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.Hp, peopleData);
         img_hpBar.fillAmount = pro.num / (float)pro.limit;
    }

    void OnSkillAttack(object[] args)
    {
        //RefreshCDShow();
    }
    ///// <summary>
    ///// cd
    ///// </summary>
    //void RefreshCDShow()
    //{
    //    if (peopleData.isMyTeam && peopleData.allSkillData.equippedSkillIdList.Count == 2)
    //    {
    //        SingleSkillData skill = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(peopleData.allSkillData.equippedSkillIdList[1],
    //            peopleData.allSkillData);
    //        if (skill.cd > 0)
    //        {
    //            txt_cd.gameObject.SetActive(true);
    //            txt_cd.SetText("CD:" + skill.cd);
    //        }
    //        else
    //        {
    //            txt_cd.gameObject.SetActive(false);
    //        }
    //    }
    //    else
    //    {
    //        txt_cd.gameObject.SetActive(false);

    //    }
    //}
    void RefreshEnergyShow()
    {
        SinglePropertyData p1MPPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, peopleData);

        float beforeFill = img_daZhaoCD.fillAmount;
        img_daZhaoCD.DOFillAmount(p1MPPro.num / (float)p1MPPro.limit, .5f);
        img_daZhaoCD.DOFillAmount(p1MPPro.num / (float)p1MPPro.limit, .5f).OnComplete(() =>
        {


        });
    }
    void ShowBigButton()
    {
        if (peopleData.isMyTeam)
        {
            if (peopleData.allSkillData.equippedSkillIdList.Count > 1)
            {
                SinglePropertyData p1MPPro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum, peopleData);
                if (p1MPPro.num >= (float)p1MPPro.limit)
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

}
