
using cfg;
using Framework.Data;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗准备面板中的弟子（包括掌门
/// </summary>
public class BattlePreparePeopleView : SingleStudentView
{
    public BattlePreparePanel parentPanel;
    public Text txt_jingJie;//境界
    public Transform trans_skillGrid;

    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn, () =>
        {
            PanelManager.Instance.OpenPanel<PeopleTipsPanel>(PanelManager.Instance.trans_layer2,peopleData);
        });
        
    }


    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_jingJie.SetText(""+RoleManager.Instance.CalcZhanDouLi(peopleData));
        PanelManager.Instance.CloseAllSingle(trans_skillGrid);
        
        // 显示当前角色已装备的技能
        if (peopleData.allSkillData != null && peopleData.allSkillData.equippedSkillIdList != null && 
            peopleData.allSkillData.equippedSkillIdList.Count >= 2)
        {
            for(int i=1; i < peopleData.allSkillData.equippedSkillIdList.Count; i++)
            {
                SingleSkillData skillData = SkillManager.Instance.GetSingleSkillDataByEquippedSkillId(
                    peopleData.allSkillData.equippedSkillIdList[i], peopleData.allSkillData);
                
                if (skillData != null)
                {
                    SkillSetting skillSetting = DataTable.FindSkillSetting(skillData.skillId);
                    if (skillSetting != null)
                    {
                        // 使用 BattleSkillView 显示技能
                        BattleSkillView skillView = PanelManager.Instance.OpenSingle<BattleSkillView>(
                            trans_skillGrid, peopleData, skillSetting, SkillViewType.Show, true);
                        
                    }
                }
            }
        }
    }
    public override void ShowStudentKuang()
    {
        base.ShowStudentKuang();
        img_yuanSu.gameObject.SetActive(false);
        if (!peopleData.isMyTeam)
        {
            img_bg.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + "img_dizijibiek7");
        }
    }
    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_skillGrid);
    }

}
