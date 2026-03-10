using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class SingleSkillView : SingleViewBase
{
    public Image icon;
    public SingleSkillData singleSkillData;
    public SkillSetting setting;
    public SkillViewType skillViewType;

    public Button btn;
    public SkillPanel skillPanel;

    //public GameObject obj_levelTag;
    public Text txt_levelTag;
    public Text txt_lv;
    //public GameObject obj_equipTag;//装配中
    public override void Init(params object[] args)
    {
        base.Init(args);
        singleSkillData = args[0] as SingleSkillData;
        int id = singleSkillData.skillId;
        skillViewType = (SkillViewType)args[1];
        setting = DataTable.FindSkillSetting(id);
        icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.skillFolderPath + setting.IconName);

   

        switch (skillViewType)
        {
            case SkillViewType.EquippedSkill:
                skillPanel = args[2] as SkillPanel;
                addBtnListener(btn, () =>
                {
                    skillPanel.ShowUpgradePanel(singleSkillData);
                });
                break;
            default:
                addBtnListener(btn, () =>
                {
                    PanelManager.Instance.OpenPanel<SkillTipsPanel>(PanelManager.Instance.trans_layer2, setting, singleSkillData.skillLevel);
                });
                break;
        }

        RegisterEvent(TheEventType.OnEquipSkill, OnEquipped);
        RegisterEvent(TheEventType.OnUnEquipSkill, OnUnEquipped);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ShowStatus();
        txt_lv.SetText("lv." + singleSkillData.skillLevel);
    }
    /// <summary>
    /// 显示状态
    /// </summary>
    void ShowStatus()
    {
        //if (singleSkillData.SkillLevel == 0)
        //{
        //    obj_levelTag.gameObject.SetActive(false);
        //    //if()
        //    obj_equipTag.gameObject.SetActive(true);
        //}
        //else
        //{
        //    ShowLevelTag();
        //    if (singleSkillData.IsEquipped)
        //    {
        //        if(skillViewType== SkillViewType.SkillKnapsack)
        //            obj_equipTag.SetActive(true);
        //        else
        //            obj_equipTag.SetActive(false);

        //    }
        //    else
        //    {
        //        obj_equipTag.SetActive(false);

        //    }
        //}

    }

    /// <summary>
    /// 装备
    /// </summary>
    /// <param name="args"></param>
    void OnEquipped(object[] args)
    {
        SingleSkillData data = args[0] as SingleSkillData;
        if (data.skillId == singleSkillData.skillId)
        {
            singleSkillData = data;

            OnOpenIng();
        }
    }
    /// <summary>
    /// 卸下
    /// </summary>
    /// <param name="args"></param>
    void OnUnEquipped(object[] args)
    {
        SingleSkillData data = args[0] as SingleSkillData;
        if (data.skillId == singleSkillData.skillId)
        {
            singleSkillData = data;

            OnOpenIng();
        }
    }

    //显示等级标签
    void ShowLevelTag()
    {
       string str= SkillManager.Instance.LevelTagStr(singleSkillData.skillLevel);
        txt_levelTag.SetText(str);
    }

}

public enum SkillViewType
{
    None=0,
    SkillKnapsack,//技能背包
    EquippedSkill,//已装备的技能
    Show,//光显示 做成自适应弹窗
 }
