using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class TouXiangChangePanel : PanelBase
{
    public TouXiangType touXiangType;
    public Transform grid;
    public List<SingleTouXiangView> viewList = new List<SingleTouXiangView>();
    public Button btn_touXiangTag;
    public Button btn_touXiangKuangTag;
    public override void Init(params object[] args)
    {
        base.Init(args);
        if (!ItemManager.Instance.CheckIfHaveItemBySettingId((int)ItemIdType.PuTongTouXiang))
            ItemManager.Instance.GetItem((int)ItemIdType.PuTongTouXiang, 1);
        if (!ItemManager.Instance.CheckIfHaveItemBySettingId((int)ItemIdType.PuTongTouXiangKuang))
            ItemManager.Instance.GetItem((int)ItemIdType.PuTongTouXiangKuang, 1);
        addBtnListener(btn_touXiangTag, () =>
        {
            ShowTouXiangPage(TouXiangType.TouXiang);
        });
         
        addBtnListener(btn_touXiangKuangTag, () =>
        {
            ShowTouXiangPage(TouXiangType.TouXiangKuang);
        });
    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
    }

    void ShowTouXiangPage(TouXiangType type)
    {
        ClearCertainParentAllSingle<SingleViewBase>(grid);
        touXiangType = type;
        List<ItemSetting> itemList = null;
        if (type == TouXiangType.TouXiang)
        {
            itemList = DataTable.FindItemSettingListByType(ItemType.TouXiang);
        }
        else
        {
            itemList = DataTable.FindItemSettingListByType(ItemType.TouXiangKuang);
        }
      
        for(int i = 0; i < itemList.Count; i++)
        {
            ItemData item = new ItemData();
            item.setting = itemList[i];
            item.settingId = itemList[i].Id.ToInt32();
            AddSingle<SingleTouXiangView>(grid, type, item, this);
        }
    }

    public void OnChoosedTouXiang(SingleTouXiangView view)
    {
        for(int i = 0; i < viewList.Count; i++)
        {
            SingleTouXiangView theView = viewList[i];
            if (theView == view)
            {
                theView.OnChoose(true);

            }
            else
            {
                theView.OnChoose(false);
            }
        }

        if (touXiangType == TouXiangType.TouXiang)
            RoleManager.Instance.ChangeTouXiang(view.item.setting );
        else
            RoleManager.Instance.ChangeTouXiangKuang(view.item.setting );

    }

    public override void Clear()
    {
        base.Clear();
        ClearCertainParentAllSingle<SingleViewBase>(grid);
        viewList.Clear();
    }
}
