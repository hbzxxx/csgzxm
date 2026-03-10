using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamPrepareStudentViewE : TeamPrepareStudentView
{
    public Text txt_shuxin;

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        RefreshShow();
        PanelManager.Instance.CloseAllSingle(trans_equip);
        PanelManager.Instance.CloseAllSingle(trans_skill);
        if (p.allSkillData.equippedSkillIdList.Count > 1)
        {
            SingleSkillData singleSkillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(p.allSkillData.equippedSkillIdList[1],
                p.allSkillData);
            SingleSkillViewE view = PanelManager.Instance.OpenSingle<SingleSkillViewE>(trans_skill, singleSkillData, SkillViewType.Show);

        }

        txt_zhanDouLi.SetText("战斗力：" + $"<color=red>{RoleManager.Instance.CalcZhanDouLi(p).ToString()}</color>");
        txt_shuxin.SetText("属性: " + txt_talent.text);
        //trans_rarity.gameObject.SetActive(false);

    }
    public override void ShowStudentView()
    {
        singleStudentView = PanelManager.Instance.OpenSingle<SingleStudentViewItem>(trans_studentViewParent, p);
    }
}
