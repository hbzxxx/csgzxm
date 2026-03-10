using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cfg;
using Framework.Data;
using UnityEngine.UI;
 
public class EquipUpgradeResPanel : PanelBase
{

    public List<List<SinglePropertyData>> proCompareList;
    public EquipProtoData data;
    List<int> beforeExp;//升级前的经验值
    public Text txt_lvBefore;
    public Text txt_lvAfter;

    public RectTransform rectTrans_grid;//大格子
    public RectTransform rectTrans_contentBg;//背景
    public float yOffset=100;//y位置补偿


    public Text txt_pro1Name;
    public Text txt_pro2Name;

    public Text txt_pro1Num;
    public Text txt_pro2Num;

    public Transform trans_mainCompareGrid;

    public Transform trans_viceCompareGrid;

    public Transform trans_studentGrid;

    List<PeopleData> studentList = new List<PeopleData>();
    List<Quality> qualityList;
    List<int> afterProInitNumList;
    public Text txt_youHuaLv;
    public StudentAddAnimGroup studentAddAnimGroup;
    public override void Init(params object[] args)
    {
        base.Init(args);

        proCompareList = args[0] as List<List<SinglePropertyData>>;
        data = args[1] as EquipProtoData;
        beforeExp = args[2] as List<int>;
        studentList = args[3] as List<PeopleData>;
        qualityList = args[4] as List<Quality>;
        afterProInitNumList = args[5] as List<int>;
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        txt_lvBefore.SetText("Lv." + (data.curLevel - 1));
        txt_lvAfter.SetText("Lv." + data.curLevel);


        //for(int i = 0; i < proCompareList.Count; i++)
        //{
        List<SinglePropertyData> theCompare1 = proCompareList[0];
        SinglePropertyData finalPro = null;
        //有对比
        if (theCompare1.Count == 2)
        {
            SinglePropertyData pro1 = theCompare1[0];
            PropertySetting propertySetting = DataTable.FindPropertySetting(pro1.id);
            txt_pro1Name.SetText(propertySetting.Name);
            txt_pro1Num.SetText(pro1.num.ToString());

            finalPro = theCompare1[1];

        }
        else
        {
            SinglePropertyData pro1 = theCompare1[0];
            PropertySetting propertySetting = DataTable.FindPropertySetting(pro1.id);
            txt_pro1Name.SetText(propertySetting.Name);
            txt_pro1Num.SetText("0");
            finalPro = theCompare1[0];
        }
        //int theMainNumInit = Mathf.RoundToInt(finalPro.Num / (1 + ((int)qualityList[0] - 1) * 0.2f));

        AddSingle<StudentProAddShowView>(trans_mainCompareGrid, (int)afterProInitNumList[0], finalPro.num);


        List<SinglePropertyData> theCompare2 = proCompareList[1];
        SinglePropertyData finalPro2 = null;
        //有对比
        if (theCompare2.Count == 2)
        {
            SinglePropertyData pro2 = theCompare2[0];
            PropertySetting propertySetting = DataTable.FindPropertySetting(pro2.id);
            txt_pro2Name.SetText(propertySetting.Name);
            txt_pro2Num.SetText(pro2.num.ToString());

            finalPro2 = theCompare2[1];

        }
        else
        {
            SinglePropertyData pro2 = theCompare2[0];
            PropertySetting propertySetting = DataTable.FindPropertySetting(pro2.id);
            txt_pro2Name.SetText(propertySetting.Name);
            txt_pro2Num.SetText("0");
            finalPro2 = theCompare2[0];
        }
        int theViceNumInit = Mathf.RoundToInt(finalPro2.num / (1 + ((int)qualityList[1] - 1) * 0.2f));
        AddSingle<StudentProAddShowView>(trans_viceCompareGrid, (int)afterProInitNumList[1], finalPro2.num);

        studentAddAnimGroup.Play((int)qualityList[0]);
        ShowStudentExpChange();
        float youHuaLv = 0;
        for (int i = 0; i < qualityList.Count; i++)
        {
            youHuaLv += EquipmentManager.Instance.CalcNewYouHuaLv(qualityList[i]);
        }

        youHuaLv = youHuaLv / qualityList.Count;
        youHuaLv /= 80;
        float youHuaShow = youHuaLv * 100;
        Rarity rarity = (Rarity)Mathf.CeilToInt(youHuaShow / 20);
        txt_youHuaLv.SetText("优化率：" + youHuaShow.ToString("0.0") + "%");
        txt_youHuaLv.color = CommonUtil.RarityColor(rarity);

        AuditionManager.Instance.PlayVoice(AudioClipType.MakeFinish);
    }

    /// <summary>
    /// 显示弟子经验值变化
    /// </summary>
    public void ShowStudentExpChange()
    {
        //ClearCertainParentAllSingle<SingleUpgradeStudentView>(trans_studentGrid);
        //if (studentList != null)
        //{
        //    for (int i = 0; i < studentList.Count; i++)
        //    {
        //        AddSingle<SingleUpgradeStudentView>(trans_studentGrid, beforeExp[i], studentList[i]);

        //    }
        //}
        //PanelManager.Instance.OpenPanel<StudentAddExpPanel>(PanelManager.Instance.trans_layer2, beforeExp, studentList);

    }

    public override void OnClose()
    {
        base.OnClose();
        if (studentList != null && studentList.Count > 0)
        {
            PanelManager.Instance.OpenPanel<StudentAddExpPanel>(PanelManager.Instance.trans_layer2, beforeExp, studentList);

        }
    }

    public override void Clear()
    {
        base.Clear();
        
        //PanelManager.Instance.CloseAllSingle(trans_studentGrid);
    }
}
