 using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 新装备准备面板
/// </summary>
public class NewEquipPreparePanel : PanelBase
{
    public EquipPictureView curChoosedPicture;//当前选择的图纸
    public Transform trans_choosedNewEquipGrid;//选择新的装备
    public Text txt_newEquip;//新装备名
    public Transform trans_newEquipProGrid;//新装备属性格子
    public Text txt_newEquipDes;//新装备名字
    public Text txt_taoZhuangDes;//套装
    public Transform trans_makeConsumeGrid;//炼制消耗

    //public Transform trans_pictureGrid;//图纸
    public List<EquipTaoZhuangGroupView> groupViewList = new List<EquipTaoZhuangGroupView>();//所有图纸



    public Button btn_startMake;//开始炼制

    public EquipMakePanel parentPanel;

    public ScrollViewNevigation scrollViewNevigation;

    public List<Button> rarityBtns;

    public Transform grid_groupView;

    public override void Init(params object[] args)
    {
        base.Init(args);
        addBtnListener(btn_startMake, StartMake);
        parentPanel = args[0] as EquipMakePanel;

        for(int i = 0; i < rarityBtns.Count; i++)
        {
            int index = i;
            Button btn = rarityBtns[index];
            addBtnListener(btn, () =>
            {
                if (parentPanel.singleDanFarmData.CurLevel < index + 1)
                {
                    PanelManager.Instance.OpenFloatWindow("炼器房" + (index + 1) + "级开放");
                    return;
                }
                ShowPictures((Rarity)(index+1));
            });
        }
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        rarityBtns[parentPanel.singleDanFarmData.CurLevel-1].onClick.Invoke();
        ////图纸
        //equipPictureViewList.Clear();
        //PanelManager.Instance.CloseAllSingle(trans_pictureGrid);
        //for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllEquipmentData.PictureList.Count; i++)
        //{
        //    SingleEquipPictureData singleEquipPictureData = RoleManager.Instance._CurGameInfo.AllEquipmentData.PictureList[i];
        //    EquipPictureView pictureView = PanelManager.Instance.OpenSingle<EquipPictureView>(trans_pictureGrid, singleEquipPictureData, this);
        //    equipPictureViewList.Add(pictureView);
        //}
        groupViewList[0].pictureViewList[0].btn.onClick.Invoke();
        ShowGuide();
    }

    void ShowGuide()
    {
        if (TaskManager.Instance.guide_makeEquip)
        {
            EquipPictureView choosedView = null;

            //指定装备
            if (TaskManager.Instance.guide_makeEquipId != (int)ItemIdType.AnyEquip)
            {
                EquipmentSetting setting = DataTable.FindEquipSetting(TaskManager.Instance.guide_makeEquipId);
                rarityBtns[setting.Rarity.ToInt32() - 1].onClick.Invoke();

                for(int i = 0; i < groupViewList.Count; i++)
                {
                    EquipTaoZhuangGroupView groupView = groupViewList[i];
                    for(int j=0;j< groupView.pictureViewList.Count; j++)
                    {

                        EquipPictureView theView = groupView.pictureViewList[j];
                        if (theView.setting.ItemId.ToInt32() == TaskManager.Instance.guide_makeEquipId)
                        {
                            choosedView = theView;
                            break;
                        }
                      
                    }

                }
            }
            //任意装备
            else
            {
                choosedView = groupViewList[0].pictureViewList[0];
            }
            if (choosedView != null)
            {
                PanelManager.Instance.LocateScrollAndTaskPoint(scrollViewNevigation, choosedView.gameObject);
            }
        }
    }

    void ShowPictures(Rarity rarity)
    {
        //图纸
        groupViewList.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(grid_groupView);
        Dictionary<int,List<int>> pictureDic= DataTable.FindEquipPictureGroupList(rarity);

        foreach(var item in pictureDic)
        {
            EquipTaoZhuangType type =(EquipTaoZhuangType)item.Key;
            List<int> pictureList = item.Value;
            List<SingleEquipPictureData> pictureDataList = new List<SingleEquipPictureData>();
            for (int i = 0; i < pictureList.Count; i++)
            {
                SingleEquipPictureData data = new SingleEquipPictureData();
                data.equipId = pictureList[i];
                pictureDataList.Add(data);
            }
            EquipTaoZhuangGroupView groupView = AddSingle<EquipTaoZhuangGroupView>(grid_groupView, type, pictureDataList,this);
            groupViewList.Add(groupView);
        }

        //for (int i = 0; i < RoleManager.Instance._CurGameInfo.AllEquipmentData.PictureList.Count; i++)
        //{
        //    SingleEquipPictureData singleEquipPictureData = RoleManager.Instance._CurGameInfo.AllEquipmentData.PictureList[i];
        //    EquipPictureView pictureView = PanelManager.Instance.OpenSingle<EquipPictureView>(trans_pictureGrid, singleEquipPictureData, this);
        //    equipPictureViewList.Add(pictureView);
        //}
        groupViewList[0].pictureViewList[0].btn.onClick.Invoke();
     }
    /// <summary>
    /// 开始做
    /// </summary>
    void StartMake()
    {
        EquipmentSetting curChoosedNewEquipSetting = DataTable.FindEquipSetting(curChoosedPicture.equipPictureData.equipId);

        List<List<int>> makeCost = CommonUtil.SplitCfg(curChoosedNewEquipSetting.MakeCost);

        for (int i = 0; i < makeCost.Count; i++)
        {
            List<int> cost = makeCost[i];
            if (!ItemManager.Instance.CheckIfItemEnough(cost[0], (ulong)cost[1]))
            {
                PanelManager.Instance.OpenFloatWindow(DataTable.FindItemSetting(cost[0]).Name + "不足");
                return;
            }
        }

        //ItemManager.Instance.LoseItem((int)ItemIdType.LingShi, curChoosedNewEquipSetting.cost.ToUInt64());
        EquipmentManager.Instance.StartMakeEquip(curChoosedNewEquipSetting,parentPanel.singleDanFarmData);
        parentPanel.ShowCurMakingEquip();
        PanelManager.Instance.ClosePanel(this);
    }

