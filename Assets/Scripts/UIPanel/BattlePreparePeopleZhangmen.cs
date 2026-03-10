using cfg;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePreparePeopleZhangmen : SingleStudentView
{
    public BattlePreparePanel parentPanel;
    public Transform trans_skillGrid;

    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn, () =>
        {
            PanelManager.Instance.OpenPanel<PeopleTipsPanel>(PanelManager.Instance.trans_layer2, peopleData);
        });
        
    }


    public override void OnOpenIng()
    {
        base.OnOpenIng();
        //txt_jingJie.SetText("ս��" + RoleManager.Instance.CalcZhanDouLi(peopleData));
        PanelManager.Instance.CloseAllSingle(trans_skillGrid);
        
        // 只显示第一个携带的技能（跳过普攻索引0，只显示索引1的技能）
        if (peopleData.allSkillData != null && peopleData.allSkillData.equippedSkillIdList != null && 
            peopleData.allSkillData.equippedSkillIdList.Count >= 2)
        {
            // 获取第一个携带的技能（索引1，跳过普攻）
            SingleSkillData skillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(
                peopleData.allSkillData.equippedSkillIdList[1], peopleData.allSkillData);
            
            if (skillData != null)
            {
                SkillSetting skillSetting = DataTable.FindSkillSetting(skillData.skillId);
                if (skillSetting != null)
                {
                    // 使用 BattleSkillView 显示第一个技能
                    BattleSkillView skillView = PanelManager.Instance.OpenSingle<BattleSkillView>(
                        trans_skillGrid, peopleData, skillSetting, SkillViewType.Show, false);
                }
            }
        }
    }
    public override void ShowStudentKuang()
    {
        base.ShowStudentKuang();
        //if (!peopleData.isMyTeam)
        //{
        //    img_bg.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + "img_dizijibiek7");
        //}
    }
    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_skillGrid);
    }

}
