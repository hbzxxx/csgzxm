using cfg;
using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelStudentView : SingleStudentView
{
    public Text txt_curWork;
    StudentHandlePanel parentPanel;
    public Image img_choosed;
    public GameObject obj_redPoint;//红点

    public override void Init(params object[] args)
    {
        base.Init(args);
        parentPanel = args[1] as StudentHandlePanel;

        addBtnListener(btn, () =>
        {
            parentPanel.OnChoosedStudent(peopleData);
        });
        img_choosed.gameObject.SetActive(false);

        RegisterEvent(TheEventType.SuccessBreakThrough, OnSuccessBreakThrough);
        RegisterEvent(TheEventType.FailBreakThrough, OnFailBreakThrough);
        RegisterEvent(TheEventType.RefreshStudentRedPoint, OnRefreshRedPoint);

    }
    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ShowName();
        ShowPortrait();
        //如果有enemysetting 则用enemysetting 的icon 如果没有 则用自己的icon 如果都没有 则用默认icon
        img_bgk.gameObject.SetActive(true);
        Debug.Log($"[InfoPanelStudentView] studentRarity={peopleData.studentRarity}, talent={peopleData.talent}");
        img_bgk.sprite = CommonUtil.StudentBgKuang(peopleData);
        int rarity = peopleData.studentRarity;
        if (rarity == 0)
            rarity = 1;
        if (peopleData.talent == (int)StudentTalent.LianGong)
            img_bg.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_dizik_" + rarity);
        else
            img_bg.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.resCommonPath + "img_dizikn_" + rarity);
        //ShowCurWork();

    }
    public override void RefreshShow()
    {
        base.RefreshShow();
        OnRedPointShow();

    }
    /// <summary>
    /// 红点
    /// </summary>
    /// <param name="node"></param>
    public void OnRedPointShow()
    {
        RedPointManager.Instance.SetRedPointUI(obj_redPoint, RedPointType.MainPanel_Btn_Student_InfoPanelStudentViewInfo, (int)(ulong)peopleData.onlyId);

    }
    void OnRefreshRedPoint(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        if (theP == peopleData)
            OnRedPointShow();
    }

    /// <summary>
    /// 成功突破
    /// </summary>
    void OnSuccessBreakThrough(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        if (theP.onlyId == peopleData.onlyId)
            OnRedPointShow();
    }

    void OnFailBreakThrough(object[] args)
    {
        PeopleData theP = args[0] as PeopleData;
        if (theP.onlyId == peopleData.onlyId)
            OnRedPointShow();
    }
    public void OnChoose(bool choose)
    {
        if (choose)
        {
            img_choosed.gameObject.SetActive(true);
        }
        else
        {
            img_choosed.gameObject.SetActive(false);
        }
    }

    public void ShowCurWork()
    {
        if (peopleData.studentStatusType == (int)StudentStatusType.DanFarmWork
            || peopleData.studentStatusType == (int)StudentStatusType.DanFarmRelax
            || peopleData.studentStatusType == (int)StudentStatusType.DanFarmQuanLi)
        {
            SingleDanFarmData singleDanFarmData = BuildingManager.Instance.FindDanFarmDataByOnlyId(peopleData.zuoZhenDanFarmOnlyId);// RoleManager.Instance._CurGameInfo.allDanFarmData.DanFarmList[peopleData.zuoZhenDanFarmIndex];
            DanFarmSetting setting = DataTable.FindDanFarmSetting(singleDanFarmData.SettingId);
            txt_curWork.SetText(setting.Name + "驻守");
        }
        else if (peopleData.studentStatusType == (int)StudentStatusType.AtExplore)
        {
            txt_curWork.SetText("正在秘境探险");
        }
        else if (peopleData.studentStatusType == (int)StudentStatusType.AtTeam)
        {
            txt_curWork.SetText("已上阵");
        }
        else
        {
            txt_curWork.SetText("");
        }
        //student.StudentStatusType = ;
        //student.ZuoZhenDanFarmIndex = curChoosedDanFarmData.Index;

    }
}
