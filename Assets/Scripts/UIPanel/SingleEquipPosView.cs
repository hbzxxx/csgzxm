using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleEquipPosView : SingleViewBase
{
    public EquipPanel parentPanel;
    public EquipItemposView itemView;
    public int posIndex;
    public PeopleData p;
    public Transform trans_grid;
    public Text txt_posName;
    public Button btn;
    public Transform stars;
    [Header("星星图标")]
    public Sprite ystar;
    public Sprite nstar;
    
    [Header("装备属性显示")]
    public Transform trans_proGrid;//属性显示格子
  
    public override void Init(params object[] args)
    {
        base.Init(args);
        p = args[0] as PeopleData;
        parentPanel = args[1] as EquipPanel;
        addBtnListener(btn, () =>
        {
            PanelManager.Instance.CloseAllPanel(parentPanel.sonPanelParent);
            PanelManager.Instance.OpenPanel<EquipChangePanel>(parentPanel.sonPanelParent, p, posIndex, parentPanel);
        });
        
        RegisterEvent(TheEventType.OnEquip, OnEquipChangedWithArgs);
        RegisterEvent(TheEventType.OnUnEquip, OnEquipChangedWithArgs);
        RegisterEvent(TheEventType.JingLianEquip, OnJingLianChanged);
    }
    
    void OnEquipChangedWithArgs(object[] args)
    {
        // 检查当前对象是否已被销毁
        if (this == null || !gameObject.activeInHierarchy)
            return;
            
        RefreshShow();
    }
    
    void OnJingLianChanged()
    {
        // 检查当前对象是否已被销毁
        if (this == null || !gameObject.activeInHierarchy)
            return;
            
        RefreshShow();
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        RefreshShow();
        txt_posName.SetText(EquipmentManager.Instance.PosTxt(posIndex));
    }

    public void LiangStar(int len) {
        if (stars == null || !stars.gameObject.activeInHierarchy)
            return;
        
        Image[] images = stars.GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
        {
            if (i < len)
            {
                images[i].sprite = ystar;
            }
            else
            {
                images[i].sprite = nstar;
            }

        }
    }

    public void RefreshShow()
    {
        // 检查当前对象是否已被销毁
        if (this == null || !gameObject.activeInHierarchy)
            return;
            
        PanelManager.Instance.CloseAllSingle(trans_grid);
        if (trans_proGrid != null)
            PanelManager.Instance.CloseAllSingle(trans_proGrid);

        ItemData itemdata = p.curEquipItemList[posIndex];
        // 检查 itemData 是否有效（settingId > 0 表示有效的装备数据）
        if (itemdata != null && itemdata.settingId > 0)
        {
            itemView = PanelManager.Instance.OpenSingle<EquipItemposView>(trans_grid, itemdata, p,null);
            if (itemdata.equipProtoData != null)
            {
                int len = itemdata.equipProtoData.jingLianLv;
                LiangStar(len);
            }
            ShowEquipProperties(itemdata);
        }
        else
        {
            // 如果 itemdata 存在但 settingId 无效，清理该槽位
            if (itemdata != null && itemdata.settingId <= 0)
            {
                p.curEquipItemList[posIndex] = null;
            }
            LiangStar(0);
        }
    }
    
    void ShowEquipProperties(ItemData equipItem)
    {
        if (trans_proGrid == null || equipItem == null || equipItem.equipProtoData == null)
            return;
            
        for (int i = 0; i < equipItem.equipProtoData.propertyList.Count; i++)
        {
            SinglePropertyData pro = equipItem.equipProtoData.propertyList[i];
            PanelManager.Instance.OpenSingle<EquipPosPropertyView>(trans_proGrid, pro.id, pro.num, (Quality)pro.quality);
        }
    }

    public override void Clear()
    {
        base.Clear();
        PanelManager.Instance.CloseAllSingle(trans_grid);
        if (trans_proGrid != null)
            PanelManager.Instance.CloseAllSingle(trans_proGrid);
        itemView = null;
    }
}