    /// <summary>
    /// 选择了一个图纸
    /// </summary>
    public void OnChoosedPicture(EquipPictureView view)
    {
        for (int i = 0; i < groupViewList.Count; i++)
        {
            EquipTaoZhuangGroupView groupView = groupViewList[i];
            for(int j = 0; j < groupView.pictureViewList.Count; j++)
            {
                EquipPictureView theView = groupView.pictureViewList[j];
                if (view.equipPictureData.equipId == theView.equipPictureData.equipId)
                {
                    theView.OnChoose(true);
                }
                else
                {
                    theView.OnChoose(false);
                }

            }
       
        }
        curChoosedPicture = view;
        EquipmentSetting setting = DataTable.FindEquipSetting(view.equipPictureData.equipId);
        ItemData itemData = new ItemData();
        itemData.settingId = view.equipPictureData.equipId;
        ClearCertainParentAllSingle<ItemView>(trans_choosedNewEquipGrid);
        AddSingle<ItemView>(trans_choosedNewEquipGrid, itemData);
        txt_newEquip.SetText(setting.Name);
        txt_newEquipDes.SetText(setting.Des);

        PanelManager.Instance.CloseAllSingle(trans_newEquipProGrid);
        List<List<int>> proList = CommonUtil.SplitCfg(setting.BasePro);
        for (int i = 0; i < proList.Count; i++)
        {
            List<int> pro = proList[i];
            int id = pro[0];
            int num = pro[1];
            PanelManager.Instance.OpenSingle<BigStudentSinglePropertyView>(trans_newEquipProGrid, id, num, Quality.None);
        }
        //List<int> skillIdList = CommonUtil.SplitCfgOneDepth(setting.skillId);
        //for (int i = 0; i < skillIdList.Count; i++)
        //{
        //    int skillId = skillIdList[i];
        //    PanelManager.Instance.OpenSingle<SingleSkillView>(trans_newEquipSkillGrid, skillId);
        //}
        List<List<int>> makeCost = CommonUtil.SplitCfg(setting.MakeCost);

        ClearCertainParentAllSingle<SingleConsumeView>(trans_makeConsumeGrid);
        for (int i = 0; i < makeCost.Count; i++)
        {
            List<int> cost = makeCost[i];
            int id = cost[0];
            int num = cost[1];
            AddSingle<SingleConsumeView>(trans_makeConsumeGrid, id, num, ConsumeType.Item);

        }

        if (TaskManager.Instance.guide_makeEquip)
        {
            //EquipPictureView choosedView = null;

            if (TaskManager.Instance.guide_makeEquipId != (int)ItemIdType.AnyEquip)
            {
                if (view.setting.ItemId.ToInt32() == TaskManager.Instance.guide_makeEquipId)
                {
                    PanelManager.Instance.ShowTaskGuidePanel(btn_startMake.gameObject);
                }
                else
                {
                    ShowGuide();
                }
             
            }
            else
            {
                PanelManager.Instance.ShowTaskGuidePanel(btn_startMake.gameObject);
            }
  
        }
        string taoZhuangDes = "";
        EquipTaoZhuangType taoZhuangType =(EquipTaoZhuangType)setting.TaoZhuang.ToInt32();
        if (taoZhuangType != EquipTaoZhuangType.None)
        {
            string des1 = EquipmentManager.Instance.TaoZhuangDes(taoZhuangType, false);
            string des2 = EquipmentManager.Instance.TaoZhuangDes(taoZhuangType, true);
            taoZhuangDes = des1 + "\n" + des2;
        }

        txt_taoZhuangDes.SetText(taoZhuangDes);
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_newEquipProGrid);


        ClearCertainParentAllSingle<SingleViewBase>(grid_groupView);

        groupViewList.Clear();
        
    }
    public override void OnClose()
    {
        base.OnClose();
        PanelManager.Instance.CloseTaskGuidePanel();
    }
}
