using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using UnityEngine.UI;
using Framework.Data;

public class BigSkillView : SingleViewBase
{
    public Transform trans_skillGrid;
    public SingleSkillData skillData;
    public SkillSetting skillSetting;
    public Button btn;
    public SkillPanel skillPanel;
    public Text txt_name;
    public Text txt_des;
    public Image img_bg;
    //public Button btn_lock;//未解锁

    public Button btn_detail;

    public GameObject obj_equipped;//已装备

    public GameObject obj_redPoint;
    public Image img_yuanSu;//元素

    public Transform trans_studyConsumeGrid;
    bool isMySkill;
    public Button btn_study;
    public Button btn_upgrade ;
    public Button btn_equip ;
    public Button btn_unEquip;
    public GameObject obj_upgradeRedPoint;
    public override void Init(params object[] args)
    {
        base.Init(args);
        skillData = args[0] as SingleSkillData;
        skillPanel = args[1] as SkillPanel;
        skillSetting = DataTable.FindSkillSetting(skillData.skillId);
        addBtnListener(btn, () =>
        {
            skillPanel.OnChoosedSkill(skillData);
        });
        addBtnListener(btn_detail, () =>
        {
            PanelManager.Instance.OpenPanel<SkillTipsPanel>(PanelManager.Instance.trans_layer2, skillSetting, skillData.skillLevel);
        });
        addBtnListener(btn_upgrade, () =>
        {
            skillPanel.ShowUpgradePanel(skillData);
        });
        addBtnListener(btn_equip, () =>
        {

            if (skillData != null)
            {
                PanelManager.Instance.CloseTaskGuidePanel();
                if (skillPanel.p.allSkillData.equippedSkillIdList.Count >= 4)
                {
                    
                    PanelManager.Instance.OpenFloatWindow(LanguageUtil.GetLanguageText((int)LanguageIdType.不能再携带更多功法了));
                    return;
                }
                SkillManager.Instance.EquipSkill(skillPanel.p, skillData);

            }
        });
        addBtnListener(btn_unEquip, () =>
        {

            if (skillData != null)
            {
                PanelManager.Instance.CloseTaskGuidePanel();

                SkillManager.Instance.UnEquipSkill(skillPanel.p, skillData);

            }
        });

        addBtnListener(btn_study, () =>
        {
            SkillManager.Instance.OnStudySkill(skillData.skillId,skillPanel.p);
            PanelManager.Instance.CloseTaskGuidePanel();
            TaskManager.Instance.TryAccomplishAllTask();
        });

        RegisterEvent(TheEventType.RefreshSkillRedPoint, RefreshRedPoint);
        RegisterEvent(TheEventType.OnEquipSkill, OnEquipSkill);
        RegisterEvent(TheEventType.OnUnEquipSkill, OnUnEquipSkill);
        RegisterEvent(TheEventType.StudySkill, OnStudySkill);


        RegisterEvent(TheEventType.SkillUpgrade, OnUpgradeSkill);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();


        RefreshShow();


    }

    public void RefreshShow()
    {


        PanelManager.Instance.CloseAllSingle(trans_skillGrid);

        PanelManager.Instance.OpenSingle<SingleSkillView>(trans_skillGrid, skillData, SkillViewType.SkillKnapsack);
        txt_name.SetText(skillSetting.Name);
        txt_des.SetText(skillSetting.Des);

        isMySkill = false;
        //是不是我的技能
        for (int i = 0; i < skillPanel.p.allSkillData.skillList.Count; i++)
        {
            if (skillData.skillId == skillPanel.p.allSkillData.skillList[i].skillId)
            {
                isMySkill = true;
                skillData = skillPanel.p.allSkillData.skillList[i];
                skillData.show = true;
                SkillManager.Instance.RefreshRedPointShow(skillData.skillId);
                break;
            }
        }
        //if (isMySkill&&skillSetting.yuanSu.ToInt32()==RoleManager.Instance._CurGameInfo.playerPeople.yuanSu)
        //{
        //    btn_lock.gameObject.SetActive(false);
        //}
        //else if(isMySkill&& skillSetting.yuanSu.ToInt32() != RoleManager.Instance._CurGameInfo.playerPeople.yuanSu)
        //{
        //    btn_lock.gameObject.SetActive(true);
        //    btn_lock.GetComponentInChildren<Text>().SetText("非当前属性");
        //}
        //else
        //{
        //    btn_lock.gameObject.SetActive(true);
        //    btn_lock.GetComponentInChildren<Text>().SetText("未习得");
        //}

        if (skillData.isEquipped)
        {
            obj_equipped.SetActive(true);
        }
        else
        {
            obj_equipped.SetActive(false);
        }

        img_yuanSu.sprite = ConstantVal.YuanSuIcon((YuanSuType)skillSetting.YuanSu.ToInt32());
        RefreshRedPoint();
        ShowBtns();
    }

