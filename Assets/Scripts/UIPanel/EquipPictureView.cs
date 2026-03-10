using Framework.Data;
using cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品
/// </summary>
public class EquipPictureView : SingleViewBase
{
    //奖励
    public SingleEquipPictureData equipPictureData;
    public Image img_icon;   //icon
    public Button btn;//弹出tips
    public GameObject obj_redPoint;//红点，刚解锁和可升级时会冒出来，点了以后会消失
    public EquipmentSetting setting;
    public NewEquipPreparePanel parentPanel;
    public Image img_bg;
    public Text txt_name;
    public GameObject obj_noMat;
    public override void Init(object[] args)
    {
        this.equipPictureData = args[0] as SingleEquipPictureData;
        setting = DataTable.FindEquipSetting(equipPictureData.equipId);
        parentPanel =(NewEquipPreparePanel)args[1];

        addBtnListener(btn, () =>
        {
            parentPanel.OnChoosedPicture(this);
        });
        
    }

    public override void OnOpenIng()
    {
        RefreshShow();
    }

    /// <summary>
    /// 刷新显示
    /// </summary>
    public virtual void RefreshShow()
    {
        //RedPointManager.Instance.SetRedPointUI(obj_redPoint, RedPointType.Item, itemData.SettingId);
        Clear();
        ShowIcon();
        ShowIfCanMake();
        txt_name.SetText(setting.Name);
    }

    void ShowIfCanMake()
    {
        bool cannotMake = false;
        EquipmentSetting curChoosedNewEquipSetting = DataTable.FindEquipSetting( equipPictureData.equipId);

        List<List<int>> makeCost = CommonUtil.SplitCfg(curChoosedNewEquipSetting.MakeCost);
        for (int i = 0; i < makeCost.Count; i++)
        {
            List<int> cost = makeCost[i];
            if (!ItemManager.Instance.CheckIfItemEnough(cost[0], (ulong)cost[1]))
            {
                cannotMake = true;
            }
        }
        obj_noMat.SetActive(cannotMake);
    }

    /// <summary>
    /// 显示icon
    /// </summary>
    void ShowIcon()
    {
        // 添加 setting 空值检查
        if (setting == null)
        {
            Debug.LogError("setting is null in EquipPictureView.ShowIcon");
            return;
        }
        
        ItemData itemData = new ItemData();
        itemData.settingId = setting.Id.ToInt32();
        ItemSetting itemSetting = DataTable.FindItemSetting(itemData.settingId);
        
        // 添加空值检查
        if (itemSetting == null)
        {
            Debug.LogError($"ItemSetting not found for settingId: {itemData.settingId}");
            return;
        }
        
        if (img_icon == null)
        {
            Debug.LogError("img_icon is null in EquipPictureView");
            return;
        }
        
        img_bg.ShowItemFrameImg(itemData);
        img_icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + itemSetting.UiName);
    }


    public void OnChoose(bool choose)
    {
        if (choose)
        {
            img_bg.color = ConstantVal.color_choosed;
        }
        else
        {
            img_bg.color = Color.white;
        }
    }

}
