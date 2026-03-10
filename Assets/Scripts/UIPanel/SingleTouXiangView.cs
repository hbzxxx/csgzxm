using Framework.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using cfg;

public class SingleTouXiangView : SingleViewBase
{
    public ItemData item;
    public Image img_kuangDi;
    public Image img_kuang;
    public Image icon;
    public GameObject obj_choose;
    public TouXiangChangePanel parentPanel;
    public Button btn;
    TouXiangType touXiangType;
    public GameObject obj_suo;
    public override void Init(params object[] args)
    {
        base.Init(args);
        touXiangType = (TouXiangType)args[0];
        item = args[1] as ItemData;
        parentPanel = args[2] as TouXiangChangePanel;
        addBtnListener(btn, () =>
        {
            parentPanel.OnChoosedTouXiang(this);

        });
        RegisterEvent(TheEventType.ChangeTouXiang, RefreshStatusShow);
        RegisterEvent(TheEventType.ChangeTouXiangKuang, RefreshStatusShow);

    }

    public override void OnOpenIng()
    {
        base.OnOpenIng();
        ItemSetting kuangSetting =null;

        //显示头像
        if (touXiangType == TouXiangType.TouXiang)
        {
            if (string.IsNullOrWhiteSpace(RoleManager.Instance._CurGameInfo.TouXiang))
            {
                kuangSetting = DataTable.FindItemSetting((int)ItemIdType.PuTongTouXiangKuang);
                //img_kuangDi.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.commonTouXiangKuangDi);
                //img_kuang.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.commonTouXiangKuang);

            }
            else
            {
                string[] arr = RoleManager.Instance._CurGameInfo.TouXiang.Split('|');
                //有头像框
                if (!string.IsNullOrWhiteSpace(arr[1]))
                {
                    kuangSetting = DataTable.FindItemSetting(arr[1].ToInt32());

                }
                //没头像框
                else
                {
                    kuangSetting = DataTable.FindItemSetting((int)ItemIdType.PuTongTouXiangKuang);

                }
            }
            icon.ShowItemIcon(item);
            icon.gameObject.SetActive(true);
        }
        else
        {
            kuangSetting = item.setting;
            icon.gameObject.SetActive(false);
        }

        string[] kuangDiAndKuang = kuangSetting.Param.Split('|');
        img_kuangDi.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + kuangDiAndKuang[0]);
        img_kuang.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + kuangDiAndKuang[1]);

        //if (item == null)
        //{
        //    if (touXiangType == TouXiangType.TouXiang)
        //    {
        //        img_kuangDi.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.commonTouXiangKuangDi);

        //        img_kuang.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + ConstantVal.commonTouXiangKuang);
        //    }
        //}
        //else
        //{
        //    icon.ShowItemIcon(item);

        //}

        //icon.sprite = ResourceManager.Instance.GetObj<Sprite>(ConstantVal.ItemIconPath + icon);
        RefreshStatusShow();

        if (ItemManager.Instance.CheckIfHaveItemBySettingId(item.settingId))
        {
            obj_suo.gameObject.SetActive(false);
        }
        else
        {
            obj_suo.gameObject.SetActive(true);

        }
    }
    void RefreshStatusShow()
    {
        if (touXiangType == TouXiangType.TouXiang)
        {
            if (RoleManager.Instance.CheckIfHaveTouXiang(item.settingId))
                obj_choose.gameObject.SetActive(true);
            else
                obj_choose.gameObject.SetActive(false);
        }
        else
        {
            if (RoleManager.Instance.CheckIfHaveTouXiangKuang(item.settingId))
                obj_choose.gameObject.SetActive(true);
            else
                obj_choose.gameObject.SetActive(false);
        }
     
    }
    public void OnChoose(bool choose)
    {
        //obj_choose.SetActive(choose);
        RefreshStatusShow();
    }
    public override void Clear()
    {
        base.Clear();

    }
}
public enum TouXiangType
{
    None=0,
    TouXiang=1,
    TouXiangKuang=2,
}