    void ShowGuide()
    {
   
    }
    void RefreshRedPoint()
    {
        RedPointManager.Instance.SetRedPointUI(obj_redPoint, RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill, skillData.skillId);
        RefreshUpgradeRedPoint();
    }
    /// <summary>
    /// 显示 按钮
    /// </summary>
    void ShowBtns()
    {
        PanelManager.Instance.CloseAllSingle(trans_studyConsumeGrid);
        if (skillData.skillLevel <= 0)
        {
            btn_upgrade.gameObject.SetActive(false);
            btn_equip.gameObject.SetActive(false);
            btn_unEquip.gameObject.SetActive(false);
            btn_study.gameObject.SetActive(true);

            ItemData consume = new ItemData();
            consume.settingId = skillData.skillId;
            consume.count = 1;
            PanelManager.Instance.OpenSingle<SingleConsumeJineng>(trans_studyConsumeGrid, (int)consume.settingId, (int)(ulong)consume.count, ConsumeType.Item);

        }
        else
        {
            btn_study.gameObject.SetActive(false);
            btn_upgrade.gameObject.SetActive(true);

            if (!skillData.isEquipped)
            {
                btn_equip.gameObject.SetActive(true);
                btn_unEquip.gameObject.SetActive(false);
            }
            else
            {
                btn_equip.gameObject.SetActive(false);
                btn_unEquip.gameObject.SetActive(true);
            }
            skillData.show = true;
            SkillManager.Instance.RefreshRedPointShow(skillData.skillId);
            RefreshUpgradeRedPoint();
        }
        //if (isMySkill)
        //{
        //    btn_study.gameObject.SetActive(false);
        //    btn_upgrade.gameObject.SetActive(false);
        //    btn_equip.gameObject.SetActive(false);
        //    btn_unEquip.gameObject.SetActive(false);
        //}
        //else
        //{


        //}
    }

    /// <summary>
    /// 装配技能
    /// </summary>
    /// <param name="args"></param>
    void OnEquipSkill(object[] args)
    {
        RefreshShow();
    }

    /// <summary>
    /// 卸下装配技能
    /// </summary>
    void OnUnEquipSkill(object[] args)
    {
        RefreshShow();
    }

    void RefreshUpgradeRedPoint()
    {
        if (skillData != null)
        {
            RedPointManager.Instance.SetRedPointUI(obj_upgradeRedPoint, RedPointType.MainPanel_Btn_Knapsack_SkillTag_Mai_Skill_Upgrade, skillData.skillId);

        }
    }

    /// <summary>
    /// 技能升级了
    /// </summary>
    /// <param name="args"></param>
    void OnUpgradeSkill(object[] args)
    {
        RefreshShow();
    }
    void OnStudySkill(object[] args)
    {
        RefreshShow();

    }
    public void OnChoose(bool choose)
    {
        if (choose)
            img_bg.color = ConstantVal.color_choosed;
        else
            img_bg.color = Color.white;

    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_skillGrid);
        PanelManager.Instance.CloseAllSingle(trans_studyConsumeGrid);

    }
}
