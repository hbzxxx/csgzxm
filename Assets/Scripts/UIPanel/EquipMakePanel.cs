using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;
using DG.Tweening;
 
public class EquipMakePanel : PanelBase
{
    public Button btn_newEquip;

    #region 当前在做的装备
    public Transform trans_curMakingEquip;//当前正在做的装备
    public Transform trans_curMakingEquipGrid;//当前正在做的装备
    public Text txt_process;//进度
    public Image img_processBar;
    public Text txt_equipName;//当前装备名



    #region 装备制造动画相关
     public Transform trans_makingEquipProGrid;//装备属性
    public List<SinglePropertyView> makingEquipProViewList = new List<SinglePropertyView>();//制作装备
    public Transform trans_addProTxtAnimViewStartPos;//增加属性动画开始点
    public Transform trans_addProTxtAnimViewParent;//增加属性动画父物体
    #endregion

    #endregion




    //public EquipmentSetting curChoosedNewEquipSetting;//当前选择的新装备
    //public EquipMakeTeamData curChoosedEquipMakeTeam;//当前选择的团队
    //public int farmId;//在哪个房
    public SingleDanFarmData singleDanFarmData;//在哪个房
    public Transform trans_sonParent;

    public override void Init(params object[] args)
    {
        base.Init(args);
        singleDanFarmData = args[0] as SingleDanFarmData;
        RegisterEvent(TheEventType.MakeEquipProcessing, ShowEquipProcessChange);
        RegisterEvent(TheEventType.FinishMakeEquip, ShowFinishEquip);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();

        //如果正在做装备
        if (RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData != null)
        {
            ShowCurMakingEquip();
           
        }
        //如果没有做装备 显示做装备panel
        else
        {
            trans_curMakingEquip.gameObject.SetActive(false);
            PanelManager.Instance.OpenPanel<NewEquipPreparePanel>(trans_sonParent, this);
        }

    }
 
    /// <summary>
    /// 显示正在做的装备
    /// </summary>
    public void ShowCurMakingEquip()
    {
        int equipId = 0;
        List<List<int>> proList = new List<List<int>>();
        EquipmentSetting setting = null;

        trans_curMakingEquip.gameObject.SetActive(true);
        if (trans_sonParent.childCount > 0)
        {
            PanelManager.Instance.ClosePanel(trans_sonParent.GetChild(0).GetComponent<PanelBase>());
        }
        PanelManager.Instance.CloseAllSingle(trans_curMakingEquipGrid);

        //显示当前正在做的装备和进度
        equipId = RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.settingId;
        setting = DataTable.FindEquipSetting(equipId);
        ItemData data = new ItemData();
        data.settingId = setting.ItemId.ToInt32();
        data.count = 1;
        PanelManager.Instance.OpenSingle<ItemView>(trans_curMakingEquipGrid, data);
        txt_equipName.SetText("[当前产出：" + setting.Name + "]");

        txt_process.SetText((100 * RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.processDay / (float)ConstantVal.equipMakeTotalDay).ToString("0.#") + "%");
        img_processBar.DOKill();
        img_processBar.fillAmount = RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.processDay / (float)ConstantVal.equipMakeTotalDay;
        ClearCertainParentAllSingle<SingleViewBase>(trans_makingEquipProGrid);
        makingEquipProViewList.Clear();
        for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.proIdList.Count; i++)
        {
            int theId = RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.proIdList[i];
            int proNum = RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.curProNumList[i];
            BigStudentSinglePropertyView view = AddSingle<BigStudentSinglePropertyView>(trans_makingEquipProGrid, theId, proNum, Quality.None);
            makingEquipProViewList.Add(view);
        }
    }
 
    /// <summary>
    /// 装备制造进度改变
    /// </summary>
    void ShowEquipProcessChange(object[] args)
    {
        EquipMakeData equipMakeData = args[0] as EquipMakeData;
        if (equipMakeData.farmOnlyId == singleDanFarmData.OnlyId)
        {
            float curProcess = equipMakeData.processDay / (float)ConstantVal.equipMakeTotalDay;
            txt_process.SetText((100 * RoleManager.Instance._CurGameInfo.AllEquipmentData.curEquipMakeData.processDay / (float)ConstantVal.equipMakeTotalDay).ToString("0.#") + "%");

            img_processBar.DOFillAmount(curProcess, 1);
            //txt_equipProcessLabel.SetText("炼制中...");
            for (int i = 0; i < makingEquipProViewList.Count; i++)
            {
                int theId = makingEquipProViewList[i].id;
                int theNum = equipMakeData.curProNumList[i];
                int offset = theNum - makingEquipProViewList[i].num;
                makingEquipProViewList[i].NumTxtLerpShow(theNum);
                if (offset > 0)
                {
                    Vector2 pos = new Vector2(trans_addProTxtAnimViewStartPos.position.x, makingEquipProViewList[i].txt_num.transform.position.y);
                    AddSingle<AddProTxtAnimView>(trans_addProTxtAnimViewParent, offset, pos);
                }
                //if (equipMakeData.ProIdList.Contains(theId))
                //{
                //    int index = equipMakeData.ProIdList.IndexOf(theId);
                //    int theNum = equipMakeData.CurProNumList[index];
                //    int offset = theNum - makingEquipProViewList[i].num;
                //    makingEquipProViewList[i].NumTxtLerpShow(theNum);
                   
                //}
            }
        }
 
    }
    /// <summary>
    /// 显示制造完成 TODO 做到红点里面 做到通知里面
    /// </summary>
    void ShowFinishEquip(object[] args)
    {
        EquipMakeData equipMakeData = args[0] as EquipMakeData;
        if (equipMakeData.farmOnlyId == singleDanFarmData.OnlyId)
        {
            TaskManager.Instance.guide_makeEquip = false;

            trans_curMakingEquip.gameObject.SetActive(false);
            PanelManager.Instance.OpenPanel<NewEquipPreparePanel>(trans_sonParent, this);
        }
    }
    public override void Clear()
    {
        base.Clear();
        if (trans_sonParent.childCount > 0)
        {
            PanelManager.Instance.ClosePanel(trans_sonParent.GetChild(0).GetComponent<PanelBase>());
        }

    }

    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.CloseTaskGuidePanel();
        TaskManager.Instance.guide_makeEquip = false;
    }

}
