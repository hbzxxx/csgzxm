using Coffee.UIEffects;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class SkillBtnView : SingleViewBase
{
    public Button btn;
    public Image img;
    public Image cd;
    public SingleSkillData skillNeiCunData;
    SkillSetting skillSetting;
    int totalCD;
    public Text txt_cd;
    public UIEffect uieffect_energy;//能量盘
    public BattlePanel parentPanel;
    public override void Init(params object[] args)
    {
        base.Init(args);
        skillNeiCunData = args[0] as SingleSkillData;
        parentPanel = args[1] as BattlePanel;
        skillSetting = DataTable.FindSkillSetting(skillNeiCunData.skillId);
        totalCD = skillSetting.Cd.ToInt32();
        img.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.skillFolderPath + skillSetting.IconName);

        addBtnListener(btn,()=> 
        {
            if (skillNeiCunData.cd > 0)
            {
                PanelManager.Instance.OpenFloatWindow("冷却中");
                
            }
            else
            {
                parentPanel.OnFangDa(skillNeiCunData.skillId);

            }
        } );
        RegisterEvent(TheEventType.SkillAttack, RefreshShow);
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        transform.localPosition = Vector3.zero;
        RefreshShow();
    }

    public void RefreshShow()
    {
        cd.fillAmount = skillNeiCunData.cd / (float)totalCD;
        txt_cd.SetText(skillNeiCunData.cd.ToString());
        if (skillNeiCunData.cd <= 0)
        {
            txt_cd.gameObject.SetActive(false);
       
        }
        else
        {
            txt_cd.gameObject.SetActive(true);
        }
        SinglePropertyData pro = RoleManager.Instance.GetCurBattleProperty(PropertyIdType.MpNum,parentPanel.p1);
        if (pro.num < pro.limit)
        {
            uieffect_energy.enabled = false;
            img.material = PanelManager.Instance.mat_grey;
        }
        else
        {
            img.material = null;
            uieffect_energy.enabled = true;
        }
    }

    ///// <summary>
    ///// 能量满了
    ///// </summary>
    //public void OnEnergyFull()
    //{
    //    RefreshShow();
    //}
}
