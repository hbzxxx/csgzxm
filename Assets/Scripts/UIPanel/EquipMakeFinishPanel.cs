 using DG.Tweening;
using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipMakeFinishPanel : PanelBase
{
    public ItemData itemData;
    public Transform trans_grid;//格子
    public ItemSetting itemSetting;
    public EquipmentSetting equipmentSetting;

    public Text txt_mainProName;
    public Transform trans_mainProCompareGrid;

    public Transform trans_vicePro;//副属性
    public Text txt_viceProName;
    public Transform trans_viceProCompareGrid;

    public Button btn_goToEquip;//去装备
    public int quality;
    List<int> beforeExpList=new List<int>();
    List<PeopleData> pList=new List<PeopleData>();
    public Transform trans_studentGrid;

    public Text txt_youHuaLv;//优化率

    public StudentAddAnimGroup animGroup;

    public override void Init(params object[] args)
    {
        base.Init(args);
        itemData = args[0] as ItemData;
        quality = (int)args[1];
        beforeExpList = args[2] as List<int>;
        pList = args[3] as List<PeopleData>;
        itemSetting = DataTable.FindItemSetting(itemData.settingId);
        equipmentSetting = DataTable.FindEquipSetting(itemData.equipProtoData.settingId);

        addBtnListener(btn_goToEquip, () =>
        {
            PanelManager.Instance.ClosePanel(this);
            //if (TaskManager.Instance.FindAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.Guide_Equip).ToString()).ToInt32() == 0)
            //{
            //    TaskManager.Instance.SpecialGuide_EquipTeach(itemData.OnlyId);
            //    PanelManager.Instance.ClosePanel(this);
            //    TaskManager.Instance.GetAchievement(AchievementType.OnceGuide, ((int)OnceGuideIdType.Battle_DaZhaoClickMai).ToString());
            //}

        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        AddSingle<ItemView>(trans_grid,itemData);
        int theMainIdFinal = itemData.equipProtoData.propertyIdList[0];
        int theMainNumFinal = itemData.equipProtoData.propertyList[0].num;

        int theMainNumInit = Mathf.RoundToInt(theMainNumFinal / (1 + (quality - 1) * 0.2f));
        int theMainNumAdd = theMainNumFinal - theMainNumInit;
        PropertySetting mainProSetting = DataTable.FindPropertySetting(theMainIdFinal);


 

        txt_mainProName.SetText(mainProSetting.Name);

        AddSingle<StudentProAddShowView>(trans_mainProCompareGrid, theMainNumInit, theMainNumFinal);

        if (itemData.equipProtoData.propertyIdList.Count >= 2)
        {
            trans_vicePro.gameObject.SetActive(true);
            int theViceIdFinal = itemData.equipProtoData.propertyIdList[1];
            int theViceNumFinal = itemData.equipProtoData.propertyList[1].num;
            int theViceNumInit = Mathf.RoundToInt(theViceNumFinal / (1 + (quality - 1) * 0.2f));
            int theViceNumAdd = theViceNumFinal - theViceNumInit;
            PropertySetting viceProSetting = DataTable.FindPropertySetting(theViceIdFinal);
            txt_viceProName.SetText(viceProSetting.Name);
            AddSingle<StudentProAddShowView>(trans_viceProCompareGrid, theViceNumInit, theViceNumFinal);
        }
        else
        {
            trans_vicePro.gameObject.SetActive(false);
        }


        //ClearCertainParentAllSingle<SingleUpgradeStudentView>(trans_studentGrid);

        //for (int i = 0; i < pList.Count; i++)
        //{
        //    AddSingle<SingleUpgradeStudentView>(trans_studentGrid, beforeExpList[i], pList[i]);
        //}
        float youHuaLv = itemData.equipProtoData.youHuaLv / 80f;
        float youHuaShow = youHuaLv * 100;
        Rarity rarity = (Rarity)Mathf.CeilToInt(youHuaShow / 20);
        txt_youHuaLv.SetText("优化率：" + youHuaShow.ToString("0.0") + "%");
        txt_youHuaLv.color = CommonUtil.RarityColor(rarity);

        animGroup.Play(quality);
        AuditionManager.Instance.PlayVoice(AudioClipType.MakeFinish);
    }

    public override void OnClose()
    {
        base.OnClose();
        if (pList.Count > 0)
        {
            PanelManager.Instance.OpenPanel<StudentAddExpPanel>(PanelManager.Instance.trans_layer2, beforeExpList, pList);
        }
    }
}
