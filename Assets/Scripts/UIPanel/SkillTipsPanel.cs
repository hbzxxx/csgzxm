 using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class SkillTipsPanel : PanelBase
{
    public SkillSetting skillSetting;
    public SkillUpgradeSetting upgradeSetting;

    public Text txt_skillName;
    public Text txt_des;
    public Text txt_functionDes;

    public override void Init(params object[] args)
    {
        base.Init(args);
        skillSetting = args[0] as SkillSetting;
        int level = (int)args[1];
        List<SkillUpgradeSetting> upgradeList = DataTable.FindSkillUpgradeListBySkillId(skillSetting.Id.ToInt32());
        int showLevel = level - 1;
        if (showLevel < 0)
            showLevel = 0;
        if (upgradeList.Count > 0)
        {
            upgradeSetting = upgradeList[showLevel];

        }
        else
        {
            upgradeSetting = null;
        }
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_skillName.SetText(skillSetting.Name);
        txt_des.SetText(skillSetting.Des);
        if (upgradeSetting != null)
            txt_functionDes.SetText(SkillManager.Instance.ShowSkillFunctionDes(upgradeSetting));
        else
            txt_functionDes.SetText("");
    }
}
