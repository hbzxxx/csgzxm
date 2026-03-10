using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class FarmPrepareStudentView : BigStudentView
{
    public Button btn_up;//出战
    //public Button btn_down;//下来

    public SingleDanFarmDetailPanel parentPanel;
    public EquipMakeBuildingPanel equipMakeBuildingPanel;
    public LianDanBuildingPanel lianDanBuildingPanel;//宝石
    public SingleDanFarmData parentFarmData;
    public GameObject obj_talentJianTou;
    public override void Init(params object[] args)
    {
        base.Init(args);
        //TODO做成接口
        parentPanel = args[1] as SingleDanFarmDetailPanel;
        if (parentPanel == null)
        {
            equipMakeBuildingPanel = args[1] as EquipMakeBuildingPanel;
            if (equipMakeBuildingPanel == null)
            {
                lianDanBuildingPanel =args[1] as LianDanBuildingPanel;
                parentFarmData = lianDanBuildingPanel.singleDanFarmData;
            }
            else
            {
                parentFarmData = equipMakeBuildingPanel.singleDanFarmData;

            }
        }
        else
        {
            parentFarmData = parentPanel.singleDanFarmData;
        }

        addBtnListener(btn_up, () =>
        {
            if (parentPanel != null)
                parentPanel.OnUpStudent(this);
            else if (equipMakeBuildingPanel != null)
                equipMakeBuildingPanel.OnUpStudent(this);
            else if (lianDanBuildingPanel != null)
                lianDanBuildingPanel.OnUpStudent(this);

            PanelManager.Instance.CloseTaskGuidePanel();
        });
        ////弟子下阵
        //addBtnListener(btn_down, () =>
        //{
        //    LianDanManager.Instance.StopZuoZhen(p.OnlyId);
        //});

     

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
    }
    public override void ShowStudentView()
    {
        singleStudentView = PanelManager.Instance.OpenSingle<SingleStudentView>(trans_studentViewParent, p,this);

    }
    public override void ShowLV()
    {
        base.ShowLV();
        txt_lv.gameObject.SetActive(false);
        //txt_zhanDouLi.gameObject.SetActive(false);
    }

    public void RefreshShow()
    {
        if ((p.studentStatusType == (int)StudentStatusType.DanFarmQuanLi
            || p.studentStatusType == (int)StudentStatusType.DanFarmRelax
            || p.studentStatusType == (int)StudentStatusType.DanFarmWork)
            &&p.zuoZhenDanFarmOnlyId==parentFarmData.OnlyId)
        {
            //p StudentStatusType.AtTeam
           // btn_down.gameObject.SetActive(true);
            btn_up.gameObject.SetActive(false);
        }
        else
        {
            btn_up.gameObject.SetActive(true);
            //btn_down.gameObject.SetActive(false);
        }


        if (parentFarmData.TalentType == p.talent)
        {
            obj_talentJianTou.gameObject.SetActive(true);
        }
        else
        {
            obj_talentJianTou.gameObject.SetActive(false);

        }
        trans_rarity.gameObject.SetActive(false);
    }

    /// <summary>
    /// 成功上阵
    /// </summary>
    /// <param name="param"></param>
    public void OnSuccessUpOrDownStudent(object[] param)
    {
        PeopleData theP = param[0] as PeopleData;
        if (p.onlyId == theP.onlyId)
        {
            p = theP;
            RefreshShow();
        }
    }
}